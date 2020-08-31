using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace L4.Models
{
    public class Caesar
    {
        private static Dictionary<char, char> CharactersTable;
        private List<char> PasswordCreation;
        private List<char> Alphabet;
        private const int bufferlength = 750;

        private void CipherDictionary(string Password, string File_Path)
        {
            var buffer = new char[bufferlength];

            using (var file = new FileStream(File_Path, FileMode.Open))
            {
                using var reader = new BinaryReader(file);

                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    buffer = reader.ReadChars(bufferlength);

                    foreach (var item in buffer)
                    {
                        Alphabet.Add(item);
                    }
                }
            }

            char[] password = Password.ToCharArray();

            for (int i = 0; i < password.Length; i++)
            {
                if (!PasswordCreation.Contains(password[i]) && Alphabet.Contains(password[i])) PasswordCreation.Add(password[i]);
            }

            foreach (var item in Alphabet)
            {
                if (!PasswordCreation.Contains(item)) PasswordCreation.Add(item);
            }

            var key = Alphabet.ToArray();
            var value = PasswordCreation.ToArray();

            for (int i = 0; i < Alphabet.Count; i++)
            {
                CharactersTable.Add(key[i], value[i]);
            }
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

        private void DecipherDictionary(string Password, string FilePath)
        {

            var buffer = new char[bufferlength];

            using (var file = new FileStream(FilePath, FileMode.Open))
            {
                using var reader = new BinaryReader(file);

                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    buffer = reader.ReadChars(bufferlength);

                    foreach (var item in buffer)
                    {
                        Alphabet.Add(item);
                    }
                }

            }

            char[] password = Password.ToCharArray();

            for (int i = 0; i < password.Length; i++)
            {
                if (!PasswordCreation.Contains(password[i]) && Alphabet.Contains(password[i])) PasswordCreation.Add(password[i]);
            }

            foreach (var item in Alphabet)
            {
                if (!PasswordCreation.Contains(item)) PasswordCreation.Add(item);
            }

            var key = Alphabet.ToArray();
            var value = PasswordCreation.ToArray();

            for (int i = 0; i < Alphabet.Count; i++)
            {
                CharactersTable.Add(value[i], key[i]);
            }
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

        public void Cipher(string FilePath, string TextPath, string Password, string WritingPath)
        {
            CharactersTable = new Dictionary<char, char>();
            PasswordCreation = new List<char>();
            Alphabet = new List<char>();
            CipherDictionary(Password, FilePath);
            WriteCipher(TextPath, WritingPath);
        }

        public void Decipher(string FilePath, string TextPath, string Password, string WritingPath)
        {
            CharactersTable = new Dictionary<char, char>();
            PasswordCreation = new List<char>();
            Alphabet = new List<char>();
            DecipherDictionary(Password, FilePath);
            WriteDecipher(TextPath, WritingPath);
        }
    }
}
