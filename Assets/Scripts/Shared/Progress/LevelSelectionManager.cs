using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionManager : MonoBehaviour
{
    //==================================================
    // DATA TOMBOL LEVEL
    //==================================================

    [System.Serializable]
    public class LevelButtonData
    {
        [Tooltip("Nomor level tombol ini.")]
        public int levelNumber;


        [Tooltip("Button level.")]
        public Button levelButton;


        [Tooltip("Icon jika level masih terkunci.")]
        public GameObject lockIcon;


        [Tooltip("Icon jika level sudah selesai.")]
        public GameObject completedIcon;


        [Tooltip(
            "Visual jika level belum tersedia, " +
            "misalnya Boss belum selesai."
        )]
        public GameObject unavailableVisual;
    }


    //==================================================
    // STAGE
    //==================================================

    [Header("Stage")]

    [SerializeField]
    private LevelManager.StageType stageType =
        LevelManager.StageType.Penjumlahan;


    [SerializeField]
    private BattleStageConfig stageConfig;


    //==================================================
    // LAUNCHER
    //==================================================

    [Header("Battle Launcher")]

    [SerializeField]
    private BattleLevelLauncher
        battleLevelLauncher;


    //==================================================
    // BUTTONS
    //==================================================

    [Header("Level Buttons")]

    [SerializeField]
    private LevelButtonData[] levelButtons;


    //==================================================
    // START
    //==================================================

    private void Start()
    {
        RefreshLevelButtons();
    }


    //==================================================
    // REFRESH BUTTON
    //==================================================

    public void RefreshLevelButtons()
    {
        if (
            LevelProgressManager.Instance ==
            null
        )
        {
            Debug.LogError(
                "LevelSelectionManager: " +
                "LevelProgressManager.Instance " +
                "tidak ditemukan."
            );

            return;
        }


        int highestUnlockedLevel =
            LevelProgressManager.Instance
                .GetUnlockedLevel(
                    stageType
                );


        Debug.Log(
            "Refresh Level Selection | " +
            "Stage = " +
            stageType +
            " | Highest Unlocked = " +
            highestUnlockedLevel
        );


        foreach (
            LevelButtonData data
            in levelButtons
        )
        {
            if (data == null)
                continue;


            //==========================================
            // UNLOCK PROGRESS
            //==========================================

            bool isUnlocked =
                LevelProgressManager.Instance
                    .IsLevelUnlocked(
                        stageType,
                        data.levelNumber
                    );


            //==========================================
            // LEVEL AVAILABLE?
            //==========================================

            bool isAvailable =
                IsLevelAvailable(
                    data.levelNumber
                );


            //==========================================
            // COMPLETED?
            //==========================================

            bool isCompleted =
                LevelProgressManager.Instance
                    .IsLevelCompleted(
                        stageType,
                        data.levelNumber
                    );


            //==========================================
            // BUTTON
            //==========================================

            if (
                data.levelButton !=
                null
            )
            {
                data.levelButton.interactable =
                    isUnlocked &&
                    isAvailable;
            }


            //==========================================
            // LOCK ICON
            //
            // Lock hanya berarti progress belum terbuka.
            //==========================================

            if (data.lockIcon != null)
            {
                data.lockIcon.SetActive(
                    !isUnlocked
                );
            }


            //==========================================
            // COMPLETED ICON
            //==========================================

            if (
                data.completedIcon !=
                null
            )
            {
                data.completedIcon.SetActive(
                    isCompleted
                );
            }


            //==========================================
            // UNAVAILABLE
            //
            // Contoh:
            // Level 10 sudah unlocked tetapi
            // boss belum tersedia.
            //==========================================

            if (
                data.unavailableVisual !=
                null
            )
            {
                data.unavailableVisual
                    .SetActive(
                        isUnlocked &&
                        !isAvailable
                    );
            }
        }
    }


    //==================================================
    // LEVEL AVAILABLE
    //==================================================

    private bool IsLevelAvailable(
        int levelNumber
    )
    {
        if (stageConfig == null)
        {
            Debug.LogError(
                "LevelSelectionManager: " +
                "StageConfig belum diisi."
            );

            return false;
        }


        BattleLevelConfig config =
            stageConfig.GetLevelConfig(
                levelNumber
            );


        if (config == null)
        {
            return false;
        }


        return config.isPlayable;
    }


    //==================================================
    // OPEN LEVEL
    //==================================================

    public void OpenLevel(
        int levelNumber
    )
    {
        //==============================================
        // PROGRESS MANAGER
        //==============================================

        if (
            LevelProgressManager.Instance ==
            null
        )
        {
            Debug.LogError(
                "LevelSelectionManager: " +
                "LevelProgressManager tidak ditemukan."
            );

            return;
        }


        //==============================================
        // CHECK UNLOCK
        //==============================================

        bool isUnlocked =
            LevelProgressManager.Instance
                .IsLevelUnlocked(
                    stageType,
                    levelNumber
                );


        if (!isUnlocked)
        {
            Debug.LogWarning(
                "Level " +
                levelNumber +
                " masih terkunci."
            );

            return;
        }


        //==============================================
        // CHECK AVAILABLE
        //==============================================

        if (
            !IsLevelAvailable(
                levelNumber
            )
        )
        {
            Debug.LogWarning(
                "Level " +
                levelNumber +
                " belum tersedia."
            );

            return;
        }


        //==============================================
        // LAUNCH
        //==============================================

        if (
            battleLevelLauncher ==
            null
        )
        {
            Debug.LogError(
                "LevelSelectionManager: " +
                "BattleLevelLauncher belum diisi."
            );

            return;
        }


        battleLevelLauncher
            .LaunchLevel(
                levelNumber
            );
    }
}