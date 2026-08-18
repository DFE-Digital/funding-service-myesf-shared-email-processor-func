namespace Pds.Shared.EmailProcessor.Services.Interfaces
{
    /// <summary>
    /// The encryption service interface.
    /// </summary>
    public interface IEncryptionService
    {
        /// <summary>
        /// Decrypts a cypher text string using symmetric key.
        /// </summary>
        /// <param name="symmetricKey">The symmetric key to use in decryption.</param>
        /// <param name="cypherText">The cypher text to decrypt.</param>
        /// <returns>The decrypted string.</returns>
        public string DecryptStringFromHex(string symmetricKey, string cypherText);

        /// <summary>
        /// Encrypts a plain text string using symmetric key.
        /// </summary>
        /// /// <param name="symmetricKey">The symmetric key to use in encryption.</param>
        /// <param name="plainText">The plain text to encrypt.</param>
        /// <returns>The encrypted string.</returns>
        public string EncryptStringToHex(string symmetricKey, string plainText);
    }
}
