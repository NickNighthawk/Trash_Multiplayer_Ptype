using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Scenes;

public class SubsceneReferences : MonoBehaviour
{
    public static SubsceneReferences instance { get; private set; }

    public SubScene[] subScenes;

    public float loadDistance = 20f;

    private void Awake()
    {
        instance = this;
    }
}
