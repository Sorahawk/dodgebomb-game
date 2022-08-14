using UnityEngine;

[CreateAssetMenu(fileName = "BoolVariable", menuName = "ScriptableObjects/Variable/BoolVariable", order = 2)]
public class BoolVariable : ScriptableObject
{
#if UNITY_EDITOR
    [Multiline]
    public string DeveloperDescription = "";
#endif
    private bool _value = false;
    public bool Value{
        get{
            return _value;
        }
    }

    public void SetValue(bool value)
    {
        _value = value;
    }

    // overload
    public void SetValue(BoolVariable value)
    {
        _value = value._value;
    }

}