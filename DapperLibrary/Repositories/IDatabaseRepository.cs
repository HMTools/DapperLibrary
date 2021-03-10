using MVVMLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DapperLibrary.Repositories
{
    public interface IDatabaseRepository<T> where T : BaseModel
    {
        bool AddItem(T item);
        bool GetItem(int id, out T result);
        bool UpdateItem(T item);

        bool DeleteItem(T item);
    }
}
