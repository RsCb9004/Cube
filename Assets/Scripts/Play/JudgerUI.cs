using UnityEngine;
using UnityEngine.UI;

public class JudgerUI: MonoBehaviour {

    private const float FadeLen = 1f / 6;

    public Text score;
    public Text combo;
    private Animation anim;

    private void Start() {
        anim = GetComponent<Animation>();
    }

    private void Update() {
        transform.rotation = Quaternion.Euler(Vector3.zero);
    }

    private void UpdText() {
        string format = string.Format(Lang.Uage.play.scoreForm, Charter.ScoreLenHex, Charter.ScoreLenDec);
        score.text = ((int)Judger.Score).ToString(format);
        combo.text = Judger.CurCombo.ToString(Lang.Uage.play.comboForm);
        if(Judger.CurCombo >= 5)
            combo.fontStyle = FontStyle.Bold;
        else
            combo.fontStyle = FontStyle.Normal;
    }

    public void Perfect() {
        UpdText();
        anim.CrossFadeQueued("Perfect", FadeLen, QueueMode.PlayNow);
    }

    public void Good() {
        UpdText();
        anim.CrossFadeQueued("Good", FadeLen, QueueMode.PlayNow);
    }

    public void Miss() {
        UpdText();
        anim.CrossFadeQueued("Miss", FadeLen, QueueMode.PlayNow);
    }
}
