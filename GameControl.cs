using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControl : MonoBehaviour
{
    public GameObject target;
    public GameObject targetLight;
    public GameObject cursor;
    public GameObject cursorLight;

    public Material ballDark;
    public Material ballLight;

    public float preRunDuration = 1;
    public float preFeedbackDuration = 2.25f;
    public float feedbackDuration = 1;
    public float postFeedbackDuration = 1;

    public float targetRadius = 0.5;

    const float COUNTDOWN_DURATION = 2.25f;

    LightControl lightControl;

    BallControl ballControl;


    void Awake() {
    }
    // Start is called before the first frame update
    void Start()
    {
	lightControl = GetComponent<LightControl>();

	DisableTarget();
	cursorLight.SetActive(false);

	StartCoroutine(ControlLoop());
    }

    // Update is called once per frame
    void Update()
    {
	
    }

    IEnumerator ControlLoop() {
	Task waitForConfig() = new Task(WaitForConfig);
	while (!waitForConfig.IsCompleted) {
	    yield return null;
	}

	SetConfig();
	lightControl.SetConfig();
	ballControl.SetConfig();
	
	while (true) {
	    Task waitForStart = new Task(WaitForStart);
	    while (!waitForStart.IsCompleted) {
		yield return null;
	    }
	    while (IsContinue()) {
		yield return new WaitForSeconds(preRunDuration);
		yield return PreTrial();
		yield return Trial();
		yield return PostTrial();
	    }
	}
    }


    IEnumerator PreTrial() {
	lightControl.DeactivateSpotlights();
	yield return new WaitForSeconds(preFeedbackDuration - COUNTDOWN_DURATION);
	yield return lightControl.Countdown();
    }

    bool lastTrialSucceeded = false;

    IEnumerator Trial() {
	ballControl.Reset();
	lastTrialSucceeded = 0;
	float time = 0;


	EnableTarget();
	OnCursor();
	ballControl.isTrialRunning = true;
	lightControl.ActiveSpotlights();

	while (time < feedbackDuration) {
	    if (Vector3.Distance(target.transform.position, cursor.transform.position).magnitude <= targetRadius) {
		lastTrialSucceeded = 1;
		break;
	    }
	    time += Time.deltaTime;
	    yield return null;
	}

	DisableTarget();
	OffCursor();
	ballControl.isTrialRunning = false;
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

    bool isContinue () {
	return true;
    }

    void WaitForConfig() {
    }

    void WaitForStart() {
    }

    void EnableTarget() {
	target.SetActive(true);
	targetLight.SetActive(true);
    }

    void DisableTarget() {
	target.SetActive(false);
	targetLight.SetActive(false);
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
    }
}

