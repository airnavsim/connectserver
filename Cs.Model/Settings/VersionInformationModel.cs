using System;
using System.Collections.Generic;
using System.Text;

namespace Cs.Model.Settings
{
    public class VersionInformationModel
    {
        public int DbVersionRightNow { get; set; }
        public int DbVersionAtleast { get; set; }


        public int AppVersion { get; set; }
        public int AppRelease { get; set; }
        public int AppBuild { get; set; }
        public string AppVersionString { get { return $"{this.AppVersion.ToString()}.{this.AppRelease.ToString()}.{this.AppBuild.ToString()}"; } }

    }
}
