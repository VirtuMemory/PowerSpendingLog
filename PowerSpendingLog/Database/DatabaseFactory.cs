using Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database
{
    public static class DatabaseFactory
    {
        public static ILoadRepository CreateDatabase(DBType databaseType)
        {        
            switch (databaseType)
            {
                case DBType.INMEMORY:
                    return new InMemoryLoadRepository();
                case DBType.XML:
                    return new XMLLoadRepository();
                default:
                    throw new InvalidOperationException($"Unknown DB type: {databaseType}");
            }
        }
    }
}
