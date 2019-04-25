//---------------------------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated by T4Model template for T4 (https://github.com/linq2db/linq2db).
//    Changes to this file may cause incorrect behavior and will be lost if the code is regenerated.
// </auto-generated>
//---------------------------------------------------------------------------------------------------

#pragma warning disable 1591

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using LinqToDB;
using LinqToDB.DataProvider.SqlServer;
using LinqToDB.Extensions;
using LinqToDB.Mapping;

namespace DataModels
{
	/// <summary>
	/// Database       : danfos
	/// Data Source    : (localdb)\mssqllocaldb
	/// Server Version : 13.00.4001
	/// </summary>
	public partial class DanfosDB : LinqToDB.Data.DataConnection
	{
		public ITable<Counter>             Counters              { get { return this.GetTable<Counter>(); } }
		public ITable<EFMigrationsHistory> EFMigrationsHistories { get { return this.GetTable<EFMigrationsHistory>(); } }
		public ITable<Home>                Homes                 { get { return this.GetTable<Home>(); } }

		public DanfosDB()
		{
			InitDataContext();
			InitMappingSchema();
		}

		public DanfosDB(string configuration)
			: base(configuration)
		{
			InitDataContext();
			InitMappingSchema();
		}

		partial void InitDataContext  ();
		partial void InitMappingSchema();

		#region FreeTextTable

		public class FreeTextKey<T>
		{
			public T   Key;
			public int Rank;
		}

		private static MethodInfo _freeTextTableMethod1 = typeof(DanfosDB).GetMethod("FreeTextTable", new Type[] { typeof(string), typeof(string) });

		[FreeTextTableExpression]
		public ITable<FreeTextKey<TKey>> FreeTextTable<TTable, TKey>(string field, string text)
		{
			return this.GetTable<FreeTextKey<TKey>>(
				this,
				_freeTextTableMethod1,
				field,
				text);
		}

		private static MethodInfo _freeTextTableMethod2 = 
			typeof(DanfosDB).GetMethods()
				.Where(m => m.Name == "FreeTextTable" &&  m.IsGenericMethod && m.GetParameters().Length == 2)
				.Where(m => m.GetParameters()[0].ParameterType.IsGenericTypeEx() && m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>))
				.Where(m => m.GetParameters()[1].ParameterType == typeof(string))
				.Single();

		[FreeTextTableExpression]
		public ITable<FreeTextKey<TKey>> FreeTextTable<TTable, TKey>(Expression<Func<TTable,string>> fieldSelector, string text)
		{
			return this.GetTable<FreeTextKey<TKey>>(
				this,
				_freeTextTableMethod2,
				fieldSelector,
				text);
		}

		#endregion
	}

	[Table(Schema="dbo", Name="Counters")]
	public partial class Counter
	{
		[PrimaryKey, Identity] public int    AutoId { get; set; } // int
		[Column,     NotNull ] public string SN     { get; set; } // nvarchar(16)
		[Column,     NotNull ] public int    Value  { get; set; } // int

		#region Associations

		/// <summary>
		/// FK_Homes_Counters_CounterId_BackReference
		/// </summary>
		[Association(ThisKey="AutoId", OtherKey="CounterId", CanBeNull=true, Relationship=Relationship.OneToMany, IsBackReference=true)]
		public IEnumerable<Home> HomesCounterIds { get; set; }

		#endregion
	}

	[Table(Schema="dbo", Name="__EFMigrationsHistory")]
	public partial class EFMigrationsHistory
	{
		[PrimaryKey, NotNull] public string MigrationId    { get; set; } // nvarchar(150)
		[Column,     NotNull] public string ProductVersion { get; set; } // nvarchar(32)
	}

	[Table(Schema="dbo", Name="Homes")]
	public partial class Home
	{
		[PrimaryKey, Identity   ] public int    AutoId    { get; set; } // int
		[Column,     NotNull    ] public string Adress    { get; set; } // nvarchar(512)
		[Column,        Nullable] public int?   CounterId { get; set; } // int

		#region Associations

		/// <summary>
		/// FK_Homes_Counters_CounterId
		/// </summary>
		[Association(ThisKey="CounterId", OtherKey="AutoId", CanBeNull=true, Relationship=Relationship.ManyToOne, KeyName="FK_Homes_Counters_CounterId", BackReferenceName="HomesCounterIds")]
		public Counter Counter { get; set; }

		#endregion
	}

	public static partial class TableExtensions
	{
		public static Counter Find(this ITable<Counter> table, int AutoId)
		{
			return table.FirstOrDefault(t =>
				t.AutoId == AutoId);
		}

		public static EFMigrationsHistory Find(this ITable<EFMigrationsHistory> table, string MigrationId)
		{
			return table.FirstOrDefault(t =>
				t.MigrationId == MigrationId);
		}

		public static Home Find(this ITable<Home> table, int AutoId)
		{
			return table.FirstOrDefault(t =>
				t.AutoId == AutoId);
		}
	}
}

#pragma warning restore 1591