using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Web;
using System.Web.Configuration;

namespace AdaPayInterface
{
    public class Datamanagement
    {
        ////public static string MyAPIUserName =
        ////    getFinpayGlobalDetails("Select ItemValue from Parameters where ItemKey = 'URAWEBUSER'", "ItemValue");

        ////public static string MyAPIPass = getFinpayGlobalDetails(
        ////    "Select ItemValue from Parameters where ItemKey = 'URAWEBPASS'", "ItemValue");

        ////private static X509Certificate2 x509Cert_ura =
        ////    new X509Certificate2(
        ////        HttpContext.Current.Server.MapPath(
        ////            System.Configuration.ConfigurationManager.AppSettings["URACertificate"]));

        ////private X509Certificate2 x509Cert_bnk =
        ////    new X509Certificate2(
        ////        HttpContext.Current.Server.MapPath(
        ////            System.Configuration.ConfigurationManager.AppSettings["BankCertificate"]),
        ////        System.Configuration.ConfigurationManager.AppSettings["BankCertPass"]);

        //////======================================================================================================
        ////public static string LogTaxTransaction(string PRN, DateTime ValueDate, string Maker, string Branch,
        ////                                       string ChequeNo, string CRAccount, string DRAccount, string BankStatus,
        ////                                       string URAStatus, string Tran_Num, string Auto, string Checker,
        ////                                       string BankCode, string BranchCode, string ClearingCode, string TranName,
        ////                                       string Currency, string TranCode, string OtherBankAccount)
        ////{
        ////    try
        ////    {
        ////        DateTime currtime = DateTime.Now;
        ////        Int32 intAuto = 0;

        ////        var sqlconn = new SqlConnection(WebConfigurationManager.ConnectionStrings["Finpay"].ConnectionString);
        ////        sqlconn.Open();
        ////        var sqlcomm = new SqlCommand
        ////                          {
        ////                              Connection = sqlconn,
        ////                              CommandType = CommandType.StoredProcedure,
        ////                              CommandText = "Update_Payment"
        ////                          };

        ////        //Here I am definied command type is Stored Procedure.
        ////        //Here I mentioned the Stored Procedure Name.
        ////        //Here I fix the variable values to Stored Procedure Parameters. You can easily understand if you can see the Stored Procedure Code.
        ////        sqlcomm.Parameters.Add(new SqlParameter("@PRN", SqlDbType.VarChar)).Value = (PRN);
        ////        sqlcomm.Parameters.Add(new SqlParameter("@ValueDate", SqlDbType.DateTime)).Value = (ValueDate);
        ////        sqlcomm.Parameters.Add(new SqlParameter("@Maker", SqlDbType.DateTime)).Value = Maker;
        ////        sqlcomm.Parameters.Add(new SqlParameter("@Branch", SqlDbType.VarChar)).Value = Branch;
        ////        sqlcomm.Parameters.Add(new SqlParameter("@ChequeNo", SqlDbType.VarChar)).Value = ChequeNo;
        ////        sqlcomm.Parameters.Add(new SqlParameter("@CRAccount", SqlDbType.VarChar)).Value = CRAccount;

        ////        sqlcomm.Parameters.Add(new SqlParameter("@DRAccount", SqlDbType.VarChar)).Value = DRAccount;
        ////        sqlcomm.Parameters.Add(new SqlParameter("@BankStatus", SqlDbType.VarChar)).Value = BankStatus;
        ////        sqlcomm.Parameters.Add(new SqlParameter("@URAStatus", SqlDbType.VarChar)).Value = URAStatus;
        ////        sqlcomm.Parameters.Add(new SqlParameter("@Tran_Num", SqlDbType.VarChar)).Value = Tran_Num;
        ////        sqlcomm.Parameters.Add(new SqlParameter("@Auto", SqlDbType.Bit)).Value = intAuto;
        ////        sqlcomm.Parameters.Add(new SqlParameter("@Checker", SqlDbType.VarChar)).Value = Checker;
        ////        sqlcomm.Parameters.Add(new SqlParameter("@BankCode", SqlDbType.VarChar)).Value = BankCode;
        ////        sqlcomm.Parameters.Add(new SqlParameter("@BranchCode", SqlDbType.VarChar)).Value = BranchCode;
        ////        sqlcomm.Parameters.Add(new SqlParameter("@ClearingCode", SqlDbType.VarChar)).Value = ClearingCode;
        ////        sqlcomm.Parameters.Add(new SqlParameter("@TranName", SqlDbType.VarChar)).Value = TranName;
        ////        sqlcomm.Parameters.Add(new SqlParameter("@Currency", SqlDbType.VarChar)).Value = Currency;
        ////        sqlcomm.Parameters.Add(new SqlParameter("@TranCode", SqlDbType.VarChar)).Value = TranCode;
        ////        sqlcomm.Parameters.Add(new SqlParameter("@OtherBankAccount", SqlDbType.VarChar)).Value =
        ////            OtherBankAccount;

        ////        sqlcomm.ExecuteNonQuery();
        ////        sqlcomm.Dispose();
        ////        sqlconn.Close();
        ////        return "";
        ////    }
        ////    catch (Exception ex)
        ////    {
        ////        LogError(ex.Message, "LogTaxTransaction", "Update_Payment");
        ////        return "";
        ////    }
        ////}

        //======================================================================================================
        public static void LogError(string msg, string fx, string caller)
        {
            try
            {
                {
                    string sPath = HttpContext.Current.Server.MapPath("~/logs");
                    if (!Directory.Exists(sPath)) Directory.CreateDirectory(sPath);
                    sPath = Path.Combine(sPath, DateTime.Now.ToString("yyyyMMdd") + ".log");
                    //Error Logging Starts
                    using (var fs = new FileStream(sPath, FileMode.Append, FileAccess.Write))
                    {
                        //using ()
                        var sw = new StreamWriter(fs);
                        sw.WriteLine(DateTime.Now.ToString("yyyyMMdd") + ": Function: " + fx + ": Being Called By: " + caller +
                                         ": Error: " + msg);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        //======================================================================================================
        public static DataTable UpdateRegistration(string prn, string trnname)
        {
            const string strUpdate =
                "UPDATE REGISTRATIONS SET CLEAREDSTATUS=@CLEAREDSTATUS WHERE PRN=@PRN AND TRANNAME=@TRANNAME";
            var result = new DataTable();
            try
            {
                using (
                    var conn =
                        new SqlConnection(WebConfigurationManager.ConnectionStrings["Finpay"].ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(strUpdate, conn))
                    {
                        cmd.Parameters.Add("@CLEAREDSTATUS", SqlDbType.Bit).Value = false;
                        cmd.Parameters.Add("@PRN", SqlDbType.VarChar).Value = prn;
                        cmd.Parameters.Add("@TRANNAME", SqlDbType.VarChar).Value = trnname;
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            result.Load(dr);
                        }
                    }
                }
            }

            catch (Exception ex)
            {
                LogError(ex.Message, "UpdateRegistration", "UPDATE REGISTRATIONS");
            }
            return result;
        }

        //====================================================================================================== 
        public static Boolean isalreadyPosted(string PRN)
        {
            try
            {
                var Posted = false;
                using (var dtlog = Datamanagement.FinbridgeAlreadyLogPosted(PRN))
                {
                    if (dtlog.Rows.Count == 0)
                    {
                        using (var dtlogd = Datamanagement.FinbridgeAlreadyLogdPosted(PRN))
                        {
                            if (dtlogd.Rows.Count == 0)
                            {
                                Posted = false;
                            }
                            else
                            {
                                Posted = true;
                            }
                        }
                    }
                    else
                    {
                        Posted = true;
                    }
                }
                return Posted;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "isalreadyPosted", "Check Transaction status");
                return false;
            }
        }

        //====================================================================================================== 
        public static Boolean isSuccessful(string PRN)
        {
            try
            {
                var FlexResp = "";
                var Successful = false;
                using (var dtlog = Datamanagement.LogTransactionstatus(PRN))
                {
                    if (dtlog.Rows.Count == 0)
                    {
                        Successful = false;
                    }
                    else
                    {
                        foreach (DataRow row in dtlog.Rows) // Loop over the rows.
                        {
                            FlexResp = row["mbresponse"].ToString();
                            switch (FlexResp)
                            {
                                case "00":
                                    Successful = true;
                                    break;
                                case "05":
                                    Successful = false;
                                    break;
                                default:
                                    Successful = false;
                                    break;
                            }
                        }
                    }
                }
                return Successful;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "isSuccessful", "Check Transaction status");
                return false;
            }
        }


        //====================================================================================================== 

        public static string coin_(string PRN)
        {
            try
            {
                var FlexResp = "";

                using (var dtlog = Datamanagement.COIN(PRN))
                {
                    if (dtlog.Rows.Count == 0)
                    {
                        return "";
                    }
                    else
                    {
                        foreach (DataRow row in dtlog.Rows) // Loop over the rows.
                        {
                            FlexResp = row["COIN"].ToString();
                            
                        }
                    }
                }
                return FlexResp;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "coin_", "Check Transaction COIN for " + PRN);
                return "";
            }
        }
        //********
        public static string Ecternalref(string PRN)
        {
            try
            {
                var FlexResp = "";
                //var Successful = false;
                using (var dtlog = Datamanagement.LogTransactionstatus(PRN))
                {
                    if (dtlog.Rows.Count == 0)
                    {
                        //Successful = false;
                        return FlexResp;
                    }
                    else
                    {
                        foreach (DataRow row in dtlog.Rows) // Loop over the rows.
                        {
                           
                            switch (row["mbresponse"].ToString())
                            {
                                case "00":
                                    FlexResp = row["EndCheck"].ToString();
                                    break;
                                case "05":
                                    FlexResp = row["EndCheck"].ToString();
                                    break;
                                default:
                                    FlexResp = "";
                                    break;
                            }
                        }
                    }
                }
                return FlexResp;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "isSuccessful", "Check Transaction status");
                //return FlexResp;
                return "";
            }
        }
        //*******
        public static DataTable LogTransactionstatus(string PRN)
        {
            PRN = PRN.Trim();
            const string strQuery = "SELECT * FROM log WHERE msgid = @PRN";
            var result = new DataTable();
            try
            {
                using (
                    var conn =
                        new SqlConnection(WebConfigurationManager.ConnectionStrings["Bridge"].ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(strQuery, conn))
                    {
                        cmd.Parameters.Add("@PRN", SqlDbType.VarChar).Value = PRN;
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            result.Load(dr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "LogTransactionstatus", "Check log status");
            }
            return result;
        }
        //**************
        public static DataTable COIN(string PRN)
        {
            PRN = PRN.Trim();
            const string strQuery = "select * from KCCARequestDetails WHERE PRN = @PRN";
            var result = new DataTable();
            try
            {
                using (
                    var conn =
                        new SqlConnection(WebConfigurationManager.ConnectionStrings["AdaPay"].ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(strQuery, conn))
                    {
                        cmd.Parameters.Add("@PRN", SqlDbType.VarChar).Value = PRN;
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            result.Load(dr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "LogTransactionstatus", "Check log status");
            }
            return result;
        }
        //====================================================================================================== 
        public static Boolean isalreadyTransacted(string PRN)
        {
            try
            {
                var Transacted = false;
                using (var dtlog = Datamanagement.PRNAlreadyTransacted(PRN))
                {
                    if (dtlog.Rows.Count == 0)
                    {
                        Transacted = false;
                    }
                    else
                    {
                        Transacted = true;
                    }
                }
                return Transacted;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "isalreadyTransacted", "Check PRN status");
                return false;
            }
        }


        public static Boolean isalreadyTransactedKCCA(string PRN)
        {
            try
            {
                var Transacted = false;
                using (var dtlog = Datamanagement.PRNAlreadyTransactedKCCA(PRN))
                {
                    if (dtlog.Rows.Count == 0)
                    {
                        Transacted = false;
                    }
                    else
                    {
                        Transacted = true;
                    }
                }
                return Transacted;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "isalreadyTransacted", "Check PRN status");
                return false;
            }
        }
        //public enum Products
        //{
        //    Tax = 0,
        //    Water = 1,
        //    Electricity = 2,
        //    CityCouncil = 3,
        //    University = 4,
        //    PayTv = 5,
        //    NSSF = 6
        //};

        //====================================================================================================== 
        public static DataTable PRNAlreadyTransacted(string PRN)
        {
            PRN = PRN.Trim();
            const string strQuery =
                "SELECT TAXPAYERNAME,AMOUNT,PRDATE,STATUS,BANKSTATUS,TIN FROM VW_URA_PAYMENT_REGISTRATIONS WHERE PRN = @PRN and  status=@stat";
            var result = new DataTable();
            try
            {
                using (
                    var conn =
                        new SqlConnection(WebConfigurationManager.ConnectionStrings["Finpay"].ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(strQuery, conn))
                    {
                        cmd.Parameters.Add("@PRN", SqlDbType.VarChar).Value = PRN;
                        cmd.Parameters.Add("@stat", SqlDbType.VarChar).Value = "T";
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            result.Load(dr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "PRNAlreadyTransacted", "Check Registrations");
            }
            return result;
        }

        //====================================================================================================== 
        public static DataTable PRNAlreadyTransactedKCCA(string PRN)
        {
            PRN = PRN.Trim();
            const string strQuery =
                "SELECT * from KCCAPAYMENTS WHERE PRN = @PRN and  status=@stat";
            var result = new DataTable();
            try
            {
                using (
                    var conn =
                        new SqlConnection(WebConfigurationManager.ConnectionStrings["AdaPay"].ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(strQuery, conn))
                    {
                        cmd.Parameters.Add("@PRN", SqlDbType.VarChar).Value = PRN;
                        cmd.Parameters.Add("@stat", SqlDbType.VarChar).Value = "T";
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            result.Load(dr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "PRNAlreadyTransacted", "Check Registrations");
            }
            return result;
        }

        //====================================================================================================== 
        public static Boolean isalreadyExisting(string PRN)
        {
            try
            {
                var existing = false;
                using (var dtlog = Datamanagement.PRNAlreadyEXISTING(PRN))
                {
                    if (dtlog.Rows.Count == 0)
                    {
                        existing = false;
                    }
                    else
                    {
                        existing = true;
                    }
                }
                return existing;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "isalreadyTransacted", "Check PRN status");
                return false;
            }
        }

        //====================================================================================================== 
        public static DataTable PRNAlreadyEXISTING(string PRN)
        {
            PRN = PRN.Trim();
            const string strQuery = "SELECT * FROM REGISTRATIONS WHERE PRN = @PRN";
            var result = new DataTable();
            try
            {
                using (
                    var conn =
                        new SqlConnection(WebConfigurationManager.ConnectionStrings["Finpay"].ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(strQuery, conn))
                    {
                        cmd.Parameters.Add("@PRN", SqlDbType.VarChar).Value = PRN;
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            result.Load(dr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "PRNAlreadyEXISTING", "Check Registrations");
            }
            return result;
        }

        //====================================================================================================== 
        public static DataTable FinbridgeAlreadyLogPosted(string PRN)
        {
            PRN = PRN.Trim() + "TAX";
            const string strQuery = "select * from LOG where MSGID = @msgid";
            var result = new DataTable();
            try
            {
                using (
                    var conn =
                        new SqlConnection(WebConfigurationManager.ConnectionStrings["Bridge"].ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(strQuery, conn))
                    {
                        cmd.Parameters.Add("@msgid", SqlDbType.VarChar).Value = PRN;
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            result.Load(dr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "FinbridgeAlreadyLogPosted", "Check Log");
            }
            return result;
        }

        //====================================================================================================== 
        public static DataTable FinbridgeAlreadyLogdPosted(string PRN)
        {
            PRN = PRN.Trim() + "TAX";
            const string strQuery = "select * from LOGD where MSGID = @msgid";
            var result = new DataTable();
            try
            {
                using (
                    var conn =
                        new SqlConnection(WebConfigurationManager.ConnectionStrings["Bridge"].ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(strQuery, conn))
                    {
                        cmd.Parameters.Add("@msgid", SqlDbType.VarChar).Value = PRN;
                        using (var dr = cmd.ExecuteReader())
                        {
                            result.Load(dr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "FinbridgeAlreadyLogdPosted", "Check Logd");
            }
            return result;
        }

        //====================================================================================================== 
        public static DataTable GetFCDBSettings(string FCDBMsgtype)
        {
            const string strQuery = "select * from FCDB_SETTINGS where FCDBMSGTYPE = @FCDBMsgtype";
            var result = new DataTable();
            try
            {
                using (
                    var conn =
                        new SqlConnection(WebConfigurationManager.ConnectionStrings["Bridge"].ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(strQuery, conn))
                    {
                        cmd.Parameters.Add("@FCDBMsgtype", SqlDbType.VarChar).Value = FCDBMsgtype;
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            result.Load(dr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "GetFCDBSettings", "Check FCDB settings");
            }
            return result;
        }

        //======================================================================================================
        public static string getFCDBSettings(string msgtype)
        {
            try
            {
                string fcdbsettings = "";
                using (DataTable dt = Datamanagement.GetFCDBSettings(msgtype))
                {
                    if (dt.Rows.Count == 0)
                    {
                    }
                    else
                    {
                        //
                        foreach (DataRow row in dt.Rows) // Loop over the rows.
                        {
                            fcdbsettings = row["FCDBINTERNALGL"].ToString();
                            fcdbsettings = fcdbsettings + "|" + row["FCDBTELLERCODE"];
                            fcdbsettings = fcdbsettings + "|" + row["FCDBDESC"];
                            fcdbsettings = fcdbsettings + "|" + row["FCDBTILLID"];
                            fcdbsettings = fcdbsettings + "|" + row["FCDBLIMIT"];

                            fcdbsettings = fcdbsettings + "|" + row["FCDBMAKER"];
                            fcdbsettings = fcdbsettings + "|" + row["FCDBCHECKER"];
                            fcdbsettings = fcdbsettings + "|" + row["Subhost"];
                            fcdbsettings = fcdbsettings + "|" + row["Msgtype"];
                            fcdbsettings = fcdbsettings + "|" + row["Procode"];
                            fcdbsettings = fcdbsettings + "|" + row["Commission"];
                            fcdbsettings = fcdbsettings + "|" + row["Trantype"];
                            fcdbsettings = fcdbsettings + "|" + row["Terminal"];
                            fcdbsettings = fcdbsettings + "|" + row["Batch"];
                            fcdbsettings = fcdbsettings + "|" + row["MessageFlag"];
                            fcdbsettings = fcdbsettings + "|" + row["Workstation"];
                            fcdbsettings = fcdbsettings + "|" + row["PosConfirmed"];
                            fcdbsettings = fcdbsettings + "|" + row["PosReversed"];
                            fcdbsettings = fcdbsettings + "|" + row["EftSource"];
                            fcdbsettings = fcdbsettings + "|" + row["IbTxn"];
                            fcdbsettings = fcdbsettings + "|" + row["Flexuserid"];
                            fcdbsettings = fcdbsettings + "|" + row["UtilityAccount"];
                            fcdbsettings = fcdbsettings + "|" + row["UtilityAccBranch"];
                            fcdbsettings = fcdbsettings + "|" + row["UtilityAccCurrency"];
                            fcdbsettings = fcdbsettings + "|" + row["MerchantType"];
                        }
                    }
                }
                return fcdbsettings;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "getFCDBSettings", "Check FCDB settings");
                return "";
            }
        }

        //====================================================================================================== 
        public static string getCustRefDetails(string CustRef)
        {
            try
                
            {
                string CustDetails = "";
                using (DataTable dt = Datamanagement.GetCUSTRef(CustRef))
                {
                    if (dt.Rows.Count == 0)
                    {
                    }
                    else
                    {
                        //
                        foreach (DataRow row in dt.Rows) // Loop over the rows.
                        {
                            CustDetails = row["CustomerRef"].ToString();
                            CustDetails = CustDetails + "|" + row["CustomerName"];
                            CustDetails = CustDetails + "|" + row["CustomerType"];
                            CustDetails = CustDetails + "|" + row["Balance"];
                            CustDetails = CustDetails + "|" + row["StatusCode"];
                            CustDetails = CustDetails + "|" + row["StatusDescription"];
                        }
                    }
                }
                return CustDetails;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "getCustRefDetails", "Check cust reference details");
                return "";
            }
        }
        public static string getKCCACustRefDetails(string CustRef)
        {
            try
            {
                string CustDetails = "";
                using (DataTable dt = Datamanagement.GetKCCACUSTRef(CustRef))
                {
                    if (dt.Rows.Count == 0)
                    {
                    }
                    else
                    {
                        //
                        foreach (DataRow row in dt.Rows) // Loop over the rows.
                        {
                            CustDetails = row["PRN"].ToString();
                            CustDetails = CustDetails + "|" + row["CustomerName"];
                            CustDetails = CustDetails + "|" + row["COIN"];
                            CustDetails = CustDetails + "|" + row["AMOUNTDUE"];
                            CustDetails = CustDetails + "|" + row["Status"];
                            CustDetails = CustDetails + "|" + row["PHONENUMBER"];
                        }
                    }
                }
                return CustDetails;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "getCustRefDetails", "Check cust reference details");
                return "";
            }
        }

        //====================================================================================================== 
        public static string getUmemeCustRefDetails(string CustRef)
        {
            try
            {
                string CustDetails = "";
                using (DataTable dt = Datamanagement.GetUmemeCUSTRef(CustRef))
                {
                    if (dt.Rows.Count == 0)
                    {
                    }
                    else
                    {
                        //
                        foreach (DataRow row in dt.Rows) // Loop over the rows.
                        {
                            CustDetails = row["CustomerRef"].ToString();
                            //CustDetails = CustDetails + "|" + row["Area"];
                            CustDetails = CustDetails + "|" + row["Balance"];
                            CustDetails = CustDetails + "|" + row["CustomerName"];
                            CustDetails = CustDetails + "|" + row["StatusCode"];
                            CustDetails = CustDetails + "|" + row["StatusDescription"];
                        }
                    }
                }
                return CustDetails;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "getUmemeCustRefDetails", "Check Umeme cust reference details");
                return "";
            }
        }
        //====================================================================================================== 
        public static string getWaterCustRefDetails(string CustRef)
        {
            try
            {
                string CustDetails = "";
                using (DataTable dt = Datamanagement.GetWaterCUSTRef(CustRef))
                {
                    if (dt.Rows.Count == 0)
                    {
                    }
                    else
                    {
                        //
                        foreach (DataRow row in dt.Rows) // Loop over the rows.
                        {
                            CustDetails = row["ReferenceNumber"].ToString();
                            CustDetails = CustDetails + "|" + row["Area"];
                            CustDetails = CustDetails + "|" + row["OutstandingBal"];
                            CustDetails = CustDetails + "|" + row["CustName"];
                            CustDetails = CustDetails + "|" + row["PropertyRef"];
                            CustDetails = CustDetails + "|" + row["CustomerError"];
                        }
                    }
                }
                return CustDetails;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "getWaterCustRefDetails", "Check Water cust reference details");
                return "";
            }
        }

        //======================================================================================================
        public static DataTable GetWaterCUSTRef(string CustReference)
        {
            const string strQuery = "select * from NWCSRequestDetails where ReferenceNumber=@cust and date=@refDate";
            var result = new DataTable();
            try
            {
                using (
                    var conn =
                        new SqlConnection(WebConfigurationManager.ConnectionStrings["Bridge"].ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(strQuery, conn))
                    {
                        cmd.Parameters.Add("@cust", SqlDbType.VarChar).Value = CustReference;
                        cmd.Parameters.Add("@refDate", SqlDbType.VarChar).Value =
                            DateTime.Now.ToString("yyMMdd");
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            result.Load(dr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "GetWaterCUSTRef", "Check water reference Details");
            }
            return result;
        }

        //======================================================================================================
        public static DataTable GetCUSTRef(string CustReference)
        {
            const string strQuery = "select * from UmemeRequestDetails where CustomerRef =@cust and date = @refDate";
            var result = new DataTable();
            try
            {
                using (
                    var conn =
                        new SqlConnection(WebConfigurationManager.ConnectionStrings["Bridge"].ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(strQuery, conn))
                    {
                        cmd.Parameters.Add("@cust", SqlDbType.VarChar).Value = CustReference;
                        cmd.Parameters.Add("@refDate", SqlDbType.VarChar).Value =
                            DateTime.Now.ToString("yyMMdd");
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            result.Load(dr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "GetCUSTRef", "Check reference Details");
            }
            return result;
        }
        public static DataTable GetKCCACUSTRef(string CustReference)
        {
            const string strQuery = "select * from KCCARequestDetails where PRN = CustReference ";
            var result = new DataTable();
            try
            {
                using (
                    var conn =
                        new SqlConnection(WebConfigurationManager.ConnectionStrings["AdaPay"].ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(strQuery, conn))
                    {
                        cmd.Parameters.Add("@cust", SqlDbType.VarChar).Value = CustReference;
                        cmd.Parameters.Add("@refDate", SqlDbType.VarChar).Value =
                            DateTime.Now.ToString("yyMMdd");
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            result.Load(dr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "GetKCCACUSTRef", "Check reference Details");
            }
            return result;
        }
        //++++++++++++++++++++++++++++++++++++++++++
        public static DataTable GetUmemeCUSTRef(string CustReference)
        {
            const string strQuery = "select * from UmemeRequestDetails where CustomerRef =@cust and date = @refDate";
            var result = new DataTable();
            try
            {
                using (
                    var conn =
                        new SqlConnection(WebConfigurationManager.ConnectionStrings["Bridge"].ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(strQuery, conn))
                    {
                        cmd.Parameters.Add("@cust", SqlDbType.VarChar).Value = CustReference;
                        cmd.Parameters.Add("@refDate", SqlDbType.VarChar).Value =
                            DateTime.Now.ToString("yyMMdd");
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            result.Load(dr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "GetUmemeCUSTRef", "Check reference Details");
            }
            return result;
        }
        //====================================================================================================== 
        public static string getPRNDetails(string PRN)
        {
            try
            {
                string PrnDetails = "";
                using (DataTable dt = Datamanagement.GetPRN(PRN))
                {
                    if (dt.Rows.Count == 0)
                    {
                    }
                    else
                    {
                        //
                        foreach (DataRow row in dt.Rows) // Loop over the rows.
                        {
                            PrnDetails = row["TIN"].ToString();
                            PrnDetails = PrnDetails + "|" + row["TaxPayername"];
                            PrnDetails = PrnDetails + "|" + row["amountdue"];
                            PrnDetails = PrnDetails + "|" + row["ExpDate"];
                            PrnDetails = PrnDetails + "|" + row["RegDate"];
                            PrnDetails = PrnDetails + "|" + row["Remark"];
                            PrnDetails = PrnDetails + "|" + row["status"];
                        }
                    }
                }
                return PrnDetails;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "getPRNDetails", "Check PRN details");
                return "";
            }
        }

        //====================================================================================================== 
        public static DataTable GetPRN(string PRN)
        {
            const string strQuery = "select * from URARequestDetails where prn =@PRN and date = @PrnDate";
            var result = new DataTable();
            try
            {
                using (
                    var conn =
                        new SqlConnection(WebConfigurationManager.ConnectionStrings["Bridge"].ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(strQuery, conn))
                    {
                        cmd.Parameters.Add("@PRN", SqlDbType.VarChar).Value = PRN;
                        cmd.Parameters.Add("@PrnDate", SqlDbType.VarChar).Value =
                            DateTime.Now.ToString("yyMMdd");
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            result.Load(dr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "GetPRN", "Check PRN Details");
            }
            return result;
        }

        //======================================================================================================
        public static DataTable InsertIntoLog(string MSGID, string SUBHOST, string MSGTYPE, string PROCODE, string DATE,
                                              string Time, string DESTBRANCH, string seqno, string REFERENCE,
                                              string AMOUNT, string COMMISSION, string ACCOUNT, string TRANTYPE,
                                              string UNIQUEID, string Terminal, string ACQUINST, string BATCH,
                                              string CARDTERMINAL, string RESERVED, string PAN, string CURRCODE,
                                              string TRIALS, string MESSAGEFLAG, string WORKSTATION, string POSCONFIRMED,
                                              string POSREVERSED, string OFFLINETXN, string ACCOUNT2, string Branch2,
                                              string MerchantType, string EFTSource, string IBTXN, string CLEARINGDATA,
                                              string FlexUserID)
        {
            var strUpdate =
                "INSERT INTO LOG(MSGID, SUBHOST, MSGTYPE , PROCODE, AUTHORISED, DATE, TIME, DESTBRANCH, FORCEPOST, [SEQ NO], POSTED, REFERENCE, AMOUNT, COMMISSION, ACCOUNT, TRANTYPE, UNIQUEID, ATM, ACQUINST, BATCH, CARDTERMINAL, RESERVED, PAN, CURRCODE, TRIALS, RESPONDED, PROCESSED, MESSAGEFLAG, PASSEDHOST, CHECKEDBALANCE, WORKSTATION, POSCONFIRMED, POSREVERSED, OFFLINETXN, ACCOUNT2, Branch2, MerchantType, EFTSource, IBTXN,message) VALUES('" +
                MSGID + "', '" + SUBHOST + "', '" + MSGTYPE + "',  '" + PROCODE + "', 0, '" + DATE + "', '" + Time +
                "', '" + DESTBRANCH + "', 0, '" + Time + "', 0, '" + REFERENCE + "', '" + AMOUNT + "', 0, '" + ACCOUNT +
                "', '" + TRANTYPE + "', '" + FlexUserID + "|" + FlexUserID + "', '" + Terminal + "', '" + DESTBRANCH +
                "', '" + BATCH + "', '" + CARDTERMINAL + "', '" + RESERVED + "', '" + PAN + "', '" + CURRCODE + "',  '" +
                TRIALS + "', 0,0, '" + MESSAGEFLAG + "', '0', '0', '" + WORKSTATION + "', '" + POSCONFIRMED + "','" +
                POSREVERSED + "', '" + OFFLINETXN + "', '" + ACCOUNT2 + "', '" + Branch2 + "', '" + MerchantType + "','" +
                EFTSource + "','" + IBTXN + "','" + CLEARINGDATA + "')";
           try
           {
                    var conn =
                   new SqlConnection(WebConfigurationManager.ConnectionStrings["Bridge"].ConnectionString);
                    conn.Open();
                    var cmd = new SqlCommand(strUpdate, conn);
                    cmd.ExecuteReader();
               
            }

            catch (Exception ex)
            {
                LogError(ex.Message, "InsertIntoLog", "INSERT TRN TO BRIDGE");
            }
            return null;
        }

        //======================================================================================================
        public static DataTable InsertIntoBridgeRegistration(string PRN, string BankStatus, string Tin, string Amount,
                                                             string Paid_dt,
                                                             string Value_dt, string Status, string Bank_branch_cd,
                                                             string Bank_tr_no,
                                                             string Chq_no, string Reason)
        {
            var strUpdate =
                "INSERT INTO UraNotification(PRN, BankStatus, Tin , Amount, Paid_dt, Value_dt, Status, Bank_branch_cd, Bank_tr_no, Chq_no, Reason) VALUES('" +
                PRN + "', '" + BankStatus + "', '" + Tin + "',  '" + Amount + "',  '" + Paid_dt + "', '" + Value_dt +
                "', '" + Status + "',  '" + Bank_branch_cd + "', '" + Bank_tr_no + "', '" + Chq_no + "',  '" + Reason +
                "')";
            var result = new DataTable();
            try
            {
                using (
                    var conn =
                        new SqlConnection(WebConfigurationManager.ConnectionStrings["Bridge"].ConnectionString))
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
                LogError(ex.Message, "InsertIntoBridgeRegistration", "INSERT BRIDGE REGISTRATION");
            }
            return result;
        }

        //======================================================================================================
        public static DataTable UpdateSentRegistration(string prn, String auth)
        {
            const string strUpdate =
                "UPDATE REGISTRATIONS SET AUTHORIZED=@AUTHORIZED,UPLOADED=@UPLOADED,SENDTOBOU=@SENDTOBOU,CHECKER=@CHECKER WHERE PRN =@PRN";
            var result = new DataTable();
            try
            {
                using (
                    var conn =
                        new SqlConnection(WebConfigurationManager.ConnectionStrings["Finpay"].ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(strUpdate, conn))
                    {
                        cmd.Parameters.Add("@AUTHORIZED", SqlDbType.Bit).Value = true;
                        cmd.Parameters.Add("@UPLOADED", SqlDbType.Bit).Value = true;
                        cmd.Parameters.Add("@SENDTOBOU", SqlDbType.Bit).Value = false;
                        cmd.Parameters.Add("@CHECKER", SqlDbType.VarChar).Value = auth;
                        cmd.Parameters.Add("@PRN", SqlDbType.VarChar).Value = prn;
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            result.Load(dr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "UpdateSentRegistration", "UPDATE UPLOADED REGISTRATIONS");
            }
            return result;
        }

        //====================================================================================================== 
        public static string DeleteUploadedData(string prn)
        {
            var strUpdate =
                "DELETE FROM UPLOADDATA WHERE NARRATION LIKE '" + prn + "%'";

            try
            {
                using (
                    var conn =
                        new SqlConnection(WebConfigurationManager.ConnectionStrings["Finpay"].ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(strUpdate, conn))
                    {
                        cmd.ExecuteReader();
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "DeleteUploadedData", "DELETE UPLOADED DATA");
            }
            return null;
        }

        //====================================================================================================== 
        public static string DeleteFromFinpay(string DeleteQuery)
        {
            try
            {
                using (
                    var conn =
                        new SqlConnection(WebConfigurationManager.ConnectionStrings["Finpay"].ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(DeleteQuery, conn))
                    {
                        cmd.ExecuteReader();
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "DeleteFromFinpay", "DELETE DATA FROM FINPAY");
            }
            return null;
        }

        //====================================================================================================== 
        public static DataTable UpdateFailedUpload(string prn, string CHECKER, string MAKER)
        {
            const string strUpdate =
                "UPDATE REGISTRATIONS SET URASTATUS =@URASTATUS,CHECKER=@CHECKER,UPLOADED=@UPLOADED,BRANCH=@BRANCH,MAKER=@MAKER WHERE PRN = @PRN";
            var result = new DataTable();
            try
            {
                using (
                    var conn =
                        new SqlConnection(WebConfigurationManager.ConnectionStrings["Finpay"].ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(strUpdate, conn))
                    {
                        cmd.Parameters.Add("@URASTATUS", SqlDbType.VarChar).Value = "A";
                        cmd.Parameters.Add("@CHECKER", SqlDbType.VarChar).Value = CHECKER;
                        cmd.Parameters.Add("@UPLOADED", SqlDbType.Bit).Value = false;
                        cmd.Parameters.Add("@BRANCH", SqlDbType.VarChar).Value = null;
                        cmd.Parameters.Add("@MAKER", SqlDbType.VarChar).Value = CHECKER;
                        cmd.Parameters.Add("@PRN", SqlDbType.VarChar).Value = prn;
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            result.Load(dr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "UpdateFailedUpload", "UPDATE FAILED REGISTRATIONS");
            }
            return result;
        }

        //====================================================================================================== 
        public static string PushRegistrationDetails(string PRN)
        {
            var strInsert =
                "INSERT INTO URARequestDetails(PRN,[Date]) VALUES('" +
                PRN + "','" + DateTime.Now.ToString("yyMMdd") + "')";
            try
            {
                using (
                    var conn =
                        new SqlConnection(WebConfigurationManager.ConnectionStrings["Bridge"].ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(strInsert, conn))
                    {
                        var dr = cmd.ExecuteReader();
                    }
                }
            }

            catch (Exception ex)
            {
                LogError(ex.Message, "PullRegistrationDetails", "INSERT TRN TO BRIDGE");
            }
            return "";
        }

        //====================================================================================================== 


        public static string UpdateDetails(string CustomerRef, string CustomerName, string CustomerType, string Balance,
                                              string StatusCode, string StatusDescription, string remark, Boolean auth)
        {
            try
            {
                var sqlconn = new SqlConnection(WebConfigurationManager.ConnectionStrings["Bridge"].ConnectionString);
                sqlconn.Open();
                var sqlcomm = new SqlCommand
                {
                    Connection = sqlconn,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "sp_UpdateUMEMEDetails"
                };
                //Here I am definied command type is Stored Procedure.
                //Here I mentioned the Stored Procedure Name.
                //Here I fix the variable values to Stored Procedure Parameters. You can easily understand if you can see the Stored Procedure Code.
                sqlcomm.Parameters.Add(new SqlParameter("@CustomerRef", SqlDbType.VarChar)).Value = (CustomerRef);
                sqlcomm.Parameters.Add(new SqlParameter("@CustomerName", SqlDbType.VarChar)).Value = (CustomerName);
                sqlcomm.Parameters.Add(new SqlParameter("@CustomerType", SqlDbType.VarChar)).Value = CustomerType;
                sqlcomm.Parameters.Add(new SqlParameter("@Balance", SqlDbType.VarChar)).Value = Balance;
                sqlcomm.Parameters.Add(new SqlParameter("@StatusCode", SqlDbType.VarChar)).Value = StatusCode;
                sqlcomm.Parameters.Add(new SqlParameter("@StatusDescription", SqlDbType.VarChar)).Value = StatusDescription;
                sqlcomm.Parameters.Add(new SqlParameter("@remark", SqlDbType.VarChar)).Value = remark;
                sqlcomm.Parameters.Add(new SqlParameter("@Authorised", SqlDbType.Bit)).Value = auth;
                sqlcomm.ExecuteNonQuery();
                sqlcomm.Dispose();
                sqlconn.Close();
                return "";
            }
            catch (Exception ex)
            {
                //LogErrors("UpdateUMEMEDetails" + " :" + ex.Message);
                return "";
            }
        }
        //**************
        public static string UpdateUMEMEDetails(string CustomerRef, string CustomerName, string CustomerType, string Balance,
                                              string StatusCode, string StatusDescription, string remark, Boolean auth)
        {
            try
            {
                var sqlconn = new SqlConnection(WebConfigurationManager.ConnectionStrings["Bridge"].ConnectionString);
                sqlconn.Open();
                var sqlcomm = new SqlCommand
                {
                    Connection = sqlconn,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "sp_UpdateUMEMEDetails"
                };
                //Here I am definied command type is Stored Procedure.
                //Here I mentioned the Stored Procedure Name.
                //Here I fix the variable values to Stored Procedure Parameters. You can easily understand if you can see the Stored Procedure Code.
                sqlcomm.Parameters.Add(new SqlParameter("@CustomerRef", SqlDbType.VarChar)).Value = (CustomerRef);
                sqlcomm.Parameters.Add(new SqlParameter("@CustomerName", SqlDbType.VarChar)).Value = (CustomerName);
                sqlcomm.Parameters.Add(new SqlParameter("@CustomerType", SqlDbType.VarChar)).Value = CustomerType;
                sqlcomm.Parameters.Add(new SqlParameter("@Balance", SqlDbType.VarChar)).Value = Balance;
                sqlcomm.Parameters.Add(new SqlParameter("@StatusCode", SqlDbType.VarChar)).Value = StatusCode;
                sqlcomm.Parameters.Add(new SqlParameter("@StatusDescription", SqlDbType.VarChar)).Value = StatusDescription;
                sqlcomm.Parameters.Add(new SqlParameter("@remark", SqlDbType.VarChar)).Value = remark;
                sqlcomm.Parameters.Add(new SqlParameter("@Authorised", SqlDbType.Bit)).Value = auth;
                sqlcomm.ExecuteNonQuery();
                sqlcomm.Dispose();
                sqlconn.Close();
                return "";
            }
            catch (Exception ex)
            {
                //LogErrors("UpdateUMEMEDetails" + " :" + ex.Message);
                return "";
            }
        }
        //***************
        public static string UpdateURARegDetails(string PRN, string TaxPayername, string amountdue, string TIN,
                                                string ExpDate, string RegDate, string remark, Boolean auth, string statuscode)
        {
            try
            {
                var sqlconn = new SqlConnection(WebConfigurationManager.ConnectionStrings["Bridge"].ConnectionString);
                sqlconn.Open();
                var sqlcomm = new SqlCommand
                {
                    Connection = sqlconn,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "sp_UpdateURADetails"
                };
                //Here I am definied command type is Stored Procedure.
                //Here I mentioned the Stored Procedure Name.
                //Here I fix the variable values to Stored Procedure Parameters. You can easily understand if you can see the Stored Procedure Code.
                sqlcomm.Parameters.Add(new SqlParameter("@PRN", SqlDbType.VarChar)).Value = (PRN);
                sqlcomm.Parameters.Add(new SqlParameter("@TaxPayername", SqlDbType.VarChar)).Value = (TaxPayername);
                sqlcomm.Parameters.Add(new SqlParameter("@amountdue", SqlDbType.VarChar)).Value = amountdue;
                sqlcomm.Parameters.Add(new SqlParameter("@TIN", SqlDbType.VarChar)).Value = TIN;
                sqlcomm.Parameters.Add(new SqlParameter("@ExpDate", SqlDbType.VarChar)).Value = ExpDate;
                sqlcomm.Parameters.Add(new SqlParameter("@RegDate", SqlDbType.VarChar)).Value = RegDate;
                sqlcomm.Parameters.Add(new SqlParameter("@Remark", SqlDbType.VarChar)).Value = remark;
                sqlcomm.Parameters.Add(new SqlParameter("@statuscode", SqlDbType.VarChar)).Value = statuscode;
                sqlcomm.Parameters.Add(new SqlParameter("@Authorised", SqlDbType.Bit)).Value = auth;
                sqlcomm.ExecuteNonQuery();
                sqlcomm.Dispose();
                sqlconn.Close();
                return "";
            }
            catch (Exception ex)
            {
                //LogErrors("UpdateURARegDetails" + " :" + ex.Message);
                return "";
            }
        }
        //***************
        public static string PushCustReferenceDetails(string CustNumber)
        {
            var strInsert =
                "INSERT INTO KCCARequestDetails(CustomerRef,[Date]) VALUES('" +
                CustNumber + "','" + DateTime.Now.ToString("yyMMdd") + "')";
            try
            {
                using (
                    var conn =
                        new SqlConnection(WebConfigurationManager.ConnectionStrings["Bridge"].ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(strInsert, conn))
                    {
                        var dr = cmd.ExecuteReader();
                    }
                }
            }

            catch (Exception ex)
            {
                LogError(ex.Message, "PushCustReferenceDetails", "INSERT cust Ref TO BRIDGE");
            }
            return "";
        }
        public static string PushKCCACustReferenceDetails(string PRN, string COIN, string CUSTOMERNAME, string PHONENUMBER,string PRNDATE,string EXPIRYDATE,string AMOUNTDUE,string PAYMENTCURRENCY,string STATUS)
        {
            var strInsert =
                "INSERT INTO KCCARequestDetails(PRN,COIN,CUSTOMERNAME,PHONENUMBER,PRNDATE,EXPIRYDATE, AMOUNTDUE, PAYMENTCURRENCY, [DATE],STATUS) VALUES('" +
                PRN + "', '" + COIN + "', '" + CUSTOMERNAME + "', '" + PHONENUMBER + "' ,'" + PRNDATE + "', '" + EXPIRYDATE + "', '" + AMOUNTDUE + "' , '" + PAYMENTCURRENCY + "', '" + DateTime.Now.ToString("yyMMdd") + "' , '" + STATUS  + "')";
            try
            {
                using (
                    var conn =
                        new SqlConnection(WebConfigurationManager.ConnectionStrings["AdaPay"].ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(strInsert, conn))
                    {
                        var dr = cmd.ExecuteReader();
                    }
                }
            }

            catch (Exception ex)
            {
                LogError(ex.Message, "PushCustReferenceDetails", "INSERT cust Ref TO BRIDGE");
            }
            return "";
        }

        //
        public static string PushUmemeCustReferenceDetails(string CustNumber)
        {
            var strInsert =
                "INSERT INTO UmemeRequestDetails(CustomerRef,[Date]) VALUES('" +
                CustNumber + "','" + DateTime.Now.ToString("yyMMdd") + "')";
            try
            {
                using (
                    var conn =
                        new SqlConnection(WebConfigurationManager.ConnectionStrings["Bridge"].ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(strInsert, conn))
                    {
                        var dr = cmd.ExecuteReader();
                    }
                }
            }

            catch (Exception ex)
            {
                LogError(ex.Message, "PushUmemeCustReferenceDetails", "INSERT Umeme cust Ref TO BRIDGE");
            }
            return "";
        }
        //
        //====================================================================================================== 
        public static string PushWaterCustReferenceDetails(string CustNumber, string Area)
        {
            var strInsert =
                "INSERT INTO NWCSRequestDetails(ReferenceNumber,Area,[Date]) VALUES('" +
                CustNumber + "','" + Area + "','" + DateTime.Now.ToString("yyMMdd") + "')";
            try
            {
                using (
                    var conn =
                        new SqlConnection(WebConfigurationManager.ConnectionStrings["Bridge"].ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(strInsert, conn))
                    {
                        var dr = cmd.ExecuteReader();
                    }
                }
            }

            catch (Exception ex)
            {
                LogError(ex.Message, "PushWaterCustReferenceDetails", "INSERT water cust Ref TO BRIDGE");
            }
            return "";
        }

        //====================================================================================================== 
        public static DataTable GetFinpayQuery(string QueryStr)
        {
            var result = new DataTable();
            try
            {
                using (
                    var conn =
                        new SqlConnection(WebConfigurationManager.ConnectionStrings["Finpay"].ConnectionString))
                {
                    conn.Open();
                    using (var cmd = new SqlCommand(QueryStr, conn))
                    {
                        using (SqlDataReader dr = cmd.ExecuteReader())
                        {
                            result.Load(dr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "GetFinpayQuery", "Global Query");
            }
            return result;
        }

        //====================================================================================================== 
        public static string getFinpayGlobalDetails(string Querystring, string FieldName)
        {
            try
            {
                string details = "";
                using (DataTable dt = Datamanagement.GetFinpayQuery(Querystring))
                {
                    if (dt.Rows.Count == 0)
                    {
                    }
                    else
                    {
                        //
                        foreach (DataRow row in dt.Rows) // Loop over the rows.
                        {
                            details = row[FieldName].ToString();
                        }
                    }
                }
                return details;
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "getFinpayGlobalDetails", "Check Global Finpay Details");
                return "";
            }
        }

        //====================================================================================================== 
        public static string insertnewfcdbpayment(string PRN, string TIN, string TAXPAYERNAME, string AMOUNT,
                                                  string PRDATE, string EXPIRYDATE, string URASTATUS, string Source)
        {
            try
            {
                var sqlconn = new SqlConnection(WebConfigurationManager.ConnectionStrings["Finpay"].ConnectionString);
                sqlconn.Open();
                var sqlcomm = new SqlCommand
                                  {
                                      Connection = sqlconn,
                                      CommandType = CommandType.StoredProcedure,
                                      CommandText = "INSERTNEW_FCDBPAYMENT"
                                  };
                //Here I am definied command type is Stored Procedure.
                //Here I mentioned the Stored Procedure Name.
                //Here I fix the variable values to Stored Procedure Parameters. You can easily understand if you can see the Stored Procedure Code.
                sqlcomm.Parameters.Add(new SqlParameter("@PRN", SqlDbType.VarChar)).Value = (PRN);
                sqlcomm.Parameters.Add(new SqlParameter("@TIN", SqlDbType.VarChar)).Value = (TIN);
                sqlcomm.Parameters.Add(new SqlParameter("@TAXPAYERNAME", SqlDbType.VarChar)).Value = TAXPAYERNAME;
                sqlcomm.Parameters.Add(new SqlParameter("@AMOUNT", SqlDbType.Decimal)).Value = Convert.ToDecimal(AMOUNT);
                sqlcomm.Parameters.Add(new SqlParameter("@PRDATE", SqlDbType.DateTime)).Value =
                    Convert.ToDateTime(PRDATE);
                sqlcomm.Parameters.Add(new SqlParameter("@EXPIRYDATE", SqlDbType.DateTime)).Value =
                    Convert.ToDateTime(EXPIRYDATE);
                sqlcomm.Parameters.Add(new SqlParameter("@URASTATUS", SqlDbType.VarChar)).Value = URASTATUS;
                sqlcomm.Parameters.Add(new SqlParameter("@Source", SqlDbType.VarChar)).Value = Source;
                sqlcomm.ExecuteNonQuery();
                sqlcomm.Dispose();
                sqlconn.Close();
                return "";
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "insertnewfcdbpayment", "insert new fcdb Registration");
                return "";
            }
        }

        //====================================================================================================== 
        public static string UpdatePRNResponded(string PRN)
        {
            try
            {
                var sqlconn = new SqlConnection(WebConfigurationManager.ConnectionStrings["Bridge"].ConnectionString);
                sqlconn.Open();
                var sqlcomm = new SqlCommand
                                  {
                                      Connection = sqlconn,
                                      CommandType = CommandType.StoredProcedure,
                                      CommandText = "sp_UpdatePRNResponded"
                                  };
                //Here I am definied command type is Stored Procedure.
                //Here I mentioned the Stored Procedure Name.
                //Here I fix the variable values to Stored Procedure Parameters. You can easily understand if you can see the Stored Procedure Code.
                sqlcomm.Parameters.Add(new SqlParameter("@PRN", SqlDbType.VarChar)).Value = (PRN);
                sqlcomm.ExecuteNonQuery();
                sqlcomm.Dispose();
                sqlconn.Close();
                return "";
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "UpdatePRNResponded", "Datamanagement");
                return "";
            }
        }

        //====================================================================================================== 
        public static string UpdateMSGIDResponded(string PRN)
        {
            try
            {
                var sqlconn = new SqlConnection(WebConfigurationManager.ConnectionStrings["Bridge"].ConnectionString);
                sqlconn.Open();
                var sqlcomm = new SqlCommand
                                  {
                                      Connection = sqlconn,
                                      CommandType = CommandType.StoredProcedure,
                                      CommandText = "sp_UpdateMSGIDResponded"
                                  };
                //Here I am definied command type is Stored Procedure.
                //Here I mentioned the Stored Procedure Name.
                //Here I fix the variable values to Stored Procedure Parameters. You can easily understand if you can see the Stored Procedure Code.
                sqlcomm.Parameters.Add(new SqlParameter("@PRN", SqlDbType.VarChar)).Value = (PRN);
                sqlcomm.ExecuteNonQuery();
                sqlcomm.Dispose();
                sqlconn.Close();
                return "";
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "UpdateMSGIDResponded", "Datamanagement");
                return "";
            }
        }

        //====================================================================================================== 
        public static string UpdatePRNSource(string PRN, string Source)
        {
            try
            {
                var sqlconn = new SqlConnection(WebConfigurationManager.ConnectionStrings["Finpay"].ConnectionString);
                sqlconn.Open();
                var sqlcomm = new SqlCommand
                                  {
                                      Connection = sqlconn,
                                      CommandType = CommandType.StoredProcedure,
                                      CommandText = "Update_Source"
                                  };
                //Here I am definied command type is Stored Procedure.
                //Here I mentioned the Stored Procedure Name.
                //Here I fix the variable values to Stored Procedure Parameters. You can easily understand if you can see the Stored Procedure Code.
                sqlcomm.Parameters.Add(new SqlParameter("@Source", SqlDbType.VarChar)).Value = (Source);
                sqlcomm.Parameters.Add(new SqlParameter("@PRN", SqlDbType.VarChar)).Value = (PRN);
                sqlcomm.ExecuteNonQuery();
                sqlcomm.Dispose();
                sqlconn.Close();
                return "";
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "UpdatePRNSource", "Datamanagement");
                return "";
            }
        }
        public static string UpdateKCCACustRefResponded(string CustREf)
        {
            try
            {
                var sqlconn = new SqlConnection(WebConfigurationManager.ConnectionStrings["Bridge"].ConnectionString);
                sqlconn.Open();
                var sqlcomm = new SqlCommand
                                  {
                                      Connection = sqlconn,
                                      CommandType = CommandType.StoredProcedure,
                                      CommandText = "sp_UpdateKCCARefResponded"
                                  };
                //Here I am definied command type is Stored Procedure.
                //Here I mentioned the Stored Procedure Name.
                //Here I fix the variable values to Stored Procedure Parameters. You can easily understand if you can see the Stored Procedure Code.
                sqlcomm.Parameters.Add(new SqlParameter("@CustREF", SqlDbType.VarChar)).Value = (CustREf);
                sqlcomm.ExecuteNonQuery();
                sqlcomm.Dispose();
                sqlconn.Close();
                return "";
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "UpdateKCCACustRefResponded", "Datamanagement");
                return "";
            }
        }
        //====================================================================================================== 
        public static string UpdateCustRefResponded(string CustREf)
        {
            try
            {
                var sqlconn = new SqlConnection(WebConfigurationManager.ConnectionStrings["Bridge"].ConnectionString);
                sqlconn.Open();
                var sqlcomm = new SqlCommand
                                  {
                                      Connection = sqlconn,
                                      CommandType = CommandType.StoredProcedure,
                                      CommandText = "sp_UpdateUmemeRefResponded"
                                  };
                //Here I am definied command type is Stored Procedure.
                //Here I mentioned the Stored Procedure Name.
                //Here I fix the variable values to Stored Procedure Parameters. You can easily understand if you can see the Stored Procedure Code.
                sqlcomm.Parameters.Add(new SqlParameter("@CustREF", SqlDbType.VarChar)).Value = (CustREf);
                sqlcomm.ExecuteNonQuery();
                sqlcomm.Dispose();
                sqlconn.Close();
                return "";
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "UpdateCustRefResponded", "Datamanagement");
                return "";
            }
        }

        //====================================================================================================== 
        public static string UpdateWaterCustRefResponded(string CustREf)
        {
            try
            {
                var sqlconn = new SqlConnection(WebConfigurationManager.ConnectionStrings["Bridge"].ConnectionString);
                sqlconn.Open();
                var sqlcomm = new SqlCommand
                                  {
                                      Connection = sqlconn,
                                      CommandType = CommandType.StoredProcedure,
                                      CommandText = "sp_UpdateWaterRefResponded"
                                  };
                //Here I am definied command type is Stored Procedure.
                //Here I mentioned the Stored Procedure Name.
                //Here I fix the variable values to Stored Procedure Parameters. You can easily understand if you can see the Stored Procedure Code.
                sqlcomm.Parameters.Add(new SqlParameter("@CustREF", SqlDbType.VarChar)).Value = (CustREf);
                sqlcomm.ExecuteNonQuery();
                sqlcomm.Dispose();
                sqlconn.Close();
                return "";
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "UpdateWaterCustRefResponded", "Datamanagement");
                return "";
            }
        }
        
        //====================================================================================================== 
        public static  string IsertintoUmemePayments(string ReferenceNumber, string PaymentName, string UtilityBranch,
                                                    string UtilityAccountNumber,
                                                    string CustomerName, string AmountDue, string AmountPaid,
                                                    string PaymentOption,
                                                    string DebitBranch, string AccountNumber, string ChequeNumber,
                                                    string CustomerRefNumber,
                                                    string customertype, string PhoneNumber,
                                                    string Remarks, string CapturedBy, string ProcCode,
                                                    string ValDate, string TransactionBranch, string statuscode,
                                                    string statusdesc, string paytype, string offline, string paycod,
                                                    string clearingdetails)
        {
            try
            {
                var sqlconn = new SqlConnection(WebConfigurationManager.ConnectionStrings["Finpay"].ConnectionString);
                sqlconn.Open();
                var sqlcomm = new SqlCommand
                                  {
                                      Connection = sqlconn,
                                      CommandType = CommandType.StoredProcedure,
                                      CommandText = "Add_UmemePayments"
                                  };
                //Here I am definied command type is Stored Procedure.
                //Here I mentioned the Stored Procedure Name.
                //Here I fix the variable values to Stored Procedure Parameters. You can easily understand if you can see the Stored Procedure Code.
                sqlcomm.Parameters.Add("@PaymentID", SqlDbType.Decimal, 4).Direction = ParameterDirection.Output;
                sqlcomm.Parameters.Add(new SqlParameter("@ReferenceNumber", SqlDbType.VarChar)).Value =
                    (ReferenceNumber);
                sqlcomm.Parameters.Add(new SqlParameter("@PaymentName", SqlDbType.VarChar)).Value = (PaymentName);
                sqlcomm.Parameters.Add(new SqlParameter("@UtilityBranch", SqlDbType.VarChar)).Value = UtilityBranch;
                sqlcomm.Parameters.Add(new SqlParameter("@UtilityAccountNumber", SqlDbType.VarChar)).Value =
                    UtilityAccountNumber;
                sqlcomm.Parameters.Add(new SqlParameter("@CustomerName", SqlDbType.VarChar)).Value = CustomerName;
                sqlcomm.Parameters.Add(new SqlParameter("@AmountDue", SqlDbType.Money)).Value = AmountDue;
                sqlcomm.Parameters.Add(new SqlParameter("@AmountPaid", SqlDbType.Money)).Value = AmountPaid;
                sqlcomm.Parameters.Add(new SqlParameter("@PaymentOption", SqlDbType.VarChar)).Value = PaymentOption;
                sqlcomm.Parameters.Add(new SqlParameter("@DebitBranch", SqlDbType.VarChar)).Value = (DebitBranch);
                sqlcomm.Parameters.Add(new SqlParameter("@AccountNumber", SqlDbType.VarChar)).Value = (AccountNumber);
                sqlcomm.Parameters.Add(new SqlParameter("@ChequeNumber", SqlDbType.VarChar)).Value = ChequeNumber;
                sqlcomm.Parameters.Add(new SqlParameter("@CustomerRefNumber", SqlDbType.VarChar)).Value =
                    CustomerRefNumber;
                sqlcomm.Parameters.Add(new SqlParameter("@customertype", SqlDbType.VarChar)).Value = customertype;
                sqlcomm.Parameters.Add(new SqlParameter("@PhoneNumber", SqlDbType.VarChar)).Value = PhoneNumber;
                sqlcomm.Parameters.Add(new SqlParameter("@Remarks", SqlDbType.VarChar)).Value = Remarks;
                sqlcomm.Parameters.Add(new SqlParameter("@CapturedBy", SqlDbType.VarChar)).Value = CapturedBy;
                sqlcomm.Parameters.Add(new SqlParameter("@ProcCode", SqlDbType.VarChar)).Value = (ProcCode);
                sqlcomm.Parameters.Add(new SqlParameter("@ValDate", SqlDbType.Date)).Value = Convert.ToDateTime(ValDate);
                sqlcomm.Parameters.Add(new SqlParameter("@TransactionBranch", SqlDbType.VarChar)).Value =
                    TransactionBranch;
                sqlcomm.Parameters.Add(new SqlParameter("@statuscode", SqlDbType.VarChar)).Value = statuscode;
                sqlcomm.Parameters.Add(new SqlParameter("@statusdesc", SqlDbType.VarChar)).Value = statusdesc;
                sqlcomm.Parameters.Add(new SqlParameter("@paytype", SqlDbType.VarChar)).Value = paytype;
                sqlcomm.Parameters.Add(new SqlParameter("@offline", SqlDbType.VarChar)).Value = offline;
                sqlcomm.Parameters.Add(new SqlParameter("@paycod", SqlDbType.VarChar)).Value = paycod;
                //sqlcomm.Parameters.Add(new SqlParameter("@clearingdetails", SqlDbType.VarChar)).Value = clearingdetails;
                sqlcomm.ExecuteNonQuery();
                sqlcomm.Dispose();
                sqlconn.Close();
                return "";
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "IsertintoUmemePayments", "Insert into umeme payments");
                return "";
            }
        }

        //=====================================================================================================
        public static string IsertintoKCCAPayments(string ReferenceNumber, string PaymentName, 
                                                    string UtilityAccountNumber,
                                                    string CustomerName, string AmountDue, string AmountPaid,
                                                    string PaymentOption,
                                                    string debitmobilenumber, 
                                                    string CustomerRefNumber,
                                                    string customertype, string PhoneNumber,
                                                    string Remarks,
                                                    string ValDate,  string statuscode,
                                                    string statusdesc, string paytype, string offline, string paycod,
                                                    string clearingdetails,string coin)
        {
            try
            {
                var sqlconn = new SqlConnection(WebConfigurationManager.ConnectionStrings["AdaPay"].ConnectionString);
                sqlconn.Open();
                var sqlcomm = new SqlCommand
                {
                    Connection = sqlconn,
                    CommandType = CommandType.StoredProcedure,
                    CommandText = "Add_KCCAPayments"
                };
                
                sqlcomm.Parameters.Add("@PaymentID", SqlDbType.Decimal, 4).Direction = ParameterDirection.Output;
                sqlcomm.Parameters.Add(new SqlParameter("@ReferenceNumber", SqlDbType.VarChar)).Value = (ReferenceNumber);
                sqlcomm.Parameters.Add(new SqlParameter("@paycoin", SqlDbType.VarChar)).Value = (coin);
                sqlcomm.Parameters.Add(new SqlParameter("@PaymentName", SqlDbType.VarChar)).Value = (PaymentName);
                sqlcomm.Parameters.Add(new SqlParameter("@UtilityAccountNumber", SqlDbType.VarChar)).Value =
                    UtilityAccountNumber;
                sqlcomm.Parameters.Add(new SqlParameter("@CustomerName", SqlDbType.VarChar)).Value = CustomerName;
                sqlcomm.Parameters.Add(new SqlParameter("@AmountDue", SqlDbType.Money)).Value = AmountDue;
                sqlcomm.Parameters.Add(new SqlParameter("@AmountPaid", SqlDbType.Money)).Value = AmountPaid;
                sqlcomm.Parameters.Add(new SqlParameter("@PaymentOption", SqlDbType.VarChar)).Value = PaymentOption;
                sqlcomm.Parameters.Add(new SqlParameter("@debitmobilenumber", SqlDbType.VarChar)).Value = (debitmobilenumber);
                sqlcomm.Parameters.Add(new SqlParameter("@CustomerRefNumber", SqlDbType.VarChar)).Value =
                    CustomerRefNumber;
                sqlcomm.Parameters.Add(new SqlParameter("@customertype", SqlDbType.VarChar)).Value = customertype;
                sqlcomm.Parameters.Add(new SqlParameter("@PhoneNumber", SqlDbType.VarChar)).Value = PhoneNumber;
                sqlcomm.Parameters.Add(new SqlParameter("@Remarks", SqlDbType.VarChar)).Value = Remarks;
                sqlcomm.Parameters.Add(new SqlParameter("@statuscode", SqlDbType.VarChar)).Value = statuscode;
                sqlcomm.Parameters.Add(new SqlParameter("@statusdesc", SqlDbType.VarChar)).Value = statusdesc;
                sqlcomm.Parameters.Add(new SqlParameter("@paytype", SqlDbType.VarChar)).Value = paytype;
                sqlcomm.Parameters.Add(new SqlParameter("@offline", SqlDbType.VarChar)).Value = offline;
                sqlcomm.Parameters.Add(new SqlParameter("@clearingdetails", SqlDbType.VarChar)).Value = clearingdetails;
                sqlcomm.ExecuteNonQuery();
                sqlcomm.Dispose();
                sqlconn.Close();
                return "";
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "IsertintoKCCAPayments", "Insert into KCCA payments");
                return "";
            }
        }

        //====================================================================================================== 
        public static string IsertintoNWSCPayments(string ReferenceNumber, string PaymentName, string UtilityBranch,
                                                   string UtilityAccountNumber,
                                                   string CustomerName, string AmountDue, string AmountPaid,
                                                   string PaymentOption,
                                                   string DebitBranch, string AccountNumber, string ChequeNumber,
                                                   string CustomerRefNumber,
                                                   string PhoneNumber,
                                                   string Remarks, string CapturedBy, string ProcCode,
                                                   string ValDate, string TransactionBranch, string clearingdetails,
                                                   string area)
        {
            try
            {
                var sqlconn = new SqlConnection(WebConfigurationManager.ConnectionStrings["Finpay"].ConnectionString);
                sqlconn.Open();
                var sqlcomm = new SqlCommand
                                  {
                                      Connection = sqlconn,
                                      CommandType = CommandType.StoredProcedure,
                                      CommandText = "Add_Payments"
                                  };
                //Here I am definied command type is Stored Procedure.
                //Here I mentioned the Stored Procedure Name.
                //Here I fix the variable values to Stored Procedure Parameters. You can easily understand if you can see the Stored Procedure Code.
                sqlcomm.Parameters.Add("@PaymentID", SqlDbType.Decimal, 4).Direction = ParameterDirection.Output;
                sqlcomm.Parameters.Add(new SqlParameter("@ReferenceNumber", SqlDbType.VarChar)).Value =
                    (ReferenceNumber);
                sqlcomm.Parameters.Add(new SqlParameter("@PaymentName", SqlDbType.VarChar)).Value = (PaymentName);
                sqlcomm.Parameters.Add(new SqlParameter("@UtilityBranch", SqlDbType.VarChar)).Value = UtilityBranch;
                sqlcomm.Parameters.Add(new SqlParameter("@UtilityAccountNumber", SqlDbType.VarChar)).Value =
                    UtilityAccountNumber;
                sqlcomm.Parameters.Add(new SqlParameter("@CustomerName", SqlDbType.VarChar)).Value = CustomerName;
                sqlcomm.Parameters.Add(new SqlParameter("@AmountDue", SqlDbType.Money)).Value = AmountDue;
                sqlcomm.Parameters.Add(new SqlParameter("@AmountPaid", SqlDbType.Money)).Value = AmountPaid;
                sqlcomm.Parameters.Add(new SqlParameter("@PaymentOption", SqlDbType.VarChar)).Value = PaymentOption;
                sqlcomm.Parameters.Add(new SqlParameter("@DebitBranch", SqlDbType.VarChar)).Value = (DebitBranch);
                sqlcomm.Parameters.Add(new SqlParameter("@AccountNumber", SqlDbType.VarChar)).Value = (AccountNumber);
                sqlcomm.Parameters.Add(new SqlParameter("@ChequeNumber", SqlDbType.VarChar)).Value = ChequeNumber;
                sqlcomm.Parameters.Add(new SqlParameter("@CustomerRefNumber", SqlDbType.VarChar)).Value =
                    CustomerRefNumber;
                sqlcomm.Parameters.Add(new SqlParameter("@CustomerArea", SqlDbType.VarChar)).Value = area;
                sqlcomm.Parameters.Add(new SqlParameter("@PhoneNumber", SqlDbType.VarChar)).Value = PhoneNumber;
                sqlcomm.Parameters.Add(new SqlParameter("@Remarks", SqlDbType.VarChar)).Value = Remarks;
                sqlcomm.Parameters.Add(new SqlParameter("@CapturedBy", SqlDbType.VarChar)).Value = CapturedBy;
                sqlcomm.Parameters.Add(new SqlParameter("@ProcCode", SqlDbType.VarChar)).Value = (ProcCode);
                sqlcomm.Parameters.Add(new SqlParameter("@ValDate", SqlDbType.Date)).Value = Convert.ToDateTime(ValDate);
                sqlcomm.Parameters.Add(new SqlParameter("@TransactionBranch", SqlDbType.VarChar)).Value =
                    TransactionBranch;
                sqlcomm.Parameters.Add(new SqlParameter("@clearingdetails", SqlDbType.VarChar)).Value = clearingdetails;
                sqlcomm.ExecuteNonQuery();
                sqlcomm.Dispose();
                sqlconn.Close();
                return "";
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "IsertintoNWSCPayments", "Insert into NWSC payments");
                return "";
            }
        }

        //====================================================================================================== 
        public static string IsertintoUploadFileDetails(string PresentedBy, string FilePath, string ReferenceNo,
                                                        string Remarks,
                                                        string AreaBranchOffice, string CompanyName, string NSSFNumber,
                                                        string ContributionYear,
                                                        string ContributionMonth, string ReceiptNumber, string Members,
                                                        string Amount,
                                                        string ChequeNumber, string PostedBy,
                                                        string TransactionCode, string ValueDate,
                                                        string TransactionBranch,
                                                        string DebitBranch, string AccountNumber, string UtilityBranch,
                                                        string UtilityAccountNumber, string PaymentOption)
        {
            try
            {
                var sqlconn = new SqlConnection(WebConfigurationManager.ConnectionStrings["Finpay"].ConnectionString);
                sqlconn.Open();
                var sqlcomm = new SqlCommand
                                  {
                                      Connection = sqlconn,
                                      CommandType = CommandType.StoredProcedure,
                                      CommandText = "Add_UploadFileDetails"
                                  };
                //Here I am definied command type is Stored Procedure.
                //Here I mentioned the Stored Procedure Name.
                //Here I fix the variable values to Stored Procedure Parameters. You can easily understand if you can see the Stored Procedure Code.
                sqlcomm.Parameters.Add("@FileUploadID", SqlDbType.Int).Direction = ParameterDirection.Output;
                sqlcomm.Parameters.Add(new SqlParameter("@PresentedBy", SqlDbType.VarChar)).Value = (PresentedBy);
                sqlcomm.Parameters.Add(new SqlParameter("@FilePath", SqlDbType.VarChar)).Value = (FilePath);
                sqlcomm.Parameters.Add(new SqlParameter("@ReferenceNo", SqlDbType.VarChar)).Value = ReferenceNo;
                sqlcomm.Parameters.Add(new SqlParameter("@Remarks", SqlDbType.VarChar)).Value = Remarks;
                sqlcomm.Parameters.Add(new SqlParameter("@AreaBranchOffice", SqlDbType.VarChar)).Value =
                    AreaBranchOffice;
                sqlcomm.Parameters.Add(new SqlParameter("@CompanyName", SqlDbType.Money)).Value = CompanyName;
                sqlcomm.Parameters.Add(new SqlParameter("@NSSFNumber", SqlDbType.Money)).Value = NSSFNumber;
                sqlcomm.Parameters.Add(new SqlParameter("@ContributionYear", SqlDbType.VarChar)).Value =
                    ContributionYear;
                sqlcomm.Parameters.Add(new SqlParameter("@ContributionMonth", SqlDbType.VarChar)).Value =
                    (ContributionMonth);
                sqlcomm.Parameters.Add(new SqlParameter("@ReceiptNumber", SqlDbType.VarChar)).Value = (ReceiptNumber);
                sqlcomm.Parameters.Add(new SqlParameter("@Members", SqlDbType.VarChar)).Value = Members;
                sqlcomm.Parameters.Add(new SqlParameter("@Amount", SqlDbType.VarChar)).Value = Amount;
                sqlcomm.Parameters.Add(new SqlParameter("@ChequeNumber", SqlDbType.VarChar)).Value = ChequeNumber;
                sqlcomm.Parameters.Add(new SqlParameter("@PostedBy", SqlDbType.VarChar)).Value = PostedBy;
                sqlcomm.Parameters.Add(new SqlParameter("@TransactionCode", SqlDbType.VarChar)).Value = TransactionCode;
                sqlcomm.Parameters.Add(new SqlParameter("@ValueDate", SqlDbType.VarChar)).Value = ValueDate;
                sqlcomm.Parameters.Add(new SqlParameter("@TransactionBranch", SqlDbType.VarChar)).Value =
                    (TransactionBranch);
                sqlcomm.Parameters.Add(new SqlParameter("@DebitBranch", SqlDbType.VarChar)).Value = (DebitBranch);
                sqlcomm.Parameters.Add(new SqlParameter("@AccountNumber", SqlDbType.VarChar)).Value = AccountNumber;
                sqlcomm.Parameters.Add(new SqlParameter("@UtilityBranch", SqlDbType.VarChar)).Value = UtilityBranch;
                sqlcomm.Parameters.Add(new SqlParameter("@UtilityAccountNumber", SqlDbType.VarChar)).Value =
                    UtilityAccountNumber;
                sqlcomm.Parameters.Add(new SqlParameter("@PaymentOption", SqlDbType.VarChar)).Value = PaymentOption;
                sqlcomm.ExecuteNonQuery();
                sqlcomm.Dispose();
                sqlconn.Close();
                return "";
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "IsertintoUploadFileDetails", "Insert into NSSF file payments");
                return "";
            }
        }

        //====================================================================================================== 
        public static string IsertintoUploadFilePatriculars(string FileUploadID, string StaffNumber, string NSSFNumber,
                                                            string ContributionType,
                                                            string ContributionYear, string EmployeeNames,
                                                            string EmployeeGrossPay,
                                                            string ContributionMonth, string EmployeeContribution,
                                                            string EmployerContribution, string TotalContribution)
        {
            try
            {
                var sqlconn = new SqlConnection(WebConfigurationManager.ConnectionStrings["Finpay"].ConnectionString);
                sqlconn.Open();
                var sqlcomm = new SqlCommand
                                  {
                                      Connection = sqlconn,
                                      CommandType = CommandType.StoredProcedure,
                                      CommandText = "Add_UploadFileParticulars"
                                  };
                //Here I am definied command type is Stored Procedure.
                //Here I mentioned the Stored Procedure Name.
                //Here I fix the variable values to Stored Procedure Parameters. You can easily understand if you can see the Stored Procedure Code.
                sqlcomm.Parameters.Add("@UploadParticularsID", SqlDbType.Int).Direction = ParameterDirection.Output;
                sqlcomm.Parameters.Add(new SqlParameter("@FileUploadID", SqlDbType.VarChar)).Value = (FileUploadID);
                sqlcomm.Parameters.Add(new SqlParameter("@StaffNumber", SqlDbType.VarChar)).Value = (StaffNumber);
                sqlcomm.Parameters.Add(new SqlParameter("@NSSFNumber", SqlDbType.VarChar)).Value = NSSFNumber;
                sqlcomm.Parameters.Add(new SqlParameter("@ContributionType", SqlDbType.VarChar)).Value =
                    ContributionType;
                sqlcomm.Parameters.Add(new SqlParameter("@ContributionYear", SqlDbType.VarChar)).Value =
                    ContributionYear;
                sqlcomm.Parameters.Add(new SqlParameter("@ContributionMonth", SqlDbType.Money)).Value =
                    ContributionMonth;
                sqlcomm.Parameters.Add(new SqlParameter("@EmployeeNames", SqlDbType.Money)).Value = EmployeeNames;
                sqlcomm.Parameters.Add(new SqlParameter("@EmployeeGrossPay", SqlDbType.VarChar)).Value =
                    EmployeeGrossPay;
                sqlcomm.Parameters.Add(new SqlParameter("@EmployeeContribution", SqlDbType.VarChar)).Value =
                    (EmployeeContribution);
                sqlcomm.Parameters.Add(new SqlParameter("@EmployerContribution", SqlDbType.VarChar)).Value =
                    (EmployerContribution);
                sqlcomm.Parameters.Add(new SqlParameter("@TotalContribution", SqlDbType.VarChar)).Value =
                    TotalContribution;
                sqlcomm.ExecuteNonQuery();
                sqlcomm.Dispose();
                sqlconn.Close();
                return "";
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "IsertintoUploadFileParticulars", "Insert into NSSF file payments");
                return "";
            }
        }

        //====================================================================================================== 
        public static string UpdateRegistration(string PRN, string ValueDate, string Maker, string Branch,
                                                string ChequeNo, string CRAccount, string DRAccount,
                                                string BankStatus,
                                                string URAStatus, string Tran_Num, Boolean Auto, string Checker,
                                                string BankCode, string BranchCode,
                                                string ClearingCode, string TranName, string Currency,
                                                string TranCode, string OtherBankAccount)
        {
            try
            {
                var sqlconn = new SqlConnection(WebConfigurationManager.ConnectionStrings["Finpay"].ConnectionString);
                sqlconn.Open();
                var sqlcomm = new SqlCommand
                                  {
                                      Connection = sqlconn,
                                      CommandType = CommandType.StoredProcedure,
                                      CommandText = "Update_Payment"
                                  };
                //Here I am definied command type is Stored Procedure.
                //Here I mentioned the Stored Procedure Name.
                //Here I fix the variable values to Stored Procedure Parameters. You can easily understand if you can see the Stored Procedure Code.
                sqlcomm.Parameters.Add(new SqlParameter("@PRN", SqlDbType.VarChar)).Value = (PRN);
                sqlcomm.Parameters.Add(new SqlParameter("@ValueDate", SqlDbType.VarChar)).Value = (ValueDate);
                sqlcomm.Parameters.Add(new SqlParameter("@Maker", SqlDbType.VarChar)).Value = Maker;
                sqlcomm.Parameters.Add(new SqlParameter("@Branch", SqlDbType.VarChar)).Value = Branch;
                sqlcomm.Parameters.Add(new SqlParameter("@ChequeNo", SqlDbType.VarChar)).Value = ChequeNo;
                sqlcomm.Parameters.Add(new SqlParameter("@CRAccount", SqlDbType.VarChar)).Value = CRAccount;
                sqlcomm.Parameters.Add(new SqlParameter("@DRAccount", SqlDbType.VarChar)).Value = DRAccount;
                sqlcomm.Parameters.Add(new SqlParameter("@BankStatus", SqlDbType.Bit)).Value = BankStatus;
                sqlcomm.Parameters.Add(new SqlParameter("@URAStatus", SqlDbType.VarChar)).Value = (URAStatus);
                sqlcomm.Parameters.Add(new SqlParameter("@Tran_Num", SqlDbType.VarChar)).Value = (Tran_Num);
                sqlcomm.Parameters.Add(new SqlParameter("@Auto", SqlDbType.Bit)).Value = Auto;
                sqlcomm.Parameters.Add(new SqlParameter("@Checker", SqlDbType.VarChar)).Value = Checker;
                sqlcomm.Parameters.Add(new SqlParameter("@BankCode", SqlDbType.VarChar)).Value = BankCode;
                sqlcomm.Parameters.Add(new SqlParameter("@BranchCode", SqlDbType.VarChar)).Value = BranchCode;
                sqlcomm.Parameters.Add(new SqlParameter("@ClearingCode", SqlDbType.VarChar)).Value = ClearingCode;
                sqlcomm.Parameters.Add(new SqlParameter("@TranName", SqlDbType.Bit)).Value = TranName;
                sqlcomm.Parameters.Add(new SqlParameter("@Currency", SqlDbType.VarChar)).Value = (Currency);
                sqlcomm.Parameters.Add(new SqlParameter("@TranCode", SqlDbType.VarChar)).Value = (TranCode);
                sqlcomm.Parameters.Add(new SqlParameter("@OtherBankAccount", SqlDbType.VarChar)).Value =
                    OtherBankAccount;
                sqlcomm.ExecuteNonQuery();
                sqlcomm.Dispose();
                sqlconn.Close();
                return "";
            }
            catch (Exception ex)
            {
                LogError(ex.Message, "UpdateRegistration", "update Registration");
                return "";
            }
        }

        //====================================================================================================== 
        public static DataTable getKCCA(string prn)
        {
            string pin = prn;
            string strUpdate =
                "SELECT * FROM KCCAPAYMENTS WHERE CustomerRefNumber =  'P150000050047'  ";
            var result = new DataTable();
            try
            {
                
                    var conn = new SqlConnection(WebConfigurationManager.ConnectionStrings["Finpay"].ConnectionString);
                
                    conn.Open();
                     var cmd = new SqlCommand(strUpdate, conn);
                     result.Load(cmd.ExecuteReader());
                   
                        //cmd.Parameters.Add("@CLEAREDSTATUS", SqlDbType.Bit).Value = false;
                        //cmd.Parameters.Add("@PRN", SqlDbType.VarChar).Value = prn;
                        //cmd.Parameters.Add("@TRANNAME", SqlDbType.VarChar).Value = trnname;
                        
                
            }

            catch (Exception ex)
            {
                LogError(ex.Message, "UpdateRegistration", "");
            }
            return result;
        }

    }
}
