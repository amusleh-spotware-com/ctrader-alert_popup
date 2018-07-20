using System;

namespace Alert
{
    public class Alert
    {
        #region Properties

        public string TradeSide { get; set; }
        public string Symbol { get; set; }
        public string TimeFrame { get; set; }
        public DateTimeOffset Time { get; set; }
        public string Comment { get; set; }

        #endregion Properties
    }
}