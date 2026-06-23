`※AI使用`

# ProjectSuguru001

このプロジェクトは、ひらがな文章をローマ字で入力するタイピングゲームの基礎実装です。
現在の中核は次の2スクリプトです。

- `Assets/Scripts/TypingController.cs`
- `Assets/Scripts/HiraganaRomajiDictionary.cs`

## 1. TypingController の意図と役割

`TypingController` は、UI表示と入力判定をつなぐ進行管理クラスです。

- 問題文（日本語）と、ローマ字ガイドの表示更新
- キーボード文字入力の受け取り（Input System の `Keyboard.current.onTextInput`）
- 現在のかな1文字に対して、入力中ローマ字が候補の接頭一致かを判定
- 1文字分が確定したら次のかなへ進行

### 処理の流れ（概要）

1. `OnEnable()`
	- `UIDocument` からラベルを取得
	- 問題文とガイドを初期表示
	- 入力状態をリセット
	- `onTextInput` を購読
2. `OnTextInput(char key)`
	- 入力文字を正規化し、許可文字（`a-z`, `-`）のみ通す
	- `TryConsumeRomanInput()` で正誤判定
	- 正しい入力だけ `inputKey` に反映し、表示更新
3. `TryConsumeRomanInput(char key)`
	- 現在かなに対応する候補群を辞書から取得
	- 「接頭一致あり」なら入力継続
	- 「完全一致あり」ならそのかなを確定し、次へ進む
4. `BuildRemainingRomanizedGuide()`
	- 未入力部分をガイド用ローマ字に変換して表示

## 2. HiraganaRomajiDictionary の意図と役割

`HiraganaRomajiDictionary` は、かな1文字に対するローマ字候補の定義テーブルです。

- `Dictionary<char, string[]>` で候補を保持
- 先頭要素を「ガイド表示用の既定表記」として利用
- `TryGetCandidates(char kana, out string[] candidates)` で参照

### 設計上のポイント

- 例: `し` は `shi/si/ci` を許可
- 例: `ん` は `n/nn/xn` を許可
- `TypingController` 側は「接頭一致判定」で処理するため、
  候補を増やすだけで入力バリエーションを拡張できる

## 3. シーンでの使い方

1. `UIDocument` を持つ GameObject に `TypingController` をアタッチ
2. UXML 側に次の `Label` 名を用意
	- `sentence-text-value`（問題文表示）
	- `romanized-sentence-text-value`（入力済み + 残りガイド表示）
3. Input System を有効化した状態で Play
4. ローマ字入力すると、正しい入力のみ緑色で進行

## 4. カスタマイズ方法

### 問題文を変更する

`TypingController` 内の次の文字列を変更します。

- `sentence`（表示用の文章）
- `rubySentence`（入力判定用のひらがな）

注意: 現在実装は「かな1文字単位」で判定しています。
拗音（きゃ、しゅ等）を1単位として厳密に扱う場合は、
辞書と入力ロジックを2文字以上トークン対応に拡張してください。

### ローマ字候補を追加・変更する

`HiraganaRomajiDictionary` の `Map` を編集してください。

- 既定ガイドを変えたい場合: 配列の先頭要素を変更
- 入力許容を増やしたい場合: 配列へ候補を追加

## 5. 今後の拡張例

- 問題文の外部データ化（ScriptableObject / JSON）
- 正答率・WPM・ミス数のスコア表示
- 文章終了時のリザルト画面
- 複数入力方式（ヘボン式/訓令式）の切り替え
- 2文字かな（きゃ、しゃ等）や促音の厳密処理
