using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace DiscreteMathLab11_A
{
    class Program
    {
        static void Main(string[] args)
        {
            Set<int> Set = new Set<int>();
            StreamReader sr = new StreamReader("input.txt");
            StreamWriter sw = new StreamWriter("output.txt");

            string[] parts = sr.ReadLine().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i<parts.Lengt && parts[i] != "0"; i++)
                Set.

            sw.Close();
        }
    }

    class Set<T> where T : IComparable
    {
        private Node<T> Root;
        public int Count { get; private set; }

        public void Insert(T Item)
        {
            Insert(Item, Root, 0);
        }

        int Insert(T Item, Node<T> Parent, int depth)
        {
            if (Root == null)
                Root = new Node<T>(Item, null);
            else
            {
                if (Parent.Value.CompareTo(Item) < 0)
                    if (Parent.Right != null)
                        return Insert(Item, Parent.Right);
                    else
                    {
                        Parent.Right = new Node<T>(Item, Parent);
                        return depth + 1;
                    }
                if (Parent.Value.CompareTo(Item) > 0)
                    if (Parent.Left != null)
                        return Insert(Item, Parent.Left);
                    else
                    {
                        Parent.Left = new Node<T>(Item, Parent);
                        return depth + 1;
                    }
            }
        }

        public void Delete(T Item)
        {
            Delete(Find(Item, Root));
        }
        void Delete(Node<T> del)
        {
            if (del != null)
            {
                if (del.Left == null && del.Right == null)
                {
                    ReplaceAsChild(del, null);
                }
                if (del.Left == null && del.Right != null || del.Right == null && del.Left != null)
                    ReplaceAsChild(del, del.Left ?? del.Right);
                if (del.Left != null && del.Right != null)
                {
                    Node<T> replacement = del.Next();
                    Delete(replacement);
                    del.Value = replacement.Value;
                }
            }
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
        }
    }
}
