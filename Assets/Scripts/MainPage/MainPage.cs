using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class MainPage: MonoBehaviour {

    public static void Setting() =>
        Loader.LoadScene("Setting");

    public static void ChooseChart() {

        Tools.OpenFileName ofn = new Tools.OpenFileName(
                Lang.Uage.chooseChartFile.filter + "\0*.cube\0\0",
                Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Charts"),
                Lang.Uage.chooseChartFile.title, "cube"
        );

        if(Tools.WindowDll.GetOpenFileName(ofn))
            Loader.LoadScene("Play", Loader.LoadData.chart, ofn.file);
    }

    public Text setting;
    public Text play;

    private void Start() {
        setting.text = Lang.Uage.mainPage.setting;
        play.text = Lang.Uage.mainPage.play;
    }
}
