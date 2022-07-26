using UnityEngine;

[CreateAssetMenu(fileName =  "GameConstants", menuName =  "ScriptableObjects/GameConstants", order =  1)]
public  class GameConstants : ScriptableObject
{
    // for player controller
    public float playerMoveSpeed = 5;
    public float dashDistance = 70;
    public int bombThrowForce = 30;

    // timer
    public float roundDuration = 300;
}