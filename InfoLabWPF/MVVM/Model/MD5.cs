using System;
using System.IO;
using System.Text;

public class MD5
{
    public byte[] ComputeHash(byte[] input)
    {
        using (var md5 = System.Security.Cryptography.MD5.Create())
        {
            return md5.ComputeHash(input);
        }
    }

    public void LoadInputFromFile(string filePath, out string inputText)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Input file not found.", filePath);
        }

        StringBuilder contentBuilder = new StringBuilder();

        try
        {
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(stream))
            {
                char[] buffer = new char[4096];
                int bytesRead;

                while ((bytesRead = reader.Read(buffer, 0, buffer.Length)) > 0)
                {
                    contentBuilder.Append(buffer, 0, bytesRead);
                }
            }

            inputText = contentBuilder.ToString();
        }
        catch (Exception ex)
        {
            inputText = string.Empty;
        }
    }

    public void SaveHashToFile(string filePath, string content)
    {
        File.WriteAllText(filePath, content);
    }
}