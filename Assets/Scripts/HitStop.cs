using System.Collections;
using UnityEngine;

public class HitStop : MonoBehaviour
{
    public float duration = 0f;

    private bool waiting;

    public void Freeze(float duration)
    {
        if (waiting) { return; }
        Time.timeScale = 0.0f;
        StartCoroutine(StartHitStop(duration));
    }

    IEnumerator StartHitStop(float duration)
    {
        waiting = true;
        yield return new WaitForSecondsRealtime(duration);
        Time.timeScale = 1.0f;
        waiting = false;
    }
}
