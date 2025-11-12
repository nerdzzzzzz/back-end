using Google.Cloud.Firestore;
using MediatR;
using Nerdz.Application.Services;
using Nerdz.Domain.Authorization;
using Nerdz.Domain.Entities;

namespace Nerdz.Application.Commands.SyncUsers
{
    internal class SyncUserCommandHandler : IRequestHandler<SyncUserCommand, UserProfile>
    {
        private readonly CollectionReference _usersCollection;
        private readonly IAuthClaimsService _authClaimsService;

        public SyncUserCommandHandler(FirestoreDb firestoreDb, IAuthClaimsService authClaimsService)
        {
            _usersCollection = firestoreDb.Collection("users");
            _authClaimsService = authClaimsService;
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
                NomeCompleto = request.Nome ?? "Usuário",
                Role = AppRoles.User,
                GrupoId = string.Empty
            };

            await userDocRef.SetAsync(newUserProfile, cancellationToken: cancellationToken);

            try
            {
                var claims = new Dictionary<string, object>
                {
                    { AppClaimTypes.Role, AppRoles.User }
                };

                await _authClaimsService.SetCustomClaimsAsync(firebaseUid, claims, cancellationToken);
            }
            catch (Exception ex)
            {
                new Exception($"Erro ao definir Custom Claims para {firebaseUid}: {ex.Message}");
            }

            return newUserProfile;
        }
    }
}
