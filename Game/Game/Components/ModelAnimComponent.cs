using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SkinnedModel;

namespace PBLGame.SceneGraph
{
    public class ModelAnimComponent : ModelComponent
    {
        private AnimationPlayer _animationPlayer;
        private AnimationClip _animationClip;
        private Matrix[] bones;
        public ModelAnimComponent(Model model, AnimationPlayer animationPlayer, AnimationClip animationClip) : base(model)
        {
            _animationPlayer = animationPlayer;
            _animationClip = animationClip;
            animationPlayer.StartClip(_animationClip);
        }

        public override void Draw(GameObject parent, Camera camera, Matrix localTransformations, Matrix worldTransformations)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                bones = _animationPlayer.GetBoneTransforms();
                foreach (SkinnedEffect effect in mesh.Effects)
                {
                    // set bone transforms
                    effect.SetBoneTransforms(bones);

                    // set world matrix
                    effect.World = worldTransformations;

                    // set view matrix
                    effect.View = camera.ViewMatrix;

                    // set projection matrix
                    effect.Projection = camera.ProjectionMatrix;

                    effect.EnableDefaultLighting();
                    effect.SpecularColor = new Vector3(0.25f);
                    effect.SpecularPower = 16;
                }
                mesh.Draw();
            }
        }

        public override void Update(GameTime gameTime)
        {         
            _animationPlayer.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);
            base.Update(gameTime);
        }
    }
}