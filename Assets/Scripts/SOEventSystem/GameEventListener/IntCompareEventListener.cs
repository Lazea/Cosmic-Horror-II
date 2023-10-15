using SOGameEventSystem;
using SOGameEventSystem.Events;
using UnityEngine;
using UnityEngine.Events;

public enum IntCompareType
{
    Equal,
    Greater,
    Lesser
}

public class IntCompareEventListener : BaseGameEventListener<int, IntGameEvent, UnityEvent<int>>
{
    public int value;
    public IntCompareType compareType;

    public UnityEvent trueResponse;
    public UnityEvent falseResponse;

    public override void OnEventRaised(int item)
    {
        switch (compareType)
        {
            case IntCompareType.Greater:
                if (item > value)
                    if (trueResponse != null)
                        trueResponse.Invoke();
                else
                    if (falseResponse != null)
                        falseResponse.Invoke();
                break;
            case IntCompareType.Lesser:
                if (item < value)
                    if (trueResponse != null)
                        trueResponse.Invoke();
                else
                    if (falseResponse != null)
                        falseResponse.Invoke();
                break;
            default:
                if (item == value)
                    if (trueResponse != null)
                        trueResponse.Invoke();
                else
                    if (falseResponse != null)
                        falseResponse.Invoke();
                break;
        }
    }
}
