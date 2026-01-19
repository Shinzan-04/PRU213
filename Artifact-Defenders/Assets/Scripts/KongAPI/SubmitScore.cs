using UnityEngine;
using System.Runtime.InteropServices;
using System;

[Obsolete("Deprecated, not releasing to Kongregate anymore", false)]
public class SubmitScore : MonoBehaviour
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void SubmitKongStat(string StatName, int StatValue);
#endif

    public void Submit(string statName, int statValue)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        SubmitKongStat(statName, statValue); // Chỉ gọi trên WebGL
#else
        Debug.Log($"SubmitScore bị bỏ qua trên nền tảng không phải WebGL: {statName} = {statValue}");
#endif
    }
}
