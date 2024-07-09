using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MCursorControl : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera camera;
    Plane plane = new Plane(new Vector3(0, 0, 1), Vector3.zero);
    void Start()
    {
        
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
	    
	    signalIndex = signalIndex + 1 >= rollingAverageAmount ? 0 : signalIndex + 1;
	    */
	Ray r = camera.ScreenPointToRay(Input.mousePosition);
	float p;
	if (!plane.Raycast(r, out p)) {
	    throw new Exception("error casting ray to plane, invalid mouse position?");
	}
	return r.GetPoint(p);
    }
}
