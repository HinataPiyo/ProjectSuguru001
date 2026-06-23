using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using System.Text;

public class TypingController : MonoBehaviour
{
    UIDocument uiDoc;

    Label sentenceText;
    Label romanizedSentenceText;

    string sentence = "こんにちはタイピングゲームへ";
    string rubySentence = "こんにちはたいぴんぐげーむへ";
    string inputKey = "";
    int currentKanaIndex = 0;
    string currentKanaInput = "";

    void Awake()
    {
        uiDoc = GetComponent<UIDocument>();
    }

    void OnEnable()
    {
        var r = uiDoc.rootVisualElement;
        sentenceText = r.Q<Label>("sentence-text-value");
        romanizedSentenceText = r.Q<Label>("romanized-sentence-text-value");

        //! テスト: 問題文と問題文のひらがな表記を表示
        sentenceText.text = sentence;
        romanizedSentenceText.text = BuildRemainingRomanizedGuide();

        inputKey = "";
        currentKanaIndex = 0;
        currentKanaInput = "";

        // 入力された文字列と残りの文字列を表示する
        UpdateInputText();

        if (Keyboard.current != null)
        {
            Keyboard.current.onTextInput += OnTextInput;        // 入力イベントの登録
        }
    }

    void OnDisable()
    {
        if (Keyboard.current != null)
        {
            Keyboard.current.onTextInput -= OnTextInput;        // 入力イベントの解除
        }
    }

    /// <summary>
    /// 入力された文字が正しいかどうかを判定する
    /// </summary>
    /// <param name="currentKey">入力された文字</param>
    void OnTextInput(char currentKey)
    {
        char normalizedKey = char.ToLowerInvariant(currentKey);
        if (!IsSupportedInputChar(normalizedKey)) return;

        bool isCorrect = TryConsumeRomanInput(normalizedKey);
        Debug.Log($"Input: {normalizedKey}, Correct: {isCorrect}");

        if(!isCorrect) return;      // 入力が正しくない場合は処理を終了

        inputKey += normalizedKey;
        UpdateInputText();
    }

    /// <summary>
    /// 入力された文字列と残りの文字列を表示する
    /// </summary>
    void UpdateInputText()
    {
        string typed = inputKey;        // 入力された文字列を取得
        string remaining = BuildRemainingRomanizedGuide();
        romanizedSentenceText.text = $"<color=#00FF00>{typed}</color><color=#FFFFFF>{remaining}</color>";
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

    /// <summary>
    /// 残りのかなをガイド用ローマ字に変換して返す
    /// </summary>
    string BuildRemainingRomanizedGuide()
    {
        if (currentKanaIndex >= rubySentence.Length) return "";     // すでに全てのかなを入力済みの場合は空文字を返す

        var sb = new StringBuilder();

        char currentKana = rubySentence[currentKanaIndex];

        // 現在のかな文字に対応するローマ字候補を取得し、最初の候補をガイド用ローマ字として使用する
        if (HiraganaRomajiDictionary.TryGetCandidates(currentKana, out var currentCandidates) && currentCandidates.Length > 0)
        {
            string currentGuide = currentCandidates[0];

            // 現在のかな文字に対応するローマ字候補の中で、入力された文字列が接頭一致する場合は、ガイド用ローマ字から入力済みの文字列を削除する
            if (currentGuide.StartsWith(currentKanaInput))
            {
                currentGuide = currentGuide.Substring(currentKanaInput.Length);
            }
            sb.Append(currentGuide);        // 現在のかな文字に対応するローマ字候補の最初の候補をガイド用ローマ字として使用する
        }
        else
        {
            sb.Append(currentKana);     // 現在のかな文字に対応するローマ字候補が存在しない場合は、かな文字をそのままガイド用ローマ字として使用する
        }

        // 残りのかな文字に対応するローマ字候補を取得し、最初の候補をガイド用ローマ字として使用する
        for (int i = currentKanaIndex + 1; i < rubySentence.Length; i++)
        {
            char kana = rubySentence[i];

            // かな文字に対応するローマ字候補を取得し、最初の候補をガイド用ローマ字として使用する
            if (HiraganaRomajiDictionary.TryGetCandidates(kana, out var candidates) && candidates.Length > 0)
            {
                sb.Append(candidates[0]);       // かな文字に対応するローマ字候補の最初の候補をガイド用ローマ字として使用する
            }
            else
            {
                sb.Append(kana);        // かな文字に対応するローマ字候補が存在しない場合は、かな文字をそのままガイド用ローマ字として使用する
            }
        }

        return sb.ToString();
    }
    
}
