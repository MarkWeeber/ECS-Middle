using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace ECS.Space
{
    public readonly partial struct ColliderAspect : IAspect
    {
        private readonly Entity entity;
        private readonly RefRW<LocalTransform> localTransform;
        private readonly RefRW<SphereColliderComponent> sphereCollider;

        public void CheckColliders()
        {
            Collider[] results = new Collider[50];
            uint collisionCount = (uint) Physics.OverlapSphereNonAlloc(localTransform.ValueRO.Position, sphereCollider.ValueRO.Radius, results);
            sphereCollider.ValueRW.TouchingCount = collisionCount;
        }
    }
}
