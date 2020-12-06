using System;

namespace FastClassLibrary
{
    public class AppConfig
    {
        public string ConnectionString { get; set; }
        public DateTime GetCurrentDate { get {
                return DateTime.Now;
            } 
        }
    }
}
