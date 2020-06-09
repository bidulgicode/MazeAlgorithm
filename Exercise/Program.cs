using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Exercise
{
    // 여기 들어가는 모든 타입은 크기비교가 가능하여야 한다.
    // 크기 비교를 제공하는 인터페이스인 
    // where T : IComparable<T> 를 구현한 타입들만 들어오게 한다.
    class PriorityQueue<T> where T : IComparable<T>
    {
        List<T> _heap = new List<T>();

        public void Push(T data)
        {
            // 힙의 맨 끝에 새로운 데이터를 삽입
            _heap.Add(data);

            // 힙의 맨 위로 올려보자 (가능하다면)
            int now = _heap.Count - 1;
            while(now > 0)
            {
                int next = (now - 1) / 2; // 부모 인덱스 구하기
                if (_heap[now].CompareTo(_heap[next]) < 0)
                    break; // 부모보다 작아서 못올라간다.

                // 크다면 교체
                T temp = _heap[now];
                _heap[now] = _heap[next];
                _heap[next] = temp;

                // 검사 위치를 이동
                now = next;
            }
        }

        // 가장 큰 데이터를 넘긴다.
        public T Pop()
        {
            // 반환할 데이터를 따로 저장
            T ret = _heap[0];

            // 마지막 데이터를 루트로 이동.
            int lastIndex = _heap.Count - 1;
            _heap[0] = _heap[lastIndex];
            _heap.RemoveAt(lastIndex);
            lastIndex--;

            // 역으로 내려가기
            int now = 0;
            while (true)
            {
                // left가 범위 밖(없는) 노드를 체크할수도 있음
                int left = 2 * now + 1;
                int right = 2 * now + 2;

                int next = now;
                // 왼쪽값이 현재값보다 크면, 왼쪽으로 이동
                if (left <= lastIndex && _heap[next].CompareTo(_heap[left]) < 0)
                    next = left;
                // 오른쪽값이 현재값(현재기준 왼쪽값 포함)보다 크면, 오른쪽으로 간다.
                if (right <= lastIndex && _heap[next].CompareTo(_heap[right]) < 0)
                    next = right;

                // 왼쪽 오른쪽 모두 현재값보다 작으면 끝
                if (next == now)
                    break;

                // 두 값을 교체하자
                T temp = _heap[now];
                _heap[now] = _heap[next];
                _heap[next] = temp;

                // 검사 위치를 이동한다.
                now = next;
            }


            return ret;
        }

        public int Count()
        {
            return _heap.Count;
        }
    }

    class Knight : IComparable<Knight>
    {
        public int Id { get; set; }

        // IComparable 인터페이스에서 요구하는 구현
        public int CompareTo(Knight other)
        {
            if (Id == other.Id)
                return 0;
            // 크기비교 연산자에 따라 오름차순 내림차순 바꿀수 있음.
            return Id > other.Id ? 1 : -1;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // 꼼수로 -를 붙이면 오름차순 정렬도 가능
            PriorityQueue<Knight> q = new PriorityQueue<Knight>();
            q.Push(new Knight() { Id = 20 });
            q.Push(new Knight() { Id = 30 });
            q.Push(new Knight() { Id = 40 });
            q.Push(new Knight() { Id = 10 });
            q.Push(new Knight() { Id = 5 });

            while(q.Count() > 0)
            {
                Console.WriteLine(q.Pop().Id);
            }
        }
    }
}
