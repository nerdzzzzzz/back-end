namespace Nerdz.Application.Services
{
    public interface IAuthClaimsService
    {
        Task SetCustomClaimsAsync(string firebaseUid,
                                  Dictionary<string, object> claims,
                                  CancellationToken cancellationToken);
    }
}
