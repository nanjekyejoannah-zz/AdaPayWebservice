using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;

namespace AdaPayInterface
{
    public static class YoApiClient
    {
        public static void GetYoApi()
        {
            var url = "https://41.220.12.206/services/yopaymentsdev/task.php";
            var systemClient = new WebClient();
            var content = systemClient.DownloadString(url);
            string user = "nanjekye";
            string pwd = "sunday";
        }
    }
}