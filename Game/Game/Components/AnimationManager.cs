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
        private bool isAnimationUpdated = true;

        private Queue<AnimationClip> animQueue = new Queue<AnimationClip>();

        private AnimationPlayer animationPlayer;

        private AnimationClip animationClip;

        private Dictionary<string, AnimationClip> animationClips = new Dictionary<string, AnimationClip>();

        public void PlayAnimation(string _key)
        {
            animQueue.Enqueue(animationClips[_key]);
            isAnimationUpdated = false;
        }

        public void PlayAnimation(string _key, bool forceChange)
        {
            if (forceChange)
            {
                animQueue.Clear();
                //animationPlayer?.Rewind();
                animationClip = animationClips[_key];
                animationPlayer = parent.GetComponent<ModelAnimatedComponent>().PlayClip(animationClip);
                animationPlayer.Looping = isLooping;
                isAnimationUpdated = true;
                
                animQueue.Enqueue(animationClips[_key]);

            }
            else
            {
                PlayAnimation(_key);
            }
            
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
            if(animationPlayer != null)
            if (animQueue.Count > 1)
            {
                animationPlayer.Looping = false;
            }
            else
            {
                animationPlayer.Looping = true;
            }

            if (animationPlayer?.Position >= animationPlayer?.Duration || animationPlayer == null)
            {
                if (animQueue.Count > 1)
                {
                    animationClip = animQueue.Dequeue();
                    //isAnimationUpdated = true;
                    animationPlayer = parent.GetComponent<ModelAnimatedComponent>().PlayClip(animationClip);
                }
                else if (animQueue.Count == 1)
                {
                    animationClip = animQueue.Peek();
                    animationPlayer = parent.GetComponent<ModelAnimatedComponent>().PlayClip(animationClip);
                }
                else
                {
                    // SHRUG
                }
            }
            //base.Update(gameTime);

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