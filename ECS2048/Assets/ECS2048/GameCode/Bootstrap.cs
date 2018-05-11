using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Collections;
using UnityEngine;
using TMPro;

public class Bootstrap : MonoBehaviour
{
    public static float4 Border { get; private set; }
    public static GameSettings GameSettings { get; private set; }

    public static EntityArchetype PlayerArchetype { get; private set; }
    public static EntityArchetype BlockArchetype { get; private set; }
    public static EntityArchetype FloorArchetype { get; private set; }

    public static MeshInstanceRenderer BlockLook { get; private set; }
    public static MeshInstanceRenderer FloorLook { get; private set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void InitializeBeforeScene()
    {
        World.Active.SetBehavioursActive(false);
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();
        SetArchetypes(entityManager);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitializeAfterScene()
    {
        GameSettings = FindObjectOfType<GameSettings>()?.GetComponent<GameSettings>();
        if (!GameSettings)
            return;

        BlockLook = Utils.GetLookFromPrototype("BlockLook");
        FloorLook = Utils.GetLookFromPrototype("FloorLook");

        World.Active.GetExistingManager<UISystem>().SetupUI(NewGame);
        NewGame(); // dummy
    }

    public static void NewGame()
    {
        var entityManager = World.Active.GetOrCreateManager<EntityManager>();

        CreatePlayer(entityManager);
        CreateFloor(entityManager);
        CreateBlocks(entityManager);
        World.Active.SetBehavioursActive(true);
        Time.timeScale = 1;
    }

    public static void GameOver()
    {
        World.Active.SetBehavioursActive(false);
        GameSettings.GameOverCanvas.SetActive(true);
        GameSettings.HUDCanvas.SetActive(false);
    }

    public static void CreateBlock(EntityManager entityManager, float3 pos, Entity block = default(Entity))
    {
        if (block == default(Entity))
            block = entityManager.CreateEntity(BlockArchetype);

        GameSettings.BlockTexts.Add(CreateTextMeshPro());

        entityManager.SetComponentData(block, new Position { Value = pos });
        entityManager.SetComponentData(block, new BlockMarker { Value = 2 });
        entityManager.SetComponentData(block, new Removable { Destroy = false });
        entityManager.SetComponentData(block, new Heading { Value = new float3(0.0f, 1.0f, 0) });
        entityManager.SetComponentData(block, new TextUI { Index = GameSettings.BlockTexts.Count - 1 });
        entityManager.AddSharedComponentData(block, BlockLook);
    }

    private static TextMeshPro CreateTextMeshPro()
    {
        GameObject newText = new GameObject("Block Text", typeof(TextMeshPro));
        TextMeshPro textMesh = newText.GetComponent<TextMeshPro>();
        textMesh.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1);
        textMesh.fontSize = 5;
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.margin = Vector4.zero;
        return textMesh;
    }

    private static void CreatePlayer(EntityManager entityManager)
    {
        var player = entityManager.CreateEntity(PlayerArchetype);
        entityManager.SetComponentData(player, new ScoreHolder() { Score = 0 });
    }

    private static void CreateBlocks(EntityManager entityManager)
    {
        int startBlocksLength = 2;
        NativeArray<Entity> startBlocks = new NativeArray<Entity>(startBlocksLength, Allocator.Temp);
        entityManager.CreateEntity(BlockArchetype, startBlocks);

        int index = 0;
        for (int x = 0; x < 1; x++)
        {
            var gapX = (x * GameSettings.GridGap) - (GameSettings.GridSize.x / 2);
            for (int y = 0; y < startBlocksLength; y++)
            {
                var block = startBlocks[index];
                var gapY = y * GameSettings.GridGap - (GameSettings.GridSize.y / 2);
                float3 pos = new float3(x + gapX, y + gapY, -1);
                CreateBlock(entityManager, pos, block);
                index++;
            }
        }

        startBlocks.Dispose();
    }

    private static void CreateFloor(EntityManager entityManager)
    {
        var halfGridSize = GameSettings.GridSize / 2;
        var lastX = GameSettings.GridSize.x - 1;
        var lastY = GameSettings.GridSize.y - 1;
        float2 firstFloorPos = -halfGridSize;
        var LastFloorPos = new float2(lastX + ((lastX * GameSettings.GridGap) - halfGridSize.x),
                                     lastY + ((lastY * GameSettings.GridGap) - halfGridSize.y));
        firstFloorPos -= 0.1f; // to be more precise;
        LastFloorPos += 0.1f;  // to be more precise;
        Border = new float4(firstFloorPos, LastFloorPos);

        Heading defaultHeading = new Heading() { Value = new float3(0.0f, 1.0f, 0) };
        NativeArray<Entity> newEntities = new NativeArray<Entity>(GameSettings.GridSize.x * GameSettings.GridSize.y, Allocator.Temp);
        entityManager.CreateEntity(FloorArchetype, newEntities);

        int index = 0;
        for (int x = 0; x < GameSettings.GridSize.x; x++)
        {
            var gapX = (x * GameSettings.GridGap) - (GameSettings.GridSize.x / 2);
            for (int y = 0; y < GameSettings.GridSize.y; y++)
            {
                var floor = newEntities[index];
                var gapY = y * GameSettings.GridGap - (GameSettings.GridSize.y / 2);
                float3 pos = new float3(x + gapX, y + gapY, 0);
                entityManager.SetComponentData(floor, new Position { Value = pos });
                entityManager.SetComponentData(floor, new FloorMarker { Pos = pos, IsFree = true });
                entityManager.SetComponentData(floor, defaultHeading);
                entityManager.AddSharedComponentData(floor, FloorLook);
                index++;

                if (y == 0)
                {

                }
            }
        }
        newEntities.Dispose();
    }

    private static void SetArchetypes(EntityManager entityManager)
    {
        var scoreHolder = ComponentType.Create<ScoreHolder>();
        var playerInput = ComponentType.Create<Input>();
        var playerTag = ComponentType.Create<PlayerMarker>();
        var blockTag = ComponentType.Create<BlockMarker>();
        var floorTag = ComponentType.Create<FloorMarker>();
        var removeTag = ComponentType.Create<Removable>();
        var textTag = ComponentType.Create<TextUI>();
        var heading = ComponentType.Create<Heading>();
        var position = ComponentType.Create<Position>();
        var transformMatrix = ComponentType.Create<TransformMatrix>();

        PlayerArchetype = entityManager.CreateArchetype(
            playerTag, scoreHolder
            );

        BlockArchetype = entityManager.CreateArchetype(
            position, transformMatrix, heading, playerInput, blockTag, removeTag, textTag
            );

        FloorArchetype = entityManager.CreateArchetype(
            position, transformMatrix, heading, floorTag
            );
    }

}
