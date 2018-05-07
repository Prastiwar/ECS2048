
using Unity.Collections;
using Unity.Entities;

static class Extensions
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
}
