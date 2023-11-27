using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace ECS.Space
{
    public class SpawnerComponentAuthoring : MonoBehaviour
    {
        public GameObject entitiyPrefab;
        public int MaxSpawn;
    }

    public class SpawnerComponentBaker : Baker<SpawnerComponentAuthoring>
    {
        public override void Bake(SpawnerComponentAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new SpawnerComponent
            {
                entitiyPrefab = GetEntity(authoring.entitiyPrefab, TransformUsageFlags.Dynamic),
                MaxSpawn = authoring.MaxSpawn
            });
        }
    }
}