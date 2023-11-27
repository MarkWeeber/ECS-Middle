using ECS.Space;
using Unity.Entities;
using UnityEngine;

namespace ECS.Space
{
    public class SphereColliderTagAuthoring : MonoBehaviour
    {
        public class SphereColliderBaker : Baker<SphereColliderTagAuthoring>
        {
            public override void Bake(SphereColliderTagAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new SphereColliderTag());
            }
        }
    }
}