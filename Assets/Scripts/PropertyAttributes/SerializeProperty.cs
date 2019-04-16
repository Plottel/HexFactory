using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// https://forum.unity.com/threads/serialize-c-properties-how-to-with-code.506027/

[System.AttributeUsage(System.AttributeTargets.Field)]
public class SerializeProperty : PropertyAttribute
{
    public string PropertyName { get; private set; }

    public SerializeProperty(string propertyName)
    {
        PropertyName = propertyName;
    }
}
