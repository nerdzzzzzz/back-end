using Google.Cloud.Firestore;
using System.ComponentModel.DataAnnotations;

namespace Nerdz.Domain.Entities
{
    public class Group : BaseDomain
    {
        [MaxLength(50)]
        public string Nome { get; set; }
        [MaxLength(50)]
        public string Descricao { get; set; }
        public string GroupProfilePictureUrl { get; set; }
        public ICollection<User> Users { get; set; } = new List<User>();
        public Guid AdminId { get; set; }
        public User Admin { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
