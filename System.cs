using System;
using System.Collections.Generic;
using System.Text;

namespace ECS
{
    /// <summary>
    /// Optional system class for convenience
    /// </summary>
    abstract class ECSSystem
    {
        protected EntitySignature signature;
        Entity[] entSet;
        int entsInSet;

        public ReadOnlyEntitySignature Signature => (ReadOnlyEntitySignature)signature;
        protected ReadOnlySpan<Entity> TargetEntities => entSet.AsSpan(0, entsInSet);

        /// <summary>
        /// Loads all entities that match the systems signature into TargetEntities
        /// </summary>
        protected void GetEntitiesFromSignature( World world )
        {
            entsInSet = 0;
            foreach (int entity in world.Entities)
            {
                if (signature.EntityMatch(world, entity))
                {
                    if (entsInSet > entSet.Length)
                        Array.Resize(ref entSet, entSet.Length * 2);

                    entSet[entsInSet++] = new Entity(entity, world);
                }
            }
        }

        public ECSSystem()
        {
            signature = new EntitySignature();
            entSet = new Entity[1024];
            entsInSet = 0;
        }
    }
}
