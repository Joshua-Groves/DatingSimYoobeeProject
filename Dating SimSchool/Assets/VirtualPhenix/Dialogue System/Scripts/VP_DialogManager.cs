using Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VirtualPhenix.Dialog;

namespace VirtualPhenix
{
    [System.Serializable]
    public class VP_DialogDirectMessage
    {
        [SerializeField] private VP_DialogMessageData m_data;
        [SerializeField] private VP_DialogMessage m_customMessage;

        private System.Action m_onDialogCompleteCallback;
        private System.Action m_onDialogEndCallback;
        private System.Action m_onDialogStartCallback;
        private System.Action m_onTextShownCallback;
        private List<VP_Dialog.Answer> m_answers;
        private List<Action> m_answerCallback;
        private Action<int> m_chooseNumberCallback;
        private Action<int> m_onAnswerChosen;
        private Action m_cancelNumberCallback = null;

        public VP_DialogMessageData Data { get { return m_data; } }
        public VP_DialogMessage CustomMessage { get { return m_customMessage; } }

        public VP_DialogDirectMessage(VP_DialogMessage _customMessage, VP_DialogMessageData _messageData, System.Action _onDialogStart = null, System.Action _onTextShownCallback = null, System.Action _onDialogCompleteCallback = null,  System.Action _onDialogEnd = null,
             System.Action<int> _onNumberChosen = null, System.Action <int> _onAnswerChosen = null, List<System.Action> _answerActions = null, System.Action _onCancelCallback = null)
        {
            m_data = _messageData;
            m_customMessage = _customMessage;

            this.m_answerCallback = new List<Action>();
            if (_answerActions != null)
            {
                int i = 0;
                VP_DialogManager.StartListeningOnChoiceSelection(OnChoiceSelected);
                foreach (Action a in _answerActions)
                {
                    if (a != null)
                    {
                        m_answerCallback.Add(a);
                        //m_data.m_answers[i].m_callback = a;
                    }
                    i++;
                }
            }
            m_onAnswerChosen = _onAnswerChosen;
            m_cancelNumberCallback = _onCancelCallback;
            m_onDialogStartCallback = _onDialogStart;
            m_onTextShownCallback = _onTextShownCallback;
            m_onDialogCompleteCallback = _onDialogCompleteCallback;
            m_onDialogEndCallback = _onDialogEnd;
            m_chooseNumberCallback = _onNumberChosen;
            m_onAnswerChosen = _onAnswerChosen;

            if (m_onDialogCompleteCallback != null)
                VP_DialogManager.StartListeningToOnDialogComplete(OnNodeCompleteInvoke);

            if (m_onTextShownCallback != null)
                VP_DialogManager.StartListeningToOnTextShown(OnTextShownInvoke);

            if (m_onDialogStartCallback != null)
                VP_DialogManager.StartListeningToOnDialogStart(OnStartInvoke);

            if (m_data.m_chooseNumber)
            {
                if (m_chooseNumberCallback != null)
                    VP_DialogManager.StartListeningToOnChooseNumber(OnNumberChosen);

                if (m_data.m_canCancel && m_cancelNumberCallback != null)
                    VP_DialogManager.StartListeningToOnNumberCancel(OnNumberCancel);
            }

            if (m_onDialogEndCallback != null)
                VP_DialogManager.StartListeningToOnDialogEnd(OnEndInvoke);
        }

        void StopListening()
        {
            if (m_onDialogCompleteCallback != null)
                VP_DialogManager.StopListeningToOnDialogComplete(OnNodeCompleteInvoke);

            if (m_onTextShownCallback != null)
                VP_DialogManager.StopListeningToOnTextShown(OnTextShownInvoke);

            if (m_onDialogStartCallback != null)
                VP_DialogManager.StopListeningToOnDialogStart(OnStartInvoke);

            if (m_onDialogEndCallback != null)
                VP_DialogManager.StopListeningToOnDialogEnd(OnEndInvoke);

            if (m_chooseNumberCallback != null)
                VP_DialogManager.StopListeningToOnChooseNumber(OnNumberChosen);

            if (m_cancelNumberCallback != null)
                VP_DialogManager.StopListeningToOnNumberCancel(OnNumberCancel);

            if (m_answerCallback != null && m_answerCallback.Count > 0 )
                VP_DialogManager.StopListeningOnChoiceSelection(OnChoiceSelected);
        }

        void OnChoiceSelected(int _index)
        {
            StopListening();

            if (m_answerCallback.Count > _index && m_answerCallback[_index] != null)
            {
                m_answerCallback[_index].Invoke();
            }
        }

        void OnNumberCancel()
        {
            StopListening();

            if (m_cancelNumberCallback != null)
                m_cancelNumberCallback.Invoke();
        }

        void OnNumberChosen(int _number)
        {
            StopListening();

            if (m_chooseNumberCallback != null)
                m_chooseNumberCallback.Invoke(_number);
        }

        void OnNodeCompleteInvoke()
        {
            StopListening();

            if (m_onDialogCompleteCallback != null)
                m_onDialogCompleteCallback.Invoke();
        }

        void OnStartInvoke()
        {
            if (m_onDialogStartCallback != null)
                m_onDialogStartCallback.Invoke();
        }

        void OnTextShownInvoke()
        {
            if (m_onTextShownCallback != null)
                m_onTextShownCallback.Invoke();
        }


        void OnNumberShownInvoke()
        {
            if (m_onTextShownCallback != null)
                m_onTextShownCallback.Invoke();
        }

        void OnEndInvoke()
        {
            if (m_onDialogEndCallback != null)
                m_onDialogEndCallback.Invoke();
        }
    }

    public class VP_DialogManager : HierarchyIcon
    {
        #region Singleton
        /// <summary>
        /// Singleton instance. Called by VP_DialogManager.Instance
        /// </summary>
        private static VP_DialogManager m_instance;
        public static VP_DialogManager Instance { get { return m_instance; } }
        [SerializeField] private bool m_destroyOnLoad = false;

        #endregion

        [Header("Current Chart")]
        /// <summary>
        /// Current Chart with a referenced graph
        /// </summary>
        [SerializeField] private VP_DialogChart m_currentChart;

        [Header("Dialog Prefab")]
        /// <summary>
        /// Prefab instantiated if it doesn't already exists in scene when ShowMessage method is called
        /// </summary>
        [SerializeField] private GameObject m_dialogPrefab = null;

        [Header("Talking Dialog")]
        [SerializeField] private bool m_usingDirectMessage;

        [Header("Default Dialog and Canvas - In case there's null")]
        [SerializeField] private VP_DialogMessage m_defaultDialogMessage;
        [SerializeField] private Transform m_defaultCanvas;


        [Header("Current Dialog and Canvas")]
        [SerializeField] private bool m_deactivateCanvasOnFadeOut = true;
 
        /// <summary>
        /// Current dialog message 
        /// </summary>
        [SerializeField] private VP_DialogMessage m_talkingDialog;
        /// <summary>
        /// Current canvas where the dialog prefab is instantiated if needed
        /// </summary>
        [SerializeField] private Transform m_dialogCanvas;
        [SerializeField] private bool m_alwaysParentDialogWithCanvas = true;

        [Header("Properties")]
        /// <summary>
        /// Checker that any dialogue graph is being played
        /// </summary>
        public bool m_speaking = false;

        [Header("Global Dialogue Variables")]
        [SerializeField] private VP_VariableDataBase m_globalDatabase = new VP_VariableDataBase();
        /// <summary>
        /// Queue of direct messages
        /// </summary>
        private Queue<VP_DialogDirectMessage> m_directMessages;
        private bool m_queueBusy { get { return m_directMessages.Count > 0 || m_speaking; } }
        private bool m_queueActive { get { return m_directMessages.Count > 0; } }
        /// <summary>
        /// Callback for sequence
        /// </summary>
        public static Action OnDialogCompleteForOutput;
        /// <summary>
        /// Callback when a node is completed. it is not just dialog node.
        /// </summary>
        private static Action OnDialogComplete;
        /// <summary>
        /// Callback called when the full text is displayed in the dialog bubble
        /// </summary>
        private static Action OnTextShown;
        /// <summary>
        /// Callback called when the current bubble is starting to be displayed
        /// </summary>
        private static Action OnDialogStart;
        /// <summary>
        /// Callback called when the answers are displayed
        /// </summary>
        private static Action OnAnswerShow;
        /// <summary>
        /// Callback called when choose number is displayed
        /// </summary>
        private static Action OnNumberShow;
        /// <summary>
        /// Callback called when the dialogue is ended. Fully ended.
        /// </summary>
        private static Action OnDialogEnd;
        /// <summary>
        /// Callback called when the user selects specific choice
        /// </summary>
        private static Action<int> OnChoiceSelection;
        /// <summary>
        /// Callback called when the user selects number
        /// </summary>
        private static Action<int> OnNumberChosen;
        /// <summary>
        /// Pressed B/Cancel Input when number is chosen 
        /// </summary>
        private static Action OnNumberCancel;
        /// <summary>
        /// Callback called when the log is starting to be recorded. 
        /// </summary>
        private static Action m_onRegisteredAble;
        /// <summary>
        /// Callback called on end or start so the user can hide the log button
        /// </summary>
        private static Action m_onRegisteredDisable;
        /// <summary>
        /// Callback called when character is speaking. This is used for VP_DialogAnimator so the user can see when a specific character is speaking.
        /// </summary>
        private static Action<VP_DialogCharacterData> OnCharacterSpeak;
        /// <summary>
        /// Callback called from DialogAnimator so you can use that transform for IK look at
        /// </summary>
        private static Action<Transform> OnAnimationTarget;
        /// <summary>
        /// Callback called when pressing "Skip"
        /// </summary>
        private static Action OnSkip;
        /// <summary>
        /// Interact Button -> If you don't want to use VP_EventManager
        /// </summary>
        private static Action OnDialogInteract;
        /// <summary>
        /// Up Button -> If you don't want to use VP_EventManager
        /// </summary>
        private static Action OnDialogUp;
        /// <summary>
        /// Down Button -> If you don't want to use VP_EventManager
        /// </summary>
        private static Action OnDialogDown;
        /// <summary>
        /// Right Button -> If you don't want to use VP_EventManager
        /// </summary>
        private static Action OnDialogRight;
        /// <summary>
        /// Left Button -> If you don't want to use VP_EventManager
        /// </summary>
        private static Action OnDialogLeft;
        /// <summary>
        /// Cancel button -> same as interact but if any of the options have select if cancelled 
        /// or number selection can be cancelled, this can be used.
        /// </summary>
        private static Action OnDialogCancel;
        /// <summary>
        /// Use this callback to select specific answer without calling VP_DialogMessage.Answer
        /// </summary>
        private static Action<int> OnExternalAnswerSelected;
        /// <summary>
        /// Callback called when the auto-time for answers starts
        /// </summary>
        private static Action OnAutoAnswerTimeStart;
        /// <summary>
        /// Callback called when the auto-time for answers ends
        /// </summary>
        private static Action OnAutoAnswerTimeEnd;
        /// <summary>
        /// Properties
        /// </summary>
        public VP_DialogMessage DialogMessage { get { return m_talkingDialog; } }
        public VP_DialogChart CurrentChart { get { return m_currentChart; } set { m_currentChart = value; } }
        public Transform CurrentCanvas { get { return m_dialogCanvas; } set { m_dialogCanvas = value; } }
        public static bool IsSpeaking { get { return m_instance && m_instance.m_speaking; } }
        public VP_VariableDataBase GlobalVariables { get { return m_globalDatabase; } }
        public VP_VariableDataBase GraphVariables { get { return m_instance.GraphVariables; } }
        public static bool IsBusy { get { return Instance.m_queueBusy; } }
        public static bool IsQueueActive { get { return Instance.m_queueActive; } }
        public int OnChoiceSelectionCount { get { return OnChoiceSelection != null ? OnChoiceSelection.GetInvocationList().Length : 0; } }

        /// <summary>
        /// Initialization
        /// </summary>
        protected void Awake()
        {
            if (m_instance == null)
            {
                m_instance = this;
                Init();
                if (!m_destroyOnLoad)
                    DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        void Init()
        {

            m_directMessages = new Queue<VP_DialogDirectMessage>();
            if (m_defaultCanvas == null)
            {
                if (m_currentChart && m_currentChart.ChartCanvas)
                {
                    m_defaultCanvas = m_currentChart.ChartCanvas;
                }
                else if (m_dialogCanvas != null)
                {
                    m_defaultCanvas = m_dialogCanvas;
                }
                else
                {
                    m_defaultCanvas = transform.GetChild(0);
                }
            }

            if (m_defaultDialogMessage == null)
            {
                m_defaultDialogMessage = (VP_DialogMessage)FindObjectOfType(typeof(VP_DialogMessage));
            }

            if (m_dialogPrefab == null)
                m_dialogPrefab = Resources.Load<GameObject>("Dialogue/Prefabs/Dialog/Dialog");

        
        }

        public void SetInitialData(VP_DialogMessage _chartmsg, Transform _canvas, VP_DialogChart _chart)
        {
            if (!m_currentChart)
                m_currentChart = _chart;

            if (!m_talkingDialog)
                m_talkingDialog = _chartmsg;

            if (!m_defaultCanvas)
                m_defaultCanvas = _canvas;

            if (!m_dialogCanvas)
                m_dialogCanvas = _canvas;

            if (!m_defaultDialogMessage)
                m_defaultDialogMessage = _chartmsg;
        }

        public void SetDefaultData()
        {
            if (m_defaultCanvas == null)
            {
                if (m_currentChart && m_currentChart.ChartCanvas)
                {
                    m_defaultCanvas = m_currentChart.ChartCanvas;
                }
                else if (m_dialogCanvas != null)
                {
                    m_defaultCanvas = m_dialogCanvas;
                }
                else
                {
                    m_defaultCanvas = transform.GetChild(0);
                }
            }

            // Assigned again to avoid warning
            if (m_dialogPrefab == null)
                m_dialogPrefab = Resources.Load<GameObject>("Dialogue/Prefabs/Dialog/Dialog");

            if (m_defaultDialogMessage == null)
            {
                m_defaultDialogMessage = (VP_DialogMessage)FindObjectOfType(typeof(VP_DialogMessage));
            }
        }

        #region Listen To Callbacks



        public static void StartListeningToOnNumberShow(Action _actionCalled)
        {
            if (OnNumberShow != null)
                OnNumberShow += _actionCalled;
            else
                OnNumberShow = _actionCalled;
        }


        public static void StopListeningToOnNumberShow(Action _actionCalled)
        {
            if (OnNumberShow != null)
                OnNumberShow -= _actionCalled;
        } 
        
        
        public static void StartListeningToOnDialogInteract(Action _actionCalled)
        {
            if (OnDialogInteract != null)
                OnDialogInteract += _actionCalled;
            else
                OnDialogInteract = _actionCalled;
        }


        public static void StopListeningToOnDialogInteract(Action _actionCalled)
        {
            if (OnDialogInteract != null)
                OnDialogInteract -= _actionCalled;
        }

        public static void StartListeningToOnNumberCancel(Action _actionCalled)
        {
            if (OnNumberCancel != null)
                OnNumberCancel += _actionCalled;
            else
                OnNumberCancel = _actionCalled;
        }


        public static void StopListeningToOnNumberCancel(Action _actionCalled)
        {
            if (OnNumberCancel != null)
                OnNumberCancel -= _actionCalled;
        }

        public static void StartListeningToOnDialogUp(Action _actionCalled)
        {
            if (OnDialogUp != null)
                OnDialogUp += _actionCalled;
            else
                OnDialogUp = _actionCalled;
        }


        public static void StopListeningToOnDialogUp(Action _actionCalled)
        {
            if (OnDialogUp != null)
                OnDialogUp -= _actionCalled;
        }

        public static void StartListeningToOnDialogRight(Action _actionCalled)
        {
            if (OnDialogRight != null)
                OnDialogRight += _actionCalled;
            else
                OnDialogRight = _actionCalled;
        }

        public static void StopListeningToOnDialogRight(Action _actionCalled)
        {
            if (OnDialogRight != null)
                OnDialogRight -= _actionCalled;
        }

        public static void StartListeningToOnDialogLeft(Action _actionCalled)
        {
            if (OnDialogLeft != null)
                OnDialogLeft += _actionCalled;
            else
                OnDialogLeft = _actionCalled;
        }

        public static void StopListeningToOnDialogLeft(Action _actionCalled)
        {
            if (OnDialogLeft != null)
                OnDialogLeft -= _actionCalled;
        }

        public static void StartListeningToOnDialogCancel(Action _actionCalled)
        {
            if (OnDialogCancel != null)
                OnDialogCancel += _actionCalled;
            else
                OnDialogCancel = _actionCalled;
        }

        public static void StopListeningToOnDialogCancel(Action _actionCalled)
        {
            if (OnDialogCancel != null)
                OnDialogCancel -= _actionCalled;
        }

        public static void StartListeningToOnDialogDown(Action _actionCalled)
        {
            if (OnDialogDown != null)
                OnDialogDown += _actionCalled;
            else
                OnDialogDown = _actionCalled;
        }


        public static void StopListeningToOnDialogDown(Action _actionCalled)
        {
            if (OnDialogDown != null)
                OnDialogDown -= _actionCalled;
        }

        public static void StartListeningToOnAutoAnswerTimeStart(Action _actionCalled)
        {
            if (OnAutoAnswerTimeStart != null)
                OnAutoAnswerTimeStart += _actionCalled;
            else
                OnAutoAnswerTimeStart = _actionCalled;
        }


        public static void StopListeningToOnAutoAnswerTimeStart(Action _actionCalled)
        {
            if (OnAutoAnswerTimeStart != null)
                OnAutoAnswerTimeStart -= _actionCalled;
        }

        public static void StartListeningToOnAutoAnswerTimeEnd(Action _actionCalled)
        {
            if (OnAutoAnswerTimeEnd != null)
                OnAutoAnswerTimeEnd += _actionCalled;
            else
                OnAutoAnswerTimeEnd = _actionCalled;
        }


        public static void StopListeningToOnAutoAnswerTimeEnd(Action _actionCalled)
        {
            if (OnAutoAnswerTimeEnd != null)
                OnAutoAnswerTimeEnd -= _actionCalled;
        }

        public static void StartListeningToOnExternalAnswerSelected(Action<int> _actionCalled)
        {
            if (OnExternalAnswerSelected != null)
                OnExternalAnswerSelected += _actionCalled;
            else
                OnExternalAnswerSelected = _actionCalled;
        }


        public static void StopListeningToOnExternalAnswerSelected(Action<int> _actionCalled)
        {
            if (OnExternalAnswerSelected != null)
                OnExternalAnswerSelected -= _actionCalled;
        }

        public static void StartListeningToOnSkip(Action _actionCalled)
        {
            if (OnSkip != null)
                OnSkip += _actionCalled;
            else
                OnSkip = _actionCalled;
        }


        public static void StopListeningToOnSkip(Action _actionCalled)
        {
            if (OnSkip != null)
                OnSkip -= _actionCalled;
        }

        public static void StartListeningToOnSpeakingTargetTransform(Action<Transform> _actionCalled)
        {
            if (OnAnimationTarget != null)
                OnAnimationTarget += _actionCalled;
            else
                OnAnimationTarget = _actionCalled;
        }


        public static void StopListeningToOnSpeakingTargetTransform(Action<Transform> _actionCalled)
        {
            if (OnAnimationTarget != null)
                OnAnimationTarget -= _actionCalled;
        }

        public static void StartListeningToOnCharacterSpeak(Action<VP_DialogCharacterData> _actionCalled)
        {
            if (OnCharacterSpeak != null)
                OnCharacterSpeak += _actionCalled;
            else
                OnCharacterSpeak = _actionCalled;
        }


        public static void StopListeningToOnCharacterSpeak(Action<VP_DialogCharacterData> _actionCalled)
        {
            if (OnCharacterSpeak != null)
                OnCharacterSpeak -= _actionCalled;
        }

        public static void StartListeningToOnRegisterDialogAble(Action _actionCalled)
        {
            if (m_onRegisteredAble != null)
                m_onRegisteredAble += _actionCalled;
            else
                m_onRegisteredAble = _actionCalled;
        }


        public static void StopListeningToOnRegisterDialogAble(Action _actionCalled)
        {
            if (m_onRegisteredAble != null)
                m_onRegisteredAble -= _actionCalled;
        }

        public static void StartListeningToOnRegisterDialogDisable(Action _actionCalled)
        {
            if (m_onRegisteredDisable != null)
                m_onRegisteredDisable += _actionCalled;
            else
                m_onRegisteredDisable = _actionCalled;
        }


        public static void StopListeningToOnRegisterDialogDisable(Action _actionCalled)
        {
            if (m_onRegisteredDisable != null)
                m_onRegisteredDisable -= _actionCalled;
        }

        public static void StartListeningToOnDialogCompleteForOutput(Action _actionCalled)
        {
            if (OnDialogCompleteForOutput != null)
                OnDialogCompleteForOutput += _actionCalled;
            else
                OnDialogCompleteForOutput = _actionCalled;
        }


        public static void StopListeningToOnDialogCompleteForOutput(Action _actionCalled)
        {
            if (OnDialogCompleteForOutput != null)
                OnDialogCompleteForOutput -= _actionCalled;
        }

        public static void StartListeningToOnDialogComplete(Action _actionCalled)
        {
            if (OnDialogComplete != null)
                OnDialogComplete += _actionCalled;
            else
                OnDialogComplete = _actionCalled;
        }

        public static void StopListeningToOnDialogComplete(Action _actionCalled)
        {
            if (OnDialogComplete != null)
                OnDialogComplete -= _actionCalled;
        }

        public static void StartListeningToOnAnswerShow(Action _actionCalled)
        {
            if (OnAnswerShow != null)
                OnAnswerShow += _actionCalled;
            else
                OnAnswerShow = _actionCalled;
        }

        public static void StopListeningToOnAnswerShow(Action _actionCalled)
        {
            if (OnAnswerShow != null)
                OnAnswerShow -= _actionCalled;
        }

        public static void StartListeningOnChoiceSelection(Action<int> _actionCalled)
        {
            if (OnChoiceSelection != null)
                OnChoiceSelection += _actionCalled;
            else
                OnChoiceSelection = _actionCalled;
        }

        public static void StopListeningOnChoiceSelection(Action<int> _actionCalled)
        {
            if (OnChoiceSelection != null)
                OnChoiceSelection -= _actionCalled;
        }



        public static void StartListeningToOnTextShown(Action _actionCalled)
        {
            if (OnTextShown != null)
                OnTextShown += _actionCalled;
            else
                OnTextShown = _actionCalled;
        }

        public static void StopListeningToOnTextShown(Action _actionCalled)
        {
            if (OnTextShown != null)
                OnTextShown -= _actionCalled;

        }

        public static void StartListeningToOnChooseNumber(Action<int> _actionCalled)
        {
            if (OnNumberChosen != null)
                OnNumberChosen += _actionCalled;
            else
                OnNumberChosen = _actionCalled;
        }


        public static void StopListeningToOnChooseNumber(Action<int> _actionCalled)
        {
            if (OnNumberChosen != null)
                OnNumberChosen -= _actionCalled;
        }

        public static void StartListeningToOnDialogStart(Action _actionCalled)
        {
            if (OnDialogStart != null)
                OnDialogStart += _actionCalled;
            else
                OnDialogStart = _actionCalled;
        }


        public static void StopListeningToOnDialogStart(Action _actionCalled)
        {
            if (OnDialogStart != null)
                OnDialogStart -= _actionCalled;
        }  

        public static void StartListeningToOnDialogEnd(Action _actionCalled)
        {
            if (OnDialogEnd != null)
                OnDialogEnd += _actionCalled;
            else
                OnDialogEnd = _actionCalled;
        }


        public static void StopListeningToOnDialogEnd(Action _actionCalled)
        {
            if (OnDialogEnd != null)
                OnDialogEnd -= _actionCalled;
        }

        #endregion

        #region Callback Clear
        public static void ClearOnAutoAnswerTimeStartAction()
        {
            OnAutoAnswerTimeStart = null;
        }    
        
        public static void ClearOnNumberShowAction()
        {
            OnNumberShow = null;
        }

        public static void ClearOnAutoAnswerTimeEndAction()
        {
            OnAutoAnswerTimeEnd = null;
        }

        public static void ClearOnExternalAnswerSelectedCallback(int _selection)
        {
            OnExternalAnswerSelected = null;
        }

        public static void ClearOnCharacterSpeakAction(VP_DialogCharacterData _character)
        {
            OnCharacterSpeak = null;
        }

        public static void ClearOnAnimationTargetAction(Transform _target)
        {
            OnAnimationTarget = null;
        }

        public static void ClearOnDialogRegisterDisableAction()
        {
            m_onRegisteredDisable = null;
        }

        public static void ClearOnDialogRegisterAbleAction()
        {
            m_onRegisteredAble = null;
        }

        public static void ClearOnDialogCompleteOutputAction()
        {
            OnDialogCompleteForOutput = null;
        }
        public static void ClearOnDialogCompleteAction()
        {
            OnDialogComplete = null;
        }

        public static void ClearOnAnswerShowAction()
        {
            OnAnswerShow = null;
        }

        public static void ClearOnDialogStartAction()
        {
            OnDialogStart = null;
        }

        public static void ClearOnDialogEndAction()
        {
            OnDialogEnd = null;
        }

        public static void ClearOnTextShownAction()
        {
            OnTextShown = null;
        }

        public static void ClearOnSkipAction()
        {
            OnSkip = null;
        }

        public static void ClearOnDialogInteractAction()
        {
            OnDialogInteract = null;
        }

        public static void ClearOnDialogUpAction()
        {
            OnDialogUp = null;
        }

        public static void ClearOnDialogDownAction()
        {
            OnDialogDown = null;
        }
        
        public static void ClearOnDialogRightAction()
        {
            OnDialogRight = null;
        }

        public static void ClearOnDialogLeftAction()
        {
            OnDialogLeft = null;
        }

        public static void ClearOnDialogCancelAction()
        {
            OnDialogCancel = null;
        }

        public static void ClearOnNumberChosenAction(int _chosenNumber)
        {
            OnNumberChosen = null;
        }

        public static void ClearOnNumberCancelAction()
        {
            OnNumberCancel = null;
        }
        #endregion

        #region Callback Invoke

        public static void OnAutoAnswerTimeStartAction()
        {
            if (OnAutoAnswerTimeStart != null)
                OnAutoAnswerTimeStart.Invoke();
        }
         public static void OnNumberShowAction()
        {
            if (OnNumberShow != null)
                OnNumberShow.Invoke();
        }

        public static void OnAutoAnswerTimeEndAction()
        {
            if (OnAutoAnswerTimeEnd != null)
                OnAutoAnswerTimeEnd.Invoke();
        }

        public static void OnExternalAnswerSelectedCallback(int _selection)
        {
            if (OnExternalAnswerSelected != null)
                OnExternalAnswerSelected.Invoke(_selection);
        }

        public static void OnCharacterSpeakAction(VP_DialogCharacterData _character)
        {
            if (OnCharacterSpeak != null)
                OnCharacterSpeak.Invoke(_character);
        }

        public static void OnAnimationTargetAction(Transform _target)
        {
            if (OnAnimationTarget != null)
                OnAnimationTarget.Invoke(_target);
        }

        public static void OnDialogRegisterDisableAction()
        {
            if (m_onRegisteredDisable != null)
                m_onRegisteredDisable.Invoke();
        }

        public static void OnDialogRegisterAbleAction()
        {
            if (m_onRegisteredAble != null)
                m_onRegisteredAble.Invoke();
        }

        public static void OnDialogCompleteOutputAction()
        {
            if (OnDialogCompleteForOutput != null)
                OnDialogCompleteForOutput.Invoke();
        }
        /// <summary>
        /// When the dialog branch ends, it calls a callback so the sequence can be performanced
        /// </summary>
        public static void OnDialogCompleteAction()
        {
            if (OnDialogComplete != null)
                OnDialogComplete.Invoke();
        }

        public static void OnAnswerShowAction()
        {
            if (OnAnswerShow != null)
                OnAnswerShow.Invoke();
        }

        public static void OnDialogStartAction()
        {
            if (OnDialogStart != null)
                OnDialogStart.Invoke();
        }

        public static void OnDialogEndAction()
        {
            if (OnDialogEnd != null)
                OnDialogEnd.Invoke();
        }

        public static void OnTextShownAction()
        {
            if (OnTextShown != null)
                OnTextShown.Invoke();
        }

        public void SelectCurrentAnswer()
        {
            DialogMessage.AnswerCurrent();
        }

        public static void SelectCurrentOption()
        {
            if (m_instance)
                m_instance.SelectCurrentAnswer();
        }

        public static void OnSkipAction()
        {
            if (OnSkip != null)
                OnSkip.Invoke();
        }

        public static void OnDialogInteractAction()
        {
            if (OnDialogInteract != null)
                OnDialogInteract.Invoke();
        }

        public static void OnDialogUpAction()
        {
            if (OnDialogUp != null)
                OnDialogUp.Invoke();
        }

        public static void OnDialogDownAction()
        {
            if (OnDialogDown != null)
                OnDialogDown.Invoke();
        }

        public static void OnDialogRightAction()
        {
            if (OnDialogRight != null)
                OnDialogRight.Invoke();
        }

        public static void OnDialogLeftAction()
        {
            if (OnDialogLeft != null)
                OnDialogLeft.Invoke();
        }

        public static void OnDialogCancelAction()
        {
            if (OnDialogCancel != null)
                OnDialogCancel.Invoke();
        }

        public static void OnNumberChosenAction(int _chosenNumber)
        {
            if (OnNumberChosen != null)
                OnNumberChosen.Invoke(_chosenNumber);
        }

        public static void OnNumberCancelAction()
        {
            if (OnNumberCancel != null)
                OnNumberCancel.Invoke();
        }

        #endregion

        #region Variables
        /// <summary>
        /// Get variable if exits in the dictionary
        /// </summary>
        /// <param name="_varName"></param>
        /// <returns></returns>
        public FieldData GetVariable<T>(string _varName, T _type)
        {
            return VP_Utils.DialogUtils.GetVariableValueFromDatabase(_varName, _type, m_globalDatabase);
        }
        /// <summary>
        /// Get variable if exits in the dictionary
        /// </summary>
        /// <param name="_varName"></param>
        /// <returns></returns>
        public FieldData GetVariableFromStringType(string _varName, string _type)
        {
            return VP_Utils.DialogUtils.GetVariableValueStrTypeFromDatabase(_varName, _type, m_globalDatabase);
        }
        public string GetVariableStringValue<T>(string _varName, T _type)
        {
            return VP_Utils.DialogUtils.GetVariableValueStringFromDatabase(_varName, _type, m_globalDatabase);
        }
        public string GetVariableStringValueFromStringType(string _varName, string _type)
        {
            return VP_Utils.DialogUtils.GetVariableValueStringFromStrTypeFromDatabase(_varName, _type, m_globalDatabase);
        }
        /// <summary>
        /// Set a custom generic variable, a value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_name"></param>
        /// <param name="value"></param>
        public void SetVariable<T>(string _name, T value)
        {
            VP_Utils.DialogUtils.SetVariableToDatabase(_name, value, m_globalDatabase);
        }

        public void SetGlobalVariable<T>(string _name, T value)
        {
            VP_Utils.DialogUtils.SetVariableToDatabase(_name, value, m_globalDatabase);
        }
        public void SetGraphVariable<T>(string _name, T value)
        {
            VP_Utils.DialogUtils.SetVariableToDatabase(_name, value, m_currentChart?.Graph?.GraphVariables);
        }
        #endregion

        public FieldData GetGraphVariable<T>(string _varName, T _type)
        {
            return m_currentChart?.Graph?.GetVariableValue(_varName, _type);
        }
        public FieldData GetGraphVariableFromStringType(string _varName, string _type)
        {
            return m_currentChart?.Graph?.GetVariableFromStringType(_varName, _type);
        }
        public string GetGraphVariableStringValue<T>(string _varName, T _type)
        {
            return m_currentChart?.Graph?.GetVariableStringValue(_varName, _type);
        }
        public string GetGraphVariableStringValueFromStringType(string _varName, string _type)
        {
            return m_currentChart?.Graph?.GetVariableStringValueFromStringType(_varName, _type);
        }

        

        public List<VP_DialogLog> GetRegisteredLogs()
        {
            return m_currentChart ? m_currentChart.GetRegisteredLogs() : null;
        }

        void NextDirectMessageAfterGraph()
        {
            NextDirectMessage(false);
            StopListeningToOnDialogEnd(NextDirectMessageAfterGraph);
        }

        public void DirectMessage(string _message, AudioClip _clip = null, DIALOG_TYPE _type = DIALOG_TYPE.REGULAR, bool _translate = false, bool showDirectly = false, bool fadeInOut = true, VP_DialogMessage _customDialogMessage = null,
            System.Action _onCompleteCallback = null, Action _onStartCallback = null, Action _onEndCallback = null, Action _onTextShown = null, VP_DialogPositionData _pos = null,
            bool _skippable = true, bool waitForInput = true, float _duration = 0.5f, bool _soundOnContinue = true, VP_DialogCharacterData _character = null, float _textSpeed = 1f, bool _waitForAudioEnd = true, 
            List<VP_Dialog.Answer> _answers = null, bool _showAnswersSameTime = true, int _autoAnswer = -1, bool _overrideTextColor = false, Color textColor = default(Color), TMPro.TMP_FontAsset _font = null, float _fontSize = 45f,
             float _timeForAnswer = 5f, bool _chooseNumber = false, Vector3 _parameters = default(Vector3), bool _canCancel = true)
        {
            VP_DialogDirectMessage directmsg = new VP_DialogDirectMessage(_customDialogMessage, new VP_DialogMessageData(_clip, _type, _message, _character, IsQueueActive, fadeInOut, _skippable, showDirectly, _textSpeed, _duration, _waitForAudioEnd, _answers,
                _showAnswersSameTime, _autoAnswer, _pos, textColor, _font, waitForInput, _fontSize, _timeForAnswer, _chooseNumber, _parameters, _canCancel, _soundOnContinue, _overrideTextColor), _onStartCallback, _onTextShown, _onCompleteCallback, _onEndCallback);

            if (m_queueBusy)
            {
                // if we are using graph, we need to trigger after it shows up
                if (!m_usingDirectMessage)
                {
                    StartListeningToOnDialogEnd(NextDirectMessageAfterGraph);
                }

                m_directMessages.Enqueue(directmsg);
            }
            else
            {
                InitDirectMessage(directmsg);
            }
        }

        public void DirectMessageWithOptions(string _message, AudioClip _clip = null, DIALOG_TYPE _type = DIALOG_TYPE.REGULAR, List<VP_Dialog.Answer> _answers = null, bool _showAnswersSameTime = true, int _autoAnswer = -1, bool _translate = false, 
           VP_DialogMessage _customDialogMessage = null, System.Action _onCompleteCallback = null, Action _onStartCallback = null, Action _onEndCallback = null, Action _onTextShown = null, 
            VP_DialogPositionData _pos = null, bool _skippable = true, bool waitForInput = true, float _duration = 0.5f, bool showDirectly = false, bool fadeInOut = true, bool _soundOnContinue = true, VP_DialogCharacterData _character = null, float _textSpeed = 1f, bool _waitForAudioEnd = true,
            bool _overrideTextColor = false, Color textColor = default(Color), TMPro.TMP_FontAsset _font = null, float _fontSize = 45f,
             float _timeForAnswer = 5f, bool _chooseNumber = false, Vector3 _parameters = default(Vector3), bool _canCancel = true)
        {
            VP_DialogDirectMessage directmsg = new VP_DialogDirectMessage(_customDialogMessage, new VP_DialogMessageData(_clip, _type, _message, _character, IsQueueActive, fadeInOut, _skippable, showDirectly, _textSpeed, _duration, _waitForAudioEnd, _answers,
               _showAnswersSameTime, _autoAnswer, _pos, textColor, _font, waitForInput, _fontSize, _timeForAnswer, _chooseNumber, _parameters, _canCancel, _soundOnContinue, _overrideTextColor), _onStartCallback, _onTextShown, _onCompleteCallback, _onEndCallback);

            if (m_queueBusy)
            {

                m_directMessages.Enqueue(directmsg);
            }
            else
            {
                InitDirectMessage(directmsg);
            }
        }

        public GameObject InstantiatePrefab(GameObject _prefab, Transform _parent)
        {
            return Instantiate(_prefab, _parent);
        }

        public GameObject InstantiatePrefab(GameObject _prefab, Vector3 _position, Quaternion _rotation)
        {
            return Instantiate(_prefab, _position, _rotation);
        }

        public void InitDirectMessageAndGraph(VP_DialogDirectMessage directmsg)
        {
            if (m_talkingDialog != null)
                m_talkingDialog.gameObject.SetActive(false);

            VP_DialogMessage customMsg = directmsg.CustomMessage;

            m_talkingDialog = (customMsg != null) ? customMsg : (m_talkingDialog == null ? m_defaultDialogMessage : m_talkingDialog);

            if (m_dialogCanvas == null)
                m_dialogCanvas = m_defaultCanvas;

            if (m_alwaysParentDialogWithCanvas && m_talkingDialog.transform.parent != m_dialogCanvas)
                m_dialogCanvas = m_talkingDialog.transform.parent;

            if (m_deactivateCanvasOnFadeOut)
                m_dialogCanvas.gameObject.SetActive(true);

            m_usingDirectMessage = true;

            ShowMessage(m_currentChart, directmsg.Data);
        }

        public void InitDirectMessage(VP_DialogDirectMessage directmsg)
        {
            if (m_talkingDialog != null)
                m_talkingDialog.gameObject.SetActive(false);

            VP_DialogMessage customMsg = directmsg.CustomMessage;

            m_talkingDialog = (customMsg != null) ? customMsg : (m_talkingDialog == null ? m_defaultDialogMessage : m_talkingDialog);

            if (m_dialogCanvas == null)
                m_dialogCanvas = m_defaultCanvas;

            if (m_alwaysParentDialogWithCanvas && m_talkingDialog.transform.parent != m_dialogCanvas)
                m_dialogCanvas = m_talkingDialog.transform.parent;

            if (m_deactivateCanvasOnFadeOut)
                m_dialogCanvas.gameObject.SetActive(true);

            m_usingDirectMessage = true;

            ShowMessage(null, directmsg.Data);
        }
        /// <summary>
        /// Manual choose numbers
        /// </summary>
        /// <param name="parameters">
        /// x = Min
        /// y = Max
        /// z = default
        /// </param>
        public void ChooseNumber(string _message, AudioClip _clip = null, DIALOG_TYPE _type = DIALOG_TYPE.REGULAR, Vector3 _parameters = default(Vector3), bool _translate = false,
           VP_DialogMessage _customDialogMessage = null, System.Action _onCompleteCallback = null, Action _onStartCallback = null, Action _onEndCallback = null, Action _onTextShown = null,
            VP_DialogPositionData _pos = null, bool _skippable = true, bool waitForInput = true, float _duration = 0.5f, bool showDirectly = false, bool fadeInOut = true, bool _soundOnContinue = true, 
            VP_DialogCharacterData _character = null, float _textSpeed = 1f, bool _waitForAudioEnd = true, bool _overrideTextColor = false, Color textColor = default(Color), TMPro.TMP_FontAsset _font = null, float _fontSize = 45f,
             float _timeForAnswer = 5f, bool _canCancel = true, List<VP_Dialog.Answer> _answers = null, bool _showAnswersSameTime = true, int _autoAnswer = -1)
        {
            VP_DialogDirectMessage directmsg = new VP_DialogDirectMessage(_customDialogMessage, new VP_DialogMessageData(_clip, _type, _message, _character, IsQueueActive, fadeInOut, _skippable, showDirectly, _textSpeed, _duration, _waitForAudioEnd, _answers,
               _showAnswersSameTime, _autoAnswer, _pos, textColor, _font, waitForInput, _fontSize, _timeForAnswer, true, _parameters, _canCancel, _soundOnContinue, _overrideTextColor), _onStartCallback, _onTextShown, _onCompleteCallback, _onEndCallback);

            if (m_queueBusy)
            {
                m_directMessages.Enqueue(directmsg);
            }
            else
            {
                InitDirectMessage(directmsg);
            }
        }


        public void ChooseNumberInGraph(string _message, Vector3 _parameters = default(Vector3), System.Action<int> _onNumberSelection = null, bool _canCancel = true, System.Action _onCancelCallback = null,  DIALOG_TYPE _type = DIALOG_TYPE.REGULAR, AudioClip _clip = null,
            bool _translate = false, VP_DialogMessage _customDialogMessage = null, System.Action _onCompleteCallback = null, Action _onStartCallback = null, Action _onEndCallback = null, Action _onTextShown = null,
            VP_DialogPositionData _pos = null, bool _skippable = true, bool waitForInput = true, float _duration = 0.5f, bool showDirectly = false, bool fadeInOut = true, bool _soundOnContinue = true,
            VP_DialogCharacterData _character = null, float _textSpeed = 1f, bool _waitForAudioEnd = true, bool _overrideTextColor = false, Color textColor = default(Color), TMPro.TMP_FontAsset _font = null, float _fontSize = 45f,
             float _timeForAnswer = 5f,List<VP_Dialog.Answer> _answers = null, bool _showAnswersSameTime = true, int _autoAnswer = -1)
        {
            VP_DialogDirectMessage directmsg = new VP_DialogDirectMessage(_customDialogMessage, new VP_DialogMessageData(_clip, _type, _message, _character, IsQueueActive, fadeInOut, _skippable, showDirectly, _textSpeed, _duration, _waitForAudioEnd, _answers,
               _showAnswersSameTime, _autoAnswer, _pos, textColor, _font, waitForInput, _fontSize, _timeForAnswer, true, _parameters, _canCancel, _soundOnContinue, _overrideTextColor), _onStartCallback, _onTextShown, _onCompleteCallback, _onEndCallback, _onNumberSelection, null, null, 
               _onCancelCallback);

            InitDirectMessageAndGraph(directmsg);
        }

        public void ChoseNumberFromMessage(int _chosenNumber)
        {
            HideMessage();

            VP_AudioManager.Instance.RemoveItem(VP_AudioSetup.Dialog.ANSWER_CLIP);
            VP_AudioManager.Instance.RemoveItem(VP_AudioSetup.Dialog.DIALOGUE_CLIP);

            if (!m_usingDirectMessage)
            {
                if (!m_currentChart)
                {
                    OnNumberChosenAction(_chosenNumber);
                    OnDialogEndAction();
                }
                else
                {
                    OnNumberChosenAction(_chosenNumber);
                }
            }
            else
            {
                //m_queueBusy = false;
                m_usingDirectMessage = false;
                if (m_directMessages.Count > 0)
                {
                    NextDirectMessage();
                }
                else
                {
                    OnNumberChosenAction(_chosenNumber);
                    OnDialogEndAction();
                }
            }
        }

        void NextDirectMessage(bool _onComplete = true)
        {
            if (_onComplete)
                OnDialogCompleteAction();

            VP_DialogDirectMessage newMsg = m_directMessages.Dequeue();
            InitDirectMessage(newMsg);

        }

        public void CancelNumberFromMessage()
        {
            HideMessage();

            VP_AudioManager.Instance.RemoveItem(VP_AudioSetup.Dialog.ANSWER_CLIP);
            VP_AudioManager.Instance.RemoveItem(VP_AudioSetup.Dialog.DIALOGUE_CLIP);

            if (!m_usingDirectMessage)
            {
                if (!m_currentChart)
                {
                    OnNumberCancelAction();
                    OnDialogEndAction();
                }
                else
                {
                    OnNumberCancelAction();
                }
            }
            else
            {
                //m_queueBusy = false;
                m_usingDirectMessage = false;
                if (m_directMessages.Count > 0)
                {
                    NextDirectMessage();
                }
                else
                {
                    OnNumberCancelAction();
                    OnDialogEndAction();
                }
            }
        }

        public float GetDialogAutoTimeProgress()
        {
            return m_talkingDialog.AutoAnswerTimeProgress();
        }

        public static float GetAutoTimeProgress()
        {
            return (m_instance && IsSpeaking) ? m_instance.GetDialogAutoTimeProgress() : 0.0f;
        }

        /// <summary>
        /// Set a current dialog game object as current 
        /// </summary>
        /// <param name="_value"></param>
        public void SetThisDialog(VP_DialogMessage value, Transform chartCanvas)
        {
            if (value != null)
                m_talkingDialog = value;

            if (chartCanvas != null)
            {
                m_dialogCanvas = chartCanvas;
                if (m_deactivateCanvasOnFadeOut)
                    m_dialogCanvas.gameObject.SetActive(true);
            }

        }
        /// <summary>
        /// Select a new chart but not start a dialog til a message is sent
        /// </summary>
        /// <param name="_newChart"></param>
        public void SelectCurrentChart(VP_DialogChart _newChart)
        {
            if (m_currentChart != _newChart)
                m_currentChart = _newChart;
        }
        /// <summary>
        /// Send the string starting message
        /// </summary>
        /// <param name="_msg"></param>
        public void _SendMessage(string _msg, Action _onEndCallback = null)
        {
            if (m_currentChart)
            {
                if (_onEndCallback != null)
                {
                    StartListeningToOnDialogEnd(_onEndCallback);
                }

                m_usingDirectMessage = false;
                m_currentChart.SendDialogMessage(_msg);
            }
            else
            {
                Debug.LogError("No chart is added to Dialog manager but key is trying to be used.");
            }
        }
        /// <summary>
        /// Hide the canvas
        /// </summary>
        public void _HideCanvas()
        {
            if (m_deactivateCanvasOnFadeOut)
                m_dialogCanvas.gameObject.SetActive(false);
        }
        /// <summary>
        /// The called method changes the chart and starts a dialog based on the data
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="_data"></param>
        public void _ShowMessage(VP_DialogChart chart, VP_DialogMessageData _data)
        {
            if (m_deactivateCanvasOnFadeOut)
                m_dialogCanvas.gameObject.SetActive(true);

            m_speaking = true;
            if (chart && chart != m_currentChart)
                m_currentChart = chart;

            if (m_talkingDialog == null)
            {
                m_talkingDialog = FindObjectOfType<VP_DialogMessage>();

                if (m_talkingDialog == null)
                {
                    GameObject go = Instantiate(m_dialogPrefab, m_dialogCanvas);
                    m_talkingDialog = go.GetComponent<VP_DialogMessage>();
                }
            }
            else
            {
                m_talkingDialog.gameObject.SetActive(true);

            }
            OnCharacterSpeakAction(_data.m_characterData);
            OnDialogStartAction();
            m_talkingDialog.ShowDialog(_data);
        }


        /// <summary>
        /// Continue to next text. If the dialog doesn't continue, the dialog GO, just disappears.
        /// </summary>
        public void ContinueText()
        {
            HideMessage();

            VP_AudioManager.Instance.RemoveItem(VP_AudioSetup.Dialog.ANSWER_CLIP);
            VP_AudioManager.Instance.RemoveItem(VP_AudioSetup.Dialog.DIALOGUE_CLIP);

            if (!m_usingDirectMessage)
            {
                if (m_currentChart)
                {
                    m_currentChart.ContinueText();
                }
                else
                {
                    OnDialogEndAction();
                }
            }
            else
            {
                //m_queueBusy = false;
                m_usingDirectMessage = false;
                if (m_directMessages.Count > 0)
                {
                    NextDirectMessage();
                }
                else
                {
                    OnDialogEndAction();
                }
            }
        }

        public void Skipping()
        {
            OnSkipAction();
            HideMessage();

            if (!m_usingDirectMessage)
            {
                if (m_currentChart)
                    m_currentChart.ContinueText();
                else
                    OnDialogEndAction();
            }
            else
            {
                //m_queueBusy = false;
                m_usingDirectMessage = false;
                m_directMessages.Clear();
                OnDialogEndAction();
            }
        }

        public void HideMessage()
        {

            m_speaking = false;

            if (m_deactivateCanvasOnFadeOut)
                m_dialogCanvas.gameObject.SetActive(false);

            m_talkingDialog.gameObject.SetActive(false);
        }

        /// <summary>
        /// Continues the dialog from the index of the answer
        /// </summary>
        /// <param name="index"></param>
        public void Answer(int index, System.Action _callback = null)
        {
            HideMessage();

            VP_AudioManager.Instance.RemoveItem(VP_AudioSetup.Dialog.ANSWER_CLIP);
            VP_AudioManager.Instance.RemoveItem(VP_AudioSetup.Dialog.DIALOGUE_CLIP);

            if (!m_usingDirectMessage)
            {
                if (m_currentChart != null)
                {
                    m_currentChart.AnswerText(index);
                }
                else
                {
                    if (OnChoiceSelection != null)
                        OnChoiceSelection.Invoke(index);
                    else if (_callback != null)
                        _callback.Invoke();

                    if (!m_queueBusy)
                    {
                        OnDialogEndAction();
                    }
                }
            }
            else
            {
                if (OnChoiceSelection != null)
                    OnChoiceSelection.Invoke(index);
                else if (_callback != null)
                    _callback.Invoke();

                if (!m_queueBusy)
                {
                    OnDialogEndAction();
                }
            }
        }

        public void ConfirmMessage(string _message, AudioClip _clip = null, DIALOG_TYPE _type = DIALOG_TYPE.REGULAR, System.Action _yesCallback = null, System.Action _noCallback = null, bool _showAnswersSameTime = true, 
            int _autoAnswer = -1, bool _translate = false,VP_DialogMessage _customDialogMessage = null, System.Action _onCompleteCallback = null, Action _onStartCallback = null, Action _onEndCallback = null, Action _onTextShown = null,
            VP_DialogPositionData _pos = null, bool _skippable = true, bool waitForInput = true, float _duration = 0.5f, bool showDirectly = false, bool fadeInOut = true, bool _soundOnContinue = true, VP_DialogCharacterData _character = null, float _textSpeed = 1f, bool _waitForAudioEnd = true,
            bool _overrideTextColor = false, Color textColor = default(Color), TMPro.TMP_FontAsset _font = null, float _fontSize = 45f,
             float _timeForAnswer = 5f, bool _chooseNumber = false, Vector3 _parameters = default(Vector3), bool _canCancel = true)
        {
            List<VP_Dialog.Answer> answers = new List<VP_Dialog.Answer>()
            {
                new VP_Dialog.Answer(_translate, "Yes", true, false, false, null, _yesCallback),
                new VP_Dialog.Answer(_translate, "No", true, true, false, null, _noCallback)
            };

            DirectMessageWithOptions(_message, _clip, _type, answers, _showAnswersSameTime, _autoAnswer, _translate, _customDialogMessage, _onCompleteCallback, _onStartCallback, _onEndCallback, _onTextShown, _pos, _skippable, waitForInput,
                _duration, showDirectly, fadeInOut, _soundOnContinue, _character, _textSpeed, _waitForAudioEnd, _overrideTextColor, textColor, _font, _fontSize, _timeForAnswer, _chooseNumber, _parameters, _canCancel);
        }

        public void NoConfirmMessage(string _message, AudioClip _clip = null, DIALOG_TYPE _type = DIALOG_TYPE.REGULAR, System.Action _yesCallback = null, System.Action _noCallback = null, bool _showAnswersSameTime = true,
            int _autoAnswer = -1, bool _translate = false, VP_DialogMessage _customDialogMessage = null, System.Action _onCompleteCallback = null, Action _onStartCallback = null, Action _onEndCallback = null, Action _onTextShown = null,
            VP_DialogPositionData _pos = null, bool _skippable = true, bool waitForInput = true, float _duration = 0.5f, bool showDirectly = false, bool fadeInOut = true, bool _soundOnContinue = true, VP_DialogCharacterData _character = null, float _textSpeed = 1f, bool _waitForAudioEnd = true,
            bool _overrideTextColor = false, Color textColor = default(Color), TMPro.TMP_FontAsset _font = null, float _fontSize = 45f,
             float _timeForAnswer = 5f, bool _chooseNumber = false, Vector3 _parameters = default(Vector3), bool _canCancel = true)
        {
            List<VP_Dialog.Answer> answers = new List<VP_Dialog.Answer>()
            {
                new VP_Dialog.Answer(_translate, "No", true, false, false, null, _noCallback),
                new VP_Dialog.Answer(_translate, "Yes", true, true, false, null, _yesCallback),
            };

            DirectMessageWithOptions(_message, _clip, _type, answers, _showAnswersSameTime, _autoAnswer, _translate, _customDialogMessage, _onCompleteCallback, _onStartCallback, _onEndCallback, _onTextShown, _pos, _skippable, waitForInput,
                _duration, showDirectly, fadeInOut, _soundOnContinue, _character, _textSpeed, _waitForAudioEnd, _overrideTextColor, textColor, _font, _fontSize, _timeForAnswer, _chooseNumber, _parameters, _canCancel);
        }



        public static void SetCurrentChart(VP_DialogChart _chart)
        {
            if (m_instance)
                m_instance.SelectCurrentChart(_chart);
        }

        public static void ShowConfirmMessage(string _message, AudioClip _clip = null, DIALOG_TYPE _type = DIALOG_TYPE.REGULAR, System.Action _yesCallback = null, System.Action _noCallback = null, bool _showAnswersSameTime = true,
            int _autoAnswer = -1, bool _translate = false, VP_DialogMessage _customDialogMessage = null, System.Action _onCompleteCallback = null, Action _onStartCallback = null, Action _onEndCallback = null, Action _onTextShown = null,
            VP_DialogPositionData _pos = null, bool _skippable = true, bool waitForInput = true, float _duration = 0.5f, bool showDirectly = false, bool fadeInOut = true, bool _soundOnContinue = true, VP_DialogCharacterData _character = null, float _textSpeed = 1f, bool _waitForAudioEnd = true,
            bool _overrideTextColor = false, Color textColor = default(Color), TMPro.TMP_FontAsset _font = null, float _fontSize = 45f,
             float _timeForAnswer = 5f, bool _chooseNumber = false, Vector3 _parameters = default(Vector3), bool _canCancel = true)
        {
            if (Instance)
            {
                Instance.ConfirmMessage(_message, _clip, _type, _yesCallback, _noCallback, _showAnswersSameTime, _autoAnswer, _translate, _customDialogMessage, _onCompleteCallback, _onStartCallback, _onEndCallback, _onTextShown, _pos, _skippable, waitForInput,
                _duration, showDirectly, fadeInOut, _soundOnContinue, _character, _textSpeed, _waitForAudioEnd, _overrideTextColor, textColor, _font, _fontSize, _timeForAnswer, _chooseNumber, _parameters, _canCancel);
            }
        }

        public static void ShowNoConfirmMessage(string _message, AudioClip _clip = null, DIALOG_TYPE _type = DIALOG_TYPE.REGULAR, System.Action _yesCallback = null, System.Action _noCallback = null, bool _showAnswersSameTime = true,
            int _autoAnswer = -1, bool _translate = false, VP_DialogMessage _customDialogMessage = null, System.Action _onCompleteCallback = null, Action _onStartCallback = null, Action _onEndCallback = null, Action _onTextShown = null,
            VP_DialogPositionData _pos = null, bool _skippable = true, bool waitForInput = true, float _duration = 0.5f, bool showDirectly = false, bool fadeInOut = true, bool _soundOnContinue = true, VP_DialogCharacterData _character = null, float _textSpeed = 1f, bool _waitForAudioEnd = true,
            bool _overrideTextColor = false, Color textColor = default(Color), TMPro.TMP_FontAsset _font = null, float _fontSize = 45f,
             float _timeForAnswer = 5f, bool _chooseNumber = false, Vector3 _parameters = default(Vector3), bool _canCancel = true)
        {
            if (Instance)
            {
                Instance.NoConfirmMessage(_message, _clip, _type, _yesCallback, _noCallback, _showAnswersSameTime, _autoAnswer, _translate, _customDialogMessage, _onCompleteCallback, _onStartCallback, _onEndCallback, _onTextShown, _pos, _skippable, waitForInput,
                _duration, showDirectly, fadeInOut, _soundOnContinue, _character, _textSpeed, _waitForAudioEnd, _overrideTextColor, textColor, _font, _fontSize, _timeForAnswer, _chooseNumber, _parameters, _canCancel);
            }
        }

        /// <summary>
        /// Static methods that only calls a method if the instance is in the scene to avoid nulls
        /// 
        /// The called method changes the chart and starts a dialog
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="_data"></param>
        public static void ShowMessage(VP_DialogChart chart, VP_DialogMessageData _data)
        {
            if (Instance)
                Instance._ShowMessage(chart, _data);
        }
        /// <summary>
        /// Static methods that only calls a method if the instance is in the scene to avoid nulls
        /// 
        /// The called method tries to start the dialog
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="_data"></param>
        public static void SendDialogMessage(string _key, VP_DialogChart _chart = null, Action _onEndCallback = null, VP_DialogMessage _customMessage = null)
        {
            if (m_instance)
            {
                if (_chart != null)
                    m_instance.CurrentChart = _chart;

                if (_customMessage != null)
                {
                    m_instance.CurrentChart.CurrentDialogMessage = _customMessage;
                }

                m_instance._SendMessage(_key, _onEndCallback);
            }

        }

        public static void SendMessage(string _key, VP_DialogChart _chart = null, Action _onEndCallback = null, VP_DialogMessage _customMessage = null)
        {
            if (m_instance)
            {
                if (_chart != null)
                    m_instance.CurrentChart = _chart;


                if (_customMessage != null)
                {
                    m_instance.CurrentChart.CurrentDialogMessage = _customMessage;
                }

                m_instance._SendMessage(_key, _onEndCallback);
            }

        }

        /// <summary>
        /// Static methods that only calls a method if the instance is in the scene to avoid nulls
        /// 
        /// The called method tries to start the dialog
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="_data"></param>
        public static void SendDialogMessageOnGraph(string _key, VP_DialogGraph _graph = null)
        {
            if (m_instance)
            {
                if (_graph != null)
                {

                    if (m_instance.CurrentChart != null)
                    {
                        m_instance.m_currentChart.Graph = (_graph);
                    }
                    else
                    {
                        Debug.LogError("No chart is added to dialgo manager, so graph can't be assigned.");

                        return;
                    }
                }

                m_instance._SendMessage(_key);
            }

        }

        public static void ShowDirectMessageInPosition(string _message, VP_DialogPositionData _pos = null, AudioClip _clip = null, DIALOG_TYPE _type = DIALOG_TYPE.REGULAR, bool _translate = false, bool showDirectly = false, 
            bool fadeInOut = true, VP_DialogMessage _customDialogMessage = null, System.Action _onCompleteCallback = null, Action _onStartCallback = null, Action _onEndCallback = null, Action _onTextShown = null, 
            bool _skippable = true, bool waitForInput = true, float _duration = 0.5f, bool _soundOnContinue = true, VP_DialogCharacterData _character = null, float _textSpeed = 1f, bool _waitForAudioEnd = true,
            List<VP_Dialog.Answer> _answers = null, bool _showAnswersSameTime = true, int _autoAnswer = -1, bool _overrideTextColor = false, Color textColor = default(Color), TMPro.TMP_FontAsset _font = null, float _fontSize = 45f,
             float _timeForAnswer = 5f, bool _chooseNumber = false, Vector3 _parameters = default(Vector3), bool _canCancel = true)
        {
            if (m_instance)
                m_instance.DirectMessage(_message, _clip, _type, _translate, showDirectly, fadeInOut, _customDialogMessage, _onCompleteCallback, _onStartCallback, _onEndCallback, _onTextShown, _pos, _skippable, waitForInput, _duration,
                    _soundOnContinue, _character, _textSpeed, _waitForAudioEnd, _answers, _showAnswersSameTime, _autoAnswer, _overrideTextColor, textColor, _font, _fontSize, _timeForAnswer, _chooseNumber, _parameters, _canCancel);
        }

        public static void ChooseNumberWithParams(string _message, Vector3 _parameters = default(Vector3), bool _canCancel = true, AudioClip _clip = null, DIALOG_TYPE _type = DIALOG_TYPE.REGULAR, bool _translate = false, bool showDirectly = false, bool fadeInOut = true, VP_DialogMessage _customDialogMessage = null,
            System.Action _onCompleteCallback = null, Action _onStartCallback = null, Action _onEndCallback = null, Action _onTextShown = null, VP_DialogPositionData _pos = null,
            bool _skippable = true, bool waitForInput = true, float _duration = 0.5f, bool _soundOnContinue = true, VP_DialogCharacterData _character = null, float _textSpeed = 1f, bool _waitForAudioEnd = true,
            bool _overrideTextColor = false, Color textColor = default(Color), TMPro.TMP_FontAsset _font = null, float _fontSize = 45f)
        {
            if (m_instance)
                m_instance.ChooseNumber(_message, _clip, _type, _parameters, _translate, _customDialogMessage, _onCompleteCallback, _onStartCallback, _onEndCallback, _onTextShown, _pos, _skippable, waitForInput, _duration, showDirectly, fadeInOut,
                    _soundOnContinue, _character, _textSpeed, _waitForAudioEnd, _overrideTextColor, textColor, _font, _fontSize, -1f, _canCancel, null, true, -1); 
        }

        public static void ChooseNumberWithParamsInGraph(string _message, Vector3 _parameters = default(Vector3), System.Action<int> _onNumberSelection = null, bool _canCancel = true, System.Action _cancelledInput = null, DIALOG_TYPE _type = DIALOG_TYPE.REGULAR, bool _translate = false,
            VP_DialogPositionData _pos = null, VP_DialogCharacterData _character = null, AudioClip _clip = null, bool showDirectly = false, bool fadeInOut = true, VP_DialogMessage _customDialogMessage = null, System.Action _onCompleteCallback = null, Action _onStartCallback = null, Action _onEndCallback = null, Action _onTextShown = null, 
            bool _skippable = true, bool waitForInput = true, float _duration = 0.5f, bool _soundOnContinue = true, float _textSpeed = 1f, bool _waitForAudioEnd = true,
            bool _overrideTextColor = false, Color textColor = default(Color), TMPro.TMP_FontAsset _font = null, float _fontSize = 45f)
        {
            if (m_instance)
                m_instance.ChooseNumberInGraph(_message, _parameters,_onNumberSelection, _canCancel, _cancelledInput, _type, _clip, _translate, _customDialogMessage, _onCompleteCallback, _onStartCallback, _onEndCallback, _onTextShown, _pos, _skippable, 
                    waitForInput, _duration, showDirectly, fadeInOut, _soundOnContinue, _character, _textSpeed, _waitForAudioEnd, _overrideTextColor, textColor, _font, _fontSize, -1f, null, true, -1);
        }

        public static void ShowDirectMessage(string _message, AudioClip _clip = null, DIALOG_TYPE _type = DIALOG_TYPE.REGULAR, bool _translate = false, bool showDirectly = false, bool fadeInOut = true, VP_DialogMessage _customDialogMessage = null,
            System.Action _onCompleteCallback = null, Action _onStartCallback = null, Action _onEndCallback = null, Action _onTextShown = null, VP_DialogPositionData _pos = null,
            bool _skippable = true, bool waitForInput = true, float _duration = 0.5f, bool _soundOnContinue = true, VP_DialogCharacterData _character = null, float _textSpeed = 1f, bool _waitForAudioEnd = true,
            List<VP_Dialog.Answer> _answers = null, bool _showAnswersSameTime = true, int _autoAnswer = -1, bool _overrideTextColor = false, Color textColor = default(Color), TMPro.TMP_FontAsset _font = null, float _fontSize = 45f,
             float _timeForAnswer = 5f, bool _chooseNumber = false, Vector3 _parameters = default(Vector3), bool _canCancel = true)
        {
            if (m_instance)
                m_instance.DirectMessage(_message, _clip, _type, _translate, showDirectly, fadeInOut, _customDialogMessage, _onCompleteCallback, _onStartCallback, _onEndCallback, _onTextShown, _pos, _skippable, waitForInput, _duration,
                    _soundOnContinue, _character, _textSpeed, _waitForAudioEnd, _answers, _showAnswersSameTime, _autoAnswer, _overrideTextColor, textColor, _font, _fontSize, _timeForAnswer, _chooseNumber, _parameters, _canCancel);
        }
        public static void ShowDirectMessageWithCharacter(string _message, AudioClip _clip = null, DIALOG_TYPE _type = DIALOG_TYPE.REGULAR, VP_DialogCharacterData _character = null, VP_DialogPositionData _pos = null, bool _translate = false, 
            bool showDirectly = false, bool fadeInOut = true, VP_DialogMessage _customDialogMessage = null, System.Action _onCompleteCallback = null, Action _onStartCallback = null, Action _onEndCallback = null, Action _onTextShown = null, 
            bool _skippable = true, bool waitForInput = true, float _duration = 0.5f, bool _soundOnContinue = true, float _textSpeed = 1f, bool _waitForAudioEnd = true,
            List<VP_Dialog.Answer> _answers = null, bool _showAnswersSameTime = true, int _autoAnswer = -1, bool _overrideTextColor = false, Color textColor = default(Color), TMPro.TMP_FontAsset _font = null, float _fontSize = 45f,
             float _timeForAnswer = 5f, bool _chooseNumber = false, Vector3 _parameters = default(Vector3), bool _canCancel = true)
        {
            if (m_instance)
                m_instance.DirectMessage(_message, _clip, _type, _translate, showDirectly, fadeInOut, _customDialogMessage, _onCompleteCallback, _onStartCallback, _onEndCallback, _onTextShown, _pos, _skippable, waitForInput, _duration,
                    _soundOnContinue, _character, _textSpeed, _waitForAudioEnd, _answers, _showAnswersSameTime, _autoAnswer, _overrideTextColor, textColor, _font, _fontSize, _timeForAnswer, _chooseNumber, _parameters, _canCancel);
        }


        public static void ShowDirectMessageAndOptions(string _message, AudioClip _clip = null, DIALOG_TYPE _type = DIALOG_TYPE.REGULAR, List<VP_Dialog.Answer> _answers = null, bool _showAnswersSameTime = true, int _autoAnswer = -1, 
            bool _translate = false, bool showDirectly = false, bool fadeInOut = true, VP_DialogMessage _customDialogMessage = null, System.Action _onCompleteCallback = null, Action _onStartCallback = null, Action _onEndCallback = null, 
            Action _onTextShown = null, VP_DialogPositionData _pos = null, bool _skippable = true, bool waitForInput = true, float _duration = 0.5f, bool _soundOnContinue = true, VP_DialogCharacterData _character = null, float _textSpeed = 1f,
            bool _waitForAudioEnd = true, bool _overrideTextColor = false, Color textColor = default(Color), TMPro.TMP_FontAsset _font = null, float _fontSize = 45f,
             float _timeForAnswer = 5f, bool _chooseNumber = false, Vector3 _parameters = default(Vector3), bool _canCancel = true)
        {
            if (m_instance)
                m_instance.DirectMessageWithOptions(_message, _clip, _type, _answers, _showAnswersSameTime, _autoAnswer, _translate, _customDialogMessage, _onCompleteCallback, _onStartCallback, _onEndCallback, _onTextShown, _pos, _skippable, waitForInput,
                _duration, showDirectly, fadeInOut, _soundOnContinue, _character, _textSpeed, _waitForAudioEnd, _overrideTextColor, textColor, _font, _fontSize, _timeForAnswer, _chooseNumber, _parameters, _canCancel);
        }

        public static void EndCurrentDialog()
        {
            if (m_instance)
                m_instance.HideMessage();
        }
    }
}