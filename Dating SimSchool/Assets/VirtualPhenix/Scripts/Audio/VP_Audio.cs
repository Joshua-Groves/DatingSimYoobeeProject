using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix
{
    public class VP_Audio : MonoBehaviour
    {
        [Header("Audio Item")]
        public bool m_useAudioManager = true;
        private VP_AudioItem m_audio;

        [Header("Initialization")]   
        public string m_audioKey;
        public VP_AudioSetup.AUDIO_TYPE m_type;
        public AudioClip m_clip;
        public AudioSource m_source;
        public bool m_playOnInit = false;
        public bool m_inLoop = false;
        public bool m_setClip = false;

        [Range(0, 1)] public float m_initialVolume = 1.0f;

        public VP_AudioItem Audio { get { return m_audio; } }
        public float Length { get { return m_audio.GetAudioLength(); } }

        private void Awake()
        {

        }

        // Start is called before the first frame update
        void Start()
        {

            if (m_useAudioManager)
            {
                if (string.IsNullOrEmpty(this.m_audioKey) || VP_AudioManager.Instance.AudioAlreadyExists(m_audioKey))
                {
                    Debug.LogError("NO AUDIO ITEM KEY OR ALREADY IN DICTIONARY. KEY: " + m_audioKey);
                    this.gameObject.SetActive(false);
                    return;
                }
            }

            if (m_source == null)
            {
                GameObject _childSource = new GameObject();
                _childSource.transform.SetParent(this.transform);
                m_source = _childSource.AddComponent<AudioSource>();
            }

            m_source.loop = m_inLoop;
            m_source.playOnAwake = m_playOnInit;

            m_audio = new VP_AudioItem(m_audioKey, m_clip, m_source, m_type, m_initialVolume, m_setClip);
            if (m_useAudioManager)
                VP_AudioManager.Instance.AddAudioItem(m_audio, m_playOnInit);
        }

        public void CreateAudio()
        {
            m_audio = new VP_AudioItem();
        }

        private void OnDestroy()
        {
            if (m_useAudioManager)
            {
                if (VP_AudioManager.Instance && !VP_MonobehaviourSettings.Quiting)
                    VP_AudioManager.Instance.RemoveItem(m_audio);
            }
        }

        public void Play()
        {
            m_audio.Play();
        }

        public void Stop()
        {
            m_audio.Stop();
        }
    }

}
