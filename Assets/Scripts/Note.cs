using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Note : MonoBehaviour
{
    public enum Pitch
    {
        Ab,
        A,
        Bb,
        B,
        C,
        Db,
        D,
        Eb,
        E,
        F,
        Gb,
        G
    }

    public enum Accidental
    {
        Natural,
        Flat,
        Sharp
    }

    public Pitch pitch;
    public Accidental accidental;
    public int octave;

    [SerializeField]
    protected GameObject flatIcon;
    [SerializeField]
    protected GameObject sharpIcon;
    [SerializeField]
    private GameObject[] ledgerLines;

    /// <summary>
    /// Creates a note using a string "root b/# octave"
    /// Example: e5, c#4, Bb5
    /// </summary>
    /// <param name="str">the string used to create the note</param>
    public void Initialize(string str)
    {
        if (str.Length != 2 && str.Length != 3)
        {

        }
        else
        {
            char[] chs = str.ToCharArray();
            pitch = (Note.Pitch)System.Enum.Parse(typeof(Note.Pitch), chs[0].ToString());
            if (str.Length == 3)
            {
                if (chs[1] == 'b')
                {
                    SetAccidental(Accidental.Flat);
                }
                else if (chs[1] == '#')
                {
                    SetAccidental(Accidental.Sharp);
                }
                else
                {
                    SetAccidental(Accidental.Natural);
                }
            }
            else
            {
                SetAccidental(Accidental.Natural);
            }
            octave = (int)System.Char.GetNumericValue(chs[str.Length - 1]);
        }
    }

    /// <summary>
    /// Modifies the pitch of the note by adding a sharp, flat or natural
    /// </summary>
    /// <param name="newAccidental">The new accidental to put on the note</param>
    protected void SetAccidental(Accidental newAccidental)
    {
        //pitch = (Pitch)((int)(pitch - 1) % 12);
        if (newAccidental == Accidental.Natural)
        {
            accidental = Accidental.Natural;
            sharpIcon.SetActive(false);
            flatIcon.SetActive(false);
        }
        else if (newAccidental == Accidental.Sharp)
        {
            accidental = Accidental.Sharp;
            sharpIcon.SetActive(true);
            flatIcon.SetActive(false);
        }
        else if (newAccidental == Accidental.Flat)
        {
            accidental = Accidental.Flat;
            sharpIcon.SetActive(false);
            flatIcon.SetActive(true);
        }
    }

    /// <summary>
    /// Places the note on a line or space, modifying pitch and octave
    /// Pass in a y-coordinate, and the note will snap to the closest valid spot
    /// </summary>
    /// <param name="yNew">The new y-coordinate, the note will move to the closest line or space</param>
    protected void MoveNote(float yNew)
    {
        transform.position = new Vector3(transform.position.x, yNew, transform.position.z);

        pitch = YToPitch(yNew);
        accidental = Accidental.Natural;
        octave = YToOctave(yNew);
        flatIcon.SetActive(false);
        sharpIcon.SetActive(false);

        ActivateLedgerLines(yNew);

        AudioManager.instance.PlayNote(this);
    }

    /// <summary>
    /// creates a string representation of the note
    /// </summary>
    /// <returns>the note string</returns>
    public override string ToString()
    {
        string acc = "";
        if (accidental == Accidental.Sharp)
        {   
            acc = "\u266F";
        }
        else if (accidental == Accidental.Flat)
        {
            acc = "\u266D";
        }
        return pitch.ToString() + acc + octave.ToString();
    }

    public bool IsAVowel()
    {
        return pitch == Note.Pitch.A || pitch == Note.Pitch.Ab || pitch == Note.Pitch.E || pitch == Note.Pitch.Eb;
    }

    /// <summary>
    /// converts a y-coordinate into a pitch
    /// </summary>
    /// <param name="val">the y-coordinate of the note</param>
    /// <returns>the note's corresponding pitch</returns>
    public static Pitch YToPitch(float val)
    {
        // assign pitch
        Pitch pitch = Pitch.C;
        if (val == -4 || val == -0.5 || val == 3)
        {
            pitch = Pitch.A;
        }
        else if (val == -3.5 || val == 0 || val == 3.5)
        {
            pitch = Pitch.B;
        }
        else if (val == -3 || val == 0.5 || val == 4)
        {
            pitch = Pitch.C;
        }
        else if (val == -2.5 || val == 1)
        {
            pitch = Pitch.D;
        }
        else if (val == -2 || val == 1.5)
        {
            pitch = Pitch.E;
        }
        else if (val == -1.5 || val == 2)
        {
            pitch = Pitch.F;
        }
        else if (val == -1 || val == 2.5)
        {
            pitch = Pitch.G;
        }

        return pitch;
    }

    /// <summary>
    /// Converts a y-coordinate into an octave
    /// </summary>
    /// <param name="val">the note's y-coordinate</param>
    /// <returns>the note's octave</returns>
    public static int YToOctave(float val)
    {
        if (val < -3)
        {
            return 3;
        }
        else if (val < 0.5)
        {
            return 4;
        }
        else if (val  < 4)
        {
            return 5;
        }
        return 6;
    }

    public static float NoteToY(Pitch pitch, int octave)
    {
        if (pitch == Pitch.A)
        {
            return -4f + 3.5f * (octave - 3);
        }
        else if (pitch == Pitch.B)
        {
            return -3.5f + 3.5f * (octave - 3);
        }
        else if (pitch == Pitch.C)
        {
            return -3f + 3.5f * (octave - 4);
        }
        else if (pitch == Pitch.D)
        {
            return -2.5f + 3.5f * (octave - 4);
        }
        else if (pitch == Pitch.E)
        {
            return -2f + 3.5f * (octave - 4);
        }
        else if (pitch == Pitch.F)
        {
            return -1.5f + 3.5f * (octave - 4);
        }
        else if (pitch == Pitch.G)
        {
            return -1f + 3.5f * (octave - 4);
        }
        return 0;
    }

    public void ActivateLedgerLines(float yVal)
    {
        // enable/disable ledgerlines
        for (int i = 0; i < 4; i++) {
            ledgerLines[i].SetActive(false);
        }

        if (yVal >= 4)
        {
            ledgerLines[0].SetActive(true);
        }
        if (yVal >= 3)
        {
            ledgerLines[1].SetActive(true);
        }
        if (yVal <= -3)
        {
            ledgerLines[2].SetActive(true);
        }
        if (yVal <= -4)
        {
            ledgerLines[3].SetActive(true);
        }
    }

    public static string GetInterval(string root, int halfSteps)
    {
        string safeRoot = root.Contains("#") ? GetEnharmonic(root) : root;
        Pitch pitch = (Pitch)System.Enum.Parse(typeof(Pitch), safeRoot);
        Pitch interval = (Pitch)((int)(pitch + halfSteps) % 12);
        string note = interval.ToString();
        // check to see if we need enharmonic
        char r = root.ToCharArray()[0];
        char n = note.ToCharArray()[0];
        if ((r < n && (n - r) % 2 != 0) || (r > n && (r - n) % 2 != 1))
        {
            return GetEnharmonic(note);
        }
        return note;
    }

    public static string GetEnharmonic(string note)
    {
        if (note.Equals("E#"))
            return "F";
        if (note.Equals("F"))
            return "E#";
        if (note.Equals("Fb"))
            return "E";
        if (note.Equals("B#"))
            return "C";
        if (note.Equals("Cb"))
            return "B";
        if (note.Equals("C"))
            return "B#";
        if (note.Length == 1)
            return note;
        char[] chs = note.ToCharArray();
        if (chs[1] == 'b')
        {
            chs[1] = '#';
            if (chs[0] == 'A')
                chs[0] = 'G';
            else
                chs[0]--;
        }
        else if (chs[1] == '#')
        {
            chs[1] = 'b';
            if (chs[0] == 'G')
                chs[0] = 'A';
            else
                chs[0]++;
        }
        return "" + chs[0] + chs[1];
    }

}
