using Google.Cloud.Firestore;

namespace Nerdz.Domain.Entities
{
    [FirestoreData]
    public class GroupProfile
    {
        [FirestoreDocumentId]
        public string Id { get; set; }
        [FirestoreProperty("nome")]
        public string Nome { get; set; } = string.Empty;

        [FirestoreProperty("dataCriacao")]
        [ServerTimestamp]
        public Timestamp DataCriacao { get; set; }

        [FirestoreProperty("descricao")]
        public string Descricao { get; set; } = string.Empty;

        [FirestoreProperty("AdminId")]
        public string AdminId { get; set; } = string.Empty; //admin do grupo fz referencia ao id do usuario
    }
}
