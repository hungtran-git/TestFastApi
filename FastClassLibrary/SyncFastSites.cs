using System;
using System.Collections.Generic;

namespace FastClassLibrary
{
    public class Site
    {
        public object Name { get; internal set; }
        public object OtherId { get; internal set; }
        public object Name2 { get; internal set; }
        public object Code { get; internal set; }
        public int Blocked { get; internal set; }
    }
    public class SiteFast {
        public int _id { get; set; }
        public string SiteCode { get; set; }
        public string SiteName { get; set; }
        public string OtherName { get; set; }
        public int Status { get; set; }
        public string Modified { get; set; }
    }
    internal class SyncFastSites
    {
        public int Type { get; set; }
        public string Msg { get; set; }
        public int Count { get; set; }
        public int Continue { get; set; }
        public string CacheID { get; set; }
        public IEnumerable<SiteFast> Data { get; set; }
    }
}