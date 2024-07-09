using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class BallControl : MonoBehaviour
{

    private GameControl gameControl;

    public int rollingAverageAmount;

    public double signalOffset = 0;
    public float accelerationScale = 1;

    public float coefficientOfRestitution = 0.6f;
    public float coefficientOfDrag = 0.2f;
    public float coefficientOfGravity = 1;
    public double gravityCurveCoeff = 2;

    public double xUpperBound = 7;
    public double xLowerBound = -7;
    public double yUpperBound = 4.5;
    public double yLowerBound = -4.5;

    public float gravityDeadzone = 1;

    double[] signalsX;
    double[] signalsY;

    [System.NonSerialized]
    public bool isTrialRunning = false;
    int signalIndex = 0;

    Vector3 acceleration = Vector3.zero;
    Vector3 velocity = Vector3.zero;


    void Awake() {
	gameControl = GameObject.Find("Control").GetComponent<GameControl>();
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

	    acceleration = GetMousePos() * accelerationScale;

	    Move();
	} else {
	    Vector3 difference = Vector3.zero - this.transform.position;
	    if (difference.magnitude < gravityDeadzone) {
		acceleration = difference.normalized * (float)(coefficientOfGravity * difference.magnitude);
	    } else {
		acceleration = difference.normalized * (float)(coefficientOfGravity / Math.Pow(difference.magnitude, gravityCurveCoeff));
	    }
	    Move();
	}
    }

    public void Reset() {
	double[] signalsX = new double[rollingAverageAmount];
	double[] signalsY = new double[rollingAverageAmount];
    }


    Vector3 GetMousePos() {
	/**
	    double signalX = 0;
	    double signalY = 0;
	    signalsX[signalIndex] = signalX + signalOffset;
	    signalsY[signalIndex] = signalY + signalOffset;
	    
	    signalIndex = signalIndex + 1 >= rollingAverageAmount ? 0 : signalIndex + 1;
	    */
	Vector3 mpos = Input.mousePosition;
	float x_adj = (mpos.x / Screen.width) - 0.5f;
	float y_adj = (mpos.y / Screen.height) - 0.5f;
	return new Vector3(x_adj, y_adj, 0);
    }


    void Move() {
	Vector3 drag_v = Vector3.Normalize(acceleration) * ((float) Math.Pow(velocity.magnitude, 2) * coefficientOfDrag);
	drag_v = Vector3.zero;
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
	return Vector3.Reflect(invelocity, bounceNormal) * coefficientOfRestitution;
    }

    public void SetConfig() {
    }
}
