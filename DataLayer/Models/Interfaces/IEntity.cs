namespace DataLayer.Models.Interfaces
{
    //All Entity have id property to make possible find by id.
    public interface IEntity<TKey>
    {
        TKey Id
        { get; set; }
    }
}

