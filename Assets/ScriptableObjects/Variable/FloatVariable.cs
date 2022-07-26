using UnityEngine;

[CreateAssetMenu(fileName = "FloatVariable", menuName = "ScriptableObjects/Variable/FloatVariable", order = 2)]
public class FloatVariable : ScriptableObject
{
#if UNITY_EDITOR
    [Multiline]
    public string DeveloperDescription = "";
#endif
    private float _value = 0;
    public float Value{
        get{
            return _value;
        }
    }

    public void SetValue(float value)
    {
        _value = value;
    }

    // overload
    public void SetValue(FloatVariable value)
    {
        _value = value._value;
    }

    public void ApplyChange(float amount)
    {
        _value += amount;
    }

    public void ApplyChange(FloatVariable
 amount)
    {
        _value += amount._value;
    }
}
