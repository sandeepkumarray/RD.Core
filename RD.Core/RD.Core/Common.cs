using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using Microsoft.Win32;
using System.Diagnostics;
using System.ComponentModel;
using System.Web.Script.Serialization;
using System.Windows.Media;

namespace RDCore
{
    /// <summary>
    /// Has a set of commonly used methods.
    /// </summary>
    public static class Common
    {
        #region Bytes To File
        /// <summary>
        /// Converts the bynary stream to file
        /// </summary>
        /// <param name="folderPath"></param>
        /// <param name="physicalFileName"></param>
        /// <param name="documentStream"></param>
        /// <returns></returns>
        public static bool BytesToFile(string folderPath, string physicalFileName, byte[] documentStream)
        {
            try
            {
                //folderPath = folderPath.Replace(System.Windows.Forms.Application.StartupPath, System.IO.Path.Combine(System.Windows.Forms.Application.StartupPath, "Temp"));
                if (!System.IO.Directory.Exists(folderPath))
                    System.IO.Directory.CreateDirectory(folderPath);

                physicalFileName = System.IO.Path.Combine(folderPath, physicalFileName);
                System.IO.FileInfo fi = new System.IO.FileInfo(physicalFileName);

                if (fi.Exists)
                {
                    string newFiletName = string.Empty;
                    for (int index = 1; fi.Exists; index++)
                    {
                        newFiletName = string.Format("{0}_{1}{2}", System.IO.Path.GetFileNameWithoutExtension(physicalFileName), index, System.IO.Path.GetExtension(physicalFileName));
                        fi = new FileInfo(Path.Combine(folderPath, newFiletName));
                    }
                    physicalFileName = Path.Combine(folderPath, newFiletName);
                }

                using (BinaryWriter binWriter = new BinaryWriter(File.Open(fi.FullName, FileMode.Create)))
                {
                    binWriter.Write(documentStream);
                }

                fi = null;
                return true;
            }
            catch (Exception e)
            {
                Logger.Write(e.Message, MessageType.Error, typeof(Common));
                return false;
            }
            finally
            {
            }
        }
        #endregion

        #region Generatehash512
        /// <summary>
        /// Generate Hash value from given string value
        /// </summary>
        /// <param name="text"></param>
        /// <returns>A managed SHA 512 hash value</returns>
        public static string Generatehash512(string text)
        {
            byte[] message = Encoding.UTF8.GetBytes(text);

            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] hashValue;
            SHA512Managed hashString = new SHA512Managed();
            string hex = "";
            hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;
        }
        #endregion

        #region Generatehash256
        /// <summary>
        /// Generate Hash value from given string value
        /// </summary>
        /// <param name="text"></param>
        /// <returns>A managed SHA 256 hash value</returns>
        public static string Generatehash256(string text)
        {
            byte[] message = Encoding.UTF8.GetBytes(text);

            UnicodeEncoding UE = new UnicodeEncoding();
            byte[] hashValue;
            SHA256Managed hashString = new SHA256Managed();
            string hex = "";
            hashValue = hashString.ComputeHash(message);
            foreach (byte x in hashValue)
            {
                hex += String.Format("{0:x2}", x);
            }
            return hex;
        }
        #endregion

        #region File To Bytes
        /// <summary>
        /// Converts the file to bynary stream
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static Byte[] FileToBytes(String filePath)
        {
            byte[] buffer;
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            try
            {
                int length = (int)fileStream.Length;
                buffer = new byte[length];
                int count;
                int sum = 0;
                while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                    sum += count;
            }
            finally
            {
                fileStream.Close();
            }
            return buffer;
        }
        #endregion

        #region Converters
        /// <summary>
        /// Convert byte array to string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string convertByteArrayToString(byte[] input)
        {
            System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();
            return enc.GetString(input);
        }

        /// <summary>
        /// Convert string to byte array
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] convertStringToByteArray(string str)
        {
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            return encoding.GetBytes(str);
        }

        /// <summary>
        /// Convert hex string to byte array
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static byte[] convertHexStringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        /// <summary>
        /// Convert byte array to string
        /// </summary>
        /// <param name="ba"></param>
        /// <returns></returns>
        public static string convertByteArrayToHexString(byte[] ba)
        {
            string hex = BitConverter.ToString(ba);
            return hex.Replace("-", "");
        }
        #endregion

        #region Registry Key get set
        /// <summary>
        /// Sets a registry key value from given key, value and location
        /// </summary>
        /// <param name="location"></param>
        /// <param name="keyName"></param>
        /// <param name="keyValue"></param>
        /// <returns>true if success and false if unsuccess</returns>
        public static bool RegistryKeySet(string location, string keyName, string keyValue)
        {
            bool result = false;
            try
            {
                using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(location, true))
                {
                    if (rk != null)
                    {
                        rk.SetValue(keyName, keyValue);
                        result = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message, MessageType.Error, typeof(Common));
                result = false;
            }
            return result;
        }

        /// <summary>
        /// Gets a registry key value from given key and location
        /// </summary>
        /// <param name="location"></param>
        /// <param name="keyName"></param>
        /// <returns>the registry key value</returns>
        public static object RegistryKeyGet(string location, string keyName)
        {
            object result = new object();
            try
            {
                using (RegistryKey rk = Registry.LocalMachine.OpenSubKey(location, true))
                {
                    if (rk != null)
                    {
                        result = rk.GetValue(keyName, new object());
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex.Message, MessageType.Error, typeof(Common));
                result = false;
            }
            return result;
        }
        #endregion

        #region Convert Bytes To Image
        /// <summary>
        /// Convert Bytes To Image
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static ImageSource ConvertBytesToImageSource(byte[] image)
        {
            if (image != null)
            {
                var source = new System.Windows.Media.Imaging.BitmapImage();
                source.BeginInit();
                source.StreamSource = new MemoryStream(image);
                source.EndInit();
                return source;
            }

            return null;
        }
        #endregion
    }

    public static class JSONHelper
    {
        public static string ToJSON(this object obj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            return serializer.Serialize(obj);
        }

        public static string ToJSON(this object obj, int recursionDepth)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            serializer.RecursionLimit = recursionDepth;
            return serializer.Serialize(obj);
        }

        public static T FromJSON<T>(this string dataString)
        {
            JavaScriptSerializer json_serializer = new JavaScriptSerializer();
            T json_Object = (T)json_serializer.DeserializeObject(dataString);
            return json_Object;
        }
    }
}