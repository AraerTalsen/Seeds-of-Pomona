using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Wallet : MonoBehaviour
{
    public TMP_Text displayBalance;
    private int currentBalance = 0;

    public int CurrentBalance
    {
        get
        {
            return currentBalance;
        }
        set 
        {
            currentBalance = value;
            displayBalance.text = $"${currentBalance}";
        }
    }
    
    public void IncrementBalance(int amount)
    {
        currentBalance += amount;
        displayBalance.text = $"${currentBalance}";
    }

    public void DecrementBalance(int amount)
    {
        currentBalance -= amount;
        displayBalance.text = $"${currentBalance}";
    }
}

/*
C# version of a closure. Consider use case
https://learn.microsoft.com/en-us/dotnet/api/system.action-1?view=net-9.0
public class CounterFactory
{
    public static (Action increment, Action decrement, Func<int> getValue) CreateCounter(int initialValue = 0)
    {
        int value = initialValue;

        Action increment = () => value++;
        Action decrement = () => value--;
        Func<int> getValue = () => value;

        return (increment, decrement, getValue);
    }
}

// Usage
public class Example
{
    public static void Main()
    {
        var counter = CounterFactory.CreateCounter();

        counter.increment();
        counter.increment();
        Console.WriteLine(counter.getValue()); // 2
        counter.decrement();
        Console.WriteLine(counter.getValue()); // 1
    }
}
*/
