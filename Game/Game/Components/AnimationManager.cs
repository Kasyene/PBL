using System;
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

        private bool isLooping = false;
        private bool isAnimationUpdated = true;

        public bool isReady { get; set; }

        public string defaultKey {get; private set; }

        private Queue<AnimationClip> animQueue = new Queue<AnimationClip>();

        private AnimationPlayer animationPlayer;

        private AnimationClip animationClip;
        private AnimationClip animationClipDefault;

        private Dictionary<string, AnimationClip> animationClips = new Dictionary<string, AnimationClip>();

        public void SetDefaultAnimation(string _key)
        {
            animationClipDefault = animationClips[_key];
            defaultKey = _key;
        }

        public void PlayAnimation(string _key)
        {
            animQueue.Enqueue(animationClips[_key]);
            isReady = false;

            if (animationPlayer == null)
            {
                animationClipDefault = animationClips[_key];
                defaultKey = _key;
                animationPlayer = parent.GetComponent<ModelAnimatedComponent>().PlayClip(animationClipDefault);
            }
        }

        public void PlayAnimation(string _key, bool forceChange)
        {
            if (forceChange)
            {
                // TODO: FINISH CODE FOR SPECIFIC CASE SCENARIO
                animQueue.Clear();
                //animationPlayer?.Rewind();
                //animationClipDefault = animationClips[_key];
                //animationPlayer = parent.GetComponent<ModelAnimatedComponent>().PlayClip(animationClipDefault);
                
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
            if (animationPlayer != null)
            {
                animationPlayer.Looping = isLooping;

                if (animationPlayer.Position >= animationPlayer.Duration)
                {
                    isReady = true;

                    if (animQueue.Count > 0)
                    {
                        animationClip = animQueue.Dequeue();
                        animationPlayer = parent.GetComponent<ModelAnimatedComponent>().PlayClip(animationClip);
                    }
                    else
                    {
                        animationPlayer = parent.GetComponent<ModelAnimatedComponent>().PlayClip(animationClipDefault);
                    }
                }
            }
           
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