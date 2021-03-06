﻿using System;
using System.Collections.Generic;
using System.Text;

namespace SpiceSharpParser.Common
{
    public interface ILocationProvider
    {
        /// <summary>
        /// Gets or sets token line number.
        /// </summary>
        int LineNumber { get; }

        /// <summary>
        /// Gets or sets start column index.
        /// </summary>
        int StartColumnIndex { get; }

        /// <summary>
        /// Gets or sets end column index.
        /// </summary>
        int EndColumnIndex { get; }

        /// <summary>
        /// Gets token file name.
        /// </summary>
        string FileName { get; }
    }

}
