using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Web;

namespace RDCore
{
    public static class RemotePost
    {
        public static string URL = "";
        public static string Method = "Post";
        public static string FormName = "MyForm";
        private static NameValueCollection _inputs;
        static RemotePost()
        {
            _inputs = new NameValueCollection();
        }

        /// <summary>
        ///  Adding Hidden fields to the Page   
        /// </summary>
        /// <param name="paramName">Name of the Hidden Field</param>
        /// <param name="paramValue">Value of the Hidden Field</param>
        public static void AddHiddenField(string paramName, string paramValue)
        {
            _inputs.Add(paramName, paramValue);
        }
        /// <summary>
        /// It will redirect page to URL provided by you 
        /// </summary>
        public static void Post()
        {

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Write("<html><head>");
            HttpContext.Current.Response.Write(String.Format("</head><body onload=document.{0}.submit()>", FormName));
            HttpContext.Current.Response.Write(String.Format("<form name={0} method={1} action={2}>", FormName, Method, URL));
            int length = _inputs.Keys.Count;

            for (int i = 0; i < length; i++)
                HttpContext.Current.Response.Write(String.Format("<input name=\"{0}\" type=hidden value=\"{1}\">", _inputs.Keys[i], _inputs.Get(i)));

            HttpContext.Current.Response.Write("</form>");
            HttpContext.Current.Response.Write("</body></html>");
            HttpContext.Current.Response.End();
        }
    }
}
