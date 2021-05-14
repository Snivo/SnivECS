using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnregisteredComponentException = ECS.Exceptions.UnregisteredComponentException;

namespace ECS
{
    class EntitySignature
    {
        Dictionary<Type, int> internalTypeIDLookup;
        LinkedList<int> lookupGaps;
        Type[] internalTypeLookup;
        SparseSet signature;

        int LookupComponent<T>() => internalTypeIDLookup[typeof(T)];

        Type LookupType(int i) => internalTypeLookup[i];

        int GetComponentIDFromWorld(World world, int id) => world.GetComponentID(LookupType(id));

        public void Add<T>()
        {
            Type t = typeof(T);

            if (internalTypeIDLookup.ContainsKey(t))
                return;
            
            int idx = internalTypeIDLookup.Count;
            if (lookupGaps.First != null)
            {
                idx = lookupGaps.First.Value;
                lookupGaps.RemoveFirst();
            }

            internalTypeIDLookup.Add(t, idx);

            if (idx >= internalTypeLookup.Length)
                Array.Resize(ref internalTypeLookup, internalTypeLookup.Length * 2);

            internalTypeLookup[idx] = t;
            signature.AddValue(idx);
        }

        public void Remove<T>()
        {
            int idx = LookupComponent<T>();

            lookupGaps.AddLast(idx);
            signature.RemoveValue(idx);
            internalTypeLookup[idx] = null;
            internalTypeIDLookup.Remove(typeof(T));
        }

        public bool EntityMatch(World world, int ent)
        {
            if (signature.Count == 0)
                return true;

            foreach(int i in signature.Values)
            {
                int idx = GetComponentIDFromWorld(world, i);

                if (!world.EntityHasComponent(idx, ent))
                    return false;
            }

            return true;
        }

        public bool HasComponent<T>() => signature.HasValue(LookupComponent<T>());

        public static explicit operator ReadOnlyEntitySignature(EntitySignature sig) => new ReadOnlyEntitySignature(sig);

        public EntitySignature()
        {
            internalTypeIDLookup = new Dictionary<Type, int>();
            internalTypeLookup = new Type[128];
            lookupGaps = new LinkedList<int>();
            signature = new SparseSet();
        }
    }

    struct ReadOnlyEntitySignature
    {
        EntitySignature sig;

        public bool EntityMatch(World world, int ent) => sig.EntityMatch(world, ent);
        public bool HasComponent<T>() => sig.HasComponent<T>();

        public ReadOnlyEntitySignature(EntitySignature sig)
        {
            this.sig = sig;
        }
    }
}
