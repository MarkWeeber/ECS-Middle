using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace ECS.Space
{
    public readonly partial struct MovementAspect : IAspect
    {
        private readonly RefRW<LocalTransform> localTransform;
        private readonly RefRW<SpeedComponent> speed;
        private readonly RefRW<MoveTargetComponent> moveTarget;
        private readonly RefRW<PlayerDataComponent> playerData;
        public void Move(float deltaTime)
        {
            MoveToTarget(deltaTime, speed.ValueRO.Value);
        }

        public void MoveSetBySpeed(float deltaTime, float speedValue)
        {
            MoveToTarget(deltaTime, speedValue);
        }

        public void SetRandomDestionationIfReached(RefRW<RandomComponent> randomComponent)
        {
            if (DestinationReached())
            {
                SetRandomDestination(randomComponent);
            }
        }

        public void SetRandomDestionationIfReachedWithRandomNewSpeed(RefRW<RandomComponent> randomComponent)
        {
            if(DestinationReached())
            {
                SetRandomDestination(randomComponent);
                SetRandomSpeed(randomComponent);
            }
        }

        private void MoveToTarget(float deltaTime, float speedValue)
        {
            playerData.ValueRW.Velocity = speedValue;
            float distance = math.distancesq(moveTarget.ValueRO.TargetPosition, localTransform.ValueRO.Position);
            if (distance > moveTarget.ValueRO.MinDistance)
            {
                float3 direction = math.normalize(moveTarget.ValueRO.TargetPosition - localTransform.ValueRO.Position);
                localTransform.ValueRW.Position += direction * speedValue * deltaTime;
            }
        }


        private bool DestinationReached()
        {
            float distance = math.distancesq(moveTarget.ValueRO.TargetPosition, localTransform.ValueRO.Position);
            if (distance < moveTarget.ValueRO.MinDistance)
            {
                return true;
            }
            return false;
        }

        private void SetRandomDestination(RefRW<RandomComponent> randomComponent)
        {
            moveTarget.ValueRW.TargetPosition = GetRandomPosition(randomComponent);
            playerData.ValueRW.MoveDirection = moveTarget.ValueRO.TargetPosition;
        }

        private void SetRandomSpeed(RefRW<RandomComponent> randomComponent)
        {
            speed.ValueRW.Value = GetRandomFloat(randomComponent);
        }

        private float3 GetRandomPosition(RefRW<RandomComponent> randomComponent)
        {
            return new float3
                (
                    randomComponent.ValueRW.Random.NextFloat(-randomComponent.ValueRO.Range, randomComponent.ValueRO.Range),
                    randomComponent.ValueRW.Random.NextFloat(-randomComponent.ValueRO.Range, randomComponent.ValueRO.Range),
                    randomComponent.ValueRW.Random.NextFloat(-randomComponent.ValueRO.Range, randomComponent.ValueRO.Range)
                );
        }

        private float GetRandomFloat(RefRW<RandomComponent> randomComponent)
        {
            return randomComponent.ValueRW.Random.NextFloat(0.02f, randomComponent.ValueRO.Range);
        }
    }
}