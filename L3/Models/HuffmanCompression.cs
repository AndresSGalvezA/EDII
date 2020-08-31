using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace L3.Models
{
    public class HuffmanCompression
    {
        private static decimal TotalData;
        private const int BufferLenght = 500;
        private static char Separator = new char();
        private static HuffmanNode Root { get; set; }
        private static Dictionary<byte, int> FrequencyTable { get; set; }
        private static Dictionary<byte, string> CharacterTable { get; set; }

        public void Compression(string ReadingPath, string WritingPath, string originalName)
        {
            FrequencyTable = new Dictionary<byte, int>();
            CharacterTable = new Dictionary<byte, string>();
            HuffmanTree(ReadingPath);
            GetPrefixCodes();
            ValueFrequencyWrite(WritingPath, originalName);
            Road(ReadingPath, WritingPath);
        }

        private static void HuffmanTree(string path)
        {
            using (var File = new FileStream(path, FileMode.Open))
            {
                var Buffer = new byte[BufferLenght];
                using var Reader = new BinaryReader(File);
                TotalData = Reader.BaseStream.Length;

                while (Reader.BaseStream.Position != Reader.BaseStream.Length)
                {
                    Buffer = Reader.ReadBytes(BufferLenght);

                    foreach (var item in Buffer)
                    {
                        if (FrequencyTable.Keys.Contains(item)) FrequencyTable[(item)]++;
                        else FrequencyTable.Add(item, 1);
                    }
                }
            }

            List<HuffmanNode> FrequencyList = new List<HuffmanNode>();

            foreach (KeyValuePair<byte, int> Nodes in FrequencyTable)
            {
                FrequencyList.Add(new HuffmanNode(Nodes.Key, Convert.ToDecimal(Nodes.Value) / TotalData));
            }
            
            while (FrequencyList.Count > 1)
            {
                if (FrequencyList.Count == 1) break;
                else
                {
                    FrequencyList = FrequencyList.OrderBy(x => x.Probability).ToList();
                    HuffmanNode Union = LinkNodes(FrequencyList[1], FrequencyList[0]);
                    FrequencyList.RemoveRange(0, 2);
                    FrequencyList.Add(Union);
                }
            }

            Root = FrequencyList[0];
        }

        private static HuffmanNode LinkNodes(HuffmanNode Major, HuffmanNode Minor)
        {
            var Parent = new HuffmanNode(Major.Probability + Minor.Probability);
            Parent.LeftNode = Major;
            Parent.RightNode = Minor;
            return Parent;
        }

        private static void Road(string ReadingPath, string WritingPath)
        {
            string Road = "";
            using var writer = new FileStream(WritingPath, FileMode.Append);
            using var File = new FileStream(ReadingPath, FileMode.Open);
            var buffer = new byte[BufferLenght];
            var Bytes = new List<byte>();
            using var reader = new BinaryReader(File);

            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                buffer = reader.ReadBytes(BufferLenght);
                
                foreach (var item in buffer)
                {
                    Road += CharacterTable[item];

                    if (Road.Length >= 8)
                    {
                        while (Road.Length > 8)
                        {
                            Bytes.Add(Convert.ToByte(Road.Substring(0, 8), 2));
                            Road = Road.Remove(0, 8);
                        }
                    }
                }

                writer.Write(Bytes.ToArray(), 0, Bytes.ToArray().Length);
                Bytes.Clear();
            }

            for (int i = Road.Length; i < 8; i++)
            {
                Road += "0";
            }

            Bytes.Add(Convert.ToByte(Road, 2));
            writer.Write(Bytes.ToArray(), 0, Bytes.ToArray().Length);
        }

        private static void ValueFrequencyWrite(string path, string originalName)
        {
            var Writing = new byte[BufferLenght];
            Separator = '|';

            if (CharacterTable.Keys.Contains(Convert.ToByte('|')))
            {
                Separator = 'ÿ';
                if (CharacterTable.Keys.Contains(Convert.ToByte('ÿ'))) Separator = 'ß';
            }

            using var originalNameWriter = new StreamWriter(path);
            originalNameWriter.WriteLine(originalName);
            originalNameWriter.Close();
            using var file = new FileStream(path, FileMode.OpenOrCreate);
            using var Writer = new BinaryWriter(file);
            Writer.Seek(0, SeekOrigin.End);
            Writing = Encoding.UTF8.GetBytes(TotalData.ToString().ToArray());
            Writer.Write(Writing);
            Writer.Write(Convert.ToByte(Separator));
            
            foreach (KeyValuePair<byte, int> Values in FrequencyTable)
            {
                Writer.Write(Values.Key);
                Writing = Encoding.UTF8.GetBytes(Values.Value.ToString().ToArray());
                Writer.Write(Writing);
                Writer.Write(Convert.ToByte(Separator));
            }
            
            Writer.Write(Convert.ToByte(Separator));
        }

        private static void PrefixCodes(HuffmanNode Node, string Route)
        {
            if (Node.IsLeaf()) CharacterTable.Add(Node.Fact, Route);
            else
            {
                if (Node.LeftNode != null) PrefixCodes(Node.LeftNode, Route + "0");
                if (Node.RightNode != null) PrefixCodes(Node.RightNode, Route + "1");
            }
        }

        private static void GetPrefixCodes()
        {
            if (Root.IsLeaf()) CharacterTable.Add(Root.Fact, "1");
            else PrefixCodes(Root, "");
        }

        public void SetCompressions(string fileName, string compressedFileName)
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
            Writer.WriteLine(OriginalName + ", " + CompressedName + ", " + CompressionRatio + ", " + CompressionFactor + ", " + Reduction + ", Huffman.");
        }

        public Stack<string> GetCompressions()
        {
            var CompressionsStack = new Stack<string>();
            
            using (var Reader = new StreamReader(Path.GetFullPath("Compressions.txt")))
            {
                while (!Reader.EndOfStream)
                {
                    CompressionsStack.Push(Reader.ReadLine());
                }
            }

            return CompressionsStack;
        }
    }
}