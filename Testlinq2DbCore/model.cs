using LinqToDB;
using LinqToDB.DataProvider.SqlServer;
using LinqToDB.Extensions;
using LinqToDB.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace TestLinq2Db
{
    public partial class ExDbModel : LinqToDB.Data.DataConnection
    {
        public ITable<billing_Devtype> Devtypes { get { return this.GetTable<billing_Devtype>(); } }
        public ITable<billing_Device> Devices { get { return this.GetTable<billing_Device>(); } }
        public ITable<billing_DevReadingType> DevReadingTypes { get { return this.GetTable<billing_DevReadingType>(); } }
        public ITable<billing_TempReading> TempReadings { get { return this.GetTable<billing_TempReading>(); } }
        public ITable<billing_TestEntity> TestTable { get { return this.GetTable<billing_TestEntity>(); } }

        public ExDbModel()
        {
            InitDataContext();
            InitMappingSchema();
        }

        public ExDbModel(string configuration)
            : base(configuration)
        {
            InitDataContext();
            InitMappingSchema();
        }

        public ExDbModel(string provider, string connection)
            : base(provider, connection)
        {
            InitDataContext();
            InitMappingSchema();
        }

        partial void InitDataContext();
        partial void InitMappingSchema();

        #region FreeTextTable

        public class FreeTextKey<T>
        {
            public T Key;
            public int Rank;
        }

        private static MethodInfo _freeTextTableMethod1 = typeof(ExDbModel).GetMethod("FreeTextTable", new Type[] { typeof(string), typeof(string) });

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
            typeof(ExDbModel).GetMethods()
                .Where(m => m.Name == "FreeTextTable" && m.IsGenericMethod && m.GetParameters().Length == 2)
                .Where(m => m.GetParameters()[0].ParameterType.IsGenericTypeEx() && m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(Expression<>))
                .Where(m => m.GetParameters()[1].ParameterType == typeof(string))
                .Single();

        [FreeTextTableExpression]
        public ITable<FreeTextKey<TKey>> FreeTextTable<TTable, TKey>(Expression<Func<TTable, string>> fieldSelector, string text)
        {
            return this.GetTable<FreeTextKey<TKey>>(
                this,
                _freeTextTableMethod2,
                fieldSelector,
                text);
        }

        #endregion
    }

    [Table(Schema = "billing", Name = "devtypes")]
    //[Table(Schema = "dbo", Name = "devtypes")]
    public partial class billing_Devtype
    {
        [Column("devtypeid"), PrimaryKey, Identity] public int Devtypeid { get; set; } // integer
        [Column("typename"), NotNull] public string Typename { get; set; } // character varying(255)
        [Column(), NotNull] public int GlobalType { get; set; } // integer

        #region Associations

        /// <summary>
        /// FK_billing.DevReadingType_billing.devtypes_DevTypeId_BackReference
        /// </summary>
        [Association(ThisKey = "Devtypeid", OtherKey = "DevTypeId", CanBeNull = true, Relationship = Relationship.OneToMany, IsBackReference = true)]
        public IEnumerable<billing_DevReadingType> BillingDevReadingTypebillingdevtypesDevTypeIds { get; set; }

         /// <summary>
        /// fk_devices_devtypeid_devtypes_devtypeid_BackReference
        /// </summary>
        [Association(ThisKey = "Devtypeid", OtherKey = "Devtypeid", CanBeNull = true, Relationship = Relationship.OneToMany, IsBackReference = true)]
        public IEnumerable<billing_Device> Fkdevicesdevtypeiddevtypeids { get; set; }

        #endregion
    }

    [Table(Schema = "billing", Name = "devices")]
    //[Table(Schema = "dbo", Name = "devices")]
    public partial class billing_Device
    {
        [Column("devid"), PrimaryKey, NotNull] public Guid Devid { get; set; } // character varying(255)
        [Column("sernum"), Nullable] public string Sernum { get; set; } // character varying(255)

        [Column("devtypeid"), NotNull] public int Devtypeid { get; set; } // integer

        #region Associations

        /// <summary>
        /// FK_billing.TempReading_billing.devices_devid_BackReference
        /// </summary>
        [Association(ThisKey = "Devid", OtherKey = "Devid", CanBeNull = true, Relationship = Relationship.OneToMany, IsBackReference = true)]
        public IEnumerable<billing_TempReading> BillingTempReadingbillingdevicesdevids { get; set; }

        /// <summary>
        /// fk_devices_devtypeid_devtypes_devtypeid
        /// </summary>
        [Association(ThisKey = "Devtypeid", OtherKey = "Devtypeid", CanBeNull = false, Relationship = Relationship.ManyToOne, KeyName = "fk_devices_devtypeid_devtypes_devtypeid", BackReferenceName = "Fkdevicesdevtypeiddevtypeids")]
        public billing_Devtype Devtype { get; set; }

        #endregion
    }

    //[Table(Schema = "dbo", Name = "DevReadingType")]
    [Table(Schema = "billing", Name = "DevReadingType")]
    public partial class billing_DevReadingType
    {
        [PrimaryKey, Identity] public int Id { get; set; } // integer
        [Column, NotNull] public int? DevTypeId { get; set; } // integer
        [Column, NotNull] public string Name { get; set; } // text
        [Column, NotNull] public int Responsibility { get; set; } // integer

        #region Associations

        /// <summary>
        /// FK_billing.TempReading_billing.DevReadingType_DevReadingTypeId_BackReference
        /// </summary>
        [Association(ThisKey = "Id", OtherKey = "DevReadingTypeId", CanBeNull = true, Relationship = Relationship.OneToMany, IsBackReference = true)]
        public IEnumerable<billing_TempReading> BillingTempReadingbillingDevReadingTypeDevReadingTypeIds { get; set; }

        /// <summary>
        /// FK_billing.DevReadingType_billing.devtypes_DevTypeId
        /// </summary>
        [Association(ThisKey = "DevTypeId", OtherKey = "Devtypeid", CanBeNull = true, Relationship = Relationship.ManyToOne, KeyName = "FK_billing.DevReadingType_billing.devtypes_DevTypeId", BackReferenceName = "BillingDevReadingTypebillingdevtypesDevTypeIds")]
        public billing_Devtype DevType { get; set; }
        #endregion
    }

    //[Table(Schema = "dbo", Name = "TempReading")]
    [Table(Schema = "billing", Name = "TempReading")]
    public partial class billing_TempReading
    {
        [Column("id"), PrimaryKey, Identity] public int Id { get; set; } // integer

        [Column(), NotNull] public string DevSerNum { get; set; } // text

        [Column("devid"), Nullable] public string Devid { get; set; } // character varying(255)
        [Column("tsdevice"), NotNull] public DateTime Ts { get; set; } // timestamp (6) without time zone
        [Column("value"), NotNull] public decimal Value { get; set; } // numeric(18,2)
        [Column(), Nullable] public int? Devtypeid { get; set; } // integer
        [Column(), Nullable] public int? DevReadingTypeId { get; set; } // integer
        [Column(), Nullable] public string ReadingTypeName { get; set; } // text
        [Column(), NotNull] public int DevGlobalType { get; set; } // integer
        [Column(), NotNull] public int Responsibility { get; set; } // integer

        #region Associations

        /// <summary>
        /// FK_billing.TempReading_billing.devices_devid
        /// </summary>
        [Association(ThisKey = "Devid", OtherKey = "Devid", CanBeNull = true, Relationship = Relationship.ManyToOne, KeyName = "FK_billing.TempReading_billing.devices_devid", BackReferenceName = "BillingTempReadingbillingdevicesdevids")]
        public billing_Device Dev { get; set; }

        /// <summary>
        /// FK_billing.TempReading_billing.DevReadingType_DevReadingTypeId
        /// </summary>
        [Association(ThisKey = "DevReadingTypeId", OtherKey = "Id", CanBeNull = true, Relationship = Relationship.ManyToOne, KeyName = "FK_billing.TempReading_billing.DevReadingType_DevReadingTypeId", BackReferenceName = "BillingTempReadingbillingDevReadingTypeDevReadingTypeIds")]
        public billing_DevReadingType DevReadingType { get; set; }

        #endregion
    }



    //[Table(Schema = "dbo", Name = "TempReading")]
    [Table(Schema = "billing", Name = "TestTable")]
    public partial class billing_TestEntity : BEntity
    {
       

        [Column(), NotNull] public string Name { get; set; } // text
    }

    public class BEntity
    {
        [Column("id"), PrimaryKey, Identity] public Guid Id { get; set; }
    }
}
