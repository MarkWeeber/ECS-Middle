using ECS.Space;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PlayerDataAuthoring : MonoBehaviour
{
	public float Velocity;
	public float3 MoveDirection;

    class Baker : Baker<PlayerDataAuthoring>
	{
		public override void Bake(PlayerDataAuthoring authoring)
		{
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PlayerDataComponent
            {
                Velocity = authoring.Velocity,
                MoveDirection = authoring.MoveDirection
            });
        }
	}
}