using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightattach : MonoBehaviour
{
    // Start is called before the first frame update
    GameObject ball;
    void Start()
    {

        ball = GameObject.Find("Sphere");
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = ball.transform.position;
    }
}
