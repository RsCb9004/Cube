using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameStart: MonoBehaviour {

    public Text tip;

    private void Start() {

        GameInit();

        tip.text = Lang.Uage.start.tip;
    }

    private void Update() {
        if(Input.anyKeyDown) {
            Loader.LoadScene("MainPage");
        }
    }

    private void GameInit() {

        UnityEngine.Random.InitState((int)DateTime.Now.Ticks);

        Directory.CreateDirectory(Application.persistentDataPath);
        Directory.CreateDirectory(Application.temporaryCachePath);

        Lang.Load();
    }
}
