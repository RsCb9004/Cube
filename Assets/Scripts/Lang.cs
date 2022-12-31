using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Lang {

    [Serializable]
    public class Language {
        [Serializable]
        public class Information {
            public float version = 0.2f;
            public string name = "default";
        }
        [Serializable]
        public class Load {
            public string tipForm = "Tip: {0}";
            public List<string> tips = new List<string> {
                "This is Cube ver0.2."
            };
        }
        [Serializable]
        public class Start {
            public string tip = "- Any Key To Start -";
        }
        [Serializable]
        public class MainPage {
            public string setting = "Setting";
            public string play = "Start";
        }
        [Serializable]
        public class Setting {
            public string quitTip = "[{0}] - Quit";
            public string language = "Language: {0}";
            public string languageDefault = "default";
            public string languageChoose = "...";
        }
        [Serializable]
        public class ChooseLanguageFile {
            public string filter = "Language Files (.json)";
            public string title = "Choose Language File";
        }
        [Serializable]
        public class ChooseChartFile {
            public string filter = "Chart Files (.cube)";
            public string title = "Choose Chart File";
        }
        [Serializable]
        public class Play {
            public string pauseTitle = "P  A  U  S  E";
            public string pauseTip = "[{0}]-Back  [{1}]-Again  [{2}]-Resume";
            public string scoreForm = "X{0}";
            public string comboForm = "x";
        }
        [Serializable]
        public class Clear {
            public string countForm = "Perfect: {0:x}  Good: {1:x}  Miss: {2:x}\nMaxCombo: {3:x}   {4}";
            public string apNote = "<color=yellow>All Perfect</color>";
            public string fcNote = "<color=cyan>Full Combo</color>";
            public string noNote = "";
            public string tip = "[{0}]-Back  [{1}]-Again";
        }

        public Information information = new Information();
        public Load load = new Load();
        public Start start = new Start();
        public MainPage mainPage = new MainPage();
        public Setting setting = new Setting();
        public ChooseLanguageFile chooseLanguageFile = new ChooseLanguageFile();
        public ChooseChartFile chooseChartFile = new ChooseChartFile();
        public Play play = new Play();
        public Clear clear = new Clear();
    }

    private static readonly string langPath =
        Path.Combine(Application.persistentDataPath, "Lang.json");

    public static Language Uage { get; private set; } = new Language();

    public static void Change(string newJson) => File.WriteAllText(langPath, newJson);

    public static void SetDefault() => Change("{}");

    public static void Load() {

        if(!File.Exists(langPath)) SetDefault();

        Uage = JsonUtility.FromJson<Language>(File.ReadAllText(langPath));
    }
}
