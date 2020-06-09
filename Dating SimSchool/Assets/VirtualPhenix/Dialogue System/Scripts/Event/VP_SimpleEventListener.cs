using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VirtualPhenix.Dialog
{
    public class VP_SimpleEventListener : MonoBehaviour
    {
        public enum ON_START_LISTENING
        {
            ENABLE,
            START,
            AWAKE,
            MANUAL
        }

        public enum ON_STOP_LISTENING
        {
            DISABLE,
            DESTROY,
            MANUAL
        }

        [SerializeField] protected ON_START_LISTENING m_startListening = ON_START_LISTENING.START;
        [SerializeField] protected ON_STOP_LISTENING m_stopListening = ON_STOP_LISTENING.DESTROY;
        [SerializeField] protected string[] m_CustomEvents;
        [SerializeField] protected UnityEvent[] m_CustomEventCallback;

        [Header("Dialog Manager Callbacks")]
        [SerializeField] protected UnityEvent m_OnStartDialog;
        [SerializeField] protected UnityEvent m_OnEndDialog;
        [SerializeField] protected UnityEvent m_OnTextShown;
        [SerializeField] protected UnityEvent m_OnCompleteDialog;
        [SerializeField] protected UnityEvent m_OnAnswersShown;
        [SerializeField] protected UnityEvent<int> m_OnAnswer;
        [SerializeField] protected UnityEvent m_OnSkip;

        protected void Awake()
        {
            if (m_startListening == ON_START_LISTENING.AWAKE)
                StartAllListeners();
        }

        protected virtual void Start()
        {
            if (m_CustomEvents == null || m_CustomEvents.Length == 0)
                m_CustomEvents = new string[0];

            if (m_CustomEventCallback == null || m_CustomEventCallback.Length == 0)
                m_CustomEventCallback = new UnityEvent[0];

            if (m_startListening == ON_START_LISTENING.START)
                StartAllListeners();
        }

        protected virtual void OnEnable()
        {
            if (m_startListening == ON_START_LISTENING.ENABLE)
                StartAllListeners();
        }

        protected virtual void OnDisable()
        {
            if (m_stopListening == ON_STOP_LISTENING.DISABLE)
                StopAllListeners();
        }

        protected virtual void OnDestroy()
        {
            if (m_stopListening == ON_STOP_LISTENING.DESTROY)
                StopAllListeners();
        }

        protected virtual void StartAllListeners()
        {
            for (int i = 0; i < m_CustomEvents.Length; i++)
            {
                if (i < m_CustomEventCallback.Length)
                    VP_EventManager.StartListening(m_CustomEvents[i], m_CustomEventCallback[i].Invoke);
            }

            VP_DialogManager.StartListeningToOnDialogStart(m_OnStartDialog.Invoke);
            VP_DialogManager.StartListeningToOnDialogComplete(m_OnCompleteDialog.Invoke);
            VP_DialogManager.StartListeningToOnDialogEnd(m_OnEndDialog.Invoke);
            VP_DialogManager.StartListeningToOnTextShown(m_OnTextShown.Invoke);
            VP_DialogManager.StartListeningToOnAnswerShow(m_OnAnswersShown.Invoke);
            VP_DialogManager.StartListeningOnChoiceSelection((int _value) => { m_OnAnswer.Invoke(_value); });
            VP_DialogManager.StartListeningToOnSkip(m_OnSkip.Invoke);
        }

        protected virtual void StopAllListeners()
        {
            for (int i = 0; i < m_CustomEvents.Length; i++)
            {
                if (i < m_CustomEventCallback.Length)
                    VP_EventManager.StopListening(m_CustomEvents[i], m_CustomEventCallback[i].Invoke);
            }

            VP_DialogManager.StopListeningToOnDialogStart(m_OnStartDialog.Invoke);
            VP_DialogManager.StopListeningToOnDialogEnd(m_OnEndDialog.Invoke);
            VP_DialogManager.StopListeningToOnTextShown(m_OnTextShown.Invoke);
            VP_DialogManager.StopListeningToOnDialogComplete(m_OnCompleteDialog.Invoke);
            VP_DialogManager.StopListeningToOnAnswerShow(m_OnAnswersShown.Invoke);
            VP_DialogManager.StopListeningOnChoiceSelection((int _value) => { m_OnAnswer.Invoke(_value); });
            VP_DialogManager.StopListeningToOnSkip(m_OnSkip.Invoke);
        }

    }

}
