using System.Collections.Generic;

public static class HiraganaRomajiDictionary
{
    // かな1文字ごとのローマ字候補。先頭要素はガイド表示用の既定表記。
    static readonly Dictionary<char, string[]> Map = new Dictionary<char, string[]>
    {
        // あ行
        { 'あ', new[] { "a" } },
        { 'い', new[] { "i", "yi" } },
        { 'う', new[] { "u", "wu", "whu" } },
        { 'え', new[] { "e" } },
        { 'お', new[] { "o" } },

        // か行
        { 'か', new[] { "ka", "ca" } },
        { 'き', new[] { "ki" } },
        { 'く', new[] { "ku", "cu", "qu" } },
        { 'け', new[] { "ke" } },
        { 'こ', new[] { "ko", "co" } },

        // さ行
        { 'さ', new[] { "sa" } },
        { 'し', new[] { "shi", "si", "ci" } },
        { 'す', new[] { "su" } },
        { 'せ', new[] { "se", "ce" } },
        { 'そ', new[] { "so" } },

        // た行
        { 'た', new[] { "ta" } },
        { 'ち', new[] { "chi", "ti" } },
        { 'つ', new[] { "tsu", "tu" } },
        { 'て', new[] { "te" } },
        { 'と', new[] { "to" } },

        // な行
        { 'な', new[] { "na" } },
        { 'に', new[] { "ni" } },
        { 'ぬ', new[] { "nu" } },
        { 'ね', new[] { "ne" } },
        { 'の', new[] { "no" } },

        // は行
        { 'は', new[] { "ha" } },
        { 'ひ', new[] { "hi" } },
        { 'ふ', new[] { "fu", "hu" } },
        { 'へ', new[] { "he" } },
        { 'ほ', new[] { "ho" } },

        // ま行
        { 'ま', new[] { "ma" } },
        { 'み', new[] { "mi" } },
        { 'む', new[] { "mu" } },
        { 'め', new[] { "me" } },
        { 'も', new[] { "mo" } },

        // や行
        { 'や', new[] { "ya" } },
        { 'ゆ', new[] { "yu" } },
        { 'よ', new[] { "yo" } },

        // ら行
        { 'ら', new[] { "ra" } },
        { 'り', new[] { "ri" } },
        { 'る', new[] { "ru" } },
        { 'れ', new[] { "re" } },
        { 'ろ', new[] { "ro" } },

        // わ行
        { 'わ', new[] { "wa" } },
        { 'を', new[] { "wo", "o" } },
        { 'ん', new[] { "n", "nn", "xn" } },

        // が行
        { 'が', new[] { "ga" } },
        { 'ぎ', new[] { "gi" } },
        { 'ぐ', new[] { "gu" } },
        { 'げ', new[] { "ge" } },
        { 'ご', new[] { "go" } },

        // ざ行
        { 'ざ', new[] { "za" } },
        { 'じ', new[] { "ji", "zi" } },
        { 'ず', new[] { "zu" } },
        { 'ぜ', new[] { "ze" } },
        { 'ぞ', new[] { "zo" } },

        // だ行
        { 'だ', new[] { "da" } },
        { 'ぢ', new[] { "di", "ji", "zi" } },
        { 'づ', new[] { "du", "zu" } },
        { 'で', new[] { "de" } },
        { 'ど', new[] { "do" } },

        // ば行
        { 'ば', new[] { "ba" } },
        { 'び', new[] { "bi" } },
        { 'ぶ', new[] { "bu" } },
        { 'べ', new[] { "be" } },
        { 'ぼ', new[] { "bo" } },

        // ぱ行
        { 'ぱ', new[] { "pa" } },
        { 'ぴ', new[] { "pi" } },
        { 'ぷ', new[] { "pu" } },
        { 'ぺ', new[] { "pe" } },
        { 'ぽ', new[] { "po" } },

        // 小書き文字など
        { 'ぁ', new[] { "xa", "la" } },
        { 'ぃ', new[] { "xi", "li" } },
        { 'ぅ', new[] { "xu", "lu" } },
        { 'ぇ', new[] { "xe", "le" } },
        { 'ぉ', new[] { "xo", "lo" } },
        { 'ゃ', new[] { "xya", "lya" } },
        { 'ゅ', new[] { "xyu", "lyu" } },
        { 'ょ', new[] { "xyo", "lyo" } },
        { 'っ', new[] { "xtsu", "xtu", "ltsu", "ltu" } },
        { 'ゎ', new[] { "xwa", "lwa" } },
        { 'ー', new[] { "-" } },
        { 'ゔ', new[] { "vu" } },
        { 'ゐ', new[] { "wi" } },
        { 'ゑ', new[] { "we" } },
    };

    /// <summary>
    /// かな文字に対応するローマ字候補を取得する
    /// </summary>
    /// <param name="kana">かな文字</param>
    /// <param name="candidates">ローマ字候補の配列</param>
    /// <returns>かな文字に対応するローマ字候補が存在する場合はtrue、存在しない場合はfalse</returns>
    public static bool TryGetCandidates(char kana, out string[] candidates)
    {
        return Map.TryGetValue(kana, out candidates);
    }
}
