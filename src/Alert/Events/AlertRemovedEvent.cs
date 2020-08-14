using Prism.Events;
using System.Collections.Generic;

namespace cAlgo.API.Alert.Events
{
    public class AlertRemovedEvent : PubSubEvent<IEnumerable<Models.AlertModel>>
    {
    }
}