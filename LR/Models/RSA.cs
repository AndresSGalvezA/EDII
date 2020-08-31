using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;

namespace LR.Models
{
    public class RSA
    {
        public static long e;
        public static long p;
        public static long q;
        public static long n;
        public static long d;
        int c1;
        public static BigInteger C;

        public void GeneratePrivateKey(long number1, long number2, int key, string pathWritng)
        {
            p = number1;
            q = number2;
            n = p * q;
            var QN = (p - 1) * (q - 1);
            e = Coprime(QN, p, q);
            d = CalculateD(QN, e);
            GetKeyN(key, pathWritng);
        }

        public int GeneratePublicKey(long number1, long number2, int key, string pathWritng)
        {
            p = number1;
            q = number2;
            n = p * q;
            var QN = (p - 1) * (q - 1);
            e = Coprime(QN, p, q);
            d = CalculateD(QN, e);
            BigInteger C = BigInteger.Pow(key, Convert.ToInt32(e)) % n;
            GetKeyC(key, pathWritng); 
            c1 = (int)C;
            return c1;
        }

        private static long Coprime(long Qn, long p, long q)
        {
            int counter = 2;
            bool CoprimeFound = false;
            var Coprime = 0;

            var MultiposPhe = new List<BigInteger>();

            var NewNumber = Qn;

            while (counter < Qn && MultiposPhe.Count < 10)
            {
                while (NewNumber % counter == 0)
                {
                    if (!MultiposPhe.Contains(counter)) MultiposPhe.Add(counter);
                    NewNumber /= counter;
                }
                if (NewNumber == 1) break;
                counter++;
            }
            
            if (!MultiposPhe.Contains(p)) MultiposPhe.Add(p);
            if (!MultiposPhe.Contains(q)) MultiposPhe.Add(q);

            var counter2 = 2;

            while (CoprimeFound == false && counter2 < Qn)
            {
                var counterCases = 0;

                foreach (var item in MultiposPhe) 
                    if (counter2 % item != 0) counterCases++;

                if (counterCases == MultiposPhe.Count)
                {
                    CoprimeFound = true;
                    Coprime = counter2;
                }

                counter2++;
            }

            return Coprime;
        }

        private static long CalculateD(long phe_, long e_)
        {
            var Matrix = new long[2, 2] { { phe_, phe_ }, { e_, 1 } };

            while (Matrix[1, 0] != 1)
            {
                var Division = Matrix[0, 0] / Matrix[1, 0];
                var NewPositionA = Matrix[0, 0] - (Division * Matrix[1, 0]);
                var NewPositionB = Matrix[0, 1] - (Division * Matrix[1, 1]);

                if (NewPositionB < 0)
                {
                    while (NewPositionB < 0)
                    {
                        NewPositionB += phe_;
                    }
                }

                var AuxA = Matrix[1, 0];
                var AuxB = Matrix[1, 1];

                Matrix[0, 0] = AuxA;
                Matrix[0, 1] = AuxB;
                Matrix[1, 0] = NewPositionA;
                Matrix[1, 1] = NewPositionB;
            }

            return Matrix[1, 1];
        }

        public void GetKeyN(int key, string pathWriting)
        {
            using var Writer = File.AppendText(Path.GetFullPath(pathWriting));
            BigInteger N = BigInteger.Pow(key, Convert.ToInt32(d)) % n;
            using var writer = File.AppendText(Path.GetFullPath("Keys.txt"));
            Writer.Write(N);
            writer.WriteLine(N + ", " + "RSA");
        }

        public void GetKeyC(int key, string pathWriting)
        {
            using var Writer = File.AppendText(Path.GetFullPath(pathWriting));
            BigInteger C = BigInteger.Pow(key, Convert.ToInt32(e)) % n;
            Writer.Write(C);
        }
        
        public Stack<string> GetKeys()
        {
            var KeysStack = new Stack<string>();

            using (var Reader = new StreamReader(Path.GetFullPath("Keys.txt")))
            {
                while (!Reader.EndOfStream)
                {
                    KeysStack.Push(Reader.ReadLine());
                }
            }

            return KeysStack;
        }
    }
}