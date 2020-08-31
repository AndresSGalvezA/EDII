using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace L4.Models
{
    public class Caesar
    {
        char[] Words = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'ñ', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'Ñ', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        private static Dictionary<char, char> CharactersTable;
        private List<char> PasswordCreation;
        private List<char> Alphabet;
        private const int bufferlength = 750;

        private void CreateCipherDictionary(string Password)
        {
            foreach (var item in Words)
                Alphabet.Add(item);

            char[] password = Password.ToCharArray();

            for (int i = 0; i < password.Length; i++)
                if (!PasswordCreation.Contains(password[i]) && Alphabet.Contains(password[i])) PasswordCreation.Add(password[i]);
            
            foreach (var item in Alphabet)
                if (!PasswordCreation.Contains(item)) PasswordCreation.Add(item);
            
            var key = Alphabet.ToArray();
            var value = PasswordCreation.ToArray();

            for (int i = 0; i < Alphabet.Count; i++)
                CharactersTable.Add(key[i], value[i]);
        }

        private void CreateCipherDictionary(int Password)
        {
            var auxQueue = new Queue<char>();
            var auxWord = 'a';
            char[] lowerWords = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'ñ', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            char[] upperWords = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'Ñ', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

            foreach (var item in lowerWords)
            {
                Alphabet.Add(item);
                auxQueue.Enqueue(item);
            }

            for (int i = 0; i < Password; i++)
            {
                auxWord = auxQueue.Dequeue();
                auxQueue.Enqueue(auxWord);
            }

            foreach (var item in upperWords)
            {
                Alphabet.Add(item);
                auxQueue.Enqueue(item);
            }

            PasswordCreation = auxQueue.ToList();
            var key = Alphabet.ToArray();
            var value = PasswordCreation.ToArray();

            for (int i = 0; i < Alphabet.Count; i++)
            {
                CharactersTable.Add(key[i], value[i]);
            }
        }

        private void CreateDecipherDictionary(string Password)
        {
            foreach (var item in Words)
                Alphabet.Add(item);

            char[] password = Password.ToCharArray();

            for (int i = 0; i < password.Length; i++)
                if (!PasswordCreation.Contains(password[i]) && Alphabet.Contains(password[i])) PasswordCreation.Add(password[i]);

            foreach (var item in Alphabet)
                if (!PasswordCreation.Contains(item)) PasswordCreation.Add(item);

            var key = Alphabet.ToArray();
            var value = PasswordCreation.ToArray();

            for (int i = 0; i < Alphabet.Count; i++)
                CharactersTable.Add(value[i], key[i]);
        }

        private void CreateDecipherDictionary(int Password)
        {
            var auxQueue = new Queue<char>();
            var auxWord = 'a';
            char[] lowerWords = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'ñ', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            char[] upperWords = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'Ñ', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

            foreach (var item in lowerWords)
            {
                Alphabet.Add(item);
                auxQueue.Enqueue(item);
            }

            for (int i = 0; i < Password; i++)
            {
                auxWord = auxQueue.Dequeue();
                auxQueue.Enqueue(auxWord);
            }

            foreach (var item in upperWords)
            {
                Alphabet.Add(item);
                auxQueue.Enqueue(item);
            }

            PasswordCreation = auxQueue.ToList();
            var key = Alphabet.ToArray();
            var value = PasswordCreation.ToArray();

            for (int i = 0; i < Alphabet.Count; i++)
                CharactersTable.Add(value[i], key[i]);
        }

        private void WriteCipher(string TextPath, string WritingPath)
        {
            var writing = new char[bufferlength];
            int i = 0;
            var buffer = new char[bufferlength];

            using (var writer = new FileStream(WritingPath, FileMode.Append))
            {
                using var file = new FileStream(TextPath, FileMode.Open);
                using var reader = new BinaryReader(file);

                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    buffer = reader.ReadChars(bufferlength);

                    foreach (var item in buffer)
                    {
                        if (CharactersTable.ContainsKey(item))
                        {
                            writing[i] = (CharactersTable[item]);
                            i++;
                        }
                        else
                        {
                            writing[i] = item;
                            i++;
                        }
                    }

                    i = 0;
                    var writers = Encoding.UTF8.GetBytes(writing);
                    writer.Write(writers, 0, writers.Length);
                    writing = new char[bufferlength];
                }
            }

            CharactersTable = new Dictionary<char, char>();
            PasswordCreation = new List<char>();
            Alphabet = new List<char>();
        }

        private void WriteDecipher(string TextPath, string WritingPath)
        {
            var writing = new char[bufferlength];
            int i = 0;
            var buffer = new char[bufferlength];

            using (var writer = new FileStream(WritingPath, FileMode.Append))
            {
                using var file = new FileStream(TextPath, FileMode.Open);
                using var reader = new BinaryReader(file);

                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    buffer = reader.ReadChars(bufferlength);

                    foreach (var item in buffer)
                    {
                        if (CharactersTable.ContainsKey(item))
                        {
                            writing[i] = (CharactersTable[item]);
                            i++;
                        }
                        else
                        {
                            writing[i] = item;
                            i++;
                        }
                    }

                    i = 0;
                    var Writer = Encoding.UTF8.GetBytes(writing);
                    writer.Write(Writer, 0, Writer.Length);
                    writing = new char[bufferlength];
                }
            }

            CharactersTable = new Dictionary<char, char>();
            PasswordCreation = new List<char>();
            Alphabet = new List<char>();
        }

        public void Cipher(string TextPath, string Password, string WritingPath)
        {
            CharactersTable = new Dictionary<char, char>();
            PasswordCreation = new List<char>();
            Alphabet = new List<char>();
            CreateCipherDictionary(Password);
            WriteCipher(TextPath, WritingPath);
        }

        public void Decipher(string TextPath, string Password, string WritingPath)
        {
            CharactersTable = new Dictionary<char, char>();
            PasswordCreation = new List<char>();
            Alphabet = new List<char>();
            CreateDecipherDictionary(Password);
            WriteDecipher(TextPath, WritingPath);
        }

        public void Cipher(string TextPath, int Password, string WritingPath)
        {
            CharactersTable = new Dictionary<char, char>();
            PasswordCreation = new List<char>();
            Alphabet = new List<char>();
            CreateCipherDictionary(Password);
            WriteCipher(TextPath, WritingPath);
        }

        public void Decipher(string TextPath, int Password, string WritingPath)
        {
            CharactersTable = new Dictionary<char, char>();
            PasswordCreation = new List<char>();
            Alphabet = new List<char>();
            CreateDecipherDictionary(Password);
            WriteDecipher(TextPath, WritingPath);
        }
    }
}