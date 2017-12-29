namespace Alert
{
    using System;

    public class Alert
    {
        #region Properties
        public string TradeSide { get; set; }
        public string Symbol { get; set; }
        public string TimeFrame { get; set; }
        public DateTime Time { get; set; }
        public string Comment { get; set; }
        #endregion
    }
}
