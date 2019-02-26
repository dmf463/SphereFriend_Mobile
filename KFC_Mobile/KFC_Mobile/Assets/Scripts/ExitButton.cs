using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButton : Hittable
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.name == "PC")
        {
            if (Services.Touch.par == Services.LevelManager.targetPar)
            {
                Services.GM.levelCompletionText.text = "SUCCESS";
            }
            else Services.GM.levelCompletionText.text = "FAILURE";
        }
    }
}
