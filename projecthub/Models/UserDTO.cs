namespace projecthub.Models
{
    public class UserDTO
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string? Name { get; set; }
        public string? Twitter { get; set; }
        public string? Linkedin { get; set; }
        public string? Github { get; set; }
        public int Points { get; set; }
        public List<Project>? Projects { get; set; }

    }
}
