using System;
using System.Text;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Data;
using System.IO;
using System.Xml;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Web.Configuration;

namespace AdaPayInterface
{
    public class InterfaceKcca
    {
        public string KCCAVendorCode = ConfigurationManager.AppSettings["KCCAVendorCode"];
        public string KCCAVendorPassword = ConfigurationManager.AppSettings["KCCAVendorPassword"];
        public string KCCASession_key = ConfigurationManager.AppSettings["KCCASession_Key"];
        public string KCCAHash = ConfigurationManager.AppSettings["KCCAHash"];
        public static string ProxyIP = ConfigurationManager.AppSettings["ProxyIP"];
        public static string ProxyPort = ConfigurationManager.AppSettings["ProxyPort"];

        



        //===================TAX PAYEMENT =============================
        public class KCCAADAPAYResponse
        {
            public string EXRef { get; set; }
            public string CustReference { get; set; }
            public string PaymentAmount { get; set; }
            public List<KCCADetails> KCCADetails { get; set; }
            public List<KCCAFailureDetials> FailureDetails { get; set; }
        }

        public class KCCADetails
        {
            public string CustomerName { get; set; }
            public string CustomerType { get; set; }
            public string Balance { get; set; }
            public string StatusCode { get; set; }
            public string StatusDescription { get; set; }
            public string AmountPaid { get; set; }
            public string Remark { get; set; }
            public string Responcecode { get; set; }
            public string FlexResponse { get; set; }
        }
       
        public class KCCAFailureDetials
        {
            public string Remark { get; set; }
            public string Responcecode { get; set; }
        }

        public string PullKCCADetails(string PRN)
        {
            string result = "";
            try
            {
                string HashKey = MD5Hash(KCCASession_key + KCCAVendorCode + KCCAVendorPassword + KCCAHash);
                KCCAGateway.BankPaymentService x = new KCCAGateway.BankPaymentService();
               
                // ----------------------- Get Session Key --------------------------
                var Sessionkey = new object();
                try
                {
                    Sessionkey = x.authenticate(KCCASession_key, KCCAVendorCode, KCCAVendorPassword, HashKey, "");                    
                }
                catch (Exception ex)
                {
                    result=ex.Message;
                }
                string skey = "";
                //DataRow rw = null;
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                StringReader strRdr = new StringReader(Sessionkey.ToString());
                try
                {
                    ds.ReadXml(strRdr);
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
                dt = ds.Tables[0];
                skey = dt.Rows[0][1].ToString();      
          

                // ------------------- Validate against prn --------------------------
                string validateresponse = "";
                try
                {
                    validateresponse = x.verifyReference  (skey, PRN, "", "");
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
                dt = new DataTable();
                ds = new DataSet();
                strRdr = new StringReader(validateresponse);
                try
                {
                    ds.ReadXml(strRdr);
                }
                catch (Exception ex)
                {
                    result = ex.Message;
                }
                dt = ds.Tables[0];

                // ---------------------- Return Xml Response -------------------------
                string PaymentDetails = "";
                MemoryStream XmlMs = new MemoryStream();
                XmlTextWriter XmlTxtWtr = new XmlTextWriter(XmlMs, System.Text.Encoding.UTF8);
                XmlTxtWtr.Formatting = Formatting.Indented;
                XmlTxtWtr.Indentation = 4;
                XmlTxtWtr.WriteStartDocument();
                XmlTxtWtr.WriteStartElement("KCCA PAYMENT DETAILS");
                XmlTxtWtr.WriteElementString("PRN",dt.Rows[0][4].ToString());
                XmlTxtWtr.WriteElementString("COIN", dt.Rows[0][1].ToString());
                XmlTxtWtr.WriteElementString("CUSTNAME", dt.Rows[0][2].ToString());
                XmlTxtWtr.WriteElementString("PHONENUMBER", dt.Rows[0][3].ToString());
                XmlTxtWtr.WriteElementString("AMOUNT", dt.Rows[0][7].ToString());
                XmlTxtWtr.WriteElementString("STATUS", dt.Rows[0][0].ToString());
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
                result= ex.Message;
            }
            return result;
        }


        public string PayKCCA(string PRN, string data, string UTILITYACCOUNT, string Amount, string debitmobilenumber, string TXNDATE, string TXNTIME, string alertcontact, string EXREF)
        {
            var FailureDetailsinfo = new KCCAADAPAYResponse();
            var KCCADetailsinfo = new KCCAADAPAYResponse();

            string HashKey = MD5Hash(KCCASession_key + KCCAVendorCode + KCCAVendorPassword + KCCAHash);
            KCCAGateway.BankPaymentService x = new KCCAGateway.BankPaymentService();

            // ----------------------- Get Session Key --------------------------
            var Sessionkey = new object();
            try
            {
                Sessionkey = x.authenticate(KCCASession_key, KCCAVendorCode, KCCAVendorPassword, HashKey, "");
            }
            catch (Exception ex)
            {
                //result = ex.Message;
            }
            string skey = "";
            //DataRow rw = null;
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            StringReader strRdr = new StringReader(Sessionkey.ToString());
            try
            {
                ds.ReadXml(strRdr);
            }
            catch (Exception ex)
            {
                //result = ex.Message;
            }
            dt = ds.Tables[0];
            skey = dt.Rows[0][1].ToString();


            // ------------------- Validate against prn --------------------------
            string validateresponse = "";
            try
            {
                validateresponse = x.verifyReference(skey, PRN, "", "");
            }
            catch (Exception ex)
            {
               // result = ex.Message;
            }
            dt = new DataTable();
            ds = new DataSet();
            strRdr = new StringReader(validateresponse);
            try
            {
                ds.ReadXml(strRdr);
            }
            catch (Exception ex)
            {
                //result = ex.Message;
            }
            dt = ds.Tables[0];
            string COIN, CUSTOMERNAME, PHONENUMBER, PRNDATE, EXPIRYDATE, AMOUNTDUE, PAYMENTCURRENCY, STATUS, StatusDescriptionx, balance, CustomerTypex;
            COIN = dt.Rows[0][1].ToString();
            CUSTOMERNAME = dt.Rows[0][2].ToString();
            PHONENUMBER = dt.Rows[0][3].ToString();
            PRNDATE = dt.Rows[0][5].ToString();
            EXPIRYDATE = dt.Rows[0][6].ToString();
            AMOUNTDUE = dt.Rows[0][7].ToString();
            StatusDescriptionx = "UNSUCCCESSFUL";
            balance = "";
            CustomerTypex = "POST PAID";

            PAYMENTCURRENCY = "";
            STATUS = dt.Rows[0][0].ToString();
            Datamanagement.PushKCCACustReferenceDetails(PRN,COIN,CUSTOMERNAME,PHONENUMBER,PRNDATE,EXPIRYDATE,AMOUNTDUE,PAYMENTCURRENCY ,STATUS );
            Thread.Sleep(5000);

            switch (STATUS)
            {
                case "A":
                    break;
                default:

                    FailureDetailsinfo.EXRef = EXREF;
                    FailureDetailsinfo.CustReference = PRN;
                    FailureDetailsinfo.PaymentAmount = Amount;
                    FailureDetailsinfo.FailureDetails = new List<KCCAFailureDetials>();
                    FailureDetailsinfo.FailureDetails.Add(new KCCAFailureDetials()
                    {
                        Remark = StatusDescriptionx,
                        Responcecode = "05"
                    });
                    if (STATUS == "T")
                    {
                        return "Transaction Already Transacted";
                    }
                    else if (STATUS == "X")
                    {
                        return "Transaction Has Expired ";
                    }
                    else
                    {
                        return "UNSUCCESSFULL";
                    }
                    break;
            }

            string sp_reference = PRN + "-" + CUSTOMERNAME;
            bool istransacted = Datamanagement.isalreadyTransactedKCCA(PRN);
            switch (istransacted)
            {
                case true:
                    FailureDetailsinfo.EXRef = EXREF;
                    FailureDetailsinfo.CustReference = PRN;
                    FailureDetailsinfo.PaymentAmount = Amount;
                    FailureDetailsinfo.FailureDetails = new List<KCCAFailureDetials>();
                    FailureDetailsinfo.FailureDetails.Add(new KCCAFailureDetials()
                    {
                        Remark =
                            "Payment reference has already been transacted",
                        Responcecode = "05"
                    });
                    return "UNSUCCESSFULL";
                    break;
                default:
                    break;
            }

            string Trantypey = "FCDB";
            PRN = PRN + Trantypey;
            string MyRef = (PRN.ToString()).Trim().Replace(" ", "").ToUpper() +
                           System.DateTime.Today.ToString("yyMMdd") + System.DateTime.Now.ToString("hhmmss") + "UTIL";

            Thread.Sleep(5000);

            //DEBIT CUSTOMER MOBILE MONEY ACCOUNT

            bool issuccess = true;
            string coin = Datamanagement.coin_(PRN);
            EXREF = "exref";

            switch (issuccess)
                {

                    case true:
                        Datamanagement.IsertintoKCCAPayments(MyRef, "KCCA Payment",
                                                              UTILITYACCOUNT, CUSTOMERNAME, balance, Amount,
                                                              "mobile money payment",
                                                               debitmobilenumber,  PRN,
                                                              CustomerTypex,
                                                              alertcontact, "",  TXNDATE,
                                                              STATUS, StatusDescriptionx, "KCCA Payment", "0", "2",
                                                              "", coin);


                        break;
                    case false:
                        FailureDetailsinfo.EXRef = EXREF;
                        FailureDetailsinfo.CustReference = PRN ;
                        FailureDetailsinfo.PaymentAmount = AMOUNTDUE;
                        FailureDetailsinfo.FailureDetails = new List<KCCAFailureDetials>();
                        FailureDetailsinfo.FailureDetails.Add(new KCCAFailureDetials()
                        {
                            Remark = "Failed To Post To Core Banking",
                            Responcecode = "05"
                        });
                        return "UNSUCCESSFULL";
                        break;
                }

                //Notify KCCA

                Thread.Sleep(5000);
                notifyKCCA(PRN);


                //RESPOND HERE
                KCCADetailsinfo.EXRef = EXREF;
                KCCADetailsinfo.CustReference = PRN;
                KCCADetailsinfo.PaymentAmount = AMOUNTDUE;
                KCCADetailsinfo.KCCADetails = new List<KCCADetails>();
                KCCADetailsinfo.KCCADetails.Add(new KCCADetails()
                {
                    AmountPaid = AMOUNTDUE,
                    Balance = AMOUNTDUE,
                    CustomerName = CUSTOMERNAME ,
                    CustomerType = "",
                    Remark = "",
                    Responcecode = "00",
                    StatusCode = STATUS ,
                    StatusDescription = StatusDescriptionx,
                    FlexResponse = EXREF
                });
                return KCCADetailsinfo.EXRef.ToString();
            }

        public string MD5Hash(string input)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] result = null;
            result = md5.ComputeHash(Encoding.ASCII.GetBytes(input));

            StringBuilder strBuilder = new StringBuilder();

            for (int i = 0; i <= result.Length - 1; i++)
            {
                strBuilder.Append(result[i].ToString("x2"));
            }
            return strBuilder.ToString();
        }
        
        //joannah ..10/7/2015
        public void notifyKCCA(string prn  )
        { 
            DataTable MyDataTable;                    

            string KCCAVendorCode = ConfigurationManager.AppSettings["KCCAVendorCode"];
            string KCCAVendorPassword = ConfigurationManager.AppSettings["KCCAVendorPassword"];
            string KCCASession_key = ConfigurationManager.AppSettings["KCCASession_Key"];
            string KCCAHash = ConfigurationManager.AppSettings["KCCAHash"];

            System.Net.ServicePointManager.CertificatePolicy = new MyPolicy();
            string HashKey = MD5Hash(KCCASession_key + KCCAVendorCode + KCCAVendorPassword + KCCAHash);
            KCCAGateway.BankPaymentService x = new KCCAGateway.BankPaymentService();
            //WebRequest.DefaultWebProxy = new WebProxy("http://" + ProxyIP + ":" + ProxyPort + "/", true);         


            // ----------------------- Get Session Key --------------------------
            var Sessionkey = new object();
            try
            {
                Sessionkey = x.authenticate(KCCASession_key, KCCAVendorCode, KCCAVendorPassword, HashKey, "");
            }
            catch (Exception ex)
            {

            }

            string KCCATransaction = "";
            string skey = "";
            string BankStatus = "C";
            DataTable  MyRst = new DataTable ();
            DataSet ds = new DataSet();
            DataTable dt = new DataTable();
            StringReader strRdr = new StringReader(Sessionkey.ToString());
            try
            {
                ds.ReadXml(strRdr);
            }
            catch (Exception ex)
            {

            }

            dt = ds.Tables [0];
            skey = dt.Rows[0][1].ToString();
            MyDataTable = Datamanagement.getKCCA(prn);
            DataRow dr = MyDataTable.Rows[0];
            

                string MyRef = dr["CustomerRefNumber"].ToString();
                string coin = dr["COIN"].ToString();
                string DRAMOUNT = dr["AmountPaid"].ToString();
                string DRACCBRANCH = dr["UtilityBranch"].ToString();
                DateTime dt1 = Convert.ToDateTime (dr["ValueDate"]);
                string TXNDATE = String.Format("{0:MM/dd/yy}", dt1);   //dr["ValueDate"].ToString(); //String.Format("{0:d}", dt);
                string TnxId = dr["ReferenceNumber"].ToString();
                int Id = Convert.ToInt32(dr["PaymentId"].ToString());


                //Create the Xml string
                var XmlMs = new MemoryStream();
                var XmlTxtWtr = new XmlTextWriter(XmlMs, System.Text.Encoding.UTF8);
                XmlTxtWtr.Formatting = Formatting.Indented;
                XmlTxtWtr.Indentation = 4;
                XmlTxtWtr.WriteStartDocument();
                XmlTxtWtr.WriteStartElement("transactionRecord");
                XmlTxtWtr.WriteElementString("PRN", MyRef);
                XmlTxtWtr.WriteElementString("COIN", coin);
                XmlTxtWtr.WriteElementString("amountPaid", DRAMOUNT);
                XmlTxtWtr.WriteElementString("paymentDate", TXNDATE);
                XmlTxtWtr.WriteElementString("valueDate", TXNDATE);
                XmlTxtWtr.WriteElementString("status", BankStatus);
                XmlTxtWtr.WriteElementString("bankBranchCode", DRACCBRANCH);
                XmlTxtWtr.WriteElementString("transactionID", TnxId);
                XmlTxtWtr.WriteEndElement();
                XmlTxtWtr.WriteEndDocument();
                XmlTxtWtr.Flush();

                var strmrdr = new StreamReader(XmlMs);
                XmlMs.Seek(0, SeekOrigin.Begin);



                KCCATransaction = strmrdr.ReadToEnd();
                // KCCATransaction = KCCATransaction.Replace("encoding=""utf-8""", "");   // .Replace("encoding=""utf-8""", "");

            
            HashKey = MD5Hash(skey + MyRef + KCCAHash);
            var KCCANotification = new KCCAGateway.BankPaymentService ();
            var validateresponse = KCCANotification.transact (skey, MyRef, KCCATransaction, HashKey, TnxId);
            //DataTable MyRst1 = new DataTable();
            //MyRst1 = ds.Tables(0)
            //DataSet dts = new DataSet();
            strRdr = new StringReader(validateresponse);
            ds.ReadXml(strRdr);

            if (dt.Rows[0][1].ToString() != "0") 
            {


            //var stat = validateresponse.
            var strUpdate = "UPDATE KCCAPayments SET  KCCAStatus='Posted'  where PaymentId = Id";
            string query1 = "Insert into KCCAPaymentsArchive Select * from KCCAPayments where CustmerRefNumber = " + prn;

            string query2 = "Delete from KCCAPayments where CustmerRefNumber =" + prn;

            try
            {
                using (
                    var conn =
                        new System.Data.SqlClient.SqlConnection(WebConfigurationManager.ConnectionStrings["Bridge"].ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new System.Data.SqlClient.SqlCommand(strUpdate, conn))
                    {
                        var df = cmd.ExecuteReader();
                    }
                }
            }

            catch (Exception ex)
            {

            }
                       
            //dat.DBAction("UPDATE KCCAPayments SET  Authorized=1,Status = 'Posted' ,KCCAStatus='Posted',AuthorizedBy = '" & Session("userName") & "' where PaymentID = " & MyPayID & "", DataManagement.DBActionType.Update)
                                        
            }

        
        }
    }
    
}