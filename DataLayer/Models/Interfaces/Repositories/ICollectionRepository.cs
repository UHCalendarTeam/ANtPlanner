using DataLayer.Models.Entities.ResourcesAndCollections;

namespace DataLayer.Models.Interfaces.Repositories
{
    public interface ICollectionRepository :IUrlContainerRepository<CalendarCollection>, IPropertyContainerRepository<CalendarCollection,string>
    {
         CalendarCollection FindwithCalendarResource(string url);

        CalendarCollection FindwithPropetiesAndCalendarResource(string url);
    }
}
