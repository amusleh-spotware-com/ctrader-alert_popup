using Prism.Mvvm;

namespace cAlgo.API.Alert.Models
{
    public class EmailTemplateModel : BindableBase
    {
        #region Fields

        private string _subject, _body;

        #endregion Fields

        #region Properties

        public string Subject
        {
            get
            {
                return _subject;
            }
            set
            {
                SetProperty(ref _subject, value);
            }
        }

        public string Body
        {
            get
            {
                return _body;
            }
            set
            {
                SetProperty(ref _body, value);
            }
        }

        #endregion Properties
    }
}