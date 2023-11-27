using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace ECS.Space
{
    public class GlobalSpeedAuthoring : MonoBehaviour
    {
        public float Speed;
    }

    public class GlobalSpeedBaker : Baker<GlobalSpeedAuthoring>
    {
        public override void Bake(GlobalSpeedAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new GlobalSpeedComponent { Value = authoring.Speed });
        }
    }
}