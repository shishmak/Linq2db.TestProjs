using LinqToDB;
using LinqToDB.Data;
using System;
using System.Linq;

namespace TestLinq2Db
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var db = new ExDbModel())
            {

                DataConnection.TurnTraceSwitchOn();
                DataConnection.WriteTraceLine = (s, s1) => { Console.WriteLine(s); };

                db.CreateTable<billing_Devtype>();
                db.CreateTable<billing_Device>();
                db.CreateTable<billing_DevReadingType>();
                db.CreateTable<billing_TempReading>();

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
                var devs = db.Devices.Join(db.Devtypes, d => d.Devtypeid, dt => dt.Devtypeid, (d, dt) => new { d, dt });

                db.TempReadings
                    .Set(t => t.Devid, u => devs.Where(w => w.d.Sernum == u.DevSerNum && w.dt.GlobalType == u.DevGlobalType).Select(s => s.d.Devid).FirstOrDefault())
                    .Set(t => t.Devtypeid, u => devs.Where(w => w.d.Sernum == u.DevSerNum && w.dt.GlobalType == u.DevGlobalType).Select(s => s.dt.Devtypeid).FirstOrDefault())
                    .Update();

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

            Console.ReadKey();
        }
    }
}
