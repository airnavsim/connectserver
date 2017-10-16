using System;
using System.Collections.Generic;
using System.Text;

namespace Cs
{
    public static class Settings
    {
        public static bool DebugToConsole { get; set; }

        public static SettingsModel.Handlers.SimulatorSettings Simulator { get; set; }

    }
}
