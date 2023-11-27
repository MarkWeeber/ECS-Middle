using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;

namespace ECS.Space
{
    public partial struct AnimationSetSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
        }
        public void OnDestroy(ref SystemState state)
        {
        }
        public void OnUpdate(ref SystemState state)
        {
            foreach (var (playerDataComponent, localTransform, animatorComponent, transformManagedComponent) in
                SystemAPI.Query<RefRO<PlayerDataComponent>, RefRO<LocalTransform>, AnimatorManagedComponent, TransformManagedComponent>())
            {
                animatorComponent.animator.SetFloat("Speed", playerDataComponent.ValueRO.Velocity);
                transformManagedComponent.Transform.position = localTransform.ValueRO.Position;
            }
        }
    }
}