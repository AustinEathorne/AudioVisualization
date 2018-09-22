using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectInstantiater : MonoSingleton<ObjectInstantiater>
{
    [Header("Prefabs")]
    public PrefabTypeGameObjectDictionary prefabDictionary;


    #region Main

    public override IEnumerator Initialize()
    {


        yield return null;
    }

    public override IEnumerator Run()
    {
        throw new NotImplementedException();
    }

    public override IEnumerator Stop()
    {
        this.isRunning = false;
        yield return null;
    }

    #endregion

    #region Instantiate

    public IEnumerator InstantiatePrefab(PrefabType _type, Transform _parent, Vector3 _position, Quaternion _rotation, Action<GameObject> _obj)
    {
        // Instantiate
        GameObject tempObj = GameObject.Instantiate(this.prefabDictionary[_type], _parent);
        tempObj.transform.localPosition = _position;
        tempObj.transform.localRotation = _rotation;
        tempObj.name = _type.ToString();

        // Object callback
        _obj(tempObj);

        yield return null;
    }

    #endregion
}

public enum PrefabType
{
    CubeVerticallyScaling = 0
}
