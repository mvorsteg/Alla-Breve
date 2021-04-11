using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] shortNotes;
    [SerializeField]
    private AudioClip[] longNotes;    

    private AudioSource audioSource;
    public static AudioManager instance;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();  
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    /// <summary>
    /// Retrieves the index, in the notes sound array, of the provided note
    /// </summary>
    /// <param name="pitch">the pitch of the note</param>
    /// <param name="octave">the ocatave of the note</param>
    /// <param name="accidental">the accidental of the note</param>
    /// <returns></returns>
    private int GetIndex(Note.Pitch pitch, int octave, Note.Accidental accidental)
    {
        int idx = (int)pitch;
        if (accidental == Note.Accidental.Flat)
        {
            idx--;
        }
        else if (accidental == Note.Accidental.Sharp)
        {
            idx++;
        }
        if (octave > 4 && pitch > Note.Pitch.B)
        {
            idx += (octave - 4) * 12;
        }
        else if (octave > 3 && pitch <= Note.Pitch.B)
        {
            idx += (octave - 3) * 12;
        }

        return idx;
    }

    /// <summary>
    /// Plays a single note
    /// </summary>
    /// <param name="note">The note to be played</param>
    public void PlayNote(Note note)
    {
        int index = GetIndex(note.pitch, note.octave, note.accidental);
        audioSource.clip = shortNotes[index];
        audioSource.Play();
    }

    /// <summary>
    /// Plays all notes in a chords
    /// </summary>
    /// <param name="notes">A list of notes in the chord</param>
    public void PlayChord(List<Note> notes)
    {
        foreach (Note n in notes)
        {
            int index = GetIndex(n.pitch, n.octave, n.accidental);
            audioSource.PlayOneShot(shortNotes[index]);
        }
    }
}