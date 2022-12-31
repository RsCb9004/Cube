using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class Charter: MonoBehaviour {

    public enum NoteType {
        barLine, tap, drag, hold
    }

    [Serializable]
    public class Chart {
        [Serializable]
        public class Information {
            public string version = "0.2";
            public float bpm;
            public float length;
            public float offset = 0;
        }
        [Serializable]
        public class Setting {
            public float holdNodeInterval = 0.5f;
            public float maxScore = 65536;
            public float comboScoreRate = 0.0625f;
            public float goodRange = 0.16f;
            public float perfectRange = 0.08f;
            public float goodScoreRate = 0.6f;
            public float minScoreS = 61440;
            public float minScoreA = 57334;
            public float minScoreB = 40960;
        }
        [Serializable]
        public class Momentable {
            public float moment;
        }
        [Serializable]
        public class Note: Momentable {
            public float endMoment;
            public int direction;
            public int style;
            public float speed = 1;
        }
        [Serializable]
        public class Events {
            [Serializable]
            public class BpmChange: Momentable {
                public float bpm;
            }
            [Serializable]
            public class JudgerMovement: Momentable {
                public Vector2 position;
                public float timeTaken = 1;
            }
            [Serializable]
            public class JudgerRotation: Momentable {
                public float rotation;
                public float timeTaken = 1;
            }

            public List<BpmChange> bpmChanges = new List<BpmChange>();
            public List<JudgerMovement> judgerMovements = new List<JudgerMovement>();
            public List<JudgerRotation> judgerRotations = new List<JudgerRotation>();
        }

        public Information information = new Information();
        public Setting setting = new Setting();
        public List<Note> barLines = new List<Note>();
        public List<Note> taps = new List<Note>();
        public List<Note> drags = new List<Note>();
        public List<Note> holds = new List<Note>();
        public Events events = new Events();
    }

    public const float BeforeTime = 2;
    public const float Sqrt2 = 1.4142135623730951f;
    public const float DistancePerBeat = 2 * Sqrt2;
    public const float NoteExistTime = 12;
    public const float MoveBackTime = 1;
    private const float EndAnimationTime = 1;

    public static Chart.Setting Setting { get; private set; }
    public static int NoteCount { get; private set; }
    public static int ScoreLenHex { get; private set; }
    public static int ScoreLenDec { get; private set; }

    public static float CurBpm { get; private set; }
    public static float CurMoment { get; private set; }

    public static string ChartJson { get; set; }
    public static string Bga { get; set; }

    public static Quaternion Rot2Quat(float rotation) => Quaternion.Euler(Vector3.forward * rotation);
    public static float Sec2Beat(float sec) => sec * CurBpm / 60;
    public static float Beat2Sec(float beat) => beat * 60 / CurBpm;
    public static float GetTime(float moment, float speed = 1) => (moment - CurMoment) / speed;
    public static bool InTime(float moment) => GetTime(moment) <= 0;

    public static float GetStartMoment(float moment, float speed) => moment - speed * NoteExistTime;

    private static float GetStartMoment(Chart.Momentable obj) =>
        obj is Chart.Note note ? GetStartMoment(obj.moment, note.speed) : obj.moment;

    private static bool Check<T>(int cur, List<T> list) where T : Chart.Momentable =>
        cur < list.Count && InTime(GetStartMoment(list[cur]));

    public JudgerAnimation judger;
    public GameObject[] notes;

    private VideoPlayer video;

    private float playMoment;
    private bool prepared;
    private bool played;
    private bool loaded;

    private Chart curChart;
    private int curBarLine;
    private int curTap;
    private int curDrag;
    private int curHold;
    private int curBpmChange;
    private int curJudgerMovement;
    private int curJudgerRotation;

    private void Start() {
        
        curChart = JsonUtility.FromJson<Chart>(ChartJson);
        ChartInit(curChart);
        
        video = GetComponent<VideoPlayer>();
        video.url = Bga;
        video.Prepare();

        Setting = curChart.setting;
        NoteCount = curChart.taps.Count + curChart.drags.Count + 2 * curChart.holds.Count;
        ScoreLenHex = (int)Mathf.Log(Setting.maxScore, 16) + 1;
        ScoreLenDec = (int)Mathf.Log(Setting.maxScore, 10) + 1;

        CurBpm = curChart.information.bpm;
        CurMoment = float.MinValue;
        playMoment = Sec2Beat(curChart.information.offset);
        played = GamePause.Pausable = false;

        curBarLine = curTap = curBpmChange = curJudgerMovement = curJudgerRotation = 0;
    }

    private void Update() {

        CurMoment += Sec2Beat(Time.deltaTime);

        if(!prepared && video.isPrepared) {
            CurMoment = Sec2Beat(-BeforeTime);
            prepared = true;
        }

        if(!played && InTime(playMoment)) {
            video.Play();
            played = GamePause.Pausable = true;
        }

        while(Check(curBpmChange, curChart.events.bpmChanges))
            CurBpm = curChart.events.bpmChanges[curBpmChange++].bpm;

        while(Check(curJudgerMovement, curChart.events.judgerMovements))
            JudgerMove(curChart.events.judgerMovements[curJudgerMovement++]);

        while(Check(curJudgerRotation, curChart.events.judgerRotations))
            JudgerRotate(curChart.events.judgerRotations[curJudgerRotation++]);

        while(Check(curBarLine, curChart.barLines))
            CreateNote(NoteType.barLine, curChart.barLines[curBarLine++]);

        while(Check(curTap, curChart.taps))
            CreateNote(NoteType.tap, curChart.taps[curTap++]);

        while(Check(curDrag, curChart.drags))
            CreateNote(NoteType.drag, curChart.drags[curDrag++]);

        while(Check(curHold, curChart.holds))
            CreateNote(NoteType.hold, curChart.holds[curHold++]);

        if(InTime(curChart.information.length) && !loaded) {
            GamePause.Pausable = false;
            StartCoroutine(judger.Move(Vector2.zero, Sec2Beat(MoveBackTime)));
            StartCoroutine(judger.End());
            StartCoroutine(End());
            loaded = true;
        }
    }

    private static void ChartInit(Chart chart) {

        chart.events.bpmChanges.Sort((a, b) => a.moment.CompareTo(b.moment));
        chart.events.judgerMovements.Sort((a, b) => a.moment.CompareTo(b.moment));
        chart.events.judgerRotations.Sort((a, b) => a.moment.CompareTo(b.moment));

        chart.barLines.Sort((a, b) => GetStartMoment(a).CompareTo(GetStartMoment(b)));
        chart.taps.Sort((a, b) => GetStartMoment(a).CompareTo(GetStartMoment(b)));
        chart.drags.Sort((a, b) => GetStartMoment(a).CompareTo(GetStartMoment(b)));
        chart.holds.Sort((a, b) => GetStartMoment(a).CompareTo(GetStartMoment(b)));
    }

    private void JudgerMove(Chart.Events.JudgerMovement movement) =>
        StartCoroutine(judger.Move(movement.position, movement.timeTaken));

    private void JudgerRotate(Chart.Events.JudgerRotation rotation) =>
        StartCoroutine(judger.Rotate(rotation.rotation, rotation.timeTaken));

    private void CreateNote(NoteType type, Chart.Note note) =>
        Instantiate(notes[(int)type], transform).GetComponentInChildren<Note>().note = note;

    private IEnumerator End() {
        AsyncOperation operation = SceneManager.LoadSceneAsync("Clear");
        operation.allowSceneActivation = false;
        yield return new WaitForSeconds(MoveBackTime + EndAnimationTime);
        operation.allowSceneActivation = true;
    }
}
