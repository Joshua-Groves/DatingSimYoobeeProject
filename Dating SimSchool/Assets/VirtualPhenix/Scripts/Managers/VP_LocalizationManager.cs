/* 
 This file is part of MIS-SIM
 
> Copyright (C) 2018 Manuel Rodríguez Matesanz
> Contact mail: mrodriguez@gbt.tfo.upm.es
>
*/
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using System.IO;
using System.Linq;

using System;
using VirtualPhenix.Dialog;

namespace VirtualPhenix.Localization
{

    /// <summary>
    /// Text Manager now just have ONE language stored
    /// </summary>
    public class VP_LocalizationManager : Helpers.HierarchyIcon
    {
        #region Singleton
        private static VP_LocalizationManager m_instance;
        public static VP_LocalizationManager Instance { get { return m_instance; } }

        #endregion

        #region Variables
        [Header("Configuration"), Space(10)]
        [SerializeField] private bool m_destroyOnLoad = false;
        private bool m_initialized = false;


        [Header("Languages"), Space(10)]
        [SerializeField] private bool m_canChangeLanguage = false;
        /// <summary>
        /// Current language - Will change if pressing language button
        /// </summary>
        [SerializeField] private SystemLanguage m_currentLanguage = SystemLanguage.Spanish;
        [HideInInspector] private List<SystemLanguage> m_activeLanguages;
        /// <summary>
        /// Dictionary of dictionaries with every text of every language
        /// </summary>
        private Dictionary<string, VP_TextItem> m_texts;
        private Dictionary<string, TextAsset> languageFiles;

        [HideInInspector] private int m_languageIndex = 0;

        public static InvokableCallback<SystemLanguage> translationCallback;

        public SystemLanguage[] ActiveLanguages { get { return m_activeLanguages.ToArray(); } }
        public SystemLanguage CurrentLanguage { get { return m_currentLanguage; } }
        public string CurrentLanguagename { get { return m_currentLanguage.ToString(); } }
        #endregion

        #region Monobehaviour and Initialization

        protected void Awake()
        {
            if (m_instance == null)
            {
                m_instance = this;
                if (!m_destroyOnLoad)
                    DontDestroyOnLoad(this);
                InitTexts();
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        void InitTexts()
        {
            if (m_initialized)
                return;

            m_activeLanguages = new List<SystemLanguage>();
            m_texts = new Dictionary<string, VP_TextItem>();

            StartLanguages();

            if (m_canChangeLanguage)
            {
                int last = PlayerPrefs.GetInt(VP_LocalizationSetup.PlayerPrefs.LAST_LANGUAGE + "_" + Application.productName, -1);
                m_currentLanguage = last != -1 ? m_activeLanguages[last] : Application.systemLanguage;
            }
            else
            {
                m_currentLanguage = Application.systemLanguage;
            }


            if (!m_activeLanguages.Contains(m_currentLanguage))
            {
                Debug.Log("Active language set to english due to the lack of "+m_currentLanguage.ToString());
                m_currentLanguage = SystemLanguage.English;
                m_languageIndex = 0;
            }
            else
            {
                for (int i = 0; i < m_activeLanguages.Count; i++)
                {
                    if (m_activeLanguages[i] == m_currentLanguage)
                    {
                        m_languageIndex = i;
                        break;
                    }
                }
            }
            m_initialized = true;
            ChangeLanguage(m_currentLanguage, false);
        }

        #endregion

        #region Language & Parse methods

        public void NextLanguage()
        {
            if (m_canChangeLanguage)
            {
                m_languageIndex = (m_languageIndex + 1 < m_activeLanguages.Count) ? m_languageIndex + 1 : 0;
                m_currentLanguage = m_activeLanguages[m_languageIndex];
                ChangeLanguage(m_currentLanguage, true);
            }
        }

        public void PreviousLanguage()
        {
            if (m_canChangeLanguage)
            {
                m_languageIndex = (m_languageIndex - 1 >= 0) ? m_languageIndex - 1 : m_activeLanguages.Count-1;
                m_currentLanguage = m_activeLanguages[m_languageIndex];
                ChangeLanguage(m_currentLanguage, true);
            }
        }

        void StartLanguages()
        {
            string extension = "";
            string folder = "";
            extension = VP_LocalizationSetup.Extension.CSV;
            folder = VP_LocalizationSetup.Folder.LOCALIZATION_FOLDER_CSV;
            languageFiles = SetAllLanguages(extension, folder);
        }
       
        bool ExistLanguage(string folder, string _extension, string _language)
        {
            return File.Exists(folder + _language + _extension);
        }

        Dictionary<string, TextAsset> SetAllLanguages(string _extension, string folder)
        {
            Dictionary<string, TextAsset> languageAssets = new Dictionary<string, TextAsset>();

            TextAsset[] assets = Resources.LoadAll<TextAsset>(folder);
            foreach (TextAsset asset in assets)
            {
                if (asset.name != VP_LocalizationSetup.DEFAULT_FILE_NAME)
                {
                    languageAssets.Add(asset.name, asset);

                    SystemLanguage lang;
                    if (VP_CustomParser.TryParse(asset.name, out lang))
                    {
                        m_activeLanguages.Add(lang);
                    }
                }
            }

            if (m_activeLanguages.Count == 0 && assets.Length > 0)
            {
                // Only default
                foreach (TextAsset asset in assets)
                {
                    if (asset.name == VP_LocalizationSetup.DEFAULT_FILE_NAME)
                    {
                        languageAssets.Add("English", asset);
                        SystemLanguage lang = SystemLanguage.English;
                        m_activeLanguages.Add(lang);
                    }
                }               
            }

            return languageAssets;
        }

        bool ParseCSV()
        {
            m_texts.Clear();
            bool canParse = false;
            bool parsed = false;
            string language = m_currentLanguage.ToString();

            canParse = languageFiles.ContainsKey(language);

            if (canParse)
            {
                TextAsset csv = languageFiles[language];
                m_texts = VP_CSVParser.ParseCSV(csv);
            }
            else
            {
                m_currentLanguage = SystemLanguage.English;
                language = m_currentLanguage.ToString();
                canParse = languageFiles.ContainsKey(language);
                if (canParse)
                {
                    TextAsset csv = languageFiles[language];
                    m_texts = VP_CSVParser.ParseCSV(csv);
                }
            }
            parsed = canParse && (m_texts.Count > 0);

            return parsed;
        }

        public void TranslateTexts(SystemLanguage _lang)
        {
            ChangeLanguage(_lang, false);
        }

        /// <summary>
        /// Method for changing the language. Calling this method will launch translate event so every text shows correctly
        /// </summary>
        /// <param name="language"></param>
        public void ChangeLanguage(SystemLanguage language, bool translateTexts = true)
        {
            if (!m_initialized)
            {
                InitTexts();
                return;
            }

            SystemLanguage oldLang = m_currentLanguage;
            m_currentLanguage = language;

            bool parsed = ParseCSV();

            if (!parsed)
                m_currentLanguage = oldLang;
            else
                PlayerPrefs.SetInt(VP_LocalizationSetup.PlayerPrefs.LAST_LANGUAGE+"_"+Application.productName, m_languageIndex);

            if (translateTexts)
            {
                if (translationCallback != null)
                    translationCallback.Invoke();
            }
        }

        /// <summary>
        /// Get the text which key is defined in pb_SetupTexts
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetTextTranslated(string key, bool showLog = true)
        {
            if (key == null || m_texts == null || !m_texts.ContainsKey(key))
            {
                if (showLog)
                    Debug.LogError("Couldn't get a translated text for " + key + " in language " + m_currentLanguage.ToString());

                return "ERROR [404]";
            }

            return m_texts[key].Text;
        }

        public bool CanTranslateText(string _key)
        {
            return (!string.IsNullOrEmpty(_key) && m_texts != null && m_texts.ContainsKey(_key));
        }

        public static string GetText(string key, bool showLog = true)
        {
            if (!Instance)
                return "ERROR [404]";

            return Instance.GetTextTranslated(key, showLog);
        }

        public static bool CanTranslate(string _key)
        {
            if (!Instance)
                return false;

            return Instance.CanTranslateText(_key);
        }

        public static SystemLanguage GetCurrentLanguage()
        {
            if (!Instance)
                return SystemLanguage.English;

            return Instance.CurrentLanguage;
        } 
#endregion
    }

}