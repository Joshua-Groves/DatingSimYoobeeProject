using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace VirtualPhenix
{
    public class VP_GameEvent : MonoBehaviour, VP_Actionable
    {
        [Header("Type"), Space(10)]
        /// <summary>
        /// If it uses Unity Events or Unity Send Message
        /// </summary>
        [SerializeField] private GAME_EVENT_TYPE m_type = GAME_EVENT_TYPE.SEND_MESSAGE;

        [Header("How to trigger"), Space(10)]
        /// <summary>
        /// If wanna use send message, which objects do you wanna call
        /// </summary>
        [SerializeField] private GameObject[] m_relatedGOs;
        /// <summary>
        /// If the event must be triggered when an event is listened, On Action or On Trigger Enter
        /// </summary>
        [SerializeField] private GAME_EVENT_TRIGGER m_trigger = GAME_EVENT_TRIGGER.ON_ACTION;
        /// <summary>
        /// If wanna use send message, which objects do you wanna call
        /// </summary>
        [SerializeField] private string[] m_triggerTags;
        /// <summary>
        /// Deactivate the game event after triggering it
        /// </summary>
        [SerializeField] private bool m_deactivateOnTrigger = true;
        [SerializeField] private bool m_canActivate = true;

        [Header("Custom Events"), Space(10)]
        /// <summary>
        /// If wanna use send message, which methods do you wanna call. The rest uses Unity Events and only the first one in the array
        /// </summary>
        [SerializeField] private string[] m_events;
        /// <summary>
        /// Events that this game event can be listening to
        /// </summary>
        [SerializeField] private string[] m_listeningEvents;

        [Header("Unity Events"), Space(10)]
        [SerializeField] private UnityEvent[] m_unityEventToCall;

        [Header("Send Messages"), Space(10)]
        [SerializeField] private string[] m_messages;

        // Use this for initialization
        void Start()
        {
            if (m_events == null || m_events.Length == 0)
            {
                m_events = new string[0];
            }

            if (m_listeningEvents == null || m_listeningEvents.Length == 0)
            {
                m_listeningEvents = new string[0];
            }

            if (m_messages == null || m_messages.Length == 0)
            {
                m_messages = new string[0];
            }

            if (m_triggerTags == null || m_triggerTags.Length == 0)
            {
                m_triggerTags = new string[0];
            }

            if (m_unityEventToCall == null || m_unityEventToCall.Length == 0)
            {
                m_unityEventToCall = new UnityEvent[0];
            }

            if (m_relatedGOs == null || m_relatedGOs.Length == 0)
            {
                m_relatedGOs = new GameObject[0];
            }
        }

        void OnEnable()
        {
            if (m_trigger == GAME_EVENT_TRIGGER.ON_EVENT)
            {
                foreach (string ev in m_listeningEvents)
                {
                    VP_EventManager.StartListening(ev, DoEvent);
                }
            }
        }

        void OnDisable()
        {
            if (m_trigger == GAME_EVENT_TRIGGER.ON_EVENT)
            {
                foreach (string ev in m_listeningEvents)
                {
                    VP_EventManager.StopListening(ev, DoEvent);
                }
            }
        }

        void OnDrawGizmos()
        {
            // Draws the Light bulb icon at position of the object.
            // Because we draw it inside OnDrawGizmos the icon is also pickable
            // in the scene view.

            Gizmos.DrawIcon(transform.position, "Game Event Gizmo.tiff", true);
        }

        public void OnAction()
        {
            if (m_trigger == GAME_EVENT_TRIGGER.ON_ACTION)
            {
                DoEvent();
            }

        }

        void DoEvent()
        {
            if (!m_canActivate)
                return;

            switch (m_type)
            {
                case GAME_EVENT_TYPE.CUSTOM_EVENT:
                    foreach (string _event in m_events)
                    {
                        VP_EventManager.TriggerEvent(_event);
                    }
                  
                    break;
                case GAME_EVENT_TYPE.UNITY_EVENT:
                    foreach (UnityEvent ev in m_unityEventToCall)
                    {
                        ev.Invoke();
                    }
                    break;
                case GAME_EVENT_TYPE.SEND_MESSAGE:
                    int index = 0;
                    foreach (GameObject go in m_relatedGOs)
                    {
                        go.SendMessage(m_messages[index]);
                        index++;
                    }
                    break;
            }

            if (m_deactivateOnTrigger)
            {
                m_canActivate = false;
            }         
        }

        public void Activate(bool _active)
        {
            m_canActivate = _active;
        }

        private void OnTriggerEnter(Collider collision)
        {
            if (!m_canActivate)
                return;

            if (m_trigger == GAME_EVENT_TRIGGER.ON_ENTER)
            {
                foreach (string go in m_triggerTags)
                {
                    if (collision.gameObject.CompareTag(go))
                    {
                        DoEvent();
                        break;
                    }
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!m_canActivate)
                return;

            if (m_trigger == GAME_EVENT_TRIGGER.ON_ENTER)
            {
                foreach (string go in m_triggerTags)
                {
                    if (collision.gameObject.CompareTag(go))
                    {
                        DoEvent();
                        break;
                    }
                }
            }
        }
    }

    public enum GAME_EVENT_TYPE
    {
        CUSTOM_EVENT,
        UNITY_EVENT,
        SEND_MESSAGE
    }

    public enum GAME_EVENT_TRIGGER
    {
        ON_ENTER,
        ON_ACTION,
        ON_EVENT
    }
}
