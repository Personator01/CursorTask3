using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class BallControl : MonoBehaviour
{

    private GameControl gameControl;

    GameObject mCursor;

    public int rollingAverageAmount;

    public double signalOffset = 0;
    public float accelerationScale = 1;

    public float coefficientOfRestitution = 0.6f;
    public float coefficientOfDrag = 0.2f;
    public float coefficientOfGravity = 1;
    public double gravityCurveCoeff = 2;
    public float attractionCurveCoefficient = 1;

    public double xUpperBound = 7;
    public double xLowerBound = -7;
    public double yUpperBound = 4.5;
    public double yLowerBound = -4.5;

    public float gravityDeadzone = 1;
    public float slowdownWhenInDeadzone = 0.9f;

    double[] signalsX;
    double[] signalsY;

    [System.NonSerialized]
    public bool isTrialRunning = false;
    int signalIndex = 0;

    Vector3 acceleration = Vector3.zero;
    Vector3 velocity = Vector3.zero;


    void Awake() {
	gameControl = GameObject.Find("Control").GetComponent<GameControl>();
	mCursor = GameObject.Find("MCursor");
    }
    // Start is called before the first frame update
    void Start()
    {
	Reset();
    }

    // Update is called once per frame
    void Update()
    {

	if (isTrialRunning) {
	    Vector3 difference = mCursor.transform.position - this.transform.position;
	    acceleration = difference.normalized * (float) Math.Pow(difference.magnitude, attractionCurveCoefficient) * accelerationScale;
	    Move();
	} else {
	    Vector3 difference = Vector3.zero - this.transform.position;
	    if (difference.magnitude < gravityDeadzone) {
		acceleration = (difference.normalized * (float)(coefficientOfGravity * difference.magnitude));
		velocity = velocity * slowdownWhenInDeadzone;
	    } else {
		acceleration = difference.normalized * (float)(coefficientOfGravity / Math.Pow(difference.magnitude, gravityCurveCoeff));
	    }
	    Move();
	}

	//For if the cursor escapes the box (corners) to keep it from flying off into infinity
	if (this.transform.position.magnitude > 20) {
	    this.transform.position = Vector3.zero;
	    velocity = Vector3.zero;
	    acceleration = Vector3.zero;
	}
    }

    public void Reset() {
	double[] signalsX = new double[rollingAverageAmount];
	double[] signalsY = new double[rollingAverageAmount];
    }


    void Move() {
	Vector3 drag_v = Vector3.Normalize(velocity) * ((float) Math.Pow(velocity.magnitude, 2) * coefficientOfDrag);
	acceleration = acceleration - drag_v;
	velocity = velocity + acceleration * Time.fixedDeltaTime;
	velocity = VelocityBounced(velocity);
	this.transform.position = this.transform.position + velocity * Time.fixedDeltaTime;
    }

    Vector3 VelocityBounced(Vector3 invelocity) {
	Vector3 nextPos = this.transform.position + invelocity * Time.fixedDeltaTime;
	Vector3 bounceNormal = Vector3.zero;
	if (nextPos.x > xUpperBound) {
	    if (nextPos.y > yUpperBound || nextPos.y < yLowerBound) { //corner bounce
		bounceNormal = invelocity * -1;
	    } else { //not corner bounce
		bounceNormal = Vector3.left;
	    }
	} else if (nextPos.x < xLowerBound) {
	    if (nextPos.y > yUpperBound || nextPos.y < yLowerBound) { //corner bounce
		bounceNormal = invelocity * -1;
	    } else { //not corner bounce
		bounceNormal = Vector3.right;
	    }
	} else if (nextPos.y > yUpperBound) {
	    bounceNormal = Vector3.down;
	} else if (nextPos.y < yLowerBound) {
	    bounceNormal = Vector3.up;
	}
	return Vector3.Reflect(invelocity, bounceNormal) * 
	    (bounceNormal == Vector3.zero ? 1 : coefficientOfRestitution);
    }

    public void SetConfig() {
    }
}
