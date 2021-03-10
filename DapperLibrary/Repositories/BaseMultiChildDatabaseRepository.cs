using MVVMLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace DapperLibrary.Repositories
{
    /// <summary>
    /// Data Base Repository For classes that has multiple parents types
    /// </summary>
    public abstract class BaseMultiChildDatabaseRepository<T> : BaseDatabaseRepository<T>, IParentedDatabaseRepository<T> where T : BaseMultiChildModel
    {
        #region Insert
        public override T AddAndGetItem(T item)
        {
            base.AddAndGetItem(item);
            GetDataAccess().InsertItem(GetAttributionTableName(), new { ItemId = item.Id, item.ParentId, item.ParentType });
            return item;
        }

        public override int AddItem(T item)
        {
            base.AddAndGetItem(item);
            GetDataAccess().InsertItem(GetAttributionTableName(), new { ItemId = item.Id, item.ParentId, item.ParentType });
            return item.Id;
        }

        public void AddAttribution(T item)
        {
            GetDataAccess().InsertItem(GetAttributionTableName(), new { ItemId = item.Id, item.ParentId, item.ParentType });
        }
        #endregion
        #region Remove
        public override void DeleteItem(T item)
        {
            base.DeleteItem(item);
            GetDataAccess().Delete(GetAttributionTableName(), new { ItemId = item.Id });
        }
        public void DeleteItemAttribution(T item, bool deleteIfNoAttributions = false)
        {
            GetDataAccess().Delete(GetAttributionTableName(), new { ItemId = item.Id, item.ParentId, item.ParentType});
            
            if(deleteIfNoAttributions && GetDataAccess().SelectWhere<T>(GetAttributionTableName(), new { ItemId = item.Id }).Count == 0) base.DeleteItem(item);
        }
        #endregion
        #region Get
        public virtual List<T> GetParentItems(int parentId, string parentType = null)
        {
            if(parentType != null)
            {
                var ids = GetDataAccess().SelectWhere<int>(GetAttributionTableName(), new { ParentId =  parentId, ParentType =  parentType }, new string[] { "ItemId" });
                return GetDataAccess().SelectByList<T>(GetMainTableName(), new { Id = ids });
            }
            else
            {
                MessageBox.Show("Error : No Parent Type Provided!");
            }
            return null;
        }

        public virtual List<int> GetParentChildsIds(int parentId, string parentType = null)
        {
            if (parentType != null)
            {
                return GetDataAccess().SelectWhere<int>(GetAttributionTableName(), new { ParentId = parentId, ParentType = parentType }, new string[] { "ItemId" });
            }
            else
            {
                MessageBox.Show("Error : No Parent Type Provided!");
            }
            return null;
        }

        public virtual int CountAttributions(T item)
        {
            return GetDataAccess().SelectWhere<int>(GetAttributionTableName(), new { ItemId = item.Id }, new[] { "Id" }).Count;
        }
        #endregion
        #region Abstract
        protected abstract string GetAttributionTableName();
        #endregion
    }
}
