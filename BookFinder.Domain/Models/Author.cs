namespace BookFinder.Domain.Models;

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

    public ICollection<Book> Books
    {
        get;
        set;
    } = new List<Book>();
}