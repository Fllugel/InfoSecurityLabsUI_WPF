using System;
using InfoLabWPF.MVVM.Model;

namespace InfoLabWPF.MVVM.Model
{
    public class RC5
    {
        private int _wordSize;
        private int _rounds;
        private uint[] _S;
        private readonly LinearCongruentialGenerator _lcg;

        public RC5(byte[] key, uint lcgModulus, uint lcgMultiplier, uint lcgIncrement, uint lcgSeed, int wordSize = 32,
            int rounds = 12)
        {
            if ((wordSize & (wordSize - 1)) != 0)
            {
                throw new ArgumentException("Invalid word size. Must be a power of 2.");
            }

            _lcg = new LinearCongruentialGenerator(lcgModulus, lcgMultiplier, lcgIncrement, lcgSeed);
            Initialize(key, wordSize, rounds);
        }

        private void Initialize(byte[] key, int wordSize, int rounds)
        {
            _wordSize = wordSize;
            _rounds = rounds;

            int u = _wordSize / 8;
            int c = (key.Length + u - 1) / u;
            int t = 2 * (_rounds + 1);

            _S = new uint[t];
            _S[0] = 0xB7E15163;
            for (int i = 1; i < t; i++)
            {
                _S[i] = _S[i - 1] + 0x9E3779B9;
            }

            uint[] L = new uint[c];
            for (int i = 0; i < key.Length; i++)
            {
                L[i / u] = (L[i / u] << 8) + key[i];
            }

            int iA = 0, iB = 0;
            uint A = 0, B = 0;
            int iterations = 3 * Math.Max(c, t);
            for (int i = 0; i < iterations; i++)
            {
                A = _S[iA] = RotateLeft(_S[iA] + A + B, 3);
                B = L[iB] = RotateLeft(L[iB] + A + B, (int)(A + B));
                iA = (iA + 1) % t;
                iB = (iB + 1) % c;
            }
        }

        private uint RotateLeft(uint value, int count)
        {
            return (value << count) | (value >> (_wordSize - count));
        }

        private uint RotateRight(uint value, int count)
        {
            return (value >> count) | (value << (_wordSize - count));
        }

        private byte[] GenerateIVUsingLCG()
        {
            int blockSize = 2 * (_wordSize / 8);
            byte[] initialIV = new byte[blockSize];
            for (int i = 0; i < blockSize; i += 4)
            {
                uint nextValue = _lcg.Next();
                byte[] bytes = BitConverter.GetBytes(nextValue);
                Array.Copy(bytes, 0, initialIV, i, Math.Min(4, blockSize - i));
            }

            return initialIV;
        }

        public void EncryptBlock(ref uint A, ref uint B)
        {
            A += _S[0];
            B += _S[1];
            for (int i = 1; i <= _rounds; i++)
            {
                A = RotateLeft(A ^ B, (int)B) + _S[2 * i];
                B = RotateLeft(B ^ A, (int)A) + _S[2 * i + 1];
            }
        }

        public void DecryptBlock(ref uint A, ref uint B)
        {
            for (int i = _rounds; i > 0; i--)
            {
                B = RotateRight(B - _S[2 * i + 1], (int)A) ^ A;
                A = RotateRight(A - _S[2 * i], (int)B) ^ B;
            }

            B -= _S[1];
            A -= _S[0];
        }

        public byte[] Encrypt(byte[] data)
        {
            int blockSize = 2 * (_wordSize / 8); // Update to the full block size
            if (data.Length == 0)
            {
                byte[] emptyDataIV = GenerateIVUsingLCG();
                Console.WriteLine("Original IV: " + BitConverter.ToString(emptyDataIV));

                uint ivA = BitConverter.ToUInt32(emptyDataIV, 0);
                uint ivB = BitConverter.ToUInt32(emptyDataIV, blockSize / 2);
                EncryptBlock(ref ivA, ref ivB);

                byte[] encryptedData = new byte[blockSize];
                Array.Copy(BitConverter.GetBytes(ivA), 0, encryptedData, 0, 4);
                Array.Copy(BitConverter.GetBytes(ivB), 0, encryptedData, 4, 4);
                return encryptedData;
            }

            int paddedLength = ((data.Length + blockSize - 1) / blockSize) * blockSize;
            byte[] paddedData = new byte[paddedLength];
            Array.Copy(data, paddedData, data.Length);

            byte[] encryptedOutput = new byte[paddedLength + blockSize];
            byte[] initialIV = GenerateIVUsingLCG();

            Console.WriteLine("Original IV: " + BitConverter.ToString(initialIV));

            uint ivFirstPart = BitConverter.ToUInt32(initialIV, 0);
            uint ivSecondPart = BitConverter.ToUInt32(initialIV, blockSize / 2);
            EncryptBlock(ref ivFirstPart, ref ivSecondPart);
            Array.Copy(BitConverter.GetBytes(ivFirstPart), 0, encryptedOutput, 0, 4);
            Array.Copy(BitConverter.GetBytes(ivSecondPart), 0, encryptedOutput, 4, 4);

            byte[] previousBlock = new byte[blockSize];
            Array.Copy(initialIV, 0, previousBlock, 0, blockSize);

            for (int i = 0; i < paddedLength; i += blockSize)
            {
                uint A = BitConverter.ToUInt32(paddedData, i);
                uint B = BitConverter.ToUInt32(paddedData, i + 4);

                uint prevA = BitConverter.ToUInt32(previousBlock, 0);
                uint prevB = BitConverter.ToUInt32(previousBlock, 4);

                A ^= prevA;
                B ^= prevB;

                EncryptBlock(ref A, ref B);

                Array.Copy(BitConverter.GetBytes(A), 0, encryptedOutput, i + blockSize, 4);
                Array.Copy(BitConverter.GetBytes(B), 0, encryptedOutput, i + blockSize + 4, 4);

                Array.Copy(BitConverter.GetBytes(A), 0, previousBlock, 0, 4);
                Array.Copy(BitConverter.GetBytes(B), 0, previousBlock, 4, 4);
            }

            return encryptedOutput;
        }

        public byte[] Decrypt(byte[] data)
        {
            int blockSize = 2 * (_wordSize / 8);
            if (data.Length == blockSize)
            {
                return new byte[0];
            }

            if (data.Length % blockSize != 0)
            {
                throw new ArgumentException("Invalid data length, must be a multiple of the block size.");
            }

            byte[] decryptedOutput = new byte[data.Length - blockSize];

            uint ivFirstPart = BitConverter.ToUInt32(data, 0);
            uint ivSecondPart = BitConverter.ToUInt32(data, blockSize / 2);
            DecryptBlock(ref ivFirstPart, ref ivSecondPart);
            byte[] initialIV = new byte[blockSize];
            Array.Copy(BitConverter.GetBytes(ivFirstPart), 0, initialIV, 0, 4);
            Array.Copy(BitConverter.GetBytes(ivSecondPart), 0, initialIV, 4, 4);

            byte[] previousBlock = new byte[blockSize];
            Array.Copy(initialIV, 0, previousBlock, 0, blockSize);

            for (int i = blockSize; i < data.Length; i += blockSize)
            {
                uint A = BitConverter.ToUInt32(data, i);
                uint B = BitConverter.ToUInt32(data, i + 4);

                uint originalA = A;
                uint originalB = B;

                DecryptBlock(ref A, ref B);

                uint prevA = BitConverter.ToUInt32(previousBlock, 0);
                uint prevB = BitConverter.ToUInt32(previousBlock, 4);

                A ^= prevA;
                B ^= prevB;

                Array.Copy(BitConverter.GetBytes(A), 0, decryptedOutput, i - blockSize, 4);
                Array.Copy(BitConverter.GetBytes(B), 0, decryptedOutput, i - blockSize + 4, 4);

                Array.Copy(BitConverter.GetBytes(originalA), 0, previousBlock, 0, 4);
                Array.Copy(BitConverter.GetBytes(originalB), 0, previousBlock, 4, 4);
            }

            return decryptedOutput;
        }
    }
}