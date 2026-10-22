public static class BattleSelectionData
{
    //==================================================
    // SELECTED LEVEL
    //==================================================

    private static int selectedLevel = 1;


    //==================================================
    // PROPERTY
    //==================================================

    public static int SelectedLevel =>
        selectedLevel;


    //==================================================
    // SET LEVEL
    //==================================================

    public static void SetSelectedLevel(
        int level
    )
    {
        if (level < 1)
        {
            level = 1;
        }


        selectedLevel =
            level;
    }


    //==================================================
    // RESET
    //==================================================

    public static void Reset()
    {
        selectedLevel = 1;
    }
}