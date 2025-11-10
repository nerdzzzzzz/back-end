namespace Nerdz.Domain.Authorization
{
    /// <summary>
    /// Define os nomes (strings) das políticas de autorização
    /// usadas em [Authorize(Policy = "...")] e na configuração.
    /// </summary>
    public static class AppPolicies
    {
        /// <summary>
        /// Política que exige que o usuário tenha a role de Admin.
        /// </summary>
        public const string AdminOnly = "AdminOnly";

        /// <summary>
        /// Política que exige que o usuário tenha o claim 'premium' = 'true'.
        /// </summary>
        public const string PremiumUser = "PremiumUser";
    }
}
