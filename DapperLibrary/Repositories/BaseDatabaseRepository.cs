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
        public virtual bool AddItem(T item)
        {
            try
            {
                item.Id = GetDataAccess().InsertItem(GetMainTableName(), GetAnonStructure(item));
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            
        }
        #endregion
        #region Delete
        public virtual bool DeleteItem(T item)
        {
            try
            {
                GetDataAccess().Delete(GetMainTableName(), new { item.Id });
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        #endregion
        #region Get
        public virtual bool GetItem(int id, out T result)
        {
            try
            {
                result = GetDataAccess().SelectWhere<T>(GetMainTableName(), new { id }).FirstOrDefault();
                return result != null;
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                result = null;
                return false;
            }
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
        public virtual bool UpdateItem(T item)
        {
            try
            {
                GetDataAccess().Update(GetMainTableName(), GetAnonStructure(item), new { item.Id });
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        #endregion
        #region Abstract
        protected abstract IDataAccess GetDataAccess();
        protected abstract string GetMainTableName();
        protected abstract object GetAnonStructure(T item);
        #endregion
    }
}
