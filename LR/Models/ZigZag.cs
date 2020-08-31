using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LR.Models
{
	public class ZigZag
	{
		readonly int bufferLenght = 200;
		List<char>[] Rows;

		private int GetN(int Key, int CharactersNumber)
		{
			return ((Key - 2) < 0) ? 0 : (CharactersNumber + 1 + (2 * (Key - 2))) / (2 * (Key - 1));
		}

		public void Cipher(string ReadingPath, string WritingPath, int Key)
		{
			var Fill = true;
			var GoingDown = true;
			var buffer = new char[bufferLenght];
			var CharactersNumber = 0;
			var SeparatorN = 0;
			var Row = 0;
			var Separator = '|';

			Rows = new List<char>[Key];

			for (int i = 0; i < Key; i++)
			{
				Rows[i] = new List<char>();
			}

			using (var file = new FileStream(ReadingPath, FileMode.Open))
			{
				using var reader = new BinaryReader(file);

				while (reader.BaseStream.Position != reader.BaseStream.Length)
				{
					buffer = reader.ReadChars(bufferLenght);

					foreach (var item in buffer)
					{
						CharactersNumber++;

						if (item == '|' && SeparatorN == 0)
						{
							Separator = 'ÿ';
							Separator++;
						}
						if (item == 'ÿ' && Separator == 1) Separator = 'ß';
					}
				}
			}


			using (var file = new FileStream(ReadingPath, FileMode.Open))
			{
				using var reader = new BinaryReader(file);

				while (reader.BaseStream.Position != reader.BaseStream.Length)
				{
					buffer = reader.ReadChars(bufferLenght);

					foreach (var item in buffer)
					{
						Rows[Row].Add(Convert.ToChar(item));

						if (Key != 1)
						{
							if (Row == 0) GoingDown = true;
							if (Row == (Key - 1)) GoingDown = false;
							if (GoingDown == true) Row++;
							else Row--;
						}
					}
				}
			}


			if (Key == 1) Fill = false;

			while (Fill)
			{
				if (GoingDown && Row == 1) Fill = false;
				else
				{
					Rows[Row].Add(Separator);
					if (Row == 0) GoingDown = true;
					if (Row == (Key - 1)) GoingDown = false;
					if (GoingDown == true) Row++;
					else Row--;
				}
			}

			var Writing = new byte[bufferLenght];

			using (var file = new FileStream(WritingPath, FileMode.Create))
			{
				using var writer = new BinaryWriter(file);
				writer.Write(Convert.ToChar(Separator));

				foreach (var item in Rows)
				{
					foreach (var Item in item)
					{
						writer.Write(Convert.ToChar(Item));
					}
				}
			}
		}

		public void Decipher(string ReadingPath, string WritingPath, int Key)
		{
			var buffer = new char[bufferLenght];
			var CharactersNumber = 0;
			var Separator = '|';
			var FirstCharacter = false;
			var _FirstCharacter = false;
			var GoingDown = true;
			var Row = 0;
			Rows = new List<char>[Key];

			for (int i = 0; i < Key; i++) Rows[i] = new List<char>();

			using (var file = new FileStream(ReadingPath, FileMode.Open))
			{
				using var reader = new BinaryReader(file);

				while (reader.BaseStream.Position != reader.BaseStream.Length)
				{
					buffer = reader.ReadChars(bufferLenght);

					foreach (var item in buffer)
					{
						if (FirstCharacter == false)
						{
							Separator = item;
							FirstCharacter = true;
						}
						else CharactersNumber++;
					}
				}
			}

			using (var file = new FileStream(ReadingPath, FileMode.Open))
			{
				using var reader = new BinaryReader(file);

				while (reader.BaseStream.Position != reader.BaseStream.Length)
				{
					buffer = reader.ReadChars(bufferLenght);

					foreach (var item in buffer)
					{
						if (_FirstCharacter == false) _FirstCharacter = true;
						else if (Key != 1)
						{
							if (Row == 0)
							{
								if (Rows[Row].Count() < GetN(Key, CharactersNumber)) Rows[Row].Add(item);
								else
								{
									Row++;
									Rows[Row].Add(item);
								}
							}
							else if (Row == (Key - 1)) Rows[Row].Add(item);
							else
							{
								if (Rows[Row].Count() < (2 * (GetN(Key, CharactersNumber) - 1))) Rows[Row].Add(item);
								else
								{
									Row++;
									Rows[Row].Add(item);
								}
							}
						}
						else Rows[Row].Add(item);
					}
				}
			}

			Row = 0;

			using (var file = new FileStream(WritingPath, FileMode.Create))
			{
				using var writer = new BinaryWriter(file);

				while (CharactersNumber != 0)
				{
					try
					{
						if (Rows[Row][0] == Separator) Rows[Row].RemoveRange(0, 1);
						else
						{
							writer.Write(Rows[Row][0]);
							Rows[Row].RemoveRange(0, 1);
						}
					}
					catch { }

					if (Key != 1)
					{
						if (Row == 0) GoingDown = true;
						if (Row == (Key - 1)) GoingDown = false;
						if (GoingDown == true) Row++;
						else Row--;
					}

					CharactersNumber--;
				}
			}
		}
	}
}