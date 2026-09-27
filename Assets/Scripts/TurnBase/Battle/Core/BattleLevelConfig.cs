using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestionRange
{
    [Header("Operand A")]
    [Min(0)]
    public int minA = 1;

    [Min(0)]
    public int maxA = 10;

    [Header("Operand B")]
    [Min(0)]
    public int minB = 1;

    [Min(0)]
    public int maxB = 10;
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

    public MathOperation operation = MathOperation.Addition;

    public string enemyName = "Slime Level 1";


    //==================================================
    // LIFE
    //==================================================

    [Header("Life Settings")]

    [Min(1)]
    public int playerMaxLives = 5;

    [Min(1)]
    public int enemyMaxLives = 5;


    //==================================================
    // BASIC SPELL
    //==================================================

    [Header("Basic Spell Question")]

    public QuestionRange basicRange;


    //==================================================
    // MEDIUM SPELL
    //==================================================

    [Header("Medium Spell Question")]

    public QuestionRange mediumRange;


    //==================================================
    // ADVANCED SPELL
    //==================================================

    [Header("Advanced Spell Question")]

    public QuestionRange advancedRange;


    //==================================================
    // DEFENSE
    //==================================================

    [Header("Defense Question")]

    public QuestionRange defenseRange;
}
