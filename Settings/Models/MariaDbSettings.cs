using System;
using System.Collections.Generic;
using System.Text;

namespace Cs.SettingsModel.Models
{
    public class MariaDbSettings
    {
        public string Host { get; set; }
        public string Port { get; set; }
        public string User { get; set; }
        public string UserPw { get; set; }
        public string DbName { get; set; }

        public int SleepTimeWhenQueryFalue { get; set; }


    }
}
