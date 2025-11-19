using System.ComponentModel.DataAnnotations;

namespace Nerdz.Domain.Entities
{
    public class User : BaseDomain
    {
        [MaxLength(50)]
        public string Nome { get; set; }

        [MaxLength(50)]
        public string Sobrenome { get; set; }

        [MaxLength(50)]
        public string Email { get; set; }
        public string ProfilePictureUrl { get; set; }

        public DateTime CreatedAt { get; set; } // TODO: Logica de criação pelo userRole (se não tiver, ou seja, jwt puro, é o primeiro login) 
        public DateTime? LastLoginAt { get; set; }

        public Guid? GroupId { get; set; }
        public Group Group { get; set; }

        public Group AdminOfGroup { get; set; }
    }
}
