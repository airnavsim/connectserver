﻿using System;
using System.Collections.Generic;
using System.Globalization;

namespace ExtPlaneNetCore.Commands
{
    public abstract class Command
    {
        public readonly string Name;

        protected Command(string name)
        {
            Name = name;
        }

        public string Build()
        {
            return string.Format(CultureInfo.InvariantCulture, "{0} {1}{2}", Name, FormatParameters(), Environment.NewLine);
        }

        protected abstract string FormatParameters();
    }
}