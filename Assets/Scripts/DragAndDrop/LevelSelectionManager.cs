using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectionManager : MonoBehaviour
{
    // ==================================================
    // DATA TOMBOL LEVEL
    // ==================================================

    [System.Serializable]
    public class LevelButtonData
    {
        [Tooltip("Nomor level dari tombol ini.")]
        public int levelNumber;

        [Tooltip("Komponen Button untuk membuka level.")]
        public Button levelButton;

        [Tooltip("Icon gembok ketika level masih terkunci.")]
        public GameObject lockIcon;
    }


    // ==================================================
    // PENGATURAN STAGE
    // ==================================================

    [Header("Pengaturan Stage")]

    [Tooltip("Stage yang ditampilkan oleh scene daftar level ini.")]
    [SerializeField]
    private LevelManager.StageType stageType =
        LevelManager.StageType.Penjumlahan;


    [Tooltip("Prefix nama scene level.")]
    [SerializeField]
    private string levelScenePrefix = "Jumlah_Level-";


    // ==================================================
    // DAFTAR TOMBOL LEVEL
    // ==================================================

    [Header("Daftar Tombol Level")]

    [Tooltip("Daftar tombol Level 1 sampai Level 10.")]
    [SerializeField]
    private LevelButtonData[] levelButtons;


    // ==================================================
    // START
    // ==================================================

    private void Start()
    {
        RefreshLevelButtons();
    }


    // ==================================================
    // REFRESH STATUS SEMUA TOMBOL
    // ==================================================

    public void RefreshLevelButtons()
    {
        if (LevelProgressManager.Instance == null)
        {
            Debug.LogError(
                "LevelProgressManager.Instance tidak ditemukan! "
                + "Status tombol level tidak dapat diperbarui."
            );

            return;
        }


        // Ambil level tertinggi yang sudah terbuka.
        int highestUnlockedLevel =
            LevelProgressManager.Instance
                .GetUnlockedLevel(stageType);


        Debug.Log(
            "Memperbarui tombol level | Stage: "
            + stageType
            + " | Level tertinggi terbuka: "
            + highestUnlockedLevel
        );


        // ==============================================
        // UPDATE SETIAP TOMBOL
        // ==============================================

        foreach (LevelButtonData data in levelButtons)
        {
            if (data == null)
                continue;


            bool isUnlocked =
                data.levelNumber <= highestUnlockedLevel;


            // ------------------------------------------
            // Atur apakah tombol dapat ditekan.
            // ------------------------------------------

            if (data.levelButton != null)
            {
                data.levelButton.interactable = isUnlocked;
            }


            // ------------------------------------------
            // Tampilkan/sembunyikan icon gembok.
            // ------------------------------------------

            if (data.lockIcon != null)
            {
                data.lockIcon.SetActive(!isUnlocked);
            }
        }
    }


    // ==================================================
    // MEMBUKA LEVEL
    // ==================================================

    public void OpenLevel(int levelNumber)
    {
        // Pastikan Progress Manager tersedia.
        if (LevelProgressManager.Instance == null)
        {
            Debug.LogError(
                "LevelProgressManager.Instance tidak ditemukan!"
            );

            return;
        }


        // ==============================================
        // CEK APAKAH LEVEL SUDAH TERBUKA
        // ==============================================

        bool isUnlocked =
            LevelProgressManager.Instance
                .IsLevelUnlocked(
                    stageType,
                    levelNumber
                );


        if (!isUnlocked)
        {
            Debug.LogWarning(
                "Level "
                + levelNumber
                + " masih terkunci!"
            );

            return;
        }


        // ==============================================
        // BUAT NAMA SCENE
        // ==============================================

        string sceneName =
            levelScenePrefix + levelNumber;


        Debug.Log(
            "Membuka scene: "
            + sceneName
        );


        // ==============================================
        // PINDAH SCENE
        // ==============================================

        SceneManager.LoadScene(sceneName);
    }
}
