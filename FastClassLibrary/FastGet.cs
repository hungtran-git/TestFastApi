using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FastClassLibrary
{
    public class FastGet
    {
        WebClient client = new WebClient();
        string _time, _token, _url, _key;

        public FastGet(string url)
        {
            _url = url;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="requestedUri"></param>
        /// <param name="nameValueCollection"></param>
        /// <returns></returns>
        public string SendData(string requestedUri, NameValueCollection nameValueCollection)
        {
            var returnValue = client.UploadValues(requestedUri, "POST", nameValueCollection);
            string jsonOuput = Encoding.UTF8.GetString(returnValue);

            return jsonOuput;
        }

        public HttpStatusCode GetData(string requestedUri, NameValueCollection nameValueCollection, out string result)
        {
            HttpStatusCode statusCode = HttpStatusCode.OK;
            result = "";
            try
            {
                var returnValue = client.UploadValues(requestedUri, nameValueCollection);
                string jsonOuput = Encoding.UTF8.GetString(returnValue);

                result = jsonOuput;
                statusCode = HttpStatusCode.OK;
            }
            catch (WebException wex)
            {
                statusCode = ((HttpWebResponse)wex.Response).StatusCode;
            }

            return statusCode;
        }

        string GenerateKey(string data)
        {
            List<int> arr = ConfigHelper.PositionList.Split(',').Select(Int32.Parse).ToList();

            System.Security.Cryptography.MD5CryptoServiceProvider md5Hasher = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hashedBytes;
            UTF8Encoding encoder = new System.Text.UTF8Encoding();
            hashedBytes = md5Hasher.ComputeHash(encoder.GetBytes(data));

            string key = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();

            string sMd5 = key;

            string sKey = "";
            for (int i = 0; i < arr.Count; i++)
                sKey += sMd5.Substring(arr[i] - 1, 1);
            return sKey;
        }

        public static NameValueCollection ConvertJsonToNameValueCollection(string json)
        {
            var dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            NameValueCollection nvc = null;
            if (dict != null)
            {
                nvc = new NameValueCollection(dict.Count);
                foreach (var k in dict)
                {
                    nvc.Add(k.Key, k.Value);
                }
            }
            return nvc;
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText.ToString());
            return Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData.ToString());
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public Task<string> RequestPassword(string requestedUri)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> LoginAsync(string requestedUri, string strOut)
        {
            string UserId = ConfigHelper.UserName;
            string time = DateTime.Now.ToString("yyyyMMddHHmmssffffff");
            string key = GenerateKey(ConfigHelper.Password + UserId + time);

            NameValueCollection collection = new NameValueCollection();
            collection.Add("userID", ConfigHelper.UserName);
            collection.Add("time", time);
            collection.Add("key", key);
            try
            {
                string json = SendData(_url + requestedUri, collection);

                DataTable dt = (DataTable)JsonConvert.DeserializeObject(json, (typeof(DataTable)));
                _token = dt.Rows[0]["token"].ToString();
                _time = time;
                _key = key;

                return true;
            }
            catch (WebException ex)
            {
                var stre = new StreamReader(ex.Response.GetResponseStream());
                strOut = stre.ReadToEnd();
                stre.Close();
            }
            return false;
        }

        public async Task<List<Item>> GetItemsAsync(string requestedUri, string dFrom, string dTo, string cacheID)
        {
            NameValueCollection collection = new NameValueCollection();
            string time = DateTime.Now.ToString("yyyyMMddHHmmssffffff");
            //string data = Base64Encode(@"[{dFrom: '01-01-2018', dTo: '01/09/2020',cacheID: '|cacheID|'}]");
            string data = Base64Encode(@"[{dFrom: '" + dFrom + "', dTo: '" + dTo + "',cacheID: '" + cacheID + "'}]");
            string key = GenerateKey(ConfigHelper.Password + time + data);

            collection.Add("token", _token);
            collection.Add("form", Base64Encode("getItems"));
            collection.Add("key", key);
            collection.Add("data", data);
            collection.Add("userId", ConfigHelper.UserName);
            collection.Add("time", time);

            try
            {
                string json = SendData(_url + requestedUri, collection);
                var o = JsonConvert.DeserializeObject<List<SyncFastItems>>(json);
                //TODO proceed data of items
                List<Item> results = new List<Item>();

                foreach (var synObject in o)
                    foreach (var itemFast in synObject.Data)
                    {
                        Item item = new Item();
                        item.Name = itemFast.ItemName;
                        item.OtherId = itemFast._id.ToString();
                        item.Name2 = itemFast.OtherName;
                        item.Code = itemFast.ItemCode;
                        item.Blocked = itemFast.Status == 1 ? 0 : 1;
                        results.Add(item);
                    }

                return results;
            }
            catch (WebException ex)
            {
                //var stre = new StreamReader(ex.Response.GetResponseStream());
                //strOut = stre.ReadToEnd();
                //stre.Close();
            }
            return null;
        }

        public async Task<List<Customer>> getCustomersAsync(string requestedUri, string dFrom, string dTo, string cacheID)
        {
            NameValueCollection collection = new NameValueCollection();
            string time = DateTime.Now.ToString("yyyyMMddHHmmssffffff");
            string data = Base64Encode(@"[{dFrom: '"+ dFrom + "', dTo: '" + dTo + "',cacheID: '" + cacheID + "'}]");
            string key = GenerateKey(ConfigHelper.Password + time + data);

            collection.Add("token", _token);
            collection.Add("form", Base64Encode("getCustomers"));
            collection.Add("key", key);
            collection.Add("data", data);
            collection.Add("userId", ConfigHelper.UserName);
            collection.Add("time", time);

            try
            {
                string json = SendData(_url + requestedUri, collection);
                var o = JsonConvert.DeserializeObject<List<SyncCustomers>>(json);
                //TODO proceed data of items
                List<Customer> results = new List<Customer>();

                foreach (var synObject in o)
                    foreach (var itemFast in synObject.Data)
                    {
                        Customer item = new Customer();
                        item.OtherId = itemFast._id.ToString();
                        item.Code = itemFast.CusCode;
                        item.Name = itemFast.CusName;
                        item.Name2 = itemFast.OtherName;
                        item.Blocked = itemFast.Status == 1 ? 0 : 1;
                        results.Add(item);
                    }

                return results;
            }
            catch (WebException ex)
            {
                //var stre = new StreamReader(ex.Response.GetResponseStream());
                //strOut = stre.ReadToEnd();
                //stre.Close();
            }
            return null;
        }

        public async Task<List<Site>> getSitesAsync(string requestedUri, string dFrom, string dTo, string cacheID)
        {
            NameValueCollection collection = new NameValueCollection();
            string time = DateTime.Now.ToString("yyyyMMddHHmmssffffff");
            string data = Base64Encode(@"[{dFrom: '" + dFrom + "', dTo: '" + dTo + "',cacheID: '" + cacheID + "'}]");
            string key = GenerateKey(ConfigHelper.Password + time + data);

            collection.Add("token", _token);
            collection.Add("form", Base64Encode("getSites"));
            collection.Add("key", key);
            collection.Add("data", data);
            collection.Add("userId", ConfigHelper.UserName);
            collection.Add("time", time);

            try
            {
                string json = SendData(_url + requestedUri, collection);
                var o = JsonConvert.DeserializeObject<List<SyncFastSites>>(json);
                //TODO proceed data of items
                List<Site> results = new List<Site>();

                foreach (var synObject in o)
                    foreach (var itemFast in synObject.Data)
                    {
                        Site item = new Site();
                        item.Name = itemFast.SiteName;
                        item.OtherId = itemFast._id.ToString();
                        item.Name2 = itemFast.OtherName;
                        item.Code = itemFast.SiteCode;
                        item.Blocked = itemFast.Status == 1 ? 0 : 1;
                        results.Add(item);
                    }

                return results;
            }
            catch (WebException ex)
            {
                //var stre = new StreamReader(ex.Response.GetResponseStream());
                //strOut = stre.ReadToEnd();
                //stre.Close();
            }
            return null;
        }

        public async Task<List<StockFast>> getStocksAsync(string requestedUri, string dFrom, string dTo, string cacheID)
        {
            NameValueCollection collection = new NameValueCollection();
            string time = DateTime.Now.ToString("yyyyMMddHHmmssffffff");
            string data = Base64Encode(@"[{dFrom: '" + dFrom + "', dTo: '" + dTo + "',cacheID: '" + cacheID + "'}]");
            string key = GenerateKey(ConfigHelper.Password + time + data);

            collection.Add("token", _token);
            collection.Add("form", Base64Encode("getStocks"));
            collection.Add("key", key);
            collection.Add("data", data);
            collection.Add("userId", ConfigHelper.UserName);
            collection.Add("time", time);

            try
            {
                string json = SendData(_url + requestedUri, collection);
                var o = JsonConvert.DeserializeObject<List<SyncFastStocks>>(json);
                //TODO proceed data of items
                List<StockFast> results = new List<StockFast>();

                foreach (var synObject in o)
                    foreach (var itemFast in synObject.Data)
                    {
                        StockFast item = new StockFast();
                        item.SiteCode = itemFast.SiteCode;
                        item.SiteCode = itemFast.SiteCode;
                        item.Stock = itemFast.Stock;
                        results.Add(item);
                    }

                return results;
            }
            catch (WebException ex)
            {
                //var stre = new StreamReader(ex.Response.GetResponseStream());
                //strOut = stre.ReadToEnd();
                //stre.Close();
            }
            return null;
        }

        //public async Task<List<Structure>> GetLocaltions(string requestedUri)
        //{
        //    NameValueCollection collection = new NameValueCollection();
        //    string time = DateTime.Now.ToString("yyyyMMddHHmmssffffff");
        //    string data = Base64Encode(@"[{dFrom: '01-01-2018', dTo: '01/09/2020',cacheID: '|cacheID|'}]");
        //    string key = GenerateKey(ConfigHelper.Password + time + data);

        //    collection.Add("token", _token);
        //    collection.Add("form", Base64Encode("getSites"));
        //    collection.Add("key", key);
        //    collection.Add("data", data);
        //    collection.Add("userId", ConfigHelper.UserName);
        //    collection.Add("time", time);

        //    try
        //    {
        //        string json = SendData(_url + requestedUri, collection);
        //        var o = JsonConvert.DeserializeObject<List<SyncFastSite>>(json);
        //        //TODO proceed data of items
        //        List<Structure> results = new List<Structure>();

        //        foreach (var synObject in o)
        //            foreach (var itemSite in synObject.Data)
        //            {
        //                Structure item = new Structure();
        //                item.Name = itemSite.SiteName;
        //                item.OtherId = itemSite.SiteCode;
        //                item.Blocked = itemSite.Status == 1 ? 0 : 1;
        //                item.ParentOtherId = "1";
        //                results.Add(item);
        //            }

        //        return results;
        //    }
        //    catch (WebException ex)
        //    {
        //        //var stre = new StreamReader(ex.Response.GetResponseStream());
        //        //strOut = stre.ReadToEnd();
        //        //stre.Close();
        //    }
        //    return null;
        //}

        //public async Task<string> SendTransaction(string requestedUri, IEnumerable<TicketHeader> ticketHeaders)
        //{
        //    NameValueCollection collection = new NameValueCollection();
        //    string time = DateTime.Now.ToString("yyyyMMddHHmmssffffff");
        //    string result = "";



        //    DateTime outDate = new DateTime();

        //    //outDate = DateTime.ParseExact(syncItem.DateCreated, ConfigHelper.DateTimeFormat, null);

        //    List<FastPickList> list = new List<FastPickList>();
        //    foreach (var vt in ticketHeaders)
        //    {
        //        outDate = DateTime.ParseExact(vt.DateCreated, ConfigHelper.DateTimeFormat, null);
        //        var total = ticketHeaders.Where(t => t.Id == vt.Id).Select(t => t.Details.Count()).FirstOrDefault();
        //        var totalweight = ticketHeaders.Where(t => t.Id == vt.Id).Select(t => t.Details.Sum(t2 => t2.Weight)).FirstOrDefault();
        //        var average = ticketHeaders.Where(t => t.Id == vt.Id).Select(t => t.Details.Average(t2 => t2.Weight)).FirstOrDefault();

        //        var o = vt.Details
        //            .Select(sd => new FastPickList()
        //            {
        //                Unit = "",
        //                CusCode = vt.BusinessPartner.Code,
        //                VcNo = vt.DocumentNumber,
        //                VcDate = outDate.ToString("yyyyMMdd"),
        //                Status = 2,
        //                Total = total,
        //                Totalweight = totalweight,
        //                Average = average,
        //                ItemId = sd.Item.Code,
        //                ItemName = "",
        //                UOM = "con",
        //                Quantity = 1,
        //                SiteCode = sd.Structure.OtherId
        //            })
        //            .ToList();
        //        list.AddRange(o);
        //    }
        //    if (list.Count < 1)
        //    {
        //        return "NOTHING TO SYNC";
        //    }
        //    string json = JsonConvert.SerializeObject(list);
        //    string data = Base64Encode(json);
        //    string key = GenerateKey(ConfigHelper.Password + time + data);
        //    collection.Add("data", data);
        //    collection.Add("key", key);
        //    collection.Add("token", _token);
        //    collection.Add("form", Base64Encode("setPicklists"));
        //    collection.Add("userId", ConfigHelper.UserName);
        //    collection.Add("time", time);
        //    try
        //    {
        //        string jsonReturn = SendData(_url + requestedUri, collection);
        //        var resultJson = JsonConvert.DeserializeObject<List<SyncFastContent>>(jsonReturn);

        //        if (resultJson[0].GetType == 1)
        //        {
        //            foreach (var vt in ticketHeaders)
        //            {
        //                TicketSync ticketSync = new TicketSync() { TicketHeaderId = vt.Id, SyncDate = ConfigHelper.GetCurrentDate() };
        //                vt.TicketSyncs.Add(ticketSync);
        //                result += $"{vt.Id} - OK";
        //            }
        //        }
        //        else
        //        {
        //            foreach (var vt in ticketHeaders)
        //                result += $"[{vt.Id} - No OK -{resultJson[0].msg}";
        //        }
        //    }
        //    catch (WebException ex)
        //    {
        //        var stre = new StreamReader(ex.Response.GetResponseStream());
        //        result = stre.ReadToEnd();
        //        stre.Close();
        //    }
        //    return result;
        //}

        //public async Task<List<BusinessPartner>> GetBusinessPartners(string requestUri)
        //{
        //    NameValueCollection collection = new NameValueCollection();
        //    string time = DateTime.Now.ToString("yyyyMMddHHmmssffffff");
        //    string data = Base64Encode(@"[{dFrom: '01-01-2018', dTo: '01/09/2020',cacheID: '|cacheID|'}]");
        //    string key = GenerateKey(ConfigHelper.Password + time + data);

        //    collection.Add("token", _token);
        //    collection.Add("form", Base64Encode("getCustomers"));
        //    collection.Add("key", key);
        //    collection.Add("data", data);
        //    collection.Add("userId", ConfigHelper.UserName);
        //    collection.Add("time", time);

        //    try
        //    {
        //        string json = SendData(_url + requestUri, collection);
        //        var o = JsonConvert.DeserializeObject<List<SyncFastBusinessPartner>>(json);
        //        //TODO proceed data of items
        //        List<BusinessPartner> results = new List<BusinessPartner>();

        //        foreach (var synObject in o)
        //            foreach (var itemBP in synObject.Data)
        //            {
        //                BusinessPartner item = new BusinessPartner();
        //                item.Name = itemBP.CusName;
        //                item.Code = itemBP.CusCode;
        //                item.OtherId = itemBP._id;
        //                item.Blocked = itemBP.Status == "1" ? 0 : 1;
        //                results.Add(item);
        //            }

        //        return results;
        //    }
        //    catch (WebException ex)
        //    {
        //        //var stre = new StreamReader(ex.Response.GetResponseStream());
        //        //strOut = stre.ReadToEnd();
        //        //stre.Close();
        //    }
        //    return null;
        //}
    }
}
