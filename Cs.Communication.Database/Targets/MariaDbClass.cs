using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading;


/*
 * TODO
 * 
 * https://dev.mysql.com/doc/connector-net/en/connector-net-connection-options.html
 * 
 * */

namespace Cs.Communication.Database.Targets
{
    public class MariaDbClass
    {
        private MySql.Data.MySqlClient.MySqlConnection _conn;

        #region Settings for class
        public string QuerySql { get; set; }

        public int QueryRunTry { get; set; }
        /// <summary>
        /// DB response timeout. default value 60 sek
        /// </summary>
        public int DbServerTimeOut { get; set; }
        public DataTable ReturnDt { get; set; }
        public string ReturnString { get; set; }
        public long ReturnInt64 { get; set; }
        public ulong ReturnUInt64 { get; set; }
        public bool QueryWasDone { get; set; }

        //  Query Debug information
        public List<string> ExtraStringList { get; set; }
        public int MySqlExceptionId { get; set; }
        public string MySqlExceptionIdText { get; set; }
        public string MySqlExceptionMessage { get; set; }

        #endregion

        public MariaDbClass()
        {

            this.QueryRunTry = 10;
            this.DbServerTimeOut = 60;
            this.QuerySql = null;
            this.ExtraStringList = new List<string>();
            this.QueryWasDone = false;
        }

        public void ClearReturnData()
        {
            this.ReturnDt = null;
            this.ReturnInt64 = 0;
            this.ReturnUInt64 = 0;
            this.ReturnString = null;
            this.QuerySql = "";
            this.QueryWasDone = false;
            this.ExtraStringList = new List<string>();
            this.MySqlExceptionId = -1;
            this.MySqlExceptionIdText = "";
            this.MySqlExceptionMessage = "";

        }

        private void Open()
        {
            
            var connStr = string.Format("sslmode=none;server={0};user={1};database={2};port={3};password={4};", Settings.Database.Host, Settings.Database.User, Settings.Database.DbName, Settings.Database.Port, Settings.Database.UserPw);
            this._conn = new MySql.Data.MySqlClient.MySqlConnection(connStr);
            this._conn.Open();

        }
        private void Close()
        {
            this._conn.Close();
        }

        #region Error handling
        public void GenerateErrorMessage()
        {
            // http://dev.mysql.com/doc/connector-net/en/connector-net-programming-connecting-errors.html

            switch (MySqlExceptionId)
            {
                case 0:
                    this.MySqlExceptionIdText = "Cannot connect to server.  Contact administrator";
                    break;
                case 1045:
                    this.MySqlExceptionIdText = "Invalid username/password, please try again";
                    break;
                default:
                    this.MySqlExceptionIdText = "id: " + MySqlExceptionId.ToString();
                    break;
            }

        }
        #endregion
        #region Query Execute - Singel Query

        public void ExecuteQuerySelect()
        {
            var count = 0;

            while (true)
            {
                count = count + 1;
                try
                {
                    this.Open();
                    var cmd = this._conn.CreateCommand();
                    cmd.CommandText = this.QuerySql;

                    cmd.CommandTimeout = this.DbServerTimeOut;

                    MySql.Data.MySqlClient.MySqlDataReader msdr = cmd.ExecuteReader();
                    DataTable dt = new DataTable();
                    for (int i = 0; i < msdr.VisibleFieldCount; i++)
                        
                        dt.Columns.Add(msdr.GetName(i), type: msdr.GetFieldType(i));
                    while (msdr.Read())
                    {
                        object[] cols = new object[msdr.VisibleFieldCount];
                        msdr.GetValues(cols);
                        dt.Rows.Add(cols);
                    }
                    msdr.Close();
                    this.ReturnDt = dt;
                    this.Close();
                    this.QueryWasDone = true;
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    this.ExtraStringList.Add("<br>Catch (MySql.Data.MySqlClient.MySqlException ex)");
                    this.QueryWasDone = false;
                    this.MySqlExceptionId = ex.Number;
                    this.MySqlExceptionMessage = ex.Message;
                    this.GenerateErrorMessage();
                    if ((ex.Number == 0) && ex.Message.StartsWith("Timeout expired."))
                        break;

                }

                catch (Exception e)
                {
                    this.QueryWasDone = false;
                    this.ExtraStringList.Add("<br>Catch (Exeption e)");
                    this.ExtraStringList.Add("<br>Execption e");
                    this.ExtraStringList.Add("<br>e.Message: <br>" + e.Message);
                    this.ExtraStringList.Add("<br>e.InneException: <br>" + e.InnerException);
                    this.ExtraStringList.Add("<br>e.Data: <br>" + e.Data);
                    this.ExtraStringList.Add("<br>e.Source: <br>" + e.Source);
                }

                if (this.QueryWasDone) break;

                if (count >= this.QueryRunTry)
                {

                    break;
                }

                Thread.Sleep(Settings.Database.SleepTimeWhenQueryFalue);

            }


        }

        public void ExecuteQueryUpdate()
        {
            var count = 0;

            while (true)
            {
                count = count + 1;
                try
                {
                    this.Open();
                    var cmd = this._conn.CreateCommand();
                    cmd.CommandText = this.QuerySql;
                    cmd.CommandTimeout = this.DbServerTimeOut;
                    cmd.ExecuteNonQuery();
                    this.Close();
                    this.QueryWasDone = true;
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    this.ExtraStringList.Add("<br>Catch (MySql.Data.MySqlClient.MySqlException ex)");
                    this.QueryWasDone = false;
                    this.MySqlExceptionId = ex.Number;
                    this.MySqlExceptionMessage = ex.Message;
                    this.GenerateErrorMessage();

                }

                catch (Exception e)
                {
                    this.QueryWasDone = false;
                    this.ExtraStringList.Add("<br>Catch (Exeption e)");
                    this.ExtraStringList.Add("<br>Execption e");
                    this.ExtraStringList.Add("<br>e.Message: <br>" + e.Message);
                    this.ExtraStringList.Add("<br>e.InneException: <br>" + e.InnerException);
                    this.ExtraStringList.Add("<br>e.Data: <br>" + e.Data);
                    this.ExtraStringList.Add("<br>e.Source: <br>" + e.Source);
                }

                if (this.QueryWasDone) break;

                if (count >= this.QueryRunTry)
                {

                    break;
                }

                Thread.Sleep(Settings.Database.SleepTimeWhenQueryFalue);
            }
        }

        public void ExecuteQueryInsertReturnRowIdInt64(bool ReturnRowIdInt64 = true, bool ReturnRowIdUInt64 = false)
        {
            var count = 0;

            while (true)
            {
                count = count + 1;
                try
                {
                    this.Open();
                    var cmd = this._conn.CreateCommand();
                    cmd.CommandText = this.QuerySql;
                    cmd.CommandTimeout = this.DbServerTimeOut;
                    cmd.ExecuteNonQuery();

                    if (ReturnRowIdInt64)
                        this.ReturnInt64 = cmd.LastInsertedId;
                    if (ReturnRowIdUInt64)
                        this.ReturnUInt64 = Convert.ToUInt64(cmd.LastInsertedId);

                    this.Close();
                    this.QueryWasDone = true;
                }
                catch (MySql.Data.MySqlClient.MySqlException ex)
                {
                    this.ExtraStringList.Add("<br>Catch (MySql.Data.MySqlClient.MySqlException ex)");
                    this.QueryWasDone = false;
                    this.MySqlExceptionId = ex.Number;
                    this.MySqlExceptionMessage = ex.Message;
                    this.GenerateErrorMessage();

                }

                catch (Exception e)
                {
                    this.QueryWasDone = false;
                    this.ExtraStringList.Add("<br>Catch (Exeption e)");
                    this.ExtraStringList.Add("<br>Execption e");
                    this.ExtraStringList.Add("<br>e.Message: <br>" + e.Message);
                    this.ExtraStringList.Add("<br>e.InneException: <br>" + e.InnerException);
                    this.ExtraStringList.Add("<br>e.Data: <br>" + e.Data);
                    this.ExtraStringList.Add("<br>e.Source: <br>" + e.Source);
                }

                if (this.QueryWasDone) break;

                if (count >= this.QueryRunTry)
                {

                    break;
                }

                Thread.Sleep(Settings.Database.SleepTimeWhenQueryFalue);
            }


        }
        #endregion


    }
}
