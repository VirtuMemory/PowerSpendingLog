using Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var dataBaseType = ConfigurationManager.AppSettings["DatabaseType"];

            if (!Enum.TryParse(dataBaseType, out DBType type))
                type = DBType.INMEMORY;
            Console.WriteLine($"{type} is being used.");

            ChannelFactory <ILoadService > factory = new ChannelFactory<ILoadService>("LoadService");
            ILoadService proxy = factory.CreateChannel();
            Console.WriteLine("Unesite putanju direktorijuma iz kojeg zelite da uciatte .csv fajlove");
            var path = Console.ReadLine();
 
            WorkLoadSender sender = new WorkLoadSender(type,path,proxy);
            sender.SendFiles();

            Console.ReadLine();
        }
    }
}
