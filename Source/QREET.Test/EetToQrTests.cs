using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QREET.Lib;

namespace QREET.Test
{
    [TestClass]
    public class EetToQrTests
    {
        // valid inputs

        // 8+4+4 = 16 symbols
        private const string FikSample     = "a77649538ae3ba50";
        // 8+4+4+4+12+2 = 34 symbols
        private const string FikLongSample = "a77649538ae3ba50123498765432109806";
        // 8+8 = 16 symbols
        private const string BkpSample     = "8ae3ba50a7764953";
        // 8+8+8+8+8 = 40 symbols
        private const string BkpLongSample = "8ae3ba50a7764953789456123456abcd12345678";
        private const string DicSample     = "00685976";
        private const string KcSample      = "340.50";
        private const string DtSample      = "201710171533";

        #region Input failing tests

        [TestMethod]
        public void NoCodeTest()
        {
            bool success = EetReceipt.TryCreate(null, null, DicSample, KcSample, DtSample, null,
                out EetReceipt receipt, out string error);
            Assert.IsFalse(success, error);
            Assert.IsNull(receipt);
        }

        [TestMethod]
        public void InvalidFikTest()
        {
            string shortFik = FikSample.Substring(1);
            bool success = EetReceipt.TryCreate(shortFik, null, DicSample, KcSample, DtSample, null,
                out EetReceipt receipt, out string error);
            Assert.IsFalse(success, error);
            Assert.IsNull(receipt);

            string longFik = FikLongSample + "a";
            bool success2 = EetReceipt.TryCreate(longFik, null, DicSample, KcSample, DtSample, null,
                out EetReceipt receipt2, out string error2);
            Assert.IsFalse(success2, error2);
            Assert.IsNull(receipt2);

            string invalidFik = FikSample + "q";
            bool success3 = EetReceipt.TryCreate(invalidFik, null, DicSample, KcSample, DtSample, null,
                out EetReceipt receipt3, out string error3);
            Assert.IsFalse(success3, error3);
            Assert.IsNull(receipt3);
        }

        [TestMethod]
        public void InvalidBkpTest()
        {
            string shortBkp = BkpSample.Substring(1);
            bool success = EetReceipt.TryCreate(null, shortBkp, DicSample, KcSample, DtSample, null,
                out EetReceipt receipt, out string error);
            Assert.IsFalse(success, error);
            Assert.IsNull(receipt);

            string longBkp = BkpLongSample + "e";
            bool success2 = EetReceipt.TryCreate(null, longBkp, DicSample, KcSample, DtSample, null,
                out EetReceipt receipt2, out string error2);
            Assert.IsFalse(success2, error2);
            Assert.IsNull(receipt2);

            string invalidBkp = BkpSample + "p";
            bool success3 = EetReceipt.TryCreate(null, invalidBkp, DicSample, KcSample, DtSample, null,
                out EetReceipt receipt3, out string error3);
            Assert.IsFalse(success3, error3);
            Assert.IsNull(receipt3);
        }

        [TestMethod]
        public void InvalidDicTest()
        {
            string shortDic = DicSample.Substring(1);
            bool success = EetReceipt.TryCreate(null, BkpSample, shortDic, KcSample, DtSample, null,
                out EetReceipt receipt, out string error);
            Assert.IsFalse(success, error);
            Assert.IsNull(receipt);

            string longDic = "12345678901";
            bool success2 = EetReceipt.TryCreate(null, BkpSample, longDic, KcSample, DtSample, null,
                out EetReceipt receipt2, out string error2);
            Assert.IsFalse(success2, error2);
            Assert.IsNull(receipt2);

            string invalidCharDic = "123456789a";
            bool success3 = EetReceipt.TryCreate(null, BkpSample, invalidCharDic, KcSample, DtSample, null,
                out EetReceipt receipt3, out string error3);
            Assert.IsFalse(success3, error3);
            Assert.IsNull(receipt3);
        }

        [TestMethod]
        public void InvalidKcTest()
        {
            string highKc = "12000000";
            bool success = EetReceipt.TryCreate(null, BkpSample, DicSample, highKc, DtSample, null,
                out EetReceipt receipt, out string error);
            Assert.IsFalse(success, error);
            Assert.IsNull(receipt);

            string lowFractionKc = "100.005";
            bool success2 = EetReceipt.TryCreate(null, BkpSample, DicSample, lowFractionKc, DtSample, null,
                out EetReceipt receipt2, out string error2);
            Assert.IsFalse(success2, error2);
            Assert.IsNull(receipt2);

            string negKc = "-45";
            bool success3 = EetReceipt.TryCreate(null, BkpSample, DicSample, negKc, DtSample, null,
                out EetReceipt receipt3, out string error3);
            Assert.IsFalse(success3, error3);
            Assert.IsNull(receipt3);

            string emptyFractionKc = "456.";
            bool success4 = EetReceipt.TryCreate(null, BkpSample, DicSample, emptyFractionKc, DtSample, null,
                out EetReceipt receipt4, out string error4);
            Assert.IsFalse(success4, error4);
            Assert.IsNull(receipt4);

            string zeroKc = "0";
            bool success5 = EetReceipt.TryCreate(null, BkpSample, DicSample, zeroKc, DtSample, null,
                out EetReceipt receipt5, out string error5);
            Assert.IsFalse(success5, error5);
            Assert.IsNull(receipt5);
        }

        [TestMethod]
        public void InvalidDtTest()
        {
            string lowDt = "201709291420";
            bool success = EetReceipt.TryCreate(null, BkpSample, DicSample, KcSample, lowDt, null,
                out EetReceipt receipt, out string error);
            Assert.IsFalse(success, error);
            Assert.IsNull(receipt);

            string shortDt = "20171015";
            bool success2 = EetReceipt.TryCreate(null, BkpSample, DicSample, KcSample, shortDt, null,
                out EetReceipt receipt2, out string error2);
            Assert.IsFalse(success2, error2);
            Assert.IsNull(receipt2);

            string longDt = "20171015142033";
            bool success3 = EetReceipt.TryCreate(null, BkpSample, DicSample, KcSample, longDt, null,
                out EetReceipt receipt3, out string error3);
            Assert.IsFalse(success3, error3);
            Assert.IsNull(receipt3);
        }

        [TestMethod]
        public void InvalidModeTest()
        {
            bool success = EetReceipt.TryCreate(FikSample, null, DicSample, KcSample, DtSample, EetReceipt.SimplifiedMode,
                out EetReceipt receipt, out string error);
            Assert.IsFalse(success, error);
            Assert.IsNull(receipt);

            string invalidMode = "D";
            bool success2 = EetReceipt.TryCreate(null, BkpSample, DicSample, KcSample, DtSample, invalidMode,
                out EetReceipt receipt2, out string error2);
            Assert.IsFalse(success2, error2);
            Assert.IsNull(receipt2);
        }

        #endregion

        #region Input passing tests

        [TestMethod]
        public void BasicTest()
        {
            bool success = EetReceipt.TryCreate(FikSample, null, DicSample, KcSample, DtSample, null,
                out EetReceipt receipt, out string error);
            Assert.IsTrue(success, error);
            Assert.IsNotNull(receipt);

            bool success2 = EetReceipt.TryCreate(null, BkpSample, DicSample, KcSample, DtSample, null,
                out EetReceipt receipt2, out string error2);
            Assert.IsTrue(success2, error2);
            Assert.IsNotNull(receipt2);

            bool success3 = EetReceipt.TryCreate(FikSample, BkpSample, DicSample, KcSample, DtSample, null,
                out EetReceipt receipt3, out string error3);
            Assert.IsTrue(success3, error3);
            Assert.IsNotNull(receipt3);
        }

        [TestMethod]
        public void FikOkTest()
        {
            bool success = EetReceipt.TryCreate(FikLongSample, null, DicSample, KcSample, DtSample, null,
                out EetReceipt receipt, out string error);
            Assert.IsTrue(success, error);
            Assert.IsNotNull(receipt);

            string midSizeFik = FikSample + "5";
            bool success2 = EetReceipt.TryCreate(midSizeFik, null, DicSample, KcSample, DtSample, null,
                out EetReceipt receipt2, out string error2);
            Assert.IsTrue(success2, error2);
            Assert.IsNotNull(receipt2);
        }

        [TestMethod]
        public void BkpOkTest()
        {
            bool success = EetReceipt.TryCreate(null, BkpLongSample, DicSample, KcSample, DtSample, null,
                out EetReceipt receipt, out string error);
            Assert.IsTrue(success, error);
            Assert.IsNotNull(receipt);

            string midSizeBkp = BkpSample + "9";
            bool success2 = EetReceipt.TryCreate(null, midSizeBkp, DicSample, KcSample, DtSample, null,
                out EetReceipt receipt2, out string error2);
            Assert.IsTrue(success2, error2);
            Assert.IsNotNull(receipt2);
        }

        [TestMethod]
        public void DicOkTest()
        {
            string size10Dic = "1234567890";
            bool success = EetReceipt.TryCreate(null, BkpSample, size10Dic, KcSample, DtSample, null,
                out EetReceipt receipt, out string error);
            Assert.IsTrue(success, error);
            Assert.IsNotNull(receipt);

            string size9Dic = "123456789";
            bool success2 = EetReceipt.TryCreate(null, BkpSample, size9Dic, KcSample, DtSample, null,
                out EetReceipt receipt2, out string error2);
            Assert.IsTrue(success2, error2);
            Assert.IsNotNull(receipt2);
        }

        [TestMethod]
        public void KcOkTest()
        {
            string minKc = "0.01";
            bool success = EetReceipt.TryCreate(null, BkpSample, DicSample, minKc, DtSample, null,
                out EetReceipt receipt, out string error);
            Assert.IsTrue(success, error);
            Assert.IsNotNull(receipt);

            string maxKc = "9999999.99";
            bool success2 = EetReceipt.TryCreate(null, BkpSample, DicSample, maxKc, DtSample, null,
                out EetReceipt receipt2, out string error2);
            Assert.IsTrue(success2, error2);
            Assert.IsNotNull(receipt2);

            string shortFractionKc = "100.5";
            bool success3 = EetReceipt.TryCreate(null, BkpSample, DicSample, shortFractionKc, DtSample, null,
                out EetReceipt receipt3, out string error3);
            Assert.IsTrue(success3, error3);
            Assert.IsNotNull(receipt3);

            string noFractionKc = "42";
            bool success4 = EetReceipt.TryCreate(null, BkpSample, DicSample, noFractionKc, DtSample, null,
                out EetReceipt receipt4, out string error4);
            Assert.IsTrue(success4, error4);
            Assert.IsNotNull(receipt4);
        }

        [TestMethod]
        public void DtOkTest()
        {
            string minDt = "201710010000";
            bool success = EetReceipt.TryCreate(null, BkpSample, DicSample, KcSample, minDt, null,
                out EetReceipt receipt, out string error);
            Assert.IsTrue(success, error);
            Assert.IsNotNull(receipt);

            string futureDt = DateTime.Now.AddYears(1).ToString("yyyyMMddHHmm", CultureInfo.InvariantCulture);
            bool success2 = EetReceipt.TryCreate(null, BkpSample, DicSample, KcSample, futureDt, null,
                out EetReceipt receipt2, out string error2);
            Assert.IsTrue(success2, error2);
            Assert.IsNotNull(receipt2);
        }

        [TestMethod]
        public void ModeTest()
        {
            bool success = EetReceipt.TryCreate(FikSample, null, DicSample, KcSample, DtSample, EetReceipt.CommonMode,
                out EetReceipt receipt, out string error);
            Assert.IsTrue(success, error);
            Assert.IsNotNull(receipt);

            bool success2 = EetReceipt.TryCreate(null, BkpLongSample, DicSample, KcSample, DtSample, EetReceipt.CommonMode,
                out EetReceipt receipt2, out string error2);
            Assert.IsTrue(success2, error2);
            Assert.IsNotNull(receipt2);

            bool success3 = EetReceipt.TryCreate(null, BkpLongSample, DicSample, KcSample, DtSample, EetReceipt.SimplifiedMode,
                out EetReceipt receipt3, out string error3);
            Assert.IsTrue(success3, error3);
            Assert.IsNotNull(receipt3);
        }

        #endregion
    }
}