using System;

namespace TestLinq2DbAspCore.Models
{
    internal enum EntryStates
    {
        Unknown,
        Insert,
        Update,
        Nothing
    }

    /// <summary>
    /// Базовая сущность в БД 
    /// </summary>
    public class BEntity : IEntity
    {
        //[Key]
        //[Column(Order = 0)]
        public Guid Id { get; set; }

        internal EntryStates State { get; set; }
    }

    public class BSettingsEntry : BEntity 
    {
        //[Column(Order = 1)]
        public Guid GatewayId { get; set; }
        //[ForeignKey("GatewayId")]
        public DAL_Gateway Gateway { get; set; }
    }
}

