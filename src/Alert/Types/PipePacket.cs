namespace cAlgo.API.Alert.Types
{
    public class PipePacket
    {
        public Enums.PipePacketType PacketType { get; set; }

        public string XmlData { get; set; }
    }
}