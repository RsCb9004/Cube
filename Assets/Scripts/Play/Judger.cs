using System.Collections.Generic;
using UnityEngine;

public class Judger: MonoBehaviour {

    private static readonly string[] KeyNames = {
        "Up", "Right", "Down", "Left"
    };

    public static Queue<NoteTap>[] TapQueue { get; set; }
    public static Queue<NoteDrag>[] DragQueue { get; set; }
    public static Queue<NoteHold>[] HoldQueue { get; set; }

    public static int PerfectCnt { get; private set; }
    public static int GoodCnt { get; private set; }
    public static int MissCnt { get; private set; }

    public static int CurCombo { get; private set; }
    public static int MaxCombo { get; private set; }

    public static bool FullCombo { get; private set; }
    public static bool AllPerfect { get; private set; }

    public static float Score { get; private set; }
    private static float noteScore;
    private static float comboScore;

    public static int Focus { get; private set; }

    private static List<int> keyList;

    public JudgerUI UI;
    private JudgerAnimation anim;

    private class TickerTimer {
        public readonly float period;
        public bool down;
        private float nxtTick;
        public TickerTimer(float _period) => period = _period;
        public void Update() {
            down = false;
            while(Charter.InTime(nxtTick)) {
                down = true;
                nxtTick += period;
            }
        }
    }
    private TickerTimer timer;

    private void Start() {

        TapQueue = new Queue<NoteTap>[4]{
            new Queue<NoteTap>(), new Queue<NoteTap>(), new Queue<NoteTap>(), new Queue<NoteTap>()
        };
        DragQueue = new Queue<NoteDrag>[4]{
            new Queue<NoteDrag>(), new Queue<NoteDrag>(), new Queue<NoteDrag>(), new Queue<NoteDrag>()
        };
        HoldQueue = new Queue<NoteHold>[4]{
            new Queue<NoteHold>(), new Queue<NoteHold>(), new Queue<NoteHold>(), new Queue<NoteHold>()
        };
        keyList = new List<int>();

        PerfectCnt = GoodCnt = MissCnt = CurCombo = MaxCombo = 0;
        FullCombo = AllPerfect = true;
        Score = noteScore = comboScore = 0;

        anim = GetComponent<JudgerAnimation>();

        timer = new TickerTimer(Charter.Setting.holdNodeInterval);
    }

    private void Update() {

        if(GamePause.Paused) return;

        timer.Update();

        for(int i = 0; i < 4; i++) {

            if(TapQueue[i].Count != 0 && GetKeyDown(i)) {
                NoteTap tar = TapQueue[i].Dequeue();
                Judge(
                    Mathf.Abs(Charter.GetTime(tar.note.moment))
                        < Charter.Sec2Beat(Charter.Setting.perfectRange) ?
                    JudgeType.perfect : JudgeType.good
                );
                tar.Hit();
                anim.Strike(i);
            }
            while(TapQueue[i].Count != 0
                && Charter.GetTime(TapQueue[i].Peek().note.moment)
                    < -Charter.Sec2Beat(Charter.Setting.goodRange)) {
                Judge(JudgeType.miss);
                TapQueue[i].Dequeue().Vanish();
            }

            while(DragQueue[i].Count != 0 && GetKey(i)) {
                Judge(JudgeType.perfect);
                DragQueue[i].Dequeue().Hit();
                anim.Strike(i);
            }
            while(DragQueue[i].Count != 0
                && Charter.GetTime(DragQueue[i].Peek().note.moment)
                    < -Charter.Sec2Beat(Charter.Setting.goodRange)) {
                Judge(JudgeType.miss);
                DragQueue[i].Dequeue().Vanish();
            }

            if(HoldQueue[i].Count != 0 && timer.down)
                anim.Strike(i, true);
            while(HoldQueue[i].Count != 0
                    && Charter.InTime(HoldQueue[i].Peek().note.endMoment)) {
                Judge(JudgeType.perfect);
                HoldQueue[i].Dequeue();
            }
            while(HoldQueue[i].Count != 0 && !GetKey(i)) {
                Judge(JudgeType.miss);
                HoldQueue[i].Dequeue().Vanish();
            }

            if(GetKeyDown(i)) {
                anim.Highlight(i);
                if(!keyList.Contains(i)) keyList.Add(i);
            }
            if(GetKeyUp(i)) {
                anim.Unlight(i);
                keyList.Remove(i);
            }
            Focus = keyList.Count != 0 ? keyList[0] : -1;
        }
    }

    private static bool GetKeyDown(int direction) {
        if(AutoPlay.Working) return AutoPlay.KeyDown[direction];
        return Input.GetButtonDown(KeyNames[direction])
            || (Focus == direction && Input.GetButtonDown("Assist"));
    }

    private static bool GetKey(int direction) {
        if(AutoPlay.Working) return AutoPlay.Key[direction];
        return Input.GetButton(KeyNames[direction]);
    }

    private static bool GetKeyUp(int direction) {
        if(AutoPlay.Working) return AutoPlay.KeyUp[direction];
        return Input.GetButtonUp(KeyNames[direction]);
    }

    private enum JudgeType {
        perfect, good, miss
    }

    private void Judge(JudgeType type) {
        switch(type) {

        case JudgeType.perfect:
            PerfectCnt++;
            noteScore += Charter.Setting.maxScore * (1 - Charter.Setting.comboScoreRate)
                / Charter.NoteCount;
            MaxCombo = Mathf.Max(MaxCombo, ++CurCombo);
            comboScore = Charter.Setting.maxScore * Charter.Setting.comboScoreRate
                / Charter.NoteCount * MaxCombo;
            Score = noteScore + comboScore;
            UI.Perfect();
            break;

        case JudgeType.good:
            GoodCnt++; AllPerfect = false;
            noteScore += Charter.Setting.maxScore * (1 - Charter.Setting.comboScoreRate)
                / Charter.NoteCount * Charter.Setting.goodScoreRate;
            MaxCombo = Mathf.Max(MaxCombo, ++CurCombo);
            comboScore = Charter.Setting.maxScore * Charter.Setting.comboScoreRate
                / Charter.NoteCount * MaxCombo;
            Score = noteScore + comboScore;
            UI.Good();
            break;

        case JudgeType.miss:
            MissCnt++; CurCombo = 0; AllPerfect = FullCombo = false;
            UI.Miss();
            break;
        }
    }
}
