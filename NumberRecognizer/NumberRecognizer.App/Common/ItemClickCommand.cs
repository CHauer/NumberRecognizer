//-----------------------------------------------------------------------
// <copyright file="ItemClickCommand.cs" company="FH Wr.Neustadt">
//     Copyright Markus Zytek. All rights reserved.
// </copyright>
// <author>Markus Zytek</author>
// <summary>Item Click Command.</summary>
//-----------------------------------------------------------------------
namespace NumberRecognizer.App.Common
{
    using System.Windows.Input;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Item Click Command.
    /// </summary>
    public static class ItemClickCommand
    {
        /// <summary>
        /// The command property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(ItemClickCommand), new PropertyMetadata(null, OnCommandPropertyChanged));

        /// <summary>
        /// Sets the command.
        /// </summary>
        /// <param name="d">The dependent.</param>
        /// <param name="value">The value.</param>
        public static void SetCommand(DependencyObject d, ICommand value)
        {
            d.SetValue(CommandProperty, value);
        }

        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <param name="d">The dependent.</param>
        /// <returns>The command.</returns>
        public static ICommand GetCommand(DependencyObject d)
        {
            return (ICommand)d.GetValue(CommandProperty);
        }

        /// <summary>
        /// Called when [command property changed].
        /// </summary>
        /// <param name="d">The dependent.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as ListViewBase;
            if (control != null)
            {
                control.ItemClick += OnItemClick;
            }
        }

        /// <summary>
        /// Called when [item click].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="ItemClickEventArgs"/> instance containing the event data.</param>
        private static void OnItemClick(object sender, ItemClickEventArgs e)
        {
            var control = sender as ListViewBase;
            var command = GetCommand(control);

            if (command != null && command.CanExecute(e.ClickedItem))
            {
                command.Execute(e.ClickedItem);
            }
        }
    }
}
