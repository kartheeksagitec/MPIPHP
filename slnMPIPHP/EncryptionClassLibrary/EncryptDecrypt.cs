using System;
using System.Text;
using Microsoft.SqlServer.Server;
using System.Security.Cryptography;
using System.IO;
using System.Data.SqlTypes;

namespace EncryptionDecryptionClassLibrary
{
    public class EncryptDecrypt
    {
        [SqlProcedure]
        public static void OpusEncrypt(string astrText, out SqlString astrEncrypted)
        {
            astrEncrypted = string.Empty;
            string astrKey = "SagitecSolutionsPrivateLimited12"; string astrIV = "1234567890123456";
            try
            {
                     
            //utlTracing.TraceMethodEnter(utlConstants.istrTraceModule, utlConstants.istrTraceUser);  //_SFW_METHOD_TRACING_//

                if (string.IsNullOrEmpty(astrText))
                astrEncrypted= string.Empty;

            if (string.IsNullOrEmpty(astrKey))
                throw new ArgumentNullException("astrKey", "Parameter value is Null or Empty");
            if (string.IsNullOrEmpty(astrIV))
                throw new ArgumentNullException("astrIV", "Parameter value is Null or Empty");

            if (astrKey.Length != 32)
                throw new ArgumentException("Key value must be 32 characters", "astrKey");
            if (astrIV.Length != 16)
                throw new ArgumentException("IV value must be 16 characters", "astrIV");

            string lstrResult = string.Empty;

            using (AesManaged aesAlg = new AesManaged())
            {
                // Initialize the Key and IV
                aesAlg.KeySize = 256;
                aesAlg.Key = ASCIIEncoding.ASCII.GetBytes(astrKey);
                aesAlg.IV = ASCIIEncoding.ASCII.GetBytes(astrIV);

                // Create an encrytor to perform the stream transform
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for encryption
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            // Write all data to the stream
                            swEncrypt.Write(astrText);
                        }

                        byte[] cryptoByte = msEncrypt.ToArray();
                        lstrResult = Convert.ToBase64String(cryptoByte, 0, cryptoByte.GetLength(0));
                    }
                }
            }

            //utlTracing.TraceMethodExit(utlConstants.istrTraceModule, utlConstants.istrTraceUser);  //_SFW_METHOD_TRACING_//
            astrEncrypted = lstrResult;
            }
            catch (Exception ex)
            {
                astrEncrypted = string.Empty;
            }
           
        }


        [SqlProcedure]
        public static void  OpusDecrypt(string astrEncrypted, out SqlString astrText)
        {
            //utlTracing.TraceMethodEnter(utlConstants.istrTraceModule, utlConstants.istrTraceUser);  //_SFW_METHOD_TRACING_//
            string astrKey = "SagitecSolutionsPrivateLimited12"; string astrIV = "1234567890123456";

            try
            {
                if (string.IsNullOrEmpty(astrEncrypted))
                {
                    astrText = string.Empty;
                    return;
                }

                if (string.IsNullOrEmpty(astrKey))
                    throw new ArgumentNullException("astrKey", "Parameter value is Null or Empty");
                if (string.IsNullOrEmpty(astrIV))
                    throw new ArgumentNullException("astrIV", "Parameter value is Null or Empty");

                if (astrKey.Length != 32)
                    throw new ArgumentException("Key value must be 32 characters", "astrKey");
                if (astrIV.Length != 16)
                    throw new ArgumentException("IV value must be 16 characters", "astrIV");

                astrText = string.Empty;

                using (AesManaged aesAlg = new AesManaged())
                {
                    // Initialize the Key and IV
                    aesAlg.KeySize = 256;
                    aesAlg.Key = ASCIIEncoding.ASCII.GetBytes(astrKey);
                    aesAlg.IV = ASCIIEncoding.ASCII.GetBytes(astrIV);

                    // Convert from base 64 string to bytes
                    byte[] cryptoByte = Convert.FromBase64String(astrEncrypted);

                    // Create a decrytor to perform the stream transform
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    // Create the streams used for decryption.
                    using (MemoryStream msDecrypt = new MemoryStream(cryptoByte))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Read the decrypted bytes from the decrypting stream and place them in a string
                                astrText = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                astrText = string.Empty;
            }

            //utlTracing.TraceMethodExit(utlConstants.istrTraceModule, utlConstants.istrTraceUser);  //_SFW_METHOD_TRACING_//

        }


        [SqlProcedure]
        public static string OpusEncryptR(string astrText)
        {
            string astrEncrypted;

            astrEncrypted = string.Empty;
            string astrKey = "SagitecSolutionsPrivateLimited12"; string astrIV = "1234567890123456";
            try
            {

                //utlTracing.TraceMethodEnter(utlConstants.istrTraceModule, utlConstants.istrTraceUser);  //_SFW_METHOD_TRACING_//

                if (string.IsNullOrEmpty(astrText))
                    astrEncrypted = string.Empty;

                if (string.IsNullOrEmpty(astrKey))
                    throw new ArgumentNullException("astrKey", "Parameter value is Null or Empty");
                if (string.IsNullOrEmpty(astrIV))
                    throw new ArgumentNullException("astrIV", "Parameter value is Null or Empty");

                if (astrKey.Length != 32)
                    throw new ArgumentException("Key value must be 32 characters", "astrKey");
                if (astrIV.Length != 16)
                    throw new ArgumentException("IV value must be 16 characters", "astrIV");

                string lstrResult = string.Empty;

                using (AesManaged aesAlg = new AesManaged())
                {
                    // Initialize the Key and IV
                    aesAlg.KeySize = 256;
                    aesAlg.Key = ASCIIEncoding.ASCII.GetBytes(astrKey);
                    aesAlg.IV = ASCIIEncoding.ASCII.GetBytes(astrIV);

                    // Create an encrytor to perform the stream transform
                    ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                    // Create the streams used for encryption
                    using (MemoryStream msEncrypt = new MemoryStream())
                    {
                        using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                            {
                                // Write all data to the stream
                                swEncrypt.Write(astrText);
                            }

                            byte[] cryptoByte = msEncrypt.ToArray();
                            lstrResult = Convert.ToBase64String(cryptoByte, 0, cryptoByte.GetLength(0));
                        }
                    }
                }

                //utlTracing.TraceMethodExit(utlConstants.istrTraceModule, utlConstants.istrTraceUser);  //_SFW_METHOD_TRACING_//
                astrEncrypted = lstrResult;
                return astrEncrypted;
            }
            catch (Exception ex)
            {
                astrEncrypted = string.Empty;
                return astrEncrypted;
            }
             
        }


        [SqlProcedure]
        public static SqlDateTime OpusDecryptR(string astrEncrypted)
        {
            //utlTracing.TraceMethodEnter(utlConstants.istrTraceModule, utlConstants.istrTraceUser);  //_SFW_METHOD_TRACING_//
            string astrText;
            string astrKey = "SagitecSolutionsPrivateLimited12"; string astrIV = "1234567890123456";

            try
            {
                if (string.IsNullOrEmpty(astrEncrypted))
                {
                    astrText = string.Empty;
                    return System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                }

                if (string.IsNullOrEmpty(astrKey))
                    throw new ArgumentNullException("astrKey", "Parameter value is Null or Empty");
                if (string.IsNullOrEmpty(astrIV))
                    throw new ArgumentNullException("astrIV", "Parameter value is Null or Empty");

                if (astrKey.Length != 32)
                    throw new ArgumentException("Key value must be 32 characters", "astrKey");
                if (astrIV.Length != 16)
                    throw new ArgumentException("IV value must be 16 characters", "astrIV");

                astrText = string.Empty;

                using (AesManaged aesAlg = new AesManaged())
                {
                    // Initialize the Key and IV
                    aesAlg.KeySize = 256;
                    aesAlg.Key = ASCIIEncoding.ASCII.GetBytes(astrKey);
                    aesAlg.IV = ASCIIEncoding.ASCII.GetBytes(astrIV);

                    // Convert from base 64 string to bytes
                    byte[] cryptoByte = Convert.FromBase64String(astrEncrypted);

                    // Create a decrytor to perform the stream transform
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    // Create the streams used for decryption.
                    using (MemoryStream msDecrypt = new MemoryStream(cryptoByte))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Read the decrypted bytes from the decrypting stream and place them in a string
                                astrText = srDecrypt.ReadToEnd();
                                if (Convert.ToDateTime(astrText) == DateTime.MinValue)
                                {
                                    return System.Data.SqlTypes.SqlDateTime.MinValue.Value;
                                }
                                else
                                {
                                    return Convert.ToDateTime(astrText);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                astrText = string.Empty;
                return System.Data.SqlTypes.SqlDateTime.MinValue.Value;
            }

            //utlTracing.TraceMethodExit(utlConstants.istrTraceModule, utlConstants.istrTraceUser);  //_SFW_METHOD_TRACING_//
         
        }

        [SqlProcedure]
        public static string OpusDecryptString(string astrEncrypted)
        {
            //utlTracing.TraceMethodEnter(utlConstants.istrTraceModule, utlConstants.istrTraceUser);  //_SFW_METHOD_TRACING_//
            string astrText;
            string astrKey = "SagitecSolutionsPrivateLimited12"; string astrIV = "1234567890123456";

            try
            {
                if (string.IsNullOrEmpty(astrEncrypted))
                {
                    astrText = string.Empty;
                    return string.Empty;
                }

                if (string.IsNullOrEmpty(astrKey))
                    throw new ArgumentNullException("astrKey", "Parameter value is Null or Empty");
                if (string.IsNullOrEmpty(astrIV))
                    throw new ArgumentNullException("astrIV", "Parameter value is Null or Empty");

                if (astrKey.Length != 32)
                    throw new ArgumentException("Key value must be 32 characters", "astrKey");
                if (astrIV.Length != 16)
                    throw new ArgumentException("IV value must be 16 characters", "astrIV");

                astrText = string.Empty;

                using (AesManaged aesAlg = new AesManaged())
                {
                    // Initialize the Key and IV
                    aesAlg.KeySize = 256;
                    aesAlg.Key = ASCIIEncoding.ASCII.GetBytes(astrKey);
                    aesAlg.IV = ASCIIEncoding.ASCII.GetBytes(astrIV);

                    // Convert from base 64 string to bytes
                    byte[] cryptoByte = Convert.FromBase64String(astrEncrypted);

                    // Create a decrytor to perform the stream transform
                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    // Create the streams used for decryption.
                    using (MemoryStream msDecrypt = new MemoryStream(cryptoByte))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Read the decrypted bytes from the decrypting stream and place them in a string
                                astrText = srDecrypt.ReadToEnd();
                                if (astrText == string.Empty)
                                {
                                    return string.Empty;
                                }
                                else
                                {
                                    return astrText;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                astrText = string.Empty;
                return string.Empty;
            }

            //utlTracing.TraceMethodExit(utlConstants.istrTraceModule, utlConstants.istrTraceUser);  //_SFW_METHOD_TRACING_//

        }
        
    }
}
