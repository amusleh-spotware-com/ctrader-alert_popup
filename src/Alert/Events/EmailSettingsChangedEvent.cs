using Prism.Events;

namespace cAlgo.API.Alert.Events
{
    public class EmailSettingsChangedEvent : PubSubEvent<Models.EmailSettingsModel>
    {
    }
}