using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class BattleResultUI : MonoBehaviour
{
    //==================================================
    // REFERENCES
    //==================================================

    [Header("References")]

    [SerializeField]
    private BattleManager battleManager;

    [SerializeField]
    private BattleUIManager battleUI;


    //==================================================
    // LEVEL INFORMATION
    //==================================================

    [Header("Level Information")]

    [SerializeField]
    private BattleLevelConfig levelConfig;

    [SerializeField]
    private LevelManager.StageType stageType =
        LevelManager.StageType.Penjumlahan;


    //==================================================
    // SCENE SETTINGS
    //==================================================

    [Header("Scene Settings")]

    [SerializeField]
    private string levelScenePrefix =
        "Jumlah_Level-";

    [SerializeField]
    private string stageSelectionScene =
        "StagePerjumlahan";

    [SerializeField]
    private int maxLevel = 10;


    //==================================================
    // OPTIONAL TEXT
    //==================================================

    [Header("Optional Text")]

    [SerializeField]
    private TMP_Text victoryLevelText;

    [SerializeField]
    private TMP_Text defeatLevelText;


    //==================================================
    // STATUS
    //==================================================

    private bool progressSaved = false;


    //==================================================
    // SHOW VICTORY
    //==================================================

    public void ShowVictory()
    {
        if (levelConfig == null)
        {
            Debug.LogError(
                "BattleResultUI: Level Config belum diisi."
            );

            return;
        }


        //==============================================
        // UPDATE TEXT
        //==============================================

        if (victoryLevelText != null)
        {
            victoryLevelText.text =
                "Level " +
                levelConfig.levelNumber +
                " Selesai!";
        }


        //==============================================
        // SIMPAN PROGRESS
        //==============================================

        SaveLevelProgress();


        //==============================================
        // SHOW POPUP
        //==============================================

        if (battleUI != null)
        {
            battleUI.ShowVictory();
        }
    }


    //==================================================
    // SHOW DEFEAT
    //==================================================

    public void ShowDefeat()
    {
        if (
            defeatLevelText != null &&
            levelConfig != null
        )
        {
            defeatLevelText.text =
                "Level " +
                levelConfig.levelNumber;
        }


        if (battleUI != null)
        {
            battleUI.ShowDefeat();
        }
    }


    //==================================================
    // SAVE LEVEL PROGRESS
    //==================================================

    private void SaveLevelProgress()
    {
        // Cegah CompleteLevel dipanggil berkali-kali.
        if (progressSaved)
            return;


        if (levelConfig == null)
            return;


        if (LevelProgressManager.Instance == null)
        {
            Debug.LogWarning(
                "BattleResultUI: " +
                "LevelProgressManager tidak ditemukan."
            );

            return;
        }


        LevelProgressManager.Instance.CompleteLevel(
            stageType,
            levelConfig.levelNumber
        );


        progressSaved = true;


        Debug.Log(
            $"Level selesai disimpan: " +
            $"{stageType} - " +
            $"Level {levelConfig.levelNumber}"
        );
    }


    //==================================================
    // NEXT LEVEL
    //==================================================

    public void NextLevel()
    {
        if (levelConfig == null)
            return;


        int currentLevel =
            levelConfig.levelNumber;


        //==============================================
        // MASIH ADA LEVEL BERIKUTNYA
        //==============================================

        if (currentLevel < maxLevel)
        {
            int nextLevel =
                currentLevel + 1;


            string nextScene =
                levelScenePrefix +
                nextLevel;


            Debug.Log(
                "Load scene: " +
                nextScene
            );


            SceneManager.LoadScene(
                nextScene
            );
        }

        //==============================================
        // LEVEL TERAKHIR
        //==============================================
        else
        {
            Debug.Log(
                "Level terakhir selesai. " +
                "Kembali ke Stage Selection."
            );


            SceneManager.LoadScene(
                stageSelectionScene
            );
        }
    }


    //==================================================
    // RETRY
    //==================================================

    public void Retry()
    {
        if (battleManager == null)
        {
            Debug.LogError(
                "BattleResultUI: " +
                "BattleManager belum diisi."
            );

            return;
        }


        // Tidak LoadScene.
        //
        // Ini penting supaya QuestionSession
        // tidak hilang.
        battleManager.RetryBattle();
    }


    //==================================================
    // BACK
    //==================================================

    public void BackToStageSelection()
    {
        SceneManager.LoadScene(
            stageSelectionScene
        );
    }


    //==================================================
    // RESET RESULT STATE
    //==================================================

    public void ResetResultState()
    {
        progressSaved = false;
    }
}
