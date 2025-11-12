using FirebaseAdmin.Auth;
using Nerdz.Application.Services;

namespace Nerdz.Api.Services
{
    public class FirebaseClaimsService : IAuthClaimsService
    {
        private readonly FirebaseAuth _firebaseAuth;

        public FirebaseClaimsService(FirebaseAuth firebaseAuth)
        {
            _firebaseAuth = firebaseAuth;
        }

        public async Task SetCustomClaimsAsync(string firebaseUid,
                                               Dictionary<string, object> claims,
                                               CancellationToken cancellationToken)
        {
            await _firebaseAuth.SetCustomUserClaimsAsync(firebaseUid, claims, cancellationToken);
        }
    }
}
