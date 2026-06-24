using UnityEngine;
using UnityEngine.InputSystem;
using System.Text;

public class TypingController : MonoBehaviour
{
    string inputKey = "";
    int currentKanaIndex = 0;
    string currentKanaInput = "";
    string rubySentence = "";

    [SerializeField] QuestionController questionCTRL;

    void Awake()
    {
        if (questionCTRL == null)
        {
            questionCTRL = GetComponent<QuestionController>();
        }
    }

    

    void OnEnable()
    {
        ResetTypingState(questionCTRL != null ? questionCTRL.CurrentRuby : "");

        if (questionCTRL != null)
        {
            questionCTRL.OnQuestionChanged += OnQuestionChanged;
        }

        if (Keyboard.current != null)
        {
            Keyboard.current.onTextInput += OnTextInput;        // 入力イベントの登録
        }
    }

    void OnDisable()
    {
        if (questionCTRL != null)
        {
            questionCTRL.OnQuestionChanged -= OnQuestionChanged;
        }

        if (Keyboard.current != null)
        {
            Keyboard.current.onTextInput -= OnTextInput;        // 入力イベントの解除
        }
    }

    void OnQuestionChanged(string ruby)
    {
        ResetTypingState(ruby);
    }

    void ResetTypingState(string ruby)
    {
        inputKey = "";
        currentKanaIndex = 0;
        currentKanaInput = "";
        rubySentence = ruby ?? "";
        UpdateInputText();
    }

    /// <summary>
    /// 入力された文字が正しいかどうかを判定する
    /// </summary>
    /// <param name="currentKey">入力された文字</param>
    void OnTextInput(char currentKey)
    {
        if (questionCTRL == null) return;
        if(questionCTRL.IsCompleted) return;      // 入力が完了している場合は処理を終了

        char normalizedKey = char.ToLowerInvariant(currentKey);
        if (!IsSupportedInputChar(normalizedKey)) return;

        bool isCorrect = TryConsumeRomanInput(normalizedKey);
        Debug.Log($"Input: {normalizedKey}, Correct: {isCorrect}");

        if(!isCorrect) return;      // 入力が正しくない場合は処理を終了

        inputKey += normalizedKey;
        UpdateInputText();

        // 入力が完了しているかどうかを判定する
        if(currentKanaIndex >= rubySentence.Length)
        {
            questionCTRL.CompleteQuestion();
        }
    }

    /// <summary>
    /// 入力された文字列と残りの文字列を表示する
    /// </summary>
    void UpdateInputText()
    {
        string typed = inputKey;        // 入力された文字列を取得
        string remaining = BuildRemainingRomanizedGuide();
        questionCTRL.SetRomanizedSentenceText($"<color=#00FF00>{typed}</color><color=#FFFFFF>{remaining}</color>");
    }

    string BuildRemainingRomanizedGuide()
    {
        if (string.IsNullOrEmpty(rubySentence)) return "";

        StringBuilder builder = new StringBuilder();

        // 現在のかな文字から残りの文字列を構築する
        for (int i = currentKanaIndex; i < rubySentence.Length; i++)
        {
            char kana = rubySentence[i];
            // かな文字に対応するローマ字候補を取得し、最初の候補を表示する
            if (HiraganaRomajiDictionary.TryGetCandidates(kana, out var candidates) && candidates.Length > 0)
            {
                string head = candidates[0];

                // 現在の入力文字列が候補の接頭一致する場合は、残りの文字列を表示する
                if (i == currentKanaIndex && !string.IsNullOrEmpty(currentKanaInput))
                {
                    if (head.StartsWith(currentKanaInput))
                    {
                        // 入力済みの文字列を除いた残りの文字列を表示する
                        builder.Append(head.Substring(currentKanaInput.Length));
                    }
                    else
                    {
                        builder.Append(head);
                    }
                }
                else
                {
                    // 入力済みの文字列がない場合は、候補の最初の文字列を表示する
                    builder.Append(head);
                }
            }
            else
            {
                // かな文字に対応するローマ字候補が存在しない場合は、かな文字自体を表示する
                builder.Append(kana);
            }
        }

        return builder.ToString();
    }

    /// <summary>
    /// 入力された文字が正しいかどうかを判定する
    /// </summary>
    bool IsSupportedInputChar(char key)
    {
        return (key >= 'a' && key <= 'z') || key == '-';
    }

    /// <summary>
    /// かな1文字に対するローマ字候補の接頭一致で入力可否を判定する
    /// </summary>
    bool TryConsumeRomanInput(char key)
    {
        if (currentKanaIndex >= rubySentence.Length) return false;      //  すでに全てのかなを入力済みの場合はfalseを返す

        char kana = rubySentence[currentKanaIndex];     // 現在のかな文字を取得
        if (!HiraganaRomajiDictionary.TryGetCandidates(kana, out var candidates)) return false;     // かな文字に対応するローマ字候補が存在しない場合はfalseを返す

        string nextInput = currentKanaInput + key;
        bool hasPrefix = false;
        bool hasExact = false;

        // かな文字に対応するローマ字候補の中で、入力された文字列が接頭一致するかどうかを判定する
        foreach (var candidate in candidates)
        {
            if (!candidate.StartsWith(nextInput)) continue;

            hasPrefix = true;
            if (candidate == nextInput)     // 入力された文字列が候補の中に完全一致する場合はhasExactをtrueにする
            {
                hasExact = true;
            }
        }

        if (!hasPrefix) return false;       // 入力された文字列がどの候補とも接頭一致しない場合はfalseを返す

        currentKanaInput = nextInput;

        // 入力された文字列が候補の中に完全一致する場合は、次のかな文字に進める
        if (hasExact)
        {
            currentKanaIndex++;
            currentKanaInput = "";
        }

        return true;
    }

}
