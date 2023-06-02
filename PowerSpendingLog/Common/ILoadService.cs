using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [ServiceContract]
    public interface ILoadService
    {
        [OperationContract]
        [FaultContract(typeof(FormatException))]
        Result ImportWorkLoad(WorkLoad workLoad);
    }
}
