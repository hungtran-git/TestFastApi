using System.Collections.Generic;

namespace FastClassLibrary
{
    public class StockFast {
        public string ItemCode { get; set; }
        public string SiteCode { get; set; }
        public int Stock { get; set; }
    }
    internal class SyncFastStocks
    {
        public int Type { get; set; }
        public string Msg { get; set; }
        public int Count { get; set; }
        public int Continue { get; set; }
        public string CacheID { get; set; }
        public IEnumerable<StockFast> Data { get; set; }
    }
}