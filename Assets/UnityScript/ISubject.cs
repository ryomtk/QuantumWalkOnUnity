using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISubject
{
    void RegisterObserver(IObserver obj);
    void EraseObserver(IObserver obj);
}
