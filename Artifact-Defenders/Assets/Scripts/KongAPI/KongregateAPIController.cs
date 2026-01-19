using UnityEngine;
using System.Runtime.InteropServices;
using System;

[Obsolete("Deprecated, not releasing to Kongregate anymore", false)]
public class KongregateAPIController : MonoBehaviour
{
    private static KongregateAPIController instance;

#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void KAPIInit();
#endif

    void Start()
    {
        if (instance == null) instance = this;
        else if (instance != this) { Destroy(gameObject); return; }

        DontDestroyOnLoad(gameObject);
        gameObject.name = "KongregateAPI";

#if UNITY_WEBGL && !UNITY_EDITOR
        KAPIInit(); // Chỉ gọi trên WebGL
#else
        Debug.Log("Kongregate API không khả dụng trên nền tảng này");
#endif
    }
}
