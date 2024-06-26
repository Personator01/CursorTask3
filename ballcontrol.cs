using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballcontrol : MonoBehaviour
{
    public int rollingAverageAmount;

    public double signalOffset = 0;
    public double accelerationScale = 1;

    public double maxVelocity = 1;

    public double coefficientOfResitution = 0.6;

    public double xUpperBound = 7;
    public double xLowerBound = -7;
    public double yUpperBound = 4.5;
    public double yLowerBound = -4.5;


    bool isTrialRunning = false;
    double[] signalsX = new double[rollingAverageAmount];
    double[] signalsY = new double[rollingAverageAmount];
    int signalIndex = 0;

    Vector3 acceleration;
    Vector3 velocity;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
	if (isTrialRunning) {
	    double signalX = 0;
	    double signalY = 0;
	    signalsX[signalIndex] = signalX + signalOffset;
	    signalsY[signalIndex] = signalY + signalOffset;
	    
	    signalIndex = signalIndex + 1 >= rollingAverageAmount ? 0 : signalIndex + 1;

	    acceleration.x = signalsX.Sum() * accelerationScale;
	    acceleration.y = signalsY.Sum() * accelerationScale;
	    Move();
	}
        
    }

    void Move() {
	velocity = Vector3.ClampMagnitude(velocity + acceleration * Time.fixedDeltaTime, maxVelocity);
	this.transform.positon = this.transform.position + Vector3.ClampMagnitude(VelocityBounced(velocity), maxVelocity) * Time.fixedDeltaTime;
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

    void SetConfig() {
    }
}
