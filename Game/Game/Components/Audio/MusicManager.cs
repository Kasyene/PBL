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
            Volume = 0.07f;
            LoadContent(Resources.Content);
        }

        public void AddSong(string _key, Song soundEffect)
        {
            if (songs.ContainsKey(_key)) return;
            songs.Add(_key, soundEffect);
        }

        public void AddSong(string _key, string _path)
        {
            if (songs.ContainsKey(_key)) return;
            songs.Add(_key, Resources.Content.Load<Song>(_path));
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
            get
            {
                return MediaPlayer.IsRepeating;
            }
            set
            {
                MediaPlayer.IsRepeating = value;
            }
        }

        public float Volume
        {
            get
            {
                return MediaPlayer.Volume;
            }
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
            songs.Add("5823", contentManager.Load<Song>("audio/music/5823"));
            songs.Add("fight", contentManager.Load<Song>("audio/music/ShroomFightMaster"));
            songs.Add("menu", contentManager.Load<Song>("audio/music/ShroomMenu"));
            base.LoadContent(contentManager);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}