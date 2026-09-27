using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    [Header("Panel Pause")]
    [Tooltip("Panel UI yang ditampilkan ketika game di-pause.")]
    [SerializeField] private GameObject pausePanel;

    [Header("Pengaturan Scene")]
    [Tooltip("Nama scene daftar level stage penjumlahan.")]
    [SerializeField] private string levelMenuSceneName = "StagePerjumlahan";

    // Menyimpan status pause.
    private bool isPaused = false;


    // ==================================================
    // START
    // ==================================================

    private void Start()
    {
        // Pastikan game berjalan normal saat scene dimulai.
        Time.timeScale = 1f;

        isPaused = false;

        // Pastikan panel pause tertutup saat game dimulai.
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }
    }


    // ==================================================
    // MEMBUKA PAUSE
    // ==================================================

    public void PauseGame()
    {
        if (isPaused)
            return;

        isPaused = true;

        // Tampilkan panel pause.
        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning(
                "Pause Panel belum dihubungkan ke PauseManager!"
            );
        }

        // Hentikan waktu game.
        Time.timeScale = 0f;

        Debug.Log("Game di-pause.");
    }


    // ==================================================
    // MELANJUTKAN GAME
    // ==================================================

    public void ResumeGame()
    {
        if (!isPaused)
            return;

        isPaused = false;

        // Kembalikan waktu game ke normal.
        Time.timeScale = 1f;

        // Tutup panel pause.
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        Debug.Log("Game dilanjutkan.");
    }


    // ==================================================
    // TOGGLE PAUSE
    // ==================================================

    public void TogglePause()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }


    // ==================================================
    // RESTART LEVEL
    // ==================================================

    public void RestartLevel()
    {
        // Sangat penting:
        // kembalikan Time Scale sebelum pindah scene.
        Time.timeScale = 1f;

        Scene currentScene = SceneManager.GetActiveScene();

        SceneManager.LoadScene(currentScene.name);
    }


    // ==================================================
    // KEMBALI KE MENU DAFTAR LEVEL
    // ==================================================

    public void BackToLevelMenu()
    {
        // Pastikan game tidak tetap ter-pause
        // setelah berpindah scene.
        Time.timeScale = 1f;

        SceneManager.LoadScene(levelMenuSceneName);
    }


    // ==================================================
    // STATUS PAUSE
    // ==================================================

    public bool IsPaused()
    {
        return isPaused;
    }


    // ==================================================
    // SAFETY SAAT OBJECT DINONAKTIFKAN
    // ==================================================

    private void OnDisable()
    {
        // Mencegah Time.timeScale tetap 0
        // jika object atau scene ditutup saat pause.
        Time.timeScale = 1f;
    }
}