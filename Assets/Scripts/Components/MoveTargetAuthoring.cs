using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace ECS.Space
{
    public class MoveTargetAuthoring : MonoBehaviour
    {
        public float MinDistance;
        public float3 TargetPosition;
    }

    public class MoveTargetBaker : Baker<MoveTargetAuthoring>
    {
        public override void Bake(MoveTargetAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new MoveTargetComponent
            {
                MinDistance = authoring.MinDistance,
                TargetPosition = authoring.TargetPosition,
            });
        }
    }
}