using UnityEngine;
using UnityEngine.UI;

public class AutoPlay: MonoBehaviour {

    public static bool Working { get; set; }
    public static bool[] KeyDown { get; private set; }
    public static bool[] KeyUp { get; private set; }
    public static bool[] Key { get; private set; }

    private Text text;

    private void Start() {

        KeyDown = new bool[4];
        KeyUp = new bool[4];
        Key = new bool[4];

        text = GetComponent<Text>();
    }

    private void Update() {
        if(text.enabled = Working)
            for(int i = 0; i < 4; i++) {

                if(KeyUp[i])
                    KeyUp[i] = false;

                if(KeyDown[i])
                    KeyDown[i] = false;

                if(Judger.HoldQueue[i].Count == 0)
                    Key[i] = !(KeyUp[i] = true);

                if(Judger.TapQueue[i].Count != 0
                        && Charter.InTime(Judger.TapQueue[i].Peek().note.moment))
                    KeyDown[i] = Key[i] = true;

                if(Judger.HoldQueue[i].Count != 0)
                    Key[i] = true;
            }
    }
}
