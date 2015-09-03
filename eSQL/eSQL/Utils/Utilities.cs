using eSQL.Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace eSQL.Utils
{
    class Utilities
    {
        /// <summary>
        /// Returns true or false from "1" or "0"
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool GetBoolValueFromString(string value)
        {
            if (value == "1")
                return true;
            else
                if (value == "0")
                    return false;
            return false;
        }

        public static bool CheckForDuplicate(ListBox listBox, object selectedItem)
        {
            bool duplicate = false;
            foreach (var item in listBox.Items)
            {
                if (item == selectedItem)
                    duplicate = true;
            }
            return duplicate;
        }

        /// <summary>
        /// Returns false or true from string "false" or "true"
        /// </summary>
        /// <param name="boolean"> Parameter </param>
        /// <returns></returns>
        public static bool GetBoolFromString(string boolean)
        {
            if (boolean.ToLower() == "true")
            {
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Returns 0 or 1 from string "false" or "true"
        /// </summary>
        /// <param name="boolean"> Parameter </param>
        /// <returns></returns>
        public static string GetIntStringFromBool(bool boolean)
        {
            if (boolean == true)
            {
                return 1.ToString();
            }
            else
                return 0.ToString();
        }

        public static string[] Split(string String)
        {
            string[] strings = String.Split('.');
            return strings;
        }

        public static List<Column> AddPrimaryKeysToTop(List<Column> columns, string tableName)
        {
            List<PrimaryKey> pkeys = DataLayer.Configuration.GetPrimaryKey(tableName);
            if (pkeys.Count != columns.Count)
            {
                foreach (PrimaryKey pkey in pkeys)
                {
                    columns.Remove(columns.FirstOrDefault(x => x.ColumnName == pkey.Key));
                    Column column = new Column();
                    int j = 100;
                    for (int i = 0; i < columns.Count; i++)
                    {
                        if (columns[i].ColumnName != pkey.Key)
                        {
                            j = i - 1;
                        }
                    }
                    if (j != 100)
                        columns.Insert(j, column);
                    else
                    {
                        columns.Insert(0, column);
                    }
                }
            }
            return columns;
        }


        public static MongoServer CreateMongoConnection()
        {
            MongoServerSettings settings = new MongoServerSettings();
            settings.Server = new MongoServerAddress("localhost", 27017);
            return new MongoServer(settings);
        }

        public static string RemoveValueFromString(string text, string key)
        {
            var str = text.Split('#');
            str = str.Where(x => x != key).ToArray();
            return String.Join("#", str);
        }

        public static List<KeyValue> GetColumnIndex(List<string> selectedColumns, string databaseName)
        {
            List<KeyValue> list = new List<KeyValue>();
            foreach (var col in selectedColumns)
            {
                List<Column> columns = DataLayer.Configuration.GetTableColumns(databaseName, col.Split('.')[0]);
                List<PrimaryKey> pkeys = DataLayer.Configuration.GetPrimaryKey(col);
                if (pkeys.Count > 0)
                {
                    PrimaryKey pkey = pkeys.FirstOrDefault();
                    Column pkeyCol = columns.Where(x => x.ColumnName == pkey.Key).FirstOrDefault();
                    columns.Remove(pkeyCol);
                    columns.Insert(0, pkeyCol);
                }
                KeyValue colkeyvalue = new KeyValue();
                colkeyvalue.key = col;
                colkeyvalue.value = columns.IndexOf(columns.Single(x => x.ColumnName == col.Split('.')[1])).ToString();
                list.Add(colkeyvalue);
            }
            return list;

        }


        public static List<KeyValue> GetAllColumnIndex(string databaseName, List<string> tables)
        {
            List<KeyValue> list = new List<KeyValue>();
            int i = 0;
            foreach (string tableName in tables)
            {
                List<Column> columns = DataLayer.Configuration.GetTableColumns(databaseName, tableName);
                List<PrimaryKey> pkeys = DataLayer.Configuration.GetPrimaryKey(tableName);
                if (pkeys.Count > 0)
                {
                    PrimaryKey pkey = pkeys.FirstOrDefault();
                    Column pkeyCol = columns.Where(x => x.ColumnName == pkey.Key).FirstOrDefault();
                    columns.Remove(pkeyCol);
                    columns.Insert(0, pkeyCol);
                }
                foreach (var col in columns)
                {
                    KeyValue colkeyvalue = new KeyValue();
                    colkeyvalue.key = tableName + "." + col.ColumnName;
                    colkeyvalue.value = i.ToString();//columns.IndexOf(columns.Single(x => x.ColumnName == col.ColumnName)).ToString();
                    list.Add(colkeyvalue);
                    i++;
                }
            }
            return list;
        }

        public static Dictionary<KeyValue, string> GetColumnType(List<string> selectedColumns, string databaseName)
        {
            Dictionary<KeyValue, string> list = new Dictionary<KeyValue, string>();
            foreach (var col in selectedColumns)
            {
                List<Column> columns = DataLayer.Configuration.GetTableColumns(databaseName, col.Split('.')[0]);
                List<PrimaryKey> pkeys = DataLayer.Configuration.GetPrimaryKey(col);
                if (pkeys.Count > 0)
                {
                    PrimaryKey pkey = pkeys.FirstOrDefault();
                    Column pkeyCol = columns.Where(x => x.ColumnName == pkey.Key).FirstOrDefault();
                    columns.Remove(pkeyCol);
                    columns.Insert(0, pkeyCol);
                }
                KeyValue colkeyvalue = new KeyValue();
                colkeyvalue.key = col;
                colkeyvalue.value = columns.IndexOf(columns.Single(x => x.ColumnName == col.Split('.')[1])).ToString();
                var type = columns.Where(x => x.ColumnName == col.Split('.')[1]).FirstOrDefault().Type;
                list.Add(colkeyvalue, type);
            }
            return list;
        }

        public static string GetColumnValues(List<KeyValue> columns, string keyvaluestring)
        {
            string[] str = keyvaluestring.Split('#');
            string result = string.Empty;
            foreach (var col in columns)
            {
                short index = 0;
                Int16.TryParse(col.value, out index);
                //if (operators != null && operators.Count(x => x.TabCol == col.key) > 0)
                //{
                //    SelectionOperator so = operators.FirstOrDefault(x => x.TabCol == col.key);
                //    if (!Compare(str[index], so.Operator, so.Value, so.Type))
                //        return string.Empty;
                //    else
                //        result += str[index] + "#";

                //}
                //else
                //{
                result += str[index] + "#";
                //}

            }
            result = result.Remove(result.Length - 1, 1);
            return result;
        }

        public static bool Compare(string value1, string oper, string value2, string type)
        {
            switch (oper)
            {
                case ">":
                    {
                        switch (type)
                        {
                            case "int":
                                {
                                    int val1 = 0;
                                    int.TryParse(value1, out val1);
                                    int val2 = 0;
                                    int.TryParse(value2, out val2);
                                    if (val1 > val2)
                                        return true;
                                    break;
                                }
                            case "varchar":
                                {
                                    if (value1.Length > value2.Length)
                                        return true;
                                    break;
                                }


                        }
                        break;
                    }
                case "<":
                    {
                        switch (type)
                        {
                            case "int":
                                {
                                    int val1 = 0;
                                    int.TryParse(value1, out val1);
                                    int val2 = 0;
                                    int.TryParse(value2, out val2);
                                    if (val1 < val2)
                                        return true;
                                    break;
                                }
                            case "varchar":
                                {
                                    if (value1.Length < value2.Length)
                                        return true;
                                    break;
                                }


                        }
                        break;
                    }
                case "<=":
                    {
                        switch (type)
                        {
                            case "int":
                                {
                                    int val1 = 0;
                                    int.TryParse(value1, out val1);
                                    int val2 = 0;
                                    int.TryParse(value2, out val2);
                                    if (val1 <= val2)
                                        return true;
                                    break;
                                }
                            case "varchar":
                                {
                                    if (value1.Length <= value2.Length)
                                        return true;
                                    break;
                                }


                        }
                        break;
                    }
                case ">=":
                    {
                        switch (type)
                        {
                            case "int":
                                {
                                    int val1 = 0;
                                    int.TryParse(value1, out val1);
                                    int val2 = 0;
                                    int.TryParse(value2, out val2);
                                    if (val1 >= val2)
                                        return true;
                                    break;
                                }
                            case "varchar":
                                {
                                    if (value1.Length >= value2.Length)
                                        return true;
                                    break;
                                }


                        }
                        break;
                    }
                case "==":
                    {
                        switch (type)
                        {
                            case "int":
                                {
                                    int val1 = 0;
                                    int.TryParse(value1, out val1);
                                    int val2 = 0;
                                    int.TryParse(value2, out val2);
                                    if (val1 == val2)
                                        return true;
                                    break;
                                }
                            case "varchar":
                                {
                                    if (value1.Equals(value2))
                                        return true;
                                    break;
                                }


                        }
                        break;
                    }
                case "!=":
                    {
                        switch (type)
                        {
                            case "int":
                                {
                                    int val1 = 0;
                                    int.TryParse(value1, out val1);
                                    int val2 = 0;
                                    int.TryParse(value2, out val2);
                                    if (val1 != val2)
                                        return true;
                                    break;
                                }
                            case "varchar":
                                {
                                    if (value1 != value2)
                                        return true;
                                    break;
                                }


                        }
                        break;
                    }
            }
            return false;
        }
    }
}
