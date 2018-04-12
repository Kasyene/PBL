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

        public ModelAnimatedComponent(string _assetName, ContentManager contentManager) : base(null)
        {
            assetName = _assetName;
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

        public override void Draw(GameObject parent, Camera camera, Matrix localTransformations, Matrix worldTransformations)
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

            foreach (ModelMesh modelMesh in model.Meshes)
            {
                foreach (Effect effect in modelMesh.Effects)
                {
                    if (effect is BasicEffect)
                    {
                        BasicEffect basicEffect = effect as BasicEffect;

                        // set world matrix
                        basicEffect.World = worldTransformations * boneTransforms[modelMesh.ParentBone.Index];

                        // set view matrix
                        basicEffect.View = camera.ViewMatrix;

                        // set projection matrix
                        basicEffect.Projection = camera.ProjectionMatrix;

                        basicEffect.EnableDefaultLighting();
                        basicEffect.PreferPerPixelLighting = true;
                    }

                    else if (effect is SkinnedEffect)
                    {
                        SkinnedEffect skinnedEffect = effect as SkinnedEffect;

                        // set world matrix
                        skinnedEffect.World = worldTransformations * boneTransforms[modelMesh.ParentBone.Index];

                        // set view matrix
                        skinnedEffect.View = camera.ViewMatrix;

                        // set projection matrix
                        skinnedEffect.Projection = camera.ProjectionMatrix;

                        skinnedEffect.EnableDefaultLighting();
                        skinnedEffect.PreferPerPixelLighting = true;
                        skinnedEffect.SetBoneTransforms(skeleton);
                    }
                   
                }
                modelMesh.Draw();
            }
        }

        public override void Update(GameTime gameTime)
        {
            animationPlayer?.Update(gameTime);
            base.Update(gameTime);
        }


    }
}