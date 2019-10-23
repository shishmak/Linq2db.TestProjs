using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestLinq2DbAspCore.Models
{
    /// <summary>
    /// интерфейс для всех сущностей в БД 
    /// </summary>
    public interface IEntity
    {
        Guid Id { get; set; }
    }
}
