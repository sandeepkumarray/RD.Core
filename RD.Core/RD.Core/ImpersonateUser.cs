using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Permissions;
using System.Security.Principal;
using System.Runtime.InteropServices;

namespace RDCore
{
    /// <summary>
    /// Impersonation
    /// usage
    /// -----
    /// using (new ImpersonateUser(domain, username, password))
    /// {
    ///    // You are the impersonated user inside here
    /// }
    /// </summary>
    public class ImpersonateUser : IDisposable
    {
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword,
                            int dwLogonType, int dwLogonProvider, ref IntPtr phToken);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public extern static bool CloseHandle(IntPtr handle);


        private static IntPtr tokenHandle = new IntPtr(0);
        private static WindowsImpersonationContext impersonatedUser;

        private string _domain = "";
        private string _username = "";
        private string _password = "";

        public ImpersonateUser(string Domain, string Username, string Password)
        {
            _domain = Domain;
            _username = Username;
            _password = Password;
            Impersonate(); //impersonating
        }

        /// <summary>
        /// Impersonate the current to the given username
        /// </summary>
        [PermissionSetAttribute(SecurityAction.Demand, Name = "FullTrust")]
        private void Impersonate()
        {
            //try
            {
                tokenHandle = IntPtr.Zero;

                // Use the unmanaged LogonUser function to get the user token for
                // the specified user, domain, and password.               

                // ---- Step - 1
                // Call LogonUser to obtain a handle to an access token.
                bool returnValue = LogonUser(this._username, this._domain, this._password,
                    (int)LogonSessionType.NewCredentials, (int)LogonProvider.Default,
                    ref tokenHandle); // tokenHandle - new security token

                if (false == returnValue)
                {
                    int ret = Marshal.GetLastWin32Error();
                    throw new System.ComponentModel.Win32Exception(ret);
                }

                // ---- Step - 2
                WindowsIdentity newId = new WindowsIdentity(tokenHandle);

                // ---- Step - 3
                impersonatedUser = newId.Impersonate();
            }
        }

        //Stops impersonation
        private void Undo()
        {
            impersonatedUser.Undo();
            // Free the tokens.
            if (tokenHandle != IntPtr.Zero)
                CloseHandle(tokenHandle);
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool Disposing)
        {
            if (Disposing)
            {
                _domain = null;
                _username = null;
                _password = null;
            }
            if (impersonatedUser != null)
                this.Undo();
            impersonatedUser = null;
        }

        ~ImpersonateUser()
        {
            Dispose(false);
        }

        #endregion
    }

    enum LogonSessionType : int
    {
        Interactive = 2,
        Network,
        Batch,
        Service,
        NetworkCleartext = 8,
        NewCredentials
    }

    enum LogonProvider : int
    {
        Default = 0, // default for platform (use this!)
        WinNT35,     // sends smoke signals to authority
        WinNT40,     // uses NTLM
        WinNT50      // negotiates Kerb or NTLM
    }
}
