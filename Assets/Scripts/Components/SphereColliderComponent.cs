using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;

namespace ECS.Space
{
    public partial struct SphereColliderComponent : IComponentData
    {
        public float Radius;
        public uint TouchingCount;
        public bool Touched;
    }
}
