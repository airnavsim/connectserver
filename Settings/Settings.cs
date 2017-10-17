using System;
using System.Collections.Generic;
using System.Text;

namespace Cs
{
    public static class Settings
    {
        public static bool DebugToConsole { get; set; }

        public static SettingsModel.Models.SimulatorSettings Simulator { get; set; }
        public static SettingsModel.Models.FoldersSettings Folders { get; set; }
        public static SettingsModel.Models.MariaDbSettings Database { get; set; }
        public static Model.Settings.VersionInformationModel VersionInformation { get; set; }

        public static Model.Settings.GlobalSettingsFileServerInfoModel Server { get; set; }
        
        public static Model.Data.DataModel Data { get; set; }

    }
}
