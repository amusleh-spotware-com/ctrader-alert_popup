using Prism.Mvvm;

namespace cAlgo.API.Alert.UI.Models
{
    public class EmailOptionsModel : BindableBase
    {
        #region Fields

        private bool _isEnabled;

        private string _sender, _recipient;

        private EmailTemplateModel _template, _defaultTemplate;

        #endregion Fields

        #region Properties

        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                SetProperty(ref _isEnabled, value);
            }
        }

        public string Sender
        {
            get
            {
                return _sender;
            }
            set
            {
                SetProperty(ref _sender, value);
            }
        }

        public string Recipient
        {
            get
            {
                return _recipient;
            }
            set
            {
                SetProperty(ref _recipient, value);
            }
        }

        public EmailTemplateModel Template
        {
            get
            {
                return _template;
            }
            set
            {
                SetProperty(ref _template, value);
            }
        }

        public EmailTemplateModel DefaultTemplate
        {
            get
            {
                return _defaultTemplate;
            }
            set
            {
                SetProperty(ref _defaultTemplate, value);
            }
        }

        #endregion Properties
    }
}