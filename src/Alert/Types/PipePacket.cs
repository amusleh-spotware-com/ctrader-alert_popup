namespace cAlgo.API.Alert.Types
{
    public class PipePacket
    {
        #region Properties

        /// <summary>
        /// The packet type
        /// </summary>
        public Enums.PipePacketType PacketType { get; set; }

        /// <summary>
        /// The packet data
        /// </summary>
        public string Data { get; set; }

        #endregion Properties
    }
}