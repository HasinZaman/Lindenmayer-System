using System;
using System.Collections;
using System.Collections.Generic;

namespace LSystem
{
	public class Sentence : IList<char>
    {
        public class Enumerator : IEnumerator<char>
        {
            private Node start;
            private Node pointer;
            public char Current
            {
                get
                {
                    return (char) pointer.value;
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return (object)Current;
                }
            }

            public Enumerator(Node start)
            {
                this.start = new Node();
                this.start.next = start;
                pointer = this.start;
            }
            
            public void Dispose()
            {
                start = null;
                pointer = null;
            }

            public bool MoveNext()
            {
                if(pointer.next == null)
                {
                    return false;
                }
                pointer = pointer.next;
                return true;
            }
            public bool MovePrev()
            {
                if (pointer.prev == null)
                {
                    return false;
                }
                pointer = pointer.prev;
                return true;
            }

            public void Reset()
            {
                pointer = start;
            }
        }

        public class Node
        {
            public Node next = null;
            public Node prev = null;
            public char? value = null;

            public Node(char value)
            {
                this.value = value;
            }
            public Node()
            {
                this.value = null;
            }
        }

        private static void connect(Node n1, Node n2)
        {
            n1.next = n2;
            n2.prev = n1;
        }

        private static void connect(Node n1, Node n2, Node n3)
        {
            connect(n1, n2);
            connect(n2, n3);
        }

        private static void remove(Node n1)
        {
            Node prev = n1.prev;
            Node next = n1.next;

            if (prev != null && next != null)
            {
                connect(prev, next);
            }
            else if (prev != null)
            {
                prev.next = null;
            }
            else if (next != null)
            {
                next.prev = null;
            }

            n1.prev = null;
            n1.next = null;
            n1.value = null;
        }

        private Node first;
        private Node last;

        public int Count
        {
            get;
            private set;
        }

        public bool IsReadOnly
        {
            get;
            private set;
        }

        public Sentence()
		{
            first = new Node();
            last = first;
        }
        public Sentence(char[] sentence)
        {
            first = new Node();
            last = first;

            foreach(char c in sentence)
            {
                Add(c);
            }
        }

        public char this[int index] 
        {
            get 
            {
                if(Count < index)
                {
                    throw new IndexOutOfRangeException();
                }

                Node pointer = first;
                if(index < Count - index)
                {
                    for (int i1 = 0; i1 < index; i1++)
                    {
                        pointer = pointer.next;
                    }
                }
                else
                {
                    pointer = last;

                    for (int i1 = Count - 1; index < i1; i1--)
                    {
                        pointer = pointer.prev;
                    }
                }

                return (char) pointer.value;
            }
            
            set
            {
                if (Count < index)
                {
                    throw new IndexOutOfRangeException();
                }

                Node pointer = first;

                for (int i1 = 0; i1 < index; i1++)
                {
                    pointer = pointer.next;
                }

                pointer.value = value;
            }
        }

        private Node getNodeAt(int index)
        {
            if (Count < index)
            {
                throw new IndexOutOfRangeException();
            }

            Node pointer;
            if (index < Count - index)
            {
                pointer = first;
                for (int i1 = 0; i1 < index; i1++)
                {
                    pointer = pointer.next;
                }
            }
            else
            {
                pointer = last;

                for (int i1 = Count - 1; index < i1; i1--)
                {
                    pointer = pointer.prev;
                }
            }

            return pointer;
        }
        public void Add(char item)
        {
            if (Count == 0)
            {
                first.value = item;
            }
            else
            {
                connect(last, new Node(item));

                last = last.next;
            }
            Count += 1;
        }

        public void Clear()
        {
            Node tmp1 = last;
            Node tmp2;
            last = first;
            first.value = null;

            while(tmp1.prev != first && tmp1 != first)
            {
                tmp2 = tmp1.prev;

                tmp1.prev = null;
                tmp1.next = null;
                tmp1.value = null;

                tmp1 = tmp2;
            }

            first.next = null;
            first.prev = null;
            first.value = null;

            Count = 0;
        }

        public bool Contains(char item)
        {
            Node pointer = first;
            for(int i1 = 0; i1 < Count; i1++)
            {
                if (pointer.value == item)
                {
                    return true;
                }
                pointer = pointer.next;
            }
            return false;
        }

        public void CopyTo(char[] array, int arrayIndex)
        {
            if(array == null)
            {
                throw new ArgumentNullException();
            }
            if(arrayIndex < 0 || Count < arrayIndex)
            {
                throw new ArgumentOutOfRangeException();
            }

            IEnumerator<char> enumerator = GetEnumerator();
            enumerator.MoveNext();
            bool endCond = true;
            for(int i1 = arrayIndex; i1 < array.Length && endCond; i1++)
            {
                array[i1] = enumerator.Current;
                endCond = enumerator.MoveNext();
            }
        }

        public IEnumerator<char> GetEnumerator()
        {
            return new Enumerator(first);
        }

        public int IndexOf(char item)
        {
            Node pointer = first;
            for (int i1 = 0; i1 < Count; i1++)
            {
                if (pointer.value == item)
                {
                    return i1;
                }
                pointer = pointer.next;
            }
            return -1;
        }

        public void Insert(int index, char item)
        {
            if(index < 0 || Count < index)
            {
                throw new IndexOutOfRangeException();
            }

            Node pointer;
            Node tmp = new Node(item);

            if(Count == 0 && index == 0)
            {
                first.value = item;
            }
            else if(index == 0)
            {

                connect(tmp, first);

                first = tmp;
            }
            else if(index == Count)
            {
                connect(last, tmp);

                last = tmp;
            }
            else if(index < Count - index)
            {
                pointer = first;

                for (int i1 = 0; i1 < index; i1++)
                {
                    pointer = pointer.next;
                }

                connect(pointer.prev, tmp, pointer);
            }
            else
            {
                pointer = last;

                for (int i1 = Count - 1; index < i1; i1--)
                {
                    pointer = pointer.prev;
                }

                connect(pointer.prev, tmp, pointer);
            }

            Count++;
        }

        public void InsertSentence(int index, char[] items)
        {
            if(items == null)
            {
                throw new NullReferenceException();
            }

            if (items.Length == 0)
            {
                throw new ArgumentException();
            }

            if (index < 0 || Count < index)
            {
                throw new IndexOutOfRangeException();
            }

            Node start = new Node(items[0]);
            Node end;
            Node pointer = start;
            
            for (int i1 = 1; i1 < items.Length; i1++)
            {
                connect(pointer, new Node(items[i1]));
                pointer = pointer.next;
            }
            end = pointer;

            insertNodes(index, start, end);
            Count += items.Length;
        }

        public void InsertSentence(int index, Sentence items)
        {
            if (items == null)
            {
                throw new NullReferenceException();
            }

            if (items.Count == 0)
            {
                throw new ArgumentException();
            }

            if (index < 0 || Count < index)
            {
                throw new IndexOutOfRangeException();
            }

            IEnumerator<char> enumerator = items.GetEnumerator();
            enumerator.MoveNext();
            Node start = new Node(enumerator.Current);
            Node end;
            Node pointer = start;

            while (enumerator.MoveNext())
            {
                connect(pointer, new Node(enumerator.Current));
                pointer = pointer.next;
            }
            end = pointer;
            
            insertNodes(index, start, end);

            Count += items.Count;
        }

        private void insertNodes(int index, Node start, Node end)
        {
            if (index == 0 && Count == 0)
            {
                first = start;
                last = end;
            }
            else if (index == 0)
            {
                connect(end, first);
                first = start;

            }
            else if (index == Count)
            {
                connect(last, start);
                last = end;
            }
            else
            {
                Node pointer = getNodeAt(index);
                connect(pointer.prev, start);
                connect(end, pointer);
            }
        }

        public bool Remove(char item)
        {
            Node pointer = first;
            
            for(int i1 = 0; i1 < Count; i1++)
            {
                if(pointer.value == item)
                {
                    if(pointer == first)
                    {
                        first = first.next;
                    }
                    else if(pointer == last)
                    {
                        last = last.prev;
                    }
                    remove(pointer);
                    Count--;
                    return true;
                }
                pointer = pointer.next;
            }
            return false;
        }

        public void RemoveAt(int index)
        {
            if (Count < index)
            {
                throw new IndexOutOfRangeException();
            }

            Node pointer;
            
            if(Count == 1 && index == 0)
            {
                first.value = null;
            }
            else if (index == 0)
            {
                pointer = first.next;
                remove(first);
                first = pointer;
            }
            else if (index == Count - 1)
            {
                pointer = last.prev;
                remove(last);
                last = pointer;
            }
            else if (index < Count - index)
            {
                pointer = first;

                for (int i1 = 0; i1 < index; i1++)
                {
                    pointer = pointer.next;
                }

                remove(pointer);
            }
            else
            {
                pointer = last;

                for (int i1 = Count - 1; index < i1; i1--)
                {
                    pointer = pointer.prev;
                }

                remove(pointer);
            }

            Count--;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Sentence Clone()
        {
            Sentence tmp = new Sentence();
            foreach(char c in this)
            {
                tmp.Add(c);
            }
            return tmp;
        }
    }
}
