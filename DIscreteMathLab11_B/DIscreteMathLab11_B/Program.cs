using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace DiscreteMathLab11_A
{
    class Program
    {
        static void Main(string[] args)
        {
            Solve();
        }

        static void Solve()
        {
            AVLSet<int> Set = new AVLSet<int>();
            StreamWriter sw = new StreamWriter("bst.out");
            foreach (string s in File.ReadAllLines("bst.in"))
            {
                string[] parts = s.Split(' ');
                switch (parts[0])
                {
                    case "insert":
                        {
                            Set.Insert(int.Parse(parts[1]));
                            break;
                        }
                    case "exists":
                        {
                            sw.WriteLine(Set.Contains(int.Parse(parts[1])) ? "true" : "false");
                            break;
                        }
                    case "delete":
                        {
                            Set.Delete(int.Parse(parts[1]));
                            break;
                        }
                    case "next":
                        {
                            sw.WriteLine(Set.AfterOutput(int.Parse(parts[1])));
                            break;
                        }
                    case "prev":
                        {
                            sw.WriteLine(Set.BeforeOutput(int.Parse(parts[1])));
                            break;
                        }
                }
            }
            sw.Close();
        }

        static void CreateTest()
        {
            StreamWriter sw = new StreamWriter("bst.in");
            Random r = new Random();
            for (int i = 0; i <= 10; i++)
            {
                sw.WriteLine("insert {0}", r.Next(0,10));
            }
            for (int i = 0; i <= 100; i++)
            {
                sw.WriteLine("next {0}", r.Next(0, 100));
            }
            for (int i = 0; i <= 100; i++)
            {
                sw.WriteLine("prev {0}", r.Next(0, 100));
            }
            for (int i = 0; i <= 100; i++)
            {
                sw.WriteLine("exists {0}", r.Next(0, 100));
            }
            for (int i = 0; i <= 100; i++)
            {
                sw.WriteLine("delete {0}", r.Next(0, 100));
            }
            sw.Close();
        }
    }

    class AVLSet<T> where T : IComparable
    {
        private Node<T> Root;
        public int Count { get; private set; }

        public void Insert(T Item)
        {
            Root = Insert(Root, Item);
        }

        Node<T> Insert(Node<T> Parent, T Item)
        {
            if (Parent == null) return new Node<T>(Item,null);
            if (Item.CompareTo(Parent.Value) < 0)
                Parent.Left = Insert(Parent.Left, Item);
            else if (Item.CompareTo(Parent.Value) > 0)
                Parent.Right = Insert(Parent.Right, Item);
            else
                return Parent;
            return Balance(Parent);
        }

        Node<T> Balance(Node<T> a)
        {
            a.CountHeight();
            if (a.HeightDiff() == 2)
            {
                if (a.Right.HeightDiff() < 0)
                    a.Right = RotateRight(a.Right);
                return RotateLeft(a);
            }
            if (a.HeightDiff() == -2)
            {
                if (a.Left.HeightDiff() > 0)
                    a.Left = RotateLeft(a.Left);
                return RotateRight(a);
            }
            return a;
        }

        Node<T> RotateLeft(Node<T> a)
        {
            Node<T> b = a.Right;
            a.Right = b.Left;
            b.Left = a;
            a.CountHeight();
            b.CountHeight();
            return b;
        }

        Node<T> RotateRight(Node<T> a)
        {
            Node<T> b = a.Left;
            a.Left = b.Right;
            b.Right = a;
            a.CountHeight();
            b.CountHeight();
            return b;
        }

        public void Delete(T Item)
        {
            Root = Remove(Root, Item);
        }

        private Node<T> Remove(Node<T> Subtree, T Item)
        {
            if (Subtree == null) return null;
            if (Item.CompareTo(Subtree.Value) < 0)
                Subtree.Left = Remove(Subtree.Left, Item);
            else if (Item.CompareTo(Subtree.Value) > 0)
                Subtree.Right = Remove(Subtree.Right, Item);
            else
            {
                Node<T> l = Subtree.Left;
                Node<T> r = Subtree.Right;
                if (r == null) return l;
                Node<T> minright = MinInSubtree(r);
                minright.Right = RemoveMin(r);
                if (minright.Right != null)
                minright.Right.Parent = minright;
                minright.Left = l;
                if (minright.Left != null)
                minright.Left.Parent = minright;
                minright.Parent = Subtree.Parent;
                return Balance(minright);
            }
            return Balance(Subtree);
        }

        private Node<T> Find(T Item, Node<T> From)
        {
            if (From != null)
            {
                if (Item.CompareTo(From.Value) < 0) return Find(Item, From.Left);
                if (Item.CompareTo(From.Value) > 0) return Find(Item, From.Right);
                if (Item.CompareTo(From.Value) == 0) return From;
            }
            return null;
        }

        private void ReplaceAsChild(Node<T> Child, Node<T> x)
        {
            if (x != null) x.Parent = Child.Parent;
            if (Child.IsRoot()) Root = x;
            else
            {
                if (Child.IsLeftChild())
                    Child.Parent.Left = x;
                else
                    Child.Parent.Right = x;
            }
        }

        private Node<T> MinInSubtree(Node<T> Subtree)
        {
            if (Subtree.Left == null) return Subtree;
            else
                return MinInSubtree(Subtree.Left);
        }

        private Node<T> RemoveMin(Node<T> Subtree)
        {
            if (Subtree.Left == null)
                return Subtree.Right;
            Subtree.Left = RemoveMin(Subtree.Left);
            return Balance(Subtree);
        }

        public bool Contains(T Item)
        {
            return Find(Item, Root) != null;
        }

        Node<T> After(T Item)
        {
            Node<T> place = Root;
            Node<T> min = Root;
            while (place != null)
            {
                if (place.Value.CompareTo(Item) > 0)
                {
                    if (place.Value.CompareTo(Item) == 1 && (min.Value.CompareTo(Item) <= 0 || min.Value.CompareTo(place.Value) == 1))
                        min = place;
                    place = place.Left;
                }
                else
                {
                    place = place.Right;
                }
            }
            return min != null && min.Value.CompareTo(Item) == 1 ? min : null;
        }
        public string AfterOutput(T x)
        {
            Node<T> a = After(x);
            if (a != null) return (a.Value.ToString());
            else
                return ("none");
        }

        Node<T> Before(T Item)
        {
            Node<T> place = Root;
            Node<T> max = Root;
            while (place != null)
            {
                if (place.Value.CompareTo(Item) < 0)
                {
                    if (place.Value.CompareTo(Item) == -1 && (max.Value.CompareTo(Item) >= 0 || max.Value.CompareTo(place.Value) == -1))
                        max = place;
                    place = place.Right;
                }
                else
                {
                    place = place.Left;
                }
            }
            return max != null && max.Value.CompareTo(Item) == -1 ? max : null;
        }
        public string BeforeOutput(T x)
        {
            Node<T> b = Before(x);
            if (b != null) return (b.Value.ToString());
            else
                return ("none");
        }

        private class Node<T>
        {
            public T Value;
            public Node<T> Parent, Left, Right;
            public int SubtreeHeight = 1;

            public Node(T Value, Node<T> Parent)
            {
                this.Value = Value;
                this.Parent = Parent;
            }

            public bool IsRoot()
            {
                return Parent == null;
            }

            public bool IsLeftChild()
            {
                return Parent != null && Parent.Left == this;
            }

            public Node<T> Next()
            {
                Node<T> pos = this;
                while (pos != null && pos.Right == null)
                    pos = pos.Parent;
                if (pos != null)
                {
                    pos = pos.Right;
                    while (pos.Left != null)
                        pos = pos.Left;
                    return pos;
                }
                return null;
            }

            public int HeightDiff()
            {
                return (Right != null ? Right.SubtreeHeight : 0) - (Left != null ? Left.SubtreeHeight : 0);
            }

            public void CountHeight()
            {
                SubtreeHeight = 1 +
                                Math.Max(Right != null ? Right.SubtreeHeight : 0, Left != null ? Left.SubtreeHeight : 0);
            }

        }
    }
}
