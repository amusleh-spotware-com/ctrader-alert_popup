using Prism.Mvvm;
using System;

namespace cAlgo.API.Alert.UI.Models
{
    public class TelegramConversation : BindableBase
    {
        #region Fields

        private string _name, _botToken;

        private long _id;

        #endregion Fields

        #region Properties

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                SetProperty(ref _name, value);
            }
        }

        public string BotToken
        {
            get
            {
                return _botToken;
            }
            set
            {
                SetProperty(ref _botToken, value);
            }
        }

        public long Id
        {
            get
            {
                return _id;
            }
            set
            {
                SetProperty(ref _id, value);
            }
        }

        #endregion Properties

        #region Methods

        public static bool operator !=(TelegramConversation obj1, TelegramConversation obj2)
        {
            if (ReferenceEquals(obj1, null))
            {
                return !ReferenceEquals(obj2, null);
            }

            return !obj1.Equals(obj2);
        }

        public static bool operator ==(TelegramConversation obj1, TelegramConversation obj2)
        {
            if (ReferenceEquals(obj1, null))
            {
                return ReferenceEquals(obj2, null);
            }

            return obj1.Equals(obj2);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is TelegramConversation))
            {
                return false;
            }

            return Equals((TelegramConversation)obj);
        }

        public bool Equals(TelegramConversation other)
        {
            return other == null ? false : Name.Equals(other.Name, StringComparison.InvariantCultureIgnoreCase) &&
                BotToken.Equals(other.BotToken, StringComparison.InvariantCultureIgnoreCase) &&
                Id == other.Id;
        }

        public override int GetHashCode()
        {
            int hash = 17;

            hash += (hash * 31) + (!string.IsNullOrEmpty(Name) ? Name.GetHashCode() : 0);
            hash += (hash * 31) + (!string.IsNullOrEmpty(BotToken) ? BotToken.GetHashCode() : 0);
            hash += (hash * 31) + Id.GetHashCode();

            return hash;
        }

        #endregion Methods
    }
}