// Primary Author : Maximiliam Rosén - maka4519

using System;
using Combat.Interfaces;
using UnityEngine;

public abstract class EntityBase : MonoBehaviour, IDamageable
{
    public abstract void TakeDamage(float damage);

    public EntityBase GetEntity()
    {
        return this;
    }

    /// <summary>
    ///     Should be implemented to return collider radius and height.
    /// </summary>
    /// <returns>A Tuple<radius, height></returns>
    public abstract Tuple<float, float> GetDimensions();
}