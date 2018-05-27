using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;
using Unity.Collections;
using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

namespace TP.ECS2048
{
    public class Bootstrap : MonoBehaviour
    {
        public static GameSettings GameSettings { get; private set; }

        public static EntityArchetype PlayerArchetype { get; private set; }
        public static EntityArchetype BlockArchetype { get; private set; }
        public static EntityArchetype FloorArchetype { get; private set; }
        
        public static MeshInstanceRenderer FloorLook { get; private set; }

        private static WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

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
            FloorLook = Utils.GetLookFromPrototype("FloorLook");

            World.Active.GetExistingManager<UISystem>().Initialize(NewGame);
            World.Active.GetExistingManager<SpawnSystem>().Initialize(GameSettings.GridSize.x * GameSettings.GridSize.y);
        }

        public static void NewGame()
        {
            var entityManager = World.Active.GetOrCreateManager<EntityManager>();

            GameSettings.MenuCanvas.gameObject.SetActive(false);
            GameSettings.GameOverCanvas.gameObject.SetActive(false);
            GameSettings.HUDCanvas.gameObject.SetActive(true);
            entityManager.DestroyAllEntities();

            CreatePlayer(entityManager);
            CreateTextMeshPros();
            CreatePlane(entityManager);

            World.Active.SetBehavioursActive(true);
            Time.timeScale = 1;
            GameSettings.StartCoroutine(InitializeMoveSystem());
        }

        public static void GameOver()
        {
            Time.timeScale = 0;
            GameSettings.GameOverCanvas.SetActive(true);
            World.Active.SetBehavioursActive(false, typeof(SpawnSystem), typeof(InputSystem), typeof(SpawnSystem));
            GameSettings.StartCoroutine(WaitForPlayAgain());
        }

        private static IEnumerator WaitForPlayAgain()
        {
            yield return waitForEndOfFrame;
            while (!UnityEngine.Input.GetMouseButtonDown(0))
            {
                yield return null;
            }
            World.Active.SetBehavioursActive(false);
            NewGame();
        }

        private static IEnumerator InitializeMoveSystem()
        {
            yield return waitForEndOfFrame;
            World.Active.GetExistingManager<MoveSystem>().Initialize();
        }

        private static void CreateBlock(EntityManager em, float3 pos, int2 posIndex, int baseValue, Entity block = default(Entity))
        {
            if (block == default(Entity))
                block = em.CreateEntity(BlockArchetype);

            em.SetComponentData(block, new Position { Value = pos });
            em.SetComponentData(block, new Heading { Value = new float3(0.0f, 1.0f, 0) });
            em.SetComponentData(block, new TextUI { Index = GetFreeTextMeshIndex() });
            em.SetComponentData(block, new Block { PosIndex = posIndex, Value = baseValue, Changed = true });
            em.AddSharedComponentData(block, Utils.NewMeshInstanceRenderer(PrimitiveType.Cube));
        }

        private static void CreatePlayer(EntityManager em)
        {
            var player = em.CreateEntity(PlayerArchetype);
            em.SetComponentData(player, new ScoreHolder() { Value = 0 });
        }

        private static void CreatePlane(EntityManager em)
        {
            int2 randIndex = GetRandomPosOnGrid();
            int2 randIndex2 = GetRandomPosOnGrid();
            Heading defaultHeading = new Heading() { Value = new float3(0.0f, 1.0f, 0) };
            NativeArray<Entity> newEntities = new NativeArray<Entity>(GameSettings.GridSize.x * GameSettings.GridSize.y, Allocator.Temp);
            em.CreateEntity(FloorArchetype, newEntities);

            int index = 0;
            for (int x = 0; x < GameSettings.GridSize.x; x++)
            {
                var gapX = (x * GameSettings.GridGap) - (GameSettings.GridSize.x / 2);
                for (int y = 0; y < GameSettings.GridSize.y; y++)
                {
                    var entity = newEntities[index];
                    var gapY = y * GameSettings.GridGap - (GameSettings.GridSize.y / 2);
                    float3 pos = new float3(x + gapX, y + gapY, 0);
                    int2 gridIndex = new int2(x, y);

                    em.SetComponentData(entity, new Position { Value = pos });
                    em.SetComponentData(entity, defaultHeading);
                    em.AddSharedComponentData(entity, FloorLook);

                    CreateBlock(em, pos, gridIndex, ShouldMakeItStartBlock(gridIndex, randIndex, randIndex2) ? 2 : 0);
                    index++;
                }
            }
            newEntities.Dispose();
        }

        private static bool ShouldMakeItStartBlock(int2 actualIndex, params int2[] randomIndexes)
        {
            int length = randomIndexes.Length;
            for (int i = 0; i < length; i++)
            {
                if (actualIndex.Equals(randomIndexes[i]))
                    return true;
            }
            return false;
        }

        private static void CreateTextMeshPros()
        {
            for (int i = 0; i < GameSettings.BlockTexts.Count; i++)
            {
                Destroy(GameSettings.BlockTexts[i].gameObject);
            }
            GameSettings.BlockTexts.Clear();

            int length = GameSettings.GridSize.x * GameSettings.GridSize.y;
            for (int i = 0; i < length; i++)
            {
                GameSettings.BlockTexts.Add(CreateTextMeshPro());
            }
        }

        private static int GetFreeTextMeshIndex()
        {
            int length = GameSettings.BlockTexts.Count;
            for (int i = 0; i < length; i++)
            {
                if (!GameSettings.BlockTexts[i].isActiveAndEnabled)
                {
                    GameSettings.BlockTexts[i].gameObject.SetActive(true);
                    return i;
                }
            }
            return -1;
        }

        private static TextMeshPro CreateTextMeshPro()
        {
            GameObject newTextObj = new GameObject("Block Text", typeof(TextMeshPro)) { isStatic = true };
            TextMeshPro textMesh = newTextObj.GetComponent<TextMeshPro>();
            textMesh.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1);
            textMesh.fontSize = 5;
            textMesh.alignment = TextAlignmentOptions.Center;
            textMesh.margin = Vector4.zero;
            textMesh.color = Color.white;
            newTextObj.SetActive(false);
            return textMesh;
        }

        private static int2 GetRandomPosOnGrid()
        {
            int x = Random.Range(0, GameSettings.GridSize.x);
            int y = Random.Range(0, GameSettings.GridSize.y);
            return new int2(x, y);
        }

        private static void SetArchetypes(EntityManager entityManager)
        {
            var scoreHolder = ComponentType.Create<ScoreHolder>();
            var textUI = ComponentType.Create<TextUI>();
            var input = ComponentType.Create<Input>();

            var playerTag = ComponentType.Create<Player>();
            var blockTag = ComponentType.Create<Block>();

            var heading = ComponentType.Create<Heading>();
            var position = ComponentType.Create<Position>();
            var transformMatrix = ComponentType.Create<TransformMatrix>();

            PlayerArchetype = entityManager.CreateArchetype(
                playerTag, scoreHolder, input
                );

            BlockArchetype = entityManager.CreateArchetype(
                position, transformMatrix, heading, blockTag, textUI
                );

            FloorArchetype = entityManager.CreateArchetype(
                position, transformMatrix, heading
                );
        }

    }
}
