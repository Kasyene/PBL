using System.Collections.Generic;
using AnimationAux;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using PBLGame.Misc.Anim;

namespace PBLGame.SceneGraph
{
    public class ModelAnimatedComponent : ModelComponent
    {
        private ModelExtra modelExtra;
        private AnimationPlayer animationPlayer = null;
        private List<Bone> bones = new List<Bone>();
        private List<AnimationClip> animationClips;

        public List<Bone> Bones => bones;
        private string assetName;

        public List<AnimationClip> AnimationClips => modelExtra.Clips;

        public ModelAnimatedComponent(string _assetName, ContentManager contentManager, Effect _modelEffect, Texture2D _texture = null, Texture2D _normal = null) : base(null, null)
        {
            assetName = _assetName;
            modelEffect = _modelEffect;
            texture = _texture;
            normalMap = _normal;
            LoadContent(contentManager);
        }

        public override void LoadContent(ContentManager contentManager)
        {
            base.LoadContent(contentManager);
            this.model = contentManager.Load<Model>(assetName);
            modelExtra = model.Tag as ModelExtra;
            System.Diagnostics.Debug.Assert(modelExtra != null);

            ObtainBones();
        }

        private void ObtainBones()
        {
            bones.Clear();
            foreach (ModelBone modelBone in model.Bones)
            {
                // Create bone and add to heirarchy
                Bone bone = new Bone(modelBone.Name, modelBone.Transform,
                    modelBone.Parent != null ? bones[modelBone.Parent.Index] : null);
                // Add bone for this model
                bones.Add(bone);
            }
        }

        public Bone FindBone(string name)
        {
            foreach (Bone bone in Bones)
            {
                if (bone.Name == name)
                    return bone;
            }

            return null;
        }

        public AnimationPlayer PlayClip(AnimationClip animationClip)
        {
            animationPlayer = new AnimationPlayer(animationClip,this);
            return animationPlayer;
        }

        public override void Draw(GameObject parent, Camera camera, Matrix localTransformations, Matrix worldTransformations, bool createShadowMap = false)
        {
            if(model == null)
                return;

            Matrix[] boneTransforms = new Matrix[bones.Count];

            for(int i = 0; i < bones.Count; ++i)
            {
                Bone bone = bones[i];
                bone.ComputeAbsoluteTransform();
                boneTransforms[i] = bone.AbsoluteTransform;
            }

            Matrix[] skeleton = new Matrix[modelExtra.Skeleton.Count];
            for (int i = 0; i < modelExtra.Skeleton.Count; ++i)
            {
                Bone bone = bones[modelExtra.Skeleton[i]];
                skeleton[i] = bone.SkinTransform * bone.AbsoluteTransform;
            }
            string techniqueName = createShadowMap ? "CreateShadowMap" : "Draw";
            foreach (ModelMesh modelMesh in model.Meshes)
            {
                foreach (ModelMeshPart part in modelMesh.MeshParts)
                {
                    part.Effect = modelEffect;
                    modelEffect.CurrentTechnique = modelEffect.Techniques[techniqueName];
                    modelEffect.Parameters["World"].SetValue(worldTransformations);
                    modelEffect.Parameters["View"].SetValue(camera.ViewMatrix);
                    modelEffect.Parameters["Projection"].SetValue(camera.ProjectionMatrix);
                    modelEffect.Parameters["WorldInverseTranspose"].SetValue(Matrix.Transpose(Matrix.Invert(worldTransformations)));
                    modelEffect.Parameters["Bones"].SetValue(skeleton);
                    modelEffect.Parameters["ViewVector"].SetValue(camera.GetViewVector());
                    SetShadingParameters(createShadowMap);
                }
                modelMesh.Draw();
                DrawBoundingBox(parent, localTransformations, worldTransformations, camera);
            }
        }

        public override BoundingBox GetBoundingBox(GameObject parent, Matrix localTransformations, Matrix worldTransformations)
        {
            Vector3 min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 max = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            Matrix[] transforms = new Matrix[model.Bones.Count];
            //System.Diagnostics.Debug.WriteLine(model.Bones.Count);
            model.CopyAbsoluteBoneTransformsTo(transforms);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart meshPart in mesh.MeshParts)
                {
                    int vertexStride = meshPart.VertexBuffer.VertexDeclaration.VertexStride;
                    int vertexBufferSize = meshPart.NumVertices * vertexStride;

                    float[] vertexData = new float[vertexBufferSize / sizeof(float)];
                    meshPart.VertexBuffer.GetData<float>(vertexData);

                    for (int i = 0; i < vertexBufferSize / sizeof(float); i += vertexStride / sizeof(float))
                    {
                        Vector3 currPosition = new Vector3(vertexData[i], vertexData[i + 1], vertexData[i + 2]);
                        currPosition = Vector3.Transform(currPosition, worldTransformations * transforms[mesh.ParentBone.Index]);
                        min = Vector3.Min(min, currPosition);
                        max = Vector3.Max(max, currPosition);
                    }
                }
            }
            return new BoundingBox(min, max);
        }

        public override void Update(GameTime gameTime)
        {
            animationPlayer?.Update(gameTime);
            base.Update(gameTime);
        }


    }
}