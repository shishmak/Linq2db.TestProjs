using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TestLinq2DbAspCore.Models
{
    public class BTest {
        [Key]
        public Guid Id { get; set; }
    }

    public partial class TestTable : BTest
    {
        public string Name { get; set; }
    }
}
