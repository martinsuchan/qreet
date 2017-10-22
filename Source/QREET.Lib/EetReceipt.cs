using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace QREET.Lib
{
    /// <summary>
    /// Model representing all necessary information for checking or registering the EET Receipt.
    /// </summary>
    public class EetReceipt
    {
        public string FIK { get; private set; }
        public string BKP { get; private set; }
        public string DIC { get; private set; }
        public decimal Price { get; private set; }
        public DateTime Date { get; private set; }
        public EetSaleMode Mode { get; private set; }

        public const string EETPrefix = "EET*1.0*";
        public const string FIKCode = "FIK";
        public const string BKPCode = "BKP";
        public const string DICCode = "DIC";
        public const string PriceCode = "KC";
        public const string DateCode = "DT";
        public const string ModeCode = "R";
        public const string CommonMode = "B";
        public const string SimplifiedMode = "Z";

        // min length: 8+4+4 = 16 symbols, max length: 8+4+4+4+12+2 = 34 symbols
        private static readonly Regex FIKRegex = new Regex("(?i)^[0-9A-F]{16,34}$");
        // min length: 8+8   = 16 symbols, max length: 8+8+8+8+8    = 40 symbols
        private static readonly Regex BKPRegex = new Regex("(?i)^[0-9A-F]{16,40}$");
        private static readonly Regex DICRegex = new Regex("^[0-9]{8,10}$");
        private static readonly Regex PriceRegex = new Regex(@"^[0-9]{1,10}(\.[0-9]{1,2})?$");
        private const decimal PriceMin = 0.01m;
        private const decimal PriceMax = 9_999_999.99m;
        private static readonly Regex DateTimeRegex = new Regex("^[0-9]{12}$");
        private static readonly DateTime DateTimeMin = new DateTime(2017, 10, 1);

        private EetReceipt() {}

        public static bool TryCreate(string qreet, out EetReceipt receipt, out string message)
        {
            if (string.IsNullOrWhiteSpace(qreet) || !qreet.StartsWith(EETPrefix))
            {
                receipt = null;
                message = $"qreet must start with {EETPrefix}";
                return false;
            }

            Dictionary<string, string> pairs = qreet.Substring(EETPrefix.Length)
                .Split(new[] { '*' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Split(new[] { ':' }, StringSplitOptions.RemoveEmptyEntries))
                .Where(p => p.Length == 2)
                .ToDictionary(p => p[0], p => p[1]);

            pairs.TryGetValue(FIKCode, out string fik);
            pairs.TryGetValue(BKPCode, out string bkp);
            pairs.TryGetValue(DICCode, out string dic);
            pairs.TryGetValue(PriceCode, out string kc);
            pairs.TryGetValue(DateCode, out string dt);
            pairs.TryGetValue(ModeCode, out string mode);

            return TryCreate(fik, bkp, dic, kc, dt, mode, out receipt, out message);
        }

        public static bool TryCreate(string fik, string bkp, string dic, string kc, string dt, string mode,
            out EetReceipt receipt, out string message)
        {
            receipt = null;
            EetReceipt temp = new EetReceipt();

            if (!string.IsNullOrEmpty(fik))
            {
                if (!FIKRegex.IsMatch(fik))
                {
                    message = "FIK must be [0-9A-F]{16,42}";
                    return false;
                }
                temp.FIK = fik.ToUpper();
            }
            if (!string.IsNullOrEmpty(bkp))
            {
                if (!BKPRegex.IsMatch(bkp))
                {
                    message = "BKP must be [0-9A-F]{16,40}";
                    return false;
                }
                temp.BKP = bkp.ToUpper();
            }
            if (temp.BKP == null && temp.FIK == null)
            {
                message = "Either FIK or BKP must be a valid value";
                return false;
            }

            if (string.IsNullOrEmpty(dic) || !DICRegex.IsMatch(dic))
            {
                message = "DIC must be [0-9]{8,10}";
                return false;
            }
            temp.DIC = dic;

            if (string.IsNullOrEmpty(kc) || !PriceRegex.IsMatch(kc) || kc.Length > 10)
            {
                message = @"Price must be [0-9]{1,10}(\.[0-9]{1,2})? and max 10 chars";
                return false;
            }
            temp.Price = decimal.Parse(kc, CultureInfo.InvariantCulture);
            if (temp.Price < PriceMin || temp.Price > PriceMax)
            {
                message = $"Price must be >= {PriceMin} and <= {PriceMax}";
                return false;
            }

            if (string.IsNullOrEmpty(dt) || !DateTimeRegex.IsMatch(dt))
            {
                message = "DateTime must be [0-9]{12} - yyyyMMddHHmm";
                return false;
            }
            temp.Date = DateTime.ParseExact(dt, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
            if (temp.Date < DateTimeMin)
            {
                message = $"DateTime must be >= {DateTimeMin}";
                return false;
            }

            switch (mode)
            {
                case SimplifiedMode:
                    temp.Mode = EetSaleMode.Simplified;
                    break;
                case CommonMode:
                case null:
                    temp.Mode = EetSaleMode.Common;
                    break;
                default:
                    message = $"Mode must be either null, {SimplifiedMode} or {CommonMode}";
                    return false;
            }

            if (temp.Mode == EetSaleMode.Simplified && temp.BKP == null)
            {
                message = "When Simplified mode is used, BKP must be provided";
                return false;
            }

            // all tests passed, return the object
            receipt = temp;
            message = null;
            return true;
        }

        public string ToQREETString()
        {
            Dictionary<string, string> pairs = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(BKP))
            {
                pairs[BKPCode] = BKP.Substring(0, 16);
            }
            else
            {
                pairs[FIKCode] = FIK.Substring(0, 16);
            }

            pairs[DICCode] = DIC;

            // see decimal.ToString https://msdn.microsoft.com/cs-cz/library/fzeeb5cd(v=vs.110).aspx
            pairs[PriceCode] = Price.ToString("0.##", CultureInfo.InvariantCulture);

            // see DateTime.ToString https://msdn.microsoft.com/cs-cz/library/zdtaw1bw(v=vs.110).aspx
            pairs[DateCode] = Date.ToString("yyyyMMddHHmm", CultureInfo.InvariantCulture);

            if (Mode == EetSaleMode.Simplified)
            {
                pairs[ModeCode] = "Z";
            }

            string pairsString = string.Join("*", pairs.Select(p => $"{p.Key}:{p.Value}"));

            string qreet = EETPrefix + pairsString;
            return qreet;
        }
    }
}