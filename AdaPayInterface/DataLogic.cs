using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security;
using System.IO;
using System.Configuration;
using System.Security.Cryptography;

namespace AdaPayInterface
{
    public class DataLogic
    {

        // Response logs
        #region
        public void ResponseLogs(string Msg)
        {
            try
            {
                string spath = ConfigurationManager.AppSettings["ResponseLogs"];
                if (Directory.Exists(spath) != true)
                {
                    Directory.CreateDirectory(spath);
                }
                string Mydate = Convert.ToString(DateTime.Now.ToString("ddMMyyyy"));
                string MyError = spath + "/" + Mydate;
                var fs = new FileStream(MyError, FileMode.Append);
                var wt = new StreamWriter(fs);
                wt.WriteLine(Msg);
                wt.Close();
            }

            catch (Exception ex)
            {
                ResponseLogs("Error in Function LogErrors " + ex);
            }
        }
        #endregion

        // Error logs
        #region
        public void ErrorLogs(string Msg)
        {
            try
            {
                string spath = ConfigurationManager.AppSettings["ErrorLogs"];
                if (Directory.Exists(spath) != true)
                {
                    Directory.CreateDirectory(spath);
                }
                string LogTime = (DateTime.Now.Hour + ":" + DateTime.Now.Minute).ToString() + " - ";
                string Mydate = Convert.ToString(DateTime.Now.ToString("ddMMyyyy"));
                string MyError = spath + "/" + Mydate;
                var fs = new FileStream(MyError, FileMode.Append);
                var wt = new StreamWriter(fs);
                wt.WriteLine(LogTime + Msg);
                wt.Close();
            }

            catch (Exception ex)
            {
                ResponseLogs("Error in Function LogErrors " + ex);
            }
        }
        #endregion

        // Encrypt Data
        #region
        public string Encrypt(string toEncrypt)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
            // Get the key from config file
            string key = "Syed Moshiur Murshed";
            //System.Windows.Forms.MessageBox.Show(key);
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            hashmd5.Clear();
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        #endregion
        public class Crypto
        {
            #region Class Constructor
            public Crypto()
            {
                string key = "Fintech";
                TripleDes.Key = TruncateHash(key, TripleDes.KeySize / 8);
                TripleDes.IV = TruncateHash("", TripleDes.BlockSize / 8);
            }
            #endregion

            #region SHA256 Hashing
            /// <summary>
            /// Computes a SHA256 hash code for the string parameter provided.
            /// </summary>
            /// <param name="Message">The string for which a SHA256 hash code is to be generated.</param>
            /// <returns>Returns a SHA256 hash code for the string parameter provided.</returns>
            public string EncryptSHA256Managed(string Message)
            {
                SHA256 sha256 = new SHA256Managed();
                byte[] sha256Bytes = Encoding.Default.GetBytes(Message);
                byte[] cryString = sha256.ComputeHash(sha256Bytes);
                string sha256Str = string.Empty;
                for (int i = 0; i < cryString.Length; i++)
                {
                    sha256Str += cryString[i].ToString("x");
                }
                return sha256Str;
            }
            #endregion

            #region TrippleDES Cryptography
            /// <summary>
            /// Performs a TrippleDES encryption on the provided string.
            /// </summary>
            /// <param name="Message">The string parameter for which a TrippleDES encryption will be returned.</param>
            /// <param name="Passphrase">The optional string key to be used in TrippleDES encryption.</param>
            /// <returns>Returns the TrippleDES representation of the provided string.</returns>
            public static string EncryptString(string Message)
            {
                try
                {
                    string Passphrase = "Fintech";
                    byte[] Results;
                    System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();

                    /* Step 1. We hash the passphrase using MD5 We use the MD5 hash generator as the result is a 128 bit byte array which is a valid 
                     * length for the TripleDES encoder we use below. */

                    MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
                    byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));

                    // Step 2. Create a new TripleDESCryptoServiceProvider object
                    TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

                    // Step 3. Setup the encoder
                    TDESAlgorithm.Key = TDESKey;
                    TDESAlgorithm.Mode = CipherMode.ECB;
                    TDESAlgorithm.Padding = PaddingMode.PKCS7;

                    // Step 4. Convert the input string to a byte[]
                    byte[] DataToEncrypt = UTF8.GetBytes(Message);

                    // Step 5. Attempt to encrypt the string
                    try
                    {
                        ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                        Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
                    }
                    finally
                    {
                        // Clear the TripleDes and Hashprovider services of any sensitive information
                        TDESAlgorithm.Clear();
                        HashProvider.Clear();
                    }

                    // Step 6. Return the encrypted string as a base64 encoded string
                    return Convert.ToBase64String(Results);
                }
                catch
                {
                    return "";
                }
            }

            /// <summary>
            /// Performs a TrippleDES decryption on the provided encrypted string.
            /// </summary>
            /// <param name="Message">The TrippleDES string parameter for which a decryption will be returned.</param>
            /// <param name="Passphrase">The optional string key to be used in TrippleDES decryption.</param>
            /// <returns>Returns the plain text of the TrippleDES string provided.</returns>
            public static string DecryptString(string Message)
            {
                try
                {
                    string Passphrase = "Fintech";
                    byte[] Results;
                    System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();

                    /* Step 1. We hash the passphrase using MD5 We use the MD5 hash generator as the result is a 128 bit byte array which is a valid 
                     * length for the TripleDES encoder we use below. */

                    MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
                    byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes(Passphrase));

                    // Step 2. Create a new TripleDESCryptoServiceProvider object
                    TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider();

                    // Step 3. Setup the decoder
                    TDESAlgorithm.Key = TDESKey;
                    TDESAlgorithm.Mode = CipherMode.ECB;
                    TDESAlgorithm.Padding = PaddingMode.PKCS7;

                    // Step 4. Convert the input string to a byte[]
                    byte[] DataToDecrypt = Convert.FromBase64String(Message);

                    // Step 5. Attempt to decrypt the string
                    try
                    {
                        ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                        Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
                    }
                    finally
                    {
                        // Clear the TripleDes and Hashprovider services of any sensitive information
                        TDESAlgorithm.Clear();
                        HashProvider.Clear();
                    }

                    // Step 6. Return the decrypted string in UTF8 format
                    return UTF8.GetString(Results);
                }
                catch
                {
                    return "";
                }
            }

            private TripleDESCryptoServiceProvider TripleDes = new TripleDESCryptoServiceProvider();
            private byte[] TruncateHash(string key, int length)
            {

                SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();

                // Hash the key.
                byte[] keyBytes = System.Text.Encoding.Unicode.GetBytes(key);
                byte[] hash = sha1.ComputeHash(keyBytes);

                // Truncate or pad the hash.
                Array.Resize(ref hash, length);
                return hash;
            }

            public string EncryptData(string Plaintext)
            {
                try
                {
                    // Convert the plaintext string to a byte array.
                    byte[] plaintextBytes = System.Text.Encoding.Unicode.GetBytes(Plaintext);

                    // Create the stream.
                    System.IO.MemoryStream ms = new System.IO.MemoryStream();
                    // Create the encoder to write to the stream.
                    CryptoStream encStream = new CryptoStream(ms, TripleDes.CreateEncryptor(), System.Security.Cryptography.CryptoStreamMode.Write);

                    // Use the crypto stream to write the byte array to the stream.
                    encStream.Write(plaintextBytes, 0, plaintextBytes.Length);
                    encStream.FlushFinalBlock();

                    // Convert the encrypted stream to a printable string.
                    return Convert.ToBase64String(ms.ToArray());

                }
                catch
                {
                    //errorLog(ex.Message, "99", "Functions", "TestConnection", Now(), "Encripting text")
                    return "";
                }
            }

            public string DecryptData(string encryptedtext)
            {

                // Convert the encrypted text string to a byte array.
                byte[] encryptedBytes = Convert.FromBase64String(encryptedtext);

                // Create the stream.
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                // Create the decoder to write to the stream.
                CryptoStream decStream = new CryptoStream(ms, TripleDes.CreateDecryptor(), System.Security.Cryptography.CryptoStreamMode.Write);

                // Use the crypto stream to write the byte array to the stream.
                decStream.Write(encryptedBytes, 0, encryptedBytes.Length);
                decStream.FlushFinalBlock();

                // Convert the plaintext stream to a string.
                return System.Text.Encoding.Unicode.GetString(ms.ToArray());
            }
            #endregion

            #region MD5 Hashing
            /// <summary>
            /// Computes an MD5 hash code for the string parameter provided.
            /// </summary>
            /// <param name="Message">The string for which an MD5 hash code is to be generated.</param>
            /// <returns>Returns an MD5 hash code for the string parameter provided.</returns>
            public static string MD5Hash(string Message)
            {
                // Create a new instance of the MD5CryptoServiceProvider object.
                MD5 md5Hasher = MD5.Create();

                // Convert the input string to a byte array and compute the hash.
                byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(Message));

                // Create a new Stringbuilder to collect the bytes and create a string.
                StringBuilder sBuilder = new StringBuilder();

                // Loop through each byte of the hashed data  and format each one as a hexadecimal string.
                for (int i = 0; i < data.Length; i++)
                    sBuilder.Append(data[i].ToString("x2"));
                return sBuilder.ToString();
            }

            /// <summary>
            /// Verifies a generated MD5 hash against the input.
            /// </summary>
            /// <param name="input">The input string.</param>
            /// <param name="hash">The input string's MD5 hash</param>
            /// <returns>Return true if the MD5 checks out, false otherwise.</returns>
            private static bool verifyMd5Hash(string input, string hash)
            {
                // Hash the input.
                string hashOfInput = MD5Hash(input);

                // Create a StringComparer an compare the hashes.
                StringComparer comparer = StringComparer.OrdinalIgnoreCase;
                if (0 == comparer.Compare(hashOfInput, hash))
                    return true;
                else
                    return false;
            }
            #endregion
        }
        // Decypt Data
        #region
        public string Decrypt(string cipherString)
        {
            byte[] keyArray;
            byte[] toEncryptArray = Convert.FromBase64String(cipherString);

            System.Configuration.AppSettingsReader settingsReader = new AppSettingsReader();
            //Get your key from config file to open the lock!
            string key = "Syed Moshiur Murshed";
            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            keyArray = hashmd5.ComputeHash(UTF8Encoding.UTF8.GetBytes(key));
            hashmd5.Clear();
            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            tdes.Clear();
            tdes.Clear();
            return UTF8Encoding.UTF8.GetString(resultArray);
        }
        #endregion

    }

    // Serialization

    #region
    public class QueryDataResponse
    {
        public List<ErrorDetails> ErrorData { get; set; }
    }
    #endregion

    #region
    public class ErrorDetails
    {
        public string Refno { get; set; }
        public int Product { get; set; }
        public string Instituition { get; set; }
        public string Branch { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorDesc { get; set; }
    }
    #endregion
}