using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Minimal service locator container for Unity w/lazy-loading.
/// </summary>
public class ServiceLocator : MonoBehaviour 
{
    /// <summary>
    /// The bindings of all services attached to the persistent game object.
    /// The type is the binding type and the MonoBehaviour is the concrete instance that it resolves to.
    /// </summary>
    private Dictionary<Type, MonoBehaviour> _serviceBindings = new Dictionary<Type, MonoBehaviour>();

    /// <summary>
    /// A lookup table for all bindings that are to be lazy loaded on a Get<T> call.
    /// </summary>
    private Dictionary<Type, Type> _lazyLookup = new Dictionary<Type, Type>();

    /// <summary>
    /// Gets the service binding that resolves the given type.  If it does not exist but is registered for lazy 
    /// loading, creates the service.
    /// </summary>
    /// <returns>
    /// Null if the service does not exist and cannot be lazy loaded.
    /// </returns>
    public T Get<T>() where T : class
    {
        return (T)Get(typeof(T));
    }

    public object Get(Type t)
    {
        object concreteObject = null;

        if(_serviceBindings.ContainsKey(t))
        {
            concreteObject = _serviceBindings[t];
        }
        else if(_lazyLookup.ContainsKey(t))
        {
            var lazyConcreteType = _lazyLookup[t];
            concreteObject = addAndDisableExistingBinding(t, lazyConcreteType);
        }

        return concreteObject;
    }

    /// <summary>
    /// Adds a service binding of the given type.
    /// </summary>
    /// <returns>The instance that was created.</returns>
    public T Add<T>() where T : MonoBehaviour
    {
        return (T)Add(typeof(T), typeof(T));
    }

    /// <summary>
    /// Adds a service binding with a lookup type of T which resolves to the concrete instance U.
    /// </summary>
    /// <returns>The instance that was created.</returns>
    public T Add<T, U>() where U : MonoBehaviour, T
    {
        return (T)Add(typeof(T), typeof(U));
    }

    public object Add(Type t, Type u)
    {
        return (object)addAndDisableExistingBinding(t, u);
    }

    /// <summary>
    /// Adds a binding for an existing service of the given binding type.
    /// </summary>
    /// <returns>The instance that was created.</returns>
    public void AddExisting<T>(T existing) where T : MonoBehaviour
    {
        Type bindingType = existing.GetType();

        // Disable any existing instance of this binding.
        if(_serviceBindings.ContainsKey(bindingType) && _serviceBindings[bindingType] != null)
        {
            _serviceBindings[bindingType].enabled = false;
        }

        _serviceBindings[bindingType] = existing;
    }

    /// <summary>
    /// Adds a service binding of the given type lazily.  Will only be created when requested by Get<T>.
    /// </summary>
    public void AddLazy<T>() where T : MonoBehaviour
    {
        AddLazy(typeof(T), typeof(T));
    }

    /// <summary>
    /// Adds a service bindingwith a lookup type of T which resolves to the concrete instance U lazily. Will 
    /// only be created when requested by Get<T>.
    /// </summary>
    public void AddLazy<T, U>() where U : MonoBehaviour, T
    {
        AddLazy(typeof(T), typeof(U));
    }

    public void AddLazy(Type t, Type u)
    {
        _lazyLookup.Add(t, u);
    }

    /// <summary>
    /// Unregisters the given type and destroys the concrete instance that it resolves to.
    /// Does nothign if the type isn't registered.
    /// </summary>
    public void Remove<T>()
    {
        Remove(typeof(T));
    }

    /// <summary>
    /// Removes a service binding and destroys the concrete instance that it resolves to.
    /// Re-enables any previous instance for this binding.
    /// </summary>
    public void Remove(Type bindingType)
    {
        if (_serviceBindings.ContainsKey(bindingType) && _serviceBindings[bindingType] != null)
        {
            Destroy(_serviceBindings[bindingType]);
        }

        // If there's layered components, enable the previous one after removal.
        // Use the component order as an implicit stack.
        var existingTypes = GetComponents(bindingType);
        if(existingTypes.Length > 0)
        {
            _serviceBindings[bindingType] = (MonoBehaviour)existingTypes[existingTypes.Length - 1];
            _serviceBindings[bindingType].enabled = true;
        }
        else
        {
            _serviceBindings.Remove(bindingType);
        }
    }

    /// <summary>
    /// Adds a concrete type or replaces it, and registers the service binding.
    /// Precondition: Concrete type must be a MonoBehaviour and extend the binding type.
    /// </summary>
    /// <returns>The MonoBehaviour that was created.</returns>
    private MonoBehaviour addAndDisableExistingBinding(Type bindingType, Type concreteType)
    {
        // Disable any existing instance of this binding.
        if(_serviceBindings.ContainsKey(bindingType) && _serviceBindings[bindingType] != null)
        {
            _serviceBindings[bindingType].enabled = false;
        }

        var concreteInstance = (MonoBehaviour)gameObject.AddComponent(concreteType);
        _serviceBindings[bindingType] = concreteInstance;
        return concreteInstance;
    }
}