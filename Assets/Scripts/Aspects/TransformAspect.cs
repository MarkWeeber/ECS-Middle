using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Properties;
using Unity.Transforms;
using UnityEngine;

namespace ECS.Space
{
    public readonly partial struct TransformAspect : IAspect
    {
        readonly RefRW<LocalToWorldTransform> m_LocalToWorldTransform;
        [Optional]
        readonly RefRW<LocalToParentTransform> m_LocalToParentTransform;
        [Optional]
        // TODO: This should be RO, blocked by DOTS-6308
        readonly RefRW<ParentToWorldTransform> m_ParentToWorldTransform;

        // --- Properties R/W ---

        /// <summary>
        /// The local to world transform, or how the entity is positioned, rotated and scaled in world space.
        /// </summary>
        [CreateProperty]
        public UniformScaleTransform LocalToWorld
        {
            get => m_LocalToWorldTransform.ValueRO.Value;
            set
            {
                m_LocalToWorldTransform.ValueRW.Value = value;
                if (HasParent())
                {
                    m_LocalToParentTransform.ValueRW.Value = ParentToWorld.InverseTransformTransform(value);
                }
            }
        }

        /// <summary>
        /// The local to parent transform, or how the entity is positioned, rotated and scaled relative to its parent.
        /// </summary>
        [CreateProperty]
        public UniformScaleTransform LocalToParent
        {
            get => m_LocalToParentTransform.ValueRO.Value;
            set
            {
                m_LocalToParentTransform.ValueRW.Value = value;
                if (HasParent())
                {
                    m_LocalToWorldTransform.ValueRW.Value = ParentToWorld.TransformTransform(value);
                }
            }
        }

        /// <summary>The world space position of the entity.</summary>
        [CreateProperty]
        public float3 Position
        {
            get => m_LocalToWorldTransform.ValueRO.Value.Position;
            set
            {
                m_LocalToWorldTransform.ValueRW.Value.Position = value;
                if (HasParent())
                {
                    m_LocalToParentTransform.ValueRW.Value.Position =
                        ParentToWorld.InverseTransformPoint(value);
                }
            }
        }

        /// <summary>The world space rotation of the entity.</summary>
        [CreateProperty]
        public quaternion Rotation
        {
            // Gets the cached value, last written by TransformHierarchySystem
            get => m_LocalToWorldTransform.ValueRO.Value.Rotation;

            // If entity has a parent, this will write to the relative transform, which has not yet been cached
            set
            {
                m_LocalToWorldTransform.ValueRW.Value.Rotation = value;
                if (HasParent())
                {
                    m_LocalToParentTransform.ValueRW.Value.Rotation =
                        ParentToWorld.InverseTransformRotation(value);
                }
            }
        }

        /// <summary>The position of this entity relative to its parent.</summary>
        [CreateProperty]
        public float3 LocalPosition
        {
            get => HasParent() ? m_LocalToParentTransform.ValueRO.Value.Position : m_LocalToWorldTransform.ValueRO.Value.Position;
            set
            {
                if (HasParent())
                {
                    m_LocalToParentTransform.ValueRW.Value.Position = value;
                    m_LocalToWorldTransform.ValueRW.Value.Position = ParentToWorld.TransformPoint(value);
                }
                else
                {
                    m_LocalToWorldTransform.ValueRW.Value.Position = value;
                }
            }
        }

        /// <summary>The rotation of this entity relative to its parent.</summary>
        [CreateProperty]
        public quaternion LocalRotation
        {
            get => HasParent() ? m_LocalToParentTransform.ValueRO.Value.Rotation : m_LocalToWorldTransform.ValueRO.Value.Rotation;
            set
            {
                if (HasParent())
                {
                    m_LocalToParentTransform.ValueRW.Value.Rotation = value;
                    m_LocalToWorldTransform.ValueRW.Value.Rotation = ParentToWorld.TransformRotation(value);
                }
                else
                {
                    m_LocalToWorldTransform.ValueRW.Value.Rotation = value;
                }
            }
        }

        // Properties Read Only
        // --------------------

        /// <summary>This is a copy of the parent's LocalToWorld transform</summary>
        public UniformScaleTransform ParentToWorld
        {
            get => m_ParentToWorldTransform.ValueRO.Value;
        }

        /// <summary>The forward direction in world space.</summary>
        public float3 Forward
        {
            get => LocalToWorld.Forward();
        }

        /// <summary>The back direction in world space.</summary>
        public float3 Back
        {
            get => -Forward;
        }

        /// <summary>The up direction in world space.</summary>
        public float3 Up
        {
            get => LocalToWorld.Up();
        }

        /// <summary>The down direction in world space.</summary>
        public float3 Down
        {
            get => -Up;
        }

        /// <summary>The right direction in world space.</summary>
        public float3 Right
        {
            get => LocalToWorld.Right();
        }

        /// <summary>The left direction in world space.</summary>
        public float3 Left
        {
            get => -Right;
        }

        /// <summary>Convert the LocalToWorld transform into a matrix.</summary>
        public float4x4 LocalToWorldMatrix
        {
            get => LocalToWorld.ToMatrix();
        }

        /// <summary>Convert the inverse of the LocalToWorld transform into a matrix.</summary>
        public float4x4 WorldToLocalMatrix
        {
            get => LocalToWorld.Inverse().ToMatrix();
        }

        /// <summary>Convert the ParentToWorld transform into a matrix.</summary>
        public float4x4 ParentToWorldMatrix
        {
            get => ParentToWorld.ToMatrix();
        }

        /// <summary>Convert the inverse of the ParentToWorld transform into a matrix.</summary>
        public float4x4 WorldToParentMatrix
        {
            get => ParentToWorld.Inverse().ToMatrix();
        }

        /// <summary>Convert the LocalToParent transform into a matrix.</summary>
        public float4x4 LocalToParentMatrix
        {
            get => LocalToParent.ToMatrix();
        }

        /// <summary>Convert the inverse of the LocalToParent transform into a matrix.</summary>
        public float4x4 ParentToLocalMatrix
        {
            get => LocalToParent.Inverse().ToMatrix();
        }

        // --- Methods ---

        /// <summary>Translate the entity in world space.</summary>
        /// <param name="translation">The relative translation.</param>
        public void TranslateWorld(float3 translation)
        {
            if (HasParent())
            {
                translation = ParentToWorld.InverseTransformDirection(translation);
            }
            TranslateLocal(translation);
        }

        /// <summary>Rotate the entity in world space.</summary>
        /// <param name="rotation">The relative rotation.</param>
        public void RotateWorld(quaternion rotation)
        {
            if (HasParent())
            {
                var childWorldRotation = math.mul(m_ParentToWorldTransform.ValueRO.Value.Rotation, LocalRotation);
                rotation = math.mul(math.mul(math.inverse(childWorldRotation), rotation), childWorldRotation);
            }
            RotateLocal(rotation);
        }

        /// <summary>Translate the entity relative to its parent.</summary>
        /// <param name="translation">The relative translation.</param>
        public void TranslateLocal(float3 translation)
        {
            if (HasParent())
            {
                m_LocalToParentTransform.ValueRW.Value.Position += translation;
                m_LocalToWorldTransform.ValueRW.Value.Position =
                    ParentToWorld.TransformPoint(m_LocalToParentTransform.ValueRO.Value.Position);
            }
            else
            {
                m_LocalToWorldTransform.ValueRW.Value.Position += translation;
            }
        }

        /// <summary>Rotate the entity relative to its parent.</summary>
        /// <param name="rotation">The relative rotation.</param>
        public void RotateLocal(quaternion rotation)
        {
            if (HasParent())
            {
                m_LocalToParentTransform.ValueRW.Value.Rotation = math.mul(m_LocalToParentTransform.ValueRO.Value.Rotation, rotation);
                m_LocalToWorldTransform.ValueRW.Value.Rotation =
                    ParentToWorld.TransformRotation(m_LocalToParentTransform.ValueRO.Value.Rotation);
            }
            else
            {
                m_LocalToWorldTransform.ValueRW.Value.Rotation = math.mul(LocalToWorld.Rotation, rotation);
            }
        }

        /// <summary>Transform a point from parent space into world space.</summary>
        /// <param name="point">The point to transform</param>
        /// <returns>The transformed point</returns>>
        public float3 TransformPointParentToWorld(float3 point)
        {
            return ParentToWorld.TransformPoint(point);
        }

        /// <summary>Transform a point from world space into parent space.</summary>
        /// <param name="point">The point to transform</param>
        /// <returns>The transformed point</returns>>
        public float3 TransformPointWorldToParent(float3 point)
        {
            return ParentToWorld.InverseTransformPoint(point);
        }

        /// <summary>Transform a point from local space into world space.</summary>
        /// <param name="point">The point to transform</param>
        /// <returns>The transformed point</returns>>
        public float3 TransformPointLocalToWorld(float3 point)
        {
            return LocalToWorld.TransformPoint(point);
        }

        /// <summary>Transform a point from world space into local space.</summary>
        /// <param name="point">The point to transform</param>
        /// <returns>The transformed point</returns>>
        public float3 TransformPointWorldToLocal(float3 point)
        {
            return LocalToWorld.InverseTransformPoint(point);
        }

        /// <summary>Transform a direction vector from parent space into world space.</summary>
        /// <param name="direction">The direction to transform</param>
        /// <returns>The transformed direction</returns>>
        public float3 TransformDirectionParentToWorld(float3 direction)
        {
            return ParentToWorld.TransformDirection(direction);
        }

        /// <summary>Transform a direction vector from world space into parent space.</summary>
        /// <param name="direction">The direction to transform</param>
        /// <returns>The transformed direction</returns>>
        public float3 TransformDirectionWorldToParent(float3 direction)
        {
            return ParentToWorld.InverseTransformDirection(direction);
        }

        /// <summary>Transform a direction vector from local space into world space.</summary>
        /// <param name="direction">The direction to transform</param>
        /// <returns>The transformed direction</returns>>
        public float3 TransformDirectionLocalToWorld(float3 direction)
        {
            return LocalToWorld.TransformDirection(direction);
        }

        /// <summary>Transform a direction vector from world space into local space.</summary>
        /// <param name="direction">The direction to transform</param>
        /// <returns>The transformed direction</returns>>
        public float3 TransformDirectionWorldToLocal(float3 direction)
        {
            return LocalToWorld.InverseTransformDirection(direction);
        }

        /// <summary>Transform a rotation quaternion from parent space into world space.</summary>
        /// <param name="rotation">The direction to transform</param>
        /// <returns>The transformed direction</returns>>
        public float3 TransformRotationParentToWorld(float3 rotation)
        {
            return ParentToWorld.TransformDirection(rotation);
        }

        /// <summary>Transform a rotation quaternion from world space into parent space.</summary>
        /// <param name="rotation">The direction to transform</param>
        /// <returns>The transformed direction</returns>>
        public float3 TransformRotationWorldToParent(float3 rotation)
        {
            return ParentToWorld.InverseTransformDirection(rotation);
        }

        /// <summary>Transform a rotation quaternion from local space into world space.</summary>
        /// <param name="rotation">The direction to transform</param>
        /// <returns>The transformed direction</returns>>
        public float3 TransformRotationLocalToWorld(float3 rotation)
        {
            return LocalToWorld.TransformDirection(rotation);
        }

        /// <summary>Transform a rotation quaternion from world space into local space.</summary>
        /// <param name="rotation">The direction to transform</param>
        /// <returns>The transformed direction</returns>>
        public float3 TransformRotationWorldToLocal(float3 rotation)
        {
            return LocalToWorld.InverseTransformDirection(rotation);
        }

        /// <summary>
        /// Compute the rotation so that the forward vector points to the target.
        /// The up vector is assumed to be world up.
        ///</summary>
        /// <param name="targetPosition">The world space point to look at</param>
        public void LookAt(float3 targetPosition)
        {
            LookAt(targetPosition, math.up());
        }

        /// <summary>
        /// Compute the rotation so that the forward vector points to the target.
        /// This version takes an up vector.
        ///</summary>
        /// <param name="targetPosition">The world space point to look at</param>
        /// <param name="up">The up vector</param>
        public void LookAt(float3 targetPosition, float3 up)
        {
            if (HasParent())
            {
                targetPosition = ParentToWorld.InverseTransformPoint(targetPosition);
            }

            var targetDir = targetPosition - LocalPosition;
            LocalRotation = quaternion.LookRotationSafe(targetDir, up);
        }

        // --- Private methods ---

        bool HasParent()
        {
            return m_LocalToParentTransform.IsValid && m_ParentToWorldTransform.IsValid;
        }
    }

    [WriteGroup(typeof(LocalToWorld))]
    public struct LocalToWorldTransform : IComponentData, IQueryTypeParameter
    {
        public UniformScaleTransform Value;
    }

    public struct LocalToParentTransform : IComponentData, IQueryTypeParameter
    {
        public UniformScaleTransform Value;
    }

    public struct ParentToWorldTransform : IComponentData, IQueryTypeParameter
    {
        public UniformScaleTransform Value;
    }

    public struct UniformScaleTransform
    {
        /// <summary>
        /// The position of this transform.
        /// </summary>
        [CreateProperty]
        public float3 Position;

        /// <summary>
        /// The uniform scale of this transform.
        /// </summary>
        [CreateProperty]
        public float Scale;

        /// <summary>
        /// The rotation of this transform.
        /// </summary>
        [CreateProperty]
        public quaternion Rotation;

        /// <summary>
        /// Convert transformation data to a human-readable string
        /// </summary>
        /// <returns>The transform value as a human-readable string</returns>
        public override string ToString()
        {
            return $"Position={Position} Rotation={Rotation} Scale={Scale}";
        }

        /// <summary>
        /// The identity transform.
        /// </summary>
        public static readonly UniformScaleTransform Identity = new UniformScaleTransform { Scale = 1.0f, Rotation = quaternion.identity };

        /// <summary>
        /// Returns the right vector of unit length.
        /// </summary>
        /// <returns>The right vector.</returns>
        public readonly float3 Right()
        {
            return TransformDirection(math.right());
        }

        /// <summary>
        /// Returns the up vector of unit length.
        /// </summary>
        /// <returns>The up vector.</returns>
        public readonly float3 Up()
        {
            return TransformDirection(math.up());
        }

        /// <summary>
        /// Returns the forward vector of unit length.
        /// </summary>
        /// <returns>The forward vector.</returns>
        public readonly float3 Forward()
        {
            return TransformDirection(math.forward());
        }

        /// <summary>
        /// Transforms a point by this transform.
        /// </summary>
        /// <param name="point">The point to be transformed.</param>
        /// <returns>The point after transformation.</returns>
        public readonly float3 TransformPoint(float3 point)
        {
            return Position + math.rotate(Rotation, point) * Scale;
        }

        /// <summary>
        /// Transforms a point by the inverse of this transform.
        /// </summary>
        /// <param name="point">The point to be transformed.</param>
        /// <returns>The point after transformation.</returns>
        public readonly float3 InverseTransformPoint(float3 point)
        {
            return math.rotate(math.conjugate(Rotation), point - Position) / Scale;
        }

        /// <summary>
        /// Transforms a direction by this transform.
        /// </summary>
        /// <param name="direction">The direction to be transformed.</param>
        /// <returns>The direction after transformation.</returns>
        public readonly float3 TransformDirection(float3 direction)
        {
            return math.rotate(Rotation, direction);
        }

        /// <summary>
        /// Transforms a direction by the inverse of this transform.
        /// </summary>
        /// <param name="direction">The direction to be transformed.</param>
        /// <returns>The direction after transformation.</returns>
        public readonly float3 InverseTransformDirection(float3 direction)
        {
            return math.rotate(math.conjugate(Rotation), direction);
        }

        /// <summary>
        /// Transforms a rotation by this transform.
        /// </summary>
        /// <param name="rotation">The rotation to be transformed.</param>
        /// <returns>The rotation after transformation.</returns>
        public readonly quaternion TransformRotation(quaternion rotation)
        {
            return math.mul(Rotation, rotation);
        }

        /// <summary>
        /// Transforms a rotation by the inverse of this transform.
        /// </summary>
        /// <param name="rotation">The rotation to be transformed.</param>
        /// <returns>The rotation after transformation.</returns>
        public readonly quaternion InverseTransformRotation(quaternion rotation)
        {
            return math.mul(math.conjugate(Rotation), rotation);
        }

        /// <summary>
        /// Transforms a scale by this transform.
        /// </summary>
        /// <param name="scale">The scale to be transformed.</param>
        /// <returns>The scale after transformation.</returns>
        public readonly float TransformScale(float scale)
        {
            return scale * Scale;
        }

        /// <summary>
        /// Transforms a scale by the inverse of this transform.
        /// </summary>
        /// <param name="scale">The scale to be transformed.</param>
        /// <returns>The scale after transformation.</returns>
        public readonly float InverseTransformScale(float scale)
        {
            return scale / Scale;
        }

        /// <summary>
        /// Transforms a UniformScaleTransform by this transform.
        /// </summary>
        /// <param name="transform">The UniformScaleTransform to be transformed.</param>
        /// <returns>The UniformScaleTransform after transformation.</returns>
        public readonly UniformScaleTransform TransformTransform(UniformScaleTransform transform)
        {
            return new UniformScaleTransform
            {
                Position = TransformPoint(transform.Position),
                Scale = TransformScale(transform.Scale),
                Rotation = TransformRotation(transform.Rotation),
            };
        }

        /// <summary>
        /// Transforms a UniformScaleTransform by the inverse of this transform.
        /// </summary>
        /// <param name="transform">The UniformScaleTransform to be transformed.</param>
        /// <returns>The UniformScaleTransform after transformation.</returns>
        public readonly UniformScaleTransform InverseTransformTransform(UniformScaleTransform transform)
        {
            return new UniformScaleTransform
            {
                Position = InverseTransformPoint(transform.Position),
                Scale = InverseTransformScale(transform.Scale),
                Rotation = InverseTransformRotation(transform.Rotation),
            };
        }

        /// <summary>
        /// Returns the inverse of this transform.
        /// </summary>
        /// <returns>The inverse of the transform.</returns>
        public readonly UniformScaleTransform Inverse()
        {
            var inverseRotation = math.conjugate(Rotation);
            var inverseScale = 1.0f / Scale;
            return new UniformScaleTransform
            {
                Position = -math.rotate(inverseRotation, Position) * inverseScale,
                Scale = inverseScale,
                Rotation = inverseRotation,
            };
        }

        /// <summary>
        /// Returns the float4x4 equivalent of this transform.
        /// </summary>
        /// <returns>The float4x4 matrix.</returns>
        public readonly float4x4 ToMatrix()
        {
            return float4x4.TRS(Position, Rotation, Scale);
        }

        /// <summary>
        /// Returns the float4x4 equivalent of the inverse of this transform.
        /// </summary>
        /// <returns>The inverse float4x4 matrix.</returns>
        public readonly float4x4 ToInverseMatrix()
        {
            return Inverse().ToMatrix();
        }

        /// <summary>
        /// Returns the UniformScaleTransform equivalent of a float4x4 matrix.
        /// </summary>
        /// <param name="matrix">The orthogonal matrix to convert.</param>
        /// <remarks>
        /// If the input matrix contains non-uniform scale, the largest value will be used.
        /// </remarks>
        /// <returns>The UniformScaleTransform.</returns>
        public static UniformScaleTransform FromMatrix(float4x4 matrix)
        {
            var position = matrix.c3.xyz;
            var scaleX = math.length(matrix.c0.xyz);
            var scaleY = math.length(matrix.c1.xyz);
            var scaleZ = math.length(matrix.c2.xyz);
            var scale = math.max(scaleX, math.max(scaleY, scaleZ));

            float3x3 normalizedRotationMatrix = math.orthonormalize(new float3x3(matrix));
            var rotation = new quaternion(normalizedRotationMatrix);

            return new UniformScaleTransform
            {
                Position = position,
                Scale = scale,
                Rotation = rotation
            };
        }

        /// <summary>
        /// Returns a UniformScaleTransform initialized with the given position and rotation. Scale will be 1.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="rotation">The rotation.</param>
        /// <returns>The UniformScaleTransform.</returns>
        public static UniformScaleTransform FromPositionRotation(float3 position, quaternion rotation)
        {
            return new UniformScaleTransform
            {
                Position = position,
                Scale = 1.0f,
                Rotation = rotation
            };
        }

        /// <summary>
        /// Returns a UniformScaleTransform initialized with the given position, rotation and scale.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="rotation">The rotation.</param>
        /// <param name="scale">The scale.</param>
        /// <returns>The UniformScaleTransform.</returns>
        public static UniformScaleTransform FromPositionRotationScale(float3 position, quaternion rotation, float scale)
        {
            return new UniformScaleTransform
            {
                Position = position,
                Scale = scale,
                Rotation = rotation
            };
        }

        /// <summary>
        /// Returns a UniformScaleTransform initialized with the given position. Rotation will be identity, and scale will be 1.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>The UniformScaleTransform.</returns>
        public static UniformScaleTransform FromPosition(float3 position)
        {
            return new UniformScaleTransform
            {
                Position = position,
                Scale = 1.0f,
                Rotation = quaternion.identity
            };
        }

        /// <summary>
        /// Returns a UniformScaleTransform initialized with the given position. Rotation will be identity, and scale will be 1.
        /// </summary>
        /// <param name="x">The x coordinate of the position.</param>
        /// <param name="y">The y coordinate of the position.</param>
        /// <param name="z">The z coordinate of the position.</param>
        /// <returns>The UniformScaleTransform.</returns>
        public static UniformScaleTransform FromPosition(float x, float y, float z)
        {
            return new UniformScaleTransform
            {
                Position = new float3(x, y, z),
                Scale = 1.0f,
                Rotation = quaternion.identity
            };
        }

        /// <summary>
        /// Returns a UniformScaleTransform initialized with the given rotation. Position will be 0,0,0, and scale will be 1.
        /// </summary>
        /// <param name="rotation">The rotation.</param>
        /// <returns>The UniformScaleTransform.</returns>
        public static UniformScaleTransform FromRotation(quaternion rotation)
        {
            return new UniformScaleTransform
            {
                Scale = 1.0f,
                Rotation = rotation
            };
        }

        /// <summary>
        /// Returns a UniformScaleTransform initialized with the given scale. Position will be 0,0,0, and rotation will be identity.
        /// </summary>
        /// <param name="scale">The scale.</param>
        /// <returns>The UniformScaleTransform.</returns>
        public static UniformScaleTransform FromScale(float scale)
        {
            return new UniformScaleTransform
            {
                Scale = scale,
                Rotation = quaternion.identity
            };
        }

        /// <summary>
        /// Returns this UniformScaleTransform translated by the specified vector.
        /// Note that this does not modify the original transform. Rather it returns a new one.
        /// </summary>
        /// <param name="translation">The translation vector.</param>
        /// <returns>A new, translated UniformScaleTransform.</returns>
        public readonly UniformScaleTransform Translate(float3 translation)
        {
            return new UniformScaleTransform
            {
                Position = Position + translation,
                Scale = Scale,
                Rotation = Rotation,
            };
        }

        /// <summary>
        /// Returns this UniformScaleTransform scaled by the specified factor.
        /// Note that this does not modify the original transform. Rather it returns a new one.
        /// </summary>
        /// <param name="scale">The scaling factor.</param>
        /// <returns>A new, scaled UniformScaleTransform.</returns>
        public readonly UniformScaleTransform ApplyScale(float scale)
        {
            return new UniformScaleTransform
            {
                Position = Position,
                Scale = Scale * scale,
                Rotation = Rotation,
            };
        }

        /// <summary>
        /// Returns this UniformScaleTransform rotated by the specified quaternion.
        /// Note that this does not modify the original transform. Rather it returns a new one.
        /// </summary>
        /// <param name="rotation">The rotation quaternion of unit length.</param>
        /// <returns>A new, rotated UniformScaleTransform.</returns>
        public readonly UniformScaleTransform Rotate(quaternion rotation)
        {
            return new UniformScaleTransform
            {
                Position = Position,
                Scale = Scale,
                Rotation = math.mul(Rotation, rotation),
            };
        }

        /// <summary>
        /// Returns this UniformScaleTransform rotated around the X axis.
        /// Note that this does not modify the original transform. Rather it returns a new one.
        /// </summary>
        /// <param name="angle">The X rotation.</param>
        /// <returns>A new, rotated UniformScaleTransform.</returns>
        public readonly UniformScaleTransform RotateX(float angle)
        {
            return Rotate(quaternion.RotateX(angle));
        }

        /// <summary>
        /// Returns this UniformScaleTransform rotated around the Y axis.
        /// Note that this does not modify the original transform. Rather it returns a new one.
        /// </summary>
        /// <param name="angle">The Y rotation.</param>
        /// <returns>A new, rotated UniformScaleTransform.</returns>
        public readonly UniformScaleTransform RotateY(float angle)
        {
            return Rotate(quaternion.RotateY(angle));
        }

        /// <summary>
        /// Returns this UniformScaleTransform rotated around the Z axis.
        /// Note that this does not modify the original transform. Rather it returns a new one.
        /// </summary>
        /// <param name="angle">The Z rotation.</param>
        /// <returns>A new, rotated UniformScaleTransform.</returns>
        public readonly UniformScaleTransform RotateZ(float angle)
        {
            return Rotate(quaternion.RotateZ(angle));
        }
    }
}