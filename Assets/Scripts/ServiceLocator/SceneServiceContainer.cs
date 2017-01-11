using UnityEngine;
using System.Collections.Generic;

public class SceneServiceContainer : MonoBehaviour 
{
    [SerializeField] private MonoBehaviour[] _components;
    private ServiceLocator _locator;

    private void Awake()
    {
        _locator = SL.Instance;

        foreach(var component in _components)
        {
            if(component != this)
            {
                _locator.AddExisting(component);
            }
        }
    }

    private void OnDestroy()
    {
        if(_components != null && _locator != null)
        {
            foreach(var component in _components)
            {
                if (component != this)
                {
                    _locator.Remove(component.GetType());
                }
            }
        }
    }
	
}
