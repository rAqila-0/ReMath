using System;
using UnityEngine;

/// <summary>
/// Resets Re:Math level-unlock progress, per-level DDA progress, and saved
/// question sequences without deleting unrelated PlayerPrefs settings.
/// </summary>
public class ReMathProgressResetter : MonoBehaviour
{
    [Header("Reset Scope")]
    [Tooltip("Jumlah level pada setiap operasi yang akan diperiksa. Sesuaikan jika jumlah level berubah.")]
    [SerializeField, Min(1)]
    private int maxLevelsPerOperation = 10;

    /// <summary>
    /// Deletes all known Re:Math progression keys. Returns true when the reset
    /// pass completes. Unrelated PlayerPrefs entries (such as audio settings)
    /// are intentionally preserved.
    /// </summary>
    public bool ResetAllProgress()
    {
        int deletedKeyCount = 0;

        // 1. Reset unlocked-level progress for every stage.
        foreach (LevelManager.StageType stage in Enum.GetValues(typeof(LevelManager.StageType)))
        {
            string key = "ReMath_UnlockedLevel_" + stage;
            deletedKeyCount += DeleteKeyIfPresent(key);
        }

        // 2. Reset per-level question history and DDA state for every math operation.
        foreach (MathOperation operation in Enum.GetValues(typeof(MathOperation)))
        {
            string operationName = operation.ToString();

            for (int level = 1; level <= Mathf.Max(1, maxLevelsPerOperation); level++)
            {
                string questionKey =
                    "ReMath_QuestionProgress_" + operationName + "_Level_" + level;

                string ddaKey =
                    "ReMath_DDA_" + operationName + "_Level_" + level;

                deletedKeyCount += DeleteKeyIfPresent(questionKey);
                deletedKeyCount += DeleteKeyIfPresent(ddaKey);
            }
        }

        PlayerPrefs.Save();

        Debug.Log(
            "Re:Math: reset progres selesai. " +
            "Jumlah key progres yang dihapus: " + deletedKeyCount + ". " +
            "Pengaturan lain yang tidak terkait progres tetap dipertahankan.");

        return true;
    }

    private static int DeleteKeyIfPresent(string key)
    {
        if (!PlayerPrefs.HasKey(key))
            return 0;

        PlayerPrefs.DeleteKey(key);
        return 1;
    }
}
