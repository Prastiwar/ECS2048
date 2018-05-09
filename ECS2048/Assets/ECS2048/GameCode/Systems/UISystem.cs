using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

[AlwaysUpdateSystem]
public class UISystem : ComponentSystem
{
    protected override void OnUpdate()
    {

    }

    public void SetupUI(System.Action playNewGame)
    {
        Bootstrap.GameSettings.PauseButton.onClick.AddListener(TogglePause);
        Parallel.ForEach(Bootstrap.GameSettings.MenuButtons, btn => btn.onClick.AddListener(RestartWorld));
        Parallel.ForEach(Bootstrap.GameSettings.QuitButtons, btn => btn.onClick.AddListener(ExitGame));
        Bootstrap.GameSettings.PlayButton.onClick.AddListener(() => OnNewGame(playNewGame));

        ActiveMenu();
    }

    private void OnNewGame(System.Action callNewGame)
    {
        callNewGame();
        Bootstrap.GameSettings.MenuCanvas.gameObject.SetActive(false);
        Bootstrap.GameSettings.HUDCanvas.gameObject.SetActive(true);
    }

    private void TogglePause()
    {
        Time.timeScale = Time.timeScale < 1 ? 1 : 0;
        Bootstrap.GameSettings.PauseCanvas.SetActive(!Bootstrap.GameSettings.PauseCanvas.activeSelf);
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
        Bootstrap.GameSettings.PauseCanvas.SetActive(false);
        Bootstrap.GameSettings.GameOverCanvas.SetActive(false);

        Bootstrap.GameSettings.MenuCanvas.SetActive(true);
    }

    private void ExitGame()
    {
        Application.Quit();
    }

}
