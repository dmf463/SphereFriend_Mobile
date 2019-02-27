using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakableWall : Hittable
{

    protected override void HitByArmor()
    {
        Camera.main.gameObject.GetComponent<ScreenShake>().TriggerShake();
        Destroy(gameObject);
    }
}
