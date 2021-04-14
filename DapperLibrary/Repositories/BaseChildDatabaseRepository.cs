using MVVMLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DapperLibrary.Repositories
{
    /// <summary>
    /// Database Repository For classes that has a sigle type of parent
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class BaseChildDatabaseRepository<T> : BaseDatabaseRepository<T>, IParentedDatabaseRepository<T> where T : BaseChildModel
    {

        public override bool AddItem(T item)
        {
            var result = base.AddItem(item);
            if (result)
                item.Changed(false);
            return result;
        }

        public override bool UpdateItem(T item)
        {
            var result = base.UpdateItem(item);
            if (result)
                item.Changed(false);
            return result;
        }

        public virtual List<T> GetChildren(int parentId, string parentType = null)
        {
            if(parentType == null)
                return GetDataAccess().SelectWhere<T>(GetMainTableName(), new { ParentId = parentId});
            return GetDataAccess().SelectWhere<T>(GetMainTableName(), new { ParentId = parentId , ParentType =  parentType});
        }

        public virtual List<int> GetChildrenIds(int parentId, string parentType = null)
        {
            if (parentType == null)
                return GetDataAccess().SelectWhere<int>(GetMainTableName(), new { ParentId = parentId }, new string[] { "Id"});
            return GetDataAccess().SelectWhere<int>(GetMainTableName(), new { ParentId = parentId, ParentType = parentType }, new string[] { "Id" });
        }
    }
}
