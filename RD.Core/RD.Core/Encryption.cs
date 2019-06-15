using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace RDCore
{
    public static class Encryption
    {
        private const string ENCRYPTION_KEY = "RDUTILITIESINMAY";
        private const string ENCRYPTION_PASSWORD = "RDPASSWORD$@";

        /// <summary>
        /// Enables encryption of a string value
        /// </summary>
        /// <param name="input">The string input for encryption</param>
        /// <returns>Encrypted value</returns>
        public static string Encrypt(string input)
        {
            // Test data
            string data = input;
            byte[] utfdata = UTF8Encoding.UTF8.GetBytes(data);
            byte[] saltBytes = UTF8Encoding.UTF8.GetBytes(ENCRYPTION_KEY);

            // Our symmetric encryption algorithm
            AesManaged aes = new AesManaged();

            // We're using the PBKDF2 standard for password-based key generation
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(ENCRYPTION_PASSWORD, saltBytes);

            // Setting our parameters
            aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
            aes.KeySize = aes.LegalKeySizes[0].MaxSize;

            aes.Key = rfc.GetBytes(aes.KeySize / 8);
            aes.IV = rfc.GetBytes(aes.BlockSize / 8);

            // Encryption
            ICryptoTransform encryptTransf = aes.CreateEncryptor();

            // Output stream, can be also a FileStream
            MemoryStream encryptStream = new MemoryStream();
            CryptoStream encryptor = new CryptoStream(encryptStream, encryptTransf, CryptoStreamMode.Write);

            encryptor.Write(utfdata, 0, utfdata.Length);
            encryptor.Flush();
            encryptor.Close();

            // Showing our encrypted content
            byte[] encryptBytes = encryptStream.ToArray();
            //string encryptedString = UTF8Encoding.UTF8.GetString(encryptBytes, 0, encryptBytes.Length);
            string encryptedString = Convert.ToBase64String(encryptBytes);

            return encryptedString;
        }

        /// <summary>
        /// Enables decryption of a string value
        /// </summary>
        /// <param name="input">The string input for decryption</param>
        /// <returns>Decrypted value</returns>
        public static string Decrypt(string base64Input)
        {
            string decryptedString = null;
            try
            {
                if (base64Input == null) return null;
                byte[] encryptBytes = Convert.FromBase64String(base64Input);
                byte[] saltBytes = Encoding.UTF8.GetBytes(ENCRYPTION_KEY);

                // Our symmetric encryption algorithm
                AesManaged aes = new AesManaged();

                // We're using the PBKDF2 standard for password-based key generation
                Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(ENCRYPTION_PASSWORD, saltBytes);

                // Setting our parameters
                aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
                aes.KeySize = aes.LegalKeySizes[0].MaxSize;

                aes.Key = rfc.GetBytes(aes.KeySize / 8);
                aes.IV = rfc.GetBytes(aes.BlockSize / 8);

                // Now, decryption
                ICryptoTransform decryptTrans = aes.CreateDecryptor();

                // Output stream, can be also a FileStream
                MemoryStream decryptStream = new MemoryStream();
                CryptoStream decryptor = new CryptoStream(decryptStream, decryptTrans, CryptoStreamMode.Write);

                decryptor.Write(encryptBytes, 0, encryptBytes.Length);
                decryptor.Flush();
                decryptor.Close();

                // Showing our decrypted content
                byte[] decryptBytes = decryptStream.ToArray();
                decryptedString = UTF8Encoding.UTF8.GetString(decryptBytes, 0, decryptBytes.Length);
            }
            catch
            {
                decryptedString = null;
            }
            return decryptedString;
        }

        /// <summary>
        /// Enables encryption of a string value with given password.
        /// </summary>
        /// <param name="input">The string input for encryption</param>
        /// <param name="PASSWORD">The password for encryption</param>
        /// <returns>Encrypted value</returns>
        public static string Encrypt(string input, string PASSWORD)
        {
            // Test data
            string data = input;
            byte[] utfdata = UTF8Encoding.UTF8.GetBytes(data);
            byte[] saltBytes = UTF8Encoding.UTF8.GetBytes(ENCRYPTION_KEY);

            // Our symmetric encryption algorithm
            AesManaged aes = new AesManaged();

            // We're using the PBKDF2 standard for password-based key generation
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(PASSWORD, saltBytes);

            // Setting our parameters
            aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
            aes.KeySize = aes.LegalKeySizes[0].MaxSize;

            aes.Key = rfc.GetBytes(aes.KeySize / 8);
            aes.IV = rfc.GetBytes(aes.BlockSize / 8);

            // Encryption
            ICryptoTransform encryptTransf = aes.CreateEncryptor();

            // Output stream, can be also a FileStream
            MemoryStream encryptStream = new MemoryStream();
            CryptoStream encryptor = new CryptoStream(encryptStream, encryptTransf, CryptoStreamMode.Write);

            encryptor.Write(utfdata, 0, utfdata.Length);
            encryptor.Flush();
            encryptor.Close();

            // Showing our encrypted content
            byte[] encryptBytes = encryptStream.ToArray();
            //string encryptedString = UTF8Encoding.UTF8.GetString(encryptBytes, 0, encryptBytes.Length);
            string encryptedString = Convert.ToBase64String(encryptBytes);

            return encryptedString;
        }

        /// <summary>
        /// Enables decryption of a string value with given password.
        /// </summary>
        /// <param name="base64Input">The string input for decryption</param>
        /// <param name="PASSWORD">The password for decryption</param>
        /// <returns>Decrypted value</returns>
        public static string Decrypt(string base64Input, string PASSWORD)
        {
            string decryptedString = null;
            try
            {
                if (base64Input == null) return null;
                byte[] encryptBytes = Convert.FromBase64String(base64Input);
                byte[] saltBytes = Encoding.UTF8.GetBytes(ENCRYPTION_KEY);

                // Our symmetric encryption algorithm
                AesManaged aes = new AesManaged();

                // We're using the PBKDF2 standard for password-based key generation
                Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(PASSWORD, saltBytes);

                // Setting our parameters
                aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
                aes.KeySize = aes.LegalKeySizes[0].MaxSize;

                aes.Key = rfc.GetBytes(aes.KeySize / 8);
                aes.IV = rfc.GetBytes(aes.BlockSize / 8);

                // Now, decryption
                ICryptoTransform decryptTrans = aes.CreateDecryptor();

                // Output stream, can be also a FileStream
                MemoryStream decryptStream = new MemoryStream();
                CryptoStream decryptor = new CryptoStream(decryptStream, decryptTrans, CryptoStreamMode.Write);

                decryptor.Write(encryptBytes, 0, encryptBytes.Length);
                decryptor.Flush();
                decryptor.Close();

                // Showing our decrypted content
                byte[] decryptBytes = decryptStream.ToArray();
                decryptedString = UTF8Encoding.UTF8.GetString(decryptBytes, 0, decryptBytes.Length);
            }
            catch
            {
                decryptedString = null;
            }
            return decryptedString;
        }

        /// <summary>
        /// Enables encryption of a string value with given password and key.
        /// </summary>
        /// <param name="input">The string input for encryption</param>
        /// <param name="PASSWORD">The password for encryption</param>
        /// <param name="KEY">The key for encryption</param>
        /// <returns>Encrypted value</returns>
        public static string Encrypt(string input, string PASSWORD, string KEY)
        {
            // Test data
            string data = input;
            byte[] utfdata = UTF8Encoding.UTF8.GetBytes(data);
            byte[] saltBytes = UTF8Encoding.UTF8.GetBytes(KEY);

            // Our symmetric encryption algorithm
            AesManaged aes = new AesManaged();

            // We're using the PBKDF2 standard for password-based key generation
            Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(PASSWORD, saltBytes);

            // Setting our parameters
            aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
            aes.KeySize = aes.LegalKeySizes[0].MaxSize;

            aes.Key = rfc.GetBytes(aes.KeySize / 8);
            aes.IV = rfc.GetBytes(aes.BlockSize / 8);

            // Encryption
            ICryptoTransform encryptTransf = aes.CreateEncryptor();

            // Output stream, can be also a FileStream
            MemoryStream encryptStream = new MemoryStream();
            CryptoStream encryptor = new CryptoStream(encryptStream, encryptTransf, CryptoStreamMode.Write);

            encryptor.Write(utfdata, 0, utfdata.Length);
            encryptor.Flush();
            encryptor.Close();

            // Showing our encrypted content
            byte[] encryptBytes = encryptStream.ToArray();
            //string encryptedString = UTF8Encoding.UTF8.GetString(encryptBytes, 0, encryptBytes.Length);
            string encryptedString = Convert.ToBase64String(encryptBytes);

            return encryptedString;
        }

        /// <summary>
        /// Enables decryption of a string value with given password and key.
        /// </summary>
        /// <param name="base64Input">The string input for decryption</param>
        /// <param name="PASSWORD">The password for decryption</param>
        /// <param name="KEY">The key for decryption</param>
        /// <returns>Decrypted value</returns>
        public static string Decrypt(string base64Input, string PASSWORD, string KEY)
        {
            string decryptedString = null;
            try
            {
                if (base64Input == null) return null;
                byte[] encryptBytes = Convert.FromBase64String(base64Input);
                byte[] saltBytes = Encoding.UTF8.GetBytes(KEY);

                // Our symmetric encryption algorithm
                AesManaged aes = new AesManaged();

                // We're using the PBKDF2 standard for password-based key generation
                Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(PASSWORD, saltBytes);

                // Setting our parameters
                aes.BlockSize = aes.LegalBlockSizes[0].MaxSize;
                aes.KeySize = aes.LegalKeySizes[0].MaxSize;

                aes.Key = rfc.GetBytes(aes.KeySize / 8);
                aes.IV = rfc.GetBytes(aes.BlockSize / 8);

                // Now, decryption
                ICryptoTransform decryptTrans = aes.CreateDecryptor();

                // Output stream, can be also a FileStream
                MemoryStream decryptStream = new MemoryStream();
                CryptoStream decryptor = new CryptoStream(decryptStream, decryptTrans, CryptoStreamMode.Write);

                decryptor.Write(encryptBytes, 0, encryptBytes.Length);
                decryptor.Flush();
                decryptor.Close();

                // Showing our decrypted content
                byte[] decryptBytes = decryptStream.ToArray();
                decryptedString = UTF8Encoding.UTF8.GetString(decryptBytes, 0, decryptBytes.Length);
            }
            catch
            {
                decryptedString = null;
            }
            return decryptedString;
        }
                
        /// <summary>
        /// Call this function to encrypt the specified message using specified key
        /// </summary>
        /// <param name="message"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string encryptData(string message, string key)
        {
            if (message == null || message.Length <= 0)
                throw new ArgumentNullException("message");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            string encryptMessage;
            try
            {
                //Set the padding mode
                PaddingMode padding = PaddingMode.ISO10126;

                //Key Value in byte array
                byte[] keyBytes = Common.convertStringToByteArray(key);

                //Message in byte array
                byte[] mesageBytes = Common.convertStringToByteArray(message);

                //Get encrypt cipher			
                RijndaelManaged cipher = getAESECBCipher(keyBytes, padding);

                //Encrypt data
                ICryptoTransform encryptor = cipher.CreateEncryptor();
                MemoryStream msEncrypt = new MemoryStream();
                CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
                csEncrypt.Write(mesageBytes, 0, mesageBytes.Length);
                csEncrypt.Close();
                byte[] encryptData = msEncrypt.ToArray();
                encryptMessage = Common.convertByteArrayToHexString(encryptData);
            }
            catch (Exception e)
            {
                throw new Exception("Exception in encrypt data:", e);
            }
            return encryptMessage;
        }

        /// <summary>
        /// Call this function to decrypt the specified message using specified key
        /// </summary>
        /// <param name="message"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string decryptData(string message, string key)
        {
            if (message == null || message.Length <= 0)
                throw new ArgumentNullException("message");
            if (key == null || key.Length <= 0)
                throw new ArgumentNullException("key");
            string decryptMessage;
            try
            {
                //Set the padding mode
                PaddingMode padding = PaddingMode.ISO10126;

                //Key Value in byte array (From HEX String)
                byte[] keyBytes = Common.convertStringToByteArray(key);

                //Message in byte array (From HEX String)
                byte[] messageBytes = Common.convertHexStringToByteArray(message);

                //Get decrypt cipher			
                RijndaelManaged cipher = getAESECBCipher(keyBytes, padding);

                //Decrypt data
                ICryptoTransform decryptor = cipher.CreateDecryptor();
                MemoryStream msDecrypt = new MemoryStream(messageBytes);
                CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
                byte[] orginalMessage = new byte[messageBytes.Length];
                csDecrypt.Read(orginalMessage, 0, orginalMessage.Length);
                decryptMessage = Common.convertByteArrayToString(orginalMessage);
            }
            catch (Exception e)
            {
                throw new Exception("Exception in decrypt data:", e);
            }
            return decryptMessage;
        }

        ///<summary>
        /// Encrypts a file using Rijndael algorithm.
        ///</summary>
        ///<param name="inputFile"></param>
        ///<param name="outputFile"></param>
        public static void EncryptFile(string inputFile, string outputFile, string Key)
        {
            try
            {
                string password = @Key; // Your Key Here
                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);

                string cryptFile = outputFile;
                FileStream fsCrypt = new FileStream(cryptFile, FileMode.Create);

                RijndaelManaged RMCrypto = new RijndaelManaged();

                CryptoStream cs = new CryptoStream(fsCrypt,
                    RMCrypto.CreateEncryptor(key, key),
                    CryptoStreamMode.Write);

                FileStream fsIn = new FileStream(inputFile, FileMode.Open);

                int data;
                while ((data = fsIn.ReadByte()) != -1)
                    cs.WriteByte((byte)data);


                fsIn.Close();
                cs.Close();
                fsCrypt.Close();
            }
            catch
            {
            }
        }

        ///<summary>
        /// Decrypts a file using Rijndael algorithm.
        ///</summary>
        ///<param name="inputFile"></param>
        ///<param name="outputFile"></param>
        public static void DecryptFile(string inputFile, string outputFile, string Key)
        {
            {
                string password = @Key; // Your Key Here

                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);

                FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);

                RijndaelManaged RMCrypto = new RijndaelManaged();

                CryptoStream cs = new CryptoStream(fsCrypt,
                    RMCrypto.CreateDecryptor(key, key),
                    CryptoStreamMode.Read);

                FileStream fsOut = new FileStream(outputFile.ToLower().Contains(".rdenc") ? (outputFile.ToLower().Replace(".rdenc", string.Empty)) : outputFile, FileMode.Create);

                int data;
                while ((data = cs.ReadByte()) != -1)
                    fsOut.WriteByte((byte)data);

                fsOut.Close();
                cs.Close();
                fsCrypt.Close();
            }
        }

        public static RijndaelManaged getAESECBCipher(byte[] keyBytes, PaddingMode padding)
        {
            RijndaelManaged cipher = new RijndaelManaged();
            cipher.KeySize = 128;
            cipher.BlockSize = 128;
            cipher.Mode = CipherMode.ECB;
            cipher.Padding = padding;
            cipher.Key = keyBytes;
            return cipher;
        }

    }
}
