using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Project.Tree
{
    public class BStarTree<T> where T : IComparable
    {
        static List<T> RoadList;
        
        public static int CreateId()
        {
            var buffer = new byte[14];
            var aux = 0;

            using (var fileStream = new FileStream(TreeSingleton.Instance.FilePath, FileMode.OpenOrCreate))
            {
                fileStream.Read(buffer, 0, 8);
                aux = Convert.ToInt32(Encoding.UTF8.GetString(buffer)) + 1;
                fileStream.Position = 0;
                fileStream.Write(Encoding.UTF8.GetBytes(aux.ToString("00000000;-00000000").ToCharArray()), 0, 8);
            }

            return aux;
        }

        static int[] WriteMetadata(int[] meta = null)
        {
            var buffer = new byte[15];

            using (var fileStream = new FileStream(TreeSingleton.Instance.FilePath, FileMode.OpenOrCreate))
            {
                if (meta == null)
                {
                    fileStream.Seek(8, SeekOrigin.Begin);
                    fileStream.Read(buffer, 0, 15);
                    var values = Encoding.UTF8.GetString(buffer).Split('|');
                    return new int[3] { Convert.ToInt32(values[0]), Convert.ToInt32(values[1]), Convert.ToInt32(values[2]) };
                }

                var Line = "";

                foreach (var item in meta)
                    Line = $"{Line}{item:0000;-0000}|";
                
                fileStream.Seek(8, SeekOrigin.Begin);
                fileStream.Write(Encoding.UTF8.GetBytes(Line.ToCharArray()), 0, 15);
            }

            return null;
        }

        public static void StartTree(string FilePath, Delegate node, Delegate str)
        {
            var Rank = 7;
            TreeSingleton.Instance.FilePath = FilePath;

            if (!File.Exists(TreeSingleton.Instance.FilePath))
            {
                var Text = $"00000000{Rank:0000;-0000}|0000|0001|";
                File.WriteAllText(TreeSingleton.Instance.FilePath, Text);
            }

            TreeSingleton.Instance.GetNode = node;
            TreeSingleton.Instance.GetString = str;
        }

        public static void InsertTree(T info)
        {
            var Metadata = WriteMetadata();
            TreeSingleton.Instance.Rank = Metadata[0];

            if (Metadata[1] == 0)
            {
                Metadata[1]++;
                Metadata[2]++;
                var Node = new Node<T>(0) { Index = 1, Values = new List<T> { info }, Children = new List<int>() };
                Node.WriteNode();
                WriteMetadata(Metadata);
            }
            else
            {
                var Carry = false;
                var CarryChild = false;
                var IsFirst = true;
                var Child = 0;
                var ChildPosition = 0;
                Insert(Metadata[1], ref info, ref Child, ref ChildPosition, ref Carry, ref CarryChild, ref IsFirst);
            }
        }

        static void Insert(int ActualIndex, ref T info, ref int child, ref int ChildPosition, ref bool HasCarry, ref bool HasChild, ref bool First)
        {
            var Metadata = WriteMetadata();
            var Actual = Node<T>.StringToNode(ActualIndex);
            var pos = 0;

            if (Actual.Children.Count == 0)
            {
                while (pos < Actual.Values.Count && Actual.Values[pos].CompareTo(info) == -1)
                    pos++;
                
                Actual.Values.Insert(pos, info);
            }
            else
            {
                while (pos < Actual.Values.Count && Actual.Values[pos].CompareTo(info) == -1)
                    pos++;
                
                Insert(Actual.Children[pos], ref info, ref child, ref ChildPosition, ref HasCarry, ref HasChild, ref First);
            }

            if (HasCarry)
            {
                pos = 0;
                Actual = Node<T>.StringToNode(Actual.Index);

                while (pos < Actual.Values.Count && Actual.Values[pos].CompareTo(info) == -1)
                    pos++;
                
                Actual.Values.Insert(pos, info);

                if (HasChild)
                {
                    Actual.Children.Insert(ChildPosition, child);
                    HasChild = false;
                }

                HasCarry = false;
            }
            if (Actual.Values.Count == Actual.MaxKeys + 1)
            {
                if (Actual.Children.Count == 0 && Actual.Parent != 0)
                {
                    var Parent = Node<T>.StringToNode(Actual.Parent);
                    var ActIndex = Parent.Children.IndexOf(Actual.Index);
                    var ForeignIndex = SearchRotate(Parent, ActIndex);
                    T Tmp = Actual.Values[0];
                    var hasCarry = false;

                    if (ForeignIndex < ActIndex)
                    {
                        var FactIndex = ActIndex == 0 ? 0 : ActIndex - 1;

                        for (int i = ActIndex; i >= ForeignIndex; i--)
                        {
                            if (Parent.Children[i] != Actual.Index) Actual = Node<T>.StringToNode(Parent.Children[i]);
                            
                            if (HasCarry)
                            {
                                Parent.Values.Insert(FactIndex, Tmp);
                                Tmp = Parent.Values[FactIndex + 1];
                                Parent.Values.RemoveAt(FactIndex + 1);
                                Parent.WriteNode();
                                Actual.Values.Add(Tmp);
                                Tmp = Actual.Values[0];

                                if (i != ForeignIndex) Actual.Values.RemoveAt(0);

                                FactIndex--;
                            }
                            else
                            {
                                Tmp = Actual.Values[0];
                                Actual.Values.RemoveAt(0);
                            }

                            Actual.WriteNode();
                            hasCarry = true;
                        }
                    }
                    else if (ForeignIndex > ActIndex)
                    {
                        var FactIndex = ActIndex == Parent.Children.Count() - 1 ? ActIndex - 1 : ActIndex;
                        
                        for (int i = ActIndex; i <= ForeignIndex; i++)
                        {
                            if (Parent.Children[i] != Actual.Index) Actual = Node<T>.StringToNode(Parent.Children[i]);
                            
                            if (hasCarry)
                            {
                                Parent.Values.Insert(FactIndex, Tmp);
                                Tmp = Parent.Values[FactIndex + 1];
                                Parent.Values.RemoveAt(FactIndex + 1);
                                Parent.WriteNode();
                                Actual.Values.Insert(0, Tmp);
                                Tmp = Actual.Values[^1];

                                if (i != ForeignIndex) Actual.Values.RemoveAt(Actual.Values.Count - 1);

                                FactIndex++;
                            }
                            else
                            {
                                Tmp = Actual.Values[^1];
                                Actual.Values.RemoveAt(Actual.Values.Count - 1);
                            }

                            Actual.WriteNode();
                            hasCarry = true;
                        }
                    }
                    else
                    {
                        var FactIndex = ActIndex == 0 ? 0 : ActIndex - 1;
                        ForeignIndex = ActIndex - 1 >= 0 ? ActIndex - 1 : ActIndex + 1;
                        var Sibling = Node<T>.StringToNode(Parent.Children[ForeignIndex]);
                        var TmpList = new List<T>();
                        var tmp = new Node<T>(Parent.Index) { Index = Metadata[2], Values = new List<T>(), Children = new List<int>() };

                        if (ActIndex - 1 == ForeignIndex)
                        {
                            foreach (var item in Sibling.Values)
                                TmpList.Add(item);
                            
                            TmpList.Add(Parent.Values[FactIndex]);

                            foreach (var item in Actual.Values)
                                TmpList.Add(item);
                            
                            var DivideQuant = (TmpList.Count - 2) / 3;
                            tmp.Values.AddRange(TmpList.GetRange(0, DivideQuant));
                            TmpList.RemoveRange(0, DivideQuant);
                            Parent.Values.RemoveAt(FactIndex);
                            Parent.Values.Insert(FactIndex, TmpList[0]);
                            TmpList.RemoveAt(0);
                            Sibling.Values.Clear();
                            Sibling.Values.AddRange(TmpList.GetRange(0, DivideQuant));
                            TmpList.RemoveRange(0, DivideQuant);
                            Parent.Values.Insert(FactIndex + 1, TmpList[0]);
                            TmpList.RemoveAt(0);
                            Actual.Values.Clear();
                            Actual.Values.AddRange(TmpList.GetRange(0, DivideQuant));
                            ForeignIndex = ActIndex - 1 >= 0 ? ActIndex - 1 : ActIndex + 2;
                            Parent.Children.Insert(ForeignIndex, tmp.Index);
                        }
                        else
                        {
                            foreach (var item in Actual.Values)
                                TmpList.Add(item);
                            
                            TmpList.Add(Parent.Values[FactIndex]);

                            foreach (var item in Sibling.Values)
                                TmpList.Add(item);
                            
                            var DividedQuant = (TmpList.Count - 2) / 3;
                            Actual.Values.Clear();
                            Actual.Values.AddRange(TmpList.GetRange(0, DividedQuant));
                            TmpList.RemoveRange(0, DividedQuant);
                            Parent.Values.RemoveAt(FactIndex);
                            Parent.Values.Insert(FactIndex, TmpList[0]);
                            TmpList.RemoveAt(0);
                            Sibling.Values.Clear();
                            Sibling.Values.AddRange(TmpList.GetRange(0, DividedQuant));
                            TmpList.RemoveRange(0, DividedQuant);
                            Parent.Values.Insert(FactIndex + 1, TmpList[0]);
                            TmpList.RemoveAt(0);
                            tmp.Values.AddRange(TmpList.GetRange(0, DividedQuant));
                            Parent.Children.Insert(ForeignIndex + 1, tmp.Index);
                        }

                        if (Parent.Values.Count > Parent.MaxKeys)
                        {
                            info = Parent.Values[0];
                            Parent.Values.RemoveAt(0);
                            ChildPosition = Parent.Children.IndexOf(tmp.Index);
                            Parent.Children.RemoveAt(ChildPosition);
                            child = tmp.Index;
                            HasCarry = true;
                            HasChild = true;
                        }

                        tmp.WriteNode();
                        Parent.WriteNode();
                        Sibling.WriteNode();
                        Metadata[2]++;
                        WriteMetadata(Metadata);
                    }
                }
                else
                {
                    Metadata = WriteMetadata();
                    var MiddlePos = Actual.Values.Count % 2 == 0 ? (Actual.Values.Count - 1) / 2 : Actual.Values.Count / 2;
                    var SiblingParent = Actual.Parent == 0 ? Metadata[2] + 1 : Actual.Parent;
                    var Sibling = new Node<T>(SiblingParent) { Index = Metadata[2], Values = Actual.Values.GetRange(0, MiddlePos) };
                    Metadata[2]++;

                    if (Actual.Children.Count != 0)
                    {
                        Sibling.Children = Actual.Children.GetRange(0, MiddlePos + 1);
                        Actual.Children.RemoveRange(0, MiddlePos + 1);

                        foreach (var childIndex in Sibling.Children)
                        {
                            var Child = Node<T>.StringToNode(childIndex);
                            Child.Parent = Sibling.Index;
                            Child.WriteNode();
                        }
                    }

                    if (Actual.Parent == 0)
                    {
                        var Parent = new Node<T>(0) { Values = new List<T> { Actual.Values[MiddlePos] }, Children = new List<int> { Sibling.Index, Actual.Index }, Index = Metadata[2] };
                        Metadata[1] = Metadata[2];
                        Metadata[2]++;
                        Parent.WriteNode();
                        Sibling.Parent = Parent.Index;
                        Actual.Parent = Parent.Index;
                    }
                    else
                    {
                        var Parent = Node<T>.StringToNode(Actual.Parent);
                        info = Actual.Values[MiddlePos];
                        pos = 0;

                        while (pos < Parent.Values.Count && Parent.Values[pos].CompareTo(info) == -1)
                            pos++;
                        
                        Parent.Values.Insert(pos, info);
                        Parent.Children.Insert(Parent.Children.IndexOf(Actual.Index), Sibling.Index);

                        if (Parent.Values.Count > Parent.MaxKeys)
                        {
                            info = Parent.Values[0];
                            Parent.Values.RemoveAt(0);
                            ChildPosition = Parent.Children.IndexOf(Sibling.Index);
                            Parent.Children.RemoveAt(ChildPosition);
                            child = Sibling.Index;
                            HasCarry = true;
                            HasChild = true;
                        }

                        Parent.WriteNode();
                    }

                    Actual.Values.RemoveRange(0, MiddlePos + 1);
                    Actual.WriteNode();
                    Sibling.WriteNode();
                    WriteMetadata(Metadata);
                }
            }
            if (First)
            {
                Actual.WriteNode();
                First = false;
            }
        }

        static int SearchRotate(Node<T> Parent, int IndexList)
        {
            var tmp = new Node<T>(1);

            if (IndexList - 1 >= 0)
            {
                tmp = Node<T>.StringToNode(Parent.Children[IndexList - 1]);

                if (tmp.Values.Count < tmp.MaxKeys) return IndexList - 1;
            }
            if (IndexList + 1 < Parent.Children.Count)
            {
                tmp = Node<T>.StringToNode(Parent.Children[IndexList + 1]);

                if (tmp.Values.Count < tmp.MaxKeys) return IndexList + 1;
            }
            if (IndexList - 2 >= 0)
            {
                for (int i = IndexList - 2; i >= 0; i--)
                {
                    tmp = Node<T>.StringToNode(Parent.Children[i]);

                    if (tmp.Values.Count < tmp.MaxKeys) return i;
                }
            }
            if (IndexList + 2 < Parent.Children.Count)
            {
                for (int i = IndexList + 2; i < Parent.Children.Count; i++)
                {
                    tmp = Node<T>.StringToNode(Parent.Children[i]);
                    
                    if (tmp.Values.Count < tmp.MaxKeys) return i;
                }
            }

            return IndexList;
        }
        
        public static List<T> Road(T info, int type = 0)
        {
            RoadList = new List<T>();
            var Metadata = WriteMetadata();
            TreeSingleton.Instance.Rank = Metadata[0];

            if (Metadata[1] != 0)
            {
                var Root = Node<T>.StringToNode(Metadata[1]);
                var Continue = true;

                if (type == 0) InOrder(Root);
                if (type == 1) Search(Root, info, ref Continue);
            }

            return RoadList;
        }

        static void InOrder(Node<T> Actual)
        {
            if (Actual.Children.Count == 0)
            {
                foreach (var Fact in Actual.Values)
                    RoadList.Add(Fact);
            }
            else
            {
                var FactPos = 1;

                foreach (var child in Actual.Children)
                {
                    InOrder(Node<T>.StringToNode(child));

                    if (FactPos < Actual.Children.Count)
                    {
                        RoadList.Add(Actual.Values[FactPos - 1]);
                        FactPos++;
                    }
                }
            }
        }

        static void Search(Node<T> Actual, T info, ref bool Continue)
        {
            var pos = 0;

            while (Continue && pos < Actual.Values.Count && (Actual.Values[pos].CompareTo(info) == -1 || Actual.Values[pos].CompareTo(info) == 0))
            {
                if (Actual.Values[pos].CompareTo(info) == 0)
                {
                    Continue = false;
                    RoadList.Add(Actual.Values[pos]);
                }

                pos++;
            }

            if (Continue && Actual.Children.Count != 0) Search(Node<T>.StringToNode(Actual.Children[pos]), info, ref Continue);
        }

        public static void Modify(T info, string[] newArray, Delegate mod)
        {
            var Metadata = WriteMetadata();
            TreeSingleton.Instance.Rank = Metadata[0];

            if (Metadata[1] != 0)
            {
                var Root = Node<T>.StringToNode(Metadata[1]);
                var Continue = true;
                Modify(Root, info, newArray, mod, ref Continue);
            }
        }

        static void Modify(Node<T> Actual, T info, string[] newArray, Delegate Mod, ref bool Continue)
        {
            var pos = 0;

            while (Continue && pos < Actual.Values.Count && (Actual.Values[pos].CompareTo(info) == -1 || Actual.Values[pos].CompareTo(info) == 0))
            {
                if (Actual.Values[pos].CompareTo(info) == 0)
                {
                    Continue = false;
                    Mod.DynamicInvoke(Actual.Values[pos], newArray);
                    Actual.WriteNode();
                }

                pos++;
            }
            if (Continue && Actual.Children.Count != 0) Modify(Node<T>.StringToNode(Actual.Children[pos]), info, newArray, Mod, ref Continue);
        }

        public static void ModifyTree(int newKey)
        {
            var Metadata = WriteMetadata();
            TreeSingleton.Instance.Rank = Metadata[0];
            
            if (Metadata[1] != 0)
            {
                var Root = Node<T>.StringToNode(Metadata[1]);
                ModifyRoad(Root, newKey);
            }
        }

        static void ModifyRoad(Node<T> Actual, int newKey)
        {
            if (Actual.Children.Count == 0)
            {
                var aux = new Node<T>(Actual.Parent) { Index = Actual.Index, Children = Actual.Children };
                
                foreach (var dato in Actual.Values)
                aux.Values.Add(dato);

                var Last = TreeSingleton.Instance.Key;
                TreeSingleton.Instance.Key = newKey;
                aux.WriteNode();
                TreeSingleton.Instance.Key = Last;
            }
            else
            {
                var DataPosition = 1;
                var aux = new Node<T>(Actual.Parent) { Index = Actual.Index, Children = Actual.Children };

                foreach (var child in Actual.Children)
                {
                    ModifyRoad(Node<T>.StringToNode(child), newKey);

                    if (DataPosition < Actual.Children.Count)
                    {
                        aux.Values.Add(Actual.Values[DataPosition - 1]);
                        DataPosition++;
                    }
                }

                var Last = TreeSingleton.Instance.Key;
                TreeSingleton.Instance.Key = newKey;
                aux.WriteNode();
                TreeSingleton.Instance.Key = Last;
            }
        }
    }
}