using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Sirenix.OdinInspector;
using System;

public class UnityQASM :MonoBehaviour, ISubject
{
    List<IObserver> observerList = new List<IObserver>();
    [SerializeField]public int times;
    [SerializeField]public bool repeat = true;
    [SerializeField]public int digits = 4;
    [SerializeField]public int cats = 1000;
    string timesStr;
    string repeatStr;
    string digitsStr;
    string catsStr;

    //保持している最後の結果
    string lastRawResult = "";
    //変換された最後の結果
    string convertedResult = "";
    Dictionary<string, int> _lastResult = new Dictionary<string, int>();
    public Dictionary<string, int> lastResult
    {
        get{
            if (convertedResult != lastRawResult)
            {
                convertedResult = lastRawResult;
                _lastResult = convertResult();
            }

            return _lastResult;
        }
    }


    void Start()
    { StartCoroutine(ResetCat(digits)); }

    [Button]
    public void StepOnce()
    {
        timesStr = times.ToString();
        repeatStr = repeat.ToString();
        digitsStr = digits.ToString();
        catsStr = cats.ToString();
        StartCoroutine(SendRequest());
    }


    IEnumerator SendRequest()
    {
        Debug.Log("Cat walk " + times + "times");
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("walk_times", timesStr));
        formData.Add(new MultipartFormDataSection("if_repeat", repeatStr));
        formData.Add(new MultipartFormDataSection("digit_num", digitsStr));
        formData.Add(new MultipartFormDataSection("cat_num", catsStr));

        UnityWebRequest www = UnityWebRequest.Post("http://127.0.0.1:8001/api/run/walk", formData);
        yield return www.SendWebRequest();

        lastRawResult = www.downloadHandler.text;
        Debug.Log("Response:" + lastRawResult);

        Debug.Log(lastResult);

        Notice();
    }

    IEnumerator ResetCat(int n = 5)
    {
        if (n == 0)
        {
            //0はエラる
            n = 5;
        }
        List<IMultipartFormSection> formData = new List<IMultipartFormSection>();
        formData.Add(new MultipartFormDataSection("digit_num", n.ToString()));
        UnityWebRequest www = UnityWebRequest.Post("http://127.0.0.1:8001/api/run/reset", formData);

        yield return www.SendWebRequest();

        Debug.Log("cat resetted with digit" + n);
    }

    Dictionary<string, int> convertResult()
    {
        var result = new Dictionary<string, int>();
        var tmp = lastRawResult.Replace("\"", "");
        tmp = tmp.Substring(tmp.IndexOf(" ") + 1); //最初の空間まで削除
        while (tmp != "")
        {
            var section = "";

            //まだコンマがあれば
            if (tmp.IndexOf(",") != -1)
            {
                //コンマの直前まで切り取って
                section = tmp.Substring(0, tmp.IndexOf(","));

                //次の空間まで削除
                tmp = tmp.Substring(tmp.IndexOf(" ") + 1);
            }
            //なければ
            else
            {
                //残るは}のみ。
                section = tmp.Replace("}", "");
                tmp = "";
            }

            //謎の空欄の後ろからポジション
            var position = section.Substring(0, section.IndexOf(":"));
            //\:より後ろが数
            var count = section.Substring(section.IndexOf(":") + 1);

            result[position] = int.Parse(count);
        }

        return result;
    }

    public void RegisterObserver(IObserver observer)
    {
        if(!observerList.Contains(observer))
        {
            observerList.Add(observer);
        }
    }

    public void EraseObserver(IObserver observer)
    {
        if(observerList.Contains(observer))
        {
            observerList.Remove(observer);
        }
    }

    void Notice()
    {
        foreach(var obs in observerList)
        {
            obs.OnNotice();
        }
    }

}
