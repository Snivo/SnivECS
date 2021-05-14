using System;
using System.Diagnostics;
using ECS.Exceptions;

namespace ECS
{
    interface IComponentSet
    {
        public bool Contains(int entity);
        public void OnEntityRemoved(int ent);
    }

    class ComponentSet<T> : IComponentSet
    {
        SparseSet entities;
        PagedArray<T> components;

        public ReadOnlySpan<int> Entities => entities.Values;

        public bool Contains(int entity) => entities.HasValue(entity);

        public void Add(int entity, T component)
        {
            entities.AddValue(entity);
            components[entity] = component;
        }

        public void Remove(int entity) => entities.RemoveValue(entity);

        public ref T Get(int entity)
        {
            Debug.Assert(entities.HasValue(entity), "Entity does not contain this component");

            return ref components.GetByReference(entity);
        }

        public void OnEntityRemoved(int ent) => entities.RemoveValue(ent);

        public ComponentSet(World world)
        {
            entities = new SparseSet();
            components = new PagedArray<T>();

            world.OnEntityRemoved += OnEntityRemoved;
        }
    }
}
