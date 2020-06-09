using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Helpers;

namespace VirtualPhenix
{
    /// <summary>
    /// * This is a modified version of Unity's Event Tutorial.
    /// * From any script you can call: 
    /// 
    /// VP_EventManager.StartListening(EventName, methodToCallbackWhenTriggered);
    /// 
    /// * to start listening a specific event. Remember to stop listening on destroy so you don't have listener bugs
    /// 
    /// VP_EventManager.StopListening(EventName, sameStartListeningMethod);
    /// 
    /// * And if you want to trigger an event, just call:
    /// 
    /// VP_EventManager.TriggerEvent(EventName);
    /// 
    /// * For easier use, you can store event names in VP_EventSetup. Then call it from anywhere:
    /// 
    /// VP_EventManager.TriggerEvent(VP_EventSetup.Localization.TRANSLATE_TEXTS);
    /// </summary>
    public class VP_EventManager : HierarchyIcon
    {
        private static VP_EventManager m_instance;
        public static VP_EventManager Instance { get { return m_instance; } }

        [SerializeField] private bool m_destroyOnLoad = false;

        /// <summary>
        /// Event dictionary
        /// </summary>
        private Dictionary<string, object> m_eventDictionary;

        protected void Awake()
        {
            if (m_instance == null)
            {
                m_instance = this;
                Init();
                if (!m_destroyOnLoad)
                {
                    DontDestroyOnLoad(this);
                }
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

        /// <summary>
        /// Dictionary initialization
        /// </summary>
        void Init()
        {
            m_eventDictionary = new Dictionary<string, object>();
        }
        /// <summary>
        /// The desired object is now listening if it wasn't
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="listener"></param>
        public static void StartListening(string eventName, UnityAction listener)
        {
            if (Instance == null || Instance.m_eventDictionary == null)
                return;

            UnityEvent thisEvent = null;
            object thisEventObject = null;
            if (Instance.m_eventDictionary.TryGetValue(eventName, out thisEventObject))
            {
                thisEvent = thisEventObject as UnityEvent;
                if (thisEvent != null)
                {
                    //thisEvent.
                    thisEvent.RemoveListener(listener);
                    thisEvent.AddListener(listener);
                }
            }
            else
            {
                thisEvent = new UnityEvent();
                thisEvent.AddListener(listener);
                Instance.m_eventDictionary.Add(eventName, thisEvent);
            }
        }
        /// <summary>
        /// The desired object is no longer listening
        /// </summary>
        /// <param name="eventName"></param>
        /// <param name="listener"></param>
        public static void StopListening(string eventName, UnityAction listener)
        {
            if (Instance == null || Instance.m_eventDictionary == null)
                return;

            UnityEvent thisEvent = null;
            object thisEventObject = null;
            if (Instance.m_eventDictionary.TryGetValue(eventName, out thisEventObject))
            {
                thisEvent = thisEventObject as UnityEvent;

                if (thisEvent != null)
                    thisEvent.RemoveListener(listener);
            }
        }
        /// <summary>
        /// Triggers the event and calls every object that is listening to that event
        /// </summary>
        /// <param name="eventName"></param>
        public static void TriggerEvent(string eventName)
        {
            if (Instance == null)
                return;

            UnityEvent thisEvent = null;
            object thisEventObject = null;
            if (Instance.m_eventDictionary.TryGetValue(eventName, out thisEventObject))
            {
                thisEvent = thisEventObject as UnityEvent;

                if (thisEvent != null)
                    thisEvent.Invoke();
            }
        }

        public static void StartListening<T>(string eventName, UnityAction<T> listener)
        {
            if (Instance == null)
                return;

            CustomEvent<T> thisEvent = null;
            object thisEventObject = null;

            if (Instance.m_eventDictionary.TryGetValue(eventName, out thisEventObject))
            {
                thisEvent = thisEventObject as CustomEvent<T>;
                if (thisEvent == null)
                {
                    thisEvent = new CustomEvent<T>();
                    /*
                    if(thisEvent==null){
                        Debug.LogError("Listen Event:" + eventName + " still has no listners");
                    }else{
                        Debug.Log("EventName:" + eventName + "Event: "+thisEvent);
                        Debug.Log("Listener " + listener);
                    }*/

                    thisEvent.AddListener(listener);

                    Instance.m_eventDictionary[eventName] = thisEvent;

                }
                else
                {
                    thisEvent.AddListener(listener);
                }
            }
            else
            {
                thisEvent = new CustomEvent<T>();
                thisEvent.AddListener(listener);
                Instance.m_eventDictionary.Add(eventName, thisEvent);
            }
        }

        public static void StopListening<T>(string eventName, UnityAction<T> listener)
        {
            if (Instance == null)
                return;

            CustomEvent<T> thisEvent = null;
            object thisEventObject = null;

            if (Instance.m_eventDictionary.TryGetValue(eventName, out thisEventObject))
            {
                thisEvent = thisEventObject as CustomEvent<T>;

                if (thisEvent != null)
                    thisEvent.RemoveListener(listener);
            }
        }

        public static void TriggerEvent<T>(string eventName, T value)
        {
            if (Instance == null)
                return;

            object eventObject = null;
            if (Instance.m_eventDictionary.TryGetValue(eventName, out eventObject))
            {
                CustomEvent<T> thisEvent = eventObject as CustomEvent<T>;
                if (thisEvent != null)
                {
                    thisEvent.Invoke(value);
                }
                else
                {
                    Debug.LogError("Event is null EventName: " + eventName + ", thisEvent: " + thisEvent);
                }

            }
        }

        public static void StartListening<T0, T1>(string eventName, UnityAction<T0, T1> listener)
        {
            if (Instance == null)
                return;

            CustomEvent<T0, T1> thisEvent = null;
            object thisEventObject = null;

            if (Instance.m_eventDictionary.TryGetValue(eventName, out thisEventObject))
            {
                thisEvent = thisEventObject as CustomEvent<T0, T1>;
                if (thisEvent == null)
                {
                    thisEvent = new CustomEvent<T0, T1>();
                    thisEvent.AddListener(listener);

                    Instance.m_eventDictionary[eventName] = thisEvent;
                }
                else
                {
                    thisEvent.AddListener(listener);
                }
            }
            else
            {
                thisEvent = new CustomEvent<T0, T1>();
                thisEvent.AddListener(listener);
                Instance.m_eventDictionary.Add(eventName, thisEvent);
            }
        }

        public static void StopListening<T0, T1>(string eventName, UnityAction<T0, T1> listener)
        {
            if (Instance == null)
                return;

            CustomEvent<T0, T1> thisEvent = null;
            object thisEventObject = null;

            if (Instance.m_eventDictionary.TryGetValue(eventName, out thisEventObject))
            {
                thisEvent = thisEventObject as CustomEvent<T0, T1>;
                thisEvent.RemoveListener(listener);
            }
        }

        public static void TriggerEvent<T0, T1>(string eventName, T0 value0, T1 value1)
        {
            if (Instance == null)
                return;

            object eventObject = null;
            if (Instance.m_eventDictionary.TryGetValue(eventName, out eventObject))
            {
                CustomEvent<T0, T1> thisEvent = eventObject as CustomEvent<T0, T1>;
                if (thisEvent != null)
                    thisEvent.Invoke(value0, value1);
            }
        }
    }

    [System.Serializable]
    public class CustomEvent<T> : UnityEvent<T>
    {

    }

    [System.Serializable]
    public class CustomEvent<T0, T1> : UnityEvent<T0, T1>
    {

    }
}
