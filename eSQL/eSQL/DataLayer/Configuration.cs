using eSQL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using MongoDB.Bson;
using MongoDB.Driver;
using eSQL.Utils;
using MongoDB.Driver.Builders;

namespace eSQL.DataLayer
{
    public class Configuration
    {
        const string filePath = "Databases.xml";
        public static List<Database> GetAllData()
        {
            List<Database> databases = new List<Database>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);
            XmlNodeList nodelistDB = xmlDoc.DocumentElement.SelectNodes("/Databases/DataBase");
            if (nodelistDB != null)
            {
                foreach (XmlNode nodeDB in nodelistDB)
                {
                    Database database = new Database();
                    XmlNode DatabaseName = nodeDB.Attributes.GetNamedItem("dataBaseName");
                    if (DatabaseName != null)
                        database.DatabaseName = DatabaseName.Value;
                    XmlNodeList nodelistTB = xmlDoc.DocumentElement.SelectNodes("/Databases/DataBase/Tables/Table");
                    List<Table> tablesList = new List<Table>();
                    foreach (XmlNode nodeTB in nodelistTB)
                    {
                        if (nodeTB != null && nodeTB.ParentNode.ParentNode == nodeDB)
                        {
                            Table table = new Table();
                            XmlNode tablename = nodeTB.Attributes.GetNamedItem("tableName");
                            if (tablename != null)
                                table.TableName = tablename.Value;
                            XmlNode fileName = nodeTB.Attributes.GetNamedItem("fileName");
                            if (fileName != null)
                                table.FileName = fileName.Value;
                            XmlNode rowLength = nodeTB.Attributes.GetNamedItem("rowLength");
                            if (rowLength != null)
                                table.RowLength = int.Parse(rowLength.Value);
                            XmlNodeList nodeListCo = xmlDoc.DocumentElement.SelectNodes("/Databases/DataBase/Tables/Table/Structure/Attribute");
                            List<Column> columnsList = new List<Column>();
                            foreach (XmlNode nodeCo in nodeListCo)
                            {
                                if (nodeCo != null && nodeCo.ParentNode.ParentNode == nodeTB)
                                {
                                    Column column = new Column();
                                    XmlNode ColumnName = nodeCo.Attributes.GetNamedItem("attributeName");
                                    if (ColumnName != null)
                                        column.ColumnName = ColumnName.Value;
                                    XmlNode Type = nodeCo.Attributes.GetNamedItem("type");
                                    if (Type != null)
                                        column.Type = Type.Value;
                                    XmlNode Length = nodeCo.Attributes.GetNamedItem("length");
                                    if (Type != null)
                                        column.Length = int.Parse(Length.Value);
                                    XmlNode isnull = nodeCo.Attributes.GetNamedItem("isnull");
                                    if (Type != null)
                                        column.IsNull = Utilities.GetBoolValueFromString(isnull.Value);
                                    columnsList.Add(column);
                                }
                            }
                            table.Columns = columnsList;

                            XmlNodeList nodeListPK = xmlDoc.DocumentElement.SelectNodes("/Databases/DataBase/Tables/Table/primaryKey/pkAttribute");
                            if (nodeListPK != null)
                            {
                                List<PrimaryKey> primaryKeysList = new List<PrimaryKey>();
                                foreach (XmlNode nodePK in nodeListPK)
                                {
                                    if (nodePK != null && nodePK.ParentNode.ParentNode == nodeTB)
                                    {
                                        PrimaryKey primaryKey = new PrimaryKey();
                                        primaryKey.Key = nodePK.InnerText;
                                        primaryKeysList.Add(primaryKey);
                                    }

                                }
                                table.PrimaryKey = primaryKeysList;
                            }
                            XmlNodeList nodeListUK = xmlDoc.DocumentElement.SelectNodes("/Databases/DataBase/Tables/Table/uniqueKeys/UniqueAttribute");
                            if (nodeListUK.Count > 0)
                            {
                                List<UniqueKey> uniqueKeysList = new List<UniqueKey>();
                                foreach (XmlNode nodeUK in nodeListUK)
                                {
                                    if (nodeUK != null && nodeUK.ParentNode.ParentNode == nodeTB)
                                    {
                                        UniqueKey uniqueKey = new UniqueKey();
                                        uniqueKey.Key = nodeUK.InnerText;
                                        uniqueKeysList.Add(uniqueKey);
                                    }
                                }
                                table.UniqueKey = uniqueKeysList;
                            }
                            XmlNodeList nodeListIF = xmlDoc.DocumentElement.SelectNodes("/Databases/DataBase/Tables/Table/IndexFiles/IndexFile");
                            if (nodeListIF.Count > 0)
                            {
                                List<IndexFile> indexfilelist = new List<IndexFile>();
                                foreach (XmlNode nodeIF in nodeListIF)
                                {
                                    if (nodeIF != null && nodeIF.ParentNode.ParentNode == nodeTB)
                                    {
                                        IndexFile indexfile = new IndexFile();
                                        XmlNode IndexName = nodeIF.Attributes.GetNamedItem("indexName");
                                        if (IndexName != null)
                                            indexfile.IndexName = IndexName.Value;
                                        XmlNode KeyLength = nodeIF.Attributes.GetNamedItem("keyLength");
                                        if (KeyLength != null)
                                            indexfile.KeyLength = int.Parse(KeyLength.Value);
                                        XmlNode IsUnique = nodeIF.Attributes.GetNamedItem("isUnique");
                                        if (IsUnique != null)
                                            indexfile.IsUnique = Utilities.GetBoolValueFromString(IsUnique.Value);
                                        XmlNode IndexType = nodeIF.Attributes.GetNamedItem("indexType");
                                        if (IndexType != null)
                                            indexfile.IndexType = IndexType.Value;
                                        XmlNode IndexAttributeNode = nodeIF.FirstChild.FirstChild;
                                        if (IndexAttributeNode.Name == "IAttribute" && IndexAttributeNode.InnerText != "")
                                            indexfile.IndexAttribute = IndexAttributeNode.InnerText;
                                        indexfilelist.Add(indexfile);
                                    }
                                }
                                table.IndexFile = indexfilelist;
                            }

                            XmlNodeList nodeListFK = xmlDoc.DocumentElement.SelectNodes("/Databases/DataBase/Tables/Table/foreignKeys/foreignKey");
                            if (nodeListFK.Count > 0)
                            {
                                List<ForeignKey> FKlist = new List<ForeignKey>();
                                foreach (XmlNode nodeFK in nodeListFK)
                                {
                                    if (nodeFK.ParentNode.ParentNode == nodeTB)
                                    {
                                        ForeignKey fk = new ForeignKey();
                                        fk.Key = nodeFK.FirstChild.InnerText;
                                        XmlNode nodeFKRef = nodeFK.LastChild;
                                        fk.RefTable = nodeFKRef.FirstChild.InnerText;
                                        fk.RefAttribute = nodeFKRef.LastChild.InnerText;
                                        FKlist.Add(fk);
                                    }
                                }
                                table.ForeignKey = FKlist;
                            }
                            tablesList.Add(table);
                        }

                    }
                    database.Tables = tablesList;
                    databases.Add(database);
                }
            }
            return databases;
        }

        public static void CreateDatabase(string dbName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);
            XmlNode dbsNode = xmlDoc.SelectSingleNode("/Databases");
            XmlNode newNode = xmlDoc.CreateNode(XmlNodeType.Element, "DataBase", null);
            XmlAttribute newAttribute = xmlDoc.CreateAttribute("dataBaseName");
            newAttribute.Value = dbName;
            newNode.Attributes.Append(newAttribute);
            dbsNode.AppendChild(newNode);
            xmlDoc.Save(filePath);
            MongoServer server = Utilities.CreateMongoConnection();
            string newname = dbName.Replace(" ", "");
            var database = server.GetDatabase(newname);
            var collection = database.GetCollection(newname);
            KeyValue kv = new KeyValue();
            collection.Save(kv);
            database.DropCollection(newname);
            //var collection = database.GetCollection(dbName);
            //KeyValue kv = new KeyValue { key = "22", value = "sss" };
            //collection.Save(kv);
            MessageBox.Show("Database " + dbName + " successfully created!");

        }
        public static void DropDatabase(string dbName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);
            XmlNodeList dbNodeList = xmlDoc.DocumentElement.SelectNodes("/Databases/DataBase");
            foreach (XmlNode node in dbNodeList)
            {
                if (node.Attributes.GetNamedItem("dataBaseName").Value == dbName)
                {
                    var currentNode = node;
                    var parentNode = node.ParentNode;
                    parentNode.RemoveChild(currentNode);
                    xmlDoc.Save(filePath);
                    MongoServer server = Utilities.CreateMongoConnection();
                    // var database = server.GetDatabase(dbName);
                    server.DropDatabase(dbName);
                    MessageBox.Show("Database " + dbName + " dropped successfully!");
                    break;
                }
            }
        }

        public static void AddTable(List<Database> dbs)
        {
            foreach (Database db in dbs)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(filePath);
                XmlNodeList dbNodeList = xmlDoc.DocumentElement.SelectNodes("/Databases/DataBase");
                foreach (XmlNode nodeDB in dbNodeList)
                {
                    XmlNode nodeDbName = nodeDB.Attributes.GetNamedItem("dataBaseName");
                    if (nodeDbName != null)
                        if (db.DatabaseName != null && db.DatabaseName == nodeDbName.Value)
                        {
                            var tablesNodeList = xmlDoc.DocumentElement.SelectNodes("/Databases/DataBase/Tables");
                            if (nodeDB.HasChildNodes)
                            {
                                foreach (XmlNode nodetables in tablesNodeList)
                                {
                                    if (nodetables.ParentNode == nodeDB)
                                    {
                                        Table t = db.Tables.FirstOrDefault();
                                        XmlNode TableNode = xmlDoc.CreateNode(XmlNodeType.Element, "Table", null);
                                        XmlAttribute tableNameAttr = xmlDoc.CreateAttribute("tableName");
                                        if (t.TableName != null)
                                            tableNameAttr.Value = t.TableName;
                                        XmlAttribute fileNameAttr = xmlDoc.CreateAttribute("fileName");
                                        if (t.TableName != null)
                                            fileNameAttr.Value = t.TableName.Replace(" ", "") + ".bin";
                                        XmlAttribute rowLengthAttr = xmlDoc.CreateAttribute("rowLength");
                                        rowLengthAttr.Value = 0.ToString();
                                        TableNode.Attributes.Append(tableNameAttr);
                                        TableNode.Attributes.Append(fileNameAttr);
                                        TableNode.Attributes.Append(rowLengthAttr);
                                        XmlNode NodeStructure = xmlDoc.CreateNode(XmlNodeType.Element, "Structure", null);
                                        foreach (var column in t.Columns)
                                        {
                                            XmlNode ColumnNode = xmlDoc.CreateNode(XmlNodeType.Element, "Attribute", null);
                                            XmlAttribute attributeName = xmlDoc.CreateAttribute("attributeName");
                                            if (column.ColumnName != null)
                                                attributeName.Value = column.ColumnName;
                                            XmlAttribute type = xmlDoc.CreateAttribute("type");
                                            if (column.Type != null)
                                                type.Value = column.Type;
                                            XmlAttribute length = xmlDoc.CreateAttribute("length");
                                            length.Value = column.Length.ToString();
                                            XmlAttribute isnull = xmlDoc.CreateAttribute("isnull");
                                            if (!column.IsNull)
                                                isnull.Value = Utils.Utilities.GetIntStringFromBool(column.IsNull);
                                            ColumnNode.Attributes.Append(attributeName);
                                            ColumnNode.Attributes.Append(type);
                                            ColumnNode.Attributes.Append(length);
                                            ColumnNode.Attributes.Append(isnull);
                                            NodeStructure.AppendChild(ColumnNode);
                                        }
                                        TableNode.AppendChild(NodeStructure);

                                        XmlNode primaryKey = xmlDoc.CreateNode(XmlNodeType.Element, "primaryKey", null);
                                        foreach (PrimaryKey pk in t.PrimaryKey)
                                        {
                                            XmlNode pkNode = xmlDoc.CreateNode(XmlNodeType.Element, "pkAttribute", null);
                                            pkNode.InnerText = pk.Key;
                                            primaryKey.AppendChild(pkNode);
                                        }
                                        TableNode.AppendChild(primaryKey);
                                        XmlNode uniqueKey = xmlDoc.CreateNode(XmlNodeType.Element, "uniqueKeys", null);
                                        foreach (UniqueKey uk in t.UniqueKey)
                                        {
                                            XmlNode ukNode = xmlDoc.CreateNode(XmlNodeType.Element, "UniqueAttribute", null);
                                            ukNode.InnerText = uk.Key;
                                            uniqueKey.AppendChild(ukNode);
                                        }
                                        TableNode.AppendChild(uniqueKey);
                                        XmlNode foreignKeys = xmlDoc.CreateNode(XmlNodeType.Element, "foreignKeys", null);
                                        foreach (ForeignKey fk in t.ForeignKey)
                                        {
                                            XmlNode fkKeyNode = xmlDoc.CreateNode(XmlNodeType.Element, "foreignKey", null);
                                            XmlNode fkKeyAttr = xmlDoc.CreateNode(XmlNodeType.Element, "fkAttribute", null);
                                            fkKeyAttr.InnerText = fk.Key;
                                            XmlNode Refnode = xmlDoc.CreateNode(XmlNodeType.Element, "references", null);
                                            XmlNode refTable = xmlDoc.CreateNode(XmlNodeType.Element, "refTable", null);
                                            refTable.InnerText = fk.RefTable;
                                            XmlNode refAttribute = xmlDoc.CreateNode(XmlNodeType.Element, "refAttribute", null);
                                            refAttribute.InnerText = fk.RefAttribute;
                                            Refnode.AppendChild(refTable);
                                            Refnode.AppendChild(refAttribute);
                                            fkKeyNode.AppendChild(fkKeyAttr);
                                            fkKeyNode.AppendChild(Refnode);
                                            foreignKeys.AppendChild(fkKeyNode);
                                        }
                                        TableNode.AppendChild(foreignKeys);
                                        nodetables.AppendChild(TableNode);
                                        xmlDoc.Save(filePath);
                                    }
                                }
                            }
                            else
                            {
                                XmlNode NodeTables = xmlDoc.CreateNode(XmlNodeType.Element, "Tables", null);
                                nodeDB.AppendChild(NodeTables);
                                Table t = db.Tables.FirstOrDefault();
                                XmlNode TableNode = xmlDoc.CreateNode(XmlNodeType.Element, "Table", null);
                                XmlAttribute tableNameAttr = xmlDoc.CreateAttribute("tableName");
                                if (t.TableName != null)
                                    tableNameAttr.Value = t.TableName;
                                XmlAttribute fileNameAttr = xmlDoc.CreateAttribute("fileName");
                                if (t.TableName != null)
                                    fileNameAttr.Value = t.TableName.Replace(" ", "") + ".ns";
                                XmlAttribute rowLengthAttr = xmlDoc.CreateAttribute("rowLength");
                                rowLengthAttr.Value = 0.ToString();
                                TableNode.Attributes.Append(tableNameAttr);
                                TableNode.Attributes.Append(fileNameAttr);
                                TableNode.Attributes.Append(rowLengthAttr);
                                XmlNode NodeStructure = xmlDoc.CreateNode(XmlNodeType.Element, "Structure", null);
                                foreach (var column in t.Columns)
                                {
                                    XmlNode ColumnNode = xmlDoc.CreateNode(XmlNodeType.Element, "Attribute", null);
                                    XmlAttribute attributeName = xmlDoc.CreateAttribute("attributeName");
                                    if (column.ColumnName != null)
                                        attributeName.Value = column.ColumnName;
                                    XmlAttribute type = xmlDoc.CreateAttribute("type");
                                    if (column.Type != null)
                                        type.Value = column.Type;
                                    XmlAttribute length = xmlDoc.CreateAttribute("length");
                                    length.Value = column.Length.ToString();
                                    XmlAttribute isnull = xmlDoc.CreateAttribute("isnull");
                                    if (!column.IsNull)
                                        isnull.Value = Utils.Utilities.GetIntStringFromBool(column.IsNull);
                                    ColumnNode.Attributes.Append(attributeName);
                                    ColumnNode.Attributes.Append(type);
                                    ColumnNode.Attributes.Append(length);
                                    ColumnNode.Attributes.Append(isnull);
                                    NodeStructure.AppendChild(ColumnNode);
                                }
                                TableNode.AppendChild(NodeStructure);

                                XmlNode primaryKey = xmlDoc.CreateNode(XmlNodeType.Element, "primaryKey", null);
                                foreach (PrimaryKey pk in t.PrimaryKey)
                                {
                                    XmlNode pkNode = xmlDoc.CreateNode(XmlNodeType.Element, "pkAttribute", null);
                                    pkNode.InnerText = pk.Key;
                                    primaryKey.AppendChild(pkNode);
                                }
                                TableNode.AppendChild(primaryKey);
                                XmlNode uniqueKey = xmlDoc.CreateNode(XmlNodeType.Element, "uniqueKeys", null);
                                foreach (UniqueKey uk in t.UniqueKey)
                                {
                                    XmlNode ukNode = xmlDoc.CreateNode(XmlNodeType.Element, "UniqueAttribute", null);
                                    ukNode.InnerText = uk.Key;
                                    uniqueKey.AppendChild(ukNode);
                                }
                                TableNode.AppendChild(uniqueKey);
                                XmlNode foreignKeys = xmlDoc.CreateNode(XmlNodeType.Element, "foreignKeys", null);
                                foreach (ForeignKey fk in t.ForeignKey)
                                {
                                    XmlNode fkKeyNode = xmlDoc.CreateNode(XmlNodeType.Element, "foreignKey", null);
                                    XmlNode fkKeyAttr = xmlDoc.CreateNode(XmlNodeType.Element, "fkAttribute", null);
                                    fkKeyAttr.InnerText = fk.Key;
                                    XmlNode Refnode = xmlDoc.CreateNode(XmlNodeType.Element, "references", null);
                                    XmlNode refTable = xmlDoc.CreateNode(XmlNodeType.Element, "refTable", null);
                                    refTable.InnerText = fk.RefTable;
                                    XmlNode refAttribute = xmlDoc.CreateNode(XmlNodeType.Element, "refAttribute", null);
                                    refAttribute.InnerText = fk.RefAttribute;
                                    Refnode.AppendChild(refTable);
                                    Refnode.AppendChild(refAttribute);
                                    fkKeyNode.AppendChild(fkKeyAttr);
                                    fkKeyNode.AppendChild(Refnode);
                                    foreignKeys.AppendChild(fkKeyNode);
                                }
                                TableNode.AppendChild(foreignKeys);
                                NodeTables.AppendChild(TableNode);
                                nodeDB.AppendChild(NodeTables);
                                xmlDoc.Save(filePath);
                            }
                        }
                }
                MongoServer server = Utilities.CreateMongoConnection();
                foreach (Table table in db.Tables)
                {
                    string newname = table.TableName.Replace(" ", "");
                    var database = server.GetDatabase(db.DatabaseName);
                    var collection = database.GetCollection(newname);
                    KeyValue kv = new KeyValue();
                    collection.Save(kv);
                    collection.RemoveAll();
                    if (table.UniqueKey != null)
                    {
                        var collectionuk = database.GetCollection(newname + "uk");
                        KeyValue kvuk = new KeyValue();
                        collectionuk.Save(kvuk);
                        collectionuk.RemoveAll();
                    }
                    if (table.ForeignKey != null)
                    {
                        var collectionfk = database.GetCollection(newname + "fk");
                        KeyValue kvfk = new KeyValue();
                        collectionfk.Save(kvfk);
                        collectionfk.RemoveAll();
                    }
                }
            }


        }

        public static List<Database> GetDatabases()
        {
            List<Database> databases = new List<Database>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);
            XmlNodeList nodelistDB = xmlDoc.DocumentElement.SelectNodes("/Databases/DataBase");
            if (nodelistDB != null)
            {
                foreach (XmlNode nodeDB in nodelistDB)
                {
                    Database database = new Database();
                    XmlNode DatabaseName = nodeDB.Attributes.GetNamedItem("dataBaseName");
                    if (DatabaseName != null)
                        database.DatabaseName = DatabaseName.Value;
                    XmlNodeList nodelistTB = xmlDoc.DocumentElement.SelectNodes("/Databases/DataBase/Tables/Table");
                    List<Table> tablesList = new List<Table>();
                    foreach (XmlNode nodeTB in nodelistTB)
                    {
                        if (nodeTB != null && nodeTB.ParentNode.ParentNode == nodeDB)
                        {
                            Table table = new Table();
                            XmlNode tablename = nodeTB.Attributes.GetNamedItem("tableName");
                            if (tablename != null)
                                table.TableName = tablename.Value;
                            tablesList.Add(table);
                        }
                    }
                    database.Tables = tablesList;
                    databases.Add(database);
                }
            }
            return databases;
        }

        public static void DropTable(string database, string tableName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);
            XmlNodeList dbNodeList = xmlDoc.DocumentElement.SelectNodes("/Databases/DataBase/Tables/Table");
            foreach (XmlNode node in dbNodeList)
            {
                XmlNode nodeName = node.Attributes.GetNamedItem("tableName");
                if (nodeName != null)
                    if (nodeName.Value == tableName)
                    {
                        var currentNode = node;
                        var parentNode = node.ParentNode;
                        parentNode.RemoveChild(currentNode);
                        xmlDoc.Save(filePath);
                        MessageBox.Show("Database " + tableName + " dropped successfully!");
                        MongoServer server = Utilities.CreateMongoConnection();
                        var db = server.GetDatabase(database);
                        db.DropCollection(tableName);
                        break;
                    }
            }
        }

        public static List<Column> GetTableColumns(string databaseName, string tableName)
        {
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);
            XmlNodeList attrNodeList = xmlDoc.DocumentElement.SelectNodes("/Databases/DataBase/Tables/Table/Structure/Attribute");
            List<Column> listcolumns = new List<Column>();
            foreach (XmlNode node in attrNodeList)
            {
                XmlNode parentNodeName = node.ParentNode.ParentNode.Attributes.GetNamedItem("tableName");
                if (parentNodeName.Value.ToString() == tableName && node.ParentNode.ParentNode.ParentNode.ParentNode.Attributes.GetNamedItem("dataBaseName").Value.ToString() == databaseName)
                {
                    Column column = new Column();
                    XmlNode columnName = node.Attributes.GetNamedItem("attributeName");
                    if (columnName != null)
                        column.ColumnName = columnName.Value;
                    XmlNode Type = node.Attributes.GetNamedItem("type");
                    if (Type != null)
                    {
                        column.Type = Type.Value;
                    }
                    XmlNode Length = node.Attributes.GetNamedItem("length");
                    if (Length != null)
                    {
                        column.Length = Convert.ToInt32(Length.Value.ToString());
                    }
                    XmlNode IsNull = node.Attributes.GetNamedItem("isnull");
                    if (IsNull != null)
                    {
                        column.IsNull = Utilities.GetBoolValueFromString(IsNull.Value.ToString());
                    }
                    listcolumns.Add(column);
                }
            }
            if (listcolumns.Count > 0)
                return listcolumns;
            else return null;
        }


        public static List<string> GetForeignKeys()
        {
            List<Database> dbs = GetAllData();
            List<string> list = new List<string>();
            foreach (Database db in dbs)
            {
                foreach (Table table in db.Tables)
                {
                    foreach (PrimaryKey pk in table.PrimaryKey)
                    {
                        string primarykey = table.TableName + "." + pk.Key;
                        list.Add(primarykey);
                    }
                }
            }
            return list;
        }

        public static List<PrimaryKey> GetPrimaryKey(string tableName)
        {
            List<PrimaryKey> pkeys = new List<PrimaryKey>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);
            XmlNodeList attrNodeList = xmlDoc.DocumentElement.SelectNodes("/Databases/DataBase/Tables/Table/primaryKey/pkAttribute");
            foreach (XmlNode node in attrNodeList)
            {
                XmlNode parentNodeName = node.ParentNode.ParentNode.Attributes.GetNamedItem("tableName");
                if (parentNodeName.Value.ToString() == tableName)
                {
                    PrimaryKey pkey = new PrimaryKey();
                    pkey.Key = node.InnerText;
                    pkeys.Add(pkey);
                }
            }
            return pkeys;
        }
        public static List<UniqueKey> GetUniqueKeys(string tableName)
        {
            List<UniqueKey> ukeys = new List<UniqueKey>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);
            XmlNodeList attrNodeList = xmlDoc.DocumentElement.SelectNodes("/Databases/DataBase/Tables/Table/uniqueKeys/UniqueAttribute");
            foreach (XmlNode node in attrNodeList)
            {
                XmlNode parentNodeName = node.ParentNode.ParentNode.Attributes.GetNamedItem("tableName");
                if (parentNodeName.Value.ToString() == tableName)
                {
                    UniqueKey ukey = new UniqueKey();
                    ukey.Key = node.InnerText;
                    ukeys.Add(ukey);
                }
            }
            return ukeys;
        }


        public static List<ForeignKey> GetForeignKeys(string tableName)
        {
            List<ForeignKey> fkeys = new List<ForeignKey>();
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filePath);
            XmlNodeList nodeListFK = xmlDoc.DocumentElement.SelectNodes("/Databases/DataBase/Tables/Table/foreignKeys/foreignKey");
            if (nodeListFK.Count > 0)
            {
                foreach (XmlNode nodeFK in nodeListFK)
                {
                    XmlNode parentNodeName = nodeFK.ParentNode.ParentNode.Attributes.GetNamedItem("tableName");
                    if (parentNodeName.Value.ToString() == tableName)
                    {
                        ForeignKey fk = new ForeignKey();
                        fk.Key = nodeFK.FirstChild.InnerText;
                        XmlNode nodeFKRef = nodeFK.LastChild;
                        fk.RefTable = nodeFKRef.FirstChild.InnerText;
                        fk.RefAttribute = nodeFKRef.LastChild.InnerText;
                        fkeys.Add(fk);
                    }
                }
            }
            if (fkeys.Count > 0)
                return fkeys;
            else return null;
        }
        public static List<Table> GetTables(string databaseName)
        {
            List<Table> listtable = new List<Table>();
            List<Database> dbs = GetAllData();
            listtable = dbs.Where(x => x.DatabaseName == databaseName).FirstOrDefault().Tables;
            if (listtable.Count > 0)
            {
                return listtable;
            }
            else return null;
        }

        public static List<string> GetOperators()
        {
            var list = new List<string>();
            list.Add("<");
            list.Add(">");
            list.Add("==");
            list.Add("!=");
            list.Add("<=");
            list.Add(">=");
            return list;
        }
    }
}
