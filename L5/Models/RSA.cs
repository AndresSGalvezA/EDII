using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using L5.Controllers;
using Microsoft.AspNetCore.Hosting;

namespace L5.Models
{
    public class RSA
    {
        public static int e;

        public void GenerateKey(int number1, int number2, string path, string pathPublic, string pathPrivate)
        {
            var p = number1;
            var q = number2;
            var n = p * q;
            var QN = (p - 1) * (q - 1);
            e = Coprime(QN, p, q);
            var d = CalculateD(QN, e);
            GetKeys(n, e, d, pathPublic, pathPrivate);
        }
        private static int Coprime(BigInteger Qn, BigInteger p, BigInteger q)
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
                    if (!MultiposPhe.Contains(counter))
                    {
                        MultiposPhe.Add(counter);
                    }
                    NewNumber = NewNumber / counter;

                }
                if (NewNumber == 1)
                {
                    break;
                }
                counter++;
            }
            if (!MultiposPhe.Contains(p))
            {
                MultiposPhe.Add(p);
            }
            if (!MultiposPhe.Contains(q))
            {
                MultiposPhe.Add(q);
            }

            var contador2 = 2;
            while (CoprimeFound == false && contador2 < Qn)
            {
                var ContadorCases = 0;
                foreach (var item in MultiposPhe)
                {
                    if (contador2 % item != 0)
                    {
                        ContadorCases++;
                    }
                }
                if (ContadorCases == MultiposPhe.Count)
                {
                    CoprimeFound = true;
                    Coprime = contador2;

                }


                contador2++;
            }
            return Coprime;

        }
        private static int CalculateD(int phe_, int e_)
        {
            var Matrix = new int[2, 2]
            {
                   { phe_,phe_ },
                   { e_,1 }
            };

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
        public void GetKeys(int n, int e, int d, string pathPublic, string pathPrivate)
        {
            using var Writer = File.AppendText(Path.GetFullPath(pathPublic));
            Writer.WriteLine("n" + "," + "e");
            Writer.WriteLine(n +","+ e);

            using var WriterPrivate = File.AppendText(Path.GetFullPath(pathPrivate));
            WriterPrivate.WriteLine("n" + "," + "d");
            WriterPrivate.WriteLine(n + "," + d);
        }

    }
}