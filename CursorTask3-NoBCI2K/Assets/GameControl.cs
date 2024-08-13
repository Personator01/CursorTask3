using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

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

    void Awake() {
	lightControl = GetComponent<LightControl>();
	ballControl = GameObject.Find("Sphere").GetComponent<BallControl>();
	cursorControl = GameObject.Find("MCursor").GetComponent<MCursorControl>();
    }
    // Start is called before the first frame update
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

	    try {
		SetConfig();
		ballControl.SetConfig();
		cursorControl.SetConfig();
	    } catch (Exception e) {
		continue;
	    }
	
	    uint trials = 0;

	    yield return new WaitForSeconds(preRunDuration);
	    while (IsContinue() && trials < n_trials) {
		yield return PreTrial();
		yield return Trial();
		yield return PostTrial();
	    }
	}
    }


    IEnumerator PreTrial() {
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

	yield return lightControl.Countdown();
    }

    public float xUpperBound = 7;
    public float xLowerBound = -7;
    public float yUpperBound = 4.5f;
    public float yLowerBound = -4.5f;

    bool lastTrialSucceeded = false;

    IEnumerator Trial() {
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
    }

    IEnumerator PostTrial() {
	Coroutine lightFlash;
	if (lastTrialSucceeded) {
	    lightFlash = StartCoroutine(lightControl.FlashGreen());
	} else {
	    lightFlash = StartCoroutine(lightControl.FlashRed());
	}
	yield return new WaitForSeconds(postFeedbackDuration);
	StopCoroutine(lightFlash);
    }

    bool IsContinue () {
	return true;
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
	



	if (preFeedbackDuration < COUNTDOWN_DURATION) {
	    throw new Exception("preFeedbackDuration must be greater than or equal to 2.25");
	}
    }
}

