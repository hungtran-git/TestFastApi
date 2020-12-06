using System.Collections.Generic;

namespace FastClassLibrary
{
    public class TicketHeader
    {
        public string DateCreated { get; internal set; }
        public object Id { get; internal set; }
        public List<object> Details { get; internal set; }
        public List<object> TicketSyncs { get; internal set; }
    }
}