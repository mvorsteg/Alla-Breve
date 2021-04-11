using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    [SerializeField]
    private Chord chord;
    [SerializeField]
    private PlayerNote player;
    [SerializeField]
    private Text chordText;
    [SerializeField]
    private GameObject popup;
    private Text popupTitle;
    private Text popupBody;
    [SerializeField]
    private Text scoreText;

    public enum Mode
    {
        Easy,
        Medium,
        Hard,
        Jazz
    }

    public Mode mode;
    public float roundTime = 5f;

    public int points = 0;

    private Vector3 chordStart;
    private Vector3 chordEnd;
    private string[] roots;
    private string[] types;

    private void Awake()
    {
        popupTitle = popup.transform.GetChild(0).GetComponent<Text>();
        popupBody = popup.transform.GetChild(1).GetComponent<Text>();    
    }

    private void Start()
    {
        mode = (LevelManager.Mode)MainMenu.instance.mode;
        Debug.Log(mode);
        SetMode(mode);
        //GenerateChord();
        chordStart = chord.transform.position;
        chordEnd = new Vector3(player.transform.position.x, chordStart.y, player.transform.position.z);
        StartRound();

    }

    private void SetMode(Mode mode)
    {
        if (mode == Mode.Easy)
        {
            roots = new string[] { "C", "G", "D", "A", "F", "Bb", "Ab" };
            types = new string[] { "Maj", "Min" };
            roundTime = 5f;
        }
        else if (mode == Mode.Medium)
        {
            roots = new string[] { "C", "C#", "Db", "D", "Eb", "E", "F", "F#", "G", "Ab", "A", "Bb", "B" };
            types = new string[] { "Maj", "Min" };
            roundTime = 5f;
        }
        else if (mode == Mode.Hard)
        {
            roots = new string[] { "C", "C#", "Db", "D", "Eb", "E", "F", "F#", "G", "Ab", "A", "Bb", "B" };
            types = new string[] { "Maj", "Min", "Dim", "Aug" };
            roundTime = 8f;
        }
        else if (mode == Mode.Jazz)
        {
            roots = new string[] { "C", "C#", "Db", "D", "Eb", "E", "F", "F#", "G", "Ab", "A", "Bb", "B" };
            types = new string[] { "Sus2", "Sus4", "Maj7", "Dom7", "Min7", "Dim7", "HDim7" };
            roundTime = 8f;
        }   

        this.mode = mode;
    }

    private void GenerateChord()
    {
        string root = roots[Random.Range(0, roots.Length)];
        string type = types[Random.Range(0, types.Length)];
        chord.Initialize(root, type, mode);
        //chord.Initialize("C", "Maj7", Mode.Easy);
        Debug.Log(chord.root);
        chordText.text = "Make a" + (chord.root.IsAVowel() ? "n " : " ") + chord.ToString() + " chord";
    }

    public void StartRound()
    {
        popup.SetActive(false);
        chord.transform.position = chordStart;
        GenerateChord();
        StartCoroutine(MoveChordOver());
    }

    private IEnumerator MoveChordOver()
    {
        float elapsedTime = 0;
        while (elapsedTime < roundTime)
        {
            elapsedTime += Time.deltaTime;
            chord.transform.position = Vector3.Lerp(chordStart, chordEnd, elapsedTime / roundTime);
            yield return null;
        }
        EvaluateChord();
    }

    private void EvaluateChord()
    {
        AudioManager.instance.PlayChord(chord.notes);
        AudioManager.instance.PlayNote(player);
        Note.Pitch p = player.pitch;
        if (player.accidental == Note.Accidental.Flat)
        {
            p = p - 1;
        }
        else if (player.accidental == Note.Accidental.Sharp)
        {
            p = p + 1;
        }
        if (p == chord.missingPitch)
        {
            popupTitle.text = "Correct!";
            popupBody.text = "You win a point";
            points++;
            scoreText.text = "score: " + points.ToString();
        }
        else
        {
            popupTitle.text = "Incorrect!";
            popupBody.text = "A" + (chord.root.IsAVowel() ? "n " : " ") + chord.ToString() + " chord contains a" + (player.IsAVowel() ? "n " : " ") + chord.missingPitchStr;
        }
        popup.SetActive(true);
    } 

    public void GoBackToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

}