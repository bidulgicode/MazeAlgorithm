using System;
using System.Collections.Generic;

namespace Exercise
{
    class TreeNode<T>
    {
        public T Data { get; set; }
        // 부모가 누군지는 모르는 Children
        public List<TreeNode<T>> Children { get; set; } = new List<TreeNode<T>>();
    }

    class Program
    {
        static TreeNode<string> MakeTree()
        {
            TreeNode<string> root = new TreeNode<string>() { Data = "R1 개발실" };
            {
                {
                    TreeNode<string> node = new TreeNode<string>() { Data = "디자인팀" };
                    node.Children.Add(new TreeNode<string>() { Data = "전투" });
                    node.Children.Add(new TreeNode<string>() { Data = "경제" });
                    node.Children.Add(new TreeNode<string>() { Data = "스토리" });
                    root.Children.Add(node);
                }
                {
                    TreeNode<string> node = new TreeNode<string>() { Data = "프로그래밍팀" };
                    node.Children.Add(new TreeNode<string>() { Data = "서버" });
                    node.Children.Add(new TreeNode<string>() { Data = "클라" });
                    node.Children.Add(new TreeNode<string>() { Data = "엔진" });
                    root.Children.Add(node);
                }
                {
                    TreeNode<string> node = new TreeNode<string>() { Data = "아트팀" };
                    node.Children.Add(new TreeNode<string>() { Data = "배경" });
                    node.Children.Add(new TreeNode<string>() { Data = "캐릭터" });
                    root.Children.Add(node);
                }
            }
            return root;
        }

        static void PrintTree(TreeNode<string> root)
        {
            // 트리의 모든노드를 다 순회하려면? -> 재귀가 편하다.
            // 일단 나부터 출력
            Console.WriteLine(root.Data);

            // 자식들도 출력하도록 떠넘김
            foreach (TreeNode<string> child in root.Children)
                PrintTree(child);   // 재귀
        }


        // 트리 높이 구하기는 코딩문제로 내기 좋다.
        static int GetHeight(TreeNode<string> root)
        {
            int height = 0;

            foreach (TreeNode<string> child in root.Children)
            {
                int newHeight = GetHeight(child) + 1;
                if (height < newHeight)
                    height = newHeight;
                // 위 if문과 동일함
                // height = Math.Max(height, newHeight);
            }
            
            return height;
        }

        static void Main(string[] args)
        {
            TreeNode<string> root = MakeTree();

            //PrintTree(root);
            Console.WriteLine(GetHeight(root));
        }
    }
}
