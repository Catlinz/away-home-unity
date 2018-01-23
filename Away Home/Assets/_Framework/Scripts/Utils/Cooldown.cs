using UnityEngine;

/// <summary>
/// A simple class to track the status of a Cooldown for something.
/// </summary>
public class Cooldown {
    public float start;
    public float end;

    public bool Finished {
        get { return _current >= end; }
    }

    public float Length {
        get { return end - start; }
    }

    public float Percent {
        get { return ((end - start) / (_current - start)) * 100; }
    }

    private float _current;

    /// <summary>
    /// Create a new Cooldown.
    /// </summary>
    /// <param name="start">The time in seconds when the cooldown starts.</param>
    /// <param name="length">The number of seconds the cooldown is.</param>
    public Cooldown(float start, float length) {
        this.start = start;
        end = start + length;
        _current = start;
    }

    /// <summary>
    /// Advance the cooldown and return whether or not it is finished.
    /// </summary>
    public bool Tick(float curTime) {
        _current = curTime;
        return Finished;
    }

    /// <summary>
    /// Return a WaitForSeconds object that will either wait for the provided number of
    /// seconds, or the time remaining, whichever is the smaller amount of time.
    /// </summary>
    public WaitForSeconds WaitFor(float seconds) {
        return new WaitForSeconds(Mathf.Min(seconds, (end - _current)));
    }

}
