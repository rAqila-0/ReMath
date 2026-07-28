using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class PopupManager : MonoBehaviour
{
    // ==================================================
    // PANEL POPUP
    // ==================================================

    [Header("Panel Popup")]

    [Tooltip("Panel yang muncul ketika jawaban benar.")]
    [SerializeField]
    private GameObject successPopup;

    [Tooltip("Panel yang muncul ketika jawaban salah.")]
    [SerializeField]
    private GameObject failedPopup;


    // ==================================================
    // REFERENSI MANAGER
    // ==================================================

    [Header("Referensi Manager")]

    [Tooltip("UndoManager untuk mereset semua item jawaban saat Coba Lagi.")]
    [SerializeField]
    private UndoManager undoManager;


    // ==================================================
    // TEKS OPSIONAL
    // ==================================================

    [Header("Teks Popup - Opsional")]

    [Tooltip("Teks judul pada popup berhasil. Boleh dikosongkan.")]
    [SerializeField]
    private TMP_Text successTitleText;

    [Tooltip("Teks hasil soal pada popup berhasil. Boleh dikosongkan.")]
    [SerializeField]
    private TMP_Text successAnswerText;

    [Tooltip("Teks pada popup gagal. Boleh dikosongkan.")]
    [SerializeField]
    private TMP_Text failedMessageText;


    // ==================================================
    // PENGATURAN SCENE
    // ==================================================

    [Header("Pengaturan Scene")]

    [Tooltip("Nama scene menu daftar level stage penjumlahan.")]
    [SerializeField]
    private string levelSelectionScene = "StagePerjumlahan";


    // ==================================================
    // AWAKE
    // ==================================================

    private void Awake()
    {
        // Pastikan popup tidak muncul saat level dimulai.
        if (successPopup != null)
        {
            successPopup.SetActive(false);
        }

        if (failedPopup != null)
        {
            failedPopup.SetActive(false);
        }
    }


    // ==================================================
    // MENAMPILKAN POPUP BERHASIL
    // ==================================================

    public void ShowSuccess()
    {
        if (failedPopup != null)
        {
            failedPopup.SetActive(false);
        }


        // Update teks popup jika referensi tersedia.
        if (successTitleText != null)
        {
            successTitleText.text = "Level Selesai!";
        }


        if (
            successAnswerText != null &&
            GameManager.Instance != null
        )
        {
            int firstNumber =
                GameManager.Instance.GetFirstNumber();

            int secondNumber =
                GameManager.Instance.GetSecondNumber();

            int answer =
                GameManager.Instance.GetTargetAnswer();


            successAnswerText.text =
                firstNumber
                + " + "
                + secondNumber
                + " = "
                + answer;
        }


        // Tampilkan popup berhasil.
        if (successPopup != null)
        {
            successPopup.SetActive(true);
        }
        else
        {
            Debug.LogWarning(
                "PopupManager: SuccessPopup belum dihubungkan di Inspector!"
            );
        }


        Debug.Log(
            "Popup berhasil ditampilkan."
        );
    }


    // ==================================================
    // MENAMPILKAN POPUP GAGAL
    // ==================================================

    public void ShowFailed()
    {
        if (successPopup != null)
        {
            successPopup.SetActive(false);
        }


        // Jangan tampilkan jawaban yang benar,
        // supaya player tetap mencoba menyelesaikan soal.
        if (failedMessageText != null)
        {
            failedMessageText.text =
                "Jawaban belum tepat.\nCoba lagi, ya!";
        }


        if (failedPopup != null)
        {
            failedPopup.SetActive(true);
        }
        else
        {
            Debug.LogWarning(
                "PopupManager: FailedPopup belum dihubungkan di Inspector!"
            );
        }


        Debug.Log(
            "Popup jawaban salah ditampilkan."
        );
    }


    // ==================================================
    // COBA LAGI
    // ==================================================

    public void RetrySameQuestion()
    {
        // Tutup popup gagal terlebih dahulu.
        if (failedPopup != null)
        {
            failedPopup.SetActive(false);
        }


        // Reset seluruh item yang sudah dimasukkan
        // tanpa membuat soal baru.
        if (undoManager != null)
        {
            undoManager.ResetAllItems();
        }
        else
        {
            Debug.LogWarning(
                "PopupManager: UndoManager belum dihubungkan. "
                + "Jawaban tidak dapat di-reset otomatis."
            );


            // Fallback: minimal reset nilai jawaban.
            if (GameManager.Instance != null)
            {
                GameManager.Instance.ResetAnswer();
            }
        }


        Debug.Log(
            "Coba lagi: jawaban di-reset dan soal yang sama dipertahankan."
        );
    }

    // ==================================================
    // MENGAMBIL NOMOR LEVEL DARI NAMA SCENE
    // ==================================================

    private int GetCurrentLevelNumber()
    {
        string sceneName =
            SceneManager.GetActiveScene().name;

        // Contoh nama scene:
        // Jumlah_Level-1
        // Jumlah_Level-2
        // Jumlah_Level-10

        int separatorIndex =
            sceneName.LastIndexOf('-');

        if (separatorIndex >= 0)
        {
            string levelNumberText =
                sceneName.Substring(separatorIndex + 1);

            if (int.TryParse(
                levelNumberText,
                out int levelNumber
            ))
            {
                return levelNumber;
            }
        }

        Debug.LogWarning(
            "PopupManager: Tidak dapat membaca nomor level "
            + "dari nama scene: "
            + sceneName
        );

        return 0;
    }


    // ==================================================
    // LANJUT KE LEVEL BERIKUTNYA
    // ==================================================

    public void NextLevel()
    {
        string currentSceneName =
            SceneManager.GetActiveScene().name;

        int currentLevel =
            GetCurrentLevelNumber();


        // ==============================================
        // VALIDASI NOMOR LEVEL
        // ==============================================

        if (currentLevel <= 0)
        {
            Debug.LogError(
                "PopupManager: Nomor level tidak valid. "
                + "Scene aktif: "
                + currentSceneName
            );

            return;
        }


        // ==============================================
        // JIKA LEVEL 10 SELESAI,
        // KEMBALI KE DAFTAR LEVEL
        // ==============================================

        if (currentLevel >= 10)
        {
            Debug.Log(
                "Level 10 selesai. Kembali ke "
                + levelSelectionScene
            );

            SceneManager.LoadScene(
                levelSelectionScene
            );

            return;
        }


        // ==============================================
        // AMBIL PREFIX NAMA SCENE
        // ==============================================

        int separatorIndex =
            currentSceneName.LastIndexOf('-');


        if (separatorIndex < 0)
        {
            Debug.LogError(
                "PopupManager: Format nama scene tidak valid: "
                + currentSceneName
            );

            return;
        }


        string scenePrefix =
            currentSceneName.Substring(
                0,
                separatorIndex + 1
            );


        // ==============================================
        // BUAT NAMA SCENE LEVEL BERIKUTNYA
        // ==============================================

        string nextSceneName =
            scenePrefix
            + (currentLevel + 1);


        Debug.Log(
            "Pindah dari "
            + currentSceneName
            + " ke "
            + nextSceneName
        );


        // ==============================================
        // PINDAH KE LEVEL BERIKUTNYA
        // ==============================================

        SceneManager.LoadScene(
            nextSceneName
        );
    }


    // ==================================================
    // KEMBALI KE MENU DAFTAR LEVEL
    // ==================================================

    public void BackToLevelSelection()
    {
        if (string.IsNullOrEmpty(levelSelectionScene))
        {
            Debug.LogError(
                "PopupManager: Nama scene level selection masih kosong!"
            );

            return;
        }


        SceneManager.LoadScene(
            levelSelectionScene
        );
    }


    // ==================================================
    // MENUTUP POPUP BERHASIL
    // ==================================================

    public void CloseSuccessPopup()
    {
        if (successPopup != null)
        {
            successPopup.SetActive(false);
        }
    }


    // ==================================================
    // MENUTUP POPUP GAGAL
    // ==================================================

    public void CloseFailedPopup()
    {
        if (failedPopup != null)
        {
            failedPopup.SetActive(false);
        }
    }
}