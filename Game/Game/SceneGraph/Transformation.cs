using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PBLGame.SceneGraph
{
    public enum TransformationOrder
    {
        PositionRotationScale,
        PositionScaleRotation,
        ScalePositionRotation,
        ScaleRotationPosition,
        RotationScalePosition,
        RotationPositionScale,
    }

    public enum RotationOrder
    {
        RotateXYZ,
        RotateXZY,
        RotateYXZ,
        RotateYZX,
        RotateZXY,
        RotateZYX,
    }

    public class Transformation
    {
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;

        public TransformationOrder transformOrder = TransformationOrder.ScaleRotationPosition;
        public RotationOrder rotationOrder = RotationOrder.RotateYXZ;

        public Transformation()
        {
            position = Vector3.Zero;
            rotation = Vector3.Zero;
            scale = Vector3.One;
        }

        public Transformation(Transformation other)
        {
            position = other.position;
            rotation = other.rotation;
            scale = other.scale;
            transformOrder = other.transformOrder;
            rotationOrder = other.rotationOrder;
        }

        public Matrix CreateRotationMatrix()
        {
            switch (rotationOrder)
            {
                case RotationOrder.RotateXYZ:
                    return Matrix.CreateRotationX(rotation.X) * Matrix.CreateRotationY(rotation.Y) * Matrix.CreateRotationZ(rotation.Z);

                case RotationOrder.RotateXZY:
                    return Matrix.CreateRotationX(rotation.X) * Matrix.CreateRotationZ(rotation.Z) * Matrix.CreateRotationY(rotation.Y);

                case RotationOrder.RotateYXZ:
                    return Matrix.CreateRotationX(rotation.Y) * Matrix.CreateRotationZ(rotation.X) * Matrix.CreateRotationY(rotation.Z);

                case RotationOrder.RotateYZX:
                    return Matrix.CreateRotationY(rotation.Y) * Matrix.CreateRotationZ(rotation.Z) * Matrix.CreateRotationX(rotation.X);

                case RotationOrder.RotateZXY:
                    return Matrix.CreateRotationZ(rotation.Z) * Matrix.CreateRotationX(rotation.X) * Matrix.CreateRotationY(rotation.Y);

                case RotationOrder.RotateZYX:
                    return Matrix.CreateRotationZ(rotation.Z) * Matrix.CreateRotationY(rotation.Y) * Matrix.CreateRotationX(rotation.X);

                default:
                    throw new System.Exception("RotationOrder not exist");
            }
        }

        public Matrix CreateMatrix()
        {
            Matrix pos = Matrix.CreateTranslation(position);
            Matrix rot = CreateRotationMatrix();
            Matrix scl = Matrix.CreateScale(scale);

            switch (transformOrder)
            {
                case TransformationOrder.PositionRotationScale:
                    return pos * rot * scl;

                case TransformationOrder.PositionScaleRotation:
                    return pos * scl * rot;

                case TransformationOrder.ScalePositionRotation:
                    return scl * pos * rot;

                case TransformationOrder.ScaleRotationPosition:
                    return scl * rot * pos;

                case TransformationOrder.RotationScalePosition:
                    return rot * scl * pos;

                case TransformationOrder.RotationPositionScale:
                    return rot * pos * scl;

                default:
                    throw new System.Exception("TransformationOrder not exist");
            }
        }
    }
}