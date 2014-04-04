using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscreteMathLab11_C
{
    class Program
    {
        static void Main(string[] args)
        {
            B2Tree t = new B2Tree();
            t.Add(1);
            t.Add(2);
            t.Add(3);
            t.Add(4);
            t.Add(5);
            t.Add(6);
            t.Add(7);
            t.Add(8);
            t.Add(9);
            t.Add(10);
        }
    }

    class B2Tree
    {
        private Node Root;

        public void Regroup(int l, int r)
        {
            Node left = null, mid = null, right = null;
            Node pos = Root;
            int children = 0;
            while (l > 0)
            {
                pos.CutLeft = 0;
                while (pos.CutLeft < (pos.hasLeaves ? pos.LeavesCount : pos.ChildrenCount))
                {
                    if (l >= pos.Children[pos.CutLeft].ItemsInSubtree)
                    {

                        l -= pos.Children[pos.CutLeft++].ItemsInSubtree;
                    }
                }
            }
        }

        public void AddFourth(Node replace, Node with1, Node with2)
        {
            bool WorkWithRoot = replace.parent == null;
            Node parent;
            if (WorkWithRoot) replace.parent = new Node {Children = new Node[] {replace, null, null}};
            parent = replace.parent;
            if (parent.ChildrenCount == 1)
            {
                parent.Children[0] = with1;
                parent.Children[1] = with2;
                with1.parent = with2.parent = parent;
                parent.ChildrenCount++;
            }
            else
            if (parent.ChildrenCount == 2)
            {
                with1.parent = with2.parent = parent;
                if (parent.Children[0] == replace)
                {
                    parent.Children[2] = parent.Children[1];
                    parent.Children[0] = with1;
                    parent.Children[1] = with2;
                }
                if (parent.Children[1] == replace)
                {
                    parent.Children[1] = with1;
                    parent.Children[2] = with2;
                }
                parent.ChildrenCount++;
            }
            else
            if (parent.ChildrenCount == 3)
            {
                Node r1 = new Node(), r2 = new Node();
                r2.ChildrenCount = r1.ChildrenCount = 2;
                if (parent.Children[0] == replace)
                {
                    with1.parent = with2.parent = r1;
                    parent.Children[1].parent = parent.Children[2].parent = r2;
                    r1.Children[0] = with1;
                    r1.Children[1] = with2;
                    r2.Children[0] = parent.Children[1];
                    r2.Children[1] = parent.Children[2];
                }
                if (parent.Children[1] == replace)
                {
                    parent.Children[0].parent = with1.parent = r1;
                    with2.parent = parent.Children[2].parent = r2;
                    r1.Children[0] = parent.Children[0];
                    r1.Children[1] = with1;
                    r2.Children[0] = with2;
                    r2.Children[1] = parent.Children[2];
                }
                if (parent.Children[2] == replace)
                {
                    parent.Children[1].parent = parent.Children[2].parent = r2;
                    with1.parent = with2.parent = r2;
                    r1.Children[0] = parent.Children[0];
                    r1.Children[1] = parent.Children[1];
                    r2.Children[0] = with1;
                    r2.Children[1] = with2;
                }
                if (WorkWithRoot)
                    Root = parent;
                AddFourth(parent, r1, r2);
                r1.ItemsInSubtree = r2.Children.Sum(x => x != null ? x.ItemsInSubtree : 0);
                r2.ItemsInSubtree = r2.Children.Sum(x => x != null ? x.ItemsInSubtree : 0);
            }
            if (WorkWithRoot) Root = parent;
        }

        public void Add(int value)
        {
            Add(Root, value);
        }

        private void Add(Node at, int value)
        {
            if (Root == null)
            {
                Root = new Node();
                Root.hasLeaves = true;
                Root.Leaves[0] = value;
                at = Root;
            }
            else
                if (at.hasLeaves)
                {
                    if (at.LeavesCount < 3)
                        at.Leaves[at.LeavesCount++] = value;
                    else
                    {
                        Node r1 = new Node();
                        r1.hasLeaves = true;
                        r1.LeavesCount = 2;
                        r1.Leaves = new int[] { at.Leaves[0], at.Leaves[1], 0 };
                        Node r2 = new Node();
                        r2.hasLeaves = true;
                        r2.LeavesCount = 2;
                        r2.Leaves = new int[] { at.Leaves[2], value, 0 };
                        AddFourth(at, r1, r2);
                    }
                }
                else
                {
                    Add(at.Children[at.ChildrenCount - 1], value);
                }
            while (at.parent != null)
            {
                at = at.parent;
                at.ItemsInSubtree = at.Children.Sum(x => x != null ? x.ItemsInSubtree : 0);
            }
        }

        public int Count { get; private set; }

        internal class Node
        {
            public int CutLeft;
            public Node parent;

            public bool hasLeaves = false;
            public int ChildrenCount = 1;
            public int LeavesCount = 1;

            private int __ItemsInSubtree;

            public int ItemsInSubtree
            {
                get { return hasLeaves ? LeavesCount : __ItemsInSubtree; }
                set { __ItemsInSubtree = value; }
            }

            public Node[] Children = new Node[3];
            public int[] Leaves = new int[3];

            public int Depth()
            {
                Node pos = this;
                int result = 2;
                while (!pos.hasLeaves)
                {
                    pos = pos.Children[0];
                    result++;
                }
                return result;
            }
        }
    }
}
