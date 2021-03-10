using System;
using System.Collections.Generic;
using System.Text;

namespace DapperLibrary.DataAccess
{
    public interface IDataAccess
    {
        int InsertItem(string tableName, object anon);
        List<T> Select<T>(string tableName, string[] columnNames = null);
        void Update(string tableName, object updateAnon, object whereAnon);
        void Delete(string tableName, object anon);
        List<T> SelectWhere<T>(string tableName, object anon, string[] columnNames = null);
        List<T> SelectWhereLike<T>(string tableName, object anon, string[] columnNames = null);
        List<T> SelectByList<T>(string tableName, object anon, string[] columnNames = null);
    }
}
