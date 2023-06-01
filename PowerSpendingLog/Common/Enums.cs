using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public enum MessageType
    {
        Info,
        Warning,
        Error
    }

    public enum LoadType
    {
        Forecast,
        Measured
    }

    public enum DBType
    {
        XML,
        INMEMORY
    }
    public enum ResultTypes
    {
        Success,
        Failed
    }
}
