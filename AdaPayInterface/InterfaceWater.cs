using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Xml;
using System.IO;
using System.Text;
using System.Configuration;
using System.Threading;
using System.Data.SqlClient;
using System.Web.Configuration;

namespace AdaPayInterface
{
    public class InterfaceWater
    {
        string VendorCode = ConfigurationManager.AppSettings["WaterVendorCode"];
        string VendorPass = ConfigurationManager.AppSettings["WaterVendorPass"];

        public string PullWaterDetails(string RefNum,string Area)
        {
            System.Net.ServicePointManager.CertificatePolicy = new MyPolicy();
            NWSCGateway.NWSCBillingInterface w = new NWSCGateway.NWSCBillingInterface();
            NWSCGateway.Customer d = w.verifyCustomerDetailsWithArea(RefNum, Area, VendorCode, VendorPass);

            // ------------------------- Serialize Xml Response -------------------------
            string PaymentDetails = "";
            MemoryStream XmlMs = new MemoryStream();
            XmlTextWriter XmlTxtWtr = new XmlTextWriter(XmlMs, System.Text.Encoding.UTF8);
            XmlTxtWtr.Formatting = Formatting.Indented;
            XmlTxtWtr.Indentation = 4;
            XmlTxtWtr.WriteStartDocument();
            XmlTxtWtr.WriteStartElement("WATER PAYMENT DETAILS");
            XmlTxtWtr.WriteElementString("PRN", d.CustRef);
            XmlTxtWtr.WriteElementString("AREA", d.Area);
            XmlTxtWtr.WriteElementString("CUSTNAME", d.CustName);
            XmlTxtWtr.WriteElementString("AMOUNT",d.OutstandingBal.ToString());
            XmlTxtWtr.WriteElementString("PROPERTYREF", d.PropertyRef);
            XmlTxtWtr.WriteElementString("CUSTERROR", d.CustomerError);
            XmlTxtWtr.WriteEndElement();
            XmlTxtWtr.WriteEndDocument();
            XmlTxtWtr.Flush();
            StreamReader strmrdr = new StreamReader(XmlMs);
            XmlMs.Seek(0, SeekOrigin.Begin);
            PaymentDetails = strmrdr.ReadToEnd();
            return PaymentDetails;
        }
        public string FinpayNWSC(string CustRefence, string Area, string EXREF, string DRACCBRANCH,
                                           string DRACCOUNTNUMBER, string DRAMOUNT, string DRACCURR, string TXNDATE,
                                           string TXNTIME, string PhoneNumber, string CustRemarks, string Location)
        {
            string ReferenceNumberx = "";
            string Areax = "";
            string OutstandingBalx = "";
            string CustNamex = "";
            string PropertyRefx = "";
            string CustomerErrorx = "";

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

            var FailureDetailsinfo = new NWSCFCDBResponse();
            var WaterDetailsinfo = new NWSCFCDBResponse();

            Datamanagement.PushWaterCustReferenceDetails(CustRefence, Location);

            Thread.Sleep(5000);

            string CustReferenceDetials = Datamanagement.getWaterCustRefDetails(CustRefence);

            string[] Custrefdets = null;
            Custrefdets = CustReferenceDetials.Split('|');
            ReferenceNumberx = Custrefdets[0];
            Areax = Custrefdets[1];
            OutstandingBalx = Custrefdets[2];
            CustNamex = Custrefdets[3];
            PropertyRefx = Custrefdets[4];
            CustomerErrorx = Custrefdets[5];

            switch (CustomerErrorx)
            {
                case "NONE":
                    //Continue
                    break;
                default:
                    //Do Zilch

                    FailureDetailsinfo.EXRef = EXREF;
                    FailureDetailsinfo.NWSCReferenceNumber = CustRefence;
                    FailureDetailsinfo.Area = Area;
                    FailureDetailsinfo.PaymentAmount = DRAMOUNT;
                    FailureDetailsinfo.FailureDetails = new List<NWSCFailureDetials>();
                    FailureDetailsinfo.FailureDetails.Add(new NWSCFailureDetials()
                                                              {
                                                                  Remark = CustomerErrorx,
                                                                  Responcecode = "05"
                                                              });
                    return "UNSUCCESSFUL";
                    break;
            }

            sp_reference = ReferenceNumberx + "-" + CustNamex;
            //Get FCDB details here

            FCDBSetings = Datamanagement.getFCDBSettings("NWSC");
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
                    FailureDetailsinfo.NWSCReferenceNumber = CustRefence;
                    FailureDetailsinfo.Area = Area;
                    FailureDetailsinfo.PaymentAmount = DRAMOUNT;
                    FailureDetailsinfo.FailureDetails = new List<NWSCFailureDetials>();
                    FailureDetailsinfo.FailureDetails.Add(new NWSCFailureDetials()
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
                    FailureDetailsinfo.NWSCReferenceNumber = CustRefence;
                    FailureDetailsinfo.Area = Area;
                    FailureDetailsinfo.PaymentAmount = DRAMOUNT;
                    FailureDetailsinfo.FailureDetails = new List<NWSCFailureDetials>();
                    FailureDetailsinfo.FailureDetails.Add(new NWSCFailureDetials()
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
                    Datamanagement.IsertintoNWSCPayments(MyRef, "Energy Payment", UtilityAccBranchY,
                                                         UtilityAccountY, CustNamex, OutstandingBalx, DRAMOUNT,
                                                         "DIRECTDEBIT",
                                                         DRACCBRANCH, DRACCOUNTNUMBER, "", OriginalReference,
                                                         PhoneNumber, CustRemarks, FCDBMAKERy, Procodey, TXNDATE,
                                                         DRACCBRANCH,
                                                         "", Areax);
                    break;
                case false:
                    FailureDetailsinfo.EXRef = EXREF;
                    FailureDetailsinfo.NWSCReferenceNumber = OriginalReference;
                    FailureDetailsinfo.Area = Area;
                    FailureDetailsinfo.PaymentAmount = DRAMOUNT;
                    FailureDetailsinfo.FailureDetails = new List<NWSCFailureDetials>();
                    FailureDetailsinfo.FailureDetails.Add(new NWSCFailureDetials()
                                                              {
                                                                  Remark = "Failed To Post To Core Banking",
                                                                  Responcecode = "05"
                                                              });
                    return "UNSUCCESSFUL";
                    break;
            }

            //Notify Payments...joannah 9/7/2015

            string VendorCode = ConfigurationManager.AppSettings["WaterVendorCode"];
            string VendorPass = ConfigurationManager.AppSettings["WaterVendorPass"];
            var service = new NWSCGateway.NWSCBillingInterface();
            DateTime convertedDate = Convert.ToDateTime(TXNDATE);
            int convertedAmount = Int32.Parse(DRAMOUNT);
            var x = service.postCustomerTransactionsWithArea(OriginalReference, CustNamex, Areax, PhoneNumber, convertedDate, convertedAmount, MyRef, "DIRECTDEBIT", VendorCode, VendorPass);
            if (x.PostError == "NONE")
            {
                var strUpdate = " UPDATE Payments set NWSCStatus = 'Posted' where ReferenceNumber = OriginalReference ";
                try
                {
                    using (

                         var conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["Bridge"].ConnectionString))
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
            }
            else
            {
                var strUpdate = " UPDATE Payments set NWSCStatus = 'UnPosted' where ReferenceNumber = OriginalReference ";
                try
                {
                    using (

                         var conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["Bridge"].ConnectionString))
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

            }
            // notiffy that is wadey for you
            //RESPOND HERE
            WaterDetailsinfo.EXRef = EXREF;
            WaterDetailsinfo.NWSCReferenceNumber = OriginalReference;
            WaterDetailsinfo.PaymentAmount = DRAMOUNT;
            WaterDetailsinfo.WaterDetails = new List<NWSCDetials>();
            WaterDetailsinfo.WaterDetails.Add(new NWSCDetials()
                                                  {
                                                      CustomerName = CustNamex, 
                                                      PropertyRef = PropertyRefx,
                                                      OutstandingBal = OutstandingBalx,
                                                      Responcecode = "00",
                                                      CustomerError = CustRemarks
                                                  });
            return WaterDetailsinfo.EXRef.ToString();
        }

        //======================================================================
    }
    public class NWSCFCDBResponse
    {
        public string EXRef { get; set; }
        public string NWSCReferenceNumber { get; set; }
        public string Area { get; set; }
        public string PaymentAmount { get; set; }
        public List<NWSCDetials> WaterDetails { get; set; }
        public List<NWSCFailureDetials> FailureDetails { get; set; }
    }

    public class NWSCDetials
    {
        public string CustomerName { get; set; }
        public string OutstandingBal { get; set; }
        public string PropertyRef { get; set; }
        public string CustomerError { get; set; }
        public string Responcecode { get; set; }
    }

    public class NWSCFailureDetials
    {
        public string Remark { get; set; }
        public string Responcecode { get; set; }
    }
}