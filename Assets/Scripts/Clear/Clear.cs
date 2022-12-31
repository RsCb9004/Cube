using UnityEngine;
using UnityEngine.UI;

public class Clear: MonoBehaviour {

    public Text score;
    public Text count;
    public Image rate;
    public Text tip;

    [System.Serializable]
    public struct RateSet {
        public Sprite Xi, S, A, B, C, Sfc, Afc, Bfc, Cfc;
    }
    public RateSet rateImgs;

    void Start() {

        string format = string.Format(Lang.Uage.play.scoreForm, Charter.ScoreLenHex);
        score.text = ((int)Judger.Score).ToString(format);

        count.text = string.Format(
            Lang.Uage.clear.countForm,
            Judger.PerfectCnt, Judger.GoodCnt, Judger.MissCnt, Judger.MaxCombo,
            Judger.AllPerfect ? Lang.Uage.clear.apNote :
            Judger.FullCombo ? Lang.Uage.clear.fcNote :
            Lang.Uage.clear.noNote
        );

        rate.sprite = GetRate(Judger.Score);
        rate.SetNativeSize();

        tip.text = string.Format(Lang.Uage.clear.tip, "D", "F");
    }

    private void Update() {
        if(Input.GetButtonDown("OK")) {
            Loader.LoadScene("MainPage");
        }
        if(Input.GetButtonDown("Cancel")) {
            Loader.LoadScene("Play");
        }
    }

    private Sprite GetRate(float score) {
        if(Judger.AllPerfect)
            return rateImgs.Xi;
        if(Judger.FullCombo) {
            if(score >= Charter.Setting.minScoreS)
                return rateImgs.Sfc;
            if(score >= Charter.Setting.minScoreA)
                return rateImgs.Afc;
            if(score >= Charter.Setting.minScoreB)
                return rateImgs.Bfc;
            return rateImgs.Cfc;
        }
        if(score >= Charter.Setting.minScoreS)
            return rateImgs.S;
        if(score >= Charter.Setting.minScoreA)
            return rateImgs.A;
        if(score >= Charter.Setting.minScoreB)
            return rateImgs.B;
        return rateImgs.C;
    }
}
