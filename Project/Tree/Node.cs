using System;
using System.IO;
using System.Collections.Generic;

namespace Project.Tree
{
    public class Node<T>
    {
        public List<int> Children = new List<int>();
        public List<T> Values = new List<T>();
        static readonly int Length = 300;
        public int Index;
        public int Parent;
        public int MaxKeys;

        public Node(int parent)
        {
            if (Parent == 0) MaxKeys = (4 * (TreeSingleton.Instance.Rank - 1)) / 3;
            else MaxKeys = TreeSingleton.Instance.Rank - 1;
            Parent = parent;
        }

        public static Node<T> StringToNode(int Position)
        {
            var ChildrenQuant = (4 * (TreeSingleton.Instance.Rank - 1) / 3) + 1;
            var CharactersQuant = 8 + (4 * ChildrenQuant) + (Length * (ChildrenQuant - 1));
            var buffer = new byte[CharactersQuant];

            using (var fs = new FileStream(TreeSingleton.Instance.FilePath, FileMode.OpenOrCreate))
            {
                fs.Seek((Position - 1) * CharactersQuant + 23, SeekOrigin.Begin);
                fs.Read(buffer, 0, CharactersQuant);
            }

            var nodeString = ConvertToString(buffer);
            var values = new List<string>();

            for (int i = 0; i < ChildrenQuant + 2; i++)
            {
                values.Add(nodeString.Substring(0, 4));
                nodeString = nodeString.Substring(4);
            }

            for (int i = 0; i < ChildrenQuant - 1; i++)
            {
                values.Add(nodeString.Substring(0, Length));
                nodeString = nodeString.Substring(Length);
            }

            var ResultNode = new Node<T>(Convert.ToInt32(values[1])) { Index = Convert.ToInt32(values[0]) };

            for (int i = 2; i < (2 + ChildrenQuant); i++)
                if (values[i].Trim() != "-") ResultNode.Children.Add(Convert.ToInt32(values[i]));

            for (int i = (2 + ChildrenQuant); i < (1 + (2 * ChildrenQuant)); i++)
                if (values[i].Trim() != "-") ResultNode.Values.Add((T)TreeSingleton.Instance.GetNode.DynamicInvoke(values[i]));

            return ResultNode;
        }

        static string ConvertToString(byte[] Line)
        {
            var Str = "";

            foreach (var Character in Line)
                Str += Convert.ToChar(Character);
            
            return Str;
        }

        static byte[] ConvertToArray(string Line)
        {
            var ByteList = new List<byte>();

            foreach (var Character in Line)
                ByteList.Add(Convert.ToByte(Character));
            
            return ByteList.ToArray();
        }

        public void WriteNode()
        {
            var children = "";
            var Data = "";
            var ChildrenQuant = (4 * (TreeSingleton.Instance.Rank - 1) / 3) + 1;

            foreach (var item in Children)
                children += item.ToString("0000;-0000");
            
            for (int i = Children.Count; i < ChildrenQuant; i++)
                children += string.Format("{0,-4}", "-");

            foreach (var item in Values)
                Data += Convert.ToString(TreeSingleton.Instance.GetString.DynamicInvoke(item));

            for (int i = Values.Count; i < (ChildrenQuant - 1); i++)
                Data += string.Format("{0,-300}", "-");

            var NodeChar = ($"{Index:0000;-0000}{Parent:0000;-0000}{children}{Data}");
            var CharactersQuant = 8 + (4 * ChildrenQuant) + (Length * (ChildrenQuant - 1));
            using var fs = new FileStream(TreeSingleton.Instance.FilePath, FileMode.OpenOrCreate);
            fs.Seek((Index - 1) * CharactersQuant + 23, SeekOrigin.Begin);
            fs.Write(ConvertToArray(NodeChar), 0, CharactersQuant);
        }
    }
}