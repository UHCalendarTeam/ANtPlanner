using DataLayer.Models.Entities.ResourcesAndCollections;

namespace DataLayer.Models.Interfaces.Repositories
{
    public interface IHomeRepository : IUrlContainerRepository<CalendarHome>,IPropertyContainerRepository<CalendarHome, string>
    {
        CalendarHome FindWihtPropertiesAndCalendarCollections(string url);

        CalendarHome FindWihtCalendarCollections(string url);
    }
}
