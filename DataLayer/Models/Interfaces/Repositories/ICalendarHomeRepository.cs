using DataLayer.Models.Entities.ResourcesAndCollections;

namespace DataLayer.Models.Interfaces.Repositories
{
    public interface ICalendarHomeRepository : IPropertyContainerRepository<CalendarHome, string>
    {
        CalendarHome FindWihtPropertiesAndCalendarCollections(string url);

        CalendarHome FindWihtProperties(string url);

        CalendarHome FindWihtCalendarCollections(string url);
    }
}
