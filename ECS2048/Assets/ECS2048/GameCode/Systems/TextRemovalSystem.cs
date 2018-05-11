using Unity.Entities;

[UpdateAfter(typeof(BlockCollisionSystem))]
[UpdateBefore(typeof(BlockRemovalSystem))]
public class TextRemovalSystem : ComponentSystem
{
    [Inject] private TextRemovalData data;
    
    protected override void OnUpdate()
    {
        for (int i = 0; i < data.Length; i++)
        {
            if (data.tag[i].Destroy)
            {
                UnityEngine.Object.Destroy(Bootstrap.GameSettings.BlockTexts[data.text[i].Index].gameObject);
            }
        }
    }
}
