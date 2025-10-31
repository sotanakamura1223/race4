using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI")]
    public GameObject winPanel;                // �ŏ��� inactive �ɂ��Ă���
    public TextMeshProUGUI winnerText;         // �����ɏo���e�L�X�g
    public Button retryButton;                 // ���g���C�{�^���i�C�Ӂj
    public Button titleButton;                 // �^�C�g���ɖ߂�i�C�Ӂj

    [Header("�ݒ�")]
    public float autoRestartDelay = 0f;        // �b�i0�ȉ��Ŏ������X�^�[�g���Ȃ��j
    public string titleSceneName = "";         // �^�C�g���V�[�����i���ݒ�Ŗ����j

    [Header("�\���F")]
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

    // �Ăяo���F�s��ID��n���i1 or 2�j
    public void PlayerLose(int loserId)
    {
        if (matchEnded) return;
        matchEnded = true;

        int winnerId = 3 - loserId;
        Debug.Log($"Player{loserId} LOSE. Player{winnerId} WIN!");

        ShowWinUI(winnerId);

        // �Q�[����~�i������~�߂����Ȃ�^�C���X�P�[����0�ɂ���j
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
        else Debug.LogWarning("titleSceneName ���ݒ肳��Ă��܂���B");
    }
}
