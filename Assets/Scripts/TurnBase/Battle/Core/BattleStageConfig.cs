using UnityEngine;

[CreateAssetMenu(
    fileName = "NewBattleStageConfig",
    menuName = "ReMath/Battle/Stage Config"
)]
public class BattleStageConfig : ScriptableObject
{
    //==================================================
    // STAGE
    //==================================================

    [Header("Stage")]

    [SerializeField]
    private MathOperation operation =
        MathOperation.Addition;


    //==================================================
    // SCENES
    //==================================================

    [Header("Scenes")]

    [SerializeField]
    private string battleSceneName =
        "TB-Jumlah";

    [SerializeField]
    private string stageSelectionSceneName =
        "StagePerjumlahan";


    //==================================================
    // LEVEL CONFIGS
    //==================================================

    [Header("Level Configs - Level 1 to 10")]

    [SerializeField]
    private BattleLevelConfig[] levelConfigs =
        new BattleLevelConfig[10];


    //==================================================
    // PROPERTIES
    //==================================================

    public MathOperation Operation =>
        operation;


    public string BattleSceneName =>
        battleSceneName;


    public string StageSelectionSceneName =>
        stageSelectionSceneName;


    public int MaxLevel =>
        levelConfigs != null
            ? levelConfigs.Length
            : 0;


    //==================================================
    // GET LEVEL CONFIG
    //==================================================

    public BattleLevelConfig GetLevelConfig(
        int levelNumber
    )
    {
        if (levelConfigs == null)
        {
            Debug.LogError(
                "BattleStageConfig: " +
                "Level Configs null."
            );

            return null;
        }


        int index =
            levelNumber - 1;


        if (
            index < 0 ||
            index >= levelConfigs.Length
        )
        {
            Debug.LogError(
                "BattleStageConfig: " +
                "Level " +
                levelNumber +
                " berada di luar range."
            );

            return null;
        }


        BattleLevelConfig config =
            levelConfigs[index];


        if (config == null)
        {
            Debug.LogError(
                "BattleStageConfig: " +
                "Config Level " +
                levelNumber +
                " belum diisi."
            );

            return null;
        }


        return config;
    }
}