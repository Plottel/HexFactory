using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMenu : MonoBehaviour
{
    public T Find<T>(string name) where T : MonoBehaviour
    {
        var options = GetComponentsInChildren<T>();
        foreach (var option in options)
        {
            if (option.name == name)
                return option;
        }

        Debug.LogError("Failed to find " + typeof(T).Name + " called " + name);
        return null;
    }
}
