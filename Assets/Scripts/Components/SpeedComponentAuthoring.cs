using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace ECS.Space
{
    public class SpeedComponentAuthoring : MonoBehaviour
    {
        public float Value;
    }

    public class SpeedComponentBaker : Baker<SpeedComponentAuthoring>
    {
        public override void Bake(SpeedComponentAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new SpeedComponent
            {
                Value = authoring.Value
            });
        }
    }
}