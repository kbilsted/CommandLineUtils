﻿// Copyright (c) Nate McMaster.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// This file has been modified from the original form. See Notice.txt in the project root for more information.

using System;
using System.IO;

namespace McMaster.Extensions.CommandLineUtils
{
    public interface IConsole
    {
        /// <summary> Raised when Ctrl+C is pressed. </summary>
        event ConsoleCancelEventHandler? CancelKeyPress;

        TextWriter Out { get; }

        TextWriter Error { get; }

        TextReader In { get; }

        /// <summary> Is stdin piped from somewhere? </summary>
        bool IsInputRedirected { get; }

        /// <summary> Is stdout being piped to somewhere? </summary>
        bool IsOutputRedirected { get; }

        /// <summary> Is stderr being piped to somewhere? </summary>
        bool IsErrorRedirected { get; }

        /// <summary> The foreground color of output. </summary>
        ConsoleColor ForegroundColor { get; set; }

        /// <summary> The background color of output. </summary>
        ConsoleColor BackgroundColor { get; set; }

        /// <summary> Resets <see cref="ForegroundColor"/> and <see cref="BackgroundColor"/>. </summary>
        void ResetColor();
    }
}
