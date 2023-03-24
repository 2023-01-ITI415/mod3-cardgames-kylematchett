using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

[RequireComponent(typeof(BezierMover))]
[RequireComponent(typeof(TMP_Text))] // FloatingScore will require TextMeshPro
public class FloatingScore : MonoBehaviour
{
    static List<FloatingScore> FS_ALL = new List<FloatingScore>();

    [Header("Inscribed")]
    public float[] fontSizes = { 10, 56, 48 }; // Scaled viaa Bezier curve // a

    [Header("Dynamic")]
    [SerializeField]
    private int _score = 0; // The backing field for score 
                            // b
    public int score
    {
        get { return (_score); }
        // c
        set
        {
            _score = value;
           
            textField.text = _score.ToString("#,##0") + " x " + ScoreManager.multiplier;// The0 is a zero 
            // d 
        }
    }

    // Define a function delegate type with one FloatingScore parameter.
    public delegate void FloatingScoreDelegate(FloatingScore fs); // e
    public event FloatingScoreDelegate FSCallbackEvent;
    // f

    private TMP_Text textField;
    private BezierMover mover;

    void Awake()
    { // Remember to replace the entire Awake method!
        textField = GetComponent<TMP_Text>();
        mover = GetComponent<BezierMover>();
    }

    /// <summary>
    /// This is largely a passthrough for BezierMover
    /// </summary>
    /// <param name="ePts">List of Vector3 Bezierpoints</param>
    /// <param name="eTimeD">The duration of themovement</param>
    /// <param name="eTimeS">The start time (default of 0 isnow)</param>
    public void Init(List<Vector2> ePts, float eTimeD = 1, float eTimeS = 0)
    {

        mover.completionEvent.AddListener(MoverCompleteCallback);
        mover.Init(ePts, eTimeD, eTimeS);
    }

    /// <summary>
    /// Update here largely manages the font scaling oftextField
    /// </summary>
    void Update()
    {
        if (mover.state == BezierMover.eState.active)
        {
            // If the mover is active, we may need to scalefonts
            if (fontSizes != null && fontSizes.Length > 0)
            {
                float size = Utils.Bezier(mover.uCurved, fontSizes); // g
                textField.fontSize = size;
            }
        }
    }

    /// <summary>
    /// Called by the UnityEvent in BezierMover when movementis complete
    /// </summary>
    void MoverCompleteCallback()
    { // Remember to replacethis entire method!
      // If there is a listener registered with thiscallback...
        if (FSCallbackEvent != null)
        {
            // h
            // then invoke the callback...
            FSCallbackEvent(this);
            FSCallbackEvent = null; // Clear out anyregistered methods
                                    // and destroy this GameObject
            Destroy(gameObject);
        }
        // If there was no listener registered, don’t destroythis GameObject
    }

    /// <summary>
    /// This is called by the FSCallbackEvent on otherFloatingScores, which
    /// allows them to add their score to this one.
    /// </summary>
    /// <param name="fs">A FloatingScore passing its score tothis one</param>
    public void FSCallback(FloatingScore fs)
    {
        score += fs.score;
    } // i

    void OnEnable() { FS_ALL.Add(this); }
    // j
    void OnDisable() { FS_ALL.Remove(this); }

    /// <summary>
    /// Reroute all existing FloatingScores to ScoreBoard atend of game
    /// </summary>
    static public void REROUTE_TO_SCOREBOARD()
    {
        // k
        Vector2 fsPosEnd = new Vector2(0.5f, 0.95f);
        foreach (FloatingScore fs in FS_ALL)
        {
            fs.mover.bezierPts[fs.mover.bezierPts.Count - 1]
           = fsPosEnd;
            fs.FSCallbackEvent = null;
            fs.FSCallbackEvent += ScoreBoard.FS_CALLBACK;
            // l
        }
        // Clear FS_ALL
        FS_ALL.Clear();
    }

}
