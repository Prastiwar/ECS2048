using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class GameSettings : MonoBehaviour
{
    public List<TextMeshPro> BlockTexts = new List<TextMeshPro>();

    [Header("Hud References")]
    public TextMeshProUGUI ScoreText;
    [Header("Canvas References")]
    public GameObject HUDCanvas;
    public GameObject PauseCanvas;
    public GameObject MenuCanvas;
    public GameObject GameOverCanvas;
    [Header("Button References")]
    public Button PlayButton;
    public Button PauseButton;
    public Button[] QuitButtons;
    public Button[] MenuButtons;
    [Header("Settings")]
    public int2 GridSize;
    public float GridGap;
}
