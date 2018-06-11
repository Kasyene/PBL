using System.Collections.Generic;
using Game.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using PBLGame.SceneGraph;

namespace Game.Components.Audio
{
    public class MusicManager : Component
    {
        private Dictionary<string, Song> songs = new Dictionary<string, Song>();
        public string currentKey { get; private set; }

        public MusicManager()
        {
            Volume = 0.1f;
        }

        public void PlaySong()
        {
            if(currentKey == null) return;
            MediaPlayer.Stop();
            MediaPlayer.Play(songs[currentKey]);
        }

        public void PlaySong(string _key)
        {
            if(!songs.ContainsKey(_key))
                return;
            currentKey = _key;
            MediaPlayer.Stop();
            MediaPlayer.Play(songs[_key]);
        }

        public void StopSong()
        {
            currentKey = null;
            MediaPlayer.Stop();
        }

        public void PauseSong()
        {
            MediaPlayer.Pause();
        }

        public bool IsRepeating
        {
            get => MediaPlayer.IsRepeating;
            set => MediaPlayer.IsRepeating = value;
        }

        public float Volume
        {
            get => MediaPlayer.Volume;
            set
            {
                if (value > 1.0f || value < 0.0f)
                {
                    if (value < 0.0f)
                        MediaPlayer.Volume = 0.0f;

                    if (value > 1.0f)
                        MediaPlayer.Volume = 1.0f;
                }
                else
                {
                    MediaPlayer.Volume = value;
                }    
            }
        }


        public override void LoadContent(ContentManager contentManager)
        {
            songs.Add("ambient", contentManager.Load<Song>("audio/music/5823"));
            base.LoadContent(contentManager);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}