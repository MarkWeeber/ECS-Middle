using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace ECS.Space
{
    public partial class MovementSystemBase : SystemBase
    {
        private float x;
        protected override void OnCreate()
        {
        }

        protected override void OnUpdate()
        {
            //EntityQuery entityQuery = GetEntityQuery(ComponentType.ReadOnly<MonoTestValue>(), ComponentType.ReadOnly<LocalTransform>());
            //EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp)
            //    .WithAllRW<MonoTestValue>()
            //    .WithAll<MovementAspect>()
            //    .Build(this);

            //Entities.ForEach((MonoTestValue mono, in LocalTransform lt) => { }).Run();
            //float deltaTime = SystemAPI.Time.DeltaTime;
            //RefRW<RandomComponent> random = SystemAPI.GetSingletonRW<RandomComponent>();
            //foreach (MovementAspect movementAspect in SystemAPI.Query<MovementAspect>())
            //{
            //    movementAspect.Move(deltaTime);
            //    movementAspect.SetRandomDestionationIfReached(random);
            //}
        }
    }
}