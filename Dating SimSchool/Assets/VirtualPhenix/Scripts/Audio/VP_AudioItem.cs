using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix
{

    [System.Serializable]
    public class VP_AudioItem
    {
        public string m_key;
        public VP_AudioSetup.AUDIO_TYPE m_type;
        public AudioSource m_source;
        public AudioClip m_clip;
        public bool m_override;
        public float m_originalVolume = 1f;

        public VP_AudioItem()
        {

        }

        public VP_AudioItem(string _key, AudioClip _clip, AudioSource _source, VP_AudioSetup.AUDIO_TYPE _type, float _originalVolume, bool _overrideClip)
        {
            m_key = _key;
            m_type = _type;
            m_source = _source;
            m_originalVolume = _originalVolume;
            m_override = _overrideClip;
            m_clip = _clip;
            m_originalVolume = m_source.volume;
        }

        public void Play()
        {
            if (m_override && m_clip && m_clip != m_source.clip)
                m_source.clip = m_clip;

            m_source.Play();
        }

        public void PlayOneShot()
        {
            if (m_clip && m_source)
            {
                m_source.PlayOneShot(m_clip);
            }
        }

        public void Stop()
        {
            m_source.Stop();
        }

        public void Pause()
        {
            m_source.Pause();
        }

        public void Resume()
        {
            m_source.Play();
        }

        public bool IsPlaying()
        {
            return m_source.isPlaying;
        }

        public VP_AudioSetup.AUDIO_TYPE GetTYPE()
        {
            return m_type;
        }

        public void Mute()
        {
            m_source.mute = true;
        }

        public void Unmute()
        {
            m_source.mute = false;
        }

        public void SetVolume(float value)
        {
            m_source.volume = value;
        }

        public void SetLoop(bool value)
        {
            m_source.loop = value;
        }

        public float GetAudioLength()
        {
            return m_source.clip ? m_source.clip.length : 0f;
        }

        public void OverrideSourceClip()
        {
            m_override = true;

            if (m_source != null && m_clip != null)
                m_source.clip = m_clip;
        }
    }
}