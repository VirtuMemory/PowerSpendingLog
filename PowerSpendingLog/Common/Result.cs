using System.Runtime.Serialization;

namespace Common
{
    [DataContract]
    public class Result
    {
        [DataMember]
        public string ResultMessage { get; set; }

        [DataMember]
        public ResultTypes ResultType { get; set; }

        public Result()
        {
            this.ResultType = ResultTypes.Success;
        }
    }
}
