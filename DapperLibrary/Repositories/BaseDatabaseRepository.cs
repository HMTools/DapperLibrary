using DapperLibrary.DataAccess;
using MVVMLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DapperLibrary.Repositories
{
    public abstract class BaseDatabaseRepository<T> : IDatabaseRepository<T> where T : BaseModel
    {
        #region Insert
        public virtual T AddAndGetItem(T item)
        {
            item.Id = GetDataAccess().InsertItem(GetMainTableName(), GetAnonStructure(item));
            return item;
        }

        public virtual int AddItem(T item)
        {
            item.Id = GetDataAccess().InsertItem(GetMainTableName(), GetAnonStructure(item));
            return item.Id;
        }
        #endregion
        #region Delete
        public virtual void DeleteItem(T item)
        {
            GetDataAccess().Delete(GetMainTableName(), new { item.Id });
        }
        #endregion
        #region Get
        public virtual T GetItem(int id)
        {
            return GetDataAccess().SelectWhere<T>(GetMainTableName(), new { id }).FirstOrDefault();
        }

        public virtual List<T> GetAll()
        {
            return GetDataAccess().Select<T>(GetMainTableName());
        }

        public virtual List<T> GetByIds(List<int> ids)
        {
            return GetDataAccess().SelectByList<T>(GetMainTableName(), new { id = ids });
        }
        #endregion
        #region Update
        public virtual void UpdateItem(T item)
        {
            GetDataAccess().Update(GetMainTableName(), GetAnonStructure(item), new { item.Id });
        }
        #endregion
        #region Abstract
        protected abstract IDataAccess GetDataAccess();
        protected abstract string GetMainTableName();
        protected abstract object GetAnonStructure(T item);
        #endregion
    }
}
