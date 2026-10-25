using UnityEngine;
using UnityEngine.UI;

public class AnswerSlotInputManager : MonoBehaviour
{
    //==================================================
    // CONSTANT
    //==================================================

    private const int MaxSlotCount = AnswerSlotGroup.MaxSupportedSlots;


    //==================================================
    // REFERENCES
    //==================================================

    [Header("References")]

    [SerializeField]
    private AnswerSlotGroup defaultAnswerSlotGroup;

    [SerializeField]
    private Button castButton;


    //==================================================
    // DEFAULT MODE
    //==================================================

    [Header("Default Input Mode")]

    [SerializeField]
    private AnswerInputMode defaultInputMode =
        AnswerInputMode.ColumnRightToLeft;


    //==================================================
    // CURRENT STATE
    //==================================================

    private AnswerSlotGroup activeAnswerSlotGroup;

    private AnswerInputMode currentInputMode =
        AnswerInputMode.ColumnRightToLeft;


    //==================================================
    // RIGHT TO LEFT DATA
    //==================================================

    private readonly int[] digits =
        new int[MaxSlotCount];

    private readonly bool[] slotFilled =
        new bool[MaxSlotCount];


    // 3 = satuan
    // 2 = puluhan
    // 1 = ratusan
    // 0 = ribuan
    private int nextSlotIndex =
        MaxSlotCount - 1;


    //==================================================
    // NORMAL NUMBER DATA
    //==================================================

    // Contoh:
    //
    // player tekan 4 lalu 9
    //
    // normalInput = "49"
    //
    // UI:
    // [ ][ ][4][9]

    private string normalInput = "";

    private int CurrentSlotCount
    {
        get
        {
            if (activeAnswerSlotGroup == null)
                return MaxSlotCount;

            int capacity = activeAnswerSlotGroup.SlotCapacity;
            return Mathf.Clamp(capacity, 1, MaxSlotCount);
        }
    }


    //==================================================
    // PROPERTIES
    //==================================================

    public AnswerInputMode CurrentInputMode =>
        currentInputMode;


    public bool HasInput
    {
        get
        {
            if (
                currentInputMode ==
                AnswerInputMode.NormalNumber
            )
            {
                return
                    !string.IsNullOrEmpty(
                        normalInput
                    );
            }


            for (
                int i = 0;
                i < CurrentSlotCount;
                i++
            )
            {
                if (slotFilled[i])
                {
                    return true;
                }
            }


            return false;
        }
    }


    public bool IsFull
    {
        get
        {
            if (
                currentInputMode ==
                AnswerInputMode.NormalNumber
            )
            {
                return
                    normalInput.Length >=
                    CurrentSlotCount;
            }


            return
                nextSlotIndex < 0;
        }
    }


    public int InputDigitCount
    {
        get
        {
            if (
                currentInputMode ==
                AnswerInputMode.NormalNumber
            )
            {
                return
                    normalInput.Length;
            }


            int count = 0;


            for (
                int i = 0;
                i < CurrentSlotCount;
                i++
            )
            {
                if (slotFilled[i])
                {
                    count++;
                }
            }


            return count;
        }
    }


    //==================================================
    // START
    //==================================================

    private void Start()
    {
        // QuestionDisplayController mungkin sudah menentukan
        // AnswerSlotGroup sebelum Start() dijalankan.
        //
        // Karena itu, default hanya digunakan jika
        // belum ada group aktif.

        if (
            activeAnswerSlotGroup == null &&
            defaultAnswerSlotGroup != null
        )
        {
            SetActiveGroup(
                defaultAnswerSlotGroup,
                defaultInputMode
            );
        }
    }


    //==================================================
    // SET ACTIVE GROUP
    //==================================================

    public void SetActiveGroup(
        AnswerSlotGroup group,
        AnswerInputMode inputMode
    )
    {
        // Hilangkan highlight dari group lama.
        if (
            activeAnswerSlotGroup != null &&
            activeAnswerSlotGroup != group
        )
        {
            activeAnswerSlotGroup
                .ClearActiveSlot();
        }


        activeAnswerSlotGroup =
            group;

        currentInputMode =
            inputMode;


        ResetInput();
    }


    //==================================================
    // SET INPUT MODE
    //==================================================

    public void SetInputMode(
        AnswerInputMode inputMode
    )
    {
        currentInputMode =
            inputMode;


        ResetInput();
    }


    //==================================================
    // INPUT DIGIT
    //==================================================

    public void InputDigit(
        int digit
    )
    {
        if (
            activeAnswerSlotGroup ==
            null
        )
        {
            Debug.LogWarning(
                "AnswerSlotInputManager: " +
                "Active AnswerSlotGroup belum diisi."
            );

            return;
        }


        if (
            digit < 0 ||
            digit > 9
        )
        {
            Debug.LogWarning(
                "AnswerSlotInputManager: " +
                "Digit harus antara 0 sampai 9."
            );

            return;
        }


        //==============================================
        // PILIH MODE INPUT
        //==============================================

        switch (currentInputMode)
        {
            case AnswerInputMode.ColumnRightToLeft:

                InputColumnRightToLeft(
                    digit
                );

                break;


            case AnswerInputMode.NormalNumber:

                InputNormalNumber(
                    digit
                );

                break;
        }


        UpdateCastButtonState();
    }


    //==================================================
    // COLUMN RIGHT TO LEFT
    //==================================================

    private void InputColumnRightToLeft(
        int digit
    )
    {
        // Semua slot sudah penuh.
        if (nextSlotIndex < 0)
            return;


        digits[nextSlotIndex] =
            digit;

        slotFilled[nextSlotIndex] =
            true;


        activeAnswerSlotGroup.SetDigit(
            nextSlotIndex,
            digit.ToString()
        );


        // Pindah satu kolom ke kiri.
        nextSlotIndex--;


        UpdateActiveSlotVisual();
    }


    //==================================================
    // NORMAL NUMBER
    //==================================================

    private void InputNormalNumber(
        int digit
    )
    {
        if (
            normalInput.Length >=
            CurrentSlotCount
        )
        {
            return;
        }


        //==============================================
        // HANDLE LEADING ZERO
        //==============================================

        // Kalau input sebelumnya hanya "0",
        // angka berikutnya menggantikan 0.
        //
        // Contoh:
        //
        // 0 lalu 5
        // menjadi 5
        //
        // bukan 05.

        if (normalInput == "0")
        {
            if (digit == 0)
            {
                return;
            }


            normalInput =
                digit.ToString();
        }
        else
        {
            normalInput +=
                digit.ToString();
        }


        RenderNormalInput();
    }


    //==================================================
    // RENDER NORMAL INPUT
    //==================================================

    private void RenderNormalInput()
    {
        if (
            activeAnswerSlotGroup ==
            null
        )
        {
            return;
        }


        //==============================================
        // RESET VISUAL / INTERNAL SLOT
        //==============================================

        for (
            int i = 0;
            i < CurrentSlotCount;
            i++
        )
        {
            digits[i] = 0;

            slotFilled[i] =
                false;
        }


        activeAnswerSlotGroup
            .ClearAll();


        if (
            string.IsNullOrEmpty(
                normalInput
            )
        )
        {
            UpdateActiveSlotVisual();

            return;
        }


        //==============================================
        // RATA KANAN
        //==============================================

        int startIndex =
            CurrentSlotCount -
            normalInput.Length;


        for (
            int i = 0;
            i < normalInput.Length;
            i++
        )
        {
            int slotIndex =
                startIndex + i;


            int digit =
                normalInput[i] - '0';


            digits[slotIndex] =
                digit;

            slotFilled[slotIndex] =
                true;


            activeAnswerSlotGroup
                .SetDigit(
                    slotIndex,
                    digit.ToString()
                );
        }


        UpdateActiveSlotVisual();
    }


    //==================================================
    // BACKSPACE
    //==================================================

    public void Backspace()
    {
        if (
            activeAnswerSlotGroup ==
            null
        )
        {
            return;
        }


        if (!HasInput)
            return;


        switch (currentInputMode)
        {
            //==========================================
            // COLUMN MODE
            //==========================================

            case AnswerInputMode.ColumnRightToLeft:

                BackspaceColumnMode();

                break;


            //==========================================
            // NORMAL MODE
            //==========================================

            case AnswerInputMode.NormalNumber:

                BackspaceNormalMode();

                break;
        }


        UpdateCastButtonState();
    }


    //==================================================
    // BACKSPACE COLUMN MODE
    //==================================================

    private void BackspaceColumnMode()
    {
        int slotToClear =
            nextSlotIndex + 1;


        if (
            slotToClear < 0 ||
            slotToClear >= CurrentSlotCount
        )
        {
            return;
        }


        digits[slotToClear] =
            0;

        slotFilled[slotToClear] =
            false;


        activeAnswerSlotGroup
            .ClearDigit(
                slotToClear
            );


        nextSlotIndex =
            slotToClear;


        UpdateActiveSlotVisual();
    }


    //==================================================
    // BACKSPACE NORMAL MODE
    //==================================================

    private void BackspaceNormalMode()
    {
        if (
            string.IsNullOrEmpty(
                normalInput
            )
        )
        {
            return;
        }


        normalInput =
            normalInput.Substring(
                0,
                normalInput.Length - 1
            );


        RenderNormalInput();
    }


    //==================================================
    // CLEAR INPUT
    //==================================================

    public void ClearInput()
    {
        ResetInput();
    }


    //==================================================
    // RESET INPUT
    //==================================================

    public void ResetInput()
    {
        for (
            int i = 0;
            i < MaxSlotCount;
            i++
        )
        {
            digits[i] = 0;

            slotFilled[i] =
                false;
        }


        nextSlotIndex =
            CurrentSlotCount - 1;


        normalInput =
            "";


        if (
            activeAnswerSlotGroup !=
            null
        )
        {
            activeAnswerSlotGroup
                .ClearAll();


            UpdateActiveSlotVisual();
        }


        UpdateCastButtonState();
    }


    //==================================================
    // TRY GET ANSWER
    //==================================================

    public bool TryGetAnswer(
        out int answer
    )
    {
        answer = 0;


        if (!HasInput)
            return false;


        //==============================================
        // NORMAL MODE
        //==============================================

        if (
            currentInputMode ==
            AnswerInputMode.NormalNumber
        )
        {
            return int.TryParse(
                normalInput,
                out answer
            );
        }


        //==============================================
        // COLUMN MODE
        //==============================================

        for (
            int i = 0;
            i < CurrentSlotCount;
            i++
        )
        {
            if (!slotFilled[i])
                continue;


            answer =
                (answer * 10) +
                digits[i];
        }


        return true;
    }


    //==================================================
    // GET INPUT STRING
    //==================================================

    public string GetInputString()
    {
        if (!HasInput)
            return "";


        //==============================================
        // NORMAL
        //==============================================

        if (
            currentInputMode ==
            AnswerInputMode.NormalNumber
        )
        {
            return normalInput;
        }


        //==============================================
        // COLUMN
        //==============================================

        string result = "";


        for (
            int i = 0;
            i < CurrentSlotCount;
            i++
        )
        {
            if (!slotFilled[i])
                continue;


            result +=
                digits[i].ToString();
        }


        return result;
    }


    //==================================================
    // ACTIVE SLOT
    //==================================================

    private void UpdateActiveSlotVisual()
    {
        if (
            activeAnswerSlotGroup ==
            null
        )
        {
            return;
        }


        //==============================================
        // NORMAL NUMBER
        //==============================================

        if (
            currentInputMode ==
            AnswerInputMode.NormalNumber
        )
        {
            if (
                normalInput.Length >=
                CurrentSlotCount
            )
            {
                activeAnswerSlotGroup
                    .ClearActiveSlot();
            }
            else
            {
                // Normal input selalu dianggap
                // mengetik dari posisi kanan,
                // lalu angka sebelumnya bergeser kiri.
                activeAnswerSlotGroup
                    .SetActiveSlot(
                        CurrentSlotCount - 1
                    );
            }


            return;
        }


        //==============================================
        // COLUMN RIGHT TO LEFT
        //==============================================

        if (nextSlotIndex < 0)
        {
            activeAnswerSlotGroup
                .ClearActiveSlot();

            return;
        }


        activeAnswerSlotGroup
            .SetActiveSlot(
                nextSlotIndex
            );
    }


    //==================================================
    // CAST BUTTON
    //==================================================

    private void UpdateCastButtonState()
    {
        if (castButton == null)
            return;


        castButton.interactable =
            HasInput;
    }


    //==================================================
    // TEST COLUMN MODE
    //==================================================

    [ContextMenu(
        "TEST - Column 85"
    )]
    private void TestColumn85()
    {
        SetInputMode(
            AnswerInputMode.ColumnRightToLeft
        );


        // 85 diketik:
        // 5 → 8

        InputDigit(5);

        InputDigit(8);


        if (
            TryGetAnswer(
                out int answer
            )
        )
        {
            Debug.Log(
                "Column Test = " +
                answer
            );
        }
    }


    //==================================================
    // TEST NORMAL 49
    //==================================================

    [ContextMenu(
        "TEST - Normal 49"
    )]
    private void TestNormal49()
    {
        SetInputMode(
            AnswerInputMode.NormalNumber
        );


        // 49 diketik normal:
        // 4 → 9

        InputDigit(4);

        InputDigit(9);


        if (
            TryGetAnswer(
                out int answer
            )
        )
        {
            Debug.Log(
                "Normal Test = " +
                answer
            );
        }
    }


    //==================================================
    // TEST NORMAL 100
    //==================================================

    [ContextMenu(
        "TEST - Normal 100"
    )]
    private void TestNormal100()
    {
        SetInputMode(
            AnswerInputMode.NormalNumber
        );


        InputDigit(1);

        InputDigit(0);

        InputDigit(0);


        if (
            TryGetAnswer(
                out int answer
            )
        )
        {
            Debug.Log(
                "Normal Test = " +
                answer
            );
        }
    }


    //==================================================
    // TEST BACKSPACE
    //==================================================

    [ContextMenu(
        "TEST - Backspace"
    )]
    private void TestBackspace()
    {
        Backspace();
    }


    //==================================================
    // TEST CLEAR
    //==================================================

    [ContextMenu(
        "TEST - Clear"
    )]
    private void TestClear()
    {
        ClearInput();
    }
}