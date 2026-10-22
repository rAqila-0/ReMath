using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;


    // ==================================================
    // TIPE STAGE
    // ==================================================

    public enum StageType
    {
        Penjumlahan,
        Pengurangan,
        Perkalian,
        Pembagian,
        Campuran
    }


    [Header("Informasi Level")]

    [Tooltip("Stage yang sedang dimainkan.")]
    [SerializeField] private StageType currentStage;

    [Tooltip("Nomor level yang sedang dimainkan.")]
    [SerializeField] private int currentLevel = 1;


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
            return;
        }

        // Baca stage dan nomor level dari nama scene.
        DetectLevelFromScene();
    }


    // ==================================================
    // MENDETEKSI STAGE DAN LEVEL DARI NAMA SCENE
    // ==================================================

    private void DetectLevelFromScene()
    {
        string sceneName =
            SceneManager.GetActiveScene().name;


        // ==============================================
        // DETEKSI STAGE
        // ==============================================

        if (sceneName.StartsWith("Jumlah_Level-"))
        {
            currentStage = StageType.Penjumlahan;
        }
        else if (sceneName.StartsWith("Kurang_Level-"))
        {
            currentStage = StageType.Pengurangan;
        }
        else if (sceneName.StartsWith("Kali_Level-"))
        {
            currentStage = StageType.Perkalian;
        }
        else if (sceneName.StartsWith("Bagi_Level-"))
        {
            currentStage = StageType.Pembagian;
        }
        else if (sceneName.StartsWith("Campuran_Level-"))
        {
            currentStage = StageType.Campuran;
        }
        else
        {
            Debug.LogWarning(
                "LevelManager tidak mengenali stage dari scene: "
                + sceneName
            );

            return;
        }


        // ==============================================
        // DETEKSI NOMOR LEVEL
        // ==============================================

        int separatorIndex =
            sceneName.LastIndexOf('-');

        if (separatorIndex < 0)
        {
            Debug.LogError(
                "Format nama scene tidak valid: "
                + sceneName
            );

            return;
        }


        string levelText =
            sceneName.Substring(separatorIndex + 1);


        if (int.TryParse(levelText, out int detectedLevel))
        {
            currentLevel = detectedLevel;

            Debug.Log(
                "Level berhasil dideteksi | "
                + "Stage: " + currentStage
                + " | Level: " + currentLevel
                + " | Scene: " + sceneName
            );
        }
        else
        {
            Debug.LogError(
                "Nomor level gagal dibaca dari scene: "
                + sceneName
            );
        }
    }


    // ==================================================
    // MENGAMBIL STAGE SAAT INI
    // ==================================================

    public StageType GetCurrentStage()
    {
        return currentStage;
    }


    // ==================================================
    // MENGAMBIL NOMOR LEVEL SAAT INI
    // ==================================================

    public int GetCurrentLevel()
    {
        return currentLevel;
    }


    // ==================================================
    // MENGAMBIL NAMA STAGE
    // ==================================================

    public string GetStageName()
    {
        return currentStage.ToString();
    }


    // ==================================================
    // MENGAMBIL NAMA SCENE SAAT INI
    // ==================================================

    public string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }
}
