using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyGameManager : MonoBehaviour
{
    public Text text;
    private void Awake()
    {
        Services.GM = this;
        Services.PlayerAI = GameObject.Find("PC").GetComponent<PlayerAI>();
        Services.PrefabDB = Resources.Load<PrefabDB>("Prefabs/PrefabDB");
        Services.Touch = GameObject.Find("TouchManager").GetComponent<TouchManager>();
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
