using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace NumberRecognizer.App.Common
{
    /// <summary>
    /// Wertkonverter, der TRUE in <see cref="Visibility.Visible"/> und FALSE in
    /// <see cref="Visibility.Collapsed"/> übersetzt.
    /// </summary>
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Ändert die Quelldaten vor der Übergabe an das Ziel zur Anzeige in der Benutzeroberfläche.
        /// </summary>
        /// <param name="value">Die Quelldaten, die ans Ziel übergeben werden.</param>
        /// <param name="targetType">Der Typ der Zieleigenschaft. Dadurch wird ein anderer Typ verwendet, abhängig davon, ob Sie mit Microsoft .NET oder Visual&amp;nbsp;C++-Komponentenerweiterungen (C++/CX) programmieren. Siehe Hinweise.</param>
        /// <param name="parameter">Ein optionaler Parameter, der in der Konverterlogik verwendet wird.</param>
        /// <param name="language">Die Sprache der Konvertierung.</param>
        /// <returns>
        /// Der Wert, der an die Zielabhängigkeitseigenschaft übergeben werden soll.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter != null)
            {
                return (value is bool && (bool)value) ? Visibility.Collapsed : Visibility.Visible;
            }
            return (value is bool && (bool)value) ? Visibility.Visible : Visibility.Collapsed;
        }

        /// <summary>
        /// Ändert die Zieldaten vor der Übergabe an das Quellobjekt. Diese Methode wird nur in TwoWay-Bindungen aufgerufen.
        /// </summary>
        /// <param name="value">Die Zieldaten, die an die Quelle übergeben werden.</param>
        /// <param name="targetType">Der Typ der Zieleigenschaft, angegeben durch eine Hilfestruktur, die den Typnamen umschließt.</param>
        /// <param name="parameter">Ein optionaler Parameter, der in der Konverterlogik verwendet wird.</param>
        /// <param name="language">Die Sprache der Konvertierung.</param>
        /// <returns>
        /// Der Wert, der an das Quellobjekt weitergeleitet wird.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return value is Visibility && (Visibility)value == Visibility.Visible;
        }
    }
}
