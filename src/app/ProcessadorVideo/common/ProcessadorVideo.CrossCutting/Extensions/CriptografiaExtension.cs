using System.Security.Cryptography;
using System.Text;

namespace ProcessadorVideo.CrossCutting.Extensions;

public static class CriptografiaExtension
{

    public static string ToMD5(this string value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        using (MD5 md5Hash = MD5.Create())
        {
            return RetonarHash(md5Hash, value);
        }
    }

    private static string RetonarHash(MD5 md5Hash, string input)
    {
        byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

        StringBuilder sBuilder = new StringBuilder();

        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }

        return sBuilder.ToString();
    }
}
