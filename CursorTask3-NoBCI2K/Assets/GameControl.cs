using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    const float COUNTDOWN_DURATION = 2.25f;

    LightControl lightControl;

    BallControl ballControl;



    void Awake() {
	lightControl = GetComponent<LightControl>();
	ballControl = GameObject.Find("Sphere").GetComponent<BallControl>();
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
	Debug.Log("a");
	Task waitForConfig = Task.Run(WaitForConfig);
	while (!waitForConfig.IsCompleted) {
	    yield return null;
	}

	SetConfig();
	lightControl.SetConfig();
	ballControl.SetConfig();
	Debug.Log("b");
	
	while (true) {
	    Task waitForStart = Task.Run(WaitForStart);
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

	float x_rand, y_rand;
	do {
	x_rand = Random.Range(xLowerBound, xUpperBound);
	y_rand = Random.Range(yLowerBound, yUpperBound);
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
	ballControl.Reset();
	lastTrialSucceeded = false;
	float time = 0;

	targetLight.SetActive(true);
	target.GetComponent<MeshRenderer>().material = targetL;
	OnCursor();
	ballControl.isTrialRunning = true;

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

    void WaitForConfig() {
    }

    void WaitForStart() {
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

