using System.Collections.Generic;
using UnityEngine;

public class NoteHold: NoteTap {

    public Sprite[] tails;
    public Material[] materials;
    public Transform tail;
    public LineRenderer line;

    public Gradient missedGradient;
    public Color missedColor;

    private bool hitted;

    protected override void Start() {

        GetComponent<SpriteRenderer>().sprite = sprites[note.style];
        tail.GetComponent<SpriteRenderer>().sprite = tails[note.style];
        line.material = materials[note.style];

        GetPosition = NoteBehaviours.GetNotePositions[note.style];
        GetRotation = NoteBehaviours.GetNoteRotations[note.style];

        Update();
    }

    protected override void Update() {

        if(!hitted) base.Update();

        tail.position = GetPosition(note.direction, Charter.GetTime(note.endMoment, note.speed));

        transform.rotation = Charter.Rot2Quat(GetRotation(note.direction, Charter.GetTime(note.moment, note.speed)));
        tail.rotation = Charter.Rot2Quat(GetRotation(note.direction, Charter.GetTime(note.endMoment, note.speed)));

        List<Vector3> nodes = new List<Vector3>();

        if(hitted)
            nodes.Add(GetPosition(note.direction, 0));

        for(
            float moment = note.moment;
            moment < note.endMoment
                && Charter.InTime(Charter.GetStartMoment(moment, note.speed));
            moment += Charter.Setting.holdNodeInterval
        )
            if(!hitted || !Charter.InTime(moment))
                nodes.Add(GetPosition(note.direction, Charter.GetTime(moment, note.speed)));

        if(Charter.InTime(Charter.GetStartMoment(note.endMoment, note.speed)))
            nodes.Add(tail.position);

        line.positionCount = nodes.Count;
        line.SetPositions(nodes.ToArray());

        if(Charter.InTime(note.endMoment)) Destroy(transform.parent.gameObject);
    }

    public override void Hit() {
        GetComponent<SpriteRenderer>().enabled = false;
        hitted = true;
        Judger.HoldQueue[note.direction].Enqueue(this);
    }

    public override void Vanish() {
        GetComponent<SpriteRenderer>().enabled = false;
        hitted = true;
        line.colorGradient = missedGradient;
        tail.GetComponent<SpriteRenderer>().color = missedColor;
    }
}
