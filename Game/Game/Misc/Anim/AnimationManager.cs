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

        private AnimationPlayer animationPlayer;

        private AnimationClip animationClip;

        private Dictionary<string, AnimationClip> animationClips = new Dictionary<string, AnimationClip>();

        public void PlayAnimation(string _key)
        {
            animationClip = animationClips[_key];
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
            if (!ReferenceEquals(animationClip,animationPlayer.Clip))
            {
                animationPlayer = parent.GetComponent<ModelAnimatedComponent>().PlayClip(animationClip);
            }
            base.Update(gameTime);
        }

        AnimationManager(GameObject _parent)
        {
            parent = _parent;
        }

        AnimationManager(Dictionary<string, AnimationClip> _animationClips)
        {
            animationClips = _animationClips;
        }
    }
}