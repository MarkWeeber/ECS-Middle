using Unity.Entities;
using UnityEngine;

namespace ECS.Space
{
    public class TransformManagedComponent : IComponentData
    {
        public Transform Transform;
    }
}