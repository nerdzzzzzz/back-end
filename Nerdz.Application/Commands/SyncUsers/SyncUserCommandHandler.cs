using Google.Cloud.Firestore;
using MediatR;
using Nerdz.Domain.Entities;

namespace Nerdz.Application.Commands.SyncUsers
{
    internal class SyncUserCommandHandler : IRequestHandler<SyncUserCommand, UserProfile>
    {
        private readonly CollectionReference _usersCollection;

        public SyncUserCommandHandler(FirestoreDb firestoreDb, CollectionReference usersCollection)
        {
            _usersCollection = firestoreDb.Collection("users");
        }

        public async Task<UserProfile> Handle(SyncUserCommand request, CancellationToken cancellationToken)
        {

            var firebaseUid = request.FirebaseUid;
            if (string.IsNullOrEmpty(firebaseUid))
            {
                throw new UnauthorizedAccessException("Token inválido.");
            }

            var userDocRef = _usersCollection.Document(firebaseUid);
            var snapshot = await userDocRef.GetSnapshotAsync(cancellationToken);

            if (snapshot.Exists)
            {
                return snapshot.ConvertTo<UserProfile>();
            }

            var newUserProfile = new UserProfile
            {
                Email = request.Email ?? "",
                NomeCompleto = request.Nome ?? "Usuário Anônimo"
            };

            await userDocRef.SetAsync(newUserProfile, options: null, cancellationToken);
            return newUserProfile;
        }
    }
}
