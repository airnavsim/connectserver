using System;
using System.Collections.Generic;
using System.Text;

namespace Cs.Model.Settings
{
    public class GlobalSettingsFileModel
    {
        public string DbHost { get; set; }
        public string DbPort { get; set; }
        public string DbName { get; set; }
        public string DbUser { get; set; }
        public string DbPw { get; set; }
        public GlobalSettingsFileServerInfoModel Srv { get; set; }


    }
   
}
