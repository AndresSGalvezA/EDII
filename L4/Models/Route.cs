using System;
using System.Linq;
using System.IO;
using System.Text;

namespace L4.Models
{
    public class Route
    {
        private char[,] CharsArray;
        private char[] buffer;
        private const int BufferLength = 1500;

        private void WriteCipherArray(string WritingPath, char[,] CharsArray)
        {
            var buffer = new char[BufferLength];
            var counter = 0;
            using var Writer = new FileStream(WritingPath, FileMode.Append);

            for (int row = 0; row < CharsArray.GetLength(0); row++)
            {
                for (int column = 0; column < CharsArray.GetLength(1); column++)
                {
                    buffer[counter] = CharsArray[row, column];
                    counter++;
                }
            }

            Writer.Write(Encoding.UTF8.GetBytes(buffer), 0, Encoding.UTF8.GetBytes(buffer).Length);
            buffer = new char[BufferLength];
            counter = 0;
        }

        private void WriteDecipherArray(string RouteType, string WritingPath, char[,] CharsArray)
        {
            var buffer = new char[BufferLength];
            var counter = 0;
            using var Writer = new FileStream(WritingPath, FileMode.Append);

            if (RouteType == "vertical")
            {
                for (int column = 0; column < CharsArray.GetLength(1); column++)
                {
                    for (int row = 0; row < CharsArray.GetLength(0); row++)
                    {
                        buffer[counter] = CharsArray[row, column];
                        counter++;
                    }
                }
            }
            else
            {
                var Level = 0;
                var ThisRow = 0;
                var ThisColumn = 0;

                //This code doesn´t work with Rows = 8 and Columns = 6 and multiples (16, 12 - 24, 18 - etc.).
                while (true)
                {
                    //Right road.
                    while (ThisColumn < CharsArray.GetLength(1) - Level)
                    {
                        buffer[counter] = CharsArray[ThisRow, ThisColumn];
                        counter++;
                        ThisColumn++;
                    }

                    ThisColumn--;

                    if (ThisRow < CharsArray.GetLength(0) - Level - 1) ThisRow++;
                    else break;

                    //Down road.
                    while (ThisRow < CharsArray.GetLength(0) - Level)
                    {
                        buffer[counter] = CharsArray[ThisRow, ThisColumn];
                        counter++;
                        ThisRow++;
                    }

                    ThisRow--;

                    if (ThisColumn > Level) ThisColumn--;
                    else break;

                    //Left road.
                    while (ThisColumn >= Level)
                    {
                        buffer[counter] = CharsArray[ThisRow, ThisColumn];
                        counter++;
                        ThisColumn--;
                    }

                    ThisColumn++;

                    if (ThisRow > Level + 1) ThisRow--;
                    else break;

                    //Up road.
                    while (ThisRow >= Level + 1)
                    {
                        buffer[counter] = CharsArray[ThisRow, ThisColumn];
                        counter++;
                        ThisRow--;
                    }

                    ThisRow++;

                    if (ThisColumn < CharsArray.GetLength(1) - Level) ThisColumn++;
                    else break;

                    Level++;
                }
            }

            Writer.Write(Encoding.UTF8.GetBytes(buffer), 0, Encoding.UTF8.GetBytes(buffer).Length);
            buffer = new char[BufferLength];
            counter = 0;
        }

        public void Cipher(string RouteType, string ReadingPath, string WritingPath, int Rows, int Columns)
        {
            buffer = new char[BufferLength];
            CharsArray = new char[Rows, Columns];
            var charsCounter = 0;

            using (var file = new FileStream(ReadingPath, FileMode.Open))
            {
                using var Reader = new BinaryReader(file);

                while (Reader.BaseStream.Position != Reader.BaseStream.Length)
                {
                    buffer = Reader.ReadChars(BufferLength);

                    if (RouteType == "vertical")
                    {
                        for (int column = 0; column < Columns; column++)
                        {
                            for (int row = 0; row < Rows; row++)
                            {
                                if (charsCounter <= Reader.BaseStream.Length)
                                {
                                    CharsArray[row, column] = buffer[charsCounter];
                                    charsCounter++;
                                }
                                else CharsArray[row, column] = 'ß';
                            }
                        }

                        WriteCipherArray(WritingPath, CharsArray);
                        charsCounter = 0;
                    }
                    else
                    {
                        var Level = 0;
                        var ThisRow = 0;
                        var ThisColumn = 0;

                        //This code doesn´t work with Rows = 8 and Columns = 6 and multiples (16, 12 - 24, 18 - etc.).
                        while (true)
                        {
                            //Right road.
                            while (ThisColumn < Columns - Level)
                            {
                                if (charsCounter <= Reader.BaseStream.Length) CharsArray[ThisRow, ThisColumn] = buffer[charsCounter];
                                else CharsArray[ThisRow, ThisColumn] = 'ß';

                                charsCounter++;
                                ThisColumn++;
                            }

                            ThisColumn--;

                            if (ThisRow < Rows - Level - 1) ThisRow++;
                            else break;

                            //Down road.
                            while (ThisRow < Rows - Level)
                            {
                                if (charsCounter <= Reader.BaseStream.Length) CharsArray[ThisRow, ThisColumn] = buffer[charsCounter];
                                else CharsArray[ThisRow, ThisColumn] = 'ß';

                                charsCounter++;
                                ThisRow++;
                            }

                            ThisRow--;

                            if (ThisColumn > Level) ThisColumn--;
                            else break;

                            //Left road.
                            while (ThisColumn >= Level)
                            {
                                if (charsCounter <= Reader.BaseStream.Length) CharsArray[ThisRow, ThisColumn] = buffer[charsCounter];
                                else CharsArray[ThisRow, ThisColumn] = 'ß';

                                charsCounter++;
                                ThisColumn--;
                            }

                            ThisColumn++;

                            if (ThisRow > Level + 1) ThisRow--;
                            else break;

                            //Up road.
                            while (ThisRow >= Level + 1)
                            {
                                if (charsCounter <= Reader.BaseStream.Length) CharsArray[ThisRow, ThisColumn] = buffer[charsCounter];
                                else CharsArray[ThisRow, ThisColumn] = 'ß';

                                charsCounter++;
                                ThisRow--;
                            }

                            ThisRow++;

                            if (ThisColumn < Columns - Level) ThisColumn++;
                            else break;

                            Level++;
                        }

                        WriteCipherArray(WritingPath, CharsArray);
                        charsCounter = 0;
                    }
                }
            }

            buffer = new char[BufferLength];
            charsCounter = 0;
        }

        public void Decipher(string RouteType, string ReadingPath, string WritingPath, int Rows, int Columns)
        {
            buffer = new char[BufferLength];
            CharsArray = new char[Rows, Columns];
            var charsCounter = 0;

            using (var file = new FileStream(ReadingPath, FileMode.Open))
            {
                using var Reader = new BinaryReader(file);

                while (Reader.BaseStream.Position != Reader.BaseStream.Length)
                {
                    buffer = Reader.ReadChars(BufferLength);
                    
                    for (int row = 0; row < Rows; row++)
                    {
                        for (int column = 0; column < Columns; column++)
                        {
                            if (charsCounter <= Reader.BaseStream.Length)
                            {
                                CharsArray[row, column] = buffer[charsCounter];
                                charsCounter++;
                            }
                            else CharsArray[row, column] = '$';
                        }
                    }

                    if (RouteType == "vertical") WriteDecipherArray("vertical", WritingPath, CharsArray);
                    else WriteDecipherArray("spiral", WritingPath, CharsArray);

                    charsCounter = 0;
                }
            }

            buffer = new char[BufferLength];
            charsCounter = 0;
        }
    }
}