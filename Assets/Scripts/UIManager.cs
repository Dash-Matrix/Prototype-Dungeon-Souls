using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public TextMeshProUGUI enemyKillText;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public TMP_Text subtitleText;         // Or use Text if not using TextMeshPro
    public RectTransform restartButton;   // Button RectTransform

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        UpdateEnemyKillUI(0);
        gameOverPanel.SetActive(false);
    }

    public void UpdateEnemyKillUI(int count)
    {
        enemyKillText.text = "Enemies Killed: " + count;
    }
    public void ShowGameOver()
    {
        gameOverPanel.gameObject.SetActive(true);

        // Fade in subtitle text
        subtitleText.alpha = 0;
        subtitleText.DOFade(1f, 0.5f).SetDelay(0.1f);

        // Scale button horizontally from 0 to full width
        restartButton.localScale = new Vector3(0f, 1f, 1f);
        restartButton.DOScaleX(1f, 0.2f).SetEase(Ease.OutBack).SetDelay(0.2f);
    }
}
