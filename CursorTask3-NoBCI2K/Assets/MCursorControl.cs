using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MCursorControl : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera camera;
    Plane plane = new Plane(new Vector3(0, 0, 1), Vector3.zero);

    public bool isTrialRunning;

    int rollingMaximumAmount = 10;
    int signalIndex = 0;

    int[] signalsX;
    int[] signalsY;

    void Awake() {
    }

    void Start()
    {
        
    }

    public void SetConfig() {
	signalsX = new int[rollingMaximumAmount];
	signalsY = new int[rollingMaximumAmount];
    }

    // Update is called once per frame
    void Update()
    {
	this.transform.position = GetPos();
    }


    Vector3 GetPos() {
	/**
	    double signalX = 0;
	    double signalY = 0;
	    signalsX[signalIndex] = signalX + signalOffset;
	    signalsY[signalIndex] = signalY + signalOffset;
	    
	    signalIndex = signalIndex + 1 >= rollingMaximumAmount ? 0 : signalIndex + 1;
	    int max_x = signalsX.Max();
	    int max_y = signalsY.Max();
	    Ray r = camera.ScreenPointToRay(max_x * Screen.width, max_y * Screen.height);
	    */
	Ray r = camera.ScreenPointToRay(Input.mousePosition);
	float p;
	if (!plane.Raycast(r, out p)) {
	    throw new Exception("error casting ray to plane, invalid mouse position?");
	}
	return r.GetPoint(p);
    }
}
