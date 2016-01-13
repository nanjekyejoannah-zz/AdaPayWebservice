using System;
using System.IO;
using System.Data;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using FinpayiAdmin.FinpayiAdmin;
using eTaxPmtLibrary;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Web.Services;
using System.Threading;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using System.Web.Configuration;

namespace AdaPayInterface
{
    public class InterfaceUra
    {
        Functions fn=new Functions();
        //Crypto cp = new Crypto();
        string EnctryptionAlgorithm = "rijndael";
        FinpayiSecurity.CryptoFactory CryptographyFactory = new FinpayiSecurity.CryptoFactory();
        FinpayiSecurity.ICrypto CrypTool = null;
        //FinpayiSecurity.ICrypto Cryptographer = CryptographyFactory.MakeCryptographer(EnctryptionAlgorith);

        string Finpayi = ConfigurationManager.ConnectionStrings["Finpay"].ConnectionString;
        string Bridgei = ConfigurationManager.ConnectionStrings["Bridge"].ConnectionString;
        X509Certificate2 x509Cert_ura = new X509Certificate2(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["URACertificate"]));
        //X509Certificate2 x509Cert_bnk = new X509Certificate2(HttpContext.Current.Server.MapPath(System.Configuration.ConfigurationManager.AppSettings["BankCertificate"]), System.Configuration.ConfigurationManager.AppSettings["BankCertPass"]);
        
        string MyAPIUserName = "";
        string MyAPIPass = "";
        

        #region Pull Ura Details
        public string PullRegistrationDetails(string MyPRN)
        {
            try
            {
                // ---------------------- Validate the Prn -------------------------
                System.Net.ServicePointManager.CertificatePolicy = new MyPolicy();
                URAGateway.UraPmtService service = new URAGateway.UraPmtService();
                eTaxPmtLibrary.UraEncryption uraEnc = new eTaxPmtLibrary.UraEncryption();
                string EnctryptionAlgorith = "rijndael";
                FinpayiSecurity.CryptoFactory CryptographyFactory = new FinpayiSecurity.CryptoFactory();
                FinpayiSecurity.ICrypto Cryptographer = CryptographyFactory.MakeCryptographer(EnctryptionAlgorith);
                MyAPIPass = fn.GetFinpayParameter("Select ItemValue from Parameters where ItemKey = 'URAWEBPASS'");
                MyAPIUserName = fn.GetFinpayParameter("Select ItemValue from Parameters where ItemKey = 'URAWEBUSER'");
                string UraPassword = Cryptographer.Decrypt(MyAPIPass);
                string s = uraEnc.EncryptedData(UraPassword, x509Cert_ura);                 
                URAGateway.PaymentRegEntity d = service.GetPRNDetails(MyAPIUserName, s, System.Configuration.ConfigurationManager.AppSettings["BANK_SHORT_CODE"], MyPRN);

                // ---------------------- Return Xml Response -------------------------
                string PaymentDetails = "";
                MemoryStream XmlMs = new MemoryStream();
                XmlTextWriter XmlTxtWtr = new XmlTextWriter(XmlMs, System.Text.Encoding.UTF8);
                XmlTxtWtr.Formatting = Formatting.Indented;
                XmlTxtWtr.Indentation = 4;
                XmlTxtWtr.WriteStartDocument();
                XmlTxtWtr.WriteStartElement("URA PAYMENT DETAILS");
                XmlTxtWtr.WriteElementString("PRN", d.Prn);
                XmlTxtWtr.WriteElementString("TIN", d.Tin);
                XmlTxtWtr.WriteElementString("CUSTNAME",d.TaxpayerName);
                XmlTxtWtr.WriteElementString("AMOUNT", d.Amount);
                XmlTxtWtr.WriteElementString("STATUS", d.StatusCode);
                XmlTxtWtr.WriteElementString("EXPIRYDATE",d.ExpiryDt);
                XmlTxtWtr.WriteEndElement();
                XmlTxtWtr.WriteEndDocument();
                XmlTxtWtr.Flush();
                StreamReader strmrdr = new StreamReader(XmlMs);
                XmlMs.Seek(0, SeekOrigin.Begin);
                PaymentDetails = strmrdr.ReadToEnd();
                return PaymentDetails;          

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion

        public String FinpayURA(string CustPRN, string EXREF, string DRACCBRANCH,
                                         string DRACCOUNTNUMBER, string DRAMOUNT, string DRACCURR, string TXNDATE,
                                         string TXNTIME)
        {
            string TINc = "";
            string TaxPayernamec = "";
            string amountduec = "";
            string ExpDatec = "";
            string RegDatec = "";
            string Remarkc = "";

            string PRNResponse = "";
            string OriginalPRN = "";
            string FCDBSetings = "";

            string FCDBINTERNALGL = "";
            string FCDBTELLERCODE = "";
            string FCDBDESC = "";
            string FCDBTILLID = "";
            string FCDBLIMIT = "";
            string FCDBMAKERy = "";
            string FCDBCHECKERy = "";
            string Subhosty = "";
            string Msgtypey = "";
            string Procodey = "";
            string Commissiony = "";
            string Trantypey = "";
            string Terminaly = "";
            string Batchy = "";
            string MessageFlagy = "";
            string Workstationy = "";
            string PosConfirmedy = "";
            string PosReversedy = "";
            string EftSourcey = "";
            string IbTxny = "";
            string Flexuseridy = "";
            string UtilityAccountY = "";
            string UtilityAccBranchY = "";
            string UtilityAccCurrencyY = "";
            string MerchantTypeY = "";

            string statuscodex = "";

            string[] FCDBSetting = null;

            bool isrepeat = false;
            bool istransacted = false;
            bool issuccess = false;
            bool isexisting = false;

            OriginalPRN = CustPRN;

            var FailureDetailsinfo = new URAFCDBResponse();
            var TaxDetailsinfo = new URAFCDBResponse();

            InterfaceUra IU = new InterfaceUra();
             //string results_= IU.PullRegistrationDetails(CustPRN);
            //***** Lets Validate this before even bothering flex.

            try
            {
                // ---------------------- Validate the Prn -------------------------
                System.Net.ServicePointManager.CertificatePolicy = new MyPolicy();
                URAGateway.UraPmtService service = new URAGateway.UraPmtService();
                eTaxPmtLibrary.UraEncryption uraEnc = new eTaxPmtLibrary.UraEncryption();
                string EnctryptionAlgorith = "rijndael";
                FinpayiSecurity.CryptoFactory CryptographyFactory = new FinpayiSecurity.CryptoFactory();
                FinpayiSecurity.ICrypto Cryptographer = CryptographyFactory.MakeCryptographer(EnctryptionAlgorith);
                MyAPIPass = fn.GetFinpayParameter("Select ItemValue from Parameters where ItemKey = 'URAWEBPASS'");
                MyAPIUserName = fn.GetFinpayParameter("Select ItemValue from Parameters where ItemKey = 'URAWEBUSER'");
                string UraPassword = Cryptographer.Decrypt(MyAPIPass);
                string s = uraEnc.EncryptedData(UraPassword, x509Cert_ura);
                URAGateway.PaymentRegEntity res = service.GetPRNDetails(MyAPIUserName, s, System.Configuration.ConfigurationManager.AppSettings["BANK_SHORT_CODE"], CustPRN);

                string remark = "";
                string TaxPayername = res.TaxpayerName;
                string amountdue = res.Amount;
                string TIN = res.Tin;
                string ExpDate = res.ExpiryDt;
                string RegDate = res.PaymentRegDt;
                string statuscode = res.StatusCode;
                if (amountdue != DRAMOUNT)
                {
                    return "UNSUCCESSFUL - INVALID AMOUNT " + DRAMOUNT + " INSTEAD OF " + amountdue;
                }
                Boolean auth = false;

                switch (statuscode)
                {
                    case "A":
                        //auth = true;
                        //remark = "Awaiting To Transact : " + res.StatusDesc;
                        break;
                    case "C":
                        //auth = false;
                        return   "Registration has been cancelled by URA! : " + res.StatusDesc;
                        //break;
                    case "X":
                        //auth = false;
                        return "Registration has expired! : " + res.StatusDesc;
                        //break;
                    case "T":
                        //auth = false;
                        return "Registration has already been transacted! : " + res.StatusDesc;
                        //break;
                    default:
                        //auth = false;
                        return "Results Unknown! : " + res.StatusDesc;
                        //break;
                }
                Datamanagement.PushRegistrationDetails(CustPRN);
                Datamanagement.UpdateURARegDetails(CustPRN, TaxPayername, amountdue, TIN, ExpDate, RegDate, remark, auth, statuscode);
                //urapt.Close();

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            
            //*****
           

            Thread.Sleep(5000);
            //Validate PRN here
            PRNResponse = Datamanagement.getPRNDetails(CustPRN);
            string[] prndets = null;

            prndets = PRNResponse.Split('|');

            string sp_reference = "";

            TINc = prndets[0];
            TaxPayernamec = prndets[1];
            amountduec = prndets[2];
            ExpDatec = prndets[3];
            RegDatec = prndets[4];
            Remarkc = prndets[5];
            statuscodex = prndets[6];

            switch (statuscodex)
            {
                case "A":
                    //Continue
                    break;
                default:
                    //Do Zilch
                    FailureDetailsinfo.EXRef = EXREF;
                    FailureDetailsinfo.PRNNUMBER = CustPRN;
                    FailureDetailsinfo.TaxAmount = DRAMOUNT;
                    FailureDetailsinfo.FailureDetails = new List<UraFailureDetials>();
                    FailureDetailsinfo.FailureDetails.Add(new UraFailureDetials()
                                                              {
                                                                  Remark = Remarkc,
                                                                  statuscode = statuscodex,
                                                                  Responcecode = "05"
                                                              });
                    return "UNSUCCESSFUL";
                    break;
            }

            //Get FCDB details here

            FCDBSetings = Datamanagement.getFCDBSettings("TAX");
            FCDBSetting = FCDBSetings.Split('|');

            FCDBINTERNALGL = FCDBSetting[0];
            FCDBTELLERCODE = FCDBSetting[1];
            FCDBDESC = FCDBSetting[2];
            FCDBTILLID = FCDBSetting[3];
            FCDBLIMIT = FCDBSetting[4];

            FCDBMAKERy = FCDBSetting[5];
            FCDBCHECKERy = FCDBSetting[6];
            Subhosty = FCDBSetting[7];
            Msgtypey = FCDBSetting[8];
            Procodey = FCDBSetting[9];
            Commissiony = FCDBSetting[10];
            Trantypey = FCDBSetting[11];
            Terminaly = FCDBSetting[12];
            Batchy = FCDBSetting[13];
            MessageFlagy = FCDBSetting[14];
            Workstationy = FCDBSetting[15];
            PosConfirmedy = FCDBSetting[16];
            PosReversedy = FCDBSetting[17];
            EftSourcey = FCDBSetting[18];
            IbTxny = FCDBSetting[19];
            Flexuseridy = FCDBSetting[20];
            UtilityAccountY = FCDBSetting[21];
            UtilityAccBranchY = FCDBSetting[22];
            UtilityAccCurrencyY = FCDBSetting[23];
            MerchantTypeY = FCDBSetting[24];


            //Check if already transacted
            istransacted = Datamanagement.isalreadyTransacted(CustPRN);
            switch (istransacted)
            {
                case true:
                    FailureDetailsinfo.EXRef = EXREF;
                    FailureDetailsinfo.PRNNUMBER = CustPRN;
                    FailureDetailsinfo.TaxAmount = DRAMOUNT;
                    FailureDetailsinfo.FailureDetails = new List<UraFailureDetials>();
                    FailureDetailsinfo.FailureDetails.Add(new UraFailureDetials()
                                                              {
                                                                  Remark = "Registration has already been transacted",
                                                                  Responcecode = "05"
                                                              });
                    return "UNSUCCESSFUL";
                    break;
                default:
                    break;
            }

            CustPRN = CustPRN + Trantypey;

            sp_reference = CustPRN + "-" + TaxPayernamec;

            isexisting = Datamanagement.isalreadyExisting(OriginalPRN);
            switch (isexisting)
            {
                case true:
                    //update source as fcdb
                    Datamanagement.UpdatePRNSource(OriginalPRN, "FCDB");
                    break;
                case false:
                    //insert into registration
                    Datamanagement.insertnewfcdbpayment(OriginalPRN, TINc, TaxPayernamec, amountduec, RegDatec, ExpDatec,
                                                        statuscodex, "FCDB");
                    break;
            }

            //update CLEAREDSTATUS here
            //Datamanagement.UpdateRegistration(CustPRN, "CLEARING CHEQUE");
            //check whether transaction already sent to the bridge
            isrepeat = Datamanagement.isalreadyPosted(CustPRN);
            if (isrepeat == false)
            {
                //post to the bridge
                Datamanagement.InsertIntoLog(CustPRN, Subhosty,
                                             Msgtypey, Procodey, TXNDATE, TXNTIME, DRACCBRANCH, CustPRN, CustPRN,
                                             DRAMOUNT, Commissiony, DRACCOUNTNUMBER, MerchantTypeY, EXREF, Terminaly,
                                             DRACCBRANCH,
                                             Batchy, Terminaly, sp_reference, DRACCOUNTNUMBER, DRACCURR, "0",
                                             MessageFlagy, Workstationy,
                                             PosConfirmedy, PosReversedy, "0", UtilityAccountY, UtilityAccBranchY,
                                             MerchantTypeY, EftSourcey, IbTxny, "", Flexuseridy);
            }

            Thread.Sleep(5000);
            issuccess = Datamanagement.isSuccessful(CustPRN);
            EXREF = Datamanagement.Ecternalref(CustPRN);
            Datamanagement.UpdateMSGIDResponded(CustPRN);
            switch (issuccess)
            {
                case true:

                    Datamanagement.UpdateRegistration(OriginalPRN, TXNDATE, FCDBTELLERCODE, DRACCBRANCH, "",
                                                      UtilityAccountY, DRACCOUNTNUMBER, "C", "T", EXREF, false,
                                                      FCDBCHECKERy, "11",
                                                      UtilityAccBranchY, "", Trantypey, DRACCURR, MerchantTypeY,
                                                      UtilityAccCurrencyY);
                    break;

                case false:
                    FailureDetailsinfo.EXRef = EXREF;
                    FailureDetailsinfo.PRNNUMBER = CustPRN;
                    FailureDetailsinfo.TaxAmount = DRAMOUNT;
                    FailureDetailsinfo.FailureDetails = new List<UraFailureDetials>();
                    FailureDetailsinfo.FailureDetails.Add(new UraFailureDetials()
                                                              {
                                                                  Remark = "Failed To Post To Core Banking",
                                                                  Responcecode = "05"
                                                              });
                    return "UNSUCCESSFUL";
                    break;
            }

            //Notify URA - If successfully posted
            Datamanagement.InsertIntoBridgeRegistration(OriginalPRN, "C", TINc, DRAMOUNT, TXNDATE, TXNDATE, "T",
                                                        DRACCBRANCH, CustPRN, "", "");

            //joannah.....9/7/2015
            Thread.Sleep(5000);
            //Notify URA here
             NotifyURA(OriginalPRN, "C", TINc, DRAMOUNT, TXNDATE, TXNDATE, "T",
                                                       DRACCBRANCH, CustPRN, "", "");

             var strUpdate = "Update REGISTRATIONS set Sent = 1, acknowledged = 1 where PRN = prn";

             try
             {
                 using (
                     var conn =
                         new SqlConnection( WebConfigurationManager.ConnectionStrings["Bridge"].ConnectionString))
                 {
                     conn.Open();
                     using (var cmd = new SqlCommand(strUpdate, conn))
                     {
                         var dr = cmd.ExecuteReader();
                     }
                 }
             }

             catch (Exception ex)
             {
                 
             }

             //joannah.....9/7/2015
            
             

            //RESPOND HERE

            TaxDetailsinfo.EXRef = EXREF;
            TaxDetailsinfo.PRNNUMBER = CustPRN;
            TaxDetailsinfo.TaxAmount = amountduec;
            TaxDetailsinfo.TAXDetails = new List<UraDetials>();
            TaxDetailsinfo.TAXDetails.Add(new UraDetials()
                                              {
                                                  amountdue = amountduec,
                                                  TaxPayername = TaxPayernamec,
                                                  TIN = TINc,
                                                  ExpDate = ExpDatec,
                                                  RegDate = RegDatec,
                                                  Remark = Remarkc,
                                                  Responcecode = "00"
                                              });

            return   TaxDetailsinfo.EXRef.ToString();
        }


        //======================================================================
        private bool IsMobileNumberValid(string mobileNumber)
        {
            // remove all non-numeric characters
            //Session["_mobileNumber"] = CleanNumber(mobileNumber);

            //// trim any leading zeros
            //Session["_mobileNumber"] = Session["_mobileNumber"].ToString().TrimStart(new char[] {'0'});

            //// check for this in case they've entered 44 (0)xxxxxxxxx or similar
            //if (Session["_mobileNumber"].ToString().StartsWith("2560"))
            //{
            //    Session["_mobileNumber"] = Session["_mobileNumber"].ToString().Remove(2, 1);
            //}

            //// add country code if they haven't entered it
            //if (!Session["_mobileNumber"].ToString().StartsWith("256"))
            //{
            //    Session["_mobileNumber"] = "256" + Session["_mobileNumber"].ToString();
            //}

            //// check if it's the right length
            //if (Session["_mobileNumber"].ToString().Length != 12)
            {
                return false;
            }

            return true;
        }
        //**********
        //public static string NotifyURA(string PRN, string BankStatus, string Tin, string Amount, string Paid_dt,
        //                              string Value_dt, string Status, string Bank_branch_cd, string Bank_tr_no,
        //                              string Chq_no, string Reason)
        //{
        //    var uraEnc = new UraEncryption();

        //    string pass_Api = Decrypt(MyAPIPass);

        //    string s = uraEnc.EncryptedData(MyAPIPass, x509Cert_ura);
        //    var urapt = new UraPmtServiceSoapClient("UraPmtServiceSoap");
        //    urapt.Open();

        //    var MyTransactionEntity = new TransactionEntity();
        //    var MyTranArray = new URAPmtInterface.TransactionEntity[1];
        //    MyTranArray.Initialize();

        //    MyTransactionEntity.Bank_cd = ConfigurationManager.AppSettings["BANK_SHORT_CODE"];
        //    MyTransactionEntity.Prn = PRN;
        //    MyTransactionEntity.Tin = Tin;
        //    MyTransactionEntity.Amount = Amount;
        //    MyTransactionEntity.Paid_dt = Paid_dt;
        //    MyTransactionEntity.Value_dt = Value_dt;
        //    MyTransactionEntity.Status = Status;
        //    MyTransactionEntity.Bank_branch_cd = Bank_branch_cd;
        //    MyTransactionEntity.Bank_tr_no = Bank_tr_no;
        //    MyTransactionEntity.Chq_no = Chq_no;
        //    MyTransactionEntity.Reason = Reason;

        //    string MyString = MyTransactionEntity.Bank_cd + MyTransactionEntity.Prn + MyTransactionEntity.Tin +
        //                      MyTransactionEntity.Amount + MyTransactionEntity.Paid_dt + MyTransactionEntity.Value_dt +
        //                      MyTransactionEntity.Status + MyTransactionEntity.Bank_branch_cd +
        //                      MyTransactionEntity.Bank_tr_no + MyTransactionEntity.Chq_no + MyTransactionEntity.Reason;


        //    var privateKey = x509Cert_bnk.PrivateKey as RSACryptoServiceProvider;
        //    byte[] buffer = new UnicodeEncoding().GetBytes(MyString);
        //    byte[] hash = new SHA1Managed().ComputeHash(buffer);
        //    if (privateKey != null)
        //    {
        //        byte[] signature = privateKey.SignHash(hash, "SHA1");

        //        //verify the signature
        //        var publicKey = x509Cert_bnk.PublicKey.Key as RSACryptoServiceProvider;
        //        bool verify = publicKey != null && publicKey.VerifyHash(hash, CryptoConfig.MapNameToOID("SHA1"), signature);

        //        MyTransactionEntity.Signature = signature;
        //    }

        //    //Add Transaction Entity to the Array
        //    MyTranArray[0] = MyTransactionEntity;

           // var res = urapt.NotifyUraPayment(MyAPIUserName, s, MyTranArray);

        //    return "";
        //}
        //**********

        //**********
        //static 

        public string NotifyURA(string PRN, string BankStatus, string Tin, string Amount, string Paid_dt,
                                      string Value_dt, string Status, string Bank_branch_cd, string Bank_tr_no,
                                   string Chq_no, string Reason)
       {
           Functions fn = new Functions();
           //Crypto cp = new Crypto();
           string EnctryptionAlgorithm = "rijndael";
           FinpayiSecurity.CryptoFactory CryptographyFactory = new FinpayiSecurity.CryptoFactory();
           FinpayiSecurity.ICrypto CrypTool = null;
           //FinpayiSecurity.ICrypto Cryptographer = CryptographyFactory.MakeCryptographer(EnctryptionAlgorith);

           string Finpayi = ConfigurationManager.ConnectionStrings["Finpay"].ConnectionString;
           string Bridgei = ConfigurationManager.ConnectionStrings["Bridge"].ConnectionString;
           X509Certificate2 x509Cert_ura = new X509Certificate2(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["URACertificate"]));
           X509Certificate2 x509Cert_bnk = new X509Certificate2(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BankCertificate"]), HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BankCertPass"]));

           string MyAPIUserName = "";
           string MyAPIPass = "";


           System.Net.ServicePointManager.CertificatePolicy = new MyPolicy();
           URAGateway.UraPmtService service = new URAGateway.UraPmtService();
           eTaxPmtLibrary.UraEncryption uraEnc = new eTaxPmtLibrary.UraEncryption();
           string EnctryptionAlgorith = "rijndael";
           //FinpayiSecurity.CryptoFactory CryptographyFactory = new FinpayiSecurity.CryptoFactory();
           FinpayiSecurity.ICrypto Cryptographer = CryptographyFactory.MakeCryptographer(EnctryptionAlgorith);
           MyAPIPass = fn.GetFinpayParameter("Select ItemValue from Parameters where ItemKey = 'URAWEBPASS'");
           MyAPIUserName = fn.GetFinpayParameter("Select ItemValue from Parameters where ItemKey = 'URAWEBUSER'");
           string UraPassword = Cryptographer.Decrypt(MyAPIPass);
           string s = uraEnc.EncryptedData(UraPassword, x509Cert_ura);



            
        //    var urapt = new UraPmtServiceSoapClient("UraPmtServiceSoap");
        //    urapt.Open();

          var MyTransactionEntity = new URAGateway.TransactionEntity ();
          var MyTranArray = new URAGateway.TransactionEntity[1];
          MyTranArray.Initialize();

          MyTransactionEntity.Bank_cd = ConfigurationManager.AppSettings["BANK_SHORT_CODE"];
          MyTransactionEntity.Prn = PRN;
          MyTransactionEntity.Tin = Tin;
          MyTransactionEntity.Amount = Amount;
          MyTransactionEntity.Paid_dt = Paid_dt;
          MyTransactionEntity.Value_dt = Value_dt;
          MyTransactionEntity.Status = Status;
          MyTransactionEntity.Bank_branch_cd = Bank_branch_cd;
          MyTransactionEntity.Bank_tr_no = Bank_tr_no;
          MyTransactionEntity.Chq_no = Chq_no;
          MyTransactionEntity.Reason = Reason;

          string MyString = MyTransactionEntity.Bank_cd + MyTransactionEntity.Prn + MyTransactionEntity.Tin +
                            MyTransactionEntity.Amount + MyTransactionEntity.Paid_dt + MyTransactionEntity.Value_dt +
                            MyTransactionEntity.Status + MyTransactionEntity.Bank_branch_cd +
                            MyTransactionEntity.Bank_tr_no + MyTransactionEntity.Chq_no + MyTransactionEntity.Reason;


          var privateKey = x509Cert_bnk.PrivateKey as RSACryptoServiceProvider;
          byte[] buffer = new UnicodeEncoding().GetBytes(MyString);
          byte[] hash = new SHA1Managed().ComputeHash(buffer);
          if (privateKey != null)
          {
              byte[] signature = privateKey.SignHash(hash, "SHA1");

              //verify the signature
              var publicKey = x509Cert_bnk.PublicKey.Key as RSACryptoServiceProvider;
              bool verify = publicKey != null && publicKey.VerifyHash(hash, CryptoConfig.MapNameToOID("SHA1"), signature);

              MyTransactionEntity.Signature = signature;
          }

          //Add Transaction Entity to the Array
          MyTranArray[0] = MyTransactionEntity;

          var res = service.NotifyUraPayment(MyAPIUserName, s, MyTranArray);

          return "";
        }

        //


        //======================================================================
        private string CleanNumber(string phone)
        {
            var digitsOnly = new Regex(@"[^\d]");
            return digitsOnly.Replace(phone, "");
        }

        //======================================================================
        public static string GetURADetails(string PRN, string Area)
        {
            var DM = new Datamanagement();
            return "";
        }

        //======================================================================
    }
     //===================TAX PAYEMENT =============================
    public class URAFCDBResponse
    {
        public string EXRef { get; set; }
        public string PRNNUMBER { get; set; }
        public string TaxAmount { get; set; }
        public List<UraDetials> TAXDetails { get; set; }
        public List<UraFailureDetials> FailureDetails { get; set; }
    }

    public class UraDetials
    {
        public string TIN { get; set; }
        public string TaxPayername { get; set; }
        public string amountdue { get; set; }
        public string ExpDate { get; set; }
        public string RegDate { get; set; }
        public string Remark { get; set; }
        public string statuscode { get; set; }
        public string Responcecode { get; set; }
    }

    public class UraFailureDetials
    {
        public string Remark { get; set; }
        public string statuscode { get; set; }
        public string Responcecode { get; set; }
    }


        //public object NotifyURAPayment(string MyPRN, string MyBankStatus, string MyNotifyNature = "NORMAL", int MyCode = 1)
        //{

        //    try
        //    {
        //        URAInterface.UraPmtService service = new URAInterface.UraPmtService();

        //        eTaxPmtLibrary.UraEncryption uraEnc = new eTaxPmtLibrary.UraEncryption();

        //        string s = uraEnc.EncryptedData(MyAPIPass, x509Cert_ura);

        //        //Declare an array of Transaction Entity
        //        URAInterface.TransactionEntity[] MyTranArray = new URAInterface.TransactionEntity[1];

        //        //Declare an item of Transaction Entity
        //        URAInterface.TransactionEntity MyTransactionEntity = new URAInterface.TransactionEntity();

        //        DataTable  MyDataTable = new DataTable();
        //        if (MyNotifyNature == "NORMAL")
        //        {
        //            MyDataTable = dat.DBAction("SELECT * from REGISTRATIONS WHERE PRN = '" + MyPRN + "'", DataManagement.DBActionType.DataTable);
        //        }
        //        else
        //        {
        //            MyDataTable = dat.DBAction("SELECT * FROM TRANSACTED_REGISTRATIONS WHERE PRN = '" + MyPRN + "'", DataManagement.DBActionType.DataTable);
        //        }


        //        if (MyDataTable.Rows.Count > 0)
        //        {
        //            MyTranArray.Initialize();

        //            MyTransactionEntity.Bank_cd = System.Configuration.ConfigurationManager.AppSettings("BANK_SHORT_CODE").ToString;
        //            //BANK CODE
        //            MyTransactionEntity.Prn = uraEnc.EncryptedData(MyPRN, x509Cert_ura);
        //            //PRN
        //            MyTransactionEntity.Tin = uraEnc.EncryptedData(MyDataTable.Rows(0).Item("TIN"), x509Cert_ura);
        //            //TIN
        //            MyTransactionEntity.Amount = uraEnc.EncryptedData(MyDataTable.Rows(0).Item("AMOUNT"), x509Cert_ura);
        //            //Amount
        //            MyTransactionEntity.Paid_dt = Strings.Format(MyDataTable.Rows(0).Item("PAYMENTDATE"), "dd-MMM-yy");
        //            // Format(dat.DBAction("SELECT PROCESSING_DATE FROM DYNAMIC_DATA", DataManagement.DBActionType.Scalar), "dd-MMM-yy") ' Format(dat.DBAction("SELECT PROCESSING_DATE FROM DYNAMIC_DATA", DataManagement.DBActionType.Scalar), "dd-MMM-yy") 'Payment Date
        //            MyTransactionEntity.Value_dt = Strings.Format(MyDataTable.Rows(0).Item("VALUEDATE"), "dd-MMM-yy");
        //            //Value Date
        //            if (MyNotifyNature == "NORMAL")
        //            {
        //                MyTransactionEntity.Status = MyBankStatus;
        //                //Status
        //            }
        //            else if (MyNotifyNature == "CLEARED")
        //            {
        //                MyTransactionEntity.Status = "C";
        //                //Status
        //            }
        //            else if (MyNotifyNature == "BOUNCED")
        //            {
        //                MyTransactionEntity.Status = "D";
        //                //Status
        //            }
        //            MyTransactionEntity.Bank_branch_cd = dat.DBAction("SELECT BOUBRANCH FROM BRANCHES WHERE SOLID = '" + HttpContext.Current.Session["Branch"] + "'", DataManagement.DBActionType.Scalar);
        //            //Bank Branch Code
        //            MyTransactionEntity.Bank_tr_no = MyDataTable.Rows(0).Item("TRAN_NUMBER");
        //            //Bank Transaction Number
        //            MyTransactionEntity.Chq_no = MyDataTable.Rows(0).Item("CHEQUENO");
        //            //Bank Cheque Number
        //            if (MyNotifyNature == "NORMAL")
        //            {
        //                MyTransactionEntity.Reason = "";
        //                //Reason
        //            }
        //            else if (MyNotifyNature == "CLEARED")
        //            {
        //                MyTransactionEntity.Reason = "";
        //                //Reason
        //            }
        //            else if (MyNotifyNature == "BOUNCED")
        //            {
        //                MyTransactionEntity.Reason = MyCode;
        //                //Reason
        //            }
        //            string MyString = MyTransactionEntity.Bank_cd + MyTransactionEntity.Prn + MyTransactionEntity.Tin + MyTransactionEntity.Amount + MyTransactionEntity.Paid_dt + MyTransactionEntity.Value_dt + MyTransactionEntity.Status + MyTransactionEntity.Bank_branch_cd + MyTransactionEntity.Bank_tr_no + MyTransactionEntity.Chq_no + MyTransactionEntity.Reason;

        //            //sign the data
        //            RSACryptoServiceProvider privateKey = x509Cert_bnk.PrivateKey as RSACryptoServiceProvider;
        //            byte[] buffer = new UnicodeEncoding().GetBytes(MyString);
        //            //Encoding.[Default].GetBytes(MyString)
        //            byte[] hash = new SHA1Managed().ComputeHash(buffer);
        //            byte[] signature = privateKey.SignHash(hash, "SHA1");
        //            //privateKey.SignData(buffer, New SHA1Managed())     

        //            //verify the signature
        //            RSACryptoServiceProvider publicKey = x509Cert_bnk.PublicKey.Key as RSACryptoServiceProvider;
        //            bool verify = publicKey.VerifyHash(hash, CryptoConfig.MapNameToOID("SHA1"), signature);
        //            //publicKey.VerifyData(buffer, New SHA1Managed(), signature)

        //            MyTransactionEntity.Signature = signature;

        //            //Add Transaction Entity to the Array
        //            MyTranArray(0) = MyTransactionEntity;

        //            string[] r = service.NotifyUraPayment(MyAPIUserName, s, MyTranArray);

        //            return r(0);

        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        return "<ResponseCode>" + MyPRN + "|E0000Y|FAILED</ResponseCode>";

        //    }
        //}

        //public object NotifyURABOUTransfer(string MyAmount, string MyAccount, System.DateTime MyBOUDate, System.DateTime MyCollDate)
        //{

        //    try
        //    {
        //        //service is the webservice that need to be authenticated using X509 certificate
        //        UraPmtService service = new UraPmtService();

        //        eTaxPmtLibrary.UraEncryption uraEnc = new eTaxPmtLibrary.UraEncryption();

        //        string s = uraEnc.EncryptedData(MyAPIPass, x509Cert_ura);

        //        //Declare an item of BOU Entity
        //        URAPmtInterface.BouEntity MyBOUEntity = new URAPmtInterface.BouEntity();

        //        MyBOUEntity.Amount = uraEnc.EncryptedData(MyAmount, x509Cert_ura);
        //        MyBOUEntity.Bank_cd = System.Configuration.ConfigurationManager.AppSettings("LOGINID");
        //        MyBOUEntity.Bou_value_dt = Microsoft.VisualBasic.Format(MyBOUDate, "dd-MMM-yy");
        //        MyBOUEntity.Coll_value_dt = Microsoft.VisualBasic.Format(MyCollDate, "dd-MMM-yy");
        //        MyBOUEntity.Source_acc_no = uraEnc.EncryptedData(MyAccount, x509Cert_ura);

        //        URAPmtInterface.BouEntity d = service.NotifyBouTransfer(MyAPIUserName, s, MyBOUEntity);

        //        return d;


        //    }
        //    catch (Exception ex)
        //    {
        //    }
        //}
             
}
    
            
           