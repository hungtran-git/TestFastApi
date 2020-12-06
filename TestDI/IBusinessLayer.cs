using System;

namespace TestDI
{
    public interface IBusinessLayer
    {
        void PerformBusiness();
    }

    public class BusinessLayer : IBusinessLayer
    {
        public void PerformBusiness()
        {
            Console.WriteLine("Perform business");
        }
    }
}