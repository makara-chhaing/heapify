﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heap
{
    public class Heap<K, D> where K : IComparable<K>
    {

        // This is a nested Node class whose purpose is to represent a node of a heap.
        private class Node : IHeapifyable<K, D>
        {
            // The Data field represents a payload.
            public D Data { get; set; }
            // The Key field is used to order elements with regard to the Binary Min (Max) Heap Policy, i.e. the key of the parent node is smaller (larger) than the key of its children.
            public K Key { get; set; }
            // The Position field reflects the location (index) of the node in the array-based internal data structure.
            public int Position { get; set; }

            public Node(K key, D value, int position)
            {
                Data = value;
                Key = key;
                Position = position;
            }

            // This is a ToString() method of the Node class.
            // It prints out a node as a tuple ('key value','payload','index')}.
            public override string ToString()
            {
                return "(" + Key.ToString() + "," + Data.ToString() + "," + Position + ")";
            }
        }

        // ---------------------------------------------------------------------------------
        // Here the description of the methods and attributes of the Heap<K, D> class starts

        public int Count { get; private set; }

        // The data nodes of the Heap<K, D> are stored internally in the List collection. 
        // Note that the element with index 0 is a dummy node.
        // The top-most element of the heap returned to the user via Min() is indexed as 1.
        private List<Node> data = new List<Node>();

        // We refer to a given comparer to order elements in the heap. 
        // Depending on the comparer, we may get either a binary Min-Heap or a binary  Max-Heap. 
        // In the former case, the comparer must order elements in the ascending order of the keys, and does this in the descending order in the latter case.
        private IComparer<K> comparer;

        // We expect the user to specify the comparer via the given argument.
        public Heap(IComparer<K> comparer)
        {
            this.comparer = comparer;

            // We use a default comparer when the user is unable to provide one. 
            // This implies the restriction on type K such as 'where K : IComparable<K>' in the class declaration.
            if (this.comparer == null) this.comparer = Comparer<K>.Default;

            // We simplify the implementation of the Heap<K, D> by creating a dummy node at position 0.
            // This allows to achieve the following property:
            // The children of a node with index i have indices 2*i and 2*i+1 (if they exist).
            data.Add(new Node(default(K), default(D), 0));
        }

        // This method returns the top-most (either a minimum or a maximum) of the heap.
        // It does not delete the element, just returns the node casted to the IHeapifyable<K, D> interface.
        public IHeapifyable<K, D> Min()
        {
            if (Count == 0) throw new InvalidOperationException("The heap is empty.");
            return data[1];
        }

        // Insertion to the Heap<K, D> is based on the private UpHeap() method
        public IHeapifyable<K, D> Insert(K key, D value)
        {
            Count++;
            Node node = new Node(key, value, Count);
            data.Add(node);
            UpHeap(Count);
            return node;
        }

        private void UpHeap(int start)
        {
            int position = start;
            while (position != 1)
            {
                if (comparer.Compare(data[position].Key, data[position / 2].Key) < 0) Swap(position, position / 2);
                position = position / 2;
            }
        }

        private void DownHeap(int start)
        {
            int position = start;
            int next = 0;
            while (position * 2 <= Count)
            {
                if (position * 2 == Count)
                {
                    if (comparer.Compare(data[position].Key, data[position * 2].Key) > 0)
                    {
                        next = position * 2;
                        Swap(position, next);
                        
                    }
                    return;
                }
                else
                {
                    if (comparer.Compare(data[position].Key, data[position * 2].Key) > 0
                && comparer.Compare(data[position * 2].Key, data[position * 2 + 1].Key) < 0)
                    {
                        next = position * 2;
                        Swap(position, next);
                    }
                    else if (comparer.Compare(data[position].Key, data[position * 2 + 1].Key) > 0
                    && comparer.Compare(data[position * 2].Key, data[position * 2 + 1].Key) > 0)
                    {
                        next = position * 2 + 1;
                        Swap(position, next);
                    }
                    else
                    {
                        return;
                    }
                }
                // if (comparer.Compare(data[position*2].Key, data[position * 2 + 1].Key) < 0){
                //         next = position*2;
                //         if(comparer.Compare(data[position].Key, data[next].Key) > 0){
                //             Swap(position, next);
                //         }else{
                //             return;
                //         }
                //     }else{
                //         next = position * 2 + 1;
                //         if(comparer.Compare(data[position].Key, data[next].Key) > 0){
                //             Swap(position, next);

                //         }else{
                //             return;
                //         }

                //     }
                position = next;
            }
        }

        // This method swaps two elements in the list representing the heap. 
        // Use it when you need to swap nodes in your solution, e.g. in DownHeap() that you will need to develop.
        private void Swap(int from, int to)
        {
            Node temp = data[from];
            data[from] = data[to];
            data[to] = temp;
            data[to].Position = to;
            data[from].Position = from;
        }

        public void Clear()
        {
            for (int i = 0; i <= Count; i++) data[i].Position = -1;
            data.Clear();
            data.Add(new Node(default(K), default(D), 0));
            Count = 0;
        }

        public override string ToString()
        {
            if (Count == 0) return "[]";
            StringBuilder s = new StringBuilder();
            s.Append("[");
            for (int i = 0; i < Count; i++)
            {
                s.Append(data[i + 1]);
                if (i + 1 < Count) s.Append(",");
            }
            s.Append("]");
            return s.ToString();
        }

        // TODO: Your task is to implement all the remaining methods.
        // Read the instruction carefully, study the code examples from above as they should help you to write the rest of the code.
        public IHeapifyable<K, D> Delete()
        {
            if (Count == 0) throw new InvalidOperationException("The heap is empty.");
            Node n;
            Swap(1, Count);
            n = data[Count];
            data.RemoveAt(Count);
            Count--;
            DownHeap(1);
            return (IHeapifyable<K, D>)n;

        }

        // Builds a minimum binary heap using the specified data according to the bottom-up approach.
        public IHeapifyable<K, D>[] BuildHeap(K[] keys, D[] data)
        {
            if (Count != 0) throw new InvalidOperationException("The heap is not empty.");
            for (int i = 0; i < keys.Length; i++)
            {
                Count++;
                Node node = new Node(keys[i], data[i], Count);
                this.data.Add(node);
            }
            // Console.WriteLine("After input:" + this.ToString());
            Node[] nodes = new Node[keys.Length];
            for (int i = keys.Length; i > 0; i--)
            {
                // Console.WriteLine("time:{0} orders:" + this.ToString(),i);
                nodes[i - 1] = this.data[i];
                DownHeap(i);
            }

            return (IHeapifyable<K, D>[])nodes;
        }

        public void DecreaseKey(IHeapifyable<K, D> element, K new_key)
        {
            int index = element.Position;
            if (comparer.Compare(element.Key, data[index].Key) != 0)
            {
                throw new InvalidOperationException("Invalid element given!");
            }
            data[index].Key = new_key;
            UpHeap(index);

        }
        public IHeapifyable<K, D> DeleteElement(IHeapifyable<K, D> element)
        {
            if (Count == 0) throw new InvalidOperationException("The heap is empty.");
            Node n;
            int pos = element.Position;
            Swap(pos, Count);
            n = data[Count];
            data.RemoveAt(Count);
            Count--;
            DownHeap(pos);
            return (IHeapifyable<K, D>)n;
        }
        public IHeapifyable<K, D> KthMinElement(int k)
        {
            if(k < 1 || k > Count) throw new InvalidOperationException("Invalid number.");
            int i = 1;

            IHeapifyable<K, D>[] nodes = new Node[k];

            // This takes O(klogn) since it deletes (O(logn)) k elements from heap and save in nodes.
            while(i < k){
                nodes[i] = Delete();
                i++;
            }

            IHeapifyable<K, D> temp = Min();

            // This takes O(klogn) since it inserts (O(logn)) k elements back to Heap.
            for(int j = 1; j < k; j++){
                Insert(nodes[j].Key, nodes[j].Data);
            }

            // So the total complexity of this is O(klogn).
            return temp;
        }

    }
}
