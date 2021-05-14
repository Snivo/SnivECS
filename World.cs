using System;
using System.Collections.Generic;
using System.Text;

using ECS.Exceptions;

namespace ECS
{
    class World
    {
        Dictionary<Type, int> componentLookup;
        IComponentSet[] components;
        SparseSet entities;
        LinkedList<int> gaps;

        public event Action<int> OnEntityCreated;
        public event Action<int> OnEntityRemoved;

        public int EntityCount => entities.Count;

        public ReadOnlySpan<int> Entities => entities.Values;

        public ref T GetComponent<T>(int ent)
        {
            if (!entities.HasValue(ent))
                throw new NullEntityException();

            int cmpId = GetComponentID<T>();

            if (cmpId == -1)
                throw new UnregisteredComponentException();

            return ref ((ComponentSet<T>)components[cmpId]).Get(ent);
        }

        public bool EntityExists(int ent) => entities.HasValue(ent);

        public bool EntityHasComponent<T>(int ent) => EntityHasComponent(GetComponentID<T>(), ent);

        public bool EntityHasComponent(int cmp, int ent)
        {
            if (cmp >= components.Length || cmp < 0)
                throw new UnregisteredComponentException();

            return components[cmp].Contains(ent);
        }

        public bool IsComponentRegistered<T>() => componentLookup.ContainsKey(typeof(T));
        
        public int CreateEntity()
        {
            int ent;

            if (gaps.First != null)
            {
                ent = gaps.First.Value;
                gaps.RemoveFirst();
            }
            else
                ent = EntityCount;

            entities.AddValue(ent);
            OnEntityCreated?.Invoke(ent);
            return ent;
        }

        public int GetComponentID<T>()
        {
            if (componentLookup.TryGetValue(typeof(T), out int ret))
                return ret;

            return -1;
        }

        public int GetComponentID(Type t)
        {
            if (componentLookup.TryGetValue(t, out int ret))
                return ret;

            return -1;
        }

        public void AddComponent<T>(int ent, T component)
        {
            if (!entities.HasValue(ent))
                throw new NullEntityException();

            Type a = typeof(T);
            if (componentLookup.TryGetValue(typeof(T), out int idx))
            {
                ComponentSet<T> set = (ComponentSet<T>)components[idx];
                set.Add(ent, component);
            }
            else
                throw new UnregisteredComponentException();
        }

        public void RegisterComponent<T>()
        {
            if (IsComponentRegistered<T>())
                return;

            int id = componentLookup.Count;

            componentLookup.Add(typeof(T), id);

            if (id >= components.Length)
                Array.Resize(ref components, components.Length + 11);

            components[id] = new ComponentSet<T>(this);
        }

        public void RemoveComponent<T>(int ent)
        {
            if (!entities.HasValue(ent))
                throw new NullEntityException();

            if (componentLookup.TryGetValue(typeof(T), out int idx))
            {
                ComponentSet<T> set = (ComponentSet<T>)components[idx];
                set.Remove(ent);
            }
            else
                throw new UnregisteredComponentException();
        }

        public void RemoveEntity(int ent)
        {
            if (!entities.HasValue(ent))
                return;

            entities.RemoveValue(ent);
            gaps.AddFirst(ent);
            OnEntityRemoved?.Invoke(ent);
        }

        public World()
        {
            componentLookup = new Dictionary<Type, int>();
            components = new IComponentSet[128];
            entities = new SparseSet();
            gaps = new LinkedList<int>();
        }
    }
}
