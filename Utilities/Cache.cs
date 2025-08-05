using System.Collections.Concurrent;
using System.Security.Cryptography;

namespace BusinessSearchTool;

public class Cache
{
    public static readonly ConcurrentDictionary<string, string> FileHashCache = new();
    public static string ComputeHash(byte[] bytes)
    {
        using var sha256 = SHA256.Create();
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToHexString(hash);
    }
}