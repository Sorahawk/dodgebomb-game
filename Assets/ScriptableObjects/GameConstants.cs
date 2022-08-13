using UnityEngine;

[CreateAssetMenu(fileName =  "GameConstants", menuName =  "ScriptableObjects/GameConstants", order =  1)]
public  class GameConstants : ScriptableObject
{
    // for player controller
    public float playerMoveSpeed = 5;
    public float dashDistance = 70;
    public int bombThrowForce = 30;
    public int powerThrowForce = 50;

    // timer
    public float roundDuration = 300;
    public float startingCountdown = 3;
    public int numberOfNonPlayableScenes = 3;
    public float respawnTimer = 5f;
    
}