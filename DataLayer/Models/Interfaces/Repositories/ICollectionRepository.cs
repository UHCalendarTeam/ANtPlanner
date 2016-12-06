using DataLayer.Models.Entities.ResourcesAndCollections;

namespace DataLayer.Models.Interfaces.Repositories
{
    public interface ICollectionRepository : IPropertyContainerRepository<CalendarCollection,string>
    {
         CalendarCollection FindwithCalendarResource(string url);

         CalendarCollection FindwithPropeties(string url);

         CalendarCollection FindwithPropetiesAndCalendarResource(string url);
    }
}
