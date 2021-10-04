using SQLite;

namespace BookLibrary.Models
{
    public class BookItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Category { get; set; }
        public string PublishedYear { get; set; }
        public string Publisher { get; set; }
        public string NoPages { get; set; }
        public string Isbn { get; set; }
        public string Summary { get; set; }
        public string ImageUrl { get; set; }
    }
}