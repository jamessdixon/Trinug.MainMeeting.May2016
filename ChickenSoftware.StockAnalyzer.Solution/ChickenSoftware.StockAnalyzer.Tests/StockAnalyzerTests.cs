using System;
using ChickenSoftware.StockAnalyzer;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChickenSoftware.StockAnalyzer.Tests
{
    [TestClass]
    public class StockAnalyzerTests
    {
        [TestMethod]
        public void GetStockCloseUsingMSFT_ReturnsExpected()
        {
            var analyzer = new StockAnalyzer();
            var price = analyzer.GetStockClose("MSFT");


        }
    }
}
