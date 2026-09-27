using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelProgressManager : MonoBehaviour
{
    public static LevelProgressManager Instance;


    // ==================================================
    // PENGATURAN
    // ==================================================

    [Header("Pengaturan Progress")]

    [Tooltip("Jumlah maksimal level dalam setiap stage.")]
    [SerializeField] private int maxLevel = 10;


    // ==================================================
    // AWAKE
    // ==================================================

    private void Awake()
    {
        // Singleton sederhana.
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // ==================================================
    // MEMBUAT KEY PLAYERPREFS
    // ==================================================

    private string GetProgressKey(
        LevelManager.StageType stage
    )
    {
        return "ReMath_UnlockedLevel_" + stage.ToString();
    }


    // ==================================================
    // MENDAPATKAN LEVEL TERTINGGI YANG TERBUKA
    // ==================================================

    public int GetUnlockedLevel(
        LevelManager.StageType stage
    )
    {
        string key = GetProgressKey(stage);

        // Default = 1
        // Artinya Level 1 selalu terbuka.
        int unlockedLevel = PlayerPrefs.GetInt(key, 1);

        return Mathf.Clamp(
            unlockedLevel,
            1,
            maxLevel
        );
    }


    // ==================================================
    // CEK APAKAH LEVEL TERBUKA
    // ==================================================

    public bool IsLevelUnlocked(
        LevelManager.StageType stage,
        int levelNumber
    )
    {
        int unlockedLevel =
            GetUnlockedLevel(stage);

        return levelNumber <= unlockedLevel;
    }


    // ==================================================
    // MENYELESAIKAN LEVEL SAAT INI
    // ==================================================

    public void CompleteCurrentLevel()
    {
        // Pastikan LevelManager tersedia.
        if (LevelManager.Instance == null)
        {
            Debug.LogError(
                "LevelManager.Instance tidak ditemukan! "
                + "Progress level tidak dapat disimpan."
            );

            return;
        }


        // Ambil informasi level dari LevelManager.
        LevelManager.StageType currentStage =
            LevelManager.Instance.GetCurrentStage();

        int currentLevel =
            LevelManager.Instance.GetCurrentLevel();


        // Simpan progress.
        CompleteLevel(
            currentStage,
            currentLevel
        );
    }


    // ==================================================
    // MENYELESAIKAN LEVEL TERTENTU
    // ==================================================

    public void CompleteLevel(
        LevelManager.StageType stage,
        int completedLevel
    )
    {
        // Validasi nomor level.
        if (completedLevel < 1 || completedLevel > maxLevel)
        {
            Debug.LogWarning(
                "Nomor level tidak valid: "
                + completedLevel
            );

            return;
        }


        int currentUnlockedLevel =
            GetUnlockedLevel(stage);


        // Jika Level 10 selesai,
        // tidak ada Level 11 yang perlu dibuka.
        if (completedLevel >= maxLevel)
        {
            Debug.Log(
                "Level terakhir selesai | Stage: "
                + stage
                + " | Level: "
                + completedLevel
            );

            return;
        }


        int nextLevel = completedLevel + 1;


        // Hanya simpan jika membuka level baru.
        if (nextLevel > currentUnlockedLevel)
        {
            string key = GetProgressKey(stage);

            PlayerPrefs.SetInt(
                key,
                nextLevel
            );

            PlayerPrefs.Save();


            Debug.Log(
                "Progress berhasil disimpan | "
                + "Stage: " + stage
                + " | Level " + completedLevel
                + " selesai"
                + " | Level " + nextLevel
                + " terbuka"
            );
        }
        else
        {
            Debug.Log(
                "Level sudah pernah diselesaikan. "
                + "Tidak ada progress baru yang perlu disimpan."
            );
        }
    }


    // ==================================================
    // RESET PROGRESS STAGE TERTENTU
    // ==================================================

    public void ResetStageProgress(
        LevelManager.StageType stage
    )
    {
        string key = GetProgressKey(stage);

        PlayerPrefs.DeleteKey(key);

        PlayerPrefs.Save();


        Debug.Log(
            "Progress stage "
            + stage
            + " berhasil di-reset."
        );
    }


    // ==================================================
    // RESET SEMUA PROGRESS RE:MATH
    // ==================================================

    public void ResetAllProgress()
    {
        foreach (
            LevelManager.StageType stage
            in System.Enum.GetValues(
                typeof(LevelManager.StageType)
            )
        )
        {
            PlayerPrefs.DeleteKey(
                GetProgressKey(stage)
            );
        }

        PlayerPrefs.Save();


        Debug.Log(
            "Semua progress Re:Math berhasil di-reset."
        );
    }


    // ==================================================
    // DEBUG PROGRESS
    // ==================================================

    public void PrintAllProgress()
    {
        foreach (
            LevelManager.StageType stage
            in System.Enum.GetValues(
                typeof(LevelManager.StageType)
            )
        )
        {
            Debug.Log(
                "Stage: "
                + stage
                + " | Level tertinggi terbuka: "
                + GetUnlockedLevel(stage)
            );
        }
    }
}
