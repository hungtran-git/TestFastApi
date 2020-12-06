using System.Collections.Generic;
using System;

namespace FastClassLibrary
{
    public class Item
    {
        public object Name { get; set; }
        public object OtherId { get; set; }
        public object Name2 { get; set; }
        public object Code { get; set; }
        public int Blocked { get; set; }
    }

    public class ItemFast
    {
        public int _id { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string OtherName { get; set; }
        public int Status { get; set; }
        public string Modified { get; set; }
    }
    public class SyncFastItems
    {
        public int Type { get; set; }
        public string Msg { get; set; }
        public int Count { get; set; }
        public int Continue { get; set; }
        public string CacheID { get; set; }
        public List<ItemFast> Data { get; set; }
        //public string Data { get; set; }
    }
}