using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;

using Cs.Model.Settings;

namespace Cs.Software.Handlers
{
    public class SettingsHandler
    {
        
        
        public SettingsHandlerReturnModel ReturnData { get; set; }
        public Cs.Debug Debug { get; set; }

        public Boolean Init()
        {
            this.ReturnData = new SettingsHandlerReturnModel();

            //  Get os information.
            //https://stackoverflow.com/questions/38790802/determine-operating-system-in-net-core


            //  Set default configure folders.
            Settings.Folders = new SettingsModel.Models.FoldersSettings()
            {
                Main = Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), "Documents", "efasim"),
                Temp = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), "efasim")
            };



            #region Test code to get environment folders
            //var dsfdf = Environment.GetEnvironmentVariable("LocalAppData");
            //var fgfdg = Environment.GetEnvironmentVariable("USERPROFILE");
            //var dfgfd = Environment.GetEnvironmentVariable("TEMP");

            //// Debug.Info("test");
            //var dsfdsf = Environment.GetEnvironmentVariable(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "LocalAppData" : "Home");
            //var dfdfgfdggfd = Environment.GetEnvironmentVariable("MYDOCUMENT");
            //var dfgfdg = Environment.GetEnvironmentVariable("TEMP");

            //var dsfdsfdsfdsf = Environment.GetEnvironmentVariable(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "CommonDocuments" : "Home");
            //var dsfsdf = Environment.GetEnvironmentVariable(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "CommonDesktopDirectory" : "Home");
            //var sgfb = Environment.GetEnvironmentVariable(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "MyDocuments" : "Home");
            //var ettgd = Environment.GetEnvironmentVariable(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "MyDocuments" : "Home");
            //// var dsfsdgg = Environment.SpecialFolder.Personal


            ////var sddsge = Environment.GetFolderPath("df");

            ////foreach (Environment.SpecialFolder sf in Enum.GetValues(typeof(System.Environment.SpecialFolder)))
            ////{
            ////    sb.AppendLine($"{sf.ToString()}, {Environment.GetFolderPath(sf)}");
            ////}

            //var enumerator = Environment.GetEnvironmentVariables().GetEnumerator();
            //while (enumerator.MoveNext())
            //{
            //    Console.WriteLine($"{enumerator.Key,5}:{enumerator.Value,100}");
            //}
            #endregion


            #region Check if folders exist ||  settings and temp folder
            
            //  Check if "main" settings folder exist
            if (!Directory.Exists(Settings.Folders.Main))
            {
                //  Folder dont exist.
                ReturnData.ErrorCode = "notfound|configurationfolder";
                ReturnData.ErrorInConfig = true;
                Debug.Error($"Folder settings: {Settings.Folders.Main} don't exist");
            }
            else
                Debug.Info($"Folder settings: {Settings.Folders.Main} Exist");

            //  Check if "temp" folder exist
            if (!Directory.Exists(Settings.Folders.Temp))
            {
                if (Directory.Exists(Path.Combine(Environment.GetEnvironmentVariable("TEMP"))))
                {
                    //  Create efasim folder
                    DirectoryInfo di = Directory.CreateDirectory(Settings.Folders.Temp);
                    Debug.Info($"Folder Temp: {Settings.Folders.Temp} Created");
                }
                else
                {
                    //  Folder dont exist.
                    ReturnData.ErrorCode = "notfound|tempfolder";
                    ReturnData.ErrorInConfig = true;
                    Debug.Error($"Folder Temp: {Settings.Folders.Temp} don't exist");
                }

            }
            else
                Debug.Info($"Folder Temp: {Settings.Folders.Temp} Exist");

            #endregion

            if (!this.GlobalSettingsFileRead())
            {
                return false;
            }


            return true;

        }

        private Boolean GlobalSettingsFileRead()
        {
            //  Do file exist.
            var tmpGlobalFile = Path.Combine(Settings.Folders.Main, "global.cfg");
            if (!File.Exists(tmpGlobalFile))
            {
                //  File Dont exist.
                ReturnData.ErrorInConfig = true;
                ReturnData.ErrorCode = "filenotfound|global.cfg";
                Debug.Error("Global.cfg dont exist. Reinstall software");
                return false;
            }

            string readText = File.ReadAllText(tmpGlobalFile);
            GlobalSettingsFileModel FileSettingsModel = JsonConvert.DeserializeObject<GlobalSettingsFileModel>(readText);
            Settings.Database = new SettingsModel.Models.MariaDbSettings()
            {
                DbName = FileSettingsModel.DbName,
                Host = FileSettingsModel.DbHost,
                Port = FileSettingsModel.DbPort,
                User = FileSettingsModel.DbUser,
                UserPw = FileSettingsModel.DbPw,
                SleepTimeWhenQueryFalue = 5000
            };




            Debug.Info("global.cfg settings imported");

            return true;
        }

    }
    public class SettingsHandlerReturnModel
    {
        public Boolean ErrorInConfig { get; set; }
        public string ErrorCode { get; set; }

    }
}
