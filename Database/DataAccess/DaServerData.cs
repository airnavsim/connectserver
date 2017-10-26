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
        #region TblFlightInfoTemp

        public Boolean TblFlightInfoTemp_InsertNewRow(string flightId,string latitude, string longitude, string speed, string speedgs, string altitude, string heading, string fuelflow, string weighttot)
        {
            this.QueryWasDone = false;

            var InsertData = new Tools.BuildSqlString();
            InsertData.Insert("datelogvalue", DateTime.UtcNow.ToString());
            InsertData.Insert("flightid", flightId);
            InsertData.Insert("latitude", latitude);
            InsertData.Insert("longitude", longitude);
            InsertData.Insert("speed", speed);
            InsertData.Insert("speedgs", speedgs);
            InsertData.Insert("altitude", altitude);
            InsertData.Insert("heading", heading);
            InsertData.Insert("fuelflow", fuelflow);
            InsertData.Insert("weighttot", weighttot);

            this.DbMaria.QuerySql = InsertData.ReturnInsertSqlString("tblflightinfotemp");
            this.DbMaria.ExecuteQueryInsertReturnRowIdInt64(false, false);

            if (this.DbMaria.QueryWasDone)
            {
                this.QueryWasDone = true;
                return true;
            }

            this.QueryWasDone = false;
            this.ErrorEvent();
            return false;

        }
        #endregion

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

        #region TblSensors
        public List<Model.Database.TblSensors> TblSensors_GetAll()
        {
            this.DbMaria.QuerySql = $"select * from tblsensors;";

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

            var hh = new List<Model.Database.TblSensors>();
            hh = this.ConvertData.ConvertDataTable<Model.Database.TblSensors>(this.DbMaria.ReturnDt);
            return hh;
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
