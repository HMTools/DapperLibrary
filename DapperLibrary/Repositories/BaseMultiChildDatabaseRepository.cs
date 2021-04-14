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
        

        public override bool AddItem(T item)
        {
            try
            {
                base.AddItem(item);
                item.Changed(false);
                GetDataAccess().InsertItem(GetAttributionTableName(), new { ItemId = item.Id, item.ParentId, item.ParentType });
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        #endregion

        #region Update
        public override bool UpdateItem(T item)
        {
            var result = base.UpdateItem(item);
            if (result)
                item.Changed(false);
            return result;
        }
        #endregion

        #region Remove
        public override bool DeleteItem(T item)
        {
            try 
            {
                base.DeleteItem(item);
                GetDataAccess().Delete(GetAttributionTableName(), new { ItemId = item.Id });
                return true;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
        public void DeleteItemAttribution(T item, bool deleteIfNoAttributions = false)
        {
            GetDataAccess().Delete(GetAttributionTableName(), new { ItemId = item.Id, item.ParentId, item.ParentType});
            
            if(deleteIfNoAttributions && GetDataAccess().SelectWhere<T>(GetAttributionTableName(), new { ItemId = item.Id }).Count == 0) base.DeleteItem(item);
        }
        #endregion
        #region Get
        public virtual List<T> GetChildren(int parentId, string parentType = null)
        {
            if(parentType != null)
            {
                var ids = GetChildrenIds(parentId, parentType);
                return GetDataAccess().SelectByList<T>(GetMainTableName(), new { Id = ids });
            }
            else
            {
                MessageBox.Show("Error : No Parent Type Provided!");
            }
            return null;
        }


        public virtual List<int> GetChildrenIds(int parentId, string parentType = null)
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
