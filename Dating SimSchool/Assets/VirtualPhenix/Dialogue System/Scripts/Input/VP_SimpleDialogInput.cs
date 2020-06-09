using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Dialog
{
    public class VP_SimpleDialogInput : MonoBehaviour
    {
        public enum KEY_USE_TYPE
        {
            DIRECT_CALLBACKS,
            EVENT_MANAGER
        }

        [SerializeField] private KEY_USE_TYPE m_currentKeyUse = KEY_USE_TYPE.DIRECT_CALLBACKS;

        [SerializeField] private KeyCode[] m_interactKeys = new KeyCode[] { KeyCode.E }; 
        [SerializeField] private KeyCode[] m_upKeys = new KeyCode[] { KeyCode.UpArrow }; 
        [SerializeField] private KeyCode[] m_downKeys = new KeyCode[] { KeyCode.DownArrow }; 

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            foreach (KeyCode key in m_interactKeys)
            {
                if (Input.GetKeyDown(key))
                {
                    if (m_currentKeyUse == KEY_USE_TYPE.DIRECT_CALLBACKS)
                        VP_DialogManager.OnDialogInteractAction();
                    else
                        VP_EventManager.TriggerEvent(VP_EventSetup.Input.INTERACT_BUTTON);
                }
            }

            foreach (KeyCode key in m_upKeys)
            {
                if (Input.GetKeyDown(key))
                {
                    if (m_currentKeyUse == KEY_USE_TYPE.DIRECT_CALLBACKS)
                        VP_DialogManager.OnDialogUpAction();
                    else
                        VP_EventManager.TriggerEvent(VP_EventSetup.Input.UP_ANSWER);
                }
            }

            foreach (KeyCode key in m_downKeys)
            {
                if (Input.GetKeyDown(key))
                {
                    if (m_currentKeyUse == KEY_USE_TYPE.DIRECT_CALLBACKS)
                        VP_DialogManager.OnDialogDownAction();
                    else
                        VP_EventManager.TriggerEvent(VP_EventSetup.Input.DOWN_ANSWER);
                }
            }
        }
    }

}
