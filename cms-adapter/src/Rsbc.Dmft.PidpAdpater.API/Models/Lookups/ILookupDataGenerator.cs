namespace pdipadapter.Models.Lookups;
public interface ILookupDataGenerator<T>
{
    IEnumerable<T> Generate();
    //Task<IEnumerable<T>> GenerateAsync();
}

