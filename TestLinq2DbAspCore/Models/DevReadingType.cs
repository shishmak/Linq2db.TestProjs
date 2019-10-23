using System;
using System.Collections.Generic;

namespace TestLinq2DbAspCore.Models
{
    public partial class DevReadingType
    {
        public int Id { get; set; }
        public int DevTypeId { get; set; }
        public string Name { get; set; }
        public int Responsibility { get; set; }
    }
}
