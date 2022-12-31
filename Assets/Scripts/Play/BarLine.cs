using UnityEngine;

public class BarLine: Note {

    private LineRenderer line;

    private void Start() {

        line = GetComponent<LineRenderer>();

        GetPosition = NoteBehaviours.GetNotePositions[0];
    }

    private void Update() {

        Vector3[] tmp = new Vector3[4];
        for(int i = 0; i < 4; i++) {
            tmp[i] = GetPosition(i, Charter.GetTime(note.moment, note.speed));
        }
        line.SetPositions(tmp);

        if(Charter.InTime(note.moment)) Vanish();
    }

    public override void Vanish() =>
        Destroy(gameObject);
}
