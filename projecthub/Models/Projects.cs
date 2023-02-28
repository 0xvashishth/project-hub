using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;

namespace projecthub.Models
{
    public class Projects
    {
        public long Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Imagesurls { get; set; }
        public string? Ytlink { get; set; }
        public User? Creater { get; set; }
        public long? CreaterId { get; set; }
        public string? Visibility { get; set; }
        public long? Likes { get; set; } = 0;
        public long? Reports { get; set; } = 0;
    }
}
