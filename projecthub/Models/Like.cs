namespace projecthub.Models
{
    public class Like
    {
        public long Id { get; set; }
        public User user { get; set; }
        public Projects project { get; set; }
    }
}
