using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BrokerageGather
{
    public class AKProvider
    {
        static AKProvider()
        {
            var list = ConfigurationManager.AppSettings["BaiduMapAK"].Split(',').ToList();
            for (int i = 0; i < list.Count; i++)
            {
                aks.Add(list[i], DateTime.Now);
            }
        }

        private static Dictionary<string, DateTime> aks = new Dictionary<string, DateTime>();

        public static Dictionary<string, DateTime> InvalidAks = new Dictionary<string, DateTime>();

        private static object o = new object();

        public static string Peek()
        {
            lock (o)
            {
                return aks.First().Key;
            }
        }

        public static void Pop(string ak)
        {
            lock (o)
            {
                if (aks.Count > 0)
                {
                    if (aks.ContainsKey(ak))
                    {
                        aks.Remove(ak);
                        InvalidAks.Add(ak, DateTime.Now);
                    }
                }
                else
                {
                    var list = InvalidAks.Where(i => i.Value.Date < DateTime.Now.Date).ToList();
                    for (int i = 0; i < list.Count; i++)
                    {
                        InvalidAks.Remove(list[i].Key);
                        aks.Add(list[i].Key, DateTime.Now);
                    }
                    if (list.Count == 0)
                    {
                        Task.Delay(DateTime.Today.AddDays(1) - DateTime.Now);
                    }
                }
            }
        }
    }
}
