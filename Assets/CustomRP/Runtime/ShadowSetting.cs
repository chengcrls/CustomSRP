using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class ShadowSetting
{
    [Min(0f)] public float maxDistance = 100f;

    public enum MapSize
    {
        _256= 256,_512= 512,_1024= 1024,
        _2048= 2048,_4096= 4096,_8192 = 8192
    }

    [System.Serializable]
    public struct Directinoal
    {
        public MapSize atlasSize;
    }

    public Directinoal directinoal = new Directinoal
    {
        atlasSize = MapSize._1024,
    };
}
