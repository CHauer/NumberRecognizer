//-----------------------------------------------------------------------
// <copyright file="LoadStateEventArgs.cs" company="FH Wr.Neustadt">
//     Copyright Markus Zytek. All rights reserved.
// </copyright>
// <author>Markus Zytek</author>
// <summary>Load State Event Args.</summary>
//-----------------------------------------------------------------------
namespace NumberRecognizer.App.Common
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Class used to hold the event data required when a page attempts to load state.
    /// </summary>
    public class LoadStateEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoadStateEventArgs"/> class.
        /// </summary>
        /// <param name="navigationParameter">The navigation parameter.</param>
        /// <param name="pageState">State of the page.</param>
        public LoadStateEventArgs(object navigationParameter, Dictionary<string, object> pageState)
            : base()
        {
            this.NavigationParameter = navigationParameter;
            this.PageState = pageState;
        }

        /// <summary>
        /// Gets the navigation parameter.
        /// </summary>
        /// <value>
        /// The navigation parameter.
        /// </value>
        public object NavigationParameter { get; private set; }

        /// <summary>
        /// Gets the state of the page.
        /// </summary>
        /// <value>
        /// The state of the page.
        /// </value>
        public Dictionary<string, object> PageState { get; private set; }
    }
}
