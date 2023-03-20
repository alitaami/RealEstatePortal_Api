using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Common
{

    /// <summary>
    /// for when we want to know we should create that class in DB
    /// </summary>
    public interface IEntity
    {


    }

    /// <summary>
    /// for when  ID type is not int
    /// </summary>
    public abstract class BaseEntity<TKey> : IEntity
    {
        public TKey Id { get; set; }

    }

    public abstract class BaseEntity : BaseEntity<int>
    {

    }
}
