using System;
using System.Threading.Tasks;

namespace ECS
{
    class Program
    {
        struct Position
        {
            public int X;
            public int Y;
            public override string ToString() => $"Position: ({X}, {Y})";
        }

        struct Velocity
        {
            public int X;
            public int Y;
        }

        class ApplyVelocitySystem : ECSSystem
        {
            public void Run(World world)
            {
                GetEntitiesFromSignature(world);

                foreach (Entity entity in TargetEntities)
                {
                    ref Velocity v = ref entity.GetComponent<Velocity>();
                    ref Position p = ref entity.GetComponent<Position>();

                    p.X += v.X;
                    p.Y += v.Y;
                }
            }

            public ApplyVelocitySystem()
            {
                signature.Add<Velocity>();
                signature.Add<Position>();
            }
        }

        static void Main(string[] args)
        {
            ApplyVelocitySystem velocitySystem = new ApplyVelocitySystem();
            World world = new World(), world2 = new World();

            world.RegisterComponent<Position>();
            world.RegisterComponent<Velocity>();
            world2.RegisterComponent<Position>();
            world2.RegisterComponent<Velocity>();
            Random rng = new Random();

            for (int i = 0; i < 25; i++)
            {
                int ent = world.CreateEntity();
                world2.CreateEntity();
                // Every 4th entity gets no components cause testing
                if (i % 4 == 0)
                    continue;

                world.AddComponent(ent, new Position());
                world.AddComponent(ent, new Velocity() { X = rng.Next(-100, 100), Y = rng.Next(-100, 100) });

                world2.AddComponent(ent, new Position());
                world2.AddComponent(ent, new Velocity() { X = rng.Next(-100, 100), Y = rng.Next(-100, 100) });
            }

            while (true)
            {
                velocitySystem.Run(world);
                velocitySystem.Run(world2);
                Console.SetCursorPosition(0, 0);

                Console.WriteLine("World 1");
                World w = world;
                for (int i = 0; i < 25; i++)
                {
                    if (velocitySystem.Signature.EntityMatch(w, i))
                        Console.WriteLine("{0} - {1}", i, w.GetComponent<Position>(i));
                }

                Console.WriteLine("World 2");
                w = world2;
                for (int i = 0; i < 25; i++)
                {
                    if (velocitySystem.Signature.EntityMatch(w, i))
                        Console.WriteLine("{0} - {1}", i, w.GetComponent<Position>(i));
                }
            }
        }
    }
}
