using MVVMLibrary.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DapperLibrary.Repositories
{
    public interface IParentedDatabaseRepository<T> : IDatabaseRepository<T> where T : BaseChildModel
    {
        List<T> GetChildren(int parentId, string parentType = null);
    }
}
