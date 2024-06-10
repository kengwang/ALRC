﻿using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Kawazu;

namespace ALRC.Creator.Extensions
{
    /// <summary>
    /// Tools for analyze.
    /// </summary>
    public static class KawazuExtendedUtilities
    {
        private const int KatakanaHiraganaShift = -96;
        private const int HiraganaKatakanaShift = 96;
        
        /// <summary>
        /// Check if the given char is hiragana.
        /// </summary>
        public static bool IsHiragana(char ch) => ch >= '\u3040' && ch <= '\u309f';

        /// <summary>
        /// Check if the given char is katakana.
        /// </summary>
        public static bool IsKatakana(char ch) => ch >= '\u30a0' && ch <= '\u30ff';

        /// <summary>
        /// Check if the given char is kana.
        /// </summary>
        public static bool IsKana(char ch) => IsHiragana(ch) || IsKatakana(ch);
        
        /// <summary>
        /// Check if the given char is kanji.
        /// </summary>
        public static bool IsKanji(char ch) => (ch >= '\u4e00' && ch <= '\u9fcf') ||
                                               (ch >= '\uf900' && ch <= '\ufaff') ||
                                               (ch >= '\u3400' && ch <= '\u4dbf');
        
        /// <summary>
        /// Check if the given char is Japanese character.
        /// </summary>
        public static bool IsJapanese(char ch) => IsKana(ch) || IsKanji(ch);

        /// <summary>
        /// Check if the given string has hiragana.
        /// </summary>
        public static bool HasHiragana(string str) => str.Any(IsHiragana);
        
        /// <summary>
        /// Check if the given string has katakana.
        /// </summary>
        public static bool HasKatakana(string str) => str.Any(IsKatakana);
        
        /// <summary>
        /// Check if the given string has kana.
        /// </summary>
        public static bool HasKana(string str) => str.Any(IsKana);
        
        /// <summary>
        /// Check if the given string has kanji.
        /// </summary>
        public static bool HasKanji(string str) => str.Any(IsKanji);
        
        /// <summary>
        /// Check if the given string has Japanese character.
        /// </summary>
        public static bool HasJapanese(string str) => str.Any(IsJapanese);

        /// <summary>
        /// Convert the given string to raw hiragana form.
        /// </summary>
        /// <param name="str"></param>
        /// <returns>The hiragana form string</returns>
        public static string ToRawHiragana(string str)
        {
            var strBuilder = new StringBuilder();
            foreach (var ch in str)
            {
                strBuilder.Append((ch > '\u30a0' && ch < '\u30f7') ? (char)(ch + KatakanaHiraganaShift) : ch);
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// Convert the given string to raw katakana form.
        /// </summary>
        /// <param name="str"></param>
        /// <returns>The katakana form string</returns>
        public static string ToRawKatakana(string str)
        {
            var strBuilder = new StringBuilder();
            foreach (var ch in str)
            {
                strBuilder.Append((ch > '\u3040' && ch < '\u3097') ? (char)(ch + HiraganaKatakanaShift) : ch);
            }

            return strBuilder.ToString();
        }

        /// <summary>
        /// Convert the given string to raw romaji form.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="system"></param>
        /// <returns>The romaji form string</returns>
        public static string ToRawRomaji(string str, RomajiSystem system = RomajiSystem.Hepburn)
        {
            var nippon = new Dictionary<string, string>
            {
                // Nums & marks
                {"１", "1"},
                {"２", "2"},
                {"３", "3"},
                {"４", "4"},
                {"５", "5"},
                {"６", "6"},
                {"７", "7"},
                {"８", "8"},
                {"９", "9"},
                {"０", "0"},
                {"！", "!"},
                {"“", "\""},
                {"”", "\""},
                {"＃", "#"},
                {"＄", "$"},
                {"％", "%"},
                {"＆", "&"},
                {"’", "'"},
                {"（", "("},
                {"）", ")"},
                {"＝", "="},
                {"～", "~"},
                {"｜", "|"},
                {"＠", "@"},
                {"‘", "`"},
                {"＋", "+"},
                {"＊", "*"},
                {"；", ";"},
                {"：", ":"},
                {"＜", "<"},
                {"＞", ">"},
                {"、", ","},
                {"。", "."},
                {"／", "/"},
                {"？", "?"},
                {"＿", "_"},
                {"・", "･"},
                {"｛", "{"},
                {"｝", "}"},
                {"＾", "^"},

                // 直音-清音(ア～ノ)
                {"あ", "a"},
                {"い", "i"},
                {"う", "u"},
                {"え", "e"},
                {"お", "o"},
                {"ア", "a"},
                {"イ", "i"},
                {"ウ", "u"},
                {"エ", "e"},
                {"オ", "o"},

                {"か", "ka"},
                {"き", "ki"},
                {"く", "ku"},
                {"け", "ke"},
                {"こ", "ko"},
                {"カ", "ka"},
                {"キ", "ki"},
                {"ク", "ku"},
                {"ケ", "ke"},
                {"コ", "ko"},

                {"さ", "sa"},
                {"し", "si"},
                {"す", "su"},
                {"せ", "se"},
                {"そ", "so"},
                {"サ", "sa"},
                {"シ", "si"},
                {"ス", "su"},
                {"セ", "se"},
                {"ソ", "so"},

                {"た", "ta"},
                {"ち", "ti"},
                {"つ", "tu"},
                {"て", "te"},
                {"と", "to"},
                {"タ", "ta"},
                {"チ", "ti"},
                {"ツ", "tu"},
                {"テ", "te"},
                {"ト", "to"},

                {"な", "na"},
                {"に", "ni"},
                {"ぬ", "nu"},
                {"ね", "ne"},
                {"の", "no"},
                {"ナ", "na"},
                {"ニ", "ni"},
                {"ヌ", "nu"},
                {"ネ", "ne"},
                {"ノ", "no"},

                // 直音-清音(ハ～ヲ)
                {"は", "ha"},
                {"ひ", "hi"},
                {"ふ", "hu"},
                {"へ", "he"},
                {"ほ", "ho"},
                {"ハ", "ha"},
                {"ヒ", "hi"},
                {"フ", "hu"},
                {"ヘ", "he"},
                {"ホ", "ho"},

                {"ま", "ma"},
                {"み", "mi"},
                {"む", "mu"},
                {"め", "me"},
                {"も", "mo"},
                {"マ", "ma"},
                {"ミ", "mi"},
                {"ム", "mu"},
                {"メ", "me"},
                {"モ", "mo"},

                {"や", "ya"},
                {"ゆ", "yu"},
                {"よ", "yo"},
                {"ヤ", "ya"},
                {"ユ", "yu"},
                {"ヨ", "yo"},

                {"ら", "ra"},
                {"り", "ri"},
                {"る", "ru"},
                {"れ", "re"},
                {"ろ", "ro"},
                {"ラ", "ra"},
                {"リ", "ri"},
                {"ル", "ru"},
                {"レ", "re"},
                {"ロ", "ro"},

                {"わ", "wa"},
                {"ゐ", "wi"},
                {"ゑ", "we"},
                {"を", "wo"},
                {"ワ", "wa"},
                {"ヰ", "wi"},
                {"ヱ", "we"},
                {"ヲ", "wo"},

                // 直音-濁音(ガ～ボ)、半濁音(パ～ポ)
                {"が", "ga"},
                {"ぎ", "gi"},
                {"ぐ", "gu"},
                {"げ", "ge"},
                {"ご", "go"},
                {"ガ", "ga"},
                {"ギ", "gi"},
                {"グ", "gu"},
                {"ゲ", "ge"},
                {"ゴ", "go"},

                {"ざ", "za"},
                {"じ", "zi"},
                {"ず", "zu"},
                {"ぜ", "ze"},
                {"ぞ", "zo"},
                {"ザ", "za"},
                {"ジ", "zi"},
                {"ズ", "zu"},
                {"ゼ", "ze"},
                {"ゾ", "zo"},

                {"だ", "da"},
                {"ぢ", "di"},
                {"づ", "du"},
                {"で", "de"},
                {"ど", "do"},
                {"ダ", "da"},
                {"ヂ", "di"},
                {"ヅ", "du"},
                {"デ", "de"},
                {"ド", "do"},

                {"ば", "ba"},
                {"び", "bi"},
                {"ぶ", "bu"},
                {"べ", "be"},
                {"ぼ", "bo"},
                {"バ", "ba"},
                {"ビ", "bi"},
                {"ブ", "bu"},
                {"ベ", "be"},
                {"ボ", "bo"},

                {"ぱ", "pa"},
                {"ぴ", "pi"},
                {"ぷ", "pu"},
                {"ぺ", "pe"},
                {"ぽ", "po"},
                {"パ", "pa"},
                {"ピ", "pi"},
                {"プ", "pu"},
                {"ペ", "pe"},
                {"ポ", "po"},

                // 拗音-清音(キャ～リョ)
                {"きゃ", "kya"},
                {"きゅ", "kyu"},
                {"きょ", "kyo"},
                {"しゃ", "sya"},
                {"しゅ", "syu"},
                {"しょ", "syo"},
                {"ちゃ", "tya"},
                {"ちゅ", "tyu"},
                {"ちょ", "tyo"},
                {"にゃ", "nya"},
                {"にゅ", "nyu"},
                {"にょ", "nyo"},
                {"ひゃ", "hya"},
                {"ひゅ", "hyu"},
                {"ひょ", "hyo"},
                {"みゃ", "mya"},
                {"みゅ", "myu"},
                {"みょ", "myo"},
                {"りゃ", "rya"},
                {"りゅ", "ryu"},
                {"りょ", "ryo"},
                {"キャ", "kya"},
                {"キュ", "kyu"},
                {"キョ", "kyo"},
                {"シャ", "sya"},
                {"シュ", "syu"},
                {"ショ", "syo"},
                {"チャ", "tya"},
                {"チュ", "tyu"},
                {"チョ", "tyo"},
                {"ニャ", "nya"},
                {"ニュ", "nyu"},
                {"ニョ", "nyo"},
                {"ヒャ", "hya"},
                {"ヒュ", "hyu"},
                {"ヒョ", "hyo"},
                {"ミャ", "mya"},
                {"ミュ", "myu"},
                {"ミョ", "myo"},
                {"リャ", "rya"},
                {"リュ", "ryu"},
                {"リョ", "ryo"},

                // 拗音-濁音(ギャ～ビョ)、半濁音(ピャ～ピョ)、合拗音(クヮ、グヮ)
                {"ぎゃ", "gya"},
                {"ぎゅ", "gyu"},
                {"ぎょ", "gyo"},
                {"じゃ", "zya"},
                {"じゅ", "zyu"},
                {"じょ", "zyo"},
                {"ぢゃ", "dya"},
                {"ぢゅ", "dyu"},
                {"ぢょ", "dyo"},
                {"びゃ", "bya"},
                {"びゅ", "byu"},
                {"びょ", "byo"},
                {"ぴゃ", "pya"},
                {"ぴゅ", "pyu"},
                {"ぴょ", "pyo"},
                {"くゎ", "kwa"},
                {"ぐゎ", "gwa"},
                {"ギャ", "gya"},
                {"ギュ", "gyu"},
                {"ギョ", "gyo"},
                {"ジャ", "zya"},
                {"ジュ", "zyu"},
                {"ジョ", "zyo"},
                {"ヂャ", "dya"},
                {"ヂュ", "dyu"},
                {"ヂョ", "dyo"},
                {"ビャ", "bya"},
                {"ビュ", "byu"},
                {"ビョ", "byo"},
                {"ピャ", "pya"},
                {"ピュ", "pyu"},
                {"ピョ", "pyo"},
                {"クヮ", "kwa"},
                {"グヮ", "gwa"},

                // 小書きの仮名、符号
                {"ぁ", "a"},
                {"ぃ", "i"},
                {"ぅ", "u"},
                {"ぇ", "e"},
                {"ぉ", "o"},
                {"ゃ", "ya"},
                {"ゅ", "yu"},
                {"ょ", "yo"},
                {"ゎ", "wa"},
                {"ァ", "a"},
                {"ィ", "i"},
                {"ゥ", "u"},
                {"ェ", "e"},
                {"ォ", "o"},
                {"ャ", "ya"},
                {"ュ", "yu"},
                {"ョ", "yo"},
                {"ヮ", "wa"},
                {"ヵ", "ka"},
                {"ヶ", "ke"},
                {"ん", "n"},
                {"ン", "n"},
                // ー: "",
                {"　", " "},

                // 外来音(イェ～グォ)
                {"いぇ", "ye"},
                // うぃ: "",
                // うぇ: "",
                // うぉ: "",
                {"きぇ", "kye"},
                // くぁ: "",
                {"くぃ", "kwi"},
                {"くぇ", "kwe"},
                {"くぉ", "kwo"},
                // ぐぁ: "",
                {"ぐぃ", "gwi"},
                {"ぐぇ", "gwe"},
                {"ぐぉ", "gwo"},
                {"イェ", "ye"},
                // ウィ: "",
                // ウェ: "",
                // ウォ: "",
                // ヴ: "",
                // ヴァ: "",
                // ヴィ: "",
                // ヴェ: "",
                // ヴォ: "",
                // ヴュ: "",
                // ヴョ: "",
                {"キェ", "kya"},
                // クァ: "",
                {"クィ", "kwi"},
                {"クェ", "kwe"},
                {"クォ", "kwo"},
                // グァ: "",
                {"グィ", "gwi"},
                {"グェ", "gwe"},
                {"グォ", "gwo"},

                // 外来音(シェ～フョ)
                {"しぇ", "sye"},
                {"じぇ", "zye"},
                {"すぃ", "swi"},
                {"ずぃ", "zwi"},
                {"ちぇ", "tye"},
                {"つぁ", "twa"},
                {"つぃ", "twi"},
                {"つぇ", "twe"},
                {"つぉ", "two"},
                // てぃ: "ti",
                // てゅ: "tyu",
                // でぃ: "di",
                // でゅ: "dyu",
                // とぅ: "tu",
                // どぅ: "du",
                {"にぇ", "nye"},
                {"ひぇ", "hye"},
                {"ふぁ", "hwa"},
                {"ふぃ", "hwi"},
                {"ふぇ", "hwe"},
                {"ふぉ", "hwo"},
                {"ふゅ", "hwyu"},
                {"ふょ", "hwyo"},
                {"シェ", "sye"},
                {"ジェ", "zye"},
                {"スィ", "swi"},
                {"ズィ", "zwi"},
                {"チェ", "tye"},
                {"ツァ", "twa"},
                {"ツィ", "twi"},
                {"ツェ", "twe"},
                {"ツォ", "two"},
                // ティ: "ti",
                // テュ: "tyu",
                // ディ: "di",
                // デュ: "dyu",
                // トゥ: "tu",
                // ドゥ: "du",
                {"ニェ", "nye"},
                {"ヒェ", "hye"},
                {"ファ", "hwa"},
                {"フィ", "hwi"},
                {"フェ", "hwe"},
                {"フォ", "hwo"},
                {"フュ", "hwyu"},
                {"フョ", "hwyo"}
            };
            var passport = new Dictionary<string, string>
            {
                // Nums & marks
                {"１", "1"},
                {"２", "2"},
                {"３", "3"},
                {"４", "4"},
                {"５", "5"},
                {"６", "6"},
                {"７", "7"},
                {"８", "8"},
                {"９", "9"},
                {"０", "0"},
                {"！", "!"},
                {"“", "\""},
                {"”", "\""},
                {"＃", "#"},
                {"＄", "$"},
                {"％", "%"},
                {"＆", "&"},
                {"’", "'"},
                {"（", "("},
                {"）", ")"},
                {"＝", "="},
                {"～", "~"},
                {"｜", "|"},
                {"＠", "@"},
                {"‘", "`"},
                {"＋", "+"},
                {"＊", "*"},
                {"；", ";"},
                {"：", ":"},
                {"＜", "<"},
                {"＞", ">"},
                {"、", ","},
                {"。", "."},
                {"／", "/"},
                {"？", "?"},
                {"＿", "_"},
                {"・", "･"},
                {"｛", "{"},
                {"｝", "}"},
                {"＾", "^"},

                // 直音-清音(ア～ノ)
                {"あ", "a"},
                {"い", "i"},
                {"う", "u"},
                {"え", "e"},
                {"お", "o"},
                {"ア", "a"},
                {"イ", "i"},
                {"ウ", "u"},
                {"エ", "e"},
                {"オ", "o"},
                
                {"か", "ka"},
                {"き", "ki"},
                {"く", "ku"},
                {"け", "ke"},
                {"こ", "ko"},
                {"カ", "ka"},
                {"キ", "ki"},
                {"ク", "ku"},
                {"ケ", "ke"},
                {"コ", "ko"},

                {"さ", "sa"},
                {"し", "shi"},
                {"す", "su"},
                {"せ", "se"},
                {"そ", "so"},
                {"サ", "sa"},
                {"シ", "shi"},
                {"ス", "su"},
                {"セ", "se"},
                {"ソ", "so"},

                {"た", "ta"},
                {"ち", "chi"},
                {"つ", "tsu"},
                {"て", "te"},
                {"と", "to"},
                {"タ", "ta"},
                {"チ", "chi"},
                {"ツ", "tsu"},
                {"テ", "te"},
                {"ト", "to"},

                {"な", "na"},
                {"に", "ni"},
                {"ぬ", "nu"},
                {"ね", "ne"},
                {"の", "no"},
                {"ナ", "na"},
                {"ニ", "ni"},
                {"ヌ", "nu"},
                {"ネ", "ne"},
                {"ノ", "no"},

                // 直音-清音(ハ～ヲ)
                {"は", "ha"},
                {"ひ", "hi"},
                {"ふ", "fu"},
                {"へ", "he"},
                {"ほ", "ho"},
                {"ハ", "ha"},
                {"ヒ", "hi"},
                {"フ", "fu"},
                {"ヘ", "he"},
                {"ホ", "ho"},

                {"ま", "ma"},
                {"み", "mi"},
                {"む", "mu"},
                {"め", "me"},
                {"も", "mo"},
                {"マ", "ma"},
                {"ミ", "mi"},
                {"ム", "mu"},
                {"メ", "me"},
                {"モ", "mo"},

                {"や", "ya"},
                {"ゆ", "yu"},
                {"よ", "yo"},
                {"ヤ", "ya"},
                {"ユ", "yu"},
                {"ヨ", "yo"},

                {"ら", "ra"},
                {"り", "ri"},
                {"る", "ru"},
                {"れ", "re"},
                {"ろ", "ro"},
                {"ラ", "ra"},
                {"リ", "ri"},
                {"ル", "ru"},
                {"レ", "re"},
                {"ロ", "ro"},

                {"わ", "wa"},
                {"ゐ", "i"},
                {"ゑ", "e"},
                {"を", "o"},
                {"ワ", "wa"},
                {"ヰ", "i"},
                {"ヱ", "e"},
                {"ヲ", "o"},

                // 直音-濁音(ガ～ボ)、半濁音(パ～ポ)
                {"が", "ga"},
                {"ぎ", "gi"},
                {"ぐ", "gu"},
                {"げ", "ge"},
                {"ご", "go"},
                {"ガ", "ga"},
                {"ギ", "gi"},
                {"グ", "gu"},
                {"ゲ", "ge"},
                {"ゴ", "go"},

                {"ざ", "za"},
                {"じ", "ji"},
                {"ず", "zu"},
                {"ぜ", "ze"},
                {"ぞ", "zo"},
                {"ザ", "za"},
                {"ジ", "ji"},
                {"ズ", "zu"},
                {"ゼ", "ze"},
                {"ゾ", "zo"},

                {"だ", "da"},
                {"ぢ", "ji"},
                {"づ", "zu"},
                {"で", "de"},
                {"ど", "do"},
                {"ダ", "da"},
                {"ヂ", "ji"},
                {"ヅ", "zu"},
                {"デ", "de"},
                {"ド", "do"},

                {"ば", "ba"},
                {"び", "bi"},
                {"ぶ", "bu"},
                {"べ", "be"},
                {"ぼ", "bo"},
                {"バ", "ba"},
                {"ビ", "bi"},
                {"ブ", "bu"},
                {"ベ", "be"},
                {"ボ", "bo"},

                {"ぱ", "pa"},
                {"ぴ", "pi"},
                {"ぷ", "pu"},
                {"ぺ", "pe"},
                {"ぽ", "po"},
                {"パ", "pa"},
                {"ピ", "pi"},
                {"プ", "pu"},
                {"ペ", "pe"},
                {"ポ", "po"},
                
                // 拗音-清音(キャ～リョ)
                {"きゃ", "kya"},
                {"きゅ", "kyu"},
                {"きょ", "kyo"},
                {"しゃ", "sha"},
                {"しゅ", "shu"},
                {"しょ", "sho"},
                {"ちゃ", "cha"},
                {"ちゅ", "chu"},
                {"ちょ", "cho"},
                {"にゃ", "nya"},
                {"にゅ", "nyu"},
                {"にょ", "nyo"},
                {"ひゃ", "hya"},
                {"ひゅ", "hyu"},
                {"ひょ", "hyo"},
                {"みゃ", "mya"},
                {"みゅ", "myu"},
                {"みょ", "myo"},
                {"りゃ", "rya"},
                {"りゅ", "ryu"},
                {"りょ", "ryo"},
                {"キャ", "kya"},
                {"キュ", "kyu"},
                {"キョ", "kyo"},
                {"シャ", "sha"},
                {"シュ", "shu"},
                {"ショ", "sho"},
                {"チャ", "cha"},
                {"チュ", "chu"},
                {"チョ", "cho"},
                {"ニャ", "nya"},
                {"ニュ", "nyu"},
                {"ニョ", "nyo"},
                {"ヒャ", "hya"},
                {"ヒュ", "hyu"},
                {"ヒョ", "hyo"},
                {"ミャ", "mya"},
                {"ミュ", "myu"},
                {"ミョ", "myo"},
                {"リャ", "rya"},
                {"リュ", "ryu"},
                {"リョ", "ryo"},
                
                // 拗音-濁音(ギャ～ビョ)、半濁音(ピャ～ピョ)、合拗音(クヮ、グヮ)
                {"ぎゃ", "gya"},
                {"ぎゅ", "gyu"},
                {"ぎょ", "gyo"},
                {"じゃ", "ja"},
                {"じゅ", "ju"},
                {"じょ", "jo"},
                {"ぢゃ", "ja"},
                {"ぢゅ", "ju"},
                {"ぢょ", "jo"},
                {"びゃ", "bya"},
                {"びゅ", "byu"},
                {"びょ", "byo"},
                {"ぴゃ", "pya"},
                {"ぴゅ", "pyu"},
                {"ぴょ", "pyo"},
                // くゎ: "",
                // ぐゎ: "",
                {"ギャ", "gya"},
                {"ギュ", "gyu"},
                {"ギョ", "gyo"},
                {"ジャ", "ja"},
                {"ジュ", "ju"},
                {"ジョ", "jo"},
                {"ヂャ", "ja"},
                {"ヂュ", "ju"},
                {"ヂョ", "jo"},
                {"ビャ", "bya"},
                {"ビュ", "byu"},
                {"ビョ", "byo"},
                {"ピャ", "pya"},
                {"ピュ", "pyu"},
                {"ピョ", "pyo"},
                // クヮ: "",
                // グヮ: "",
                
                // 小書きの仮名、符号
                {"ぁ", "a"},
                {"ぃ", "i"},
                {"ぅ", "u"},
                {"ぇ", "e"},
                {"ぉ", "o"},
                {"ゃ", "ya"},
                {"ゅ", "yu"},
                {"ょ", "yo"},
                {"ゎ", "wa"},
                {"ァ", "a"},
                {"ィ", "i"},
                {"ゥ", "u"},
                {"ェ", "e"},
                {"ォ", "o"},
                {"ャ", "ya"},
                {"ュ", "yu"},
                {"ョ", "yo"},
                {"ヮ", "wa"},
                {"ヵ", "ka"},
                {"ヶ", "ke"},
                {"ん", "n"},
                {"ン", "n"},
                // ー: "",
                {"　", " "},
                
                // 外来音(イェ～グォ)
                // いぇ: "",
                // うぃ: "",
                // うぇ: "",
                // うぉ: "",
                // きぇ: "",
                // くぁ: "",
                // くぃ: "",
                // くぇ: "",
                // くぉ: "",
                // ぐぁ: "",
                // ぐぃ: "",
                // ぐぇ: "",
                // ぐぉ: "",
                // イェ: "",
                // ウィ: "",
                // ウェ: "",
                // ウォ: "",
                {"ヴ", "b"}
                // ヴァ: "",
                // ヴィ: "",
                // ヴェ: "",
                // ヴォ: "",
                // ヴュ: "",
                // ヴョ: "",
                // キェ: "",
                // クァ: "",
                // クィ: "",
                // クェ: "",
                // クォ: "",
                // グァ: "",
                // グィ: "",
                // グェ: "",
                // グォ: "",

                // 外来音(シェ～フョ)
                // しぇ: "",
                // じぇ: "",
                // すぃ: "",
                // ずぃ: "",
                // ちぇ: "",
                // つぁ: "",
                // つぃ: "",
                // つぇ: "",
                // つぉ: "",
                // てぃ: "",
                // てゅ: "",
                // でぃ: "",
                // でゅ: "",
                // とぅ: "",
                // どぅ: "",
                // にぇ: "",
                // ひぇ: "",
                // ふぁ: "",
                // ふぃ: "",
                // ふぇ: "",
                // ふぉ: "",
                // ふゅ: "",
                // ふょ: "",
                // シェ: "",
                // ジェ: "",
                // スィ: "",
                // ズィ: "",
                // チェ: "",
                // ツァ: "",
                // ツィ: "",
                // ツェ: "",
                // ツォ: "",
                // ティ: "",
                // テュ: "",
                // ディ: "",
                // デュ: "",
                // トゥ: "",
                // ドゥ: "",
                // ニェ: "",
                // ヒェ: "",
                // ファ: "",
                // フィ: "",
                // フェ: "",
                // フォ: "",
                // フュ: "",
                // フョ: ""
            };
            var hepburn = new Dictionary<string, string>
            {
                // Nums & marks
                {"１", "1"},
                {"２", "2"},
                {"３", "3"},
                {"４", "4"},
                {"５", "5"},
                {"６", "6"},
                {"７", "7"},
                {"８", "8"},
                {"９", "9"},
                {"０", "0"},
                {"！", "!"},
                {"“", "\""},
                {"”", "\""},
                {"＃", "#"},
                {"＄", "$"},
                {"％", "%"},
                {"＆", "&"},
                {"’", "'"},
                {"（", "("},
                {"）", ")"},
                {"＝", "="},
                {"～", "~"},
                {"｜", "|"},
                {"＠", "@"},
                {"‘", "`"},
                {"＋", "+"},
                {"＊", "*"},
                {"；", ";"},
                {"：", ":"},
                {"＜", "<"},
                {"＞", ">"},
                {"、", ","},
                {"。", "."},
                {"／", "/"},
                {"？", "?"},
                {"＿", "_"},
                {"・", "･"},
                {"｛", "{"},
                {"｝", "}"},
                {"＾", "^"},

                // 直音-清音(ア～ノ)
                {"あ", "a"},
                {"い", "i"},
                {"う", "u"},
                {"え", "e"},
                {"お", "o"},
                {"ア", "a"},
                {"イ", "i"},
                {"ウ", "u"},
                {"エ", "e"},
                {"オ", "o"},
                
                {"か", "ka"},
                {"き", "ki"},
                {"く", "ku"},
                {"け", "ke"},
                {"こ", "ko"},
                {"カ", "ka"},
                {"キ", "ki"},
                {"ク", "ku"},
                {"ケ", "ke"},
                {"コ", "ko"},

                {"さ", "sa"},
                {"し", "shi"},
                {"す", "su"},
                {"せ", "se"},
                {"そ", "so"},
                {"サ", "sa"},
                {"シ", "shi"},
                {"ス", "su"},
                {"セ", "se"},
                {"ソ", "so"},

                {"た", "ta"},
                {"ち", "chi"},
                {"つ", "tsu"},
                {"て", "te"},
                {"と", "to"},
                {"タ", "ta"},
                {"チ", "chi"},
                {"ツ", "tsu"},
                {"テ", "te"},
                {"ト", "to"},

                {"な", "na"},
                {"に", "ni"},
                {"ぬ", "nu"},
                {"ね", "ne"},
                {"の", "no"},
                {"ナ", "na"},
                {"ニ", "ni"},
                {"ヌ", "nu"},
                {"ネ", "ne"},
                {"ノ", "no"},

                // 直音-清音(ハ～ヲ)
                {"は", "ha"},
                {"ひ", "hi"},
                {"ふ", "fu"},
                {"へ", "he"},
                {"ほ", "ho"},
                {"ハ", "ha"},
                {"ヒ", "hi"},
                {"フ", "fu"},
                {"ヘ", "he"},
                {"ホ", "ho"},

                {"ま", "ma"},
                {"み", "mi"},
                {"む", "mu"},
                {"め", "me"},
                {"も", "mo"},
                {"マ", "ma"},
                {"ミ", "mi"},
                {"ム", "mu"},
                {"メ", "me"},
                {"モ", "mo"},

                {"や", "ya"},
                {"ゆ", "yu"},
                {"よ", "yo"},
                {"ヤ", "ya"},
                {"ユ", "yu"},
                {"ヨ", "yo"},

                {"ら", "ra"},
                {"り", "ri"},
                {"る", "ru"},
                {"れ", "re"},
                {"ろ", "ro"},
                {"ラ", "ra"},
                {"リ", "ri"},
                {"ル", "ru"},
                {"レ", "re"},
                {"ロ", "ro"},

                {"わ", "wa"},
                {"ゐ", "i"},
                {"ゑ", "e"},
                {"を", "o"},
                {"ワ", "wa"},
                {"ヰ", "i"},
                {"ヱ", "e"},
                {"ヲ", "o"},

                // 直音-濁音(ガ～ボ)、半濁音(パ～ポ)
                {"が", "ga"},
                {"ぎ", "gi"},
                {"ぐ", "gu"},
                {"げ", "ge"},
                {"ご", "go"},
                {"ガ", "ga"},
                {"ギ", "gi"},
                {"グ", "gu"},
                {"ゲ", "ge"},
                {"ゴ", "go"},

                {"ざ", "za"},
                {"じ", "ji"},
                {"ず", "zu"},
                {"ぜ", "ze"},
                {"ぞ", "zo"},
                {"ザ", "za"},
                {"ジ", "ji"},
                {"ズ", "zu"},
                {"ゼ", "ze"},
                {"ゾ", "zo"},

                {"だ", "da"},
                {"ぢ", "ji"},
                {"づ", "zu"},
                {"で", "de"},
                {"ど", "do"},
                {"ダ", "da"},
                {"ヂ", "ji"},
                {"ヅ", "zu"},
                {"デ", "de"},
                {"ド", "do"},

                {"ば", "ba"},
                {"び", "bi"},
                {"ぶ", "bu"},
                {"べ", "be"},
                {"ぼ", "bo"},
                {"バ", "ba"},
                {"ビ", "bi"},
                {"ブ", "bu"},
                {"ベ", "be"},
                {"ボ", "bo"},

                {"ぱ", "pa"},
                {"ぴ", "pi"},
                {"ぷ", "pu"},
                {"ぺ", "pe"},
                {"ぽ", "po"},
                {"パ", "pa"},
                {"ピ", "pi"},
                {"プ", "pu"},
                {"ペ", "pe"},
                {"ポ", "po"},
                
                // 拗音-清音(キャ～リョ)
                {"きゃ", "kya"},
                {"きゅ", "kyu"},
                {"きょ", "kyo"},
                {"しゃ", "sha"},
                {"しゅ", "shu"},
                {"しょ", "sho"},
                {"ちゃ", "cha"},
                {"ちゅ", "chu"},
                {"ちょ", "cho"},
                {"にゃ", "nya"},
                {"にゅ", "nyu"},
                {"にょ", "nyo"},
                {"ひゃ", "hya"},
                {"ひゅ", "hyu"},
                {"ひょ", "hyo"},
                {"みゃ", "mya"},
                {"みゅ", "myu"},
                {"みょ", "myo"},
                {"りゃ", "rya"},
                {"りゅ", "ryu"},
                {"りょ", "ryo"},
                {"キャ", "kya"},
                {"キュ", "kyu"},
                {"キョ", "kyo"},
                {"シャ", "sha"},
                {"シュ", "shu"},
                {"ショ", "sho"},
                {"チャ", "cha"},
                {"チュ", "chu"},
                {"チョ", "cho"},
                {"ニャ", "nya"},
                {"ニュ", "nyu"},
                {"ニョ", "nyo"},
                {"ヒャ", "hya"},
                {"ヒュ", "hyu"},
                {"ヒョ", "hyo"},
                {"ミャ", "mya"},
                {"ミュ", "myu"},
                {"ミョ", "myo"},
                {"リャ", "rya"},
                {"リュ", "ryu"},
                {"リョ", "ryo"},
                
                // 拗音-濁音(ギャ～ビョ)、半濁音(ピャ～ピョ)、合拗音(クヮ、グヮ)
                {"ぎゃ", "gya"},
                {"ぎゅ", "gyu"},
                {"ぎょ", "gyo"},
                {"じゃ", "ja"},
                {"じゅ", "ju"},
                {"じょ", "jo"},
                {"ぢゃ", "ja"},
                {"ぢゅ", "ju"},
                {"ぢょ", "jo"},
                {"びゃ", "bya"},
                {"びゅ", "byu"},
                {"びょ", "byo"},
                {"ぴゃ", "pya"},
                {"ぴゅ", "pyu"},
                {"ぴょ", "pyo"},
                // くゎ: "",
                // ぐゎ: "",
                {"ギャ", "gya"},
                {"ギュ", "gyu"},
                {"ギョ", "gyo"},
                {"ジャ", "ja"},
                {"ジュ", "ju"},
                {"ジョ", "jo"},
                {"ヂャ", "ja"},
                {"ヂュ", "ju"},
                {"ヂョ", "jo"},
                {"ビャ", "bya"},
                {"ビュ", "byu"},
                {"ビョ", "byo"},
                {"ピャ", "pya"},
                {"ピュ", "pyu"},
                {"ピョ", "pyo"},
                // クヮ: "",
                // グヮ: "",
                
                // 小書きの仮名、符号
                {"ぁ", "a"},
                {"ぃ", "i"},
                {"ぅ", "u"},
                {"ぇ", "e"},
                {"ぉ", "o"},
                {"ゃ", "ya"},
                {"ゅ", "yu"},
                {"ょ", "yo"},
                {"ゎ", "wa"},
                {"ァ", "a"},
                {"ィ", "i"},
                {"ゥ", "u"},
                {"ェ", "e"},
                {"ォ", "o"},
                {"ャ", "ya"},
                {"ュ", "yu"},
                {"ョ", "yo"},
                {"ヮ", "wa"},
                {"ヵ", "ka"},
                {"ヶ", "ke"},
                {"ん", "n"},
                {"ン", "n"},
                // ー: "",
                {"　", " "},
                
                // 外来音(イェ～グォ)
                {"いぇ", "ye"},
                {"うぃ", "wi"},
                {"うぇ", "we"},
                {"うぉ", "wo"},
                {"きぇ", "kye"},
                {"くぁ", "kwa"},
                {"くぃ", "kwi"},
                {"くぇ", "kwe"},
                {"くぉ", "kwo"},
                {"ぐぁ", "gwa"},
                {"ぐぃ", "gwi"},
                {"ぐぇ", "gwe"},
                {"ぐぉ", "gwo"},
                {"イェ", "ye"},
                {"ウィ", "wi"},
                {"ウェ", "we"},
                {"ウォ", "wo"},
                {"ヴ", "vu"},
                {"ヴァ", "va"},
                {"ヴィ", "vi"},
                {"ヴェ", "ve"},
                {"ヴォ", "vo"},
                {"ヴュ", "vyu"},
                {"ヴョ", "vyo"},
                {"キェ", "kya"},
                {"クァ", "kwa"},
                {"クィ", "kwi"},
                {"クェ", "kwe"},
                {"クォ", "kwo"},
                {"グァ", "gwa"},
                {"グィ", "gwi"},
                {"グェ", "gwe"},
                {"グォ", "gwo"},

                // 外来音(シェ～フョ)
                {"しぇ", "she"},
                {"じぇ", "je"},
                // すぃ: "",
                // ずぃ: "",
                {"ちぇ", "che"},
                {"つぁ", "tsa"},
                {"つぃ", "tsi"},
                {"つぇ", "tse"},
                {"つぉ", "tso"},
                {"てぃ", "ti"},
                {"てゅ", "tyu"},
                {"でぃ", "di"},
                {"でゅ", "dyu"},
                {"とぅ", "tu"},
                {"どぅ", "du"},
                {"にぇ", "nye"},
                {"ひぇ", "hye"},
                {"ふぁ", "fa"},
                {"ふぃ", "fi"},
                {"ふぇ", "fe"},
                {"ふぉ", "fo"},
                {"ふゅ", "fyu"},
                {"ふょ", "fyo"},
                {"シェ", "she"},
                {"ジェ", "je"},
                // スィ: "",
                // ズィ: "",
                {"チェ", "che"},
                {"ツァ", "tsa"},
                {"ツィ", "tsi"},
                {"ツェ", "tse"},
                {"ツォ", "tso"},
                {"ティ", "ti"},
                {"テュ", "tyu"},
                {"ディ", "di"},
                {"デュ", "dyu"},
                {"トゥ", "tu"},
                {"ドゥ", "du"},
                {"ニェ", "nye"},
                {"ヒェ", "hye"},
                {"ファ", "fa"},
                {"フィ", "fi"},
                {"フェ", "fe"},
                {"フォ", "fo"},
                {"フュ", "fyu"},
                {"フョ", "fyo"}
            };
            
            var romajiSystem = new Dictionary<RomajiSystem, Dictionary<string, string>>()
            {
                {RomajiSystem.Hepburn, hepburn},
                {RomajiSystem.Nippon, nippon},
                {RomajiSystem.Passport, passport}
            };
            
            var regTsu = new Regex(@"(っ|ッ)([bcdfghijklmnopqrstuvwyz])", RegexOptions.Multiline); // Double the sokuon
            var regXtsu = new Regex(@"っ|ッ", RegexOptions.Multiline);
            
            var pnt = 0;
            var builder = new StringBuilder();

            switch (system)
            {
                // [PASSPORT] 長音省略 「―」の場合
                case RomajiSystem.Passport:
                    str = str.Replace("ー", "");
                    break;
                // [NIPPON|HEPBURN] 撥音の特殊表記 a、i、u、e、o、y
                case RomajiSystem.Nippon:
                case RomajiSystem.Hepburn:
                {
                    var regHatu = new Regex(@"(ん|ン)(?=あ|い|う|え|お|ア|イ|ウ|エ|オ|ぁ|ぃ|ぅ|ぇ|ぉ|ァ|ィ|ゥ|ェ|ォ|や|ゆ|よ|ヤ|ユ|ヨ|ゃ|ゅ|ょ|ャ|ュ|ョ)", RegexOptions.Multiline);
                    var matches = regHatu.Matches(str);
                    var indices = new List<int>();
                    foreach (Match item in matches)
                    {
                        indices.Add(item.Index);
                    }
                    if (indices.Count != 0) {
                        var mStr = "";
                        for (var i = 0; i < indices.Count; i++)
                        {
                            if (i == 0) {
                                mStr += $"{str.Substring(0, indices[i])}'";
                            }
                            else {
                                mStr += $"{str.Substring(indices[i - 1], indices[i] - indices[i - 1])}'";
                            }
                        }
                        mStr += str[indices[^1]..];
                        str = mStr;
                    }

                    break;
                }
            }

            // [ALL] kana to roman chars
            var max = str.Length;
            while (pnt < max)
            {
                string r;
                if (pnt != max -1 && romajiSystem[system].ContainsKey(str.Substring(pnt, 2)))
                {
                    r = romajiSystem[system][str.Substring(pnt, 2)];
                    builder.Append(r);
                    builder.Append(' ');
                    pnt += 2;
                }
                else if (romajiSystem[system].ContainsKey(str.Substring(pnt, 1)))
                {
                    r = romajiSystem[system][str.Substring(pnt, 1)];
                    builder.Append(r);
                    builder.Append(' ');
                    pnt += 1;
                }
                else
                {
                    var ch = str.Substring(pnt, 1);
                    builder.Append(ch);
                    pnt += 1;
                }
            }

            var result = regTsu.Replace(builder.ToString(), "$2$2"); // Double the sokuon
            
            // [PASSPORT|HEPBURN] 子音を重ねて特殊表記
            if (system == RomajiSystem.Passport || system == RomajiSystem.Hepburn) {
                result = result.Replace("cc", "tc");
            }

            result = regXtsu.Replace(result, "tsu");

            // [PASSPORT|HEPBURN] 撥音の特殊表記 b、m、p
            if (system == RomajiSystem.Passport || system == RomajiSystem.Hepburn) {
                result = result.Replace("nm", "mm");
                result = result.Replace("nb", "mb");
                result = result.Replace("np", "mp");
            }

            // [NIPPON] 長音変換
            if (system == RomajiSystem.Nippon) {
                result = result.Replace("aー", "â");
                result = result.Replace("iー", "î");
                result = result.Replace("uー", "û");
                result = result.Replace("eー", "ê");
                result = result.Replace("oー", "ô");
            }
            
            // [HEPBURN] 長音変換
            if (system == RomajiSystem.Hepburn) {
                result = result.Replace("aー", "ā");
                result = result.Replace("iー", "ī");
                result = result.Replace("uー", "ū");
                result = result.Replace("eー", "ē");
                result = result.Replace("oー", "ō");
            }

            return result;
        }

        /// <summary>
        /// Get the character type of given string.
        /// </summary>
        internal static TextType GetTextType(string str)
        {
            var hasKanji = false;
            var hasKana = false;
            foreach (var ch in str)
            {
                if (IsKanji(ch))
                {
                    hasKanji = true;
                }
                
                else if (IsKana(ch))
                {
                    hasKana = true;
                }

                if (hasKana && hasKanji)
                {
                    return TextType.KanjiKanaMixed;
                }
            }

            if (hasKana) return TextType.PureKana;
            if (hasKanji) return TextType.PureKanji;
            return TextType.Others;
        }
    }
}