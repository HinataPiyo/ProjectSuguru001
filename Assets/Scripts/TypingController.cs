using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class TypingController : MonoBehaviour
{
    UIDocument uiDoc;

    Label sentenceText;
    Label romanizedSentenceText;

    string sentence = "こんにちはタイピングゲーム";
    string romanizedSentence = "konnichiwataipinguge-mu";
    string inputKey = "";

    void Awake()
    {
        uiDoc = GetComponent<UIDocument>();
    }

    void OnEnable()
    {
        var r = uiDoc.rootVisualElement;
        sentenceText = r.Q<Label>("sentence-text-value");
        romanizedSentenceText = r.Q<Label>("romanized-sentence-text-value");

        //! テスト: 問題文と問題文のローマ字表記を表示
        sentenceText.text = sentence;
        romanizedSentenceText.text = romanizedSentence;

        // 入力された文字列と残りの文字列を表示する
        UpdateInputText();

        Keyboard.current.onTextInput += OnTextInput;        // 入力イベントの登録
    }

    void OnDisable()
    {
        Keyboard.current.onTextInput -= OnTextInput;        // 入力イベントの解除
    }

    /// <summary>
    /// 入力された文字が正しいかどうかを判定する
    /// </summary>
    /// <param name="currentKey">入力された文字</param>
    void OnTextInput(char currentKey)
    {
        bool isCorrect = IsInputCorrect(currentKey);
        Debug.Log($"Input: {currentKey}, Correct: {isCorrect}");

        if(!isCorrect) return;      // 入力が正しくない場合は処理を終了

        inputKey += currentKey;
        UpdateInputText();
    }

    /// <summary>
    /// 入力された文字列と残りの文字列を表示する
    /// </summary>
    void UpdateInputText()
    {
        string typed = inputKey;        // 入力された文字列を取得
        string remaining = romanizedSentence.Substring(inputKey.Length);        // 入力された文字列の長さを基準に残りの文字列を取得
        romanizedSentenceText.text = $"<color=#00FF00>{typed}</color><color=#FFFFFF>{remaining}</color>";
    }

    /// <summary>
    /// 入力された文字が正しいかどうかを判定する
    /// </summary>
    bool IsInputCorrect(char key)
    {
        string expectedKey = inputKey + key;
        return romanizedSentence.StartsWith(expectedKey);
    }
    
}
