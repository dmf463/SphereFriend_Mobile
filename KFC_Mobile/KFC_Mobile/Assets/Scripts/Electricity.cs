using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Electricity : Hittable
{
    public GameObject shock;
    public float liveTime;
    public float deadTime;
    public float elapsedTime;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (shock.activeSelf)
        {
            if (elapsedTime > liveTime)
            {
                shock.SetActive(false);
                elapsedTime = 0;
            }
        }
        else if (!shock.activeSelf)
        {
            if(elapsedTime > deadTime)
            {
                shock.SetActive(true);
                elapsedTime = 0;
            }
        }
    }
}
