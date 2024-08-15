using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class MCursorControl : MonoBehaviour
{
    // Start is called before the first frame update
    public Camera camera;
    Plane plane = new Plane(new Vector3(0, 0, 1), Vector3.zero);

    public bool isTrialRunning;

    int rollingMaximumAmount = 10;
    int signalIndex = 0;

    double[] signalsX;
    double[] signalsY;

    UnityBCI2000 bci;

    void Awake() {
	bci = GameObject.Find("BCI2000").GetComponent<UnityBCI2000>();

	bci.OnIdle(remote => {
		});
    }

    void Start()
    {
        
    }

    public void SetConfig() {
	signalsX = new double[rollingMaximumAmount];
	signalsY = new double[rollingMaximumAmount];
    }

    // Update is called once per frame
    void Update()
    {
	if (isTrialRunning) {
	    this.transform.position = GetPos();
	}
    }


    Vector3 GetPos() {
	double signalX = bci.Control.GetSignal(1, bci.CurrentSampleOffset());
	double signalY = bci.Control.GetSignal(2, bci.CurrentSampleOffset());
	signalsX[signalIndex] = signalX;
	signalsY[signalIndex] = signalY;
	
	signalIndex = signalIndex + 1 >= rollingMaximumAmount ? 0 : signalIndex + 1;
	double max_x = signalsX.Max();
	double max_y = signalsY.Max();
	Ray r = camera.ScreenPointToRay(new Vector3((float) max_x * Screen.width, (float) max_y * Screen.height, 0));
	float p;
	if (!plane.Raycast(r, out p)) {
	    throw new Exception("error casting ray to plane, invalid mouse position?");
	}
	return r.GetPoint(p);
    }
}
