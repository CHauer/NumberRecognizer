//-----------------------------------------------------------------------
// <copyright file="SaveStateEventArgs.cs" company="FH Wr.Neustadt">
//     Copyright Markus Zytek. All rights reserved.
// </copyright>
// <author>Markus Zytek</author>
// <summary>Save State Event Args.</summary>
//-----------------------------------------------------------------------
namespace NumberRecognizer.App.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Class used to hold the event data required when a page attempts to save state.
    /// </summary>
    public class SaveStateEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SaveStateEventArgs"/> class.
        /// </summary>
        /// <param name="pageState">An empty dictionary to be populated with serializable state.</param>
        public SaveStateEventArgs(Dictionary<string, object> pageState)
            : base()
        {
            this.PageState = pageState;
        }

        /// <summary>
        /// Gets the state of the page.
        /// </summary>
        /// <value>
        /// The state of the page.
        /// </value>
        public Dictionary<string, object> PageState { get; private set; }
    }
}
