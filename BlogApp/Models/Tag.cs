namespace BlogApp.Models
{
    public class Tag
    {
        public int TagId { get; set; }
        public string Name { get; set; }

        public ICollection<Article> Articles { get; set; } = new List<Article>();
    }
}
