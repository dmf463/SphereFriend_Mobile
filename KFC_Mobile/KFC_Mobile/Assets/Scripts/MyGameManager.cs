using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MyGameManager : MonoBehaviour
{
    public Text text;
    public Text levelCompletionText;
    private void Awake()
    {
        Services.GM = this;
        Services.Touch = GameObject.Find("TouchManager").GetComponent<TouchManager>();
        Services.LevelManager = GameObject.Find("LevelManager").GetComponent<MyLevelManager>();
        Services.MyCritter = GameObject.Find("PC").GetComponent<MyCritter>();
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

    public void LevelComplete(string outcome)
    {
        levelCompletionText.text = outcome;
    }
}
