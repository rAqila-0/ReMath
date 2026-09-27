using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [Header("Panel Tutorial")]
    [Tooltip("Panel UI yang berisi petunjuk cara menyelesaikan level.")]
    [SerializeField] private GameObject tutorialPanel;

    [Header("Referensi Manager")]
    [Tooltip("Referensi ke PauseManager agar status pause tidak bentrok.")]
    [SerializeField] private PauseManager pauseManager;

    // Menyimpan status tutorial.
    private bool isTutorialOpen = false;


    // ==================================================
    // START
    // ==================================================

    private void Start()
    {
        isTutorialOpen = false;

        // Pastikan panel tutorial tertutup saat level dimulai.
        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }
    }


    // ==================================================
    // MEMBUKA TUTORIAL
    // ==================================================

    public void OpenTutorial()
    {
        // Jangan buka lagi jika sudah terbuka.
        if (isTutorialOpen)
            return;

        isTutorialOpen = true;

        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning(
                "Tutorial Panel belum dihubungkan ke TutorialManager!"
            );

            return;
        }

        // Hentikan waktu game sementara.
        Time.timeScale = 0f;

        Debug.Log("Tutorial dibuka.");
    }


    // ==================================================
    // MENUTUP TUTORIAL
    // ==================================================

    public void CloseTutorial()
    {
        if (!isTutorialOpen)
            return;

        isTutorialOpen = false;

        if (tutorialPanel != null)
        {
            tutorialPanel.SetActive(false);
        }

        // Jika PauseManager ada dan game sedang di-pause,
        // jangan lanjutkan waktu game.
        if (pauseManager != null && pauseManager.IsPaused())
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }

        Debug.Log("Tutorial ditutup.");
    }


    // ==================================================
    // TOGGLE TUTORIAL
    // ==================================================

    public void ToggleTutorial()
    {
        if (isTutorialOpen)
        {
            CloseTutorial();
        }
        else
        {
            OpenTutorial();
        }
    }


    // ==================================================
    // STATUS TUTORIAL
    // ==================================================

    public bool IsTutorialOpen()
    {
        return isTutorialOpen;
    }


    // ==================================================
    // SAFETY
    // ==================================================

    private void OnDisable()
    {
        isTutorialOpen = false;
    }
}