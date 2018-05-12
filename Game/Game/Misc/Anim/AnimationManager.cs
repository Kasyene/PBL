using System.Collections.Generic;
using AnimationAux;
using Microsoft.Xna.Framework;
using PBLGame.SceneGraph;
using Component = PBLGame.SceneGraph.Component;

namespace PBLGame.Misc.Anim
{
    public class AnimationManager : Component
    {
        private GameObject parent;

        private bool isLooping = true;

        private AnimationPlayer animationPlayer;

        private AnimationClip animationClip;

        private Dictionary<string, AnimationClip> animationClips = new Dictionary<string, AnimationClip>();

        public void PlayAnimation(string _key)
        {
            //animationPlayer?.Rewind();
            animationClip = animationClips[_key];
            animationPlayer = parent.GetComponent<ModelAnimatedComponent>().PlayClip(animationClip);
            animationPlayer.Looping = isLooping;
        }

        public void AddAnimation(AnimationClip _animClip, string _key)
        {
            animationClips.Add(_key,_animClip);
        }

        public Dictionary<string, AnimationClip> GetAnimationDictionary()
        {
            return animationClips;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public AnimationManager(GameObject _parent)
        {
            parent = _parent;
        }

        AnimationManager(Dictionary<string, AnimationClip> _animationClips)
        {
            animationClips = _animationClips;
        }
    }
}