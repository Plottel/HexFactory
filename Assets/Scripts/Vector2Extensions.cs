using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector2Extensions
{
    /// <summary>
    /// Same as ToString(), but first converts the components to integers to remove decimal place.
    /// Use this only when you are confident your Vector components are both integers.
    /// </summary>
    public static string ToStringInt(this Vector2 v)
    {
        string x = ((int)v.x).ToString();
        string y = ((int)v.y).ToString();

        return string.Concat("(", x, ",", y, ")");
    }
}
