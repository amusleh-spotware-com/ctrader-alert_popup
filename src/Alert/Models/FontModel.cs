using Prism.Mvvm;
using System.Windows.Media;
using System.Xml.Serialization;

namespace cAlgo.API.Alert.Models
{
    public class FontModel : BindableBase
    {
        #region Fields

        private int _size;

        private FontFamily _family;

        private FontWeightModel _weightModel;

        private FontStyleModel _styleModel;

        #endregion Fields

        #region Properties

        public int Size
        {
            get
            {
                return _size;
            }
            set
            {
                SetProperty(ref _size, value);
            }
        }

        [XmlIgnore]
        public FontFamily Family
        {
            get
            {
                return _family;
            }
            set
            {
                SetProperty(ref _family, value);
            }
        }

        public string FamilyName
        {
            get
            {
                return Family.Source;
            }
            set
            {
                Family = new FontFamily(value);
            }
        }

        public FontWeightModel WeightModel
        {
            get
            {
                return _weightModel;
            }
            set
            {
                SetProperty(ref _weightModel, value);
            }
        }

        public FontStyleModel StyleModel
        {
            get
            {
                return _styleModel;
            }
            set
            {
                SetProperty(ref _styleModel, value);
            }
        }

        #endregion Properties
    }
}