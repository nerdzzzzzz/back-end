namespace Nerdz.Domain.Authorization
{
    /// <summary>
    /// Define os valores (strings) possíveis para o claim de Role (AppClaimTypes.Role).
    /// </summary>
    public static class AppRoles
    {
        /// <summary>
        /// Acesso total ao sistema.
        /// </summary>
        public const string Admin = "Admin";

        /// <summary>
        /// Usuário com benefícios pagos ("usuário pró").
        /// </summary>
        public const string ProUser = "ProUser";

        /// <summary>
        /// Usuário padrão/comum.
        /// </summary>
        public const string User = "User";
    }
}
