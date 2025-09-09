namespace BookFinder.Domain;

public class Author
{
    public Guid Id
    {
        get; 
        set; 
    }

    public String Name
    {
        get; 
        set;
    } = string.Empty;

    public String OpenLibraryKey
    {
        get;
        set;
    } = string.Empty;

    public ICollection<Book> books
    {
        get;
        set;
    } = new List<Book>();
}