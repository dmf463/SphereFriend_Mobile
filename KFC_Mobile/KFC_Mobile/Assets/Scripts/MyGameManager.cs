using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGameManager : MonoBehaviour
{
    private void Awake()
    {
        Services.GM = this;
        Services.PlayerMovement = GameObject.Find("PC").GetComponent<PlayerMovement>();
        Services.PrefabDB = Resources.Load<PrefabDB>("Prefabs/PrefabDB");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
