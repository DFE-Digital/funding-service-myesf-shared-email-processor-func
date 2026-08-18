using System.Threading.Tasks;

namespace Pds.Shared.EmailProcessor.Services.Interfaces
{
    /// <summary>
    /// Key vault service for retreiving secrets.
    /// </summary>
    public interface IKeyVaultService
    {
        /// <summary>
        /// Retrieves the secret value for the provided secret name.
        /// </summary>
        /// <param name="secretName">Key vault secret name.</param>
        /// <returns>secret value.</returns>
        Task<string> GetSecretValue(string secretName);
    }
}
