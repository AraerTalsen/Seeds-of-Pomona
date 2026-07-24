using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class RuntimeAlarmEvent : IRuntimeEvent
{
    private float _lifespan;
    private Action _tickProcess;
    private List<Action> _callbacks;
    
    public float Elapsed { get; set; }
    public bool IsFinished
    {
        get
        {
            bool isFinished = Elapsed >= _lifespan;
            if(isFinished)
                _callbacks.ForEach( c => c.Invoke());
            return isFinished;
        }
    }
    public UnityEngine.Object Owner { get; set; }

    private RuntimeAlarmEvent(float lifespan, Action tickProcess = null, List<Action> callbacks = null)
    {
        _lifespan = lifespan;
        _tickProcess = tickProcess;
        _callbacks = callbacks;
    }

    public async Task Activate()
    {
        if(_tickProcess != null || (_callbacks != null && _callbacks.Count > 0)) EventRunner.Run(this);
        float elapsed = 0;
        while(elapsed < 0.1f)
        {
            elapsed += Time.deltaTime;
            await Task.Yield();
        }
    }

    public void Tick() => _tickProcess?.Invoke();

    public static RuntimeAlarmEvent Create(float lifespan, Action tickProcess = null, List<Action> callbacks = null)
    {
        RuntimeAlarmEvent alarm = new(lifespan, tickProcess, callbacks);
        
        return alarm;
    }
}
