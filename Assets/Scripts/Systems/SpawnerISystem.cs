using Unity.Burst;
using Unity.Entities;
using Unity.Collections;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Entities.UniversalDelegates;
using System.Linq;

namespace ECS.Space
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct SpawnerISystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<SpawnerComponent>();
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {

        }
        //[BurstCompile] - burst compile not usable since we are dealing with managed data
        public void OnUpdate(ref SystemState state)
        {
            // spawning entities
            SpawnerComponent spawnerComponent = SystemAPI.GetSingleton<SpawnerComponent>();
            EntityCommandBuffer entityCommandBufferSpawning = SystemAPI.GetSingleton<BeginInitializationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            EntityCommandBuffer ecb = new EntityCommandBuffer(Allocator.Temp);
            float3 newPosition = new float3(0f, 0f, 0f);
            EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<PlayerDataComponent>().Build(state.EntityManager);
            EntityManager entityManager = state.EntityManager;
            int existingEntitiesCount = entityQuery.CalculateEntityCount();
            int maxWidth = 10;
            int maxLength = 10;
            int x = 0;
            int y = 0;
            int z = 0;
            for (int i = existingEntitiesCount; i < spawnerComponent.MaxSpawn; i++)
            {
                Entity spawnedEntity = ecb.Instantiate(spawnerComponent.entitiyPrefab);
                newPosition = new float3(x * 2f, y * 2.5f, z * 2f);
                ecb.SetComponent<LocalTransform>(spawnedEntity, new LocalTransform { Position = newPosition, Scale = 1f, Rotation = quaternion.identity });
                x++;
                if (x >= maxWidth)
                {
                    x = 0;
                    z++;
                    if (z >= maxLength)
                    {
                        z = 0;
                        y++;
                    }
                }
            }
            ecb.Playback(state.EntityManager);
            // spawning gameobject and linking components to entity
            EntityCommandBuffer ecb2 = new EntityCommandBuffer(Allocator.Temp);
            foreach ((GameObjectManagedComponent gameObjectManagedComponent, RefRO<LocalTransform> localTransform, Entity entity)
                in SystemAPI.Query<GameObjectManagedComponent, RefRO<LocalTransform>>().WithEntityAccess())
            {
                GameObject spawnedGameObject = GameObject.Instantiate(gameObjectManagedComponent.gameObject);
                AnimatorManagedComponent animationComponent = new AnimatorManagedComponent();
                animationComponent.animator = spawnedGameObject.GetComponent<Animator>();
                ecb2.AddComponent<AnimatorManagedComponent>(entity, animationComponent);
                TransformManagedComponent transformManagedComponent = new TransformManagedComponent();
                transformManagedComponent.Transform = spawnedGameObject.transform;
                transformManagedComponent.Transform.position = localTransform.ValueRO.Position;
                ecb2.AddComponent<TransformManagedComponent>(entity, transformManagedComponent);
                ecb2.RemoveComponent(entity, typeof(GameObjectManagedComponent));
            }
            ecb2.Playback(state.EntityManager);
        }
    }
}