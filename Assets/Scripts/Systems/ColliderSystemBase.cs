using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace ECS.Space
{
    public partial class ColliderSystemBase : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithAll<SphereColliderComponent>()
                .WithAll<LocalTransform>().
                ForEach(
                (ref SphereColliderComponent sphereCollider, in LocalTransform localTransform) =>
                {
                    bool touching = Physics.CheckSphere(localTransform.Position, sphereCollider.Radius);
                    if (touching) 
                    {
                        sphereCollider.TouchingCount = 2;
                    }
                    else
                    {
                        sphereCollider.TouchingCount = 1;
                    }
                }
                ).Run();
        }
    }

    public partial struct ColliderIJob : IJobEntity
    {
        public void Execute(ColliderAspect colliderAspect)
        {
            colliderAspect.CheckColliders();
        }
    }
}
