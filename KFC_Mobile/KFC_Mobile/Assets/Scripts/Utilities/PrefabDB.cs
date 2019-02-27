using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//basically a class to hold and reference any prefab super easily
//it's an easy to use pattern

[CreateAssetMenu(menuName = "Prefab DB")]
public class PrefabDB : ScriptableObject
{
    [SerializeField]
    private GameObject genericAudioSource;
    public GameObject GenericAudioSource { get { return genericAudioSource; } }

    [SerializeField]
    private GameObject enemy;
    public GameObject Enemy { get { return enemy; } }
}
