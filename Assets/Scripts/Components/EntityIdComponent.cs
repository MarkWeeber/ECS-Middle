using Unity.Entities;

namespace ECS.Space
{
    public partial struct EntityIdComponent : IComponentData
    {
        public uint Id;
    }
}