using System;
using System.Linq;
using System.Windows;

namespace cAlgo.API.Alert.Behaviors
{
    public static class ScrollToItemBehavior
    {
        public static readonly DependencyProperty ItemProperty = DependencyProperty.RegisterAttached(
            "Item",
            typeof(object),
            typeof(ScrollToItemBehavior),
            new PropertyMetadata(null, OnItemChange));

        public static object GetItem(DependencyObject source)
        {
            return (object)source.GetValue(ItemProperty);
        }

        public static void SetItem(DependencyObject source, object value)
        {
            source.SetValue(ItemProperty, value);
        }

        private static void OnItemChange(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            d.GetType().GetMethods().First(method => method.Name.Equals("ScrollIntoView", StringComparison.InvariantCultureIgnoreCase) &&
                method.GetParameters().Count() == 1).Invoke(d, new object[] { e.NewValue });
        }
    }
}