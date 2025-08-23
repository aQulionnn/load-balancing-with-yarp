using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;

namespace webapi.Encryption;

public class SecureHttpFilter(SystemSettings settings) 
    : IAsyncResourceFilter
{
    private readonly Aes _cryptoProvider = InitializeAes(settings.EncryptionKey);
    
    public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
    {
        context.HttpContext.Request.Body = DecryptInputStream(context.HttpContext.Request.Body);
        context.HttpContext.Response.Body = EncryptOutputStream(context.HttpContext.Response.Body);

        if (context.HttpContext.Request.QueryString.HasValue)
        {
            var decodeQuery = DecodeString(context.HttpContext.Request.QueryString.Value[1..]);
            context.HttpContext.Request.QueryString = new QueryString($"?{decodeQuery}");
        }
        
        await next();
        await context.HttpContext.Request.Body.DisposeAsync();
        await context.HttpContext.Response.Body.DisposeAsync();
    }

    private CryptoStream EncryptOutputStream(Stream outputStream)
    {
        var encryptor = _cryptoProvider.CreateEncryptor();
        var base64Transform = new ToBase64Transform();
        var encodeStream = new CryptoStream(outputStream, base64Transform, CryptoStreamMode.Write);
        
        return new CryptoStream(encodeStream, encryptor, CryptoStreamMode.Write);
    }

    private CryptoStream DecryptInputStream(Stream inputStream)
    {
        var decryptor = _cryptoProvider.CreateDecryptor();
        var base64Transform = new FromBase64Transform(FromBase64TransformMode.IgnoreWhiteSpaces);
        var decodedStream = new CryptoStream(inputStream, base64Transform, CryptoStreamMode.Read);
        
        return new CryptoStream(decodedStream, decryptor, CryptoStreamMode.Read);
    }

    private string DecodeString(string encryptedText)
    {
        using var memoryStream = new MemoryStream(Convert.FromBase64String(encryptedText));
        using var cryptoStream = new CryptoStream(memoryStream, _cryptoProvider.CreateDecryptor(), CryptoStreamMode.Read);
        using var reader = new StreamReader(cryptoStream);
        
        return reader.ReadToEnd();
    }
    
    private static Aes InitializeAes(string secretKey)
    {
        var paddedKey = secretKey.PadRight(32, '0');
        var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(paddedKey[..32]);
        aes.IV = Encoding.UTF8.GetBytes(paddedKey[..16]);
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;
        return aes;
    }
}