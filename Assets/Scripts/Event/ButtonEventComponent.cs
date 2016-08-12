using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtonEventComponent : MonoBehaviour
{
    private Dictionary<string, object> userdata = new Dictionary<string, object>();

    void OnClick()
    {
        NotificationCenter center = NotificationCenter.Instance;
        center.Post(gameObject.name, gameObject, userdata);
    }

    public void AddUserdata(string key, object value)
    {
        if (userdata.ContainsKey(key))
            userdata.Remove(key);

        userdata.Add(key, value);
    }
}