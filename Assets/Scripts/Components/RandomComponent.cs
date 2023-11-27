using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace ECS.Space
{
    public struct RandomComponent : IComponentData
    {
        public Unity.Mathematics.Random Random;
        public float Range;
    }
}