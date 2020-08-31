using System;
using System.IO;
using System.Numerics;

namespace LR.Models
{
    public class DiffieHellman
    {
        const int p = 107;
        const int g = 43;
        readonly int a;
        readonly int b;
        BigInteger B = 1;
        BigInteger K;
        long A;
        
        public DiffieHellman(int SecretNumberA)
        {
            a = SecretNumberA;
            b = 6;
            B = (BigInteger.Pow(g, b)) % p;
        }

        /// <summary>
        /// Devuelve la llava pública del usuario (A).
        /// </summary>
        public long GetMainKey()
        {
            return Convert.ToInt64(Math.Pow(g, a)) % p;
        }

        /// <summary>
        /// Devuelve la llave pública de la otra persona (B).
        /// </summary>
        public long GetForeignKey()
        {
            long b1;
            b1 = (long)B;
            return b1;
        }

        /// <summary>
        /// Devuelve la llave privada común (K).
        /// </summary>
        public int GetCommonKey()
        {
            int k1;
            K = (BigInteger.Pow((long)B, a)) % p;
            k1 = (int)K;
            return k1;
        }

        public void FilePath(string patha, string pathA)
        {
            using var Writer = File.AppendText(Path.GetFullPath(patha));
            Writer.Write(a);

            BigInteger A1 = (BigInteger.Pow(g, a)) % p;

            using var Writer1 = File.AppendText(Path.GetFullPath(pathA));
            Writer1.Write(A1);

            using var writer = File.AppendText(Path.GetFullPath("Keys.txt"));
            writer.WriteLine(A1 + "," + "Diffie-Hellman");
        }
    }
}