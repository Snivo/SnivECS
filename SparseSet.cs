using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ECS
{
    class SparseSet
    {
        PagedArray<int> sparse;
        int[] dense;

        public int Count { get; private set; }

        public ReadOnlySpan<int> Values => dense.AsSpan(0, dense.Length);

        public bool HasValue(int val) => sparse.ContainsKey(val) && sparse[val] < Count && dense[sparse[val]] == val;

        public void AddValue(int val)
        {
            if (HasValue(val))
                return;

            if (Count >= dense.Length)
                Array.Resize(ref dense, dense.Length * 2);

            dense[Count] = val;
            sparse[val] = Count;

            Count++;
        }

        public void RemoveValue(int val)
        {
            if (!HasValue(val))
                return;

            int last = Count - 1, temp = dense[last];
            dense[sparse[val]] = temp;
            sparse[temp] = sparse[val];

            Count--;
        }

        public SparseSet()
        {
            dense = new int[128];
            sparse = new PagedArray<int>();
        }
    }
}
