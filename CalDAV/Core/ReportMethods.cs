using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CalDAV.Core
{
    public class ReportMethods:IReportMethods
    {
        public string ExpandProperty()
        {
            throw new NotImplementedException();
        }

        public string CalendarQuery(List<string> filters )
        {
            //realiza una busqueda y devuelve todos los elementos que cumplan los filtros
            //devuelve todos los COR especificados en el pedido
            //con el uso de del elemento XML calendar-data se puede especificar cuales componentes de calendaerio y cuales propiedades retornar
                //de los COR que cumplan los filtros

            throw new NotImplementedException();
        }
    }
}
