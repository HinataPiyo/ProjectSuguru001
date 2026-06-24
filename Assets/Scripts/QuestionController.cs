using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class QuestionController : MonoBehaviour
{
    [SerializeField] SentenceDataConvert sentenceDataConvert;

    public List<SentenceDataConvert.Entry> Questions => sentenceDataConvert.Questions;
    bool isCompleted = false;
    public bool IsCompleted => isCompleted;
    public string CurrentSentence => currentSentence;
    public string CurrentRuby => currentRuby;
    public event System.Action<string> OnQuestionChanged;

    UIDocument uiDoc;

    Label sentenceText;
    Label romanizedSentenceText;

    string currentSentence = "こんにちはタイピングゲームへ";
    string currentRuby = "こんにちはたいぴんぐげーむへ";

    int maxQuestionCount = 5;
    int currentQuestionCount = 0;
    HashSet<string> usedQuestions = new HashSet<string>();

    void Awake()
    {
        uiDoc = GetComponent<UIDocument>();
        var r = uiDoc.rootVisualElement;
        sentenceText = r.Q<Label>("sentence-text-value");
        romanizedSentenceText = r.Q<Label>("romanized-sentence-text-value");

        sentenceText.text = "";
        romanizedSentenceText.text = "";
    }

    public void CompleteQuestion()
    {
        isCompleted = true;
        Debug.Log("Question completed!");
    }

    // ローマ字表記の表示を更新するメソッド
    public void SetRomanizedSentenceText(string text) => romanizedSentenceText.text = text;

    void Start() => StartCoroutine(Looping());

    IEnumerator Looping()
    {
        yield return sentenceDataConvert.LoadCSV();     // CSVの読み込みが完了するまで待機

        for (int i = 0; i < maxQuestionCount; i++)
        {
            currentQuestionCount++;
            isCompleted = false;

            // 未使用の問題だけを候補にして選ぶ
            List<SentenceDataConvert.Entry> available = new List<SentenceDataConvert.Entry>();
            foreach (var q in Questions)
            {
                if (!usedQuestions.Contains(q.sentence))
                {
                    available.Add(q);
                }
            }

            if (available.Count == 0)
            {
                yield break;
            }

            int randomIndex = Random.Range(0, available.Count);
            SentenceDataConvert.Entry question = available[randomIndex];
            usedQuestions.Add(question.sentence);

            currentSentence = question.sentence;
            currentRuby = question.ruby;

            // 問題文とひらがな表記を表示
            sentenceText.text = currentSentence;
            romanizedSentenceText.text = currentRuby;
            OnQuestionChanged?.Invoke(currentRuby);

            // 入力が完了するまで待機
            while (!isCompleted)
            {
                yield return null;
            }

            // 入力が完了したら次の問題へ

            // 最大問題数に達したらループを終了
            if(currentQuestionCount >= maxQuestionCount)
            {
                Debug.Log("All questions completed!");
                break;
            }
        }
    }
}