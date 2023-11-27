using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace ECS.Space
{
    public partial class SpawnerSystemBase : SystemBase
    {
        protected override void OnUpdate()
        {
            //EntityQuery entityTagQuery = EntityManager.CreateEntityQuery(typeof(EntityTagComponent));
            //SpawnerComponent spawnerComponent = SystemAPI.GetSingleton<SpawnerComponent>();
            //int maxSpawn = spawnerComponent.MaxSpawn;
            //int spawnedAmmount = entityTagQuery.CalculateEntityCount();
            //if (spawnedAmmount < maxSpawn)
            //{
            //    EntityManager.Instantiate(spawnerComponent.prefab);
            //}
        }
    }
}