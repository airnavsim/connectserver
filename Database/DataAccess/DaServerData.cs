using System;
using System.Collections.Generic;
using System.Text;

namespace Cs.Communication.Database.DataAccess
{
    public class DaServerData
    {
        private Targets.MariaDbClass DbMaria { get; set; }

        private Tools.ConvertData ConvertData { get; set; }
        private Tools.Security Security { get; set; }
        private Tools.BuildSqlString SqlStringBuild { get; set; }

        public List<String> ErrorData { get; set; }
        public bool QueryWasDone { get; set; }

        public DaServerData()
        {
            this.DbMaria = new Targets.MariaDbClass();
            this.ConvertData = new Tools.ConvertData();
            this.Security = new Tools.Security();
            this.SqlStringBuild = new Tools.BuildSqlString();
            this.QueryWasDone = false;

        }

        #region TblSetting

        public Dictionary<string,string> TblSetting_GetAll()
        {
            this.DbMaria.QuerySql = $"select * from tblsetting;";

            this.DbMaria.ExecuteQuerySelect();

            if (!this.DbMaria.QueryWasDone)
            {
                //  Error when running query
                this.ErrorEvent();
                return null;
            }

            this.QueryWasDone = true;
            if (this.DbMaria.ReturnDt.Rows.Count == 0)
            {
                //  No row exist
                return null;
            }

            var hh = new List<Model.Database.TblSetting>();
            hh = this.ConvertData.ConvertDataTable<Model.Database.TblSetting>(this.DbMaria.ReturnDt);


            var tmpReturn = new Dictionary<string, string>();
            foreach (var aa in hh)
            {
                tmpReturn.Add(aa.Name, aa.Value);
            }
            hh.Clear();


            return tmpReturn;
        }
        #endregion

        private void ErrorEvent()
        {
            this.ErrorEvent("daServerData error");
            //TODO  Add error message from dbmaria class to this log.

        }
        private void ErrorEvent(string message)
        {
            if (this.ErrorData is null)
            {
                this.ErrorData = new List<string>();
            }

            this.ErrorData.Add(message);
        }

    }
}
