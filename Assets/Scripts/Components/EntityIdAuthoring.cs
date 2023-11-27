using ECS.Space;
using Unity.Entities;
using UnityEngine;

namespace ECS.Space
{
    public class EntityIdAuthoring : MonoBehaviour
    {       
        public uint EntityId;

        private void Awake()
        {
            EntityId = 10;
            EntityId = (uint)GameObject.FindObjectsOfType(typeof(EntityIdAuthoring)).Length;
            EntityId = 11;
            Debug.Log(EntityId);
        }

        public class EntityIdBaker : Baker<EntityIdAuthoring>
        {
            public override void Bake(EntityIdAuthoring authoring)
            {
                uint entityId = 10;
                entityId = (uint)GameObject.FindObjectsOfType(typeof(EntityIdAuthoring)).Length;
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new EntityIdComponent
                {
                    Id = entityId
                });
            }
        }
    }
}
