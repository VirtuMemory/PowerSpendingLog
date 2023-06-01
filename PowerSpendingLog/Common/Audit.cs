using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class Audit
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public MessageType MessageType { get; set; }
        public string Message { get; set; }

        public static int _nextId = 0;
        public Audit()
        {
            Id = _nextId;
        }
    }
}
