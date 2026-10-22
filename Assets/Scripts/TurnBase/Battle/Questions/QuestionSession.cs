using System.Collections.Generic;

public class QuestionSession
{
    //==================================================
    // SUPPORTED SPELL TIERS
    //==================================================

    private static readonly SpellTier[] SupportedTiers =
    {
        SpellTier.Basic,
        SpellTier.Medium,
        SpellTier.Advanced,
        SpellTier.Defense
    };


    //==================================================
    // QUESTION HISTORY
    //==================================================

    private readonly Dictionary<
        SpellTier,
        List<QuestionData>
    > questionHistory =
        new Dictionary<
            SpellTier,
            List<QuestionData>
        >();


    //==================================================
    // CURRENT INDEX
    //==================================================

    private readonly Dictionary<
        SpellTier,
        int
    > currentIndexes =
        new Dictionary<
            SpellTier,
            int
        >();


    //==================================================
    // CONSTRUCTOR
    //==================================================

    public QuestionSession()
    {
        foreach (
            SpellTier tier
            in SupportedTiers
        )
        {
            InitializeTier(
                tier
            );
        }
    }


    //==================================================
    // INITIALIZE TIER
    //==================================================

    private void InitializeTier(
        SpellTier spellTier
    )
    {
        if (
            !questionHistory.ContainsKey(
                spellTier
            )
        )
        {
            questionHistory[
                spellTier
            ] =
                new List<QuestionData>();
        }


        if (
            !currentIndexes.ContainsKey(
                spellTier
            )
        )
        {
            currentIndexes[
                spellTier
            ] = 0;
        }
    }


    //==================================================
    // START NEW SESSION
    //==================================================

    public void StartNewSession()
    {
        foreach (
            SpellTier tier
            in SupportedTiers
        )
        {
            InitializeTier(
                tier
            );


            questionHistory[
                tier
            ].Clear();


            currentIndexes[
                tier
            ] = 0;
        }
    }


    //==================================================
    // RESTART ATTEMPT
    //==================================================

    public void RestartAttempt()
    {
        foreach (
            SpellTier tier
            in SupportedTiers
        )
        {
            InitializeTier(
                tier
            );


            // History TIDAK dihapus.
            //
            // Kita hanya mengembalikan
            // index ke soal pertama.
            currentIndexes[
                tier
            ] = 0;
        }
    }


    //==================================================
    // GET RECORDED QUESTION
    //==================================================

    public bool TryGetRecordedQuestion(
        SpellTier spellTier,
        out QuestionData question
    )
    {
        question = null;


        InitializeTier(
            spellTier
        );


        int index =
            currentIndexes[
                spellTier
            ];


        List<QuestionData> history =
            questionHistory[
                spellTier
            ];


        if (
            index < 0 ||
            index >= history.Count
        )
        {
            return false;
        }


        question =
            history[
                index
            ];


        currentIndexes[
            spellTier
        ] =
            index + 1;


        return true;
    }


    //==================================================
    // RECORD NEW QUESTION
    //==================================================

    public void RecordNewQuestion(
        QuestionData question
    )
    {
        if (question == null)
            return;


        SpellTier tier =
            question.spellTier;


        InitializeTier(
            tier
        );


        questionHistory[
            tier
        ].Add(
            question
        );


        currentIndexes[
            tier
        ] =
            questionHistory[
                tier
            ].Count;
    }


    //==================================================
    // GET RECORDED COUNT
    //==================================================

    public int GetRecordedCount(
        SpellTier spellTier
    )
    {
        InitializeTier(
            spellTier
        );


        return questionHistory[
            spellTier
        ].Count;
    }
}