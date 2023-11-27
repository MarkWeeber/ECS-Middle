using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace ECS.Space
{
    public class RandomComponentAuthoring : MonoBehaviour
    {
        public float Range;
    }

    public class RandomComponentBaker : Baker<RandomComponentAuthoring>
    {
        public override void Bake(RandomComponentAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new RandomComponent
            {
                Random = new Unity.Mathematics.Random(1),
                Range = authoring.Range
            });
        }
    }
}