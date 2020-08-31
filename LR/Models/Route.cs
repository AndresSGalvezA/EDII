using System.IO;
using System.Text;

namespace LR.Models
{
    public class Route
    {
        private int BufferLength;

        private void WriteCipherArray(string WritingPath, char[,] CharsArray)
        {
            var buffer = new char[CharsArray.GetLength(0) * CharsArray.GetLength(1)];
            int counter = 0;
            using var Writer = new FileStream(WritingPath, FileMode.Append);

            for (int row = 0; row < CharsArray.GetLength(0); row++)
            {
                for (int column = 0; column < CharsArray.GetLength(1); column++)
                {
                    buffer[counter] = CharsArray[row, column];
                    counter++;
                }
            }

            Writer.Write(Encoding.UTF8.GetBytes(buffer));
        }

        private void WriteDecipherArray(string RouteType, string WritingPath, char[,] CharsArray)
        {
            var buffer = new char[CharsArray.GetLength(0) * CharsArray.GetLength(1)];
            var counter = 0;
            using var Writer = new FileStream(WritingPath, FileMode.Append);

            if (RouteType == "vertical")
            {
                for (int column = 0; column < CharsArray.GetLength(1); column++)
                {
                    for (int row = 0; row < CharsArray.GetLength(0); row++)
                    {
                        if (CharsArray[row, column] != 'ß') buffer[counter] = CharsArray[row, column];
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
                        if (CharsArray[ThisRow, ThisColumn] != 'ß') buffer[counter] = CharsArray[ThisRow, ThisColumn];
                        counter++;
                        ThisColumn++;
                    }

                    ThisColumn--;

                    if (ThisRow < CharsArray.GetLength(0) - Level - 1) ThisRow++;
                    else break;

                    //Down road.
                    while (ThisRow < CharsArray.GetLength(0) - Level)
                    {
                        if (CharsArray[ThisRow, ThisColumn] != 'ß') buffer[counter] = CharsArray[ThisRow, ThisColumn];
                        counter++;
                        ThisRow++;
                    }

                    ThisRow--;

                    if (ThisColumn > Level) ThisColumn--;
                    else break;

                    //Left road.
                    while (ThisColumn >= Level)
                    {
                        if (CharsArray[ThisRow, ThisColumn] != 'ß') buffer[counter] = CharsArray[ThisRow, ThisColumn];
                        counter++;
                        ThisColumn--;
                    }

                    ThisColumn++;

                    if (ThisRow > Level + 1) ThisRow--;
                    else break;

                    //Up road.
                    while (ThisRow >= Level + 1)
                    {
                        if (CharsArray[ThisRow, ThisColumn] != 'ß') buffer[counter] = CharsArray[ThisRow, ThisColumn];
                        counter++;
                        ThisRow--;
                    }

                    ThisRow++;

                    if (ThisColumn < CharsArray.GetLength(1) - Level) ThisColumn++;
                    else break;

                    Level++;
                }
            }

            Writer.Write(Encoding.UTF8.GetBytes(buffer));
        }

        public void Cipher(string RouteType, string ReadingPath, string WritingPath, int Rows, int Columns)
        {
            BufferLength = Rows * Columns;
            var buffer = new char[BufferLength];
            var CharsArray = new char[Rows, Columns];
            var charsCounter = 0;

            using var file = new FileStream(ReadingPath, FileMode.Open);
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
                            if (charsCounter < buffer.Length)
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
                            if (charsCounter < buffer.Length) CharsArray[ThisRow, ThisColumn] = buffer[charsCounter];
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
                            if (charsCounter < buffer.Length) CharsArray[ThisRow, ThisColumn] = buffer[charsCounter];
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
                            if (charsCounter < buffer.Length) CharsArray[ThisRow, ThisColumn] = buffer[charsCounter];
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
                            if (charsCounter < buffer.Length) CharsArray[ThisRow, ThisColumn] = buffer[charsCounter];
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

        public void Decipher(string RouteType, string ReadingPath, string WritingPath, int Rows, int Columns)
        {
            BufferLength = Rows * Columns;
            var buffer = new char[BufferLength];
            var CharsArray = new char[Rows, Columns];
            var charsCounter = 0;

            using var file = new FileStream(ReadingPath, FileMode.Open);
            using var Reader = new BinaryReader(file);

            while (Reader.BaseStream.Position != Reader.BaseStream.Length)
            {
                buffer = Reader.ReadChars(BufferLength);

                for (int row = 0; row < Rows; row++)
                {
                    for (int column = 0; column < Columns; column++)
                    {
                        if (charsCounter < buffer.Length)
                        {
                            CharsArray[row, column] = buffer[charsCounter];
                            charsCounter++;
                        }
                        else CharsArray[row, column] = 'ß';
                    }
                }

                if (RouteType == "vertical") WriteDecipherArray("vertical", WritingPath, CharsArray);
                else WriteDecipherArray("spiral", WritingPath, CharsArray);
                charsCounter = 0;
            }
        }
    }
}