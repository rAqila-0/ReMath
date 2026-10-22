using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // ==================================================
    // SINGLETON
    // ==================================================

    public static GameManager Instance { get; private set; }


    // ==================================================
    // REFERENSI MANAGER
    // ==================================================

    [Header("Referensi Manager")]

    [Tooltip("PopupManager untuk menampilkan popup benar atau salah.")]
    [SerializeField]
    private PopupManager popupManager;

    [Tooltip("UIManager untuk memperbarui tampilan jawaban player.")]
    [SerializeField]
    private UIManager uiManager;


    // ==================================================
    // DATA SOAL AKTIF
    // ==================================================

    [Header("Data Soal Aktif")]

    [Tooltip("Angka pertama dari soal aktif.")]
    [SerializeField]
    private int firstNumber;

    [Tooltip("Angka kedua dari soal aktif.")]
    [SerializeField]
    private int secondNumber;

    [Tooltip("Hasil penjumlahan yang menjadi jawaban benar.")]
    [SerializeField]
    private int targetAnswer;


    // ==================================================
    // JAWABAN PLAYER
    // ==================================================

    [Header("Jawaban Player")]

    [Tooltip("Total nilai item yang sudah dimasukkan player.")]
    [SerializeField]
    private int currentAnswer = 0;


    // ==================================================
    // STATUS LEVEL
    // ==================================================

    [Header("Status Level")]

    [Tooltip("Menandakan apakah level sudah berhasil diselesaikan.")]
    [SerializeField]
    private bool levelCompleted = false;


    // ==================================================
    // AWAKE
    // ==================================================

    private void Awake()
    {
        // Cegah lebih dari satu GameManager
        // aktif dalam scene yang sama.
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    // ==================================================
    // START
    // ==================================================

    private void Start()
    {
        // Cari referensi otomatis jika belum
        // dihubungkan melalui Inspector.

        if (popupManager == null)
        {
            popupManager = FindObjectOfType<PopupManager>();
        }

        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
        }


        // Pastikan tampilan jawaban dimulai dari 0.
        UpdateAnswerUI();
    }


    // ==================================================
    // MEMULAI LEVEL
    //
    // Dipanggil oleh QuestionGenerator ketika
    // soal baru dipilih.
    // ==================================================

    public void StartLevel(
        int newFirstNumber,
        int newSecondNumber
    )
    {
        // Simpan data soal.
        firstNumber = newFirstNumber;
        secondNumber = newSecondNumber;


        // Hitung jawaban benar.
        targetAnswer =
            firstNumber + secondNumber;


        // Reset jawaban player.
        currentAnswer = 0;


        // Level belum selesai.
        levelCompleted = false;


        // Perbarui tampilan jawaban.
        UpdateAnswerUI();


        Debug.Log(
            "GameManager: Level dimulai"
            + " | Scene: "
            + SceneManager.GetActiveScene().name
            + " | Soal: "
            + firstNumber
            + " + "
            + secondNumber
            + " = "
            + targetAnswer
        );
    }


    // ==================================================
    // MENAMBAHKAN NILAI KE JAWABAN
    //
    // Dipanggil oleh DropZone ketika item berhasil
    // masuk ke AnswerContent.
    // ==================================================

    public void AddValueToAnswer(int value)
    {
        // Jangan menerima input baru jika
        // level sudah selesai.
        if (levelCompleted)
        {
            Debug.LogWarning(
                "GameManager: Level sudah selesai. "
                + "Nilai tidak dapat ditambahkan."
            );

            return;
        }


        // Validasi nilai item.
        if (!IsValidItemValue(value))
        {
            Debug.LogWarning(
                "GameManager: Nilai item tidak valid: "
                + value
                + ". Nilai yang diperbolehkan hanya 1, 10, atau 100."
            );

            return;
        }


        // Tambahkan nilai item ke jawaban.
        currentAnswer += value;


        // Update tampilan UI.
        UpdateAnswerUI();


        Debug.Log(
            "GameManager: Nilai ditambahkan"
            + " | +"
            + value
            + " | Current Answer: "
            + currentAnswer
            + " | Target: "
            + targetAnswer
        );
    }


    // ==================================================
    // MENGURANGI NILAI DARI JAWABAN
    //
    // Dipanggil oleh UndoManager.
    // ==================================================

    public void RemoveValueFromAnswer(int value)
    {
        // Jangan mengubah jawaban jika
        // level sudah selesai.
        if (levelCompleted)
        {
            Debug.LogWarning(
                "GameManager: Level sudah selesai. "
                + "Jawaban tidak dapat diubah."
            );

            return;
        }


        // Validasi nilai.
        if (!IsValidItemValue(value))
        {
            Debug.LogWarning(
                "GameManager: Nilai yang ingin dikurangi "
                + "tidak valid: "
                + value
            );

            return;
        }


        // Kurangi jawaban.
        currentAnswer -= value;


        // Pastikan tidak menjadi negatif.
        if (currentAnswer < 0)
        {
            currentAnswer = 0;
        }


        // Update UI.
        UpdateAnswerUI();


        Debug.Log(
            "GameManager: Nilai dikurangi"
            + " | -"
            + value
            + " | Current Answer: "
            + currentAnswer
        );
    }


    // ==================================================
    // RESET JAWABAN
    //
    // Dipanggil oleh UndoManager ketika Reset ditekan
    // atau ketika player mencoba soal yang sama lagi.
    // ==================================================

    public void ResetAnswer()
    {
        currentAnswer = 0;


        // Update tampilan.
        UpdateAnswerUI();


        Debug.Log(
            "GameManager: Jawaban berhasil di-reset menjadi 0."
        );
    }


    // ==================================================
    // CEK JAWABAN
    //
    // Dipanggil oleh tombol Paw / SubmitButton.
    // ==================================================

    public void CheckAnswer()
    {
        // ==============================================
        // CEGAH SUBMIT BERULANG
        // ==============================================

        if (levelCompleted)
        {
            Debug.LogWarning(
                "GameManager: Level sudah selesai. "
                + "Jawaban tidak dapat diperiksa lagi."
            );

            return;
        }


        Debug.Log(
            "GameManager: Memeriksa jawaban"
            + " | Player: "
            + currentAnswer
            + " | Target: "
            + targetAnswer
        );


        // ==============================================
        // JAWABAN BENAR
        // ==============================================

        if (currentAnswer == targetAnswer)
        {
            HandleCorrectAnswer();
        }

        // ==============================================
        // JAWABAN SALAH
        // ==============================================

        else
        {
            HandleWrongAnswer();
        }
    }


    // ==================================================
    // MENANGANI JAWABAN BENAR
    // ==================================================

    private void HandleCorrectAnswer()
    {
        // Tandai bahwa level sudah selesai.
        levelCompleted = true;


        Debug.Log(
            "JAWABAN BENAR!"
            + " | Scene: "
            + SceneManager.GetActiveScene().name
            + " | Soal: "
            + firstNumber
            + " + "
            + secondNumber
            + " = "
            + targetAnswer
            + " | Jawaban Player: "
            + currentAnswer
        );


        // ==============================================
        // MAINKAN SUARA BENAR
        // ==============================================

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayCorrectSound();
        }
        else
        {
            Debug.LogWarning(
                "GameManager: AudioManager.Instance tidak ditemukan. "
                + "Suara jawaban benar tidak dimainkan."
            );
        }


        // ==============================================
        // SIMPAN PROGRESS LEVEL
        // ==============================================

        CompleteLevelProgress();


        // ==============================================
        // TAMPILKAN POPUP BERHASIL
        // ==============================================

        if (popupManager != null)
        {
            popupManager.ShowSuccess();
        }
        else
        {
            Debug.LogWarning(
                "GameManager: PopupManager belum dihubungkan "
                + "di Inspector!"
            );
        }
    }


    // ==================================================
    // MENANGANI JAWABAN SALAH
    // ==================================================

    private void HandleWrongAnswer()
    {
        Debug.Log(
            "JAWABAN SALAH!"
            + " | Scene: "
            + SceneManager.GetActiveScene().name
            + " | Target: "
            + targetAnswer
            + " | Jawaban Player: "
            + currentAnswer
        );


        // ==============================================
        // MAINKAN SUARA SALAH
        // ==============================================

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayWrongSound();
        }
        else
        {
            Debug.LogWarning(
                "GameManager: AudioManager.Instance tidak ditemukan. "
                + "Suara jawaban salah tidak dimainkan."
            );
        }


        // ==============================================
        // TAMPILKAN POPUP GAGAL
        // ==============================================

        if (popupManager != null)
        {
            popupManager.ShowFailed();
        }
        else
        {
            Debug.LogWarning(
                "GameManager: PopupManager belum dihubungkan "
                + "di Inspector!"
            );
        }
    }


    // ==================================================
    // MENYIMPAN PROGRESS LEVEL
    //
    // Menggunakan CompleteLevel() yang memang tersedia
    // pada LevelProgressManager milik proyek Re:Math.
    // ==================================================

    private void CompleteLevelProgress()
    {
        // Pastikan LevelProgressManager tersedia.
        if (LevelProgressManager.Instance == null)
        {
            Debug.LogWarning(
                "GameManager: LevelProgressManager.Instance "
                + "tidak ditemukan. Progress level tidak disimpan."
            );

            return;
        }


        // Ambil nomor level dari nama scene.
        int currentLevel =
            GetCurrentLevelNumber();


        // Validasi nomor level.
        if (currentLevel <= 0)
        {
            Debug.LogWarning(
                "GameManager: Nomor level tidak valid. "
                + "Progress tidak dapat disimpan."
            );

            return;
        }


        // Simpan penyelesaian level pada
        // Stage Penjumlahan.
        LevelProgressManager.Instance.CompleteLevel(
            LevelManager.StageType.Penjumlahan,
            currentLevel
        );


        Debug.Log(
            "GameManager: Progress berhasil diproses"
            + " | Stage: Penjumlahan"
            + " | Level selesai: "
            + currentLevel
        );
    }


    // ==================================================
    // MENGAMBIL NOMOR LEVEL DARI NAMA SCENE
    //
    // Contoh:
    //
    // Jumlah_Level-1  -> 1
    // Jumlah_Level-5  -> 5
    // Jumlah_Level-10 -> 10
    // ==================================================

    private int GetCurrentLevelNumber()
    {
        string sceneName =
            SceneManager.GetActiveScene().name;


        int separatorIndex =
            sceneName.LastIndexOf('-');


        // Tidak menemukan karakter "-".
        if (separatorIndex < 0)
        {
            Debug.LogWarning(
                "GameManager: Format nama scene tidak sesuai: "
                + sceneName
            );

            return 0;
        }


        // Ambil teks setelah karakter "-".
        string levelNumberText =
            sceneName.Substring(
                separatorIndex + 1
            );


        // Ubah teks menjadi angka.
        if (
            int.TryParse(
                levelNumberText,
                out int levelNumber
            )
        )
        {
            return levelNumber;
        }


        Debug.LogWarning(
            "GameManager: Tidak dapat membaca nomor level "
            + "dari nama scene: "
            + sceneName
        );


        return 0;
    }


    // ==================================================
    // VALIDASI NILAI ITEM
    // ==================================================

    private bool IsValidItemValue(int value)
    {
        return
            value == 1 ||
            value == 10 ||
            value == 100;
    }


    // ==================================================
    // UPDATE TAMPILAN JAWABAN
    // ==================================================

    private void UpdateAnswerUI()
    {
        // Cari UIManager otomatis jika referensi hilang.
        if (uiManager == null)
        {
            uiManager = FindObjectOfType<UIManager>();
        }


        if (uiManager != null)
        {
            uiManager.UpdateCurrentAnswer(
                currentAnswer
            );
        }
        else
        {
            Debug.LogWarning(
                "GameManager: UIManager tidak ditemukan. "
                + "Tampilan Current Answer tidak dapat diperbarui."
            );
        }
    }


    // ==================================================
    // GETTER - CURRENT ANSWER
    // ==================================================

    public int GetCurrentAnswer()
    {
        return currentAnswer;
    }


    // ==================================================
    // GETTER - TARGET ANSWER
    // ==================================================

    public int GetTargetAnswer()
    {
        return targetAnswer;
    }


    // ==================================================
    // GETTER - FIRST NUMBER
    // ==================================================

    public int GetFirstNumber()
    {
        return firstNumber;
    }


    // ==================================================
    // GETTER - SECOND NUMBER
    // ==================================================

    public int GetSecondNumber()
    {
        return secondNumber;
    }


    // ==================================================
    // GETTER - STATUS LEVEL
    // ==================================================

    public bool IsLevelCompleted()
    {
        return levelCompleted;
    }
}