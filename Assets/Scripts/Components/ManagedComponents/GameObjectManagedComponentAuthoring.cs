using Unity.Entities;
using UnityEngine;

public class GameObjectManagedComponentAuthoring : MonoBehaviour
{
    public GameObject gameObjectPrefab;
	class Baker : Baker<GameObjectManagedComponentAuthoring>
	{
		public override void Bake(GameObjectManagedComponentAuthoring authoring)
		{
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            GameObjectManagedComponent gameObjectComponent = new GameObjectManagedComponent();
            gameObjectComponent.gameObject = authoring.gameObjectPrefab;
            AddComponentObject(entity, gameObjectComponent);
        }
	}
}