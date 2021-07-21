using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;
using Sirenix.OdinInspector;

public class StepManager : MonoBehaviour, IObserver
{
    UnityQASM stepper;
    // Start is called before the first frame update
    [SerializeField] float stepInterval = 2f;
    Dictionary<int, int> lastResult = new Dictionary<int, int>();
    bool active = false;
    int nowStep = 0;
    StringBuilder resultMSG = new StringBuilder();

    void Start()
    {
        stepper = GetComponent<UnityQASM>();
        stepper.RegisterObserver(this);
    }

    [Button]
    public void StartStep()
    {
        if (!active)
        {
            active = true;
            stepper.StepOnce();
            return;
        }

        Debug.LogWarning("aready started!");
    }

    //activeなら連鎖的に次を呼ぶ
    public void OnNotice()
    {
        if (active)
        {
            lastResult.Clear();
            foreach (var placeCount in stepper.lastResult)
            {       
                lastResult[Convert.ToInt32(placeCount.Key, 2)] = placeCount.Value;
            }

            UpdateData();
            stepper.StepOnce();
        }
    }

    void UpdateData()
    {
        BuildResult();
        TextPort.ShowString(resultMSG.ToString());
    }

    void BuildResult()
    {
        resultMSG.Clear();
        foreach (var result in lastResult)
        {
            resultMSG.AppendFormat("{0,5} : {1,5} cats\n", result.Key, result.Value);
        }
    }
}
