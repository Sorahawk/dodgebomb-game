using UnityEngine;

[CreateAssetMenu(fileName = "PlayerVariable", menuName = "ScriptableObjects/Variable/PlayerVariable", order = 2)]
public class PlayerVariable : ScriptableObject
{
#if UNITY_EDITOR
    [Multiline]
    public string DeveloperDescription = "";
#endif
    private float _moveSpeed = 0;
    private int _score = 0;
    private int _powerup = 0;

    // MOVE SPEED
    public float MoveSpeed{
        get{
            return _moveSpeed;
        }
    }

    public void SetMoveSpeed(float value)
    {
        _moveSpeed = value;
    }

    public void SetMoveSpeed(PlayerVariable value)
    {
        _moveSpeed = value._moveSpeed;
    }

    public void ApplyMoveSpeedChange(float amount)
    {
        _moveSpeed += amount;
    }

    public void ApplyMoveSpeedChange(PlayerVariable amount)
    {
        _moveSpeed += amount._moveSpeed;
    }

    // SCORE
    public int Score{
        get{
            return _score;
        }
    }

    public void SetScore(int value)
    {
        _score = value;
    }

    public void SetScore(PlayerVariable value)
    {
        _score = value._score;
    }

    public void ApplyScoreChange(int amount)
    {
        _score += amount;
    }

    public void ApplyScoreChange(PlayerVariable amount)
    {
        _score += amount._score;
    }

    // POWERUP
    public int Powerup{
        get{
            return _powerup;
        }
    }

    public void SetPowerup(int value)
    {
        _powerup = value;
    }

    public void SetPowerup(PlayerVariable value)
    {
        _score = value._score;
    }
}
