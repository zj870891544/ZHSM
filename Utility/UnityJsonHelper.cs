using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework;
using UnityEngine;


namespace ZHSM
{
    public class UnityJsonHelper : Utility.Json.IJsonHelper
    {
        public string ToJson(object obj)
        {
            return JsonUtility.ToJson(obj);
        }

        public T ToObject<T>(string json)
        {
            return JsonUtility.FromJson<T>(json);
        }

        public object ToObject(Type objectType, string json)
        {
            return JsonUtility.FromJson(json, objectType);
        }
    }
}