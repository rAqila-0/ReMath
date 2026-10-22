using UnityEngine;

[System.Serializable]
public class QuestionRange
{
    [Header("Operand A")]
    [Min(0)] public int minA = 1;
    [Min(0)] public int maxA = 10;

    [Header("Operand B")]
    [Min(0)] public int minB = 1;
    [Min(0)] public int maxB = 10;
}


[CreateAssetMenu(
    fileName = "NewBattleLevelConfig",
    menuName = "ReMath/Battle/Level Config"
)]
public class BattleLevelConfig : ScriptableObject
{
    //==================================================
    // LEVEL INFORMATION
    //==================================================

    [Header("Level Information")]

    [Min(1)]
    public int levelNumber = 1;

    public MathOperation operation =
        MathOperation.Addition;

    [Tooltip("Apakah level ini sudah dapat dimainkan?")]
    public bool isPlayable = true;


    //==================================================
    // ENEMY
    //==================================================

    [Header("Enemy")]

    public string enemyName =
        "Slime Merah";

    public Sprite enemySprite;


    //==================================================
    // LIFE
    //==================================================

    [Header("Life Settings")]

    [Min(1)]
    public int playerMaxLives = 5;

    [Min(1)]
    public int enemyMaxLives = 5;


    //==================================================
    // ATTACK QUESTION RANGE
    //==================================================

    [Header("Basic Attack - 2 Digits")]
    public QuestionRange basicRange;


    [Header("Medium Attack - 3 Digits")]
    public QuestionRange mediumRange;


    [Header("Advanced Attack - 4 Digits")]
    public QuestionRange advancedRange;


    //==================================================
    // MULTIPLICATION / DIVISION PROGRESSION
    //==================================================

    [Header("Multiplication / Division Progression")]

    [Range(1, 10)]
    public int maxFactMultiplier = 10;


    //==================================================
    // LEGACY DEFENSE RANGE
    //
    // Defense sekarang menggunakan Adaptive Defense.
    // Field ini sementara belum dihapus supaya asset lama aman.
    //==================================================

    [Header("Legacy - Defense Range (Unused)")]
    public QuestionRange defenseRange;
}