using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ECS.Space
{
    public class TestComponentAuthoring : MonoBehaviour
    {
        public float3 Position;
    }

    public class TestComponentBaker : Baker<TestComponentAuthoring>
    {
        public override void Bake(TestComponentAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new TestComponent
            {
                Position = authoring.Position,
                Position2 = new float3(2f, 1f, 0f)
            }) ;
        }
    }
}