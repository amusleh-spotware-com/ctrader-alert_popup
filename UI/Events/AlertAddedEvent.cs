using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Events;

namespace cAlgo.API.Alert.UI.Events
{
    public class AlertAddedEvent : PubSubEvent<Models.AlertModel>
    {
    }
}