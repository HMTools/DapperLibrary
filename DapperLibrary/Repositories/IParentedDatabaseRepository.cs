using MVVMLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DapperLibrary.Repositories
{
    public interface IParentedDatabaseRepository<T> : IDatabaseRepository<T> where T : BaseChildModel
    {
        List<T> GetParentItems(int parentId, string parentType = null);
    }
}
