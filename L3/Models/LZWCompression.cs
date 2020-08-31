using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace L3.Models
{
    public class LZWCompression
    {
        private int BitsNumber { get; set; }
        private const int BufferLenght = 500;
        private readonly Dictionary<string, int> CharacterTable = new Dictionary<string, int>();
        private readonly Dictionary<string, int> WritingTable = new Dictionary<string, int>();
        private readonly Dictionary<byte, int> ByteTable = new Dictionary<byte, int>();

        public void Compression(string ReadingPath, string WritingPath, string originalName)
        {
            InitialDictionary(ReadingPath);
            BitsNumberMethod(ReadingPath);
            WriteDictionary(WritingPath, originalName);
            CompleteDictionary(ReadingPath, WritingPath);
        }
        
        private void BitsNumberMethod(string ReadingPath)
        {
            var Buffer = new byte[BufferLenght];
            var LastCharacter = "";
            using var file = new FileStream(ReadingPath, FileMode.Open);
            using var reader = new BinaryReader(file);

            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                Buffer = reader.ReadBytes(BufferLenght);

                foreach (var item in Buffer)
                {
                    var bytes = Convert.ToString(Convert.ToChar(item));

                    if (!CharacterTable.ContainsKey((LastCharacter + bytes)))
                    {
                        CharacterTable.Add(LastCharacter + bytes, (CharacterTable.Count + 1));
                        LastCharacter = bytes;
                    }
                    else LastCharacter += bytes;
                }
            }

            var number = CharacterTable.Values.Max();
            var bits = Convert.ToString(number, 2);
            BitsNumber = bits.Length;
        }

        private void InitialDictionary(string ReadingPath)
        {
            var Buffer = new byte[BufferLenght];
            using var file = new FileStream(ReadingPath, FileMode.Open);
            using var reader = new BinaryReader(file);

            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                Buffer = reader.ReadBytes(BufferLenght);

                foreach (var item in Buffer)
                {
                    var bytes = Convert.ToString(Convert.ToChar(item));

                    if (!CharacterTable.ContainsKey(bytes))
                    {

                        if (CharacterTable.Count() == 0)
                        {
                            CharacterTable.Add(bytes, 1);
                            ByteTable.Add(item, 1);
                            WritingTable.Add(bytes, 1);
                        }
                        else
                        {
                            CharacterTable.Add(bytes, (CharacterTable.Count + 1));
                            ByteTable.Add(item, (ByteTable.Count + 1));
                            WritingTable.Add(bytes, (WritingTable.Count + 1));
                        }
                    }
                }
            }
        }

        private void WriteDictionary(string WritingPath, string originalName)
        {
            var separator = '|';
            var writing = new byte[BufferLenght];

            if (CharacterTable.Keys.Contains("|"))
            {
                separator = 'ÿ';
                if (CharacterTable.Keys.Contains("ÿ")) separator = 'ß';
            }

            using var originalNameWriter = new StreamWriter(WritingPath);
            originalNameWriter.WriteLine(originalName);
            originalNameWriter.Close();
            using var file = new FileStream(WritingPath, FileMode.OpenOrCreate);
            using var writer = new BinaryWriter(file);
            writer.Seek(0, SeekOrigin.End);
            writing = Encoding.UTF8.GetBytes(BitsNumber.ToString().ToArray());
            writer.Write(writing);
            writer.Write(Convert.ToByte(separator));

            foreach (KeyValuePair<byte, int> Values in ByteTable)
            {
                writer.Write(Values.Key);
                writing = Encoding.UTF8.GetBytes(Values.Value.ToString().ToArray());
                writer.Write(writing);
                writer.Write(Convert.ToByte(separator));
            }

            writer.Write(Convert.ToByte(separator));
        }
        
        private void CompleteDictionary(string ReadingPath, string WritingPath)
        {
            var writing = new List<byte>();
            var Road = "";
            var Buffer = new byte[BufferLenght];
            var Outcome = new List<int>();
            var LastCharacter = "";
            using var writer = new FileStream(WritingPath, FileMode.Append);
            using var file = new FileStream(ReadingPath, FileMode.Open);
            using var reader = new BinaryReader(file);

            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                Buffer = reader.ReadBytes(BufferLenght);

                foreach (var item in Buffer)
                {
                    var bytes = Convert.ToString(Convert.ToChar(item));

                    if (!WritingTable.ContainsKey(LastCharacter + bytes))
                    {
                        WritingTable.Add(LastCharacter + bytes, WritingTable.Count + 1);
                        var _bits = Convert.ToString(WritingTable[LastCharacter], 2);
                        var Completes = _bits.PadLeft(BitsNumber, '0');
                        Road += Completes;

                        if (Road.Length >= 8)
                        {
                            while (Road.Length > 8)
                            {
                                writing.Add((Convert.ToByte(Road.Substring(0, 8), 2)));
                                Road = Road.Remove(0, 8);
                            }
                        }

                        LastCharacter = bytes;
                    }
                    else LastCharacter += bytes;
                }

                writer.Write(writing.ToArray(), 0, writing.ToArray().Length);
                writing.Clear();
            }

            var bit = Convert.ToString(WritingTable[LastCharacter], 2);
            var Complete = bit.PadLeft(BitsNumber, '0');
            Road += Complete;
            List<byte> Write = new List<byte>();

            while (Road.Length > 8)
            {
                Write.Add(Convert.ToByte(Road.Substring(0, 8), 2));
                Road = Road.Remove(0, 8);
            }

            if (Road.Length != 0)
            {
                for (int j = 0; Road.Length < 8; j++)
                {
                    Road += "0";
                }

                Write.Add(Convert.ToByte(Road, 2));
            }

            writer.Write(Write.ToArray(), 0, Write.ToArray().Length);
        }

        public void SetCompressionsLZW(string fileName, string compressedFileName)
        {
            var OriginalFile = new FileInfo(fileName);
            var OriginalName = OriginalFile.Name;
            double OriginalWeight = OriginalFile.Length;
            var CompressedFile = new FileInfo(compressedFileName);
            var CompressedName = CompressedFile.Name;
            double CompressedWeight = CompressedFile.Length;
            double CompressionRatio = CompressedWeight / OriginalWeight;
            double CompressionFactor = OriginalWeight / CompressedWeight;
            double Reduction = 100 - (CompressionRatio * 100);
            using var Writer = File.AppendText(Path.GetFullPath("Compressions.txt"));
            Writer.WriteLine(OriginalName + ", " + CompressedName + ", " + CompressionRatio + ", " + CompressionFactor + ", " + Reduction + ", LZW.");
        }
    }
}