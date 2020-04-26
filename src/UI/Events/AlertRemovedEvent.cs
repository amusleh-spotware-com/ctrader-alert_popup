using Prism.Events;
using System.Collections.Generic;

namespace cAlgo.API.Alert.UI.Events
{
    public class AlertRemovedEvent : PubSubEvent<IEnumerable<Models.AlertModel>>
    {
    }
}