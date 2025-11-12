using Google.Cloud.Firestore;
using MediatR;
using Nerdz.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.Security.Authentication;

namespace Nerdz.Application.Commands.CreateGroup
{
    public class CreateGroupCommandHandler : IRequestHandler<CreateGroupCommand, GroupProfile>
    {
        private readonly FirestoreDb _db;
        private readonly CollectionReference _gruposCollection;
        private readonly CollectionReference _usuariosCollection;

        public CreateGroupCommandHandler(FirestoreDb firestoreDb)
        {
            _db = firestoreDb;
            _gruposCollection = _db.Collection("grupos"); // Coleção de Grupos
            _usuariosCollection = _db.Collection("usuarios"); // Coleção de Usuários
        }

        public async Task<GroupProfile> Handle(CreateGroupCommand request, CancellationToken cancellationToken)
        {

            var userDocRef = _usuariosCollection.Document(request.FirebaseUid);
            var userSnapshot = await userDocRef.GetSnapshotAsync(cancellationToken);

            if (!userSnapshot.Exists)
                throw new ValidationException("Perfil de usuário não encontrado.");

            var userProfile = userSnapshot.ConvertTo<UserProfile>();

            if (!string.IsNullOrEmpty(userProfile.GrupoId))
                throw new ValidationException("Usuário já pertence a um grupo.");

            var newGroup = new GroupProfile
            {
                Nome = request.Nome,
                Descricao = request.Descricao,
                AdminId = request.FirebaseUid
            };

            DocumentReference groupRef = await _gruposCollection.AddAsync(newGroup, cancellationToken);
            var updates = new Dictionary<string, object>
            {
                { "grupoId", groupRef.Id }
            };
            await userDocRef.UpdateAsync(updates, cancellationToken: cancellationToken);
            newGroup.Id = groupRef.Id;
            return newGroup;
        }
    }
}
