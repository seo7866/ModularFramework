using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ModularFramework.Data.Query.Interfaces
{
    public interface IQueryProvider
    {
        string GetQuery(MethodBase method);
    }
}