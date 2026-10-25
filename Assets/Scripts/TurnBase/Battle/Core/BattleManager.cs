using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    //==================================================
    // LEVEL CONFIG
    //==================================================

    [Header("Stage Config")]

    [SerializeField]
    private BattleStageConfig stageConfig;


    [Header("Editor Fallback Level")]

    [SerializeField]
    private BattleLevelConfig levelConfig;


    //==================================================
    // CHARACTER REFERENCES
    //==================================================

    [Header("Characters")]

    [SerializeField]
    private PlayerBattle playerBattle;

    [SerializeField]
    private EnemyBattle enemyBattle;

    [SerializeField]
    private EnemyVisualController
        enemyVisualController;


    //==================================================
    // SYSTEM REFERENCES
    //==================================================

    [Header("Battle Systems")]

    [SerializeField]
    private BattleUIManager battleUI;

    [SerializeField]
    private SpellController spellController;

    [SerializeField]
    private QuestionManager questionManager;

    [Header("Question Input")]

    [SerializeField]
    private AnswerSlotInputManager
        answerSlotInputManager;

    [Header("Battle Result")]

    [SerializeField]
    private BattleResultUI battleResultUI;

    [Header("Battle Animation")]

    [SerializeField]
    private BattleAnimationController battleAnimation;

    [Header("Battle Audio")]

    [SerializeField]
    private BattleAudioController battleAudio;

    [Header("Turn Presentation")]

    [SerializeField]
    private TurnPresentationController
        turnPresentation;


    //==================================================
    // TIMING
    //==================================================

    [Header("Timing")]

    [SerializeField]
    private float feedbackDuration = 1.2f;

    [Header("Turn Timing")]

    [SerializeField]
    private float enemyWarningDelay = 0.5f;

    //==================================================
    // ADAPTIVE DEFENSE
    //==================================================

    [Header("Adaptive Defense")]

    [SerializeField]
    private AdaptiveDefenseManager
        adaptiveDefenseManager;


    //==================================================
    // CURRENT BATTLE STATE
    //==================================================

    private BattleTurn currentTurn;

    private SpellTier currentSpellTier;

    private bool questionActive = false;

    private bool isResolving = false;

    private bool battleEnded = false;

    // True only when the most recently ended battle ended in defeat.
    // RetryBattle uses this to distinguish result-screen Retry from pause Retry.
    private bool battleWasDefeated = false;


    //==================================================
    // PROPERTIES
    //==================================================

    public BattleTurn CurrentTurn =>
        currentTurn;

    public bool QuestionActive =>
        questionActive;

    public bool BattleEnded =>
        battleEnded;


    //==================================================
    // START
    //==================================================

    private void Start()
    {
        if (!ResolveLevelConfig())
        {
            enabled = false;

            return;
        }


        InitializeBattle();
    }

    //==================================================
    // RESOLVE LEVEL CONFIG
    //==================================================

    private bool ResolveLevelConfig()
    {
        //==============================================
        // STAGE CONFIG AVAILABLE
        //==============================================

        if (stageConfig != null)
        {
            int selectedLevel =
                BattleSelectionData
                    .SelectedLevel;


            BattleLevelConfig selectedConfig =
                stageConfig.GetLevelConfig(
                    selectedLevel
                );


            if (selectedConfig != null)
            {
                levelConfig =
                    selectedConfig;


                Debug.Log(
                    "BattleManager: " +
                    "Memuat Level " +
                    selectedLevel +
                    " | Enemy = " +
                    levelConfig.enemyName
                );


                return true;
            }


            Debug.LogWarning(
                "BattleManager: " +
                "Config level terpilih tidak ditemukan. " +
                "Mencoba Editor Fallback."
            );
        }


        //==============================================
        // EDITOR FALLBACK
        //==============================================

        if (levelConfig != null)
        {
            Debug.LogWarning(
                "BattleManager: " +
                "Menggunakan Editor Fallback Level Config."
            );


            return true;
        }


        Debug.LogError(
            "BattleManager: " +
            "Tidak ada Level Config yang dapat digunakan."
        );


        return false;
    }


    //==================================================
    // INITIALIZE BATTLE
    //==================================================

    private void InitializeBattle()
    {
        if (!ValidateReferences())
        {
            Debug.LogError(
                "BattleManager: Ada reference penting " +
                "yang belum diisi di Inspector."
            );

            enabled = false;

            return;
        }


        //==============================================
        // PLAYER & ENEMY
        //==============================================

        playerBattle.Initialize(
            levelConfig.playerMaxLives
        );

        enemyBattle.Initialize(
            levelConfig.enemyMaxLives
        );

        if (enemyVisualController != null)
        {
            enemyVisualController.Initialize(
                levelConfig
            );
        }


        //==============================================
        // QUESTION SYSTEM
        //==============================================

        questionManager.Initialize(
            levelConfig
        );

        if (adaptiveDefenseManager != null)
        {
            adaptiveDefenseManager
                .ResetAdaptation();
        }

        if (battleResultUI != null)
        {
            battleResultUI.InitializeRuntime(
                levelConfig,
                stageConfig
            );
        }


        //==============================================
        // UI
        //==============================================

        battleUI.Initialize(
            levelConfig
        );


        //==============================================
        // INPUT
        //==============================================

        if (answerSlotInputManager != null)
        {
            answerSlotInputManager
                .ResetInput();
        }

        spellController.ResetSelection();


        //==============================================
        // STATE
        //==============================================

        battleEnded = false;
        battleWasDefeated = false;
        isResolving = false;
        questionActive = false;

        battleAnimation.ResetVisualState();


        StartPlayerTurn();


        Debug.Log(
            $"Battle dimulai. " +
            $"Level {levelConfig.levelNumber} - " +
            $"{levelConfig.operation}"
        );
    }


    //==================================================
    // PLAYER TURN
    //==================================================

    public void StartPlayerTurn()
    {
        if (battleEnded)
            return;


        StartCoroutine(
            PlayerTurnSequence()
        );
    }

    private IEnumerator PlayerTurnSequence()
    {
        //==================================================
        // STATE
        //==================================================

        currentTurn =
            BattleTurn.Player;


        questionActive = false;

        // Disable input selama intro turn.
        isResolving = true;


        //==================================================
        // RESET
        //==================================================

        questionManager.ClearCurrentQuestion();

        if (answerSlotInputManager != null)
        {
            answerSlotInputManager
                .ResetInput();
        }

        spellController.ResetSelection();


        turnPresentation.StopWarning();


        //==================================================
        // PREPARE UI
        //==================================================

        battleUI.PrepareTurnTransition(
            BattleTurn.Player
        );


        //==================================================
        // TURN INTRO
        //==================================================

        yield return turnPresentation
            .PlayTurnIntro(
                BattleTurn.Player
            );


        //==================================================
        // PLAYER ACTION
        //==================================================

        battleUI.ShowPlayerActionPanel();


        isResolving = false;


        Debug.Log(
            "=== GILIRAN PEMAIN ==="
        );
    }


    //==================================================
    // PLAYER CONFIRM SPELL
    //==================================================

    public void ConfirmPlayerSpell(
        SpellTier spellTier
    )
    {
        if (battleEnded)
            return;

        if (isResolving)
            return;

        if (questionActive)
            return;

        if (currentTurn != BattleTurn.Player)
            return;


        if (spellTier == SpellTier.Defense)
            return;


        currentSpellTier =
            spellTier;


        int damage =
            spellController.GetSpellDamage(
                spellTier
            );


        //==============================================
        // RESET INPUT
        //==============================================

        if (answerSlotInputManager != null)
        {
            answerSlotInputManager
                .ResetInput();
        }


        //==============================================
        // SHOW QUESTION UI
        //==============================================

        battleUI.ShowQuestionMode(
            spellTier,
            damage
        );


        //==============================================
        // GENERATE QUESTION
        //==============================================

        QuestionData question =
            questionManager.GenerateQuestion(
                spellTier
            );


        if (question == null)
        {
            Debug.LogError(
                "BattleManager: " +
                "Gagal membuat soal Player."
            );

            questionActive = false;

            battleUI.ShowPlayerTurn();

            return;
        }


        questionActive = true;


        Debug.Log(
            $"Player merapal {spellTier}. " +
            $"Damage jika benar: {damage}."
        );
    }


    //==================================================
    // ENEMY TURN
    //==================================================

    private void StartEnemyTurn()
    {
        if (battleEnded)
            return;


        StartCoroutine(
            EnemyTurnSequence()
        );
    }

    private IEnumerator EnemyTurnSequence()
    {
        //==================================================
        // STATE
        //==================================================

        currentTurn =
            BattleTurn.Enemy;


        questionActive = false;

        isResolving = true;


        //==================================================
        // RESET
        //==================================================

        questionManager.ClearCurrentQuestion();

        if (answerSlotInputManager != null)
        {
            answerSlotInputManager
                .ResetInput();
        }

        spellController.ResetSelection();


        //==================================================
        // PREPARE UI
        //==================================================

        battleUI.PrepareTurnTransition(
            BattleTurn.Enemy
        );


        //==================================================
        // TURN INTRO
        //==================================================

        yield return turnPresentation
            .PlayTurnIntro(
                BattleTurn.Enemy
            );


        //==================================================
        // WARNING
        //==================================================

        turnPresentation.StartWarning();


        yield return new WaitForSeconds(
            enemyWarningDelay
        );


        //==================================================
        // DEFENSE BUTTON
        //==================================================

        battleUI.ShowEnemyActionPanel();


        isResolving = false;


        Debug.Log(
            "=== GILIRAN MUSUH ==="
        );
    }


    //==================================================
    // DEFENSE SPELL
    //==================================================

    public void StartDefenseQuestion()
    {
        if (battleEnded)
            return;

        if (isResolving)
            return;

        if (questionActive)
            return;

        if (currentTurn != BattleTurn.Enemy)
            return;


        currentSpellTier =
            SpellTier.Defense;


        if (answerSlotInputManager != null)
        {
            answerSlotInputManager
                .ResetInput();
        }


        //==============================================
        // QUESTION PANEL
        //==============================================

        battleUI.ShowQuestionMode(
            SpellTier.Defense,
            0
        );


        // Warning tetap aktif selama
        // Player sedang menjawab Defense.


        //==============================================
        // GENERATE DEFENSE QUESTION
        //==============================================

        QuestionData question =
            questionManager.GenerateQuestion(
                SpellTier.Defense
            );


        if (question == null)
        {
            Debug.LogError(
                "BattleManager: " +
                "Gagal membuat soal Defense."
            );

            questionActive = false;

            battleUI.ShowEnemyTurn();

            return;
        }


        questionActive = true;


        Debug.Log(
            "Player mulai merapal " +
            "Sihir Pertahanan."
        );
    }


    //==================================================
    // SUBMIT ANSWER
    //==================================================

    public void SubmitAnswer()
    {
        if (battleEnded)
            return;

        if (isResolving)
            return;

        if (!questionActive)
            return;


        //==============================================
        // AMBIL JAWABAN DARI ANSWER SLOT
        //==============================================

        if (answerSlotInputManager == null)
        {
            Debug.LogError(
                "BattleManager: " +
                "AnswerSlotInputManager belum diisi."
            );

            return;
        }


        if (!answerSlotInputManager.TryGetAnswer(
            out int playerAnswer
        ))
        {
            Debug.LogWarning(
                "BattleManager: " +
                "Player belum memasukkan jawaban."
            );

            return;
        }


        //==============================================
        // CHECK ANSWER
        //==============================================

        bool isCorrect =
            questionManager.CheckAnswer(
                playerAnswer
            );


        questionActive = false;

        isResolving = true;

        battleUI.ShowBattleAnimationMode();


        // Saat Defense sudah dijawab,
        // Warning Arrow berhenti.
        if (currentTurn == BattleTurn.Enemy)
        {
            turnPresentation.StopWarning();
        }


        //==============================================
        // RESOLVE BERDASARKAN TURN
        //==============================================

        if (currentTurn == BattleTurn.Player)
        {
            StartCoroutine(
                ResolvePlayerAnswer(
                    isCorrect
                )
            );
        }
        else
        {
            StartCoroutine(
                ResolveDefenseAnswer(
                    isCorrect
                )
            );
        }
    }


    //==================================================
    // RESOLVE PLAYER ANSWER
    //==================================================

    private IEnumerator ResolvePlayerAnswer(
    bool isCorrect
    )
    {
        //==================================================
        // JAWABAN BENAR
        //==================================================

        if (isCorrect)
        {
            //==============================================
            // FEEDBACK
            //==============================================

            battleUI.ShowMantraSuccess();

            battleAudio.PlayMantraSuccess();


            yield return new WaitForSeconds(
                feedbackDuration
            );


            battleUI.HideFeedback();


            //==============================================
            // CAST + PROJECTILE
            //==============================================

            battleAudio.PlaySpellCast(
                currentSpellTier
            );

            yield return battleAnimation
                .PlayPlayerAttackUntilImpact(
                    currentSpellTier
                );

            battleAudio.PlayMagicImpact();


            // Projectile sekarang tepat
            // mengenai Enemy.


            //==============================================
            // DAMAGE PADA TITIK IMPACT
            //==============================================

            int damage =
                spellController.GetSpellDamage(
                    currentSpellTier
                );


            enemyBattle.TakeDamage(
                damage
            );


            //==============================================
            // ENEMY MATI?
            //==============================================

            if (enemyBattle.IsDead)
            {
                yield return battleAnimation
                    .PlayEnemyDefeat();

                battleAudio.PlayVictory();

                yield return battleAnimation
                    .PlayPlayerVictory();


                HandleVictory();


                yield break;
            }


            //==============================================
            // ENEMY HIT
            //==============================================

            battleAudio.PlayEnemyHit();

            yield return battleAnimation
                .PlayEnemyHit();
        }

        //==================================================
        // JAWABAN SALAH
        //==================================================

        else
        {
            battleUI.ShowMantraFailed();

            battleAudio.PlayMantraFailed();


            yield return new WaitForSeconds(
                feedbackDuration
            );


            battleUI.HideFeedback();


            yield return battleAnimation
                .PlayPlayerSpellFail();


            Debug.Log(
                "Serangan Player MISS."
            );
        }


        //==================================================
        // NEXT TURN
        //==================================================

        StartEnemyTurn();
    }


    //==================================================
    // RESOLVE DEFENSE ANSWER
    //==================================================

    private IEnumerator ResolveDefenseAnswer(
    bool isCorrect
    )
    {
        //==============================================
        // ADAPTIVE DEFENSE RESULT
        //==============================================
        if (adaptiveDefenseManager != null)
        {
            adaptiveDefenseManager
                .RegisterDefenseResult(
                    isCorrect
                );
        }

        //==================================================
        // DEFENSE BERHASIL
        //==================================================

        if (isCorrect)
        {
            battleUI.ShowMantraSuccess();

            battleAudio.PlayMantraSuccess();


            yield return new WaitForSeconds(
                feedbackDuration
            );


            battleUI.HideFeedback();


            //==============================================
            // ENEMY MENYERANG
            //==============================================

            battleAudio.PlayEnemyAttack();


            yield return battleAnimation
                .PlayDefenseSuccess(
                    () =>
                    {
                        battleAudio
                            .PlayShieldBlock();
                    },
                    levelConfig != null && levelConfig.isBoss
                );


            Debug.Log(
                "Sihir Pertahanan berhasil. " +
                "Serangan musuh diblok."
            );
        }

        //==================================================
        // DEFENSE SALAH
        //==================================================

        else
        {
            battleUI.ShowMantraFailed();

            battleAudio.PlayMantraFailed();


            yield return new WaitForSeconds(
                feedbackDuration
            );


            battleUI.HideFeedback();


            //==============================================
            // ENEMY ATTACK
            //==============================================

            battleAudio.PlayEnemyAttack();


            yield return battleAnimation
                .PlayEnemyAttackUntilImpact(
                    levelConfig != null && levelConfig.isBoss
                );


            //==============================================
            // PLAYER DAMAGE
            //==============================================

            playerBattle.TakeDamage(1);

            battleAudio.PlayPlayerHit();


            yield return battleAnimation
                .PlayPlayerHit();


            //==============================================
            // PLAYER DEAD
            //==============================================

            if (playerBattle.IsDead)
            {
                battleAudio.PlayDefeat();


                yield return battleAnimation
                    .PlayPlayerDefeat();


                HandleDefeat();


                yield break;
            }
        }


        StartPlayerTurn();
    }


    //==================================================
    // VICTORY
    //==================================================

    private void HandleVictory()
    {
        // Save the tier/question that actually defeated the enemy before the
        // battle UI closes. Do not persist the unused next question/tier.
        if (questionManager != null)
            questionManager.CommitLevelCompletion();

        battleEnded = true;
        battleWasDefeated = false;
        isResolving = false;
        questionActive = false;

        turnPresentation.ResetPresentation();


        //==============================================
        // RESULT
        //==============================================

        if (battleResultUI != null)
        {
            battleResultUI.ShowVictory();
        }
        else
        {
            battleUI.ShowVictory();
        }


        Debug.Log(
            "=== VICTORY ==="
        );
    }


    //==================================================
    // DEFEAT
    //==================================================

    private void HandleDefeat()
    {
        battleEnded = true;
        battleWasDefeated = true;
        isResolving = false;
        questionActive = false;

        turnPresentation.ResetPresentation();


        if (battleResultUI != null)
        {
            battleResultUI.ShowDefeat();
        }
        else
        {
            battleUI.ShowDefeat();
        }


        Debug.Log(
            "=== DEFEAT ==="
        );
    }


    //==================================================
    // RETRY BATTLE
    //==================================================

    public void RetryBattle()
    {
        // Called from both the pause menu and the defeat result panel.
        // Pause Retry replays the exact sequence; defeat Retry keeps the tier
        // reached when the player lost and begins a fresh attempt there.
        bool retryAfterDefeat = battleEnded && battleWasDefeated;

        StopAllCoroutines();

        turnPresentation.ResetPresentation();

        battleAnimation.ResetVisualState();


        //==================================================
        // RESET BATTLE STATE
        //==================================================

        battleEnded = false;
        battleWasDefeated = false;
        isResolving = false;
        questionActive = false;

        if (battleResultUI != null)
        {
            battleResultUI.ResetResultState();
        }


        //==================================================
        // RESET LIFE
        //==================================================

        playerBattle.ResetLives();

        enemyBattle.ResetLives();


        //==================================================
        // RESTART QUESTION SESSION
        //==================================================

        // Reset streaks first so QuestionManager persists a clean new attempt.
        if (adaptiveDefenseManager != null)
        {
            adaptiveDefenseManager.RestartAttemptStreaks();
        }

        if (retryAfterDefeat)
        {
            // Defeat result Retry: keep the tier reached at defeat and begin a
            // fresh sequence at that tier (do not restore the original tier).
            questionManager.RestartAfterDefeat();
            Debug.Log("Retry setelah kalah: melanjutkan dari tier saat kalah.");
        }
        else
        {
            // Pause Retry: preserve the established behavior and replay the
            // exact question sequence/tier history from this attempt.
            questionManager.RestartSessionAttempt();
            Debug.Log("Retry dari pause: mengulang urutan soal percobaan yang sama.");
        }


        //==================================================
        // RESET SPELL & INPUT
        //==================================================

        spellController.ResetSelection();

        if (answerSlotInputManager != null)
        {
            answerSlotInputManager
                .ResetInput();
        }


        //==================================================
        // RESET UI
        //==================================================

        battleUI.HideResultPopups();

        battleUI.HideFeedback();

        turnPresentation.StartWarning();
        turnPresentation.StopWarning();


        //==================================================
        // START AGAIN
        //==================================================

        StartPlayerTurn();


        Debug.Log(
            "Battle diulang dengan " +
            "Question Session yang sama."
        );
    }


    //==================================================
    // VALIDATE REFERENCES
    //==================================================

    private bool ValidateReferences()
    {
        if (levelConfig == null)
        {
            Debug.LogError(
                "BattleManager: Level Config kosong."
            );

            return false;
        }


        if (playerBattle == null)
        {
            Debug.LogError(
                "BattleManager: PlayerBattle kosong."
            );

            return false;
        }


        if (enemyBattle == null)
        {
            Debug.LogError(
                "BattleManager: EnemyBattle kosong."
            );

            return false;
        }

        if (enemyVisualController == null)
        {
            Debug.LogError(
                "BattleManager: " +
                "EnemyVisualController kosong."
            );

            return false;
        }


        if (battleUI == null)
        {
            Debug.LogError(
                "BattleManager: BattleUIManager kosong."
            );

            return false;
        }


        if (spellController == null)
        {
            Debug.LogError(
                "BattleManager: SpellController kosong."
            );

            return false;
        }


        if (questionManager == null)
        {
            Debug.LogError(
                "BattleManager: QuestionManager kosong."
            );

            return false;
        }


        if (answerSlotInputManager == null)
        {
            Debug.LogError(
                "BattleManager: " +
                "AnswerSlotInputManager kosong."
            );

            return false;
        }

        if (battleResultUI == null)
        {
            Debug.LogError(
                "BattleManager: BattleResultUI kosong."
            );

            return false;
        }

        if (battleAnimation == null)
        {
            Debug.LogError(
                "BattleManager: BattleAnimationController kosong."
            );

            return false;
        }

        if (battleAudio == null)
        {
            Debug.LogError(
                "BattleManager: " +
                "BattleAudioController kosong."
            );

            return false;
        }

        if (turnPresentation == null)
        {
            Debug.LogError(
                "BattleManager: " +
                "TurnPresentationController kosong."
            );

            return false;
        }


        return true;
    }
}
