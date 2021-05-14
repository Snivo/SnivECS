using System;
using System.Collections.Generic;

using UnregisteredComponentException = ECS.Exceptions.UnregisteredComponentException;

namespace ECS
{
    class EntitySignature
    {
        Dictionary<Type, int> internalTypeIDLookup;
        LinkedList<int> lookupGaps;
        Type[] internalTypeLookup;
        SparseSet signature;

        int LookupComponent<T>()
        {
            if (internalTypeIDLookup.TryGetValue(typeof(T), out int idx))
                return idx;

            return -1;
        }

        Type LookupType(int i)
        {
            if (i >= internalTypeLookup.Length)
                return null;

            return internalTypeLookup[i];
        }

        int GetComponentIDFromWorld(World world, int id)
        {
            Type t = LookupType(id);

            if (t == null) 
                throw new UnregisteredComponentException("The component does not exist in the given signature");


            return world.GetComponentID(t);
        }

        public void Add<T>()
        {
            Type t = typeof(T);

            if (LookupComponent<T>() != -1)
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

            if (idx == -1)
                throw new UnregisteredComponentException("The component does not exist in the given signature");

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

                if (idx == -1)
                    return false;

                if (!world.EntityHasComponent(idx, ent))
                    return false;
            }

            return true;
        }

        public bool HasComponent<T>()
        {
            int idx = LookupComponent<T>();

            if (idx == -1)
                throw new UnregisteredComponentException();

            return signature.HasValue(idx);
        }

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
