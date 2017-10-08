using System;
using System.Globalization;
using System.Linq;

namespace QREET.Lib
{
    public static class EetConverter
    {
        private static readonly DateTime minDate = new DateTime(2017, 10, 1, 0, 0, 0);

        public static EetBinData ToBinData(this EetData eet)
        {
            EetBinData data = new EetBinData();

            ulong dic = eet.DIC & 0x0000_007f_ffff_ffff;
            data.dic = BitConverter.GetBytes(dic).Take(5).ToArray();

            data.bkp = eet.BKP.Replace("-", "").Substring(0, 16).ToByteArray();

            data.price = BitConverter.GetBytes((uint)(eet.Price * 100)).ToArray();

            TimeSpan ts = eet.Date - minDate;
            data.date = BitConverter.GetBytes((uint)ts.TotalMinutes).Take(3).ToArray();

            return data;
        }

        public static byte[] ToByteArray(this string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "The binary key cannot have an odd number of digits: {0}", hexString));
            }

            byte[] hexAsBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < hexAsBytes.Length; i++)
            {
                string byteValue = hexString.Substring(i * 2, 2);
                hexAsBytes[i] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }
            return hexAsBytes;
        }
    }
}