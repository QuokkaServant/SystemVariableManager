using System.Diagnostics;
using System.Security.Cryptography;

namespace SystemVariableService.Utils
{
    public static class DataProtectionApi
    {
        public static byte[]? Encryption(byte[]? data, byte[]? optionalEntropy = null, DataProtectionScope scope = DataProtectionScope.CurrentUser)
        {
            try
            {
                return data == null ? null : ProtectedData.Protect(data, optionalEntropy, scope);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }

        public static byte[]? Decryption(byte[]? data, byte[]? optionalEntropy = null, DataProtectionScope scope = DataProtectionScope.CurrentUser)
        {
            try
            {
                return data == null ? null : ProtectedData.Unprotect(data, optionalEntropy, scope);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                return null;
            }
        }
    }
}
