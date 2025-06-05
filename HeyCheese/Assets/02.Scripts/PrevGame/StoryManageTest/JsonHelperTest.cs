using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class JsonHelperTest : MonoBehaviour
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }
    
    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
