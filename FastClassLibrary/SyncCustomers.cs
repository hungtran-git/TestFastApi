using System.Collections.Generic;
using System;

namespace FastClassLibrary
{
    public class Customer
    {
        public object Name { get; set; }
        public object OtherId { get; set; }
        public object Name2 { get; set; }
        public object Code { get; set; }
        public int Blocked { get; set; }
    }

    public class CustomerFast
    {
        public int _id { get; set; }
        public string CusCode { get; set; }
        public string CusName { get; set; }
        public string OtherName { get; set; }
        public int Status { get; set; }
        public string Modified { get; set; }
    }
    public class SyncCustomers
    {
        public int Type { get; set; }
        public string Msg { get; set; }
        public int Count { get; set; }
        public int Continue { get; set; }
        public string CacheID { get; set; }
        public List<CustomerFast> Data { get; set; }
        //public string Data { get; set; }
    }
}