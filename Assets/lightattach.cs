using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lightattach : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject obj;
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position = obj.transform.position;
    }
}
