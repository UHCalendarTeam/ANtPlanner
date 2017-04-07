using DataLayer.Models.Entities.ResourcesAndCollections;

namespace DataLayer.Models.Interfaces.Repositories
{
    public interface IHomeRepository : IUrlContainerRepository<CalendarHome>,IPropertyContainerRepository<CalendarHome, string>
    {
        CalendarHome FindWithPropertiesAndCalendarCollections(string url);

        CalendarHome FindWithCalendarCollections(string url);
    }
}
