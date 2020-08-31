using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace L3.Models
{
    public class LZWDecompression
    {
        private readonly Dictionary<int, string> CharacterTable = new Dictionary<int, string>();
        private const int BufferLenght = 750;
        int Total_Bits { get; set; }
        private static char Divide = new char();

        public void Decompression(string ReadingPath, string WritingPath)
        {
            LZWDictionary(ReadingPath);
            Writing(ReadingPath, WritingPath);
        }

        public void LZWDictionary(string path)
        {
            using var nameJumper = new StreamReader(path);
            var position = nameJumper.ReadLine().Length;
            nameJumper.Close();

            using (var File = new FileStream(path, FileMode.Open))
            {
                File.Position = position + 1;
                byte bit = new byte();
                int separator = 0;
                var buffer = new byte[BufferLenght];
                string Total_Data = "";
                string Frequency = "";
                string Character = "";
                int final = 0;
                using (var reader = new BinaryReader(File))
                {
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        buffer = reader.ReadBytes(BufferLenght);
                        foreach (var item in buffer)
                        {
                            var charr = Convert.ToChar(item);
                            if (separator == 0)
                            {
                                if (charr == '|' || charr == 'ÿ' || charr == 'ß')
                                {
                                    separator = 1;
                                    if (charr == '|') Divide = '|';
                                    else if (charr == 'ÿ') Divide = 'ÿ';
                                    else Divide = 'ß'; 
                                }
                                else Total_Data += charr.ToString();
                            }
                            else if (separator == 2) break;
                            else
                            {
                                if (final == 1 && charr == Divide)
                                {
                                    final = 2;
                                    separator = 2;
                                }
                                else final = 0;

                                if (Character == "") { Character = charr.ToString(); bit = item; }
                                else if (charr == Divide && final == 0)
                                {
                                    CharacterTable.Add(Convert.ToInt32(Frequency), Convert.ToChar(bit).ToString());
                                    Character = "";
                                    Frequency = "";
                                    final = 1;
                                }
                                else Frequency += charr.ToString(); 
                            }
                        }
                    }
                }

                Total_Bits = Convert.ToInt32(Total_Data);
            }

            var x = CharacterTable;
        }

        private void Writing(string ReadingPath, string WritingPath)
        {
            var buffer = new byte[BufferLenght];
            string previous = "";
            string current = "";
            string New = "";
            int initial = 0;
            string road = "";
            using var file = new FileStream(WritingPath, FileMode.OpenOrCreate);
            using var writer = new BinaryWriter(file);
            using var nameJumper = new StreamReader(ReadingPath);
            var position = nameJumper.ReadLine().Length;
            nameJumper.Close();
            using var File = new FileStream(ReadingPath, FileMode.Open);
            File.Position = position + 1;
            using var reader = new BinaryReader(File);

            while (reader.BaseStream.Position != reader.BaseStream.Length)
            {
                buffer = reader.ReadBytes(BufferLenght);

                foreach (var item in buffer)
                {

                    if (initial == 0 && Convert.ToChar(item) == Divide) { initial = 1; }
                    else if (initial == 1 && Convert.ToChar(item) == Divide) { initial = 2; }
                    else if (initial == 2)
                    {
                        var bits = Convert.ToString(item, 2);
                        var full = bits.PadLeft(8, '0');
                        road += full;

                        if (road.Length >= Total_Bits)
                        {
                            while (road.Length > Total_Bits)
                            {
                                var num = road.Substring(0, Total_Bits);
                                var convert = Convert.ToInt32(num, 2);
                                road = road.Remove(0, Total_Bits);

                                if (CharacterTable.ContainsKey(convert))
                                {
                                    previous = current;
                                    current = CharacterTable[convert];
                                    New = previous + current.Substring(0, 1);
                                    if (!CharacterTable.ContainsValue(New)) CharacterTable.Add(CharacterTable.Count + 1, New);
                                    writer.Write(Encoding.UTF8.GetBytes(current.ToArray()));
                                }
                                else
                                {
                                    previous = current;
                                    New = previous + current.Substring(0, 1);
                                    if (!CharacterTable.ContainsValue(New)) CharacterTable.Add(CharacterTable.Count + 1, New);
                                    if (convert != 0) current = CharacterTable[convert];
                                    writer.Write(Encoding.UTF8.GetBytes(current.ToArray()));
                                }
                            }
                        }
                    }
                    else initial = 0;
                }
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