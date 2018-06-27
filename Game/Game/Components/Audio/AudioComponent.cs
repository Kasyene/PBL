using System.Collections.Generic;
using Game.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using PBLGame.SceneGraph;

namespace Game.Components.Audio
{
    public class AudioComponent : Component
    {
        private Dictionary<string, SoundEffect> soundEffects = new Dictionary<string, SoundEffect>();
        private SoundEffectInstance soundInstance;
        private SoundEffectInstance soundInstanceLoop;
        private GameObject gameObject;

        private AudioListener listener = new AudioListener();
        private AudioEmitter emitter = new AudioEmitter();

        public string currentKey { get; private set; }

        private float _volume = 1.0f;

        public AudioComponent(GameObject _gameObject)
        {
            gameObject = _gameObject;
            LoadContent(Resources.Content);
        }

        public void AddSound(string _key, SoundEffect soundEffect)
        {
            if(soundEffects.ContainsKey(_key)) return;
            soundEffects.Add(_key,soundEffect);
        }

        public void AddSound(string _key, string _path)
        {
            if (soundEffects.ContainsKey(_key)) return;
            soundEffects.Add(_key, Resources.Content.Load<SoundEffect>(_path));
        }

        public void PlaySound(string _key)
        {
            if (!soundEffects.ContainsKey(_key)) return;
            soundInstance = soundEffects[_key].CreateInstance();
            soundInstance.Volume = Volume;
            soundInstance.Apply3D(listener,emitter);
            soundInstance.Play();
        }

        public void PlaySound2D(string _key)
        {
            if (!soundEffects.ContainsKey(_key)) return;
            soundInstance = soundEffects[_key].CreateInstance();
            soundInstance.Volume = Volume;
            soundInstance.Play();
        }

        public void PlaySoundLoop(string _key)
        {
            if(!soundEffects.ContainsKey(_key)) return;
            soundInstanceLoop = soundEffects[_key].CreateInstance();
            soundInstanceLoop.IsLooped = true;
            soundInstanceLoop.Volume = Volume;
            soundInstanceLoop.Apply3D(listener, emitter);
            soundInstanceLoop.Play(); 
        }

        public void StopSoundLoop()
        {
            soundInstanceLoop?.Stop();
        }

        public float Volume
        {
            get
            {
                return _volume;
            }
            set
            {
                if (value > 1.0f || value < 0.0f)
                {
                    if (value < 0.0f)
                        _volume = 0.0f;

                    if (value > 1.0f)
                        _volume = 1.0f;
                }
                else
                {
                    _volume = value;
                }
            }
        }

        public float MasterVolume
        {
            get
            {
                return SoundEffect.MasterVolume;
            }
            set
            {
                if (value > 1.0f || value < 0.0f)
                {
                    if (value < 0.0f)
                        SoundEffect.MasterVolume = 0.0f;

                    if (value > 1.0f)
                        SoundEffect.MasterVolume = 1.0f;
                }
                else
                {
                    SoundEffect.MasterVolume = value;
                }
            }
        }

        public override void LoadContent(ContentManager contentManager)
        {
            // TEST SOUND
            soundEffects.Add("roblox", contentManager.Load<SoundEffect>("audio/sfx/roblox_mono"));
            soundEffects.Add("hit", contentManager.Load<SoundEffect>("audio/sfx/hit_mono"));
            soundEffects.Add("attack1", contentManager.Load<SoundEffect>("audio/sfx/attack1"));
            soundEffects.Add("attack2", contentManager.Load<SoundEffect>("audio/sfx/attack2"));
            soundEffects.Add("attack3", contentManager.Load<SoundEffect>("audio/sfx/attack3"));
            soundEffects.Add("attack4", contentManager.Load<SoundEffect>("audio/sfx/attack4"));
            soundEffects.Add("attack5", contentManager.Load<SoundEffect>("audio/sfx/attack5"));
            soundEffects.Add("damage", contentManager.Load<SoundEffect>("audio/sfx/damage"));
            soundEffects.Add("jump", contentManager.Load<SoundEffect>("audio/sfx/jump"));
            soundEffects.Add("jumpSlow", contentManager.Load<SoundEffect>("audio/sfx/jump_slow"));
            soundEffects.Add("doors", contentManager.Load<SoundEffect>("audio/sfx/doors"));
            soundEffects.Add("cut", contentManager.Load<SoundEffect>("audio/sfx/cut"));
            soundEffects.Add("shoot", contentManager.Load<SoundEffect>("audio/sfx/shoot"));
            soundEffects.Add("teleport", contentManager.Load<SoundEffect>("audio/sfx/teleport"));
            soundEffects.Add("woosh", contentManager.Load<SoundEffect>("audio/sfx/woosh"));
            soundEffects.Add("timeSlow", contentManager.Load<SoundEffect>("audio/sfx/time_slow"));
            soundEffects.Add("timeSpeed", contentManager.Load<SoundEffect>("audio/sfx/time_speed"));


            base.LoadContent(contentManager);
        }

        public override void Update(GameTime gameTime)
        {
            if (gameObject != null || Resources.CameraVector3 != null)
            {
                Vector3 diff = gameObject.Position - Resources.CameraVector3;
                listener.Position = gameObject.Position / 1000;
                emitter.Position = Resources.CameraVector3 / 1000;
            }
            
            base.Update(gameTime);
        }
    }
}