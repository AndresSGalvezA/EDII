using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace L3.Models
{
    public class HuffmanDecompression
    {
        public const int BufferLenght = 500;
        private static HuffmanNode Root { get; set; }
        private static Dictionary<string, byte> CharacterTable { get; set; }
        private static Dictionary<byte, int> FrequencyTable { get; set; }
        public static int DataSize { get; set; }
        public static decimal DataLenght;
        private static char Separator = new char();

        public void Decompression(string ReadingPath, string WritingPath)
        {
            FrequencyTable = new Dictionary<byte, int>();
            CharacterTable = new Dictionary<string, byte>();
            HuffmanTree(ReadingPath);
            GetPrefixCodes();
            Traversal(ReadingPath, WritingPath);
        }

        static void HuffmanTree(string path)
        {
            using var nameJumper = new StreamReader(path);
            var position = nameJumper.ReadLine().Length;
            nameJumper.Close();

            using (var File = new FileStream(path, FileMode.Open))
            {
                File.Position = position + 1;
                int separator1 = 0;
                var buffer = new byte[BufferLenght];
                string Data_Lenght1 = "";
                string frequency = "";
                string Datamount = "";
                int final = 0;
                byte bit = new byte();
                using (var reader = new BinaryReader(File))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        buffer = reader.ReadBytes(BufferLenght);
                        foreach (var item in buffer)
                        {

                            if (separator1 == 0)
                            {
                                if (Convert.ToChar(item) == '|' || Convert.ToChar(item) == 'ÿ' || Convert.ToChar(item) == 'ß')
                                {
                                    separator1 = 1;
                                    if (Convert.ToChar(item) == '|') Separator = '|';
                                    else if (Convert.ToChar(item) == 'ÿ') Separator = 'ÿ';
                                    else Separator = 'ß';
                                }
                                else Data_Lenght1 += Convert.ToChar(item).ToString();
                            }
                            else if (separator1 == 2) break;
                            else
                            {
                                if (final == 1 && Convert.ToChar(item) == Separator)
                                {
                                    final = 2;
                                    separator1 = 2;
                                }
                                else final = 0;

                                if (Datamount == "") { Datamount = Convert.ToChar(item).ToString(); bit = item; }
                                else if (Convert.ToChar(item) == Separator && final == 0)
                                {
                                    FrequencyTable.Add(bit, Convert.ToInt32(frequency));
                                    Datamount = "";
                                    frequency = "";
                                    final = 1;
                                }
                                else frequency += Convert.ToChar(item).ToString();
                            }

                        }
                    }
                }
                
                DataLenght = Convert.ToDecimal(Data_Lenght1);
            }

            List<HuffmanNode> FrequencyList = new List<HuffmanNode>();

            foreach (KeyValuePair<byte, int> Nodes in FrequencyTable)
            {
                FrequencyList.Add(new HuffmanNode(Nodes.Key, Convert.ToDecimal(Nodes.Value) / DataLenght));
            }

            FrequencyList = FrequencyList.OrderBy(x => x.Probability).ToList();

            while (FrequencyList.Count > 1)
            {
                FrequencyList = FrequencyList.OrderBy(x => x.Probability).ToList();
                HuffmanNode Link = LinkNodes(FrequencyList[1], FrequencyList[0]);
                FrequencyList.RemoveRange(0, 2);
                FrequencyList.Add(Link);
            }

            Root = FrequencyList[0];
        }

        private static void PrefixCodes(HuffmanNode Node, string traversal)
        {
            if (Node.IsLeaf()) { CharacterTable.Add(traversal, Node.Fact); return; }
            else
            {
                if (Node.LeftNode != null) PrefixCodes(Node.LeftNode, traversal + "0");
                if (Node.RightNode != null) PrefixCodes(Node.RightNode, traversal + "1");
            }
        }
        
        public static HuffmanNode LinkNodes(HuffmanNode Higher, HuffmanNode Lower)
        {
            HuffmanNode Parent = new HuffmanNode(Higher.Probability + Lower.Probability);
            Parent.LeftNode = Higher;
            Parent.RightNode = Lower;
            return Parent;
        }

        private static void GetPrefixCodes()
        {
            if (Root.IsLeaf()) CharacterTable.Add("1", Root.Fact);
            else PrefixCodes(Root, "");
        }

        private static void Traversal(string ReadingPath, string WritingPath)
        {
            int written = 0;
            int i = 0;
            string validation = "";
            int start = 0;
            string traversal = "";
            List<byte> bites = new List<byte>();
            using var file = new FileStream(WritingPath, FileMode.OpenOrCreate);
            using var writer = new BinaryWriter(file);
            using var nameJumper = new StreamReader(ReadingPath);
            var position = nameJumper.ReadLine().Length;
            nameJumper.Close();
            using var File = new FileStream(ReadingPath, FileMode.Open);
            File.Position = position + 1;
            var buffer = new byte[BufferLenght];
            using var reader = new BinaryReader(File);
            
            while (reader.BaseStream.Position != reader.BaseStream.Length && written < DataLenght)
            {
                buffer = reader.ReadBytes(BufferLenght);
                
                foreach (var item in buffer)
                {
                    written++;
                    if (start == 0 && Convert.ToChar(item) == Separator) start = 1;
                    else if (start == 1 && Convert.ToChar(item) == Separator) start = 2;
                    else if (start == 2)
                    {
                        var bits = Convert.ToString(item, 2);
                        var full = bits.PadLeft(8, '0');
                        traversal += full;
                        var comparation = traversal.ToCharArray();
                        i = 0;
                        
                        while (i < traversal.Length)
                        {
                            validation += comparation[i];
                            i++;
                            
                            if (CharacterTable.Keys.Contains(validation))
                            {
                                i = 0;
                                bites.Add(CharacterTable[validation]);
                                traversal = traversal.Remove(0, validation.Length);
                                comparation = traversal.ToCharArray();
                                validation = "";
                            }
                        }

                        validation = "";
                    }
                    else start = 0;
                }

                writer.Write(bites.ToArray());
                bites.Clear();
            }
        }

        public string GetOriginalName(string ReadingPath)
        {
            using var getName = new StreamReader(ReadingPath);
            var originalName = getName.ReadLine();
            getName.Close();
            return originalName;
        }
    }
}