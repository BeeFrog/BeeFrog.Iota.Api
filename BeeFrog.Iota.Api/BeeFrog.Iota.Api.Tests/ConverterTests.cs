using System;
using System.Collections.Generic;
using System.Text;
using BeeFrog.Iota.Api.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace BeeFrog.Iota.Api.Tests
{
    [TestClass]
    public class ConverterTests
    {
        [TestMethod]
        public void ConvertAsciiToTrytes()
        {
            Assert.AreEqual(Converter.AsciiToTrytes("HELLOWORLD"), "RBOBVBVBYBFCYBACVBNB");
        }

        [TestMethod]
        public void ConvertTrytesToAscii()
        {
            Assert.AreEqual(Converter.TrytesToAscii("RBOBVBVBYBFCYBACVBNB"),"HELLOWORLD");
        }
    }
}
