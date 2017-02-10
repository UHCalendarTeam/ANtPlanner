using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataLayer.Models.Interfaces.Repositories
{
   public interface IUrlContainerRepository<T>
   {
         T FindUrl(string url);
   }
}
