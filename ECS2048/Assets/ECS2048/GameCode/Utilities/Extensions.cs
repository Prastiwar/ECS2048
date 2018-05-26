using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace TP.ECS2048
{
    public static class Extensions
    {
        public static void DestroyAllEntities(this EntityManager manager)
        {
            NativeArray<Entity> entitiesArray = manager.GetAllEntities();
            manager.DestroyEntity(entitiesArray);
            entitiesArray.Dispose();
        }

        public static void SetBehavioursActive(this World world, bool value)
        {
            foreach (var bm in world.BehaviourManagers)
            {
                bm.Enabled = value;
            }
        }

        public static void SetBehavioursActive(this World world, bool value, params System.Type[] types)
        {
            foreach (var bm in world.BehaviourManagers)
            {
                foreach (var type in types)
                {
                    if(type == bm.GetType())
                        bm.Enabled = value;
                }
            }
        }

        public static bool IsPowOf2(this int num)
        {
            return num != 0 && (num & (num - 1)) == 0;
        }
    }
}
