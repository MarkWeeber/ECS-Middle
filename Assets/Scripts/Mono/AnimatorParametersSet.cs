using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace ECS.Space
{
    public class AnimatorParametersSet : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        [SerializeField] private float speed;
        private Entity targetEntity;
        private void Start()
        {
            targetEntity = GetTargetEntity();
        }

        private void Update()
        {
            if (targetEntity != Entity.Null)
            {
                speed = World.DefaultGameObjectInjectionWorld.EntityManager.GetComponentData<GlobalSpeedComponent>(targetEntity).Value;
            }
            else
            {
                speed = -10;
            }
            animator.SetFloat("Speed", speed);
        }

        private Entity GetTargetEntity()
        {
            Entity responseEntity = Entity.Null;
            EntityQuery query = World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntityQuery(typeof(GlobalSpeedComponent));
            NativeArray<Entity> entityArray = query.ToEntityArray(Allocator.Temp);
            if (entityArray.Length > 0)
            {
                responseEntity = entityArray[0];
            }
            return responseEntity;
        }
    }
}