namespace projecthub.Models
{
    public class ProjectDTO
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Imagesurls { get; set; }
        public string? Ytlink { get; set; }
        public string? Creator { get; set; }
        public long? Likes { get; set; } = 0;
        public long? Reports { get; set; } = 0;
    }
}
