using System;
using System.Management;
using System.Security;

namespace RDCore
{
    public abstract class BaseConfiguration
    {
        protected internal string _key = "";

        public virtual string Key
        {
            get
            {
                return this._key;
            }
            set
            {
                this._key = value;
            }
        }

        public virtual int MachineCode
        {
            get
            {
                return BaseConfiguration.getMachineCode();
            }
        }

        [SecuritySafeCritical]
        private static int getMachineCode()
        {
            methods methods = new methods();
            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("select * from Win32_Processor");
            string s = "";
            foreach (ManagementObject managementObject in managementObjectSearcher.Get())
                s += (string)managementObject.GetPropertyValue("ProcessorId");
            managementObjectSearcher.Query = new ObjectQuery("select * from Win32_BIOS");
            foreach (ManagementObject managementObject in managementObjectSearcher.Get())
                s += (string)managementObject.GetPropertyValue("SerialNumber");
            managementObjectSearcher.Query = new ObjectQuery("select * from Win32_BaseBoard");
            foreach (ManagementObject managementObject in managementObjectSearcher.Get())
                s += (string)managementObject.GetPropertyValue("SerialNumber");
            if (string.IsNullOrEmpty(s) | s == "00" | s.Length <= 3)
                s += BaseConfiguration.getHddSerialNumber();
            return methods.getEightByteHash(s, 100000);
        }

        [SecuritySafeCritical]
        private static string getHddSerialNumber()
        {
            ManagementObjectSearcher managementObjectSearcher1 = new ManagementObjectSearcher("\\root\\cimv2", "select * from Win32_DiskPartition WHERE BootPartition=True");
            uint num = 999;
            using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = managementObjectSearcher1.Get().GetEnumerator())
            {
                if (enumerator.MoveNext())
                    num = Convert.ToUInt32(enumerator.Current.GetPropertyValue("Index"));
            }
            if ((int)num == 999)
                return string.Empty;
            ManagementObjectSearcher managementObjectSearcher2 = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive where Index = " + num.ToString());
            string str1 = "";
            using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = managementObjectSearcher2.Get().GetEnumerator())
            {
                if (enumerator.MoveNext())
                    str1 = enumerator.Current.GetPropertyValue("Name").ToString();
            }
            if (string.IsNullOrEmpty(str1.Trim()))
                return string.Empty;
            if (str1.StartsWith("\\\\.\\"))
                str1 = str1.Replace("\\\\.\\", "%");
            ManagementObjectSearcher managementObjectSearcher3 = new ManagementObjectSearcher("SELECT * FROM Win32_PhysicalMedia WHERE Tag like '" + str1 + "'");
            string str2 = string.Empty;
            using (ManagementObjectCollection.ManagementObjectEnumerator enumerator = managementObjectSearcher3.Get().GetEnumerator())
            {
                if (enumerator.MoveNext())
                    str2 = enumerator.Current.GetPropertyValue("SerialNumber").ToString();
            }
            return str2;
        }
    }
}
