using System.Collections.Generic;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace TP.ECS2048
{
    public class GameSettings : MonoBehaviour
    {
        public List<TextMeshPro> BlockTexts = new List<TextMeshPro>();

        [Header("Block Look")]
        public Material MaterialToCopy;
        public List<Color> BlockColors = new List<Color>();
        [Header("Hud References")]
        public TextMeshProUGUI ScoreValueText;
        [Header("Canvas References")]
        public GameObject HUDCanvas;
        public GameObject MenuCanvas;
        public GameObject GameOverCanvas;
        [Header("Button References")]
        public Button PlayButton;
        public Button[] QuitButtons;
        [Header("Settings")]
        public int2 GridSize;
        public float GridGap;
    }
}
