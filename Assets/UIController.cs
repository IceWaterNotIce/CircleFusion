using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public TextMeshProUGUI lblScore; // 用於顯示分數的 TextMeshPro 元件
    public TextMeshProUGUI lblGameOver; // 用於顯示遊戲結束訊息的 TextMeshPro 元件
    public Button btnGameRestart; // 用於重新開始遊戲的按鈕
    public GameObject panelGameOver; // 包含 lblGameOver 與 btnGameRestart 的面板

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (panelGameOver != null)
            panelGameOver.SetActive(false);
        SetScore(0);
        btnGameRestart.onClick.AddListener(OnRestartClicked);
    }

    void OnEnable()
    {
        GameManager.OnGameOver += ShowGameOver;
        GameManager.OnScoreChanged += SetScore;
    }

    void OnDisable()
    {
        GameManager.OnGameOver -= ShowGameOver;
        GameManager.OnScoreChanged -= SetScore;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetScore(int score)
    {
        if (lblScore != null)
            lblScore.text = $"Score: {score}";
    }

    public void ShowGameOver()
    {
        if (panelGameOver != null)
            panelGameOver.SetActive(true);
    }

    public void HideGameOver()
    {
        if (panelGameOver != null)
            panelGameOver.SetActive(false);
    }

    public void OnRestartClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }
}
