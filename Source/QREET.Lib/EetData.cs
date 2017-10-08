using System;
using System.Diagnostics;

namespace QREET.Lib
{
    public class EetData
    {
        public string BKP { get; set; }
        public ulong DIC { get; set; }
        public decimal Price { get; set; }
        public DateTime Date { get; set; }

        public void Validate()
        {
           // TODO 
        }

        public EetData()
        {
            BKP = "0a1a530a-508d8b2a-87af04aa-9d09680c-d04f542d";
            DIC = 00685976;
            Price = 659.73m;
            Date = new DateTime(2017, 10, 1, 14, 40, 0);

            // DIC = 8-10 digits
            DIC = 8_563_111_370;

            // price
            // stored as Price*100 => uint
            // max price = 42.949.672,95;
            decimal maxPrice = (decimal) uint.MaxValue/10;
            Debug.Assert(Price <= maxPrice);
            uint price = (uint)(Price * 100);

            // date
            // minutes since 2017-10-01 00:00:00
            // max 2049-08-24 20:16:00
            DateTime minDate = new DateTime(2017, 10, 1, 0, 0, 0);
            DateTime maxDate = new DateTime(2049, 8, 24, 20, 16, 0);
            Debug.Assert(Date >= minDate && Date <= maxDate);
            uint date = (uint)(Date - minDate).TotalMinutes;
        }
    }
}