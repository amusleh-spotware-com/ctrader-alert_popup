using Prism.Events;

namespace cAlgo.API.Alert.Events
{
    public class TelegramSettingsChangedEvent : PubSubEvent<Models.TelegramSettingsModel>
    {
    }
}