using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellController : MonoBehaviour
{
    //==================================================
    // REFERENCES
    //==================================================

    [Header("References")]

    [SerializeField]
    private BattleUIManager battleUI;

    [SerializeField]
    private BattleManager battleManager;


    //==================================================
    // SPELL DAMAGE
    //==================================================

    [Header("Spell Damage")]

    [SerializeField]
    private int basicDamage = 1;

    [SerializeField]
    private int mediumDamage = 2;

    [SerializeField]
    private int advancedDamage = 3;


    //==================================================
    // CURRENT SELECTION
    //==================================================

    private SpellTier? selectedSpell = null;


    //==================================================
    // PROPERTIES
    //==================================================

    public bool HasSelectedSpell =>
        selectedSpell.HasValue;

    public SpellTier? SelectedSpell =>
        selectedSpell;


    //==================================================
    // BASIC
    //==================================================

    public void SelectBasicSpell()
    {
        HandleSpellSelection(
            SpellTier.Basic
        );
    }


    //==================================================
    // MEDIUM
    //==================================================

    public void SelectMediumSpell()
    {
        HandleSpellSelection(
            SpellTier.Medium
        );
    }


    //==================================================
    // ADVANCED
    //==================================================

    public void SelectAdvancedSpell()
    {
        HandleSpellSelection(
            SpellTier.Advanced
        );
    }


    //==================================================
    // HANDLE SELECTION
    //==================================================

    private void HandleSpellSelection(
        SpellTier spellTier
    )
    {
        if (spellTier == SpellTier.Defense)
            return;


        //==============================================
        // TAP PERTAMA / GANTI PILIHAN
        //==============================================

        if (!selectedSpell.HasValue ||
            selectedSpell.Value != spellTier)
        {
            selectedSpell = spellTier;


            int damage =
                GetSpellDamage(
                    spellTier
                );


            if (battleUI != null)
            {
                battleUI.ShowSpellPreview(
                    spellTier,
                    damage
                );
            }


            Debug.Log(
                $"Spell dipilih: {spellTier}. " +
                $"Damage {damage}. " +
                $"Tap lagi untuk merapal."
            );


            return;
        }


        //==============================================
        // TAP KEDUA
        //==============================================

        ConfirmSelectedSpell();
    }


    //==================================================
    // CONFIRM
    //==================================================

    private void ConfirmSelectedSpell()
    {
        if (!selectedSpell.HasValue)
            return;


        SpellTier spellTier =
            selectedSpell.Value;


        Debug.Log(
            $"Spell dikonfirmasi: " +
            $"{spellTier}"
        );


        if (battleManager != null)
        {
            battleManager.ConfirmPlayerSpell(
                spellTier
            );
        }
        else
        {
            Debug.LogError(
                "SpellController: " +
                "BattleManager belum diisi."
            );
        }
    }


    //==================================================
    // GET DAMAGE
    //==================================================

    public int GetSpellDamage(
        SpellTier spellTier
    )
    {
        switch (spellTier)
        {
            case SpellTier.Basic:
                return basicDamage;

            case SpellTier.Medium:
                return mediumDamage;

            case SpellTier.Advanced:
                return advancedDamage;

            default:
                return 0;
        }
    }


    //==================================================
    // GET SELECTED DAMAGE
    //==================================================

    public int GetSelectedSpellDamage()
    {
        if (!selectedSpell.HasValue)
            return 0;


        return GetSpellDamage(
            selectedSpell.Value
        );
    }


    //==================================================
    // GET SELECTED SPELL
    //==================================================

    public SpellTier GetSelectedSpell()
    {
        if (!selectedSpell.HasValue)
        {
            Debug.LogWarning(
                "SpellController: " +
                "Belum ada spell dipilih."
            );

            return SpellTier.Basic;
        }


        return selectedSpell.Value;
    }


    //==================================================
    // RESET
    //==================================================

    public void ResetSelection()
    {
        selectedSpell = null;


        if (battleUI != null)
        {
            battleUI.HideSpellPreview();
        }
    }
}