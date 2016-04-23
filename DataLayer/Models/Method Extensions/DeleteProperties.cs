using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLayer.Models.Entities;

namespace DataLayer.Models.Method_Extensions
{
    public static class DeleteProperties
    {
        public static void DeleteResource(this CalDavContext context, CalendarResource resource)
        {
            context.Properties.RemoveRange(resource.Properties);
            context.CalendarResources.Remove(resource);
        }

        public static void DeleteCollection(this CalDavContext context, CalendarCollection collection)
        {
            context.Properties.RemoveRange(collection.Properties);
            foreach (var resource in collection.CalendarResources)
            {
                context.DeleteResource(resource);
            }
            context.CalendarCollections.Remove(collection);
        }
    }
}
