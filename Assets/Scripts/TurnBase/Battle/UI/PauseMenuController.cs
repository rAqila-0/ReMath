
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject pauseButtonObject;

    [Header("Battle Reference")]
    [SerializeField] private BattleManager battleManager;

    [Header("Stage Navigation")]
    [SerializeField] private string stageSelectionSceneName;

    private bool isPaused = false;

    private void Awake()
    {
        // Pastikan scene selalu dimulai dalam kondisi normal.
        Time.timeScale = 1f;
        isPaused = false;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
        else
        {
            Debug.LogError(
                "PauseMenuController: PausePanel belum diisi.",
                this
            );
        }
    }

    public void PauseGame()
    {
        if (isPaused)
            return;

        if (pausePanel == null)
        {
            Debug.LogError(
                "PauseMenuController: PausePanel tidak ditemukan.",
                this
            );
            return;
        }

        isPaused = true;

        // Menghentikan waktu game.
        Time.timeScale = 0f;

        pausePanel.SetActive(true);

        if (pauseButtonObject != null)
        {
            pauseButtonObject.SetActive(false);
        }
    }

    public void ResumeGame()
    {
        isPaused = false;

        // Melanjutkan waktu game.
        Time.timeScale = 1f;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (pauseButtonObject != null)
        {
            pauseButtonObject.SetActive(true);
        }
    }

    public void RetryBattle()
    {
        // Wajib melanjutkan waktu sebelum reset battle.
        Time.timeScale = 1f;
        isPaused = false;

        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        if (pauseButtonObject != null)
        {
            pauseButtonObject.SetActive(true);
        }

        if (battleManager == null)
        {
            Debug.LogError(
                "PauseMenuController: BattleManager belum diisi.",
                this
            );
            return;
        }

        // Gunakan sistem Retry yang sudah ada.
        battleManager.RetryBattle();
    }

    public void BackToStage()
    {
        if (string.IsNullOrWhiteSpace(stageSelectionSceneName))
        {
            Debug.LogError(
                "PauseMenuController: Nama scene Stage belum diisi.",
                this
            );

            ResumeGame();
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(
            stageSelectionSceneName))
        {
            Debug.LogError(
                "Scene tidak ditemukan di Build Settings: " +
                stageSelectionSceneName,
                this
            );

            ResumeGame();
            return;
        }

        // Pastikan scene berikutnya tidak ikut dalam kondisi pause.
        Time.timeScale = 1f;
        isPaused = false;

        SceneManager.LoadScene(stageSelectionSceneName);
    }

    private void OnDestroy()
    {
        // Pengaman agar scene berikutnya tidak ikut membeku.
        Time.timeScale = 1f;
    }
}
