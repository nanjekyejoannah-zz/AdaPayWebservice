using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.Text;
using System.Data;
using System.Security.Cryptography;
using System.Configuration;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace AdaPayInterface
{
  
    public enum FCDBProducts
    {
        URA = 0,
        WATER = 1,
        ELECTRICITY = 2,
        KCCA = 3,
        UNIVERISTY = 4,
        PAYTV = 5,
        NSSF = 6
    }

    public enum DatabaseType
    {
        FINPAY=1,
        BRIDGE=2
    }

    public class Functions
    {
        string Bridge = ConfigurationManager.ConnectionStrings["Bridge"].ConnectionString;
        string Finpay = ConfigurationManager.ConnectionStrings["Finpay"].ConnectionString;
        SqlConnection con=new SqlConnection();
        SqlCommand cmd=new SqlCommand();
        SqlDataAdapter da=new SqlDataAdapter();
        SqlDataReader dr;

        public string GetFinpayParameter(string sql)
        {
            string result = "";
            con.ConnectionString = Finpay;
            try
            {
                con.Open();
                cmd = new SqlCommand(sql, con);
                dr = cmd.ExecuteReader();
                if(dr.Read() == true)
                    {
                      result = dr[0].ToString();
                    }
                    }
            catch(Exception ex)
            {
                result = ex.Message;
            }                 

            dr.Close();
            con.Close();
            return result;
        }
        public static string WriteSuccessResponseFromMakePayment(string PRN, int serviceid, string institution, string branch, string transactionnumber, string statusmessage)
        {

            string StrEmpty = "";
            string response = "";
            MemoryStream memory_stream = new MemoryStream();
            XmlTextWriter xml_text_writer = new XmlTextWriter(memory_stream, System.Text.Encoding.UTF8);
            xml_text_writer.Formatting = Formatting.Indented;
            xml_text_writer.Indentation = 4;
            xml_text_writer.WriteStartDocument(true);

            xml_text_writer.WriteStartElement("PAYMENT");

            xml_text_writer.WriteElementString("Ref_No", PRN);
            xml_text_writer.WriteElementString("Product", serviceid.ToString());
            if (institution != "")
            {
                xml_text_writer.WriteElementString("Institution", institution);
            }
            else
            {
                xml_text_writer.WriteElementString("Institution", StrEmpty);
            }
            if (branch != "")
            {
                xml_text_writer.WriteElementString("Branch", branch);
            }
            else
            {
                xml_text_writer.WriteElementString("Branch", StrEmpty);
            }

            xml_text_writer.WriteElementString("Transaction_Number", transactionnumber);
            xml_text_writer.WriteElementString("Status_Message", statusmessage);

            xml_text_writer.WriteEndElement();

            xml_text_writer.WriteEndDocument();
            xml_text_writer.Flush();
            StreamReader stream_reader = new StreamReader(memory_stream);
            memory_stream.Seek(0, SeekOrigin.Begin);
            response = stream_reader.ReadToEnd();
            xml_text_writer.Close();
            return response;
        }

        public static string WriteFailureResponseFromMakePayment(string PRN, int serviceid, string branch, string errorcode, string error, string institution = "")
        {

            string StrEmpty = "";
            string response = "";
            MemoryStream memory_stream = new MemoryStream();
            XmlTextWriter xml_text_writer = new XmlTextWriter(memory_stream, System.Text.Encoding.UTF8);
            xml_text_writer.Formatting = Formatting.Indented;
            xml_text_writer.Indentation = 4;
            xml_text_writer.WriteStartDocument(true);

            xml_text_writer.WriteStartElement("PAYMENT");

            xml_text_writer.WriteElementString("Ref_No", PRN);
            xml_text_writer.WriteElementString("Product", serviceid.ToString());
            if (institution != "")
            {
                xml_text_writer.WriteElementString("Institution", institution);
            }
            else
            {
                xml_text_writer.WriteElementString("Institution", StrEmpty);
            }
            if (branch != "")
            {
                xml_text_writer.WriteElementString("Branch", branch);
            }
            else
            {
                xml_text_writer.WriteElementString("Branch", StrEmpty);
            }

            xml_text_writer.WriteElementString("Error_Code", errorcode);
            xml_text_writer.WriteElementString("Error_Description", error);
            xml_text_writer.WriteEndElement();
            xml_text_writer.WriteEndDocument();
            xml_text_writer.Flush();
            StreamReader stream_reader = new StreamReader(memory_stream);
            memory_stream.Seek(0, SeekOrigin.Begin);
            response = stream_reader.ReadToEnd();
            xml_text_writer.Close();
            return response;
        }


        public bool ValidateCredentials(string appid,string passphrase)
        {
            con.ConnectionString = Finpay;
            bool result = false;
            try
            {
                con.Open();
                cmd = new SqlCommand("SELECT * FROM FINTERFACE WHERE APPID='" + appid + "'", con);
                dr = cmd.ExecuteReader();
                if (dr.Read() == true)
                {
                   if(dr["appid"].ToString() == appid & dr["passphrase"].ToString() == passphrase)
                    {
                        result= true;
                    }
                }
            }
            catch (Exception ex)
            {
                result= false;
            }
            return result;
        }

        #region Cipher Logic
        
        public string EncryptData(string CipherText)
        {
            string result = "";
            try
            {
                var aesAlg = NewRijndaelManaged(CipherText);
                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                var msEncrypt = new MemoryStream();
                using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                using (var swEncrypt = new StreamWriter(csEncrypt))
                {
                    swEncrypt.Write(result);
                }

                result= Convert.ToBase64String(msEncrypt.ToArray());
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return result;
        }

        public string DecryptData(string CipherText)
        {
            string result = "";
            try
            {
                var aesAlg = NewRijndaelManaged("salt");
                var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                var cipher = Convert.FromBase64String(CipherText);

                using (var msDecrypt = new MemoryStream(cipher))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            result = srDecrypt.ReadToEnd();
                        }
                    }
                }                
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return result;
        }

        public static bool IsBase64String(string base64String)
        {
            base64String = base64String.Trim();
            return (base64String.Length % 4 == 0) &&
                   Regex.IsMatch(base64String, @"^[a-zA-Z0-9\+/]*={0,3}$", RegexOptions.None);

        }

        private static RijndaelManaged NewRijndaelManaged(string salt)
        {
            if (salt == null) throw new ArgumentNullException("salt");
            var saltBytes = Encoding.ASCII.GetBytes(salt);
            var key = new Rfc2898DeriveBytes("560A18CD-6346-4CF0-A2E8-671F9B6B9EA9", saltBytes);

            var aesAlg = new RijndaelManaged();
            aesAlg.Key = key.GetBytes(aesAlg.KeySize / 8);
            aesAlg.IV = key.GetBytes(aesAlg.BlockSize / 8);

            return aesAlg;
        }
        #endregion

    }
    }
