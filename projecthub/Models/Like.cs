namespace projecthub.Models
{
    public class Like
    {
        public long Id { get; set; }
        public User user { get; set; }
        public Project project { get; set; }
    }
}
