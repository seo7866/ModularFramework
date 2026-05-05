using System;
using System.Collections.Generic;
using System.Text;

namespace ModularFramework.DependencyInjection.Enums
{
    public enum DependencyInjectionLifeTime
    {
        Ignore,
        Singleton,
        Scoped,
        Transient
    }
}
