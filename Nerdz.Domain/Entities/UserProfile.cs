using Google.Cloud.Firestore;
using Nerdz.Domain.Authorization;

namespace Nerdz.Domain.Entities
{
    [FirestoreData]
    public class UserProfile
    {
        [FirestoreProperty("email")]
        public string Email { get; set; } = string.Empty;

        [FirestoreProperty("nomeCompleto")]
        public string NomeCompleto { get; set; } = string.Empty;

        [FirestoreProperty("dataCriacao")]
        [ServerTimestamp]
        public Timestamp DataCriacao { get; set; }

        [FirestoreProperty(AppClaimTypes.Role)]
        public string Role { get; set; } = AppRoles.User;

        [FirestoreProperty(AppClaimTypes.Premium)]
        public bool Premium { get; set; } = false;

        [FirestoreProperty("grupoId")]
        public string GrupoId { get; set; } = string.Empty;
    }
}
