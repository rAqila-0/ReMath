using System.Collections;
using System.Collections.Generic;


//==================================================
// DATA SATU SOAL
//==================================================

[System.Serializable]
public class BattleQuestion
{
    public int operandA;
    public int operandB;
    public int correctAnswer;

    public MathOperation operation;
    public SpellTier spellTier;


    public BattleQuestion(
        int operandA,
        int operandB,
        int correctAnswer,
        MathOperation operation,
        SpellTier spellTier
    )
    {
        this.operandA = operandA;
        this.operandB = operandB;
        this.correctAnswer = correctAnswer;
        this.operation = operation;
        this.spellTier = spellTier;
    }
}


//==================================================
// QUESTION SESSION
//==================================================

public class QuestionSession
{
    // Riwayat soal berdasarkan jenis spell.
    private readonly Dictionary
        <SpellTier, List<BattleQuestion>>
        questionHistory =
        new Dictionary
        <SpellTier, List<BattleQuestion>>();


    // Posisi soal yang sedang digunakan
    // untuk setiap jenis spell.
    private readonly Dictionary
        <SpellTier, int>
        currentIndexes =
        new Dictionary
        <SpellTier, int>();


    //==================================================
    // SPELL TIERS
    //==================================================

    private readonly SpellTier[] spellTiers =
    {
        SpellTier.Basic,
        SpellTier.Medium,
        SpellTier.Advanced,
        SpellTier.Defense
    };


    //==================================================
    // CONSTRUCTOR
    //==================================================

    public QuestionSession()
    {
        StartNewSession();
    }


    //==================================================
    // NEW SESSION
    //==================================================

    public void StartNewSession()
    {
        questionHistory.Clear();
        currentIndexes.Clear();


        foreach (SpellTier tier in spellTiers)
        {
            questionHistory.Add(
                tier,
                new List<BattleQuestion>()
            );

            currentIndexes.Add(
                tier,
                0
            );
        }
    }


    //==================================================
    // RETRY ATTEMPT
    //==================================================

    public void RestartAttempt()
    {
        foreach (SpellTier tier in spellTiers)
        {
            currentIndexes[tier] = 0;
        }
    }


    //==================================================
    // AMBIL SOAL LAMA
    //==================================================

    public bool TryGetRecordedQuestion(
        SpellTier spellTier,
        out BattleQuestion question
    )
    {
        EnsureTierExists(spellTier);


        int index =
            currentIndexes[spellTier];

        List<BattleQuestion> questions =
            questionHistory[spellTier];


        // Masih ada soal dari percobaan sebelumnya.
        if (index < questions.Count)
        {
            question =
                questions[index];

            currentIndexes[spellTier]++;

            return true;
        }


        question = null;

        return false;
    }


    //==================================================
    // SIMPAN SOAL BARU
    //==================================================

    public void RecordNewQuestion(
        BattleQuestion question
    )
    {
        if (question == null)
            return;


        EnsureTierExists(
            question.spellTier
        );


        questionHistory[
            question.spellTier
        ].Add(question);


        // Soal baru langsung dianggap sudah digunakan.
        currentIndexes[
            question.spellTier
        ]++;
    }


    //==================================================
    // GET RECORDED COUNT
    //==================================================

    public int GetRecordedCount(
        SpellTier spellTier
    )
    {
        EnsureTierExists(spellTier);

        return questionHistory[
            spellTier
        ].Count;
    }


    //==================================================
    // SAFETY
    //==================================================

    private void EnsureTierExists(
        SpellTier spellTier
    )
    {
        if (!questionHistory.ContainsKey(spellTier))
        {
            questionHistory.Add(
                spellTier,
                new List<BattleQuestion>()
            );
        }

        if (!currentIndexes.ContainsKey(spellTier))
        {
            currentIndexes.Add(
                spellTier,
                0
            );
        }
    }
}