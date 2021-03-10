using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DapperLibrary.DataAccess
{
    public class SqLiteDataAccess : IDataAccess
    {
        public int InsertItem(string tableName, object anon)
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                string columns = string.Join(", ", anon.GetType().GetProperties
                    (BindingFlags.Instance | BindingFlags.Public)
                    .Select(p =>  p.Name.ToString()));
                string fields = string.Join(", ", anon.GetType().GetProperties
                    (BindingFlags.Instance | BindingFlags.Public)
                    .Select(p => $"@{p.Name}"));

                var sql = $"INSERT INTO {tableName} ({columns}) Values ({fields}); ; SELECT Last_Insert_Rowid() ;";

                return conn.ExecuteScalar<int>(sql, anon);
            }
        }

        public List<T> Select<T>(string tableName, string[] columnNames = null)
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                string columns = columnNames == null ?  "*" : string.Join(", ", columnNames);

                var sql = $"SELECT {columns} FROM {tableName};";

                return conn.Query<T>(sql).ToList();
            }
        }

        public void Update(string tableName, object updateAnon, object whereAnon)
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                var merged = new System.Dynamic.ExpandoObject() as IDictionary<string, object>;

                string update = string.Join(" , ", updateAnon.GetType().GetProperties
                    (BindingFlags.Instance | BindingFlags.Public)
                    .Select(p => { merged[p.Name] = p.GetValue(updateAnon); return $"{p.Name} = @{p.Name}"; }));

                string where = string.Join(" AND ", whereAnon.GetType().GetProperties
                    (BindingFlags.Instance | BindingFlags.Public)
                    .Select(p => { merged[p.Name] = p.GetValue(whereAnon); return $"{p.Name} = @{p.Name}"; }));


                var sql = $"UPDATE {tableName} SET {update} WHERE {where};";

                conn.Execute(sql, merged);
            }
        }

        public void Delete(string tableName, object anon)
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                string where = string.Join(" AND ", anon.GetType().GetProperties
                    (BindingFlags.Instance | BindingFlags.Public)
                    .Select(p => $"{p.Name} = @{p.Name}"));

                var sql = $"DELETE FROM {tableName} WHERE {where};";
                conn.Execute(sql, anon);
            }
        }

        public List<T> SelectWhere<T>(string tableName, object anon, string[] columnNames = null)
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                string columns = columnNames == null ? "*" : string.Join(", ", columnNames);

                string where = string.Join(" AND ", anon.GetType().GetProperties
                    (BindingFlags.Instance | BindingFlags.Public)
                    .Select(p => $"{p.Name} = @{p.Name}"));

                var sql = $"SELECT {columns} FROM {tableName} WHERE {where};";

                return conn.Query<T>(sql, anon).ToList();
            }
        }

        public List<T> SelectByList<T>(string tableName, object anon, string[] columnNames = null)
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                string columns = columnNames == null ? "*" : string.Join(", ", columnNames);

                string where = string.Join(" AND ", anon.GetType().GetProperties
                    (BindingFlags.Instance | BindingFlags.Public)
                    .Select(p => $"{p.Name} in @{p.Name}"));

                var sql = $"SELECT {columns} FROM {tableName} WHERE {where};";


                return conn.Query<T>(sql, anon).ToList();
            }
        }

        public List<T> SelectWhereLike<T>(string tableName, object anon, string[] columnNames = null)
        {
            using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
            {
                string columns = columnNames == null ? "*" : string.Join(", ", columnNames);

                string where = string.Join(" AND ", anon.GetType().GetProperties
                    (BindingFlags.Instance | BindingFlags.Public)
                    .Select(p => $"{p.Name} like @{p.Name}"));


                var sql = $"SELECT {columns} FROM {tableName} WHERE {where};";
                return conn.Query<T>(sql, anon).ToList();
            }
        }

        private static string LoadConnectionString(string id = "SQLite")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }



        //public static void MapMultipleObjects()
        //{
        //    using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
        //    {
        //        string sql = @"select pe.*, ph.*
        //                        from dbo.Person pe
        //                        left join dbo.phone ph
        //                        on pe.CellPhoneId = ph.Id";
        //        var people = conn.Query<FullPersonModel, PhoneModel, FullPersonModel>(sql,
        //            (person, phone) => { person.CellPhone = phone; return person; });

        //        foreach(var p in people)
        //        {

        //        }
        //    }
        //}

        //public static void MapMultipleObjectsWithParameters(string lastName)
        //{
        //    using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
        //    {
        //        var p = new
        //        {
        //            LastName = lastName
        //        };

        //        string sql = @"select pe.*, ph.*
        //                        from dbo.Person pe
        //                        left join dbo.phone ph
        //                        on pe.CellPhoneId = ph.Id
        //                        where pe.LastName = @LastName;";
        //        var people = conn.Query<FullPersonModel, PhoneModel, FullPersonModel>(sql,
        //            (person, phone) => { person.CellPhone = phone; return person; }, p);

        //        foreach (var p in people)
        //        {

        //        }
        //    }
        //}

        //public static void MultipleSets()
        //{
        //    using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
        //    {

        //        string sql = @"select * from dbo.Person;
        //                       select * from dbo.Phone";
        //        List<PersonModel> people = null;
        //        List<PhoneModel> phones = null;

        //        using(var lists = conn.QueryMultiple(Sql))
        //        {
        //            people = lists.Read<PersonModel>().ToList(); // same order as the sql query
        //            phones = lists.Read<PhoneModel>().ToList();
        //        }

        //        foreach (var p in people)
        //        {

        //        }
        //    }
        //}

        //public static void MultipleSetsWithParameters(string lastName, string partialPhoneNumber)
        //{
        //    using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
        //    {

        //        string sql = @"select * from dbo.Person where LastName = @LastName;
        //                       select * from dbo.Phone where PhoneNumber like '%' + @PartialPhoneNumber + '%';";
        //        List<PersonModel> people = null;
        //        List<PhoneModel> phones = null;

        //        var p = new
        //        {
        //            LastName = lastName,
        //            PartialPhoneNumber = partialPhoneNumber
        //        };

        //        using (var lists = conn.QueryMultiple(sql, p))
        //        {
        //            people = lists.Read<PersonModel>().ToList(); // same order as the sql query
        //            phones = lists.Read<PhoneModel>().ToList();
        //        }

        //        foreach (var p2 in people)
        //        {

        //        }
        //    }
        //}

        //public static void OutputParameters(string firstName, string lastName)
        //{
        //    using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
        //    {
        //        var p = new DynamicParameters();
        //        p.Add("@Id", 0, DbType.Int32, ParameterDirection.Output);
        //        p.Add("@FirstName", firstName);
        //        p.Add("@LastName", lastName);

        //        string sql = $@"insert into dbo.Person (FirstName, LastName)
        //                        values (@FirstName, @LastName)
        //                        select @Id = @@IDENTITY";

        //        conn.Execute(sql, p);

        //        int NewIdentity = p.Get<int>("@Id");

        //    }
        //}

        //public static void RunWithTransactions(string firstName, string lastName)
        //{
        //    using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
        //    {
        //        var p = new DynamicParameters();
        //        p.Add("@FirstName", firstName);
        //        p.Add("@LastName", lastName);

        //        string sql = $@"insert into dbo.Person (FirstName, LastName)
        //                        values (@FirstName, @LastName);";

        //        conn.Open();
        //        using (var trans = conn.BeginTransaction())
        //        {
        //            int recordsUpdated = conn.Execute(sql, p, trans);

        //            try
        //            {
        //                conn.Execute("update dbo.Person set Id = 1", transaction: trans);
        //                trans.Commit();
        //            }
        //            catch(Exception ex)
        //            {
        //                Console.WriteLine($"Error: { ex.Message }");
        //                trans.Rollback();
        //            }
        //        }

        //        conn.Execute(sql, p);

        //        int NewIdentity = p.Get<int>("@Id");

        //    }
        //}

        //public static void InsertDataSet()
        //{
        //    using (IDbConnection conn = new SQLiteConnection(LoadConnectionString()))
        //    {
        //        var troopers = GetTroopers();
        //        var p = new
        //        {
        //            people = troopers.AsTableValuedParameter("BasicUDT")
        //        };
        //        int recordsAffected = conn.Execute("dbo.spPerson_InsertSet", p, commandType: CommandType.StoredProcedure);

        //        Console.WriteLine($"Records affected: {recordsAffected}");
        //    }
        //}

        //private static DataTable GetTroopers()
        //{
        //    var output = new DataTable();
        //    output.Columns.Add("FirstName", typeof(string));
        //    output.Columns.Add("LastName", typeof(string));

        //    output.Rows.Add("Trooper", "12344");
        //    output.Rows.Add("Trooper", "12345");
        //    output.Rows.Add("Trooper", "12346");
        //    output.Rows.Add("Trooper", "123447");
        //    output.Rows.Add("Trooper", "123448");
        //    output.Rows.Add("Trooper", "12349");

        //    return output;
        //}



        //var output = conn.Query<FirstRowObject>("SELECT * FROM FirstTable", new DynamicParameters());
        //        return output.AsList();
    }
}
