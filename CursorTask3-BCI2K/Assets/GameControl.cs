using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System;
using BCI2000RemoteNET;

public class GameControl : MonoBehaviour
{
    public GameObject target;
    public GameObject targetLight;
    public GameObject cursor;
    public GameObject cursorLight;

    public Material ballDark;
    public Material ballLight;
    
    public Material targetM;
    public Material targetL;

    public float preRunDuration = 1;
    public float preFeedbackDuration = 2.25f;
    public float feedbackDuration = 1;
    public float postFeedbackDuration = 1;

    public float targetRadius = 0.5f;
    public float targetMinDistanceFromCenter = 2;

    public int n_trials = 10;

    const float COUNTDOWN_DURATION = 2.25f;

    LightControl lightControl;

    BallControl ballControl;

    MCursorControl cursorControl;

    UnityBCI2000 bci;

    void Awake() {
	bci = GameObject.Find("BCI2000").GetComponent<UnityBCI2000>();
	lightControl = GetComponent<LightControl>();
	ballControl = GameObject.Find("Sphere").GetComponent<BallControl>();
	cursorControl = GameObject.Find("MCursor").GetComponent<MCursorControl>();
	bci.OnIdle(remote => {
		remote.AddEvent("PreFeedback", 1);
		remote.AddEvent("Feedback", 1);
		remote.AddEvent("PostFeedback", 1);
		remote.AddEvent("TargetHit", 1);
		remote.AddEvent("Timeout", 1);
		remote.AddEvent("TrialNumber", 16);
		remote.AddEvent("TargetPositionX", 16);
		remote.AddEvent("TargetPositionY", 16);

		remote.AddParameter("Application:Task", "PreFeedbackDuration", preFeedbackDuration.ToString());
		remote.AddParameter("Application:Task", "FeedbackDuration", feedbackDuration.ToString());
		remote.AddParameter("Application:Task", "PostFeedbackDuration", postFeedbackDuration.ToString());
		remote.AddParameter("Application:Task", "TargetRadius", targetRadius.ToString());
		remote.AddParameter("Application:Task", "Trials", n_trials.ToString());
		});
    }
    // Start is called before the first frame updateistria
    void Start()
    {
	Debug.Log("a");

	target.SetActive(false);
	targetLight.SetActive(false);
	cursorLight.SetActive(false);

	StartCoroutine(ControlLoop());
    }

    // Update is called once per frame
    void Update()
    {
	
    }

    IEnumerator ControlLoop() {
	while (true) {
	    StartCoroutine(bci.PollSystemState(BCI2000Remote.SystemState.Resting));

	    try {
		SetConfig();
		ballControl.SetConfig();
		cursorControl.SetConfig();
	    } catch (Exception e) {
		continue;
	    }
	
	    uint trials = 0;

	    StartCoroutine(bci.PollSystemState(BCI2000Remote.SystemState.Running));

	    yield return new WaitForSeconds(preRunDuration);
	    while (IsContinue() && trials < n_trials) {
		bci.Control.SetEvent("TrialNumber", trials + 1);
		yield return PreTrial();
		yield return Trial();
		yield return PostTrial();
	    }
	}
    }


    IEnumerator PreTrial() {
	bci.Control.SetEvent("PreFeedback", 1);
	lightControl.DeactivateSpotlights();
	yield return new WaitForSeconds(preFeedbackDuration - COUNTDOWN_DURATION);

	float x_rand, y_rand;
	do {
	x_rand = UnityEngine.Random.Range(xLowerBound, xUpperBound);
	y_rand = UnityEngine.Random.Range(yLowerBound, yUpperBound);
	target.transform.position = new Vector3(x_rand, y_rand, 0);
	} while (target.transform.position.magnitude < targetMinDistanceFromCenter);
	targetLight.transform.position = new Vector3(x_rand, y_rand, 0);
	target.SetActive(true);
	bci.Control.SetEvent("TargetPositionX", (uint) ((target.transform.position.x + 7) * 1000));
	bci.Control.SetEvent("TargetPositionY", (uint) ((target.transform.position.y + 4.5) * 1000));

	yield return lightControl.Countdown();
	bci.Control.SetEvent("PreFeedback", 0);
    }

    public float xUpperBound = 7;
    public float xLowerBound = -7;
    public float yUpperBound = 4.5f;
    public float yLowerBound = -4.5f;

    bool lastTrialSucceeded = false;

    IEnumerator Trial() {
	bci.Control.SetEvent("Feedback", 1);
	lastTrialSucceeded = false;
	float time = 0;

	targetLight.SetActive(true);

	target.GetComponent<MeshRenderer>().material = targetL;
	OnCursor();
	ballControl.isTrialRunning = true;
	cursorControl.isTrialRunning = true;

	while (time < feedbackDuration) {
	    if (Vector3.Distance(target.transform.position, cursor.transform.position) <= targetRadius) {
		lastTrialSucceeded = true;
		break;
	    }
	    time += Time.deltaTime;
	    yield return null;
	}

	target.SetActive(false);
	targetLight.SetActive(false);
	target.GetComponent<MeshRenderer>().material = targetM;
	OffCursor();
	ballControl.isTrialRunning = false;
	cursorControl.isTrialRunning = false;
	lightControl.ResetCountdown();
	bci.Control.SetEvent("Feedback", 0);
	if (lastTrialSucceeded) {
	    bci.Control.PulseEvent("TargetHit", 1);
	} else {
	    bci.Control.PulseEvent("Timeout", 1);
	}
    }

    IEnumerator PostTrial() {
	bci.Control.SetEvent("PostFeedback", 1);
	Coroutine lightFlash;
	if (lastTrialSucceeded) {
	    lightFlash = StartCoroutine(lightControl.FlashGreen());
	} else {
	    lightFlash = StartCoroutine(lightControl.FlashRed());
	}
	yield return new WaitForSeconds(postFeedbackDuration);
	StopCoroutine(lightFlash);
	bci.Control.SetEvent("PostFeedback", 0);
    }

    bool IsContinue () {
	return bci.Control.GetSystemState() == BCI2000Remote.SystemState.Running;
    }



    void OffCursor() {
	cursor.GetComponent<MeshRenderer>().material = ballDark;
	cursorLight.SetActive(false);
    }

    void OnCursor() {
	cursor.GetComponent<MeshRenderer>().material = ballLight;
	cursorLight.SetActive(true);
    }

    void SetConfig() {
	try {
	    preFeedbackDuration = float.Parse(bci.Control.GetParameter("PreFeedbackDuration"));
	    feedbackDuration = float.Parse(bci.Control.GetParameter("FeedbackDuration"));
	    postFeedbackDuration = float.Parse(bci.Control.GetParameter("PostFeedbackDuration"));
	    targetRadius = float.Parse(bci.Control.GetParameter("TargetRadius"));
	} catch (FormatException e) {
	    bci.Control.Error("Could not parse one of PreFeedbackDuration, FeedbackDuration, PostFeedbackDuration, or TargetRadius as a float");
	    throw e;
	}
	try {
	    n_trials = int.Parse(bci.Control.GetParameter("Trials"));
	} catch (FormatException e) {
	    bci.Control.Error("Could not parse Trials as an int");
	    throw e;
	}

	if (preFeedbackDuration < COUNTDOWN_DURATION) {
	    bci.Control.Error("preFeedbackDuration must be greater than or equal to 2.25");
	    throw new Exception("preFeedbackDuration must be greater than or equal to 2.25");
	}
    }
}

