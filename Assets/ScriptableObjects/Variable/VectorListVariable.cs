using UnityEngine;

[CreateAssetMenu(fileName = "VectorListVariable", menuName = "ScriptableObjects/Variable/VectorListVariable", order = 2)]
public class VectorListVariable : ScriptableObject
{
#if UNITY_EDITOR
    [Multiline]
    public string DeveloperDescription = "";
#endif
    private Vector3[] _vectorList = new Vector3[6];
    private int currentIndex = 0;

    // MOVE SPEED
    public Vector3[] VectorList{
        get{
            return _vectorList;
        }
    }

    public void addVector(Vector3 value)
    {
        if(currentIndex<6){
            _vectorList[currentIndex] = value;
            currentIndex+=1;
        }
    }

    public void resetVectorList()
    {
        _vectorList = new Vector3[6];
        currentIndex = 0;
    }
}
