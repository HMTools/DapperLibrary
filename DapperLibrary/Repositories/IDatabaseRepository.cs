using MVVMLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DapperLibrary.Repositories
{
    public interface IDatabaseRepository<T> where T : BaseModel
    {
        int AddItem(T item);
        T AddAndGetItem(T item);
        T GetItem(int id);
        void UpdateItem(T item);

        void DeleteItem(T item);
    }
}
