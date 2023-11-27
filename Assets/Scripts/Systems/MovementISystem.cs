using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

namespace ECS.Space
{
    [BurstCompile]
    public partial struct MovementISystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            RefRW<RandomComponent> randomComponent = SystemAPI.GetSingletonRW<RandomComponent>();
            float deltaTime = SystemAPI.Time.DeltaTime;
            float setSpeed = SystemAPI.GetSingleton<GlobalSpeedComponent>().Value;
            MovementJob movementJob = new MovementJob
            {
                deltaTime = deltaTime,
                setSpeed = setSpeed
            };
            JobHandle jobHandle = movementJob.ScheduleParallel(state.Dependency);
            jobHandle.Complete();

            MovementSetRandomDestination movementSetRandomDestination = new MovementSetRandomDestination
            { 
                randomComponent = randomComponent
            };
            movementSetRandomDestination.Run();
        }
    }

    [BurstCompile]
    public partial struct MovementJob : IJobEntity
    {
        public float deltaTime;
        public float setSpeed;
        [BurstCompile]
        public void Execute(MovementAspect movementAspect)
        {
            movementAspect.Move(deltaTime);
            //movementAspect.MoveSetBySpeed(deltaTime, setSpeed);
        }
    }

    [BurstCompile]
    public partial struct MovementSetRandomDestination : IJobEntity
    {
        [NativeDisableUnsafePtrRestriction]
        public RefRW<RandomComponent> randomComponent;
        [BurstCompile]
        public void Execute(MovementAspect movementAspect)
        {
            //movementAspect.SetRandomDestionationIfReached(randomComponent);
            movementAspect.SetRandomDestionationIfReachedWithRandomNewSpeed(randomComponent);
        }
    }
}