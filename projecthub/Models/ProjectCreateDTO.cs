namespace projecthub.Models
{
    public class ProjectCreateDTO
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Imagesurls { get; set; }
        public string? Ytlink { get; set; }
        public string? creator { get; set; }
        public string? visibility { get; set; }
    }
}
