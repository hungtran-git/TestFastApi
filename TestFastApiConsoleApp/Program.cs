using FastClassLibrary;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace TestFastApiConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {

            var pass = ConfigHelper.Password;

            IConfiguration configuration = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              .AddEnvironmentVariables()
              .Build();
            var cfg = configuration.Get<AppConfig>();

            var settingsConfigHelper = new SettingsConfigHelper(configuration, "ConnectionString");
            #region StartTesting
            //var baseUrl = "http://fsd.fast.com.vn/DeheusSP_API";
            var baseUrl = "http://fsd.fast.com.vn/DeheusSP_API";
            var loginPage = "/apilogin.ashx";
            var queryPage = "/apiquery.ashx";
            string dFrom = new DateTime(2018, 1, 30).ToString("dd/MM/yyyy");
            string dTo = new DateTime(2021, 1, 30).ToString("dd/MM/yyyy");
            string cacheID = "";
            //dữ liệu khi có sự thay đổi danh mục khách hàng trên FAST
            List<Customer> listCustomer = null;
            //dữ liệu khi có sự thay đổi danh mục hàng hóa vật tư trên FAST
            List<Item> listItem = null;
            //dữ liệu khi có sự thay đổi danh mục kho trên FAST
            List<Site> listSite = null;
            //dữ liệu stock
            List<StockFast> listStock = null;
            FastGet fastHelper = new FastGet(baseUrl);
            var isLogin =  fastHelper.LoginAsync(loginPage, null).Result;
            if (isLogin) {
                listCustomer = fastHelper.getCustomersAsync(queryPage, dFrom, dTo, cacheID).Result;
                listItem = fastHelper.GetItemsAsync(queryPage, dFrom, dTo, cacheID).Result;
                listSite = fastHelper.getSitesAsync(queryPage, dFrom, dTo, cacheID).Result;
                listStock = fastHelper.getStocksAsync(queryPage, dFrom, dTo, cacheID).Result;
            }
            #endregion
            Console.ReadKey();
        }

        //static void Main(string[] args)
        //{
        //    var userId = "4";
        //    var curTime = DateTime.Now.ToString("yyyyMMddhhmmssffffff");
        //    var privateKey = "Fast@2020@DeheusSP";
        //    var keyOrders = "2,9,30,18,23,16,12,17,22,1";
        //    var key = GenerateKey(userId, curTime, privateKey, keyOrders);
        //    var baseUrl = "http://fsd.fast.com.vn/DeheusSP_API";

        //    var postContent = $"userID={userId}&time={curTime}&key={key}";
        //    var controller = "APIlogin.aspx";
        //    var url = $"{baseUrl}/{controller}";
        //    var data = new StringContent(postContent, Encoding.UTF8, "application/x-www-form-urlencoded");
        //    var _httpClient = new HttpClient();

        //    //var response = _httpClient.PostAsync(url, data).Result;
        //    using (var response = _httpClient.PostAsync(url, data).Result)
        //    {
        //        try
        //        {
        //            string apiResponse = response.Content.ReadAsStringAsync().Result;
        //        }
        //        catch (Exception e)
        //        {
        //            throw new Exception(e.Message);
        //        }
        //        //TODO: xu ly apiResponse trong truong hop co loi
        //    }
        //    Console.WriteLine("Hello World!");
        //    Console.ReadLine();
        //}

        private static string GenerateKey(string userId, string time, string privateKey, string keyOrders)
        {
            var content = MD5Hash(privateKey + userId + time);
            var kOrders = keyOrders.Split(',');
            var key = "";
            for (int i = 0; i < kOrders.Length; i++)
            {
                var pos = int.Parse(kOrders[i]) - 1;
                key += content[pos];
            }

            return key;
        }

        private static string MD5Hash(string input)
        {
            StringBuilder hash = new StringBuilder();
            MD5CryptoServiceProvider md5provider = new MD5CryptoServiceProvider();
            byte[] bytes = md5provider.ComputeHash(new UTF8Encoding().GetBytes(input));

            for (int i = 0; i < bytes.Length; i++)
            {
                hash.Append(bytes[i].ToString("x2"));
            }
            return hash.ToString();
        }
    }

}
