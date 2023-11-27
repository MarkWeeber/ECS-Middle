using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ECS.Space
{
    public struct TestComponent : IComponentData
    {
        public float3 Position;
        public float3 Position2;
        public float x;
    }
}