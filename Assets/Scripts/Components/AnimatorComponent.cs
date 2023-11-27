using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace ECS.Space
{
    public class AnimatorComponent : ICleanupComponentData
    {
        public Animator Animator;
    }
}