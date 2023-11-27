using Unity.Entities;
using Unity.Mathematics;

public struct PlayerDataComponent : IComponentData
{
    public float Velocity;
    public float3 MoveDirection;
}