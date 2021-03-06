﻿using Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Localization;
using XNode;

namespace VirtualPhenix.Dialog
{
    public enum LISTEN_TIME
    {
        AWAKE,
        START
    }

    public class VP_DialogChart : HierarchyIcon
    {
        [Tooltip("If Dialogue Manager is in this scene, set this to Start or use Scripting Order to be sure to be executed after it.")]
        /// <summary>
        /// The chart can init automatically on start or awake depending on need
        /// </summary>
        public LISTEN_TIME m_InitChart = LISTEN_TIME.START;
        /// <summary>
        /// This chart graph. The dialog manager access the graph from this variable
        /// </summary>
        [SerializeField] private VP_DialogGraph m_graph = null;
        /// <summary>
        /// Dialogue used by the current chart. Different charts can use different dialogue messages :D
        /// </summary>
        [SerializeField] private VP_DialogMessage m_dialog = null;
        /// <summary>
        /// This chart canvas
        /// </summary>
        [SerializeField] private Transform m_chartChanvas = null;
        /// <summary>
        /// Which is the current node of the graph. Mainly used for debugging.
        /// </summary>
        [HideInInspector] public VP_Dialog m_currentNode;
        /// <summary>
        /// Did you correctly setup a start node?
        /// </summary>
        private bool hasStart;
        /// <summary>
        /// Wanna start the dialogue on init?
        /// </summary>
        public bool m_setChartOnStart = true;

        public VP_DialogGraph Graph { get { return m_graph; } set { m_graph = value; } }
        public VP_DialogMessage CurrentDialogMessage { get { return m_dialog; } set { m_dialog = value; } }
        public Transform ChartCanvas { get { return m_chartChanvas; } set { m_chartChanvas = value; } }

        private void Awake()
        {
          
            if (m_InitChart == LISTEN_TIME.AWAKE)
            {
                if (m_graph == null)
                {
                    if (m_setChartOnStart)
                    {
                        Debug.LogError("No Graph is assigned to the chart!");
                        return;
                    }
  
                }
                else
                {
                    Initialize();
                }
            }
 
        }

        private void Start()
        {
            if (m_InitChart == LISTEN_TIME.START)
            {
                if (m_graph == null)
                {
                    if (m_setChartOnStart)
                    {
                        Debug.LogError("No Graph is assigned to the chart!");
                        return;
                    }
                }
                else
                {
                    Initialize();
                }
            }

            // if we already initialized, we can start
            if (hasStart)
            {
                m_graph?.SetCurrentAfterInit(true);
            }
            
            m_currentNode = m_graph?.Current;
        }
        /// <summary>
        /// Init the data
        /// </summary>
        private void Initialize()
        {
#if UNITY_EDITOR
            if (m_graph == null)
            {
                UnityEditor.EditorUtility.DisplayDialog("GRAPH MISSING", "The dialog graph is missing from the dialog chart object. Please, set it from inspector.", "Ok");
                UnityEditor.EditorApplication.isPlaying = false;
            }
#endif

            m_graph.m_chart = this;

            if (m_setChartOnStart)
                VP_DialogManager.Instance.SelectCurrentChart(this);

            if (m_dialog != null)
                VP_DialogManager.Instance.SetThisDialog(m_dialog, m_chartChanvas);

            hasStart = m_graph.HasOnStartEvent();
        }

#if UNITY_EDITOR
        /// <summary>
        /// When exiting the game, we delete everything. Just on
        /// </summary>
        private void OnApplicationQuit()
        {
            if (m_currentNode)
                m_currentNode.IsCurrent = false;

            m_currentNode = null;
            if (m_graph)
                m_graph.SetCurrent(null);
        }
#endif
        /// <summary>
        /// Set data from context
        /// </summary>
        public void SetIntanceData(Transform _canvas, VP_DialogMessage _msg)
        {
            this.m_chartChanvas = _canvas;
            this.m_dialog = _msg;
        }

        public List<VP_DialogLog> GetRegisteredLogs()
        {
            return m_graph ? m_graph.GetAllRegisteredDialogs() : null;
        }

        public void RegisterLog(VP_DialogCharacterData _character, string _text)
        {
            if (m_graph != null)
                m_graph.RegisterDialog(_character, _text);
        }

        public void ClearRegisteredLogs()
        {
            if(m_graph != null)
                m_graph.DeleteAllRegisteredDialogs();
        }

        public void DeleteAllRegisteredDialogsByCharacterName(string _characterName)
        {
            if (m_graph != null)
                DeleteAllRegisteredDialogsByCharacterName(_characterName);
        }

        public void DeleteRegisteredDialog(int _index)
        {
            if (m_graph != null)
                m_graph.DeleteRegisteredDialog(_index);
        }

        public VP_DialogLog GetRegisteredDialog(int _index)
        {
              return (m_graph != null) ? m_graph.GetRegisteredDialog(_index) : null;
        }

        public List<VP_DialogLog> GetAllRegisteredDialogsByCharacterName(string _characterName)
        {
            return m_graph != null ? m_graph.GetAllRegisteredDialogsByCharacterName(_characterName) : null;
        }


        public void SetThisChartToDialogueManager()
        {
            VP_DialogManager.SetCurrentChart(this);
        }
        /// <summary>
        /// Send the message to the graph
        /// </summary>
        /// <param name="_message"></param>
        public void SendDialogMessage(string _message)
        {
            if (m_graph)
                m_graph.StartDialog(_message);
            else
                Debug.LogError("No Graph is in the current chart, so dialog can't be started with KEY. Are you trying direct text?");
        }
        /// <summary>
        /// Trigger a new dialog with the setup data from the graph
        /// </summary>
        /// <param name="dialog"></param>
        public void TriggerDialog(VP_Dialog dialog)
        {
            if (dialog == null)
                return;

            if (dialog.dialog != null)
            {
                GameObject go = Instantiate(dialog.dialog.gameObject, m_chartChanvas);
                go.SetActive(false);
                VP_DialogMessage msg = go.GetComponent<VP_DialogMessage>();
                VP_DialogManager.Instance.SetThisDialog(msg, m_chartChanvas);
            }
            else if (VP_DialogManager.Instance.DialogMessage != m_dialog)
            {
                VP_DialogManager.Instance.SetThisDialog(m_dialog, m_chartChanvas);
            }
      
            bool ret = false;

            ret = dialog.hideAfter ? true : CheckIfIsLastDialog(dialog, dialog);

            TMPro.TMP_FontAsset font = dialog.characterData != null && dialog.characterData.m_customFont != null ? dialog.characterData.m_customFont : dialog.font;
            string parsedTxt = dialog.m_useLocalization ? VP_LocalizationManager.GetText(dialog.key) : dialog.key;

            Color c = dialog.color;
            if (dialog.characterData != null && dialog.characterData.m_overrideTextColor)
            {
                c = dialog.characterData.m_characterDefaultTextColor;
            }

            float fontSize = dialog.m_overideFontSize ? 45f : dialog.m_fontSize;
       
            // register
            if (dialog.m_registerDialog)
                RegisterLog(dialog.characterData, parsedTxt);

            VP_DialogMessageData messageData = new VP_DialogMessageData(dialog.clip, dialog.dialogType, parsedTxt, dialog.characterData, ret, dialog.m_fadeInOut, dialog.m_canSkipWithInput,
             dialog.m_showDirectly, dialog.m_textSpeed, dialog.m_fadeDuration, dialog.waitForAudioEnd, dialog.answers, dialog.m_answerAtTheSameTime, dialog.m_autoAnswer, dialog.m_positionData, c, font,
             dialog.waitForInput, dialog.m_fontSize, dialog.m_timeForAnswer, false, Vector3.zero, false, dialog.m_soundOnContinue, dialog.overrideColor);
            VP_DialogManager.ShowMessage(this, messageData);
        }
        /// <summary>
        /// We need to check if this node is the end of a conversation. We do it recursively
        /// </summary>
        /// <param name="dialogToCheck"></param>
        /// <param name="dialog"></param>
        /// <returns></returns>
        public bool CheckIfIsLastDialog(VP_Dialog dialogToCheck, VP_DialogBaseNode dialog)
        {
            if (dialogToCheck.output == null || (dialogToCheck.output is VP_DialogEvent && dialogToCheck.output == null))
                return true;

            if (dialog is VP_Dialog && dialog != dialogToCheck)
                return false;

            if (dialog.output == null)
            {
                if (dialog is VP_Dialog)
                {
                    return false;
                }
                else
                {
                    if (dialog is VP_DialogBranch)
                    {
                        if ((dialog as VP_DialogBranch).IsSuccess())
                        {
                            if ((dialog as VP_DialogBranch).isTrue != null)
                            {
                                return CheckIfIsLastDialog(dialogToCheck, ((dialog as VP_DialogBranch).isTrue));
                            }
                        }
                        else
                        {
                            if ((dialog as VP_DialogBranch).isFalse != null)
                            {
                                return CheckIfIsLastDialog(dialogToCheck, ((dialog as VP_DialogBranch).isFalse));
                            }
                        }
                    }
                    else if (dialog is VP_DialogMultipleOutputs)
                    {
                        List<VP_DialogBaseNode> list = (dialog as VP_DialogMultipleOutputs).outputs;
                        foreach (VP_DialogBaseNode outp in list)
                        {
                            if (outp != null)
                            {
                                return CheckIfIsLastDialog(dialogToCheck, outp);
                            }
                        }
                    }

                    return true;
                }
            }

            return CheckIfIsLastDialog(dialogToCheck, dialog.output);
        }
        /// <summary>
        /// Continue with index = 0; > Output by default
        /// </summary>
        /// <param name="index"></param>
        public void ContinueText()
        {
            m_graph.ContinueText();
        }

        /// <summary>
        /// Continue of specific index. Called from DialogManager
        /// </summary>
        /// <param name="index"></param>
        public void AnswerText(int index)
        {
            m_graph.ContinueText(index);
        }
    }

}
