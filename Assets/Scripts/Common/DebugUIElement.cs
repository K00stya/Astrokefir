using TMPro;
using UnityEngine;

public class DebugUIElement : MonoBehaviour
{
    public TextMeshProUGUI Lable;
    public TextMeshProUGUI Data;

    public void OnDataChange(string data)
    {
        Data.text = data;
    }
}
