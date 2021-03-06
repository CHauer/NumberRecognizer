﻿//-----------------------------------------------------------------------
// <copyright file="NetworkStatusType.cs" company="FH Wr.Neustadt">
//     Copyright Christoph Hauer. All rights reserved.
// </copyright>
// <author>Christoph Hauer</author>
// <summary>NetworkStatusType Data Contract.</summary>
//-----------------------------------------------------------------------

namespace NumberRecognizer.Cloud.Contract.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// NetworkStatusType Enumeration.
    /// </summary>
    [DataContract]
    public enum NetworkStatusType : int
    {
        /// <summary>
        /// The not started
        /// </summary>
        [EnumMember]
        NotStarted = 1,

        /// <summary>
        /// The running
        /// </summary>
        [EnumMember]
        Running = 2,

        /// <summary>
        /// The ready
        /// </summary>
        [EnumMember]
        Ready = 4,

        /// <summary>
        /// The error
        /// </summary>
        [EnumMember]
        Error = 8
    }
}
