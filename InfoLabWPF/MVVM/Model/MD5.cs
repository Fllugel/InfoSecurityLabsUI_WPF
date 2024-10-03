using System;
using System.IO;
using System.Text;

public class MD5
{
    private readonly uint[] _s = new uint[] { 7, 12, 17, 22, 5, 9, 14, 20, 4, 11, 16, 23, 6, 10, 15, 21 };
    private readonly uint[] _k = new uint[64];
    private readonly uint[] _state = new uint[4];

    public MD5()
    {
        Initialize();
        InitializeK();
    }

    public byte[] ComputeHash(byte[] input)
    {
        // Reinitialize the state before each computation
        Initialize();

        // Step 1: Padding the input
        byte[] paddedInput = PadInput(input);
    
        // Step 2: Initialize MD5 buffer
        uint[] M = new uint[16];
    
        // Step 3: Process each 512-bit chunk
        for (int i = 0; i < paddedInput.Length; i += 64)
        {
            Buffer.BlockCopy(paddedInput, i, M, 0, 64);

            // Convert little-endian byte order to uint32 values
            for (int j = 0; j < 16; j++)
            {
                M[j] = BitConverter.ToUInt32(paddedInput, i + j * 4);
            }

            ProcessChunk(M);
        }
    
        // Step 4: Output hash
        return GetResult();
    }

    private void Initialize()
    {
        _state[0] = 0x67452301;
        _state[1] = 0xEFCDAB89;
        _state[2] = 0x98BADCFE;
        _state[3] = 0x10325476;
    }

    private void InitializeK()
    {
        for (uint i = 0; i < 64; i++)
        {
            _k[i] = (uint)(Math.Abs(Math.Sin(i + 1)) * (1L << 32));
        }
    }

    private byte[] PadInput(byte[] input)
    {
        int originalLength = input.Length;
        int paddingLength = (56 - (input.Length + 1) % 64) % 64;
        byte[] padded = new byte[originalLength + paddingLength + 9]; 

        Buffer.BlockCopy(input, 0, padded, 0, originalLength);
        padded[originalLength] = 0x80; 

        ulong lengthBits = (ulong)originalLength * 8;
        byte[] lengthBytes = BitConverter.GetBytes(lengthBits);
        
        if (BitConverter.IsLittleEndian)
        {
            Buffer.BlockCopy(lengthBytes, 0, padded, padded.Length - 8, 8);
        }

        return padded;
    }

    private void ProcessChunk(uint[] M)
    {
        uint a = _state[0];
        uint b = _state[1];
        uint c = _state[2];
        uint d = _state[3];

        for (uint i = 0; i < 64; i++)
        {
            uint f, g;

            if (i < 16)
            {
                f = (b & c) | (~b & d);
                g = i;
            }
            else if (i < 32)
            {
                f = (d & b) | (~d & c);
                g = (5 * i + 1) % 16;
            }
            else if (i < 48)
            {
                f = b ^ c ^ d;
                g = (3 * i + 5) % 16;
            }
            else
            {
                f = c ^ (b | ~d);
                g = (7 * i) % 16;
            }

            uint temp = d;
            d = c;
            c = b;
            b = b + RotateLeft(a + f + _k[i] + M[g], (int)_s[(i / 16) * 4 + (i % 4)]);
            a = temp;
        }

        _state[0] += a;
        _state[1] += b;
        _state[2] += c;
        _state[3] += d;
    }

    private byte[] GetResult()
    {
        byte[] result = new byte[16];
        for (int i = 0; i < 4; i++)
        {
            byte[] temp = BitConverter.GetBytes(_state[i]);
            
            if (BitConverter.IsLittleEndian)
            {
                Buffer.BlockCopy(temp, 0, result, i * 4, 4);
            }
        }
        return result;
    }

    private uint RotateLeft(uint x, int n)
    {
        return (x << n) | (x >> (32 - n));
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
