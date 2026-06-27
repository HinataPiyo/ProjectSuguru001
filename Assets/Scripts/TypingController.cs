using UnityEngine;
using UnityEngine.InputSystem;
using System;
using System.Collections.Generic;
using System.Text;

public class TypingController : MonoBehaviour
{
    const string SokuonKana = "っ";

    string inputKey = "";
    int currentKanaIndex = 0;
    string currentKanaInput = "";
    string rubySentence = "";
    readonly List<string> rubyUnits = new List<string>();

    [SerializeField] QuestionController questionController;

    void OnEnable()
    {
        // 現在の問題文に同期してからイベント購読を行う。
        ResetTypingState(questionController != null ? questionController.CurrentRuby : "");

        if (questionController != null)
        {
            questionController.OnQuestionChanged += OnQuestionChanged;
        }

        if (Keyboard.current != null)
        {
            Keyboard.current.onTextInput += OnTextInput;
        }
    }

    void OnDisable()
    {
        if (questionController != null)
        {
            questionController.OnQuestionChanged -= OnQuestionChanged;
        }

        if (Keyboard.current != null)
        {
            Keyboard.current.onTextInput -= OnTextInput;
        }
    }

    void OnQuestionChanged(string ruby)
    {
        ResetTypingState(ruby);
    }

    void ResetTypingState(string ruby)
    {
        // 問題が切り替わるたびに、入力済み状態とかな分割結果を初期化する。
        inputKey = "";
        currentKanaIndex = 0;
        currentKanaInput = "";
        rubySentence = ruby ?? "";
        BuildRubyUnits();
        UpdateInputText();
    }

    /// <summary>
    /// キーボード入力を受け取り、現在の問題に対して入力を1文字進める。
    /// </summary>
    /// <param name="currentKey">入力された文字</param>
    void OnTextInput(char currentKey)
    {
        if (!CanHandleInput()) return;

        char normalizedKey = char.ToLowerInvariant(currentKey);
        if (!IsSupportedInputChar(normalizedKey)) return;

        bool isCorrect = TryConsumeRomanInput(normalizedKey);

        if (!isCorrect) return;

        inputKey += normalizedKey;
        UpdateInputText();

        if (currentKanaIndex >= rubyUnits.Count)
        {
            questionController.CompleteQuestion();
        }
    }

    /// <summary>
    /// 問題コントローラーが有効で、かつ現在の問題が未完了の時だけ入力を受け付ける。
    /// </summary>
    bool CanHandleInput()
    {
        return questionController != null && !questionController.IsCompleted;
    }

    /// <summary>
    /// 入力された文字列と残りの文字列を表示する
    /// </summary>
    void UpdateInputText()
    {
        // 入力済み部分と未入力部分を色分けしてガイド表示する。
        string typed = inputKey;
        string remaining = BuildRemainingRomanizedGuide();
        questionController.SetRomanizedSentenceText($"<color=#00FF00>{typed}</color><color=#FFFFFF>{remaining}</color>");
    }

    string BuildRemainingRomanizedGuide()
    {
        if (rubyUnits.Count == 0) return "";

        StringBuilder builder = new StringBuilder();

        // 現在位置以降のかな単位を、表示用の代表ローマ字候補へ変換する。
        for (int i = currentKanaIndex; i < rubyUnits.Count; i++)
        {
            builder.Append(GetGuideTextForUnit(i));
        }

        return builder.ToString();
    }

    /// <summary>
    /// 指定位置のかな単位を、現在の入力状態を踏まえた表示文字列へ変換する。
    /// </summary>
    string GetGuideTextForUnit(int unitIndex)
    {
        string guide = HiraganaRomajiDictionary.GetPrimaryCandidateOrKana(rubyUnits[unitIndex]);

        if (unitIndex != currentKanaIndex || string.IsNullOrEmpty(currentKanaInput))
        {
            return guide;
        }

        return guide.StartsWith(currentKanaInput, StringComparison.Ordinal)
            ? guide.Substring(currentKanaInput.Length)
            : guide;
    }

    /// <summary>
    /// 英字入力と長音記号入力だけを受け付ける。
    /// </summary>
    bool IsSupportedInputChar(char key)
    {
        return (key >= 'a' && key <= 'z') || key == '-';
    }

    /// <summary>
    /// 現在のかな単位に対して、入力中ローマ字の接頭一致を判定する。
    /// </summary>
    bool TryConsumeRomanInput(char key)
    {
        if (currentKanaIndex >= rubyUnits.Count) return false;

        string[] candidates = GetCandidatesForCurrentKana();
        if (candidates.Length == 0) return false;

        string nextInput = currentKanaInput + key;
        bool hasPrefix = HasMatchingCandidate(candidates, nextInput, requireExactMatch: false);
        bool hasExact = HasMatchingCandidate(candidates, nextInput, requireExactMatch: true);

        if (!hasPrefix) return false;

        currentKanaInput = nextInput;

        if (hasExact)
        {
            currentKanaIndex++;
            currentKanaInput = "";
        }

        return true;
    }

    /// <summary>
    /// ローマ字候補群に対して、接頭一致または完全一致する候補があるかを調べる。
    /// </summary>
    bool HasMatchingCandidate(IEnumerable<string> candidates, string input, bool requireExactMatch)
    {
        foreach (var candidate in candidates)
        {
            bool isMatch = requireExactMatch
                ? candidate == input
                : candidate.StartsWith(input, StringComparison.Ordinal);

            if (isMatch)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 現在のかな文字に対応するローマ字候補を取得する
    /// かな文字が促音（っ）の場合は、次のかな文字のローマ字候補の頭文字も含めて取得する
    /// </summary>
    string[] GetCandidatesForCurrentKana()
    {
        if (currentKanaIndex >= rubyUnits.Count)
        {
            return Array.Empty<string>();
        }

        string kana = rubyUnits[currentKanaIndex];
        string[] candidates = HiraganaRomajiDictionary.GetCandidatesOrEmpty(kana);

        // 促音だけは、次のかなの頭子音を許容して入力しやすくする。
        if (kana != SokuonKana)
        {
            return candidates;
        }

        return MergeSokuonCandidates(candidates);
    }

    /// <summary>
    /// 促音の基本候補に加えて、次のかなの頭子音を単独入力候補として補完する。
    /// </summary>
    string[] MergeSokuonCandidates(string[] baseCandidates)
    {
        List<string> mergedCandidates = new List<string>(baseCandidates);

        if (currentKanaIndex + 1 < rubyUnits.Count &&
            HiraganaRomajiDictionary.TryGetCandidates(rubyUnits[currentKanaIndex + 1], out var nextCandidates))
        {
            foreach (var nextCandidate in nextCandidates)
            {
                if (string.IsNullOrEmpty(nextCandidate)) continue;

                char initial = nextCandidate[0];

                string initialText = initial.ToString();
                if (IsSokuonCandidateInitial(initial) && !mergedCandidates.Contains(initialText))
                {
                    mergedCandidates.Add(initialText);
                }
            }
        }

        return mergedCandidates.ToArray();
    }

    /// <summary>
    /// 促音の直後に来る頭文字として自然な子音だけを許可する。
    /// </summary>
    bool IsSokuonCandidateInitial(char initial)
    {
        return !(initial == 'a' || initial == 'i' || initial == 'u' || initial == 'e' || initial == 'o' || initial == '-');
    }

    /// <summary>
    /// かな文字列を、拗音を優先しながら入力判定単位へ分割する。
    /// </summary>
    void BuildRubyUnits()
    {
        rubyUnits.Clear();

        for (int i = 0; i < rubySentence.Length; i++)
        {
            if (i + 1 < rubySentence.Length)
            {
                // 2文字かなが候補辞書にある場合は、1文字より優先してまとめる。
                string pair = rubySentence.Substring(i, 2);
                if (HiraganaRomajiDictionary.TryGetCandidates(pair, out _))
                {
                    rubyUnits.Add(pair);
                    i++;
                    continue;
                }
            }

            rubyUnits.Add(rubySentence[i].ToString());
        }
    }

}
