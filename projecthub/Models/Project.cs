using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;

namespace projecthub.Models
{
    public class Project
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public SqlDateTime CreatedAt;
        public SqlDateTime EditedAt { get; set; }
        public string[] Imagesurl { get; set; } = Array.Empty<string>();
        public string Ytlink { get; set; }
        public User Creater { get; set; }
        public string Visibility { get; set; }
        public long Likes { get; set; } = 0;
        public long Reports { get; set; } = 0;
    }
}
