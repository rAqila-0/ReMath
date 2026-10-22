using UnityEngine;
using UnityEngine.UI;

public class EnemyVisualController : MonoBehaviour
{
    //==================================================
    // REFERENCES
    //==================================================

    [Header("Enemy Visual")]

    [SerializeField]
    private Image enemyImage;

    [SerializeField]
    private SpriteRenderer enemySpriteRenderer;


    //==================================================
    // INITIALIZE
    //==================================================

    public void Initialize(
        BattleLevelConfig config
    )
    {
        if (config == null)
        {
            Debug.LogError(
                "EnemyVisualController: " +
                "BattleLevelConfig null."
            );

            return;
        }


        SetEnemySprite(
            config.enemySprite
        );
    }


    //==================================================
    // SET SPRITE
    //==================================================

    public void SetEnemySprite(
        Sprite sprite
    )
    {
        if (sprite == null)
        {
            Debug.LogWarning(
                "EnemyVisualController: " +
                "Enemy Sprite belum diisi."
            );

            return;
        }


        //==============================================
        // UI IMAGE
        //==============================================

        if (enemyImage != null)
        {
            enemyImage.sprite =
                sprite;

            enemyImage.preserveAspect =
                true;

            enemyImage.enabled =
                true;
        }


        //==============================================
        // SPRITE RENDERER
        //==============================================

        if (
            enemySpriteRenderer !=
            null
        )
        {
            enemySpriteRenderer.sprite =
                sprite;

            enemySpriteRenderer.enabled =
                true;
        }
    }
}