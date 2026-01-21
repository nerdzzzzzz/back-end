using Google.Cloud.Firestore;
using Google.Cloud.Firestore.V1;
using MediatR;
using Nerdz.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Nerdz.Application.Commands.DeleteGroup
{
    internal class DeleteGroupCommandHandler : IRequestHandler<DeleteGroupCommand>
    {
        private readonly FirestoreDb _db;
        private readonly CollectionReference _gruposCollection;
        private readonly CollectionReference _usuariosCollection;

        public DeleteGroupCommandHandler(FirestoreDb firestoreDb)
        {
            _db = firestoreDb;
            _gruposCollection = _db.Collection("grupos"); // Coleção de Grupos
            _usuariosCollection = _db.Collection("usuarios"); // Coleção de Usuários
        }

        public async Task Handle(DeleteGroupCommand request, CancellationToken cancellationToken)
        {
            //    var userDocRef = _usuariosCollection.Document(request.FirebaseUid);
            //    var userSnapshot = await userDocRef.GetSnapshotAsync(cancellationToken);

            //    if (!userSnapshot.Exists)
            //        throw new ValidationException("Perfil de usuário não encontrado.");

            //    var userProfile = userSnapshot.ConvertTo<User>();

            //    if (string.IsNullOrEmpty(userProfile.GrupoId))
            //        throw new ValidationException($"Grupo não achado para o usuario {userProfile.NomeCompleto}");

            //    var groupDocRef = _gruposCollection.Document(userProfile.GrupoId);
            //    var groupSnapShot = await groupDocRef.GetSnapshotAsync(cancellationToken);

            //    if (!groupSnapShot.Exists)
            //        throw new ValidationException("Grupo de usuário não encontrado.");

            //    var userGroup = groupSnapShot.ConvertTo<Group>();

            //    //if (userGroup.AdminId != request.FirebaseUid)
            //    //    throw new ValidationException("O usuário não é um administrador do grupo.");

            //    //Query userListQuery = _usuariosCollection.WhereEqualTo("GrupoId", userProfile.GrupoId);
            //    QuerySnapshot userListSnapshot = await userListQuery.GetSnapshotAsync(cancellationToken);

            //    WriteBatch batch = _db.StartBatch();

            //    foreach (var userDoc in userListSnapshot.Documents)
            //    {
            //        var user = userDoc.ConvertTo<User>();

            //        var updateUser = new Dictionary<string, object>
            //        {
            //            { "grupoId", FieldValue.Delete }
            //        };

            //        batch.Update(userDoc.Reference, updateUser);
            //    }
            //    batch.Delete(groupDocRef);

            //    await batch.CommitAsync(cancellationToken);
        }
    }
}
