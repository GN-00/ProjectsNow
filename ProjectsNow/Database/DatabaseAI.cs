using Dapper;
using System;
using System.Linq;
using System.Reflection;
using System.Data.SqlClient;
using ProjectsNow.Attributes;

namespace ProjectsNow.Database
{
    public static class DatabaseAI
    {
        public static readonly int CompanyCreationYear = 2015;
        public static readonly long CompanyVAT = 300108673800003;
        public static readonly JobOrder store = new JobOrder() { ID = 0, Code = "Factory Store", CustomerName = "Factory Store", ProjectName = "Factory Store" };
        public static readonly string FactoryStoreName =  "Factory Store";

        public static string ConnectionString
        {
            get
            {
                if (App.computerName == "HASAN2-PC")
                    return @"Data Source=hasan2-pc\PN;Initial Catalog=ProjectsNow;Integrated Security=False;User ID=sa;Password=Wing00Gundam;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False";

                else if (App.computerName == "WG-0")
                    return @"Data Source=WG-0\PN;Initial Catalog=ProjectsNow;Integrated Security=False;User ID=sa;Password=Wing00Gundam;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False";

                else
                    return @"Data Source=PCAPSSYSTEM\PROJECTSNOW;Initial Catalog=ProjectsNow;Integrated Security=False;User ID=sa;Password=Wing00Gundam;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False"; ;
            }
        }
        //public static readonly string connectionString = $@"Data Source =.; Initial Catalog = ProjectsNow; Integrated Security = True";
        //Work
        //public static string connectionString = @"Data Source=PCAPSSYSTEM\PROJECTSNOW;Initial Catalog=ProjectsNow;Integrated Security=False;User ID=sa;Password=Wing00Gundam;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False";
        //Home
        //public static string connectionString = @"Data Source=LAPTOP-UFU5NEQP\PN;Initial Catalog=ProjectsNow;Integrated Security=False;User ID=sa;Password=Wing00Gundam;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False";
        //My Work
        //public static string connectionString = @"Data Source=hasan2-pc\PN;Initial Catalog=ProjectsNow;Integrated Security=False;User ID=sa;Password=Wing00Gundam;Connect Timeout=15;Encrypt=False;TrustServerCertificate=False";

        public static string GetTable<T>() where T : new()
        {
            var propertiesToUpdateCount = 0;
            string query = "Select";
            PropertyInfo[] properties = typeof(T).GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                var checkAttribute = (DontRead)typeof(T).GetProperty(properties[i].Name).GetCustomAttribute(typeof(DontRead));
                if (checkAttribute == null)
                    query += $"{(propertiesToUpdateCount++ == 0 ? " " : ", ")}{properties[i].Name}";
            }
            return ($"{query} From {((ReadTable)typeof(T).GetCustomAttribute(typeof(ReadTable))).Name}");
        }

        public static string GetRecords<T>(string condition) where T : new()
        {
            var propertiesToUpdateCount = 0;
            string query = "Select";
            PropertyInfo[] properties = typeof(T).GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                var checkAttribute = (DontRead)typeof(T).GetProperty(properties[i].Name).GetCustomAttribute(typeof(DontRead));
                if (checkAttribute == null)
                    query += $"{(propertiesToUpdateCount++ == 0 ? " " : ", ")}{properties[i].Name}";
            }
            return ($"{query} From {((ReadTable)typeof(T).GetCustomAttribute(typeof(ReadTable))).Name} {condition}");
        }

        public static string GetOneRecord<T>(int ID) where T : new()
        {
            var propertiesToUpdateCount = 0;
            string query = "Select Top(1)";
            string condition = "";
            PropertyInfo[] properties = typeof(T).GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                var checkAttribute = (DontRead)typeof(T).GetProperty(properties[i].Name).GetCustomAttribute(typeof(DontRead));
                if (checkAttribute == null)
                    query += $"{(propertiesToUpdateCount++ == 0 ? " " : ", ")}{properties[i].Name}";

                var checkID = (ID)typeof(T).GetProperty(properties[i].Name).GetCustomAttribute(typeof(ID));
                if (checkID != null)
                    condition = $"Where {properties[i].Name} = {ID}";
            }
            return ($"{query} From {((ReadTable)typeof(T).GetCustomAttribute(typeof(ReadTable))).Name} {condition}");
        }

        public static string UpdateRecord<T>() where T : new()
        {
            var propertiesToUpdateCount = 0;
            string query = $"Update {((WriteTable)typeof(T).GetCustomAttribute(typeof(WriteTable))).Name} set ";
            string condition = "";
            PropertyInfo[] properties = typeof(T).GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                var checkID = (ID)typeof(T).GetProperty(properties[i].Name).GetCustomAttribute(typeof(ID));
                if (checkID != null)
                    condition = $"Where {properties[i].Name} = @{properties[i].Name}";

                var checkAttribute = (DontWrite)typeof(T).GetProperty(properties[i].Name).GetCustomAttribute(typeof(DontWrite));
                if (checkAttribute == null && checkID == null)
                    query += $"{(propertiesToUpdateCount++ == 0 ? " " : ", ")}{properties[i].Name} = @{properties[i].Name}";
            }
            return ($"{query} {condition}");
        }

        public static string UpdateRecord<T>(string condition) where T : new()
        {
            var propertiesToUpdateCount = 0;
            string query = $"Update {((WriteTable)typeof(T).GetCustomAttribute(typeof(WriteTable))).Name} set ";
            PropertyInfo[] properties = typeof(T).GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                var checkID = (ID)typeof(T).GetProperty(properties[i].Name).GetCustomAttribute(typeof(ID));

                var checkAttribute = (DontWrite)typeof(T).GetProperty(properties[i].Name).GetCustomAttribute(typeof(DontWrite));
                if (checkAttribute == null && checkID == null)
                    query += $"{(propertiesToUpdateCount++ == 0 ? " " : ", ")}{properties[i].Name} = @{properties[i].Name}";
            }
            return ($"{query} {condition}");
        }

        public static string UpdateRecords<T>() where T : new()
        {
            var propertiesToUpdateCount = 0;
            string query = $"Update {((WriteTable)typeof(T).GetCustomAttribute(typeof(WriteTable))).Name} set ";
            string condition = "";
            PropertyInfo[] properties = typeof(T).GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                var checkID = (ID)typeof(T).GetProperty(properties[i].Name).GetCustomAttribute(typeof(ID));
                if (checkID != null)
                    condition = $"Where {properties[i].Name} = @{properties[i].Name}";

                var checkAttribute = (DontWrite)typeof(T).GetProperty(properties[i].Name).GetCustomAttribute(typeof(DontWrite));
                if (checkAttribute == null && checkID == null)
                    query += $"{(propertiesToUpdateCount++ == 0 ? " " : ", ")}{properties[i].Name} = @{properties[i].Name}";
            }
            return ($"{query} {condition}");
        }

        public static string InsertRecord<T>() where T : new()
        {
            var propertiesToUpdateCount = 0;
            string query = $"Insert Into {((WriteTable)typeof(T).GetCustomAttribute(typeof(WriteTable))).Name} ";
            string columns = $"";
            string values = $"";
            PropertyInfo[] properties = typeof(T).GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                var checkID = (ID)typeof(T).GetProperty(properties[i].Name).GetCustomAttribute(typeof(ID));
                var checkAttribute = (DontWrite)typeof(T).GetProperty(properties[i].Name).GetCustomAttribute(typeof(DontWrite));
                if (checkAttribute == null && checkID == null)
                {
                    columns += $"{(propertiesToUpdateCount == 0 ? " " : ", ")}{properties[i].Name}";
                    values += $"{(propertiesToUpdateCount++ == 0 ? " " : ", ")}@{properties[i].Name}";
                }
            }
            return ($"{query} ({columns}) Values({values}) Select @@IDENTITY");
        }

        public static string InsertRecordWithID<T>() where T : new()
        {
            var propertiesToUpdateCount = 0;
            string query = $"Insert Into {((WriteTable)typeof(T).GetCustomAttribute(typeof(WriteTable))).Name} ";
            string columns = $"";
            string values = $"";
            PropertyInfo[] properties = typeof(T).GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                var checkAttribute = (DontWrite)typeof(T).GetProperty(properties[i].Name).GetCustomAttribute(typeof(DontWrite));
                if (checkAttribute == null)
                {
                    columns += $"{(propertiesToUpdateCount == 0 ? " " : ", ")}{properties[i].Name}";
                    values += $"{(propertiesToUpdateCount++ == 0 ? " " : ", ")}@{properties[i].Name}";
                }
            }
            return ($"{query} ({columns}) Values ({values}) ");
        }

        public static string DeleteRecord<T>(int ID) where T : new()
        {
            string query = $"Delete From {((WriteTable)typeof(T).GetCustomAttribute(typeof(WriteTable))).Name} ";
            PropertyInfo[] properties = typeof(T).GetProperties();
            var checkID = typeof(T).GetProperties().Where(item => Attribute.IsDefined(item, typeof(ID))).ToList().FirstOrDefault();
            query += $"Where {checkID.Name} = {ID} ";
            return query;
        }


        public static string GetFields<T>() where T : new()
        {
            string query = "Select ";
            string joinQuery = "";
            string tableName = ((ReadTable)typeof(T).GetCustomAttribute(typeof(ReadTable))).Name;
            PropertyInfo[] properties = typeof(T).GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                var joinIDAttribute = (JoinID)typeof(T).GetProperty(properties[i].Name).GetCustomAttribute(typeof(JoinID));

                if (joinIDAttribute != null)
                {
                    joinQuery += $"Left Outer Join {joinIDAttribute.ToTable} On {tableName}.{properties[i].Name} = {joinIDAttribute.ToTable}.{properties[i].Name} ";

                    if (joinIDAttribute.To2ndTable != null)
                        joinQuery += $"Left Outer Join {joinIDAttribute.To2ndTable} On {tableName}.{properties[i].Name} = {joinIDAttribute.To2ndTable}.{properties[i].Name} ";

                    if (joinIDAttribute.To3rdTable != null)
                        joinQuery += $"Left Outer Join {joinIDAttribute.To3rdTable} On {tableName}.{properties[i].Name} = {joinIDAttribute.To3rdTable}.{properties[i].Name} ";

                    if (joinIDAttribute.To4thTable != null)
                        joinQuery += $"Left Outer Join {joinIDAttribute.To4thTable} On {tableName}.{properties[i].Name} = {joinIDAttribute.To4thTable}.{properties[i].Name} ";

                    if (joinIDAttribute.To5thTable != null)
                        joinQuery += $"Left Outer Join {joinIDAttribute.To5thTable} On {tableName}.{properties[i].Name} = {joinIDAttribute.To5thTable}.{properties[i].Name} ";

                    if (joinIDAttribute.To6thTable != null)
                        joinQuery += $"Left Outer Join {joinIDAttribute.To6thTable} On {tableName}.{properties[i].Name} = {joinIDAttribute.To6thTable}.{properties[i].Name} ";

                    if (joinIDAttribute.To7thTable != null)
                        joinQuery += $"Left Outer Join {joinIDAttribute.To7thTable} On {tableName}.{properties[i].Name} = {joinIDAttribute.To7thTable}.{properties[i].Name} ";

                    if (joinIDAttribute.To8thTable != null)
                        joinQuery += $"Left Outer Join {joinIDAttribute.To8thTable} On {tableName}.{properties[i].Name} = {joinIDAttribute.To8thTable}.{properties[i].Name} ";
                }

                var checkAttribute = (DontRead)typeof(T).GetProperty(properties[i].Name).GetCustomAttribute(typeof(DontRead));
                if (checkAttribute == null)
                {
                    var joinAttribute = (Join)typeof(T).GetProperty(properties[i].Name).GetCustomAttribute(typeof(Join));

                    if (joinAttribute == null)
                        query += $"{tableName}.{properties[i].Name}, ";
                    else
                        query += $"{joinAttribute.Name}.{properties[i].Name}, ";
                }
            }
            return ($"{query.Substring(0, query.Length - 2)} From {tableName} {joinQuery} ");
        }


        public static void InsertSelect<T, T1>(this SqlConnection connection) where T : new()
        {
            var propertiesToUpdateCount = 0;
            string query = $"Insert Into {((WriteTable)typeof(T).GetCustomAttribute(typeof(WriteTable))).Name} ";
            string columns = $"";
            string values = $"Select ";
            PropertyInfo[] properties = typeof(T).GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                var checkID = (ID)typeof(T).GetProperty(properties[i].Name).GetCustomAttribute(typeof(ID));
                var checkAttribute = (DontWrite)typeof(T).GetProperty(properties[i].Name).GetCustomAttribute(typeof(DontWrite));
                if (checkAttribute == null && checkID == null)
                {
                    columns += $"{(propertiesToUpdateCount == 0 ? " " : ", ")}{properties[i].Name}";
                    values += $"{(propertiesToUpdateCount++ == 0 ? " " : ", ")}{properties[i].Name}";
                }
            }
            connection.Execute($"{query} ({columns}) {values} From {((ReadTable)typeof(T1).GetCustomAttribute(typeof(ReadTable))).Name}");
        }
        public static void InsertSelect<T, T1>(this SqlConnection connection, string condition) where T : new()
        {
            var propertiesToUpdateCount = 0;
            string query = $"Insert Into {((WriteTable)typeof(T).GetCustomAttribute(typeof(WriteTable))).Name} ";
            string columns = $"";
            string values = $"Select ";
            PropertyInfo[] properties = typeof(T).GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                var checkID = (ID)typeof(T).GetProperty(properties[i].Name).GetCustomAttribute(typeof(ID));
                var checkAttribute = (DontWrite)typeof(T).GetProperty(properties[i].Name).GetCustomAttribute(typeof(DontWrite));
                if (checkAttribute == null && checkID == null)
                {
                    columns += $"{(propertiesToUpdateCount == 0 ? " " : ", ")}{properties[i].Name}";
                    values += $"{(propertiesToUpdateCount++ == 0 ? " " : ", ")}{properties[i].Name}";
                }
            }

            connection.Execute($"{query} ({columns}) {values} From {((ReadTable)typeof(T1).GetCustomAttribute(typeof(ReadTable))).Name} {condition}");
        }


    }
}
