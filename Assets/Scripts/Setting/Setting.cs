using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class Setting: MonoBehaviour {
    public static void LangSetDefault() =>
        Loader.LoadScene("Setting", Loader.LoadData.language);

    public static void LangChange() {
        Tools.OpenFileName ofn = new Tools.OpenFileName(
            Lang.Uage.chooseLanguageFile.filter + "\0*.json\0\0",
            Path.Combine(Directory.GetParent(Application.dataPath).FullName, "LanguageFiles"),
            Lang.Uage.chooseLanguageFile.title, "json"
        );

        if(Tools.WindowDll.GetOpenFileName(ofn))
            Loader.LoadScene("Setting", Loader.LoadData.language, ofn.file);
    }

    public Text quitTip;
    public Text language;
    public Text langDefault;
    public Text langChoose;

    private void Start() {
        quitTip.text = string.Format(Lang.Uage.setting.quitTip, "Esc");
        language.text = string.Format(Lang.Uage.setting.language, Lang.Uage.information.name);
        langDefault.text = Lang.Uage.setting.languageDefault;
        langChoose.text = Lang.Uage.setting.languageChoose;
    }

    private void Update() {
        if(Input.GetButtonDown("Pause")) Loader.LoadScene("MainPage");
    }
}
