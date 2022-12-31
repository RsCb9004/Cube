using UnityEngine;

public class Note: MonoBehaviour {

    public Charter.Chart.Note note;
    protected NoteBehaviours.GetNotePosition GetPosition;
    protected NoteBehaviours.GetNoteRotation GetRotation;

    public virtual void Hit() => Destroy(gameObject);

    public virtual void Vanish() => GetComponent<Animation>().Play();
}
