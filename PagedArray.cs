using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ECS
{
    class PagedArray<T> : IEnumerable<T>
    {
        T[][] elements;
       
        public int PageSize { get; }

        public bool IsPageAllocated(int page)
        {
            return page < elements.Length && elements[page] != null;
        }

        public bool ContainsKey(int idx)
        {
            int page = idx / PageSize;

            if (page >= elements.Length)
                return false;

            if (elements[page] == null)
                return false;

            return elements[page][idx % PageSize] != null;
        }

        public ref T GetByReference(int idx) => ref elements[idx / PageSize][idx % PageSize];

        public T this[int idx]
        {
            get => elements[idx / PageSize][idx % PageSize];
            set
            {
                int page = idx / PageSize;
                if (!IsPageAllocated(page))
                    AllocatePage(page);
                elements[idx / PageSize][idx % PageSize] = value;
            }
        }

        void AllocatePage(int page) 
        {
            if (page >= elements.Length)
                Array.Resize(ref elements, page + 1);

            elements[page] = new T[PageSize];
        }

        public IEnumerator<T> GetEnumerator()
        {
            int idx = 0;
            for (int page = 0; page < elements.Length; page++)
            {
                T[] curPage = elements[page];

                if (curPage == null)
                        continue;

                for (int i = 0; i < curPage.Length; i++)
                {
                    T obj = curPage[i];
                    if (obj == null)
                        continue;

                    yield return obj;
                }
                idx += PageSize;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public PagedArray(int pageSize = 1024)
        {
            PageSize = pageSize;
            elements = new T[1][];
        }
    }
}
