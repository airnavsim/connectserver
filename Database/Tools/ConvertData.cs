using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;


namespace Cs.Communication.Database.Tools
{
    public class ConvertData
    {
        public List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }

        private T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name.ToLower() == column.ColumnName)
                    {
                        // try
                        {
                            // if (dr[column.ColumnName].GetType() == System.Boolean)
                            if (dr[column.ColumnName] != null)
                            {
                                // if (typeof(T) == typeof(bool))
                                if (pro == typeof(bool))
                                {
                                    pro.SetValue(obj, Convert.ToBoolean(dr[column.ColumnName]), null);
                                }
                                else if (pro == typeof(string))
                                {
                                    //if (dr[column.ColumnName] is  DBNull)
                                    //    pro.SetValue(obj,"",null);
                                    //else if (dr[column.ColumnName].ToString() == "")
                                    //    pro.SetValue(obj, "", null);
                                    //else
                                    pro.SetValue(obj, dr[column.ColumnName].ToString(), null);
                                }
                                else if (pro == typeof(DBNull))
                                {
                                    pro.SetValue(obj, "", null);
                                }
                                else
                                {
                                    if (dr[column.ColumnName] is DBNull)
                                        pro.SetValue(obj, "", null);
                                    else
                                        // if (dr[column.ColumnName]. != typeof (DBNull))
                                        pro.SetValue(obj, dr[column.ColumnName], null);
                                }

                            }

                            // string dd = pro.GetType();



                        }
                        //catch (Exception)
                        //{

                        //    pro.SetValue(obj,"", null);
                        //}

                    }

                    else
                        continue;
                }
            }
            return obj;
        }

        public int DbToint(object value)
        {

            if (value == null) return -1;
            else
                return Convert.ToInt16(value);
        }
        public uint DbToUint(object value)
        {
            //TODO z001
            if (value == null) return 0;
            else
                return Convert.ToUInt16(value);
        }
        public ulong DbToUlong(object value)
        {

            if (value == null) return 0;
            else if (value is DBNull) return 0;
            else
                return Convert.ToUInt64(value);
        }
        public string DbToString(object value)
        {
            if (value == null) return "";
            else
                return value.ToString();
        }

        public DateTime DbToDateTime(object value)
        {
            if (value == null) return Convert.ToDateTime("2000-01-01 00:00");
            else
                return Convert.ToDateTime(value);
        }
        public bool DbToBool(object value)
        {
            if (value == null) return false;
            else
                return Convert.ToBoolean(value);
        }
        public string BoolToDb(bool value)
        {
            if (value)
                return "1";

            return "0";
        }
    }
}
