using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace DiscreteMathLab11_C_True
{
    class Program
    {
        static void Main(string[] args)
        {
            SplayTreeImpicit<int> t = new SplayTreeImpicit<int>();
            StreamReader sr = new StreamReader("movetofront.in");
            StreamWriter sw = new StreamWriter("movetofront.out");
            string[] parts = sr.ReadLine().Split(' ');
            int n = int.Parse(parts[0]),
                m = int.Parse(parts[1]);
            for (int i = 1; i <= n; i++)
                t.Add(i);
            for (int i = 0; i < m; i++)
            {
                parts = sr.ReadLine().Split(' ');
                t.Reshuffle(int.Parse(parts[0]), int.Parse(parts[1]));
            }
            sw.WriteLine(t.ToString());
            sw.Close();
        }
    }

    class SplayTreeImpicit<T> where T : IComparable
    {
        private Node<T> Root;

        public override string ToString()
        {
            return LUR(Root);
        }

        private string LUR(Node<T> pos)
        {
            if (pos == null) return "";
            return (LUR(pos.Left) + pos.Value + " " + LUR(pos.Right));
        }

        public void Add(T Item)
        {
            if (Root == null)
                Root = new Node<T>(Item, null);
            else
            {
                Splay(Max());
                Root.Right = new Node<T>(Item, Root);
                Root.CountItems();
            }
        }

        public void Reshuffle(int l, int r)
        {
            if (l == 1) return;
            Node<T> ltree = CutLeft(Find(l));
            r = r - l + 1;
            bool right = r < Root.ItemsInSubtree;
            Node<T> rtree = null;
            if (right)
                rtree = CutRight(Find(r));

           MergeWith(ltree);
            if (right)
            MergeWith(rtree);
        }

        private void MergeWith(Node<T> with)
        {
            Splay(Max());
            Root.Right = with;
            with.Parent = Root;
            Root.CountItems();
        }

        private Node<T> Find(int x)
        {
            var pos = Root;
            while (x > 0 && pos != null)
            {
                if (x == 0 || x == (pos.Left != null ? pos.Left.ItemsInSubtree : 0) + 1) return pos;
                else
                    if (pos.Left != null && x < pos.Left.ItemsInSubtree + 1) pos = pos.Left;
                    else
                        if (pos.Left == null  || x > pos.Left.ItemsInSubtree + 1)
                        {
                            x -= (pos.Left != null ? pos.Left.ItemsInSubtree : 0) + 1;
                            pos = pos.Right;
                        }
            }
            return pos;
        }

        private Node<T> CutLeft(Node<T> x)
        {
            Splay(x);
            var result = x.Left;
            ReplaceAsChild(result, null);
            x.CountItems();
            return result;
        }

        private Node<T> CutRight(Node<T> x)
        {
            Splay(x);
            var result = x.Right;
            ReplaceAsChild(result, null);
            x.CountItems();
            return result;
        }

        private Node<T> Max()
        {
            var pos = Root;
            while (pos.Right != null)
                pos = pos.Right;
            return pos;
        }

        private Node<T> Min()
        {
            var pos = Root;
            while (pos.Left != null)
                pos = pos.Left;
            return pos;
        }

        private Node<T> Next(Node<T> x)
        {
            Splay(x);
            if (x.Right != null)
            {
                Node<T> pos = x.Right;
                while (pos.Left != null)
                {
                    pos = pos.Left;
                }
                return pos;
            }
            return null;
        }

        private void Splay(Node<T> x)
        {
            while (!x.isRoot)
            {
                if (x.isLeftChild)
                {
                    if (x.Parent.isRoot)
                    {
                        TurnRight(x);
                    }
                    else if (x.Parent.isLeftChild)
                    {
                        TurnRight(x.Parent);
                        TurnRight(x);
                    }
                    else if (!x.Parent.isLeftChild)
                    {
                        TurnRight(x);
                        TurnLeft(x);
                    }
                }
                else
                {
                    if (x.Parent.isRoot)
                    {
                        TurnLeft(x);
                    }
                    else if (!x.Parent.isLeftChild)
                    {
                        TurnLeft(x.Parent);
                        TurnLeft(x);
                    }
                    else if (x.Parent.isLeftChild)
                    {
                        TurnLeft(x);
                        TurnRight(x);
                    }
                }
            }
            x.CountItems();
        }

        private void TurnRight(Node<T> x)
        {
            Node<T> p = x.Parent;

            x.Parent = p.Parent;
            if (p.isRoot) Root = x;
            else
                if (p.isLeftChild) p.Parent.Left = x;
                else
                    if (!p.isLeftChild) p.Parent.Right = x;

            p.Left = x.Right;
            if (x.Right != null)
            x.Right.Parent = p;

            x.Right = p;
            p.Parent = x;

            x.Right.CountItems();
            x.CountItems();
        }

        private void TurnLeft(Node<T> x)
        {
            Node<T> p = x.Parent;

            x.Parent = p.Parent;
            if (p.isRoot) Root = x;
            else
                if (p.isLeftChild) p.Parent.Left = x;
                else
                    if (!p.isLeftChild) p.Parent.Right = x;

            p.Right = x.Left;
            if (x.Left != null)
            x.Left.Parent = p;

            x.Left = p;
            p.Parent = x;

            x.Left.CountItems();
            x.CountItems();
        }

        private void ReplaceAsChild(Node<T> n, Node<T> x)
        {
            if (x != null)
                x.Parent = n.Parent;
            if (n.isRoot) Root = x;
            else
                if (n.isLeftChild)
                    n.Parent.Left = x;
                else
                    n.Parent.Right = x;
            if (x != null && x.Parent != null)
                x.Parent.CountItems();
        }

        class Node<T>
        {
            public bool isRoot { get { return Parent == null; } }
            public bool isLeftChild { get { return !isRoot && Parent.Left == this; } }

            public readonly T Value;
            public Node<T> Left, Right, Parent;
            public int ItemsInSubtree = 1;

            public void CountItems()
            {
                ItemsInSubtree = 1 +
                                 (Left != null ? Left.ItemsInSubtree : 0) +
                                 (Right != null ? Right.ItemsInSubtree : 0);
            }

            public Node(T value, Node<T> parent)
            {
                Value = value;
                Parent = parent;
            }
        }
    }
}
