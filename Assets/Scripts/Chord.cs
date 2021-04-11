using UnityEngine;
using System.Collections.Generic;

public class Chord : MonoBehaviour
{
    public enum Type
    {
        Maj,
        Min,
        Dim,
        Aug,
        Sus2,
        Sus4,
        Maj7,
        Dom7,
        Min7,
        Dim7,
        HDim7,

    }

    private Dictionary<string, string[]> chordTones;

    public List<Note> notes;  
    [SerializeField]
    private GameObject notePrefab;
    [SerializeField]
    private GameObject[] ledgerLines;

    private string rootStr;
    public Note root;
    public Type type;

    public Note.Pitch missingPitch;
    public string missingPitchStr;

    private void Start()
    {
        chordTones = new Dictionary<string, string[]>();
        chordTones.Add("Ab", new string[] {"Ab", "C", "Eb", "G"});
        chordTones.Add("A", new string[] {"A", "C#", "E", "G#"});
        chordTones.Add("Bb", new string[] {"Bb", "D", "F", "A"});
        chordTones.Add("B", new string[] {"B", "D#", "F#", "A#"});
        chordTones.Add("C", new string[] {"C", "E", "G", "B"});
        chordTones.Add("C#", new string[] {"C#", "E#", "G#", "B#"});
        chordTones.Add("Db", new string[] {"Db", "F", "Ab", "C"});
        chordTones.Add("D", new string[] {"D", "F#", "A", "C#"});
        chordTones.Add("Eb", new string[] {"Eb", "G", "Bb", "D"});
        chordTones.Add("E", new string[] {"E", "G#", "B", "D#"});
        chordTones.Add("F", new string[] {"F", "A", "C", "E"});
        chordTones.Add("F#", new string[] {"F#", "A#", "C#", "E#"});
        chordTones.Add("G", new string[] {"G", "B", "D", "F#"});
    }

    /// <summary>
    /// Creates a chord using a string "root type"
    /// Examples: CMaj, DbMin7, FSus4
    /// </summary>
    /// <param name="root">the root note of the chord</param>
    /// <param name="type">the type of chord</param>
    public void Initialize(string root, string type, LevelManager.Mode mode)
    {
        Clear();
        rootStr = root;
        Debug.Log(root+type);
        // assign type to chord
        this.type = (Type)System.Enum.Parse(typeof(Type), type);

        for (int i = 0; i < 4; i++) {
            ledgerLines[i].SetActive(false);
        }

        // expand chord to get all notes
        List<string> expansion = new List<string>();
        expansion.Add(root);
        switch (type) 
        {
            case "Maj" :
                expansion.Add(Note.GetInterval(root, 4));
                expansion.Add(Note.GetInterval(root, 7));
                break;
            case "Min" :
                expansion.Add(Note.GetInterval(root, 3));
                expansion.Add(Note.GetInterval(root, 7));
                break;
            case "Dim" :
                expansion.Add(Note.GetInterval(root, 3));
                expansion.Add(Note.GetInterval(root, 6));
                break;
            case "Aug" :
                expansion.Add(Note.GetInterval(root, 4));
                expansion.Add(Note.GetInterval(root, 8));
                break;
            case "Sus2" :
                expansion.Add(Note.GetInterval(root, 2));
                expansion.Add(Note.GetInterval(root, 7));
                break;
            case "Sus4" :
                expansion.Add(Note.GetInterval(root, 5));
                expansion.Add(Note.GetInterval(root, 7));
                break;
            case "Maj7" :
                expansion.Add(Note.GetInterval(root, 4));
                expansion.Add(Note.GetInterval(root, 7));
                expansion.Add(Note.GetInterval(root, 11));
                break;
            case "Min7" :
                expansion.Add(Note.GetInterval(root, 3));
                expansion.Add(Note.GetInterval(root, 7));
                expansion.Add(Note.GetInterval(root, 10));
                break;
            case "Dim7" :
                expansion.Add(Note.GetInterval(root, 3));
                expansion.Add(Note.GetInterval(root, 6));
                expansion.Add(Note.GetInterval(root, 9));
                break;
            case "HDim7" :
                expansion.Add(Note.GetInterval(root, 3));
                expansion.Add(Note.GetInterval(root, 6));
                expansion.Add(Note.GetInterval(root, 10));
                break;
        }

        // decide which note will be dropped
        int missingNote = Random.Range(0, expansion.Count);

        int rootOctave = -1;
        for (int i = 0; i < expansion.Count; i++)
        {
            string s = expansion[i];
            // decide octaves based on LevelManager's mode
            int noteOctave = GetOctave(s, root, rootOctave, this.type < Type.Maj7, mode);
            if (root.Equals(s))
            {   
                rootOctave = noteOctave;    
            }
            if (i == missingNote)
            {
                missingPitchStr = s;
                if (s.Contains("#"))
                {
                    s = Note.GetEnharmonic(s);
                }
                missingPitch = (Note.Pitch)System.Enum.Parse(typeof(Note.Pitch), s);
            }
            else
            {
                Note note = Instantiate(notePrefab, transform).GetComponent<Note>();
                Debug.Log(s+" "+noteOctave);
                note.Initialize(s+noteOctave); 
                float y = Note.NoteToY((Note.Pitch)System.Enum.Parse(typeof(Note.Pitch), s.ToCharArray()[0].ToString()), noteOctave);
                note.transform.localPosition = new Vector3(note.transform.localPosition.x, transform.position.y + y, note.transform.localPosition.z);
                notes.Add(note);

                // enable ledger lines
                if (y >= 4)
                {
                    ledgerLines[0].SetActive(true);
                }
                if (y >= 3)
                {
                    ledgerLines[1].SetActive(true);
                }
                if (y <= -3)
                {
                    ledgerLines[2].SetActive(true);
                }
                if (y <= -4)
                {
                    ledgerLines[3].SetActive(true);
                }
            }
        }
        this.root = notes[0];
    }

    /// <summary>
    /// Picks an octave for the note to be in, based on the root, type of chord, and gamemode
    /// </summary>
    /// <param name="note">the note we are picking an octave for</param>
    /// <param name="root">the root of the chord, which may or may not be the same as note</param>
    /// <param name="rootOctave">the octave of the root note, which we may or may not know (just pass -1 if we are the root)</param>
    /// <param name="is7th">if the chord is a 7th chord</param>
    /// <param name="mode">the gamemode</param>
    /// <returns>a valid octave for the note</returns>
    private static int GetOctave(string note, string root, int rootOctave, bool is7th, LevelManager.Mode mode)
    {
        char ch = note.ToCharArray()[0];
        char r = root.ToCharArray()[0];
        bool isRoot = ch == r;
        // if we are the root and need the chord to stack 
        if (isRoot && mode != LevelManager.Mode.Hard)
        {
            if (ch <= 'B')
                return Random.Range(3, 5);
            else if (ch <= 'D' || (ch <= 'F' && !is7th))
                return Random.Range(4, 6);
            else
                return 4;
        }
        // else if we need the chord to stack but are not root
        else if (!isRoot && mode != LevelManager.Mode.Hard)
        {
            if (r <= 'B')
                return rootOctave + 1;
            else if (ch < r && ch > 'B')
                return rootOctave + 1;
            else   
                return rootOctave;
        }
        // if we don't care if the chord is inverted
        else
        {   
            if (ch <= 'B')
                return Random.Range(3,6);
            else if (ch == 'C')
                return Random.Range(4, 7);
            else
                return Random.Range(4, 6);
        }
    }

    private void Clear()
    {
        while (notes.Count > 0)
        {
            Note n = notes[0];
            notes.RemoveAt(0);
            Destroy(n.gameObject);
        }
    }

    public override string ToString()
    {
        string str = rootStr + " ";
        switch (type)
        {
            case Type.Maj :
                str += "Major";
                break;
            case Type.Min :
                str += "Minor";
                break;
            case Type.Dim :
                str += "Diminished";
                break;
            case Type.Aug :
                str += "Augmented";
                break;
            case Type.Sus2 :
                str += "Suspended 2nd";
                break;
            case Type.Sus4 :
                str += "Suspended 4th";
                break;
            case Type.Maj7 :
                str += "Major 7th";
                break;
            case Type.Dom7 :
                str += "7th";
                break;
            case Type.Min7 :
                str += "Minor 7th";
                break;
            case Type.Dim7 :
                str += "Fully Diminished 7th";
                break;
            case Type.HDim7 :
                str += "Half Dimished 7th";
                break;
        }
        return str;
    }

}