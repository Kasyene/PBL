using System;
using System.Collections.Generic;
using AnimationAux;
using Game.Components.Audio;
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
        private bool isStoped = false;
        private bool isMultiplierDequeued = true;

        public float EstimatedTime { get; private set; }
        public int AnimID { get; private set; }
        private int previousAnimID = -1;

        public bool isReady { get; set; }

        public string defaultKey {get; private set; }

        private string currentKey;

        private Queue<string> animQueue = new Queue<string>();
        private Queue<float> multiplierQueue = new Queue<float>();

        private AnimationPlayer animationPlayer;

        private AnimationClip animationClip;
        private AnimationClip animationClipDefault;

        private AudioComponent audioComponent;

        private Dictionary<string, AnimationClip> animationClips = new Dictionary<string, AnimationClip>();

        public void SetDefaultAnimation(string _key)
        {
            animationClipDefault = animationClips[_key];
            defaultKey = _key;
        }

        public void PlayAnimation(string _key)
        {
            animQueue.Enqueue(_key);
            isReady = false;

            if (animationPlayer == null)
            {
                animationClipDefault = animationClips[_key];
                defaultKey = _key;
                animationPlayer = parent.GetComponent<ModelAnimatedComponent>().PlayClip(animationClipDefault);
            }
        }

        public void AnimationBreak()
        {
            animQueue.Clear();
            currentKey = null;
            animationPlayer?.Stop();
            AnimID = (AnimID + 1) % Int32.MaxValue;
        }

        public void AnimationStop()
        {
            isStoped = true;
        }

        public void AnimationStart()
        {
            isStoped = false;
        }

        public void PlayAnimation(string _key, bool forceChange)
        {
            if (forceChange)
            {
                // TODO: FINISH CODE FOR SPECIFIC CASE SCENARIO
                if (multiplierQueue.Count > 0)
                {
                    float tempPeek = multiplierQueue.Dequeue();
                    multiplierQueue.Clear();
                    multiplierQueue.Enqueue(tempPeek);
                }
                animQueue.Clear();
                animationPlayer.Stop();
                PlayAnimation(_key);
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

        public bool isCurrentAnimation(string _key)
        {
            return _key == currentKey;
        }

        public Dictionary<string, AnimationClip> GetAnimationDictionary()
        {
            return animationClips;
        }

        public void SetPlaybackMultiplier(float multiplier = 1.0f)
        {
            multiplierQueue.Enqueue(multiplier);
        }

        private void playAudio()
        {
            switch (animQueue.Peek())
            {
                case "slash":
                    audioComponent.PlaySound2D("attack1");
                    break;
                case "slashR":
                    audioComponent.PlaySound2D("attack2");
                    break;
                case "slashL":
                    audioComponent.PlaySound2D("attack3");
                    break;
                case "throw":
                    audioComponent.PlaySound2D("attack4");
                    break;
                default:
                    break;
            }
        }

        private void playDefaultAudio()
        {
            switch (defaultKey)
            {
                case "walk":
                    audioComponent.PlaySound2D("walk");
                    break;
                default:
                    break;
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (animationPlayer != null && !isStoped)
            {
                EstimatedTime = (animationPlayer.Duration - animationPlayer.Position) * animationPlayer.GetMultiplier();

                if (animationPlayer.Position >= animationPlayer.Duration || animationPlayer.Position < 0 )
                {
                    if (animationPlayer.Position < 0)
                    {
                        animationPlayer.SetMultiplier();
                    }

                    isMultiplierDequeued = false;

                    AnimID = (AnimID + 1) % Int32.MaxValue;

                    isReady = true;

                    if (animQueue.Count > 0)
                    {
                       playAudio();
                       currentKey = animQueue.Dequeue();
                       animationClip = animationClips[currentKey];
                       
                       animationPlayer = parent.GetComponent<ModelAnimatedComponent>().PlayClip(animationClip);
                    }
                    else
                    {
                       currentKey = null;
                       animationPlayer = parent.GetComponent<ModelAnimatedComponent>().PlayClip(animationClipDefault);
                        playDefaultAudio();
                    }
                }

                if (multiplierQueue.Count > 0)
                {
                    animationPlayer?.SetMultiplier(multiplierQueue.Dequeue());
                    isMultiplierDequeued = true;
                }
            }
           
            base.Update(gameTime);
        }

        public AnimationManager(GameObject _parent)
        {
            parent = _parent;
            audioComponent = new AudioComponent(parent);
        }

        AnimationManager(Dictionary<string, AnimationClip> _animationClips)
        {
            animationClips = _animationClips;
        }
    }
}