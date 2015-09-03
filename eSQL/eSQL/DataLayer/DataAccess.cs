using eSQL.Model;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver.Linq;
using MongoDB.Driver.Builders;
using eSQL.Utils;
using System.Windows.Forms;

namespace eSQL.DataLayer
{
    public class DataAccess
    {
        public static void InsertInTable(string databaseName, string tableName, KeyValue keyvalue)
        {
            MongoServer server = Utilities.CreateMongoConnection();

            if (server.DatabaseExists(databaseName))
            {
                var database = server.GetDatabase(databaseName);
                if (database.CollectionExists(tableName))
                {
                    var collection = database.GetCollection(tableName);
                    collection.Save(keyvalue);

                }
            }
        }
        public static List<string> GetAllTableKeys(string databaseName, string tableName)
        {
            List<string> list = new List<string>();
            MongoServer server = Utilities.CreateMongoConnection();

            if (server.DatabaseExists(databaseName))
            {
                var database = server.GetDatabase(databaseName);
                if (database.CollectionExists(tableName))
                {
                    var collection = database.GetCollection(tableName);
                    list = (from p in collection.AsQueryable<KeyValue>()
                            select p.key).ToList();
                }
            }
            return list;
        }


        public static List<KeyValue> GetAllTableKeysValues(string databaseName, string tableName)
        {
            List<KeyValue> list = new List<KeyValue>();
            MongoServer server = Utilities.CreateMongoConnection();

            if (server.DatabaseExists(databaseName))
            {
                var database = server.GetDatabase(databaseName);
                if (database.CollectionExists(tableName))
                {
                    var collection = database.GetCollection(tableName);
                    list = (from p in collection.AsQueryable<KeyValue>()
                            select p).ToList();
                }
            }
            return list;
        }

        public static void Delete(string databaseName, string tableName, string key)
        {
            List<KeyValue> list = new List<KeyValue>();
            MongoServer server = Utilities.CreateMongoConnection();

            if (server.DatabaseExists(databaseName))
            {
                var database = server.GetDatabase(databaseName);
                if (database.CollectionExists(tableName))
                {
                    var collection = database.GetCollection(tableName);
                    var query = Query<KeyValue>.EQ(x => x.key, key);
                    collection.Remove(query);
                }
            }
        }



        public static bool CheckRestriction(string databaseName, string tableName, string text)
        {
            List<KeyValue> list = new List<KeyValue>();
            MongoServer server = Utilities.CreateMongoConnection();

            if (server.DatabaseExists(databaseName))
            {
                var database = server.GetDatabase(databaseName);
                if (database.CollectionExists(tableName))
                {
                    var collection = database.GetCollection(tableName);
                    if (collection.FindAs<KeyValue>(Query.EQ("key", text)).Count() == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public static bool CheckRestriction(string databaseName, string tableName, string type, string text)
        {
            List<KeyValue> list = new List<KeyValue>();
            MongoServer server = Utilities.CreateMongoConnection();

            if (server.DatabaseExists(databaseName))
            {
                var database = server.GetDatabase(databaseName);
                if (database.CollectionExists(tableName))
                {
                    var collection = database.GetCollection(tableName + type);
                    if (collection.FindAs<KeyValue>(Query.EQ("key", text)).Count() > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool CheckRestriction(string databaseName, ForeignKey fkey, string text)
        {
            List<KeyValue> list = new List<KeyValue>();
            MongoServer server = Utilities.CreateMongoConnection();
            if (server.DatabaseExists(databaseName))
            {
                var database = server.GetDatabase(databaseName);
                if (database.CollectionExists(fkey.RefTable))
                {
                    var collection = database.GetCollection(fkey.RefTable);
                    if (collection.FindAs<KeyValue>(Query.EQ("key", text)).Count() > 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void UpdateExistingFK(string databaseName, string tableName, KeyValue fkey)
        {
            List<KeyValue> list = new List<KeyValue>();
            MongoServer server = Utilities.CreateMongoConnection();
            if (server.DatabaseExists(databaseName))
            {
                var database = server.GetDatabase(databaseName);
                if (database.CollectionExists(tableName))
                {
                    var collection = database.GetCollection(tableName + "fk");
                    if (collection.FindAs<KeyValue>(Query.EQ("key", fkey.key)).Count() > 0)
                    {
                        var query = Query<KeyValue>.EQ(e => e.key, fkey.key);
                        var kvelem = collection.AsQueryable<KeyValue>().First(x => x.key == fkey.key);
                        kvelem.value += "#" + fkey.value;
                        var update = Update<KeyValue>.Set(x => x.value, kvelem.value);
                        collection.Update(query, update);
                    }
                    else
                    {
                        collection.Save(fkey);
                    }
                }
            }
        }


        public static void DeleteKeys(string databaseName, string tableName, string type, string key)
        {
            List<KeyValue> list = new List<KeyValue>();
            MongoServer server = Utilities.CreateMongoConnection();

            if (server.DatabaseExists(databaseName))
            {
                var database = server.GetDatabase(databaseName);
                tableName += type;
                if (database.CollectionExists(tableName))
                {
                    var collection = database.GetCollection(tableName);
                    KeyValue kvelem = null;
                    IMongoQuery query = Query<KeyValue>.EQ(x => x.value, key);
                    if (type == "uk")
                    {
                        kvelem = collection.AsQueryable<KeyValue>().FirstOrDefault(x => x.value == key);
                    }
                    else
                    {
                        query = Query<KeyValue>.Where(x => x.value.Contains(key));
                        kvelem = collection.AsQueryable<KeyValue>().FirstOrDefault(x => x.value.Contains(key));
                    }
                    if (kvelem != null)
                    {
                        kvelem.value = Utilities.RemoveValueFromString(kvelem.value, key);
                        if (kvelem.value.Length > 0)
                        {
                            var update = Update<KeyValue>.Set(x => x.value, kvelem.value);
                            collection.Update(query, update);
                        }
                        else
                        {
                            collection.Remove(query);
                        }
                    }
                }
            }
        }


        public static Table CheckForReferencesFK(string databaseName, string tableName, string value)
        {
            var alldata = DataLayer.Configuration.GetAllData();
            var columns = DataLayer.Configuration.GetTableColumns(databaseName, tableName);
            Database db = alldata.Where(x => x.DatabaseName == databaseName).First();
            if (db != null)
            {
                var currentTable = db.Tables.Where(x => x.TableName == tableName).First();
                if (currentTable != null)
                {
                    PrimaryKey pkey = currentTable.PrimaryKey.FirstOrDefault();
                    foreach (Table table in db.Tables)
                    {
                        ForeignKey fkey = table.ForeignKey.FirstOrDefault();
                        if (fkey != null)
                        {
                            if (currentTable.TableName == fkey.RefTable && currentTable.Columns.Count(x => x.ColumnName == fkey.RefAttribute) > 0)
                            {
                                MongoServer server = Utilities.CreateMongoConnection();
                                if (server.DatabaseExists(databaseName))
                                {
                                    var database = server.GetDatabase(databaseName);
                                    if (database.CollectionExists(table.TableName + "fk"))
                                    {
                                        var collection = database.GetCollection(table.TableName + "fk");
                                        if (collection.FindAs<KeyValue>(Query.EQ("key", value)).Count() > 0)
                                        {
                                            return currentTable;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }


        public static Table CheckForReferencesFK(string databaseName, string tableName)
        {
            var alldata = DataLayer.Configuration.GetAllData();
            var columns = DataLayer.Configuration.GetTableColumns(databaseName, tableName);
            Database db = alldata.Where(x => x.DatabaseName == databaseName).First();
            if (db != null)
            {
                var currentTable = db.Tables.Where(x => x.TableName == tableName).First();
                if (currentTable != null)
                {
                    PrimaryKey pkey = currentTable.PrimaryKey.FirstOrDefault();
                    foreach (Table table in db.Tables)
                    {
                        ForeignKey fkey = table.ForeignKey.FirstOrDefault();
                        if (fkey != null)
                        {
                            if (currentTable.TableName == fkey.RefTable && currentTable.Columns.Count(x => x.ColumnName == fkey.RefAttribute) > 0)
                            {
                                return currentTable;
                            }
                        }
                    }
                }
            }
            return null;
        }




        public static Table GetReferencedTable(string databaseName, string tableName)
        {
            var alldata = DataLayer.Configuration.GetAllData();
            var columns = DataLayer.Configuration.GetTableColumns(databaseName, tableName);
            Database db = alldata.Where(x => x.DatabaseName == databaseName).First();
            if (db != null)
            {
                var currentTable = db.Tables.Where(x => x.TableName == tableName).First();
                if (currentTable != null)
                {
                    PrimaryKey pkey = currentTable.PrimaryKey.FirstOrDefault();
                    foreach (Table table in db.Tables)
                    {
                        ForeignKey fkey = table.ForeignKey.FirstOrDefault();
                        if (fkey != null)
                        {
                            if (currentTable.TableName == fkey.RefTable && currentTable.Columns.Count(x => x.ColumnName == fkey.RefAttribute) > 0)
                            {
                                return table;
                            }
                        }
                    }
                }
            }
            return null;
        }
        public static Table CheckIfTableisReferenced(string databaseName, string tableName)
        {
            var alldata = DataLayer.Configuration.GetAllData();
            var columns = DataLayer.Configuration.GetTableColumns(databaseName, tableName);
            Database db = alldata.Where(x => x.DatabaseName == databaseName).First();
            if (db != null)
            {
                var currentTable = db.Tables.Where(x => x.TableName == tableName).First();
                if (currentTable != null)
                {
                    ForeignKey fkey = currentTable.ForeignKey.FirstOrDefault();
                    foreach (Table table in db.Tables.Where(x => x.TableName != tableName))
                    {
                        PrimaryKey pkey = table.PrimaryKey.FirstOrDefault();
                        if (pkey != null && fkey != null)
                        {
                            if (table.TableName == fkey.RefTable && table.Columns.Count(x => x.ColumnName == fkey.RefAttribute) > 0)
                            {
                                return table;
                            }
                        }
                    }
                }
            }
            return null;
        }


        public static List<string> GetKeyValuesSelectedColumns(string databaseName, List<KeyValue> selectedColumns, bool singletable, List<SelectionOperator> operators)
        {
            List<KeyValue> list = new List<KeyValue>();
            List<string> result = new List<string>();
            MongoServer server = Utilities.CreateMongoConnection();
            if (selectedColumns.Count > 0)
            {
                if (server.DatabaseExists(databaseName))
                {
                    var database = server.GetDatabase(databaseName);
                    if (singletable)
                    {
                        string tabcol = selectedColumns.FirstOrDefault().key.Split('.')[0];
                        var liste = new List<string>();
                        liste.Add(tabcol);
                        var tablecolumnindex = Utilities.GetAllColumnIndex(databaseName, liste);
                        if (database.CollectionExists(tabcol))
                        {
                            var collection = database.GetCollection(tabcol);
                            var keyvalues = (from p in collection.AsQueryable<KeyValue>()
                                             select p).ToList();


                            foreach (KeyValue kv in keyvalues)
                            {
                                string str = kv.key + "#" + kv.value;
                                result.Add(str);
                            }
                        }
                        var aux = new List<string>();
                        result.ForEach(x => { aux.Add(x); });
                        if (operators != null)
                            foreach (var op in operators)
                            {
                                int index = tablecolumnindex.IndexOf(tablecolumnindex.FirstOrDefault(x => x.key == op.TabCol)); ;
                                foreach (var item in result)
                                {
                                    string[] s = item.Split('#');
                                    if (!Utilities.Compare(s[index], op.Operator, op.Value, op.Type))
                                    {
                                        aux.Remove(item);
                                    }
                                }
                            }
                        result = new List<string>();
                        foreach (var item in aux)
                        {
                            var res = Utilities.GetColumnValues(selectedColumns, item);
                            if (res != string.Empty)
                                result.Add(res);
                        }
                        return result.Distinct().ToList();
                    }
                    else
                    {
                        //This is the part with the Join operation
                        List<string> selectedtables = new List<string>();
                        foreach (KeyValue str in selectedColumns)
                        {
                            selectedtables.Add(str.key.Split('.')[0]);
                        }
                        selectedtables = selectedtables.Distinct().ToList();
                        foreach (var item in selectedtables)
                        {
                            string tableName = item;
                            Table tablefk = CheckIfTableisReferenced(databaseName, item);
                            if (tablefk != null)
                            {
                                if (database.CollectionExists(tablefk.TableName))
                                {
                                    var collection = database.GetCollection(tablefk.TableName);
                                    var query = (from p in collection.AsQueryable<KeyValue>()
                                                 select p).ToList();
                                    if (query.Count > 0)
                                    {

                                        if (database.CollectionExists(item + "fk"))
                                        {
                                            var col2 = database.GetCollection(item + "fk");
                                            var tablevalues = new List<string>();
                                            foreach (var itm in query)
                                            {
                                                var query2 = (from p in col2.AsQueryable<KeyValue>()
                                                              where p.key == itm.key
                                                              select p).FirstOrDefault();
                                                if (query2 != null)
                                                {
                                                    var splitvalue = query2.value.Split('#');
                                                    var collection2 = database.GetCollection(item);
                                                    foreach (var s in splitvalue)
                                                    {
                                                        var query3 = (from p in collection2.AsQueryable<KeyValue>()
                                                                      where p.key == s
                                                                      select p.key + "#" + p.value).FirstOrDefault();

                                                        tablevalues.Add(itm.key + "#" + itm.value + "#" + query3);
                                                    }
                                                }
                                                else
                                                {
                                                    tablevalues.Add(itm.key + "#" + itm.value);
                                                }

                                            }


                                            var tablecolumnsindex = Utilities.GetAllColumnIndex(databaseName, selectedtables);
                                            var aux = new List<string>();
                                            tablevalues.ForEach(x => { aux.Add(x); });
                                            if (operators != null)
                                                foreach (var op in operators)
                                                {
                                                    string val = tablecolumnsindex.Where(x => x.key == op.TabCol).FirstOrDefault().value;
                                                    int index = 0;
                                                    int.TryParse(val, out index);
                                                    foreach (var value in tablevalues)
                                                    {
                                                        string[] s = value.Split('#');
                                                        if (index >= s.Length)
                                                        {
                                                            aux.Remove(value);
                                                        }
                                                        else
                                                            if (!Utilities.Compare(s[index], op.Operator, op.Value, op.Type))
                                                            {
                                                                aux.Remove(value);
                                                            }
                                                    }
                                                }
                                            tablevalues = new List<string>();
                                            List<string> indexes = new List<string>();
                                            foreach (var selectedKeyValue in selectedColumns)
                                            {
                                                var index = tablecolumnsindex.Where(x => x.key == selectedKeyValue.key).FirstOrDefault().value;
                                                indexes.Add(index);
                                            }

                                            foreach (string str in aux)
                                            {
                                                string[] s = str.Split('#');
                                                string values = string.Empty;
                                                for (int i = 0; i < indexes.Count; i++)
                                                {
                                                    int index = 0;
                                                    int.TryParse(indexes[i], out index);
                                                    if (index < s.Count())
                                                        values += s[index] + "#";
                                                    else
                                                        values += "NULL" + "#";
                                                }
                                                values = values.Remove(values.Length - 1, 1);
                                                tablevalues.Add(values);
                                            }
                                            return tablevalues.Distinct().ToList();

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }



        public static List<string> GetKeyValuesIndexJoin(string databaseName, List<KeyValue> selectedColumns, bool singletable, List<SelectionOperator> operators)
        {
            List<KeyValue> list = new List<KeyValue>();
            List<string> result = new List<string>();
            MongoServer server = Utilities.CreateMongoConnection();
            if (selectedColumns.Count > 0)
            {
                if (server.DatabaseExists(databaseName))
                {
                    var database = server.GetDatabase(databaseName);
                    if (singletable)
                    {
                        string tabcol = selectedColumns.FirstOrDefault().key.Split('.')[0];
                        var liste = new List<string>();
                        liste.Add(tabcol);
                        var tablecolumnindex = Utilities.GetAllColumnIndex(databaseName, liste);
                        if (database.CollectionExists(tabcol))
                        {
                            var collection = database.GetCollection(tabcol);
                            var keyvalues = (from p in collection.AsQueryable<KeyValue>()
                                             select p).ToList();


                            foreach (KeyValue kv in keyvalues)
                            {
                                string str = kv.key + "#" + kv.value;
                                result.Add(str);
                            }
                        }
                        var aux = new List<string>();
                        result.ForEach(x => { aux.Add(x); });
                        if (operators != null)
                            foreach (var op in operators)
                            {
                                int index = tablecolumnindex.IndexOf(tablecolumnindex.FirstOrDefault(x => x.key == op.TabCol)); ;
                                foreach (var item in result)
                                {
                                    string[] s = item.Split('#');
                                    if (!Utilities.Compare(s[index], op.Operator, op.Value, op.Type))
                                    {
                                        aux.Remove(item);
                                    }
                                }
                            }
                        result = new List<string>();
                        foreach (var item in aux)
                        {
                            var res = Utilities.GetColumnValues(selectedColumns, item);
                            if (res != string.Empty)
                                result.Add(res);
                        }
                        return result.Distinct().ToList();
                    }
                    else
                    {
                        //This is the part with the Join operation
                        List<string> selectedtables = new List<string>();
                        foreach (KeyValue str in selectedColumns)
                        {
                            selectedtables.Add(str.key.Split('.')[0]);
                        }
                        selectedtables = selectedtables.Distinct().ToList();
                        foreach (var item in selectedtables)
                        {
                            string tableName = item;
                            Table tablefk = DataLayer.DataAccess.GetReferencedTable(databaseName, item);
                            if (tablefk != null)
                            {
                                if (database.CollectionExists(tablefk.TableName + "fk"))
                                {
                                    var collection = database.GetCollection(tablefk.TableName + "fk");
                                    var query = (from p in collection.AsQueryable<KeyValue>()
                                                 select p).ToList();
                                    if (query.Count > 0)
                                    {

                                        if (database.CollectionExists(item))
                                        {
                                            var col2 = database.GetCollection(item);
                                            var tablevalues = new List<string>();
                                            foreach (var itm in query)
                                            {
                                                var query2 = (from p in col2.AsQueryable<KeyValue>()
                                                              where p.key == itm.key
                                                              select p).FirstOrDefault();
                                                StringBuilder value = new StringBuilder();
                                                if (query2 != null)
                                                    value.Append(query2.key + "#" + query2.value);

                                                var splitvalue = itm.value.Split('#');
                                                var collection2 = database.GetCollection(tablefk.TableName);
                                                foreach (var s in splitvalue)
                                                {
                                                    var query3 = (from p in collection2.AsQueryable<KeyValue>()
                                                                  where p.key == s
                                                                  select p.key + "#" + p.value).FirstOrDefault();
                                                    if (query3 != null)
                                                        value.Append("#" + query3);
                                                    tablevalues.Add(value.ToString());
                                                    value.Clear();
                                                    if (query2 != null)
                                                        value.Append(query2.key + "#" + query2.value);
                                                    //tablevalues.Add(itm.key + "#" + itm.value + "#" + query3);
                                                }


                                            }


                                            var tablecolumnsindex = Utilities.GetAllColumnIndex(databaseName, selectedtables);
                                            var aux = new List<string>();
                                            tablevalues.ForEach(x => { aux.Add(x); });
                                            if (operators != null)
                                                foreach (var op in operators)
                                                {
                                                    string val = tablecolumnsindex.Where(x => x.key == op.TabCol).FirstOrDefault().value;
                                                    int index = 0;
                                                    int.TryParse(val, out index);
                                                    foreach (var value in tablevalues)
                                                    {
                                                        string[] s = value.Split('#');
                                                        if (index >= s.Length)
                                                        {
                                                            aux.Remove(value);
                                                        }
                                                        else
                                                            if (!Utilities.Compare(s[index], op.Operator, op.Value, op.Type))
                                                            {
                                                                aux.Remove(value);
                                                            }
                                                    }
                                                }
                                            tablevalues = new List<string>();
                                            List<string> indexes = new List<string>();
                                            foreach (var selectedKeyValue in selectedColumns)
                                            {
                                                var index = tablecolumnsindex.Where(x => x.key == selectedKeyValue.key).FirstOrDefault().value;
                                                indexes.Add(index);
                                            }

                                            foreach (string str in aux)
                                            {
                                                string[] s = str.Split('#');
                                                string values = string.Empty;
                                                for (int i = 0; i < indexes.Count; i++)
                                                {
                                                    int index = 0;
                                                    int.TryParse(indexes[i], out index);
                                                    if (index < s.Count())
                                                        values += s[index] + "#";
                                                    else
                                                        values += "NULL" + "#";
                                                }
                                                values = values.Remove(values.Length - 1, 1);
                                                tablevalues.Add(values);
                                            }
                                            return tablevalues.Distinct().ToList();

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }
    }
}

