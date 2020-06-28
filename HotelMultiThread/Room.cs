using System;

namespace HotelMultiThread
{
    public class Room
    {
        public int Number { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public DateTime Now { get; set; }
        public bool Empty { get; set; }

        public int Left
        {
            get
            {
                return (int)(CheckOut - Now).TotalDays;
            }
        }
    }
}