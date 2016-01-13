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
using System.Collections.Generic;
using System.Threading;
using System.Web.Services;
using System.Web.Services.Protocols;
using FinpayiAdmin.FinpayiAdmin;
using System.Data.SqlClient;
using System.Web.Configuration;
namespace AdaPayInterface
{
    public class InterfaceUmeme
    {
        string UmemeVendorCode = ConfigurationManager.AppSettings["UMEMEVendorCode"];
        string UmemeVendorPassword = ConfigurationManager.AppSettings["UMEMEVendorPass"];
        //var dat =  WebConfigurationManager.ConnectionStrings["Finpay"].ConnectionString;


        public string PullUmemeDetails(string Prn)
        {
            //YoApiClient.GetYoApi();
            // ---------------------- Validate the Prn -------------------------
            System.Net.ServicePointManager.CertificatePolicy = new MyPolicy();
            UMEMEGateway.EPayment service = new UMEMEGateway.EPayment();
            UMEMEGateway.Customer x = new UMEMEGateway.Customer();
            var d = new object();
            x = service.ValidateCustomer(Prn, UmemeVendorCode, UmemeVendorPassword);

            if (x.StatusDescription == "SUCCESS" && x.CustomerRef == "")
            {
                x.CustomerRef = Prn;
            }

            // ------------------------- Serialize Xml Response -------------------------  
            string PaymentDetails = "";
            MemoryStream XmlMs = new MemoryStream();
            XmlTextWriter XmlTxtWtr = new XmlTextWriter(XmlMs, System.Text.Encoding.UTF8);
            XmlTxtWtr.Formatting = Formatting.Indented;
            XmlTxtWtr.Indentation = 4;
            XmlTxtWtr.WriteStartDocument();
            XmlTxtWtr.WriteStartElement("UMEME PAYMENT DETAILS");
            XmlTxtWtr.WriteElementString("PRN", x.CustomerRef);
            XmlTxtWtr.WriteElementString("CREDIT", x.Credit.ToString());
            XmlTxtWtr.WriteElementString("CUSTNAME", x.CustomerName);
            XmlTxtWtr.WriteElementString("AMOUNT", x.Balance.ToString());
            XmlTxtWtr.WriteElementString("CUSTYPE", x.CustomerType);
            XmlTxtWtr.WriteElementString("STATUS", x.StatusCode);
            XmlTxtWtr.WriteEndElement();
            XmlTxtWtr.WriteEndDocument();
            XmlTxtWtr.Flush();
            StreamReader strmrdr = new StreamReader(XmlMs);
            XmlMs.Seek(0, SeekOrigin.Begin);
            PaymentDetails = strmrdr.ReadToEnd();
            return PaymentDetails;
        }
        public string FinpayUmeme(string CustRefence, string Area, string EXREF, string DRACCBRANCH,
                                          string DRACCOUNTNUMBER, string DRAMOUNT, string DRACCURR, string TXNDATE,
                                          string TXNTIME, string PhoneNumber, string CustRemarks, string Location)
        {
            string ReferenceNumberx = "";
            string Areax = "";
            string OutstandingBalx = "";
            string CustNamex = "";
            string PropertyRefx = "";
            string CustomerErrorx = "";
            string cust_ref_ = CustRefence;

            string OriginalReference = "";
            OriginalReference = CustRefence;

            string[] FCDBSetting = null;
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

            string sp_reference = "";

            bool isrepeat = false;
            bool istransacted = false;
            bool issuccess = false;

            string valuefield = "";

            var FailureDetailsinfo = new UMEMEFCDBResponse();
            var UmemeDetailsinfo = new UMEMEFCDBResponse();

            Datamanagement.PushUmemeCustReferenceDetails(CustRefence);

            //
            System.Net.ServicePointManager.CertificatePolicy = new MyPolicy();

            string UmemeVendorCode = ConfigurationManager.AppSettings["UMEMEVendorCode"];
            string UmemeVendorPassword = ConfigurationManager.AppSettings["UMEMEVendorPass"];

            UMEMEGateway.EPayment service = new UMEMEGateway.EPayment();
            UMEMEGateway.Customer x = new UMEMEGateway.Customer();
            var d = new object();
            x = service.ValidateCustomer(OriginalReference , UmemeVendorCode, UmemeVendorPassword);



            var res = service.ValidateCustomer(CustRefence, UmemeVendorCode, UmemeVendorPassword);
            string remark = "";
            string CustomerRef = res.CustomerRef;
            string CustomerName = res.CustomerName;
            string CustomerType = res.CustomerType;
            string Balance = Convert.ToString(res.Balance);
            string StatusCode = res.StatusCode;
            string StatusDescription = res.StatusDescription;
            Boolean auth = false;

            if (res.StatusDescription != "SUCCESS")
            {
                auth = false;
                remark = res.StatusDescription;
            }
            else
            {

                auth = true;
                remark = "Successful";
            }


            Datamanagement.UpdateUMEMEDetails(CustRefence, CustomerName, CustomerType, Balance, StatusCode, StatusDescription, remark, auth);
            //umemept.Close();
            //
            //Thread.Sleep(5000);

            string CustReferenceDetials = Datamanagement.getUmemeCustRefDetails(CustRefence);

            string[] Custrefdets = null;
            Custrefdets = CustReferenceDetials.Split('|');
            ReferenceNumberx = Custrefdets[0];
            //Areax = Custrefdets[1];
            OutstandingBalx = Custrefdets[1];
            CustNamex = Custrefdets[2];
            string statuscodex = Custrefdets[3];
            PropertyRefx = Custrefdets[4];
            //CustomerErrorx = Custrefdets[5];

            switch (PropertyRefx)
            {
                case "SUCCESS":
                    //Continue
                    break;
                default:
                    //Do Zilch

                    FailureDetailsinfo.EXRef = EXREF;
                    FailureDetailsinfo.CustReference = CustRefence;
                    FailureDetailsinfo.PaymentAmount = DRAMOUNT;
                    FailureDetailsinfo.FailureDetails = new List<UmemeFailureDetials>();
                    FailureDetailsinfo.FailureDetails.Add(new UmemeFailureDetials()
                                                              {
                                                                  Remark = CustomerErrorx,
                                                                  Responcecode = "05"
                                                              });
                    return "UNSUCCESSFUL";
                    break;
            }

            sp_reference = ReferenceNumberx + "-" + CustNamex;
            //Get FCDB details here

            FCDBSetings = Datamanagement.getFCDBSettings("UMEME");
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

            istransacted = Datamanagement.isalreadyTransacted(CustRefence);

            switch (istransacted)
            {
                case true:
                    FailureDetailsinfo.EXRef = EXREF;
                    FailureDetailsinfo.CustReference = CustRefence;
                    ////FailureDetailsinfo.Area = Area;
                    FailureDetailsinfo.PaymentAmount = DRAMOUNT;
                    FailureDetailsinfo.FailureDetails = new List<UmemeFailureDetials>();
                    FailureDetailsinfo.FailureDetails.Add(new UmemeFailureDetials()
                                                              {
                                                                  Remark =
                                                                      "Payment reference has already been transacted",
                                                                  Responcecode = "05"
                                                              });
                    return "UNSUCCESSFUL";
                    break;
                default:
                    break;
            }

            CustRefence = CustRefence + Trantypey;
            string MyRef = (OriginalReference.ToString()).Trim().Replace(" ", "").ToUpper() +
                           System.DateTime.Today.ToString("yyMMdd") + System.DateTime.Now.ToString("hhmmss") + "UTIL";


            valuefield =
                Datamanagement.getFinpayGlobalDetails(
                    "select * from Payments where CustomerRefNumber = '" + OriginalReference +
                    "' AND STATUS= 'Posted' AND AccountNumber= '" + DRACCOUNTNUMBER + "' AND AMOUNTPAID= '" + DRAMOUNT +
                    "' AND VALUEDATE='" + TXNDATE + "' and phonenumber='" + PhoneNumber + "'", "CustomerRefNumber");
            if (valuefield == "")
            {
                valuefield =
                    Datamanagement.getFinpayGlobalDetails(
                        "select * from PaymentsArchive where CustomerRefNumber = '" + OriginalReference +
                        "' AND STATUS= 'Posted' AND AccountNumber= '" + DRACCOUNTNUMBER + "' AND AMOUNTPAID= '" +
                        DRAMOUNT + "' AND VALUEDATE='" + TXNDATE + "' and phonenumber='" + PhoneNumber + "'",
                        "CustomerRefNumber");
            }
            switch (valuefield)
            {
                case "":

                    break;
                default:
                    FailureDetailsinfo.EXRef = EXREF;
                    FailureDetailsinfo.CustReference = CustRefence;
                    //FailureDetailsinfo.Area = Area;
                    FailureDetailsinfo.PaymentAmount = DRAMOUNT;
                    FailureDetailsinfo.FailureDetails = new List<UmemeFailureDetials>();
                    FailureDetailsinfo.FailureDetails.Add(new UmemeFailureDetials()
                                                              {
                                                                  Remark =
                                                                      "Payment reference has already been transacted",
                                                                  Responcecode = "05"
                                                              });
                    return "UNSUCCESSFUL";
                    break;
            }

            //check whether transaction already sent to the bridge
            isrepeat = Datamanagement.isalreadyPosted(MyRef);
            if (isrepeat == false)
            {
                //post to the bridge
                Datamanagement.InsertIntoLog(MyRef, Subhosty,
                                             Msgtypey, Procodey, TXNDATE, TXNTIME, DRACCBRANCH, CustRefence, CustRefence,
                                             DRAMOUNT, Commissiony, DRACCOUNTNUMBER, MerchantTypeY, EXREF, Terminaly,
                                             DRACCBRANCH,
                                             Batchy, Terminaly, sp_reference, DRACCOUNTNUMBER, DRACCURR, "0",
                                             MessageFlagy,
                                             Workstationy,
                                             PosConfirmedy, PosReversedy, "0", UtilityAccountY, UtilityAccBranchY,
                                             MerchantTypeY, EftSourcey, IbTxny, "", Flexuseridy);
            }

            Thread.Sleep(5000);
            issuccess = Datamanagement.isSuccessful(MyRef);
            EXREF = Datamanagement.Ecternalref(MyRef);
            switch (issuccess)
            {
                case true:
                    //joannah...7/8/2015
                    Datamanagement.IsertintoUmemePayments(MyRef, "Energy Payment", UtilityAccBranchY,
                                                         UtilityAccountY, CustNamex, OutstandingBalx, DRAMOUNT,
                                                         "DIRECTDEBIT",
                                                         DRACCBRANCH, DRACCOUNTNUMBER, "", OriginalReference,
                                                        "", PhoneNumber, CustRemarks, FCDBMAKERy, Procodey, TXNDATE,
                                                         DRACCBRANCH,
                                                         statuscodex, PropertyRefx, "", "", "", "");
                    break;
                case false:
                    FailureDetailsinfo.EXRef = EXREF;
                    FailureDetailsinfo.CustReference = CustRefence;
                    //FailureDetailsinfo.Area = Area;
                    FailureDetailsinfo.PaymentAmount = DRAMOUNT;
                    FailureDetailsinfo.FailureDetails = new List<UmemeFailureDetials>();
                    FailureDetailsinfo.FailureDetails.Add(new UmemeFailureDetials()
                                                              {
                                                                  Remark = "Failed To Post To Core Banking",
                                                                  Responcecode = "05"
                                                              });
                    return "UNSUCCESSFUL";
                    break;
            }

            


            //notify umeme payments..joannah 9/7/2015
            PostUmemebill(CustRefence, OriginalReference, CustomerName, DRAMOUNT, "DIRECTDEBIT", "", PhoneNumber, remark, FCDBMAKERy, "254", TXNDATE, "", 0, "2", "2");
            

            //RESPOND HERE
            UmemeDetailsinfo.EXRef = EXREF;
            UmemeDetailsinfo.CustReference = OriginalReference;
            UmemeDetailsinfo.PaymentAmount = DRAMOUNT;
            UmemeDetailsinfo.UmemeDetails = new List<UmemeDetials>();
            UmemeDetailsinfo.UmemeDetails.Add(new UmemeDetials()
                                                  {
                                                      CustomerName = CustNamex,
                                                      AmountPaid = OutstandingBalx,
                                                      Responcecode = "00",
                                                      Remark = CustRemarks
                                                  });
            return UmemeDetailsinfo.EXRef.ToString();
        }

        //======================================================================


             public void PostUmemebill(string refernum , string strReferenceNum , string strCustomerName , string strAmountPaid , string strPayOption , string strChequeNum , string strPhoneNumber , string strRemarks , string strUser , string  strTranCode , string strValDate , string custype , int statcode, string statdesc , string paytypes )
             {

                //System.Net.ServicePointManager.CertificatePolicy = new MyPolicy();
                UMEMEGateway.EPayment service  = new UMEMEGateway.EPayment (); 
                //var  pr =  new System.Net.WebProxy(ProxyIP, ProxyPort);
                //If UseProxy = True Then
                //    service.Proxy = pr
                //End If
                UMEMEGateway.Transaction  MyTransactionEntity =  new UMEMEGateway.Transaction();
                string VendorCode = ConfigurationManager.AppSettings["UMEMEVendorCode"];
                string VendorPass = ConfigurationManager.AppSettings["UMEMEVendorPass"];


                X509Certificate2 x509Cert_bnk = new X509Certificate2(HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["BankCertificate"]), ConfigurationManager.AppSettings["BankCertPass"]);
                
                 
                 string vendorid  = refernum ;
                string tranno ;
                int reversal  = 0 ;
                int mode = 0;

                if  (strCustomerName =="REVERSAL") 
                {
                    tranno = refernum;
                    vendorid = strReferenceNum + strTranCode ;
                    reversal = 1 ;
                }
                else
                {
                     tranno = "";
                     reversal = 0;
                }
                string  det  = DateTime.Today.ToString (); //Format(Date.Today, "dd/MM/yyyy");
                if( strTranCode == "Authorize") 
                {
                    strAmountPaid = strAmountPaid.Substring(0, strAmountPaid.Length - 2);
                }
                if (reversal == 1 ) 
                {
                    strAmountPaid = "-" +  strAmountPaid.Substring(0, strAmountPaid.Length - 2);
                }


                var validateresponse = service.ValidateCustomer(strReferenceNum, UmemeVendorCode, UmemeVendorPassword);

                 string strrefnum = strReferenceNum.Trim ();
                 string validy = strCustomerName.Trim();
                 string phoneno = strPhoneNumber.Trim();
                 string custtype = custype.Trim();
                 string vendoidentity = vendorid.Trim();
                 string vendacode = UmemeVendorCode.Trim();
                 string vendapass = UmemeVendorPassword.Trim();
                 string dets = det.Trim();
                 string payty = paytypes.Trim();
                 string user = strUser.Trim();
                 string ammount = strAmountPaid.Trim();
                 string remark = strRemarks.Trim();
                 string payoptiony = strPayOption.Trim();

                string MyString  = strrefnum  + validy + phoneno + custtype + vendoidentity +  vendacode +  vendapass + dets + payty + user + ammount + remark + payoptiony  ;


                RSACryptoServiceProvider privateKey  = (x509Cert_bnk.PrivateKey) as RSACryptoServiceProvider ;          // RSACryptoServiceProvider (x509Cert_bnk.PrivateKey);

                var buffer  = new ASCIIEncoding().GetBytes(MyString) ;   //Encoding.[Default].GetBytes(MyString)
                var hash  = new SHA1Managed().ComputeHash(buffer);
                var signature  = privateKey.SignData(buffer, "SHA1"); //privateKey.SignData(buffer, New SHA1Managed())     
                string digitalsignature  = System.Convert.ToBase64String(signature);
                //Dim strConvertedSignature As String
                digitalsignature = Convert.ToBase64String(signature);
                //verify the signature
                RSACryptoServiceProvider publicKey   = (x509Cert_bnk.PublicKey.Key) as RSACryptoServiceProvider;

                Boolean verify   = privateKey.VerifyHash(hash, CryptoConfig.MapNameToOID("SHA1"), signature); //publicKey.VerifyData(buffer, New SHA1Managed(), signature)

                MyTransactionEntity.StatusDescription = validateresponse.StatusDescription.Trim();
                MyTransactionEntity.DigitalSignature = digitalsignature.Trim();
                MyTransactionEntity.StatusCode = validateresponse.StatusCode.Trim();
                DateTime dt = DateTime.Today;
                MyTransactionEntity.PaymentDate = String.Format("{0:d}", dt); //DateTime .Today .ToString (); //Format(Date.Today, "dd/MM/yyyy");
                MyTransactionEntity.Password = UmemeVendorPassword.Trim();
                MyTransactionEntity.TranAmount = strAmountPaid.Trim();
                MyTransactionEntity.Teller = strUser.Trim();
                MyTransactionEntity.VendorCode = UmemeVendorCode.Trim();
                MyTransactionEntity.TranNarration = strRemarks.Trim();
                MyTransactionEntity.VendorTranId = vendorid.Trim();
                MyTransactionEntity.TranIdToReverse = tranno.Trim();
                paytypes = "2";
                MyTransactionEntity.PaymentType = paytypes.Trim();
                MyTransactionEntity.TranType = strPayOption.Trim();
                MyTransactionEntity.CustomerRef = validateresponse.CustomerRef.Trim();
                MyTransactionEntity.CustomerName = validateresponse.CustomerName.Trim();
                MyTransactionEntity.CustomerType = validateresponse.CustomerType.Trim();
                MyTransactionEntity.CustomerTel = strPhoneNumber.Trim();
                string rever = reversal.ToString();
                MyTransactionEntity.Reversal =  rever  .Trim();
                string modey = mode.ToString();
                MyTransactionEntity.Offline =  modey.Trim();

               var Token = new UMEMEGateway.Token();
               UMEMEGateway.Response PostUmemeTransactionsResponse = new UMEMEGateway.Response();
                
               var conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["Finpay"].ConnectionString);

               // Dim postresponse As UmemeBillInterface.Response
                //check customer type to determine method to use
                if (validateresponse.CustomerType == "POSTPAID" )
                 {
                     
                        PostUmemeTransactionsResponse = service.PostBankUmemePayment(MyTransactionEntity);

                        string receiptno  = PostUmemeTransactionsResponse.ReceiptNumber;
                        string status  = PostUmemeTransactionsResponse.StatusCode;
                        string desc = PostUmemeTransactionsResponse.StatusDescription;
                        if (desc == "SUCCESS" ) 
                        {
                            if (rever == "0" )
                            {
                                string query = "UPDATE UmemePayments set umemeStatus = 'Posted' where ReferenceNumber = " + strrefnum ;
                                string query1 = "Insert into UmemePaymentsArchive Select * from umemePayments where ReferenceNumber = " + strrefnum;

                                string query2 = "Delete from umemePayments where ReferenceNumber =" + strrefnum;

                                var cmd = new SqlCommand(query, conn);
                                var cmd1 = new SqlCommand(query1, conn);
                                var cmd2 = new SqlCommand(query2, conn);
                                cmd.ExecuteScalar();

                            }
                                else
                            {
                                string query = "UPDATE UmemePayments set umemeStatus = 'Reversed' where ReferenceNumber = " + strrefnum;
                                string query1 = "Insert into UmemePaymentsArchive Select * from umemePayments where ReferenceNumber = " + strrefnum;

                                string query2 = "Delete from umemePayments where ReferenceNumber =" + strrefnum;

                                var cmd = new SqlCommand(query, conn);
                                var cmd1 = new SqlCommand(query1, conn);
                                var cmd2 = new SqlCommand(query2, conn);
                                cmd.ExecuteScalar();
                                //dat.DBAction("UPDATE UmemePaymentsarchive set umemeStatus = 'Reversed', ReceiptNo='" & receiptno & "',FEEDBACK= '" & desc & "' where ReferenceNumber = '" & ref & "'", DataManagement.DBActionType.Update)
                            }
                        }
                   }
                   else
                    {
                        if ( rever == "0" ) 
                        {
                               string query = "UPDATE UmemePayments set FEEDBACK = + desc + where ReferenceNumber = " + strrefnum ;
                                var cmd = new SqlCommand(query, conn);
                                cmd.ExecuteScalar();
                            //dat.DBAction("UPDATE UmemePayments set FEEDBACK= '" & desc & "' where ReferenceNumber = '" & ref & "'", DataManagement.DBActionType.Update)
                        }
                        else
                        {
                            string query = "UPDATE UmemePaymentsarchive set FEEDBACK = + desc + where ReferenceNumber = " + strrefnum ;
                            var cmd = new SqlCommand(query, conn);
                            cmd.ExecuteScalar();
                            //dat.DBAction("UPDATE UmemePaymentsarchive set FEEDBACK= '" & desc & "' where ReferenceNumber = '" & ref & "'", DataManagement.DBActionType.Update)
                        }
                    }
                
                if  ( validateresponse.CustomerType == "PREPAID" )
                {
                   //var Token = new UMEMEGateway.Token();
                    Token = service.PostYakaPayment(MyTransactionEntity);
                    string account, debt, receiptno, status, desc, meterno, unitno, tokenvalue, inflation, tax, fx, fuel, amount, prepaidt ;
                    account = Token.PayAccount;
                    debt = Token.DebtRecovery;
                    receiptno = Token.ReceiptNumber;
                    status = Token.StatusCode;
                    desc = Token.StatusDescription;
                    meterno = Token.MeterNumber;
                    unitno = Token.Units;
                    tokenvalue = Token.TokenValue;
                    inflation = Token.Inflation;
                    tax = Token.Tax;
                    fx = Token.Fx;
                    fuel = Token.Fuel;
                    amount = Token.TotalAmount;
                    prepaidt = Token.PrepaidToken;

                    if (desc == "SUCCESS" )
                    {
                                string query = "UPDATE UmemePayments set umemeStatus = 'Posted' where ReferenceNumber = " + strrefnum ;
                                var cmd = new SqlCommand(query, conn);
                                cmd.ExecuteScalar();
                        ////'Dim str As String = "UPDATE UmemePayments set umemeStatus = 'Posted'  AND ReceiptNo='" & receiptno & "' AND FEEDBACK= '" & desc & "' where ReferenceNumber = '" & ref & "'"
                        //dat.DBAction("UPDATE UmemePayments set umemeStatus = 'Posted', ReceiptNo='" & receiptno & "',FEEDBACK= '" & desc & "' where ReferenceNumber = '" & ref & "'", DataManagement.DBActionType.Update)
                        ////'--- Weka token Hapa ---
                        //Dim strAct As String = "INSERT INTO PREPAID_TOKEN(PaymentID,ReferenceNumber,ReceiptNo,PayAccount,DebtRecovery,MeterNumber,Units,TokenValue,Inflation,Tax,FX,Fuel,TotalAmount,PrepaidToken,StatusCode,StatusDesc)" & _
                        //"VALUES((Select PaymentID from umemePayments where ReferenceNumber = '" & vendorid & "'),'" & vendorid & "','" & receiptno & "','" & account & "','" & debt & "','" & meterno & "','" & unitno & "','" & tokenvalue & "','" & inflation & "','" & tax & "','" & fx & "','" & fuel & "','" & amount & "','" & prepaidt & "','" & statcode & "','" & desc & "')"
                        //dat.DBAction(strAct, DataManagement.DBActionType.Insert)
                    }
                    else
                    {
                            string query = "UPDATE UmemePaymentsarchive set FEEDBACK = + desc + where ReferenceNumber = " + strrefnum ;
                            var cmd = new SqlCommand(query, conn);
                            cmd.ExecuteScalar();
                        //dat.DBAction("UPDATE UmemePayments set FEEDBACK= '" & desc & "' where ReferenceNumber = '" & ref & "'", DataManagement.DBActionType.Update)
                        //'reverse transaction from core
                    }


                }
                if (validateresponse.CustomerType == "POSTPAID")
                {
                    //return PostUmemeTransactionsResponse;
                }
                else
                {
                    //return Token;

                }
            
             }

        }



        


        public class UMEMEFCDBResponse
        {
            public string EXRef { get; set; }
            public string CustReference { get; set; }
            public string PaymentAmount { get; set; }
            public List<UmemeDetials> UmemeDetails { get; set; }
            public List<UmemeFailureDetials> FailureDetails { get; set; }
        }

        public class UmemeDetials
        {
            public string CustomerName { get; set; }
            public string CustomerType { get; set; }
            public string Balance { get; set; }
            public string StatusCode { get; set; }
            public string StatusDescription { get; set; }
            public string AmountPaid { get; set; }
            public string Remark { get; set; }
            public string Responcecode { get; set; }
        }

        public class UmemeFailureDetials
        {
            public string Remark { get; set; }
            public string Responcecode { get; set; }
        }


        public class MyPolicy : ICertificatePolicy
        {

            public bool CheckValidationResult(ServicePoint srvPoint, X509Certificate cert, WebRequest request, int certificateProblem)
            {
                //Return True to force the certificate to be accepted.
                return true;
            }
        }


    
}


