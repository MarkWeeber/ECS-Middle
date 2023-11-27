using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace ECS.Space
{
    public class MonoTestValue : MonoBehaviour
    {
        [SerializeField] private uint targetEntityId;
        [SerializeField] private float xValue;

        private Entity targetEntity;

        private void Start()
        {
            targetEntity = GetTargetEntity();
        }

        private void LateUpdate()
        {
            if (Input.GetKeyDown(KeyCode.K))
            {
                targetEntity = GetTargetEntity();
            }
            if (targetEntity != Entity.Null)
            {
                xValue = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<LocalTransform>(targetEntity).Position.x;
            }
            else
            {
                xValue = -10;
            }
        }

        private Entity GetTargetEntity()
        {
            Entity responseEntity = Entity.Null;
            EntityQuery query = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(EntityIdComponent));
            NativeArray<Entity> entityArray = query.ToEntityArray(Allocator.Temp);
            if (entityArray.Length > 0)
            {
                foreach (Entity entity in entityArray)
                {
                    EntityIdComponent entityIdComponent = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<EntityIdComponent>(entity);
                    if (entityIdComponent.Id == targetEntityId)
                    {
                        responseEntity = entity;
                        break;
                    }
                }
            }
            return responseEntity;
        }
    }
}