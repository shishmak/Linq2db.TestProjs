using System;
using System.Collections.Generic;

namespace TestLinq2DbAspCore.Models
{
    public partial class Devices
    {
        public Guid Devid { get; set; }
        public string Sernum { get; set; }
        public int Devtypeid { get; set; }
    }
}
