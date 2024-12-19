using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils : object
{
    public static GameObject FindObject(this GameObject parent, string name)
    {
        Transform[] trs = parent.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in trs)
        {
            // Ensure we are not considering the parent itself
            if (t != parent.transform && t.name == name)
            {
                return t.gameObject;
            }
        }
        return null;
    }

    public static GameObject FindObject(this GameObject parent, string name, bool startsWith)
    {
        Transform[] trs = parent.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in trs)
        {
            // Ensure we are not considering the parent itself
            if (t != parent.transform &&
                (t.name == name || (startsWith && t.name.StartsWith(name))))
            {
                return t.gameObject;
            }
        }
        return null;
    }
}
