
using System;
using System.Collections.Generic;
using System.Linq;

namespace TestLinq2DbAspCore.Models
{
    public class DAL_Gateway // : BEntity
    {
        public Guid Id { get; set; }

        public DAL_Gateway()
        {
            GatewaySets = new HashSet<DAL_GatewaySettings>();
        }

        public long SerialNum { get; set; }
        
        public IEnumerable<DAL_GatewaySettings> GatewaySets { get; set; }
    }

    public class DAL_GatewaySettings //: BSettingsEntry
	{
        public Guid Id { get; set; }
        public Guid GatewayId { get; set; }
        public DAL_Gateway Gateway { get; set; }
	}
}
