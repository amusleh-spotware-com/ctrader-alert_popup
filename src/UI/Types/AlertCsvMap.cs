using CsvHelper.Configuration;

namespace cAlgo.API.Alert.UI.Types
{
    public class AlertCsvMap : ClassMap<Models.AlertModel>
    {
        public AlertCsvMap()
        {
            Map(alertModel => alertModel.Symbol);
            Map(alertModel => alertModel.Time);
            Map(alertModel => alertModel.TimeFrame);
            Map(alertModel => alertModel.TradeSide);
            Map(alertModel => alertModel.TriggeredBy);
            Map(alertModel => alertModel.Price);
            Map(alertModel => alertModel.Comment);
        }
    }
}