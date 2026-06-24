using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SentenceDataConvert : MonoBehaviour
{
    [System.Serializable]
    public class Entry
    {
        public string sentence;
        public string ruby;
    }

    List<Entry> entries = new List<Entry>();

    public List<Entry> Questions => entries;

    public IEnumerator LoadCSV()
    {
        TextAsset csvFile = Resources.Load<TextAsset>("Sentence/000_Test_Sentence");

        if (csvFile == null)
        {
            Debug.LogError("CSVの読み込みに失敗しました: Resources/Sentence/000_Test_Sentence.csv を確認してください。");
            yield break;
        }

        string[] lines = csvFile.text.Split('\n');

        for (int i = 1; i < lines.Length; i++) // 1行目はヘッダ
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line))
                continue;

            // カンマ区切りで分ける
            string[] columns = line.Split(',');
            if (columns.Length < 2)
                continue;

            string sentence = columns[0].Trim();
            string ruby = columns[1].Trim();

            entries.Add(new Entry{ sentence = sentence, ruby = ruby });
        }

        Debug.Log($"読込件数 : {entries.Count}");
    }
}