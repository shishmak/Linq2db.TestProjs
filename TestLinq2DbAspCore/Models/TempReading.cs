using System;
using System.Collections.Generic;

namespace TestLinq2DbAspCore.Models
{
    public partial class TempReading
    {
        public int Id { get; set; }
        public string DevSerNum { get; set; }
        public string Devid { get; set; }
        public DateTime Tsdevice { get; set; }
        public decimal Value { get; set; }
        public int? Devtypeid { get; set; }
        public int? DevReadingTypeId { get; set; }
        public string ReadingTypeName { get; set; }
        public int DevGlobalType { get; set; }
        public int Responsibility { get; set; }
    }
}
