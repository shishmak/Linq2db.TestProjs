using DataModels;
using LinqToDB;
using LinqToDB.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;

namespace TestLinq2Db
{
    class Program
    {
        private static BulkCopyOptions _bulkCopyOptions = new BulkCopyOptions { KeepIdentity = true , BulkCopyType = BulkCopyType.MultipleRows, MaxBatchSize = 10};

        public static int Chanel(string DeviceNumber) => int.TryParse((DeviceNumber ?? "0")[0].ToString(), out int v) ? v : 0;
        public static int Chanel2(string DeviceNumber) => (int)(DeviceNumber == null || !char.IsDigit(DeviceNumber[0]) ? 0 : char.GetNumericValue(DeviceNumber[0]));

        public static void Test1() {
            #region Linq2db
            using (var db = new ExDbModel())
            {

                DataConnection.TurnTraceSwitchOn();
                DataConnection.WriteTraceLine = (s, s1) => { Console.WriteLine(s); };

                try
                {
                    db.CreateTable<billing_Devtype>();
                    db.CreateTable<billing_Device>();
                    db.CreateTable<billing_DevReadingType>();
                    db.CreateTable<billing_TempReading>();
                }
                catch { }

                #region Source is table (correct build)
                db.TempReadings
                    .Set(p => p.DevReadingTypeId,
                            u => db.DevReadingTypes.Where(w => w.Name == u.ReadingTypeName && w.DevTypeId == u.Devtypeid).Select(s => s.Id).FirstOrDefault()
                    )
                    .Set(p => p.Responsibility,
                            u => db.DevReadingTypes.Where(w => w.Name == u.ReadingTypeName && w.DevTypeId == u.Devtypeid).Select(s => s.Responsibility).FirstOrDefault()
                    )
                    .Update();

                #region Sql correct resault
                /*
                    UPDATE
                        billing."TempReading"
                    SET
                        "DevReadingTypeId" = (
                                SELECT
                                        w_1."Id"
                                FROM
                                        billing."DevReadingType" w_1
                                WHERE
                                        w_1."Name" = billing."TempReading"."ReadingTypeName" AND
                                        w_1."DevTypeId" = billing."TempReading"."Devtypeid"
                                LIMIT 1
                        ),
                        "Responsibility" = (
                                SELECT
                                        w_2."Responsibility"
                                FROM
                                        billing."DevReadingType" w_2
                                WHERE
                                        w_2."Name" = billing."TempReading"."ReadingTypeName" AND
                                        w_2."DevTypeId" = billing."TempReading"."Devtypeid"
                                LIMIT 1
                        )
                 */

                #endregion
                #endregion

                #region Source is join of two tables (error build)
                //var devs = db.Devices.Join(db.Devtypes, d => d.Devtypeid, dt => dt.Devtypeid, (d, dt) => new { d, dt });
                //
                //db.TempReadings
                //    .Set(t => t.Devid, u => devs.Where(w => w.d.Sernum == u.DevSerNum && w.dt.GlobalType == u.DevGlobalType).Select(s => s.d.Devid).FirstOrDefault())
                //    .Set(t => t.Devtypeid, u => devs.Where(w => w.d.Sernum == u.DevSerNum && w.dt.GlobalType == u.DevGlobalType).Select(s => s.dt.Devtypeid).FirstOrDefault())
                //    .Update();
                //
                #region Sql error resault
                #region build sql
                /*
update
    billing."TempReading"
set
    devid = ( 
    select
        s_1.devid
    from
        (
        select
            t1.devtypeid,
            t1.sernum,
            t1.devid
        from
            billing.devices t1 ) s_1
    inner join (
        select -- error select wrapper 
            dt_1.devtypeid
        from
            (
            select -- in correct variant needs only this one
                t2.devtypeid,
                t2."GlobalType"
            from
                billing.devtypes t2 ) dt_1 ) t3 on
        s_1.devtypeid = t3.devtypeid
    where
        s_1.sernum = billing."TempReading"."DevSerNum"
        and dt_1."GlobalType" = billing."TempReading"."DevGlobalType"
    limit 1 ),
    "Devtypeid" = (
    select
        dt_2.devtypeid
    from
        (
        select
            t4.devtypeid,
            t4.sernum
        from
            billing.devices t4 ) s_2
    inner join (
        select --error select wrapper
            dt_2.devtypeid
        from
            (
            select -- in correct variant needs only this one
                t5.devtypeid,
                t5."GlobalType"
            from
                billing.devtypes t5 ) dt_2 ) t6 on
        s_2.devtypeid = t6.devtypeid
    where
        s_2.sernum = billing."TempReading"."DevSerNum"
        and dt_2."GlobalType" = billing."TempReading"."DevGlobalType"
    limit 1 )   
                */
                #endregion // build sql

                #region exception
                /*
                 Error
                    Exception: Npgsql.PostgresException
                    Message  : 42P01: таблица "dt_1" отсутствует в предложении FROM
                       в Npgsql.NpgsqlConnector.<DoReadMessage>d__148.MoveNext()
                    --- Конец трассировка стека из предыдущего расположения, где возникло исключение ---
                       в System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
                       в System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
                       в System.Runtime.CompilerServices.ValueTaskAwaiter`1.GetResult()
                       в Npgsql.NpgsqlConnector.<ReadMessage>d__147.MoveNext()
                    --- Конец трассировка стека из предыдущего расположения, где возникло исключение ---
                       в Npgsql.NpgsqlConnector.<ReadMessage>d__147.MoveNext()
                    --- Конец трассировка стека из предыдущего расположения, где возникло исключение ---
                       в System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
                       в System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
                       в System.Runtime.CompilerServices.ValueTaskAwaiter`1.GetResult()
                       в Npgsql.NpgsqlConnector.<ReadExpecting>d__154`1.MoveNext()
                    --- Конец трассировка стека из предыдущего расположения, где возникло исключение ---
                       в System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
                       в System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
                       в System.Runtime.CompilerServices.ValueTaskAwaiter`1.GetResult()
                       в Npgsql.NpgsqlDataReader.<NextResult>d__32.MoveNext()
                    --- Конец трассировка стека из предыдущего расположения, где возникло исключение ---
                       в System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
                       в System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
                       в Npgsql.NpgsqlDataReader.NextResult()
                       в Npgsql.NpgsqlCommand.<Execute>d__71.MoveNext()
                    --- Конец трассировка стека из предыдущего расположения, где возникло исключение ---
                       в System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
                       в System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
                       в System.Runtime.CompilerServices.ValueTaskAwaiter`1.GetResult()
                       в Npgsql.NpgsqlCommand.<ExecuteNonQuery>d__84.MoveNext()
                    --- Конец трассировка стека из предыдущего расположения, где возникло исключение ---
                       в System.Runtime.CompilerServices.TaskAwaiter.ThrowForNonSuccess(Task task)
                       в System.Runtime.CompilerServices.TaskAwaiter.HandleNonSuccessAndDebuggerNotification(Task task)
                       в Npgsql.NpgsqlCommand.ExecuteNonQuery()
                       в LinqToDB.Data.DataConnection.ExecuteNonQuery() в C:\projects\linq2db\Source\LinqToDB\Data\DataConnection.cs:строка 1114
                */
                #endregion // exception

                #region correct sql looks like this
                /*
                update
                    billing."TempReading"
                set
                    devid = (
                    select
                        s_1.devid
                    from
                        (
                        select
                            t1.devtypeid,
                            t1.sernum,
                            t1.devid
                        from
                            billing.devices t1 ) s_1
                    inner join (
                            select
                                t2.devtypeid,
                                t2."GlobalType"
                            from
                                billing.devtypes t2 ) dt_1  on
                        s_1.devtypeid = dt_1.devtypeid
                    where
                        s_1.sernum = billing."TempReading"."DevSerNum"
                        and dt_1."GlobalType" = billing."TempReading"."DevGlobalType"
                    limit 1 ),
                    "Devtypeid" = (
                    select
                        dt_2.devtypeid
                    from
                        (
                        select
                            t4.devtypeid,
                            t4.sernum
                        from
                            billing.devices t4 ) s_2
                    inner join (
                            select
                                t5.devtypeid,
                                t5."GlobalType"
                            from
                                billing.devtypes t5 ) dt_2  on
                        s_2.devtypeid = dt_2.devtypeid
                    where
                        s_2.sernum = billing."TempReading"."DevSerNum"
                        and dt_2."GlobalType" = billing."TempReading"."DevGlobalType"
                    limit 1 )
                */
                #endregion // correct sql

                #endregion // Sql error resault
                #endregion // Source is join of two tables
            }
            #endregion
        }

        public static void Test2()
        {
            using (var db = new ExDbModel())
            {
                try
                {
                    //db.CreateTable<billing_Devtype>();
                    //db.CreateTable<billing_Device>();
                    //db.CreateTable<billing_DevReadingType>();
                    //db.CreateTable<billing_TempReading>();
                    //db.CreateTable<billing_TestEntity>();
                    db.CreateTable<billing_TestBaseEntity>();
                    db.CreateTable<billing_TestChEntity>();
                } catch { }

                DataConnection.TurnTraceSwitchOn();
                DataConnection.WriteTraceLine = (s, s1) => { Console.WriteLine(s); };

                var pe = new billing_TestBaseEntity[] {
                    new billing_TestBaseEntity { Id = Guid.NewGuid(), Name = "T1" },
                    new billing_TestBaseEntity { Id = Guid.NewGuid(), Name = "T2" },
                    new billing_TestBaseEntity { Id = Guid.NewGuid(), Name = "T3" },
                };
                db.BulkCopy(_bulkCopyOptions, pe);
                var ce = new billing_TestChEntity[] {
                    new billing_TestChEntity { Id = Guid.NewGuid(), BaseId = pe[0].Id, Name = "ToT1" },
                    new billing_TestChEntity { Id = Guid.NewGuid(), BaseId = pe[1].Id, Name = "ToT2" },
                };
                db.BulkCopy(_bulkCopyOptions,  ce );

                var group = db.TestBaseTable.GroupJoin(db.TestChTable, dt => dt.Id, d => d.BaseId, (dt, d) => new { dt, d });
                var smany1 = group.SelectMany(sm => sm.d.DefaultIfEmpty(), (sm, d) => new { sm.dt, d } );
                //var smany2 = db.Devtypes.GroupJoin(db.Devices, dt => dt.Devtypeid, d => d.Devtypeid, (dt, d) => new { dt, d}).SelectMany(sm => sm.d.DefaultIfEmpty(), (sm, d) => new { sm.dt, d });

                //var g2 =
                //    from b in db.TestBaseTable
                //    from c in db.TestChTable.LeftJoin(l => l.BaseId == b.Id)
                //    select new { b, c }; 
                
                var g2 =
                     from b in db.Gw
                     from c in db.GwSet.LeftJoin(l => l.GatewayId == b.Id)
                     select new { b, c };

                var g3 = db.Gw.SelectMany(b => db.GwSet.Where(l => l.GatewayId == b.Id).DefaultIfEmpty(), (b, c) => new { b, c });

                var q1 = smany1.ToList();
                //var q2 = g2.ToList();
                var q3 = g3.ToList();

                //Console.WriteLine(string.Join("\n", db.Homes.ToList()));
                //
                //var typId1s = db.Devices.Select(s => s.Devtypeid).AsCte("CTE_1");
                //var typId2s = typId1s.Distinct().AsCte("CTE_2");
                //var typId3s = typId2s.Distinct().AsCte("CTE_3");
                //var typId4s = typId3s.Distinct().AsCte("CTE_4");
                //
                //typId1s.Count();

                //var qCte = db.Devtypes.Where(w => w.Devtypeid.NotIn(typId2s)).ToList();

                //var d = new billing_Device
                //{
                //    Devid = Guid.NewGuid(),
                //    Sernum = "Test01",
                //    Devtypeid = 1
                //};
                //
                //db.Devices.Insert(() => new billing_Device
                //{
                //    Devid = Guid.NewGuid(),
                //    Sernum = "Test01",
                //    Devtypeid = 1
                //});

                //db.BeginTransaction();

                //
                //db.Devtypes.Insert(() => new billing_Devtype { Typename = "TestType1", GlobalType = 1 });
                //db.Devtypes.Insert(() => new billing_Devtype { Typename = "TestType2", GlobalType = 1 });
                //db.Devtypes.Insert(() => new billing_Devtype { Typename = "TestType3", GlobalType = 1 });
                //db.Devtypes.Insert(() => new billing_Devtype { Typename = "TestType4", GlobalType = 1 });
                //try
                //{
                //    db.BulkCopy(_bulkCopyOptions, new billing_TestEntity[] { new billing_TestEntity { Id = Guid.NewGuid(), Name = "T1" } });
                //    db.Insert(new billing_TestEntity { Id = Guid.NewGuid(), Name = "T1" });
                //}
                //catch (Exception e)
                //{
                //    Console.WriteLine(e.Message);
                //}

                //
                //var o3 =4;
                //
                //var rnd = new Random();
                //var range = Enumerable.Range(0, 500000).Select(s => new billing_Device
                //{
                //    Devid = Guid.NewGuid(),
                //    Sernum = "Test01",
                //    Devtypeid = rnd.Next(1, o3) * 2 - 1
                //});
                //
                //db.BulkCopy(_bulkCopyOptions, range);
                //
                //var group = db.Devtypes.GroupJoin(db.Devices, dt => dt.Devtypeid, d => d.Devtypeid, (dt, d) => new { dt, d = d.DefaultIfEmpty() });
                //var smany1 = group.SelectMany(sm => sm.d.Select(d => new { sm.dt, d }));
                //var smany2 = db.Devtypes.GroupJoin(db.Devices, dt => dt.Devtypeid, d => d.Devtypeid, (dt, d) => new { dt, d}).SelectMany(sm => sm.d.DefaultIfEmpty(), (sm, d) => new { sm.dt, d });
                //
                //var q1 = smany2.ToList();
                //var q2 = db.Devtypes.GroupJoin(db.Devices, dt => dt.Devtypeid, d => d.Devtypeid, (dt, d) => new { dt, d }).SelectMany(sm => sm.d.DefaultIfEmpty(), (sm, d) => new { sm.dt, d }).Where(w => w.d == null).ToList();


                //var q2d= db.Devtypes
                //    .GroupJoin(db.Devices.Select(s => (int?) s.Devtypeid).Distinct()
                //        , dt => dt.Devtypeid, d => d, (dt, d) => new { dt, d })
                //    .SelectMany(sm => sm.d.DefaultIfEmpty(), (sm, d) => new { sm.dt, d })
                //    .Where(w => w.d == null)
                //    .ToList();
                //
                //var q3 = db.Devtypes.Where(w => !db.Devices.Select(s => s.Devtypeid).Contains(w.Devtypeid)).ToList();
                //var q4 = db.Devtypes.Where(w => !db.Devices.Any(a=>a.Devtypeid == w.Devtypeid)).ToList();
                //var qNotIN = db.Devtypes.Where(w => w.Devtypeid.NotIn(db.Devices.Select(s => s.Devtypeid).Distinct())).ToList();
                //var qIN = db.Devtypes
                //    .Where(w => w.Devtypeid.In(
                //        db.Devices.Select(s => s.Devtypeid).Distinct()
                //    )).ToList();

                //var typIds = db.Devices.Select(s => s.Devtypeid).AsCte("c1");
                //var typId2s = typIds.Distinct().AsCte("c2");
                //
                //var qCte = db.Devtypes.Where(w => w.Devtypeid.NotIn(typId2s)).ToList();
                //var qCte2 = db.Devtypes.Where(w => !cte.Contains(w.Devtypeid)).ToList();


                //var gb = db.Devtypes.GroupBy(g => g.GlobalType).Select(s => new { s.Key, count = s.Count(), max = s.Select(s1 => s1.Typename).Max() }).ToList();

                //db.Devices.Where(w => w.Sernum == "Test01")
                //    .Set(s => s.Devtype, new billing_Devtype { Typename = "TestType3", GlobalType = 1 })
                //    .Update()
                //;
            }
        }

        private static void Test3() 
            =>new[] {"local/zzz", "/zzzz", "local", "local/", "zzz.a", "zzz.a/", "/llll/aaa/zzzz", "c:/llll/zzzz", "/c:/llll/zzzz" }
            .Select(s=> new {s, dir = Path.GetDirectoryName(s), file = Path.GetFileName(s), noe = Path.GetFileNameWithoutExtension(s)})
            .ToList()
            .ForEach(f => Console.WriteLine("{0}\ndir: {1}\nfile: {2}\nnoe: {3}\n", f.s, f.dir, f.file, f.noe ));

        private static void Test4()
            => MethodNotSupport();

        private static void MethodNotSupport(
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            var msg = $"Plugin '{typeof(Program).Name}' not support method '{memberName}' (file: {sourceFilePath}: {sourceLineNumber})";
            Console.WriteLine(msg);
        }

        private static void Test5()
        {
            var ser = JsonConvert.SerializeObject(
                new A(),
                new JsonSerializerSettings() { PreserveReferencesHandling = PreserveReferencesHandling.None, ReferenceLoopHandling = ReferenceLoopHandling.Ignore }
            ).ToString();           

            Console.WriteLine(ser);
            JsonConvert.DeserializeObject<A>(ser);
        }

        static void Main(string[] args)
        {
            //Test1();

            Test2();

            //Test3();

            //Test4();

            //Test5();

            //Test6();

            //Test7();

            Console.ReadKey();
        }

        private static void Test7()
        {
            Console.WriteLine(DateTime.Now.ToString("ddMMyy_hhmmss_FF"));

        }

        private static void Test6()
        {
            var b = new B();
            Console.WriteLine($"{b.x} {b.xr} | {b.y} {b.yr}");
            Thread.Sleep(1000);
            Console.WriteLine($"{b.x} {b.xr} | {b.y} {b.yr}");
            Thread.Sleep(1000);
            Console.WriteLine($"{b.x} {b.xr} | {b.y} {b.yr}");
        }
    }

    interface I {
        string x { get;}
        int y { get;}
    }

    class Base {
        public string Type => GetType().Name;
    
        public Func<string> Opts { get; set; } = () => "hello";
    }

    class A : Base, I
    {
        private int _c = 0;
        public int y => _c++;
        public string x { get => $"Hello {DateTime.Now}";}
    }

    class B : I {
        public B() {
            a = new A();
            x = a.x;
            y = a.y;
        }

        private I a;
        public string x { get; private set; }
        public string xr => a?.x;

        public int y { get; private set; }
        public int yr => a?.y ?? 0;

    }




}
