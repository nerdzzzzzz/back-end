namespace Nerdz.Domain.Authorization
{
    /// <summary>
    /// Define os nomes (chaves) dos claims customizados que 
    /// serão armazenados no token JWT.
    /// </summary>
    public static class AppClaimTypes
    {
        /// <summary>
        /// O "cargo" do usuário (ex: Admin, User, ProUser).
        /// </summary>
        public const string Role = "role";

        /// <summary>
        /// Se o usuário tem o benefício "premium" (ex: "true").
        /// </summary>
        public const string Premium = "premium";
    }
}
