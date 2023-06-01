using System;
using System.IO;
using System.Runtime.Serialization;

namespace Common
{
    [DataContract]
    public class WorkLoad : IDisposable
    {
        [DataMember]
        public MemoryStream MS { get; set; }

        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public DBType DbType { get; set; }

        public WorkLoad(MemoryStream ms, string fileName, DBType dBType)
        {
            this.MS = ms;
            this.FileName = fileName;
            this.DbType = dBType;
        }

        public void Dispose()
        {
            if (MS == null)
                return;
            try
            {
                MS.Dispose();
                MS.Close();
                MS = null;
            }
            catch (System.Exception)
            {
                Console.WriteLine("Unsuccesful disposing!");
            };
        }
    }
}
