using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace ECS.Space
{
    public struct GlobalSpeedComponent : IComponentData
    {
        public float Value;
    }
}