using System;
using Unity.Entities;
using UnityEngine;

public class AnimatorManagedComponent : IComponentData
{
    public Animator animator;

    //public void Dispose()
    //{
    //    UnityEngine.Object.Destroy(animator);
    //}

    //public object Clone()
    //{
    //    return new AnimatorManagedComponent { animator = UnityEngine.Object.Instantiate(animator) };
    //}
}