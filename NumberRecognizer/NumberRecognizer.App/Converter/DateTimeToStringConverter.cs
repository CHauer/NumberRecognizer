//-----------------------------------------------------------------------
// <copyright file="DateTimeToStringConverter.cs" company="FH Wr.Neustadt">
//     Copyright Markus Zytek. All rights reserved.
// </copyright>
// <author>Markus Zytek</author>
// <summary>DateTime To String Converterer.</summary>
//-----------------------------------------------------------------------
namespace NumberRecognizer.App.Converter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Windows.UI.Xaml.Data;

    /// <summary>
    /// DateTime To String Converter.
    /// </summary>
    public class DateTimeToStringConverter : IValueConverter
    {
        /// <summary>
        /// Modifies the source data before passing it to the target for display in the UI.
        /// </summary>
        /// <param name="value">The source data being passed to the target.</param>
        /// <param name="targetType">The type of the target property. This uses a different type depending on whether you're programming with Microsoft .NET or Visual C++ component extensions (C++/CX). See Remarks.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="language">The language of the conversion.</param>
        /// <returns>
        /// The value to be passed to the target dependency property.
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value != null)
            {
                DateTime dateTime = (DateTime)value;
                return dateTime.ToString("dd.MM.yyyy HH:mm:ss");
            }

            return string.Empty;
        }

        /// <summary>
        /// Modifies the target data before passing it to the source object. This method is called only in TwoWay bindings.
        /// </summary>
        /// <param name="value">The target data being passed to the source.</param>
        /// <param name="targetType">The type of the target property, specified by a helper structure that wraps the type name.</param>
        /// <param name="parameter">An optional parameter to be used in the converter logic.</param>
        /// <param name="language">The language of the conversion.</param>
        /// <returns>
        /// The value to be passed to the source object.
        /// </returns>
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
