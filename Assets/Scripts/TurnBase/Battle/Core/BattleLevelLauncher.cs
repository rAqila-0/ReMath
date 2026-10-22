using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleLevelLauncher : MonoBehaviour
{
    //==================================================
    // STAGE CONFIG
    //==================================================

    [Header("Stage Config")]

    [SerializeField]
    private BattleStageConfig stageConfig;


    //==================================================
    // LAUNCH LEVEL
    //==================================================

    public void LaunchLevel(
        int levelNumber
    )
    {
        if (stageConfig == null)
        {
            Debug.LogError(
                "BattleLevelLauncher: " +
                "Stage Config belum diisi."
            );

            return;
        }


        //==============================================
        // VALIDATE LEVEL
        //==============================================

        BattleLevelConfig config =
            stageConfig.GetLevelConfig(
                levelNumber
            );


        if (config == null)
        {
            Debug.LogWarning(
                "BattleLevelLauncher: " +
                "Level " +
                levelNumber +
                " belum tersedia."
            );

            return;
        }

        //==================================================
        // CEK LEVEL SUDAH TERSEDIA
        //==================================================

        if (!config.isPlayable)
        {
            Debug.LogWarning(
                "BattleLevelLauncher: " +
                "Level " +
                levelNumber +
                " belum tersedia."
            );

            return;
        }


        //==============================================
        // SAVE SELECTED LEVEL
        //==============================================

        BattleSelectionData
            .SetSelectedLevel(
                levelNumber
            );


        Debug.Log(
            "BattleLevelLauncher: " +
            "Memilih Level " +
            levelNumber
        );


        //==============================================
        // LOAD ONE BATTLE SCENE
        //==============================================

        SceneManager.LoadScene(
            stageConfig.BattleSceneName
        );
    }
}