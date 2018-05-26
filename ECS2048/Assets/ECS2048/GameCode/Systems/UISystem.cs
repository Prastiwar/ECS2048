using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace TP.ECS2048
{
    [AlwaysUpdateSystem]
    public class UISystem : ComponentSystem
    {
        [Inject] private TextData data;
        [Inject] private ScoreData pData;

        protected override void OnUpdate()
        {
            for (int i = 0; i < data.Length; i++)
            {
                int index = data.Text[i].Index;
                if (index >= 0)
                {
                    // get pos for text from block
                    var pos = data.Position[i].Value;
                    if (data.Block[i].Value == 0)
                        pos.z = 1; // behind block
                    else
                        pos.z = -2; // it's in front of block

                    Bootstrap.GameSettings.BlockTexts[index].transform.position = pos;
                    Bootstrap.GameSettings.BlockTexts[index].text = data.Block[i].Value.ToString();
                }
            }
            Bootstrap.GameSettings.ScoreValueText.text = pData.ScoreHolder[0].Value.ToString();
        }

        public void Initialize(System.Action playNewGame)
        {
            Parallel.ForEach(Bootstrap.GameSettings.QuitButtons, btn => btn.onClick.AddListener(ExitGame));
            Bootstrap.GameSettings.PlayButton.onClick.AddListener(() => OnNewGame(playNewGame));

            ActiveMenu();
        }

        private void OnNewGame(System.Action callNewGame)
        {
            callNewGame();
            Bootstrap.GameSettings.MenuCanvas.gameObject.SetActive(false);
            Bootstrap.GameSettings.GameOverCanvas.gameObject.SetActive(false);
            Bootstrap.GameSettings.HUDCanvas.gameObject.SetActive(true);
        }

        private void RestartWorld()
        {
            World.Active.SetBehavioursActive(false);
            World.Active.GetExistingManager<EntityManager>().DestroyAllEntities();
            ActiveMenu();
        }

        private void ActiveMenu()
        {
            Bootstrap.GameSettings.HUDCanvas.SetActive(false);
            Bootstrap.GameSettings.GameOverCanvas.SetActive(false);

            Bootstrap.GameSettings.MenuCanvas.SetActive(true);
        }

        private void ExitGame()
        {
            Application.Quit();
        }

    }
}
