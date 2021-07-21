using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextPort : MonoBehaviour
{
    static List<TextPort> portList = new List<TextPort>();
    [SerializeField]public int id;
    TMP_Text text;

    void Start()
    {
        portList.Add(this);
        text = GetComponent<TMP_Text>();
    }

    public static void ShowString(string message,int id = 0)
    {
        var port = portList.Find(x => x.id == id);
        port.text.text = message;
    }

    void OnDestroy()
    {
        portList.Remove(this);
    }
}
