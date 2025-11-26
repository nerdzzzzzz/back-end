using MediatR;
using Nerdz.Application.Interfaces.Repositories;
using Nerdz.Application.Services;
using Nerdz.Domain.Authorization;
using Nerdz.Domain.Entities;
using System.Linq.Expressions;

namespace Nerdz.Application.Commands.SyncUsers
{
    internal class SyncUserCommandHandler : IRequestHandler<SyncUserCommand, User>
    {
        private readonly IAuthClaimsService authClaimsService;
        private readonly IRepository<User> userRepository;

        public SyncUserCommandHandler(IAuthClaimsService authClaimsService, IRepository<User> userRepository)
        {
            this.authClaimsService = authClaimsService;
            this.userRepository = userRepository;
        }

        public async Task<User> Handle(SyncUserCommand request, CancellationToken cancellationToken)
        {

            var firebaseUid = request.FirebaseUid;
            if (string.IsNullOrEmpty(firebaseUid))
                throw new UnauthorizedAccessException("Token inválido.");

            Expression<Func<User, bool>> filter =
                u =>
                    u.FirebaseUid == firebaseUid;

            var user = await userRepository.GetFirstAsync(u => u.FirebaseUid == firebaseUid, cancellationToken);
            var newUser = false;
            if (user == null)
            {
                newUser = true;
                user = new User
                {
                    FirebaseUid = request.FirebaseUid,
                    Email = request.Email,
                    CreatedAt = DateTime.UtcNow,
                    LastLoginAt = DateTime.UtcNow,
                };

                await userRepository.SaveOrUpdateAsync(user, cancellationToken);
            }
            else
                user.LastLoginAt = DateTime.UtcNow;

            try
            {
                if (newUser)
                {
                    var claims = new Dictionary<string, object>
                {
                    { AppClaimTypes.Role, AppRoles.User }
                };

                    await authClaimsService.SetCustomClaimsAsync(firebaseUid, claims, cancellationToken);
                }
            }
            catch (Exception ex)
            {
                new Exception($"Erro ao definir Custom Claims para {firebaseUid}: {ex.Message}");
            }
            return user;
        }
    }
}
