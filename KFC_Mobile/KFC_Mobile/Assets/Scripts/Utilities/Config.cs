
using UnityEngine;

[CreateAssetMenu (menuName = "Config")]
public class Config : ScriptableObject
{
    [SerializeField] private float _movementSpeed;
    public float MovementSpeed { get { return _movementSpeed; } }

    [SerializeField] private float _jumpSpeed;
    public float JumpSpeed { get { return _movementSpeed; } }

    [SerializeField] private float _wanderSpeed;
    public float WanderSpeed { get { return _wanderSpeed;} }

    [SerializeField] private float _fingerLevel;
    public float FingerLevel { get { return _fingerLevel; } }



}
