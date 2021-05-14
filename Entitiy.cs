using System;
using System.Collections.Generic;
using System.Text;

namespace ECS
{
    /// <summary>
    /// Convenience entity wrapper for temporary use
    /// </summary>
    struct Entity
    {
        int entity;
        World world;

        public bool IsValid => world.EntityExists(entity);

        public ref T GetComponent<T>() => ref world.GetComponent<T>(entity);
        public void AddComponent<T>(T component) => world.AddComponent(entity, component);
        public void RemoveComponent<T>() => world.RemoveComponent<T>(entity);
        public bool HasComponent<T>() => world.EntityHasComponent<T>(entity);
        public bool MatchesSignature(EntitySignature signature) => signature.EntityMatch(world, entity);
        public bool MatchesSignature(ReadOnlyEntitySignature signature) => signature.EntityMatch(world, entity);
        public void Delete() => world.RemoveEntity(entity);

        public Entity(int entity, World world)
        {
            this.entity = entity;
            this.world = world;
        }
    }
}
