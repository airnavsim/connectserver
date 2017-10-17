using System;
using System.Collections.Generic;
using System.Text;
using securityDB = Cs.Communication.Database.Tools.SecurityStatic;

namespace Cs.Communication.Database.Tools
{
    public class BuildSqlString
    {
        private string _sqlColum;
        private string _sqlValue;
        private string _sqlUpdateString;

        public string Version { get { return "2015" + "08" + "09" + "2140"; } }
        // INSERT INTO tbl_name (a,b,c) VALUES(1,2,3),(4,5,6),(7,8,9);

        public void ClearAllSqlInformationInString()
        {
            this._sqlColum = "";
            this._sqlValue = "";
            this._sqlUpdateString = "";


        }
        public void Insert(string sqlColumeName, string sqlColumeValue)
        {
            sqlColumeName = sqlColumeName.ToLower();

            if (string.IsNullOrEmpty(this._sqlColum))
                this._sqlColum = sqlColumeName;
            else
                this._sqlColum = this._sqlColum + "," + sqlColumeName;

            if (String.IsNullOrEmpty(this._sqlValue))
                this._sqlValue = securityDB.Db.Fnuttify(sqlColumeValue);
            else
                this._sqlValue = this._sqlValue + "," + securityDB.Db.Fnuttify(sqlColumeValue);

        }

        public string ReturnInsertSqlString(string tablename)
        {
            string sql = string.Format("insert into {0} ({1}) values ({2});", tablename, this._sqlColum, this._sqlValue);
            return sql;
        }

        public void Update(string sqlColumeName, string sqlColumeValue)
        {
            sqlColumeName = sqlColumeName.ToLower();


            if (string.IsNullOrEmpty(this._sqlUpdateString))
                this._sqlUpdateString = string.Format("{0}={1}", sqlColumeName, securityDB.Db.Fnuttify(sqlColumeValue));
            else
                this._sqlUpdateString = this._sqlUpdateString + string.Format(",{0}={1}", sqlColumeName, securityDB.Db.Fnuttify(sqlColumeValue));

        }
        public string ReturnUpdateSqlString(string tablename, string whereSats)
        {
            return string.Format("update {0} SET {1} where {2};", tablename, this._sqlUpdateString, whereSats);
            //x sqlUpdateString = "";
            //x return sql;
        }
    }
}
