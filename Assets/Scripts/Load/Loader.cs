using System;
using System.IO;
using System.IO.Compression;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loader: MonoBehaviour {

    public enum LoadData {
        none, chart, language
    }

    private static string scene;
    private static LoadData data;
    private static string dataPath;

    public static void LoadScene(string tarScene,
        LoadData dataType = LoadData.none, string path = null) {

        scene = tarScene; data = dataType; dataPath = path;
        SceneManager.LoadScene("Load");
    }

    private static async void StartLoad() {
        try {
            switch(data) {
            case LoadData.chart:
                await Task.Run(() => LoadChart(dataPath));
                break;
            case LoadData.language:
                await Task.Run(() => LoadLang(dataPath));
                break;
            }
        }
        finally {
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
            operation.allowSceneActivation = true;
        }
    }

    [Serializable]
    private class ChartInf {
        public float version = 0.2f;
        public string chart = "chart.json";
        public string video = "video.mp4";
        public bool autoPlay = false;
    }

    private static string tempPath;

    private static void LoadChart(string chartPath) {

        FileInfo[] files = new DirectoryInfo(tempPath).GetFiles();
        foreach(var i in files) File.Delete(i.FullName);

        ZipFile.ExtractToDirectory(chartPath, tempPath);

        ChartInf inf = File.Exists(Path.Combine(tempPath, "Inf.json")) ?
            JsonUtility.FromJson<ChartInf>(File.ReadAllText(Path.Combine(tempPath, "Inf.json"))) :
            new ChartInf();

        Charter.ChartJson = File.ReadAllText(Path.Combine(tempPath, inf.chart));

        Charter.Bga = Path.Combine(tempPath, inf.video);

        AutoPlay.Working = inf.autoPlay;
    }

    private static void LoadLang(string langPath) {

        if(langPath == null) Lang.SetDefault();
        else Lang.Change(File.ReadAllText(langPath));

        Lang.Load();
    }


    public Text tip;

    private void Start() {

        tempPath = Application.temporaryCachePath;

        string tipContent = Lang.Uage.load.tips[
            UnityEngine.Random.Range(0, Lang.Uage.load.tips.Count - 1)
        ];
        tip.text = string.Format(Lang.Uage.load.tipForm, tipContent);

        StartLoad();
    }
}
