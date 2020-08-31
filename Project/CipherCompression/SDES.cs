using System;
using System.IO;
using Project.Tree;

namespace Project.Models
{
    public class SDES
    {
        private static string k1 = "";
        private static string k2 = "";
        
        static string Permute(string key, int type)
        {
            if (type == 1) return $"{ key[9] }{ key[5] }{ key[4] }{ key[6] }{ key[1] }{ key[7] }{ key[0] }{ key[8] }{ key[3] }{ key[2] }";
            if(type == 2) return $"{ key[2] }{ key[5] }{ key[9] }{ key[7] }{ key[3] }{ key[1] }{ key[0] }{ key[8] }";
            if(type == 3) return $"{ key[3] }{ key[0] }{ key[2] }{ key[1] }";
            return string.Empty;
        }

        static string P_1(string Key, bool Inv)
        {
            if (!Inv) return $"{ Key[5] }{ Key[2] }{ Key[0] }{ Key[3] }{ Key[7] }{ Key[6] }{ Key[1] }{ Key[4] }";
            return $"{ Key[2] }{ Key[6] }{ Key[1] }{ Key[3] }{ Key[7] }{ Key[0] }{ Key[5] }{ Key[4] }";
        }

        static string LS(string key, int type)
        {
            if (type == 1) return $"{ key[1] }{ key[2] }{ key[3] }{ key[4] }{ key[0] }";
            return $"{ key[2] }{ key[3] }{ key[4] }{ key[0] }{ key[1] }";
        }

        static string XOR(string key1, string key2)
        {
            var aux = string.Empty;

            for (int i = 0; i < key1.Length; i++)
                aux += key1[i] == key2[i] ? '0' : '1';

            return aux;
        }

        static string EP(string key)
        {
            return $"{ key[0] }{ key[3] }{ key[2] }{ key[1] }{ key[2] }{ key[1] }{ key[0] }{ key[3] }";
        }

        static string S01(string key)
        {
            var S0 = new string[4, 4] { { "00", "01", "11", "10" }, { "11", "00", "01", "00" }, { "10", "01", "00", "11" }, { "01", "10", "00", "00" } };
            var S1 = new string[4, 4] { { "00", "10", "11", "00" }, { "11", "00", "01", "01" }, { "10", "10", "00", "10" }, { "10", "00", "11", "00" } };
            return $"{S0[Convert.ToInt32($"{key[0]}{key[3]}", 2), Convert.ToInt32($"{key[1]}{key[2]}", 2)]}{S1[Convert.ToInt32($"{key[4]}{key[7]}", 2), Convert.ToInt32($"{key[5]}{key[6]}", 2)]}";
        }

        static string SW(string key)
        {
            return $"{key.Substring(4)}{key.Substring(0, 4)}";
        }

        static void CreateKey(string key, bool cipher)
        {
            var ResultLS = Permute(key, 1);
            var LS1 = $"{LS(ResultLS.Substring(0, 5), 1)}{LS(ResultLS.Substring(5), 1)}";
            k1 = cipher ? Permute(LS1, 2) : Permute($"{LS(LS1.Substring(0, 5), 2)}{LS(LS1.Substring(5), 2)}", 2);
            k2 = cipher ? Permute($"{LS(LS1.Substring(0, 5), 2)}{LS(LS1.Substring(5), 2)}", 2) : Permute(LS1, 2);
        }

        static string fk(string key, string k)
        {
            var ResultXOR1 = XOR(k, EP(key.Substring(4)));
            var ResultXOR2 = XOR(key.Substring(0, 4), Permute(S01(ResultXOR1), 3));
            return $"{ResultXOR2}{key.Substring(4)}";
        }

        public static string CipDec(string info, bool cipher)
        {
            var res = "";

            if (TreeSingleton.Instance.Key < 1024)
            {
                CreateKey(Convert.ToString(TreeSingleton.Instance.Key, 2).PadLeft(10, '0'), cipher);

                foreach (var character in info)
                {
                    var binCharacter = Convert.ToString(Convert.ToInt32(character), 2).PadLeft(8, '0');
                    var ResultIP = P_1(binCharacter, true);
                    var ResultSW = SW(fk(ResultIP, k1));
                    var ResultIP2 = P_1(fk(ResultSW, k2), false);
                    res += Convert.ToChar(Convert.ToInt32(ResultIP2, 2));
                }
            }

            return res;
        }

        public static void GetKey()
        {
            if (!Directory.Exists("Key")) Directory.CreateDirectory("Key");

            if (!File.Exists("Key//key.txt"))
            {
                var text = CipDec(TreeSingleton.Instance.Key.ToString(), true);
                File.WriteAllText("Key//key.txt", text);
            }
            else TreeSingleton.Instance.Key = Convert.ToInt32(CipDec(File.ReadAllText("Datos//k.txt"), false));
        }
    }
}