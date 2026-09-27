using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    //==================================================
    // LEVEL CONFIG
    //==================================================

    [Header("Level Config")]

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

    [SerializeField]
    private NumpadInputManager numpadInput;

    [Header("Battle Result")]

    [SerializeField]
    private BattleResultUI battleResultUI;

    [Header("Battle Animation")]

    [SerializeField]
    private BattleAnimationController battleAnimation;

    [Header("Battle Audio")]

    [SerializeField]
    private BattleAudioController battleAudio;


    //==================================================
    // TIMING
    //==================================================

    [Header("Timing")]

    [SerializeField]
    private float feedbackDuration = 1.2f;


    //==================================================
    // CURRENT BATTLE STATE
    //==================================================

    private BattleTurn currentTurn;

    private SpellTier currentSpellTier;

    private bool questionActive = false;

    private bool isResolving = false;

    private bool battleEnded = false;


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
        InitializeBattle();
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


        //==============================================
        // QUESTION SYSTEM
        //==============================================

        questionManager.Initialize(
            levelConfig
        );


        //==============================================
        // UI
        //==============================================

        battleUI.Initialize(
            levelConfig
        );


        //==============================================
        // INPUT
        //==============================================

        numpadInput.ResetInput();

        spellController.ResetSelection();


        //==============================================
        // STATE
        //==============================================

        battleEnded = false;
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


        currentTurn =
            BattleTurn.Player;


        questionActive = false;
        isResolving = false;


        questionManager.ClearCurrentQuestion();

        numpadInput.ResetInput();

        spellController.ResetSelection();


        battleUI.ShowPlayerTurn();


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

        numpadInput.ResetInput();


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

        bool success =
            questionManager.GenerateQuestion(
                spellTier
            );


        if (!success)
        {
            Debug.LogError(
                "BattleManager: " +
                "Gagal membuat soal Player."
            );

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


        currentTurn =
            BattleTurn.Enemy;


        questionActive = false;
        isResolving = false;


        questionManager.ClearCurrentQuestion();

        numpadInput.ResetInput();

        spellController.ResetSelection();


        battleUI.ShowEnemyTurn();


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


        numpadInput.ResetInput();


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

        bool success =
            questionManager.GenerateQuestion(
                SpellTier.Defense
            );


        if (!success)
        {
            Debug.LogError(
                "BattleManager: " +
                "Gagal membuat soal Defense."
            );

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
        // AMBIL JAWABAN DARI NUMPAD
        //==============================================

        if (!numpadInput.TryGetAnswer(
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
            battleUI.SetWarning(false);
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

            battleUI.ShowCorrectFeedback();

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
        //==================================================
        // DEFENSE BERHASIL
        //==================================================

        if (isCorrect)
        {
            battleUI.ShowCorrectFeedback();

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
                    }
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
                .PlayEnemyAttackUntilImpact();


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
        battleEnded = true;
        isResolving = false;
        questionActive = false;


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
        isResolving = false;
        questionActive = false;


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
        StopAllCoroutines();

        battleAnimation.ResetVisualState();


        //==================================================
        // RESET BATTLE STATE
        //==================================================

        battleEnded = false;
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

        // PENTING:
        //
        // Soal lama TIDAK dihapus.
        // Index soal kembali ke awal.
        //
        // Jadi pertanyaan yang pernah muncul
        // akan digunakan lagi.
        questionManager.RestartSessionAttempt();


        //==================================================
        // RESET SPELL & INPUT
        //==================================================

        spellController.ResetSelection();

        numpadInput.ResetInput();


        //==================================================
        // RESET UI
        //==================================================

        battleUI.HideResultPopups();

        battleUI.HideFeedback();

        battleUI.SetWarning(false);


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


        if (numpadInput == null)
        {
            Debug.LogError(
                "BattleManager: NumpadInputManager kosong."
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


        return true;
    }
}
