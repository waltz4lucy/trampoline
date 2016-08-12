using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class NotificationCenter {
    private static NotificationCenter instance;
    private Dictionary<string, List<NotificationObserver>> observersDictionary = new Dictionary<string, List<NotificationObserver>>();

    private NotificationCenter() {}

    public static NotificationCenter Instance {
        get {
            if (instance == null) {
                instance = new NotificationCenter();
            }
            return instance;
        }
    }

    public void AddObserver(string key, object observer, string name) {
        List<NotificationObserver> list;
        if (observersDictionary.ContainsKey(key)) {
            list = observersDictionary[key];
        } else {
            list = new List<NotificationObserver>();
            observersDictionary.Add(key, list);
        }

        NotificationObserver o = new NotificationObserver(observer, name);
        if (!list.Contains(o)) {
            list.Add(o);
        }
    }

    public void RemoveObserver(string key, object observer, string name) {
        if (observersDictionary.ContainsKey(key)) {
            List<NotificationObserver> list = observersDictionary[key];
            list.Remove(new NotificationObserver(observer, name));
        }
    }

    public void Clear() {
        observersDictionary.Clear();
    }

    public void Post(string key, object sender, Dictionary<string, object> userdata) {
        if (observersDictionary.ContainsKey(key)) {
            List<NotificationObserver> list = observersDictionary[key];

            // TODO Concurrent Modification 관련 오류가 있음. 복사본 사용하도록 수정이 필요함.
            foreach (NotificationObserver observer in list) {
                Type thisType = observer.target.GetType();
                MethodInfo theMethod = thisType.GetMethod(observer.method);
                if (theMethod != null) {
                    theMethod.Invoke(observer.target, new object[] {sender, userdata});
                }
            }
        }
    }
}