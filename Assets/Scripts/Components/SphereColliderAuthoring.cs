using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Entities;
using UnityEngine;

namespace ECS.Space
{
    public class SphereColliderAuthoring : MonoBehaviour
    {
        public float Radius;
        public uint TouchCount;
        public bool Touched;

        public class SphereColliderBaking : Baker<SphereColliderAuthoring>
        {
            public override void Bake(SphereColliderAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new SphereColliderComponent
                {
                    Radius = authoring.Radius,
                    TouchingCount = authoring.TouchCount,
                    Touched = authoring.Touched
                });
            }
        }
    }
}
