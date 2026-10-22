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



    private BattleStageConfig
    runtimeStageConfig;


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
        if (
            levelConfig == null ||
            runtimeStageConfig == null
        )
        {
            Debug.LogError(
                "BattleResultUI: " +
                "Level/Stage Config belum tersedia."
            );

            return;
        }


        int nextLevel =
            levelConfig.levelNumber + 1;


        //==============================================
        // STAGE SELESAI
        //==============================================

        if (
            nextLevel >
            runtimeStageConfig.MaxLevel
        )
        {
            BackToStageSelection();

            return;
        }


        //==============================================
        // CEK CONFIG NEXT LEVEL
        //==============================================

        BattleLevelConfig nextConfig =
            runtimeStageConfig
                .GetLevelConfig(
                    nextLevel
                );


        if (nextConfig == null)
        {
            Debug.LogWarning(
                "BattleResultUI: " +
                "Next Level belum tersedia."
            );

            BackToStageSelection();

            return;
        }


        //==============================================
        // SAVE SELECTION
        //==============================================

        BattleSelectionData
            .SetSelectedLevel(
                nextLevel
            );


        //==============================================
        // LOAD SCENE YANG SAMA
        //==============================================

        UnityEngine.SceneManagement
            .SceneManager
            .LoadScene(
                runtimeStageConfig
                    .BattleSceneName
            );
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
        if (runtimeStageConfig == null)
        {
            Debug.LogError(
                "BattleResultUI: " +
                "Stage Config tidak tersedia."
            );

            return;
        }


        UnityEngine.SceneManagement
            .SceneManager
            .LoadScene(
                runtimeStageConfig
                    .StageSelectionSceneName
            );
    }
    
    //==================================================
    // RESET RESULT STATE
    //==================================================

    public void ResetResultState()
    {
        progressSaved = false;
    }

    //==================================================
    // INITIALIZE RUNTIME
    //==================================================

    public void InitializeRuntime(
        BattleLevelConfig currentLevelConfig,
        BattleStageConfig currentStageConfig
    )
    {
        levelConfig =
            currentLevelConfig;


        runtimeStageConfig =
            currentStageConfig;


        progressSaved =
            false;
    }

    
}
