using ECS.Space;
using Unity.Entities;
using UnityEngine;

public class AnimatorManagedComponentAuthoring : MonoBehaviour
{
	public Animator animator;
	class Baker : Baker<AnimatorManagedComponentAuthoring>
	{
		public override void Bake(AnimatorManagedComponentAuthoring authoring)
		{
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AnimatorManagedComponent animationComponent = new AnimatorManagedComponent();
            animationComponent.animator = authoring.animator;
            AddComponentObject(entity, animationComponent);
        }
	}
}