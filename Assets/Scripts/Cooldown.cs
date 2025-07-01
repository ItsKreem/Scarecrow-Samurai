using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Cooldown
{
    public enum Progress
    {
        Ready,
        Started,
        InProgress,
        Finished
    }

    public Progress CurrentProgress = Progress.Ready;
    public float TimeLeft
    {
        get
        {
            return _currentDuration;
        }
    }

    public bool IsOnCooldown
    {
        get
        {
            return _isOnCooldown;
        }
    }
    public float Duration = 1.0f;

    private float _currentDuration = 0f;
    private bool _isOnCooldown = false;

    private Coroutine _coroutine;

    public void StartCooldown()
    {
        if (CurrentProgress is Progress.Started or Progress.InProgress)
            return;

        _coroutine = CoroutineHost.Instance.StartCoroutine(DoCooldown());
    }

    public void StopCooldown()
    {
        if (_coroutine != null)
            CoroutineHost.Instance.StopCoroutine(_coroutine);

        _currentDuration = 0f;
        _isOnCooldown = false;
        CurrentProgress = Progress.Ready;
    }

    IEnumerator DoCooldown()
    {
        CurrentProgress = Progress.Started;
        _currentDuration = Duration;
        _isOnCooldown = true;

        while (_currentDuration > 0)
        {
            _currentDuration -= Time.deltaTime;
            CurrentProgress = Progress.InProgress;

            yield return null;
        }

        _currentDuration = 0f;
        _isOnCooldown = false;

        CurrentProgress = Progress.Finished;
    }
}
