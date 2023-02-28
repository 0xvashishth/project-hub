namespace projecthub.Models
{
    public class User
    {
        public User(long id, string email, string? name, string? twitter, string? linkedin, string? github, byte[]? passwordHash, byte[]? passwordSalt)
        {
            Id=id;
            Email=email;
            Name=name;
            Twitter=twitter;
            Linkedin=linkedin;
            Github=github;
            PasswordHash=passwordHash;
            PasswordSalt = passwordSalt;
        }

        public long Id { get; set; }
        public string Email { get; set; }
        public string? Name { get; set; }
        public string? Twitter { get; set; }
        public string? Linkedin { get; set; }
        public string? Github { get; set; }
        public int Points { get; set; }
        public byte[]? PasswordHash { get; set; } = Array.Empty<byte>();
        public byte[]? PasswordSalt { get; set; } = Array.Empty<byte>();
        public string? Secret_token { get; set; }
        public List<Project>? Projects { get; set; }
    }
}
