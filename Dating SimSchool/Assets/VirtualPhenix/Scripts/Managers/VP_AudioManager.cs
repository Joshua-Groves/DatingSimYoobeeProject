using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix
{
    public class VP_AudioManager : Helpers.HierarchyIcon
    {
        private static VP_AudioManager m_instance;
        public static VP_AudioManager Instance { get { return m_instance; } }

        [SerializeField] private bool m_destroyOnLoad = false;

        /// <summary>
        /// Dictionary of audio items
        /// </summary>
        private Dictionary<string, VP_AudioItem> m_audioDictionary;

        [Header("Add audios to the dictionary if they don't exist there")]
        [SerializeField] private bool m_addNonExistingItems = false;

        [Header("Common-used audios")]
        [SerializeField] private List<VP_AudioItem> m_predefinedAudios;

        [Header("One Shot Source")]
        [SerializeField] private AudioSource m_oneShotSource;
        [SerializeField] private AudioSource m_bgmSource;
        [SerializeField] private AudioSource m_sfxSource;
        [SerializeField] private AudioSource m_voiceSource;

        [Header("Volumes")]
        [SerializeField] private float m_masterVolume = 1.0f;
        [SerializeField] private float m_bgmVolume = 1.0f;
        [SerializeField] private float m_sfxVolume = 1.0f;
        [SerializeField] private float m_voiceVolume = 1.0f;

        bool m_initialized = false;

        public float MasterVolume { get { return m_masterVolume; } }
        public float BGMVolume { get { return m_bgmVolume; } }
        public float SfxVolume { get { return m_sfxVolume; } }
        public float VoiceVolume { get { return m_voiceVolume; } }
        public AudioSource OneShotSource { get { return m_oneShotSource; } }
        public AudioSource BGMSource { get { return m_bgmSource; } }
        public AudioSource SFXSource { get { return m_sfxSource; } }
        public AudioSource VoiceSource { get { return m_voiceSource; } }

        private void Awake()
        {
            if (m_instance == null)
            {
                m_instance = this;
                OnInit();

                if (!m_destroyOnLoad)
                    DontDestroyOnLoad(this.gameObject);

            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        private void OnInit()
        {
            if (m_initialized)
                return;


            m_audioDictionary = new Dictionary<string, VP_AudioItem>();
            if (m_predefinedAudios == null)
                m_predefinedAudios = new List<VP_AudioItem>();

            if (!m_bgmSource)
            {
                GameObject newSource = new GameObject("BGM Source");
                newSource.transform.parent = this.transform;
                m_bgmSource = newSource.AddComponent<AudioSource>();
            }

            if (!m_sfxSource)
            {
                GameObject newSource = new GameObject("SFX Source");
                newSource.transform.parent = this.transform;
                m_sfxSource = newSource.AddComponent<AudioSource>();
            }

            if (!m_voiceSource)
            {
                GameObject newSource = new GameObject("SFX Source");
                newSource.transform.parent = this.transform;
                m_voiceSource = newSource.AddComponent<AudioSource>();
            }

            if (!m_oneShotSource)
            {
                GameObject newSource = new GameObject("One Shot Source");
                newSource.transform.parent = this.transform;
                m_oneShotSource = newSource.AddComponent<AudioSource>();
            }

            foreach (VP_AudioItem item in m_predefinedAudios)
                m_audioDictionary.Add(item.m_key, item);

            m_initialized = true;

        }

        public void SetAllItemsToHalfVolume()
        {
            foreach (VP_AudioItem item in m_audioDictionary.Values)
            {
                item.m_source.volume *= 0.25f;
            }
        }

        public void SetAllItemsToRegularVolume()
        {
            foreach (VP_AudioItem item in m_audioDictionary.Values)
            {
                item.m_source.volume *= 4f;
            }
        }


        public void OverrideItem(VP_AudioItem _item)
        {
            m_audioDictionary[_item.m_key] = _item;
        }

        public static void FadeItemOut(string _item, float time, float _desiredVolume = 0f)
        {
            if (m_instance)
                m_instance.FadeOutItem(_item, time);
        }

        public void FadeOutItem(string _item, float time, float _desiredVolume = 0f)
        {
            if (m_audioDictionary.ContainsKey(_item))
            {
                StopCoroutine(FadeOutCor(m_audioDictionary[_item].m_source, time));
                StartCoroutine(FadeOutCor(m_audioDictionary[_item].m_source, time));
            }
        }

        IEnumerator FadeOutCor(AudioSource source, float time, float _desiredVolume = 0f)
        {
            float amount = source.volume / time;

            while (amount > _desiredVolume)
            {
                source.volume -= amount;
                yield return null;
            }


        }

        public void SetVolumesSettings(VP_AudioSettings settings, bool _save)
        {
            if (settings == null)
                return;

            m_bgmVolume = settings.m_bgmVolume;
            m_sfxVolume = settings.m_sfxVolume;
            m_voiceVolume = settings.m_voiceVolume;

            if (m_audioDictionary == null)
                OnInit();

            foreach (VP_AudioItem item in m_audioDictionary.Values)
            {
                if (item == null)
                    continue;

                switch (item.m_type)
                {
                    case VP_AudioSetup.AUDIO_TYPE.BGM:

                        item.m_source.volume = item.m_originalVolume * m_bgmVolume * m_masterVolume;
                        break;
                    case VP_AudioSetup.AUDIO_TYPE.SFX:

                        item.m_source.volume = item.m_originalVolume * m_sfxVolume * m_masterVolume;
                        break;
                    case VP_AudioSetup.AUDIO_TYPE.VOICE:
                        item.m_source.volume = item.m_originalVolume * m_voiceVolume * m_masterVolume;
                        break;
                }
            }
        }

        public void StopAllItems(VP_AudioSetup.AUDIO_TYPE _type)
        {
            if (m_audioDictionary == null || m_audioDictionary.Count == 0)
                return;

            foreach (VP_AudioItem item in m_audioDictionary.Values)
            {
                if (item == null || item.m_type != _type)
                    continue;

                switch (item.m_type)
                {
                    case VP_AudioSetup.AUDIO_TYPE.BGM:
                        item.m_source.volume = item.m_originalVolume * m_bgmVolume * m_masterVolume;
                        break;
                    case VP_AudioSetup.AUDIO_TYPE.SFX:

                        item.m_source.volume = item.m_originalVolume * m_sfxVolume * m_masterVolume;
                        break;
                    case VP_AudioSetup.AUDIO_TYPE.VOICE:
                        item.m_source.volume = item.m_originalVolume * m_voiceVolume * m_masterVolume;
                        break;
                }
            }
        }

        public float GetAudioLengthFromItem(string _item)
        {
            if (m_audioDictionary.ContainsKey(_item))
            {
                return m_audioDictionary[_item].GetAudioLength();
            }

            return 0f;
        }

        public void ClearNullAudios()
        {
            foreach (string key in m_audioDictionary.Keys)
            {
                if (m_audioDictionary[key] == null)
                {
                    m_audioDictionary.Remove(key);
                }
            }
        }

        public void RemoveItem(VP_AudioItem item)
        {
            if (m_predefinedAudios == null)
                return;

            if (m_predefinedAudios.Contains(item))
            {
                m_predefinedAudios.Remove(item);
            }

            if (m_audioDictionary.ContainsKey(item.m_key))
            {
                m_audioDictionary.Remove(item.m_key);
            }
        }

        public void RemoveItem(string item)
        {
            if (m_predefinedAudios == null)
                return;

            if (m_audioDictionary.ContainsKey(item))
            {
                m_audioDictionary.Remove(item);
            }
        }

        public void AddToPredefined(VP_AudioItem item)
        {
            if (m_predefinedAudios == null)
                return;

            if (!m_predefinedAudios.Contains(item))
            {
                m_predefinedAudios.Add(item);
            }
            else
            {
                Debug.LogError("AudioManager already contains that item.");
            }
        }

        public void StopAudioItem(string _key, bool _removeIt = false)
        {
            if (m_audioDictionary.ContainsKey(_key))
            {
                m_audioDictionary[_key].m_source.Stop();

                if (_removeIt)
                    m_audioDictionary.Remove(_key);
            }



        }

        public void PlayAudioItem(string _key)
        {
            if (m_audioDictionary.ContainsKey(_key))
            {
                m_audioDictionary[_key].Play();
            }

        }

        public void PlayItemOneShot(string _key)
        {
            if (m_audioDictionary.ContainsKey(_key))
            {
                m_audioDictionary[_key].PlayOneShot();
            }
        }

        public void PlayAndStopPrevious(AudioClip _clip, VP_AudioSetup.AUDIO_TYPE _type = VP_AudioSetup.AUDIO_TYPE.SFX, bool _overrideClip = false, float _volume = 1f, string _keyAdd = "", bool _addItForLater = false, bool _inLoop = false)
        {
            foreach (VP_AudioItem item in m_audioDictionary.Values)
            {
                if (item == null || item.m_type != _type)
                    continue;

                switch (item.m_type)
                {
                    case VP_AudioSetup.AUDIO_TYPE.BGM:

                        item.Stop();
                        break;
                    case VP_AudioSetup.AUDIO_TYPE.SFX:

                        item.Stop();
                        break;
                    case VP_AudioSetup.AUDIO_TYPE.VOICE:
                        item.Stop();
                        break;
                }
            }

            PlayAudio(_clip, _type, _overrideClip, _volume, _keyAdd, _addItForLater, _inLoop);
        }

        public void PlayAndStopPrevious(string _clipKey, VP_AudioSetup.AUDIO_TYPE _type = VP_AudioSetup.AUDIO_TYPE.SFX, float _volume = 1.0f)
        {
            foreach (VP_AudioItem item in m_audioDictionary.Values)
            {
                if (item == null || item.m_type != _type)
                    continue;

                switch (item.m_type)
                {
                    case VP_AudioSetup.AUDIO_TYPE.BGM:

                        item.Stop();
                        break;
                    case VP_AudioSetup.AUDIO_TYPE.SFX:

                        item.Stop();
                        break;
                    case VP_AudioSetup.AUDIO_TYPE.VOICE:
                        item.Stop();
                        break;
                }
            }

            if (m_audioDictionary.ContainsKey(_clipKey))
                m_audioDictionary[_clipKey].SetVolume(_volume);

            PlayAudioItem(_clipKey);
        }

        public void PlayAudio(AudioClip _clip, VP_AudioSetup.AUDIO_TYPE _type = VP_AudioSetup.AUDIO_TYPE.SFX, bool _overrideClip = false, float _volume = 1f, string _keyAdd = "", bool _addItForLater = false, bool _inLoop = false)
        {
            if (_clip == null)
                return;

            string _key = string.IsNullOrEmpty(_keyAdd) ? _clip.name : _keyAdd;
            if (m_audioDictionary.ContainsKey(_key))
            {
                m_audioDictionary[_key].Play();

            }
            else
            {
                GameObject newSource = new GameObject();
                newSource.transform.parent = this.transform;
                AudioSource source = newSource.AddComponent<AudioSource>();
                source.clip = _clip;

                VP_AudioItem audioItem = new VP_AudioItem(_key, _clip, source, _type, _volume, _overrideClip);

                switch (_type)
                {
                    case VP_AudioSetup.AUDIO_TYPE.BGM:
                        audioItem.m_source.volume = m_bgmVolume;
                        break;
                    case VP_AudioSetup.AUDIO_TYPE.SFX:
                        audioItem.m_source.volume = m_sfxVolume;
                        break;
                    case VP_AudioSetup.AUDIO_TYPE.VOICE:
                        audioItem.m_source.volume = m_voiceVolume;
                        break;

                }

                audioItem.Play();

                if (_addItForLater || m_addNonExistingItems)
                    m_audioDictionary.Add(_key, audioItem);
            }
        }

        public void PlayOneShot(AudioClip _clip, VP_AudioSetup.AUDIO_TYPE _type = VP_AudioSetup.AUDIO_TYPE.SFX, float _volume = 1f, string _keyAdd = "", bool _addItForLater = false)
        {
            if (m_oneShotSource)
            {
                m_oneShotSource.PlayOneShot(_clip);
            }
            else
            {
                GameObject newSource = new GameObject("Audio:" + _clip.name);
                newSource.transform.parent = this.transform;
                m_oneShotSource = newSource.AddComponent<AudioSource>();

                switch (_type)
                {
                    case VP_AudioSetup.AUDIO_TYPE.BGM:
                        m_oneShotSource.volume = m_bgmVolume * m_masterVolume;
                        break;
                    case VP_AudioSetup.AUDIO_TYPE.SFX:
                        m_oneShotSource.volume = m_sfxVolume * m_masterVolume;
                        break;
                }

                m_oneShotSource.PlayOneShot(_clip);
            }

            if (_addItForLater && !string.IsNullOrEmpty(_keyAdd))
            {
                if (!m_audioDictionary.ContainsKey(_keyAdd))
                    m_audioDictionary.Add(_keyAdd, new VP_AudioItem(_keyAdd, _clip, m_oneShotSource, _type, _volume, false));
            }
        }

        public void PlayOneShot(AudioClip _clip, VP_AudioSetup.AUDIO_TYPE _type = VP_AudioSetup.AUDIO_TYPE.SFX, bool _addIt = false, string _key = "")
        {
            if (m_oneShotSource)
            {
                m_oneShotSource.PlayOneShot(_clip);
            }
            else
            {
                GameObject newSource = new GameObject("Audio:" + _clip.name);
                newSource.transform.parent = this.transform;
                m_oneShotSource = newSource.AddComponent<AudioSource>();

                switch (_type)
                {
                    case VP_AudioSetup.AUDIO_TYPE.BGM:
                        m_oneShotSource.volume = m_bgmVolume * m_masterVolume;
                        break;
                    case VP_AudioSetup.AUDIO_TYPE.SFX:
                        m_oneShotSource.volume = m_sfxVolume * m_masterVolume;
                        break;
                }

                m_oneShotSource.PlayOneShot(_clip);
            }

            if (_addIt && !string.IsNullOrEmpty(_key))
            {
                AddAudioItem(new VP_AudioItem(_key, _clip, m_oneShotSource, _type, 1f, false));
            }
        }

        public void AddAudioItem(VP_AudioItem _item, bool _play = false)
        {

            switch (_item.m_type)
            {
                case VP_AudioSetup.AUDIO_TYPE.BGM:
                    _item.m_source.volume = m_bgmVolume * _item.m_originalVolume * m_masterVolume;
                    break;
                case VP_AudioSetup.AUDIO_TYPE.SFX:
                    _item.m_source.volume = m_sfxVolume * _item.m_originalVolume * m_masterVolume;
                    break;
                case VP_AudioSetup.AUDIO_TYPE.VOICE:
                    _item.m_source.volume = m_voiceVolume * _item.m_originalVolume * m_masterVolume;
                    break;
            }

            if (m_audioDictionary.ContainsKey(_item.m_key))
            {
                if (_play)
                    _item.Play();
                else
                    _item.Stop();
            }
            else
            {
                m_audioDictionary.Add(_item.m_key, _item);

                if (_play)
                    _item.Play();
                else
                    _item.Stop();
            }
        }

        public void SetDimensionOfAudioItem(string _key, float _dimValue)
        {
            if (AudioAlreadyExists(_key))
            {
                m_audioDictionary[_key].m_source.panStereo = _dimValue;
            }
        }

        public void SetPitchOfAudioItem(string _key, float _newPitch)
        {
            if (AudioAlreadyExists(_key))
            {
                m_audioDictionary[_key].m_source.pitch = _newPitch;
            }
        }

        public bool AudioAlreadyExists(string _key)
        {
            return m_audioDictionary.ContainsKey(_key);
        }

        /// <summary>
        /// Play an audio based on the audioName/key. That Audio is stored in the audioManager
        /// </summary>
        /// <param name="_audioName"></param>
        public void PlayAudio(string _audioName, VP_AudioSetup.AUDIO_TYPE _type = VP_AudioSetup.AUDIO_TYPE.SFX, bool _overrideClip = false, float _volume = 1f)
        {
            if (m_audioDictionary.ContainsKey(_audioName))
            {
                m_audioDictionary[_audioName].Play();
            }
            else
            {
                AudioClip audio = Resources.Load<AudioClip>(VP_AudioSetup.AUDIO_PATH + _audioName);

                if (audio)
                {
                    GameObject newSource = new GameObject();
                    newSource.transform.parent = this.transform;
                    AudioSource source = newSource.AddComponent<AudioSource>();
                    source.clip = audio;

                    VP_AudioItem audioItem = new VP_AudioItem(_audioName, audio, source, _type, _volume, _overrideClip);

                    switch (_type)
                    {
                        case VP_AudioSetup.AUDIO_TYPE.BGM:
                            audioItem.m_source.volume = m_bgmVolume;
                            break;
                        case VP_AudioSetup.AUDIO_TYPE.SFX:
                            audioItem.m_source.volume = m_sfxVolume;
                            break;
                    }

                    audioItem.Play();

                    if (m_addNonExistingItems)
                        m_audioDictionary.Add(_audioName, audioItem);
                }

            }
        }

        public void PlayOneShotAudioInSource(AudioClip _clip, VP_AudioSetup.AUDIO_TYPE _type, float _volume)
        {
            switch (_type)
            {
                case VP_AudioSetup.AUDIO_TYPE.BGM:
                    m_bgmSource.PlayOneShot(_clip, _volume);
                    break;
                case VP_AudioSetup.AUDIO_TYPE.SFX:
                    m_sfxSource.PlayOneShot(_clip, _volume);
                    break;
                case VP_AudioSetup.AUDIO_TYPE.VOICE:
                    m_voiceSource.PlayOneShot(_clip, _volume);
                    break;
            }
        }

        public void PlayAudioInSource(AudioClip _clip, VP_AudioSetup.AUDIO_TYPE _type)
        {
            switch (_type)
            {
                case VP_AudioSetup.AUDIO_TYPE.BGM:
                    m_bgmSource.Stop();
                    m_bgmSource.clip = _clip;
                    m_bgmSource.Play();
                    break;
                case VP_AudioSetup.AUDIO_TYPE.SFX:
                    m_sfxSource.Stop();
                    m_sfxSource.clip = _clip;
                    m_sfxSource.Play();
                    break;
                case VP_AudioSetup.AUDIO_TYPE.VOICE:
                    m_voiceSource.Stop();
                    m_voiceSource.clip = _clip;
                    m_voiceSource.Play();
                    break;
            }
        }

        public void StopAudioInSource(VP_AudioSetup.AUDIO_TYPE _type)
        {
            switch (_type)
            {
                case VP_AudioSetup.AUDIO_TYPE.BGM:
                    m_bgmSource.Stop();
                    break;
                case VP_AudioSetup.AUDIO_TYPE.SFX:
                    m_sfxSource.Stop();
                    break;
                case VP_AudioSetup.AUDIO_TYPE.VOICE:
                    m_voiceSource.Stop();
                    break;
            }
        }

        public void SetSourceVolume(VP_AudioSetup.AUDIO_TYPE _type, float _volume)
        {
            switch (_type)
            {
                case VP_AudioSetup.AUDIO_TYPE.BGM:
                    m_bgmSource.volume = _volume;
                    break;
                case VP_AudioSetup.AUDIO_TYPE.SFX:
                    m_sfxSource.volume = _volume;
                    break;
                case VP_AudioSetup.AUDIO_TYPE.VOICE:
                    m_voiceSource.volume = _volume;
                    break;
            }
        }

        public void StopAudioAndPlayInSource(VP_AudioSetup.AUDIO_TYPE _stopType, AudioClip _clip, VP_AudioSetup.AUDIO_TYPE _type)
        {
            StopAudioInSource(_stopType);
            PlayAudioInSource(_clip, _type);
        }

        public void StopAndPlayAudioItems(string _stopKey, string _playKey, bool _oneShot = false)
        {
            StopAudioItem(_stopKey);

            if (!_oneShot)
                PlayAudioItem(_playKey);
            else
                PlayItemOneShot(_playKey);
        }

        public void StopAndPlayAudios(string _stopKey, AudioClip _playClip, VP_AudioSetup.AUDIO_TYPE _type, bool _oneShot = false, bool _override = true, float _volume = 1f, string key = "", bool _add = true)
        {
            StopAudioItem(_stopKey);

            if (!_oneShot)
                PlayAudio(_playClip, _type, _override, _volume, key, _add);
            else
                PlayOneShot(_playClip, _type, _volume);
        }

        IEnumerator DestroyAfterPlay(float _time, string _key, GameObject _sourceObj)
        {
            float timer = 0;

            while (timer < _time)
            {
                yield return null;
            }

            if (m_audioDictionary.ContainsKey(_key))
                m_audioDictionary.Remove(_key);

            Destroy(_sourceObj);
        }



        public static void PlayAudioItemByKey(string _itemKey)
        {
            if (Instance)
                Instance.PlayAudioItem(_itemKey);
        }
        public static void StopAudioItembyKey(string _itemKey)
        {
            if (Instance)
                Instance.StopAudioItem(_itemKey);
        }
        public static void PlayAudioItemOneShotbyKey(string _itemKey)
        {
            if (Instance)
                Instance.PlayItemOneShot(_itemKey);
        }

        public static void StopAndPlayByKey(string _stopKey, string _playKey)
        {
            if (Instance)
            {
                Instance.StopAndPlayAudioItems(_stopKey, _playKey);
            }
        }

        public static void StopAndReplay(string _stopKey)
        {
            if (Instance)
            {
                Instance.StopAndPlayAudioItems(_stopKey, _stopKey);
            }
        }

        public static void StopAudioItembyKey(string _itemKey, bool _deleteIt = false)
        {
            if (Instance)
                Instance.StopAudioItem(_itemKey, _deleteIt);
        }

        public static void StopAllAudiosByType(VP_AudioSetup.AUDIO_TYPE _type)
        {
            if (Instance)
                Instance.StopAllItems(_type);
        }

        public static void PlayOneShot(AudioClip _clip, VP_AudioSetup.AUDIO_TYPE _type = VP_AudioSetup.AUDIO_TYPE.SFX, bool _overrideClip = false, float _volume = 1f, string _keyAdd = "", bool _addItForLater = false)
        {
            if (Instance)
            {
                Instance.PlayOneShot(_clip, _type, _volume, _keyAdd, _addItForLater);
            }
        }


        public static void PlayNewAudio(AudioClip _clip, VP_AudioSetup.AUDIO_TYPE _type = VP_AudioSetup.AUDIO_TYPE.SFX, bool _overrideClip = false, float _volume = 1f, string _keyAdd = "", bool _addItForLater = false)
        {
            if (Instance)
            {
                Instance.PlayAudio(_clip, _type, _overrideClip, _volume, _keyAdd, _addItForLater);
            }
        }

    }

}
