using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Transforms2D;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Collections;
using System;

public class Bootstrap : MonoBehaviour
{
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

    private static void CreatePlayer(EntityManager entityManager)
    {
        var player = entityManager.CreateEntity(PlayerArchetype);
        //entityManager.SetComponentData(player, new PlayerInput());
        entityManager.SetComponentData(player, new ScoreHolder() { Value = 0 });
    }

    private static void CreateBlocks(EntityManager entityManager)
    {
        int startBlocksLength = 2;
        Heading defaultHeading = new Heading() { Value = new float3(0.0f, 1.0f, 0) };
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
                entityManager.SetComponentData(block, new Position { Value = new float3(x + gapX, y + gapY, -1) });
                entityManager.SetComponentData(block, defaultHeading);
                entityManager.AddSharedComponentData(block, BlockLook);
                index++;
            }
        }

        startBlocks.Dispose();
    }

    public static void CreateFloor(EntityManager entityManager)
    {
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
                entityManager.SetComponentData(floor, new Position { Value = new float3(x + gapX, y + gapY, 0) });
                entityManager.SetComponentData(floor, defaultHeading);
                entityManager.AddSharedComponentData(floor, FloorLook);
                index++;
            }
        }
        newEntities.Dispose();
    }

    private static void SetArchetypes(EntityManager entityManager)
    {
        var scoreHolder = ComponentType.Create<ScoreHolder>();
        var playerInput = ComponentType.Create<PlayerInput>();
        var playerTag = ComponentType.Create<PlayerMarker>();
        var blockTag = ComponentType.Create<BlockMarker>();
        var heading = ComponentType.Create<Heading>();
        var position = ComponentType.Create<Position>();
        var transformMatrix = ComponentType.Create<TransformMatrix>();

        PlayerArchetype = entityManager.CreateArchetype(
            playerTag, scoreHolder
            );

        BlockArchetype = entityManager.CreateArchetype(
            position, transformMatrix, heading, playerInput, blockTag
            );

        FloorArchetype = entityManager.CreateArchetype(
            position, transformMatrix, heading
            );
    }

}
