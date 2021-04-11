using UnityEngine;
using UnityEngine.UI;

public class PlayerNote : Note {

    private float yCurr;

    private Camera mainCamera;
    [SerializeField]
    private Text noteText;
    private bool dragging = false;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.GetRayIntersection (ray, Mathf.Infinity);          
            if (dragging || hit.collider != null && hit.collider.transform == this.transform) {
                float yRaw = mainCamera.ScreenToWorldPoint(Input.mousePosition).y;
                int yInt = Mathf.RoundToInt(2 * yRaw);
                float yClamped = Mathf.Clamp(yInt / 2f, -4f, 4f);
                if (yClamped != yCurr)
                {
                    dragging = true;
                    MoveNote(yClamped);
                    yCurr = yClamped;
                }
            }
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            dragging = false;
        }
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            CycleAccidental();
        }
        noteText.text = this.ToString();
        if (accidental == Accidental.Flat)
        {

        }
    }

    /// <summary>
    /// Cycles between sharp, flat, and natural
    /// </summary>
    private void CycleAccidental()
    {
        if (accidental == Accidental.Natural)
        {
            SetAccidental(Accidental.Sharp);
        }
        else if (accidental == Accidental.Sharp)
        {
            SetAccidental(Accidental.Flat);
        }
        else
        {
            SetAccidental(Accidental.Natural);
        }
        AudioManager.instance.PlayNote(this);
    }


}