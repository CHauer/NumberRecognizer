using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace NumberRecognizer.App.Common
{
    /// <summary>
    /// 
    /// </summary>
    public static class ItemClickCommand
    {
        /// <summary>
        /// The command property
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand),
            typeof(ItemClickCommand), new PropertyMetadata(null, OnCommandPropertyChanged));

        /// <summary>
        /// Sets the command.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="value">The value.</param>
        public static void SetCommand(DependencyObject d, ICommand value)
        {
            d.SetValue(CommandProperty, value);
        }

        /// <summary>
        /// Gets the command.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns></returns>
        public static ICommand GetCommand(DependencyObject d)
        {
            return (ICommand)d.GetValue(CommandProperty);
        }

        /// <summary>
        /// Called when [command property changed].
        /// </summary>
        /// <param name="d">The d.</param>
        /// <param name="e">The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.</param>
        private static void OnCommandPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var control = d as ListViewBase;
            if (control != null)
                control.ItemClick += OnItemClick;
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
                command.Execute(e.ClickedItem);
        }
    }
}
