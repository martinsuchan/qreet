using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QREET.Lib;

namespace QREET.Test
{
    [TestClass]
    public class QrToEetTests
    {
        // valid inputs

        private const string ValidQREET = "EET*1.0*BKP:8AE3BA50A7764953*DIC:00685976*KC:650.35*DT:201710051327";

        #region Input failing tests

        [TestMethod]
        public void BasicFailingTest()
        {
            bool success = EetReceipt.TryCreate(null, out EetReceipt receipt, out string error);
            Assert.IsFalse(success, error);
            Assert.IsNull(receipt);

            bool success2 = EetReceipt.TryCreate("", out EetReceipt receipt2, out string error2);
            Assert.IsFalse(success2, error2);
            Assert.IsNull(receipt2);

            bool success3 = EetReceipt.TryCreate("  ", out EetReceipt receipt3, out string error3);
            Assert.IsFalse(success3, error3);
            Assert.IsNull(receipt3);

            bool success4 = EetReceipt.TryCreate("asdf", out EetReceipt receipt4, out string error4);
            Assert.IsFalse(success4, error4);
            Assert.IsNull(receipt4);
        }

        #endregion

        #region Input passing tests

        [TestMethod]
        public void BasicTest()
        {
            bool success = EetReceipt.TryCreate(ValidQREET, out EetReceipt receipt, out string error);
            Assert.IsTrue(success, error);
            Assert.IsNotNull(receipt);

            Assert.AreEqual(receipt.BKP, "8AE3BA50A7764953");
            Assert.AreEqual(receipt.DIC, "00685976");
            Assert.AreEqual(receipt.Price, 650.35m);
            Assert.AreEqual(receipt.Date, new DateTime(2017, 10, 5, 13, 27, 0));
            Assert.AreEqual(receipt.Mode, EetSaleMode.Common);

            string newQREET = receipt.ToQREETString();
            Assert.AreEqual(ValidQREET, newQREET);
        }

        #endregion
    }
}