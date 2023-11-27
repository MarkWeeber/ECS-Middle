using ECS.Space;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace ECS.Space
{
    public partial class TestSystemBase : SystemBase
    {
        EntityQuery entityQuery;
        Collider[] collisions = new Collider[150];
        RaycastHit[] raycastHits = new RaycastHit[50];

        protected override void OnCreate()
        {
            entityQuery = GetEntityQuery(
            ComponentType.ReadOnly<Transform>(),
            ComponentType.ReadOnly<Animator>());
        }

        protected override void OnUpdate()
        {
            float deltaTime = SystemAPI.Time.DeltaTime;
            Entities
                .WithAll<SphereColliderComponent>()
                .WithAll<LocalTransform>()
                .ForEach
                (
                    (ref SphereColliderComponent sphereColliderComponent, in LocalTransform localTransform) =>
                    {
                        bool touching = Physics.CheckSphere(localTransform.Position, sphereColliderComponent.Radius);
                        if (touching)
                        {
                            sphereColliderComponent.TouchingCount = 3;
                        }
                        else
                        {
                            sphereColliderComponent.TouchingCount = 1;
                        }
                        //foreach (RaycastHit hit in Physics.SphereCastAll(localTransform.Position, sphereColliderComponent.Radius, Vector3.zero, 0f))
                        //{

                        //}
                        //raycastHits = Physics.SphereCastAll(localTransform.Position, sphereColliderComponent.Radius, Vector3.zero, 0f);
                        //uint collisionCount = (uint)Physics.OverlapSphereNonAlloc(localTransform.Position, sphereColliderComponent.Radius, this.collisions);
                        //sphereColliderComponent.TouchingCount = collisionCount;
                    }
                ).Run();
            //Entities
            //    .WithAll<Transform>()
            //    .WithAll<Animator>()
            //    .ForEach(
            //    (Transform t, Animator a) => 
            //    {
            //        t.position += new Vector3(1f, 1f, 1f) * deltaTime;
            //        a.SetFloat("Speed", 12f);
            //    }
            //    ).Run();
        }
    }
}
