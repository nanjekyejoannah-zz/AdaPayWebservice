using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using AdaPayInterface;

namespace AdaPayInterface
{
    /// <summary>
    /// Summary description for FinInterface
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class AdaPayInterface : System.Web.Services.WebService
    {        
        [WebMethod]
        public string Query(string RefNo,int Product,int AppId,string PassPhrase,string Location="")
        {
            FCDBProducts p = (FCDBProducts)Product;
            string results="";
            switch (p)
            {
                case FCDBProducts.URA:
                    if (RefNo != "")
                    {
                        try
                        {
                            InterfaceUra IU = new InterfaceUra();
                            return IU.PullRegistrationDetails(RefNo);
                        }                      
                        catch (Exception ex)
                        {
                            results= ex.Message;
                        }                                    
                    }
                    break;
                case FCDBProducts.KCCA:
                    if (RefNo != "")
                    {                        
                        InterfaceKcca IK = new InterfaceKcca();
                        results= IK.PullKCCADetails(RefNo);
                    }
                    break;
                case FCDBProducts .WATER:
                    if (RefNo != "")
                    {
                        InterfaceWater IW = new InterfaceWater();
                        results = IW.PullWaterDetails(RefNo, Location);
                    }
                    break;
                case FCDBProducts .ELECTRICITY:
                    if (RefNo != "")
                    {
                        InterfaceUmeme IM = new InterfaceUmeme();
                        results=IM.PullUmemeDetails(RefNo);
                    }
                    break;                   
            }
            return results;
        }

        [WebMethod]
        public string MakePayment(string PRN, int serviceid, string amount, string debitmobilenumber,  string UTILITYACCOUNT, string alertcontact, int paymode, int appid, string apppassword, string xmldata = "", string branch = "", string Institution = "",string Location = "")
        {
            DateTime xtime = System.DateTime.Now;
            string MyTime = xtime.ToString("hhmmss");
            string MyDate = xtime.ToString("yyMMdd");
            string response = "";
            string trnumber = "";
            string s_message = "";
            DataLogic Dl = new DataLogic();
            FCDBProducts p = (FCDBProducts)serviceid;
            string results = "";

            if (PRN == "")
            {
                response = Functions.WriteSuccessResponseFromMakePayment(PRN, serviceid, Institution, branch, trnumber, "FAIL - MISSING PRN");
                return response;
            }

            if (debitmobilenumber == "")
            {                
                response = Functions.WriteSuccessResponseFromMakePayment(PRN, serviceid, Institution, branch, trnumber, "FAIL - MISSING DEBIT ACCOUNT");
                return response;
            }
            if (branch == "")
            {

                response = Functions.WriteSuccessResponseFromMakePayment(PRN, serviceid, Institution, branch, trnumber, "FAIL - MISSING ACCOUNT BRANCH ");
                return response;
            }

            if (amount == "")
            {
                
                response = Functions.WriteSuccessResponseFromMakePayment(PRN, serviceid, Institution, branch, trnumber, "FAIL - MISSING AMOUNT");
                return response;
            }
            switch (p)
            {
                case FCDBProducts.KCCA:
                    if (PRN != "")
                    {
                        if (appid == 3) // WELCOME TO THE TELLLER INTERFACE. ALLOW ALL TRANSACTIONS
                        {

                            // Begin upload process

                            string responsefromCBS = "";
                            InterfaceKcca KResp = new InterfaceKcca();
                            trnumber = KResp.PayKCCA(PRN, xmldata, UTILITYACCOUNT, amount, debitmobilenumber, MyDate, MyTime, alertcontact,"");

                            if (trnumber != "UNSUCCESSFULL")
                            {


                                response = Functions.WriteSuccessResponseFromMakePayment(PRN, serviceid, Institution, branch, trnumber, "SUCCESS");

                            }
                            else
                            {
                                response = Functions.WriteSuccessResponseFromMakePayment(PRN, serviceid, Institution, branch, trnumber, "FAIL");
                            }

                        }

                    }
                    else
                    {
                        response = "Reference number missing.";
                        response =Functions.WriteFailureResponseFromMakePayment(PRN, serviceid, branch, "1", "Reference number missing", Institution);
                        Dl.ResponseLogs(response);
                        return response; // Exit method at this point 
                    }
                    break;
                case FCDBProducts.URA:
                    if (PRN != "")
                    {
                        if (appid == 0) // WELCOME TO THE TELLLER INTERFACE. ALLOW ALL TRANSACTIONS
                        {

                            // Begin upload process

                            string responsefromCBS = "";
                            InterfaceUra UResp = new InterfaceUra();
                            //trnumber=UResp.FinpayURA(PRN, xmldata, branch, debitaccount, amount, "UGX", MyDate, MyTime);

                            if (trnumber.Substring(0, 12) != "UNSUCCESSFUL")
                            {
                                response = Functions.WriteSuccessResponseFromMakePayment(PRN, serviceid, Institution, branch, trnumber, "SUCCESS");
                            }
                            else
                            {
                                response = Functions.WriteSuccessResponseFromMakePayment(PRN, serviceid, Institution, branch, trnumber, "FAIL");
                            }

                        }

                    }
                    else
                    {
                        response = "Reference number missing.";
                        response = Functions.WriteFailureResponseFromMakePayment(PRN, serviceid, branch, "1", "Reference number missing", Institution);
                        Dl.ResponseLogs(response);
                         // Exit method at this point 
                    }
                    break;
                case FCDBProducts.WATER:
                    if (PRN != "")
                    {
                        if (appid == 1) // WELCOME TO THE TELLLER INTERFACE. ALLOW ALL TRANSACTIONS
                        {

                            // Begin upload process

                            string responsefromCBS = "";
                            InterfaceWater WResp = new InterfaceWater();
                           // trnumber=WResp.FinpayNWSC(PRN, branch, xmldata, branch, debitaccount, amount, "UGX", MyDate, MyTime, "", "", Location);

                            if (trnumber != "UNSUCCESSFUL")
                            {
                                response = Functions.WriteSuccessResponseFromMakePayment(PRN, serviceid, Institution, branch, trnumber, "SUCCESS");
                            }
                            else
                            {
                                response = Functions.WriteSuccessResponseFromMakePayment(PRN, serviceid, Institution, branch, trnumber, "FAIL");
                            }

                        }

                    }
                    else
                    {
                        response = "Reference number missing.";
                        response = Functions.WriteFailureResponseFromMakePayment(PRN, serviceid, branch, "1", "Reference number missing", Institution);
                        Dl.ResponseLogs(response);
                        // Exit method at this point 
                    }
                    break;

                case FCDBProducts.ELECTRICITY:
                    if (PRN != "")
                    {
                        if (appid == 2) // WELCOME TO THE TELLLER INTERFACE. ALLOW ALL TRANSACTIONS
                        {

                            // Begin upload process

                            string responsefromCBS = "";
                            InterfaceUmeme IM = new InterfaceUmeme();
                            //trnumber = IM.FinpayUmeme(PRN, branch, xmldata, branch, debitaccount, amount, "UGX", MyDate, MyTime, "", "", Location);
                            //trnumber = WResp.(PRN, branch, xmldata, branch, debitaccount, amount, "UGX", MyDate, MyTime, "", "", Location);

                            if (trnumber != "UNSUCCESSFUL")
                            {
                                response = Functions.WriteSuccessResponseFromMakePayment(PRN, serviceid, Institution, branch, trnumber, "SUCCESS");
                            }
                            else
                            {
                                response = Functions.WriteSuccessResponseFromMakePayment(PRN, serviceid, Institution, branch, trnumber, "FAIL");
                            }

                        }

                    }
                    else
                    {
                        response = "Reference number missing.";
                        response = Functions.WriteFailureResponseFromMakePayment(PRN, serviceid, branch, "1", "Reference number missing", Institution);
                        Dl.ResponseLogs(response);
                        // Exit method at this point 
                    }
                    break;
            }
            return response;
        }


        public bool  NotifyUtilityProvider()
        {
            return true;
        }


    }

}
    
