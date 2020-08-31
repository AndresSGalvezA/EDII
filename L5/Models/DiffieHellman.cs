using System;
using System.IO;
using System.IO.Compression;
using System.Collections.Generic;
using System.Numerics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using System.Linq;

namespace L5.Models
{
    public class DiffieHellman
    {
        const int p = 107;
        const int g = 43;
        readonly int a;
        readonly int b;
        long B = 1;
        readonly Random rnd = new Random();

        public DiffieHellman(int SecretNumberA)
        {
            a = SecretNumberA;
            b = rnd.Next();
            B = Convert.ToInt64(Math.Pow(g, b)) % p;
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
            return B;
        }

        /// <summary>
        /// Devuelve la llave privada común (K).
        /// </summary>
        public long GetCommonKey()
        {
            return Convert.ToInt64(Math.Pow(B, a)) % p;
        }
    }
}