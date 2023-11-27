using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace ECS.Space
{
    public class EntityTagComponentAuthoring : MonoBehaviour
    {
        
    }

    public class EntityTagComponentBaker : Baker<EntityTagComponentAuthoring>
    {
        public override void Bake(EntityTagComponentAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EntityTagComponent());
        }
    }
}