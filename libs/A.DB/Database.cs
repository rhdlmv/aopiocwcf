namespace A.DB
{
    using System;

    public class Database
    {
        private static string _connectionString;
        private static string _defaultSchema;
        private string db_name;
        private A.DB.DbType db_type;
        private string iv;
        private string key;
        private string location;
        private string password;
        private string path;
        private string username;

        public Database()
        {
            this.path = "database.dat";
            this.key = "33053600";
            this.iv = "00000000";
            this.db_type = A.DB.DbType.ACCESS;
        }

        public Database(string path)
        {
            this.path = path;
            this.key = "33053600";
            this.iv = "00000000";
            this.db_type = A.DB.DbType.ACCESS;
        }

        public Database(string path, string key, string iv)
        {
            this.path = path;
            this.key = key;
            this.iv = iv;
            this.db_type = A.DB.DbType.ACCESS;
        }

        public bool CheckConnection()
        {
            bool flag = true;
            DbOperator @operator = new DbOperator(this.getDatabaseConnectionString(), null);
            try
            {
                @operator.Open();
                @operator.Close();
            }
            catch (Exception)
            {
                flag = false;
            }
            return flag;
        }

        private byte[] convert(string str)
        {
            byte[] buffer = new byte[str.Length];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = Convert.ToByte(str[i]);
            }
            return buffer;
        }

        private string getAccessConnectionString()
        {
            string str3 = ("Provider=Microsoft.Jet.OLEDB.4.0;" + "Password=" + this.password + ";") + "User ID=" + this.username + ";";
            return (str3 + "Data Source=" + this.location + @"\" + this.db_name + ";");
        }

        public string getDatabaseConnectionString()
        {
            return this.getDatabaseConnectionString(this.db_type);
        }

        public string getDatabaseConnectionString(A.DB.DbType type)
        {
            switch (type)
            {
                case A.DB.DbType.MSSQL:
                    return this.getSQLConnectionString();

                case A.DB.DbType.ACCESS:
                    return this.getAccessConnectionString();

                case A.DB.DbType.EXCEL:
                    return this.getExcelConnectionString();

                case A.DB.DbType.ORACLE:
                    return this.getOracleConnectionString();

                case A.DB.DbType.VISTADB:
                    return this.getVistaDbConnectionString();
            }
            return null;
        }

        private string getExcelConnectionString()
        {
            string str3 = ("Provider=Microsoft.Jet.OLEDB.4.0;" + "Password=" + this.password + ";") + "User ID=" + this.username + ";";
            return ((str3 + "Data Source=" + this.location + @"\" + this.db_name + ";") + "Extended Properties=\"Excel 8.0;\"");
        }

        private string getOracleConnectionString()
        {
            string str = "Provider=msdaora;";
            return ((((((str + "Data Source=" + this.db_name + ";") + "User ID=" + this.username + ";") + "Password=" + this.password + ";") + "Persist Security Info=False;") + "Connection LifeTime = 15;" + "Connection Timeout = 15;") + "Decr Pool Size = 1;" + "Pooling = true;");
        }

        private string getSQLConnectionString()
        {
            string str = "Provider=SQLOLEDB.1;";
            return ((((((((str + "Persist Security Info=False;") + "User ID=" + this.username + ";") + "Password=" + this.password + ";") + "Data Source=" + this.location + ";") + "Initial Catalog=" + this.db_name + ";") + "Use Procedure for Prepare=1;" + "Auto Translate=True;") + "Packet Size=4096;" + "Use Encryption for Data=False;") + "Connection Timeout = 60;" + "Tag with column collation when possible=False;");
        }

        private string getVistaDbConnectionString()
        {
            string str = "";
            if (this.password != null)
            {
                str = str + "Password=" + this.password + ";";
            }
            string str3 = str;
            return ((str3 + "Data Source=" + this.location + @"\" + this.db_name + ";") + "Open Mode=ExclusiveReadWrite;");
        }

        public static A.DB.DbType parseDbType(string url)
        {
            string[] strArray = url.Split(new char[] { ';' });
            int length = strArray.Length;
            for (int i = 0; i < length; i++)
            {
                if (strArray[i].StartsWith("Provider"))
                {
                    string[] strArray2 = strArray[i].Split(new char[] { '=' });
                    if ((strArray2.Length == 2) && (strArray2[1] != null))
                    {
                        if (strArray2[1].Trim().StartsWith("SQL"))
                        {
                            return A.DB.DbType.MSSQL;
                        }
                        if (strArray2[1].Trim().StartsWith("Microsoft.Jet.OLEDB"))
                        {
                            return A.DB.DbType.ACCESS;
                        }
                        if (strArray2[1].Trim().StartsWith("msdaora"))
                        {
                            return A.DB.DbType.ORACLE;
                        }
                    }
                }
                else if (strArray[i].ToLower().StartsWith("data source") && strArray[i].ToLower().Contains(".vdb"))
                {
                    return A.DB.DbType.VISTADB;
                }
            }
            return A.DB.DbType.UNKNOWN;
        }

        public void read()
        {
        }

        public void write()
        {
        }

        public static string ConnectionString
        {
            get
            {
                return _connectionString;
            }
            set
            {
                _connectionString = value;
            }
        }

        public string DbName
        {
            get
            {
                return this.db_name;
            }
            set
            {
                this.db_name = value;
            }
        }

        public A.DB.DbType DbType
        {
            get
            {
                return this.db_type;
            }
            set
            {
                this.db_type = value;
            }
        }

        public static string DefaultSchema
        {
            get
            {
                return _defaultSchema;
            }
            set
            {
                _defaultSchema = value;
            }
        }

        public string Location
        {
            get
            {
                return this.location;
            }
            set
            {
                this.location = value;
            }
        }

        public string Password
        {
            get
            {
                return this.password;
            }
            set
            {
                this.password = value;
            }
        }

        [Obsolete("This Property is Obsolete,You Should use ConnectionString property to instead")]
        public static string Url
        {
            get
            {
                return _connectionString;
            }
            set
            {
                _connectionString = value;
            }
        }

        public string Username
        {
            get
            {
                return this.username;
            }
            set
            {
                this.username = value;
            }
        }
    }
}

