using System.ComponentModel;
using Unity.Entities;
using Unity.Mathematics;

namespace ECS.Space
{
    public struct MoveTargetComponent : IComponentData
    {
        public float MinDistance;
        public float3 TargetPosition;
    }
}