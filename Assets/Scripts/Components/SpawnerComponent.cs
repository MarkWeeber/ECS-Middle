using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace ECS.Space
{
    public struct SpawnerComponent : IComponentData
    {
        public Entity entitiyPrefab;
        public int MaxSpawn;
    }
}