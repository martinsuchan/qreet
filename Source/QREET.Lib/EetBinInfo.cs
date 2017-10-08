namespace QREET.Lib
{
    /// <summary>
    /// Model obsahující binární data reprezentující všechny potřebné údaje pro registraci EET účtenky.
    /// </summary>
    public class EetBinData
    {
        /// <summary>
        /// DIČ bez předpony CZ, 8-10 číslic, po převedení na ulong použito posledních 34 bitů ~ 5 bajtů.
        /// </summary>
        public byte[] dic;   // 5 bytes

        /// <summary>
        /// První dva bloky BKP kódu, 16 hexadecimálních symbolů, celkem 8 bajtů.
        /// </summary>
        public byte[] bkp;   // 8 bytes

        /// <summary>
        /// Cena transakce, zapsaná jako decimal hodnota * 100, po převedení na uint použity 4 bajty.
        /// Maximální hodnota transakce je (2^32)/100 = 42 949 672,95 Kč
        /// </summary>
        public byte[] price; // 4 bytes - max 42.949.672,95

        /// <summary>
        /// Datum transakce, vyjádřené jako počet minut od 1. října 2017, po převedení na uint použity spodní 3 bajty.
        /// Minimální datum transakce je 1. října 2017, 0:00. Maximální datum transakce je min+2^24 minut = 24. srpna 2049, 20:16
        /// </summary>
        public byte[] date;  // 3 bytes - max 24.08.2049 20:16:00

        public byte[] GetBytes()
        {
            byte[] result = new byte[20];
            dic.CopyTo(result, 0);
            bkp.CopyTo(result, 5);
            price.CopyTo(result, 13);
            date.CopyTo(result, 17);
            return result;
        }
    }
}