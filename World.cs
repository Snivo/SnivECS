using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            int cmpId = GetComponentID<T>();

            Debug.Assert(cmpId != -1, "The specified component has not been registered");

            return ref ((ComponentSet<T>)components[cmpId]).Get(ent);
        }

        public bool EntityExists(int ent) => entities.HasValue(ent);

        public bool EntityHasComponent<T>(int ent) => EntityHasComponent(GetComponentID<T>(), ent);

        public bool EntityHasComponent(int cmp, int ent)
        {
            Debug.Assert(cmp < components.Length && cmp >= 0, "The specified component has not been registered");

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
            Debug.Assert(componentLookup.ContainsKey(typeof(T)), "The specified component has not been registered");

            return componentLookup[typeof(T)];
        }

        public int GetComponentID(Type t)
        {
            Debug.Assert(componentLookup.ContainsKey(t), "The specified component has not been registered");

            return componentLookup[t];
        }

        public void AddComponent<T>(int ent, T component)
        {
            Debug.Assert(entities.HasValue(ent), "The specified entity does not exist");
            Debug.Assert(componentLookup.ContainsKey(typeof(T)), "The specified component has not been registered");

            ((ComponentSet<T>)components[componentLookup[typeof(T)]]).Add(ent, component);
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
            Debug.Assert(componentLookup.ContainsKey(typeof(T)));
            Debug.Assert(entities.HasValue(ent), "The specified entity does not exist");

            ((ComponentSet<T>)components[componentLookup[typeof(T)]]).Remove(ent);
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
