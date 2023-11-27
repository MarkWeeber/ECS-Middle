using Unity.Burst;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

namespace ECS.Space
{
    public partial struct CollisionISystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {

        }

        public void OnUpdate(ref SystemState state)
        {
            SimulationSingleton simulationSingleton = SystemAPI.GetSingleton<SimulationSingleton>();
            state.Dependency = new CollideTriggerJob().Schedule(simulationSingleton, state.Dependency);
        }
    }

    public struct CollideTriggerJob : ITriggerEventsJob
    {
        public void Execute(TriggerEvent triggerEvent)
        {
            
        }
    }
}