using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI")]
    public GameObject winPanel;                // 最初は inactive にしておく
    public TextMeshProUGUI winnerText;         // 中央に出すテキスト
    public Button retryButton;                 // リトライボタン（任意）
    public Button titleButton;                 // タイトルに戻る（任意）

    [Header("設定")]
    public float autoRestartDelay = 0f;        // 秒（0以下で自動リスタートしない）
    public string titleSceneName = "";         // タイトルシーン名（未設定で無視）

    [Header("表示色")]
    public Color colorP1 = new Color(0.0f, 0.5f, 1.0f);
    public Color colorP2 = new Color(1.0f, 0.2f, 0.2f);

    bool matchEnded = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if (winPanel != null) winPanel.SetActive(false);

        if (retryButton != null) retryButton.onClick.AddListener(OnRetryClicked);
        if (titleButton != null) titleButton.onClick.AddListener(OnTitleClicked);
    }

    // 呼び出し：敗者IDを渡す（1 or 2）
    public void PlayerLose(int loserId)
    {
        if (matchEnded) return;
        matchEnded = true;

        int winnerId = 3 - loserId;
        Debug.Log($"Player{loserId} LOSE. Player{winnerId} WIN!");

        ShowWinUI(winnerId);

        // ゲーム停止（操作を止めたいならタイムスケールを0にする）
        Time.timeScale = 0f;

        if (autoRestartDelay > 0f)
        {
            StartCoroutine(AutoRestartUnscaled(autoRestartDelay));
        }
    }

    void ShowWinUI(int winnerId)
    {
        if (winPanel != null) winPanel.SetActive(true);

        if (winnerText != null)
        {
            winnerText.text = $"PLAYER {winnerId} WINS!";
            winnerText.color = (winnerId == 1) ? colorP1 : colorP2;
            StartCoroutine(DoPopInAnimation(winnerText.rectTransform));
        }
    }

    IEnumerator DoPopInAnimation(RectTransform rt)
    {
        if (rt == null) yield break;
        float t = 0f;
        float dur = 0.28f;
        Vector3 from = Vector3.one * 0.6f;
        Vector3 to = Vector3.one * 1.05f;
        rt.localScale = from;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.SmoothStep(0f, 1f, t / dur);
            rt.localScale = Vector3.Lerp(from, to, k);
            yield return null;
        }
        t = 0f;
        dur = 0.12f;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            rt.localScale = Vector3.Lerp(to, Vector3.one, t / dur);
            yield return null;
        }
        rt.localScale = Vector3.one;
    }

    IEnumerator AutoRestartUnscaled(float delay)
    {
        float timer = 0f;
        while (timer < delay)
        {
            timer += Time.unscaledDeltaTime;
            yield return null;
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnRetryClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnTitleClicked()
    {
        Time.timeScale = 1f;
        if (!string.IsNullOrEmpty(titleSceneName)) SceneManager.LoadScene(titleSceneName);
        else Debug.LogWarning("titleSceneName が設定されていません。");
    }
}
