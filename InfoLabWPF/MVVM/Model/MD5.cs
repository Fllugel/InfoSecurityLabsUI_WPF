using System;
using System.IO;
using System.Linq;

public class MD5
{
    private static readonly uint[] T = new uint[64];
    private static readonly uint[] ShiftAmounts = { 7, 12, 17, 22, 5, 9, 14, 20, 4, 11, 16, 23, 6, 10, 15, 21 };

    private uint[] X = new uint[16];

    static MD5()
    {
        for (int i = 0; i < 64; i++)
        {
            T[i] = (uint)(Math.Abs(Math.Sin(i + 1)) * 4294967296);
        }
    }

    private static uint RotateLeft(uint x, int n)
    {
        return (x << n) | (x >> (32 - n));
    }

    private static uint F(uint x, uint y, uint z)
    {
        return (x & y) | (~x & z);
    }

    private static uint G(uint x, uint y, uint z)
    {
        return (x & z) | (y & ~z);
    }

    private static uint H(uint x, uint y, uint z)
    {
        return x ^ y ^ z;
    }

    private static uint I(uint x, uint y, uint z)
    {
        return y ^ (x | ~z);
    }

    private void ProcessBlock(ref uint a, ref uint b, ref uint c, ref uint d)
    {
        uint A = a, B = b, C = c, D = d;

        for (int i = 0; i < 64; i++)
        {
            uint FVal;
            int g;
            if (i < 16)
            {
                FVal = F(B, C, D);
                g = i;
            }
            else if (i < 32)
            {
                FVal = G(B, C, D);
                g = (5 * i + 1) % 16;
            }
            else if (i < 48)
            {
                FVal = H(B, C, D);
                g = (3 * i + 5) % 16;
            }
            else
            {
                FVal = I(B, C, D);
                g = (7 * i) % 16;
            }

            uint temp = D;
            D = C;
            C = B;
            B = B + RotateLeft(A + FVal + X[g] + T[i], (int)ShiftAmounts[i % 4]);
            A = temp;
        }

        a += A;
        b += B;
        c += C;
        d += D;
    }

    public byte[] ComputeHash(byte[] input)
    {
        uint a = 0x67452301;
        uint b = 0xEFCDAB89;
        uint c = 0x98BADCFE;
        uint d = 0x10325476;

        int originalLength = input.Length * 8;
        byte[] paddedInput = PadInput(input);

        for (int i = 0; i < paddedInput.Length / 64; i++)
        {
            Buffer.BlockCopy(paddedInput, i * 64, X, 0, 64);
            ProcessBlock(ref a, ref b, ref c, ref d);
        }

        return BitConverter.GetBytes(a)
            .Concat(BitConverter.GetBytes(b))
            .Concat(BitConverter.GetBytes(c))
            .Concat(BitConverter.GetBytes(d))
            .ToArray();
    }

    private byte[] PadInput(byte[] input)
    {
        int paddingSize = (56 - (input.Length + 1) % 64) % 64;
        byte[] paddedInput = new byte[input.Length + paddingSize + 9];

        Buffer.BlockCopy(input, 0, paddedInput, 0, input.Length);
        paddedInput[input.Length] = 0x80;

        byte[] lengthBytes = BitConverter.GetBytes((ulong)(input.Length * 8));
        Buffer.BlockCopy(lengthBytes, 0, paddedInput, paddedInput.Length - 8, 8);

        return paddedInput;
    }

    public void LoadInputFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("Input file not found.", filePath);
        }

        try
        {
            string inputText = File.ReadAllText(filePath);
            byte[] inputData = System.Text.Encoding.UTF8.GetBytes(inputText);
            byte[] hash = ComputeHash(inputData);

            string hashString = BitConverter.ToString(hash).Replace("-", "").ToUpper();
            Console.WriteLine(hashString);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading input from file: {ex.Message}");
        }
    }

    public void SaveHashToFile(string filePath, byte[] hash)
    {
        try
        {
            string hashString = BitConverter.ToString(hash).Replace("-", "").ToUpper();
            File.WriteAllText(filePath, hashString);
            Console.WriteLine("Hash saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving hash to file: {ex.Message}");
        }
    }
}
