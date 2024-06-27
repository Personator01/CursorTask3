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

    public double preRunDuration = 1;
    public double preFeedbackDuration = 2.25;
    public double feedbackDuration = 1;
    public double postFeedbackDuration = 1;

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
	Debug.Log("a");
	lightControl.Countdown();
    }

    // Update is called once per frame
    void Update()
    {
	
    }

    void DisableTarget() {
	target.SetActive(false);
	targetLight.SetActive(false);
    }

    void offCursor() {
	cursor.GetComponent<MeshRenderer>().material = ballDark;
	cursorLight.SetActive(false);
    }

    void onCursor() {
	cursor.GetComponent<MeshRenderer>().material = ballLight;
	cursorLight.SetActive(true);
    }
}

