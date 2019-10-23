using LinqToDB;
using LinqToDB.Data;
using System;
using System.Threading.Tasks;
using TestLinq2Db;
using System.Linq;

namespace Testlinq2DbCore
{
    class Program
    {
        private static BulkCopyOptions _bulkCopyOptions = new BulkCopyOptions { KeepIdentity = true, BulkCopyType = BulkCopyType.MultipleRows, MaxBatchSize = 10 };
        static void Main(string[] args)
        {
            DataConnection.DefaultSettings = new MySettings();
            DataConnection.TurnTraceSwitchOn();
            DataConnection.WriteTraceLine = (s, s1) => { Console.WriteLine(s); };


            using (var db = new ExDbModel())
                try
                {
                    //db.BulkCopy(_bulkCopyOptions, new billing_TestEntity[] { new billing_TestEntity { Id = Guid.NewGuid(), Name = "T1" } });
                    //var z = new billing_TestEntity { Id = Guid.NewGuid(), Name = "T1" };
                    ////db.Insert(z);
                    ////db.Insert(new billing_TestEntity { Id = Guid.NewGuid(), Name = "T1" });
                    //db.InsertAsync(z).Wait();

                    var test =
                        from dt in db.GetTable<billing_Devtype>()
                        from d in  db.GetTable<billing_Device>().LeftJoin(pr => pr.Devtypeid == dt.Devtypeid)
                        select new { dt, d }
                           ;

                    var x0 = test.ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
        }
    }
}
