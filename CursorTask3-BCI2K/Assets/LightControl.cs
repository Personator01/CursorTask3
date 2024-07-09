using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightControl : MonoBehaviour
{
    GameObject bulb1;
    GameObject bulbLight1;
    GameObject bulb2;
    GameObject bulbLight2;
    GameObject bulb3;
    GameObject bulbLight3;
    GameObject bulb4;
    GameObject bulbLight4;

    Light spot1;
    Light spot2;

    Color spotColor;


    public Material lightOffN;
    public Material lightOnN;
    public Material lightOffG;
    public Material lightOnG;

    void Awake() {
	FindObjs();
	ResetCountdown();
    }
    // Start is called before the first frame update
    void Start()
    {
	DeactivateSpotlights();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetConfig() {
    }

    void FindObjs() {
	bulb1 = GameObject.Find("lightsph1");
	bulbLight1 = GameObject.Find("bulb1");
	bulb2 = GameObject.Find("lightsph2");
	bulbLight2 = GameObject.Find("bulb2");
	bulb3 = GameObject.Find("lightsph3");
	bulbLight3 = GameObject.Find("bulb3");
	bulb4 = GameObject.Find("lightsph4");
	bulbLight4 = GameObject.Find("bulb4");

	spot1 = GameObject.Find("spot1").GetComponent<Light>();
	spot2 = GameObject.Find("spot2").GetComponent<Light>();
	spotColor = spot1.color;
    }

    public IEnumerator Countdown() {
	bulb1.GetComponent<MeshRenderer>().material = lightOnN;
	bulbLight1.SetActive(true);
	yield return new WaitForSeconds(0.75f);
	bulb2.GetComponent<MeshRenderer>().material = lightOnN;
	bulbLight2.SetActive(true);
	yield return new WaitForSeconds(0.75f);
	bulb3.GetComponent<MeshRenderer>().material = lightOnN;
	bulbLight3.SetActive(true);
	yield return new WaitForSeconds(0.75f);
	bulb4.GetComponent<MeshRenderer>().material = lightOnG;
	bulbLight4.SetActive(true);
    }

    public void ResetCountdown() {
	bulb1.GetComponent<MeshRenderer>().material = lightOffN;
	bulbLight1.SetActive(false);
	bulb2.GetComponent<MeshRenderer>().material = lightOffN;
	bulbLight2.SetActive(false);
	bulb3.GetComponent<MeshRenderer>().material = lightOffN;
	bulbLight3.SetActive(false);
	bulb4.GetComponent<MeshRenderer>().material = lightOffG;
	bulbLight4.SetActive(false);
    }

    public IEnumerator FlashGreen() {
	while (true) {
	    spot1.color = Color.black;
	    spot2.color = Color.black;
	    yield return new WaitForSeconds(0.3f);
	    spot1.color = Color.green;
	    spot2.color = Color.green;
	    yield return new WaitForSeconds(0.3f);
	}
    }
    public IEnumerator FlashRed() {
	while (true) {
	    spot1.color = Color.black;
	    spot2.color = Color.black;
	    yield return new WaitForSeconds(0.3f);
	    spot1.color = Color.red;
	    spot2.color = Color.red;
	    yield return new WaitForSeconds(0.3f);
	}
    }

    public void ActivateSpotlights() {
	spot1.color = spotColor;
	spot2.color = spotColor;
	
    }
    public void DeactivateSpotlights() {
	spot1.color = Color.black;
	spot2.color = Color.black;
    }
}
