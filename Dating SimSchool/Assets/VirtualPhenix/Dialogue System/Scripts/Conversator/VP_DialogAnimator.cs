using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Dialog
{
    public class VP_DialogAnimator : MonoBehaviour
    {
        public enum ANIMATION_BASE
        {
            NONE,
            ANIMATE_ALWAYS_WITHOUT_CHARACTER,
            ANIMATE_WITHOUT_CHARACTER,
            ON_CHARACTER,
            ON_CHARACTER_NAME
        }

        [Header("Animation")]
        /// <summary>
        /// Animator component
        /// </summary>
        [SerializeField] protected Animator m_animator;
        [SerializeField] protected string m_speakParameter = "talk";
        [SerializeField] protected bool m_speaking = false;
        /// <summary>
        /// You need to set this true manually for ANIMATE_WITHOUT_CHARACTER to work
        /// </summary>
        protected bool m_speakRefresh = false;


        [Header("Character")]
        /// <summary>
        /// Character related-> When this character speaks, the animator will play
        /// </summary>
        [SerializeField] protected VP_DialogCharacterData m_character = null;
        /// <summary>
        /// It starts if the character is the same file, just the name...
        /// </summary>
        [SerializeField] protected ANIMATION_BASE m_animationCheck = ANIMATION_BASE.ON_CHARACTER;
        /// <summary>
        /// Target designed mainly for IK
        /// </summary>
        [SerializeField] protected Transform m_target = null;

        protected virtual void Start()
        {
            if (m_animationCheck != ANIMATION_BASE.NONE && !m_animator)
                m_animator = GetComponent<Animator>();
        }

        protected virtual void OnEnable()
        {
            StartAllListeners();
        }

        protected virtual void OnDisable()
        {
            StopAllListeners();
        }

        protected virtual void StartAllListeners()
        {
            VP_DialogManager.StartListeningToOnCharacterSpeak(OnCharacterSpeaking);
            VP_DialogManager.StartListeningToOnTextShown(OnTextShown);
            VP_DialogManager.StartListeningToOnDialogEnd(OnDialogEnd);
        }

        protected virtual void StopAllListeners()
        {
            VP_DialogManager.StopListeningToOnCharacterSpeak(OnCharacterSpeaking);
            VP_DialogManager.StopListeningToOnTextShown(OnTextShown);
            VP_DialogManager.StopListeningToOnDialogEnd(OnDialogEnd);
        }

        protected virtual bool IsCharacter(VP_DialogCharacterData _character)
        {
            if (m_animationCheck != ANIMATION_BASE.NONE)
            {
                if (_character && m_character)
                {
                    switch (m_animationCheck)
                    {
                        case ANIMATION_BASE.ON_CHARACTER:
                            if (m_character == _character)
                            {
                                return true;
                            }
                            break;
                        case ANIMATION_BASE.ON_CHARACTER_NAME:
                            if (m_character.characterName == _character.characterName)
                            {
                                return true;
                            }
                            break;
                    }
                }
                else if (_character && !m_character)
                {
                    switch (m_animationCheck)
                    {
                        case ANIMATION_BASE.ANIMATE_ALWAYS_WITHOUT_CHARACTER:
                            return true;
                        case ANIMATION_BASE.ANIMATE_WITHOUT_CHARACTER:
                            if (m_speakRefresh)
                            {
                                // it means it is not us, so no more refresh
                                m_speakRefresh = false;
                                OnTextShown();
                            }
                            break;
                    }
                }
                else
                {
                    switch (m_animationCheck)
                    {
                        case ANIMATION_BASE.ANIMATE_ALWAYS_WITHOUT_CHARACTER:
                            return true;
                        case ANIMATION_BASE.ANIMATE_WITHOUT_CHARACTER:
                            if (m_speakRefresh)
                            {
                                return true;
                            }
                            break;
                    }
                }
            }

            return false;
        }

        protected virtual void OnCharacterSpeaking(VP_DialogCharacterData _character)
        {
            if (IsCharacter(_character))
            {
                m_speaking = true;

                if (m_animator)
                    m_animator.SetBool(m_speakParameter, true);

                if (m_target != null)
                {
                    VP_DialogManager.OnAnimationTargetAction(m_target);
                }
            }
        }

        protected virtual void OnTextShown()
        {
            if (m_speaking)
            {
                m_speaking = false;
                if (m_animator)
                    m_animator.SetBool(m_speakParameter, false);
            }
        }

        protected virtual void OnDialogEnd()
        {
            if (m_speakRefresh)
            {
                m_speakRefresh = false;
            }
        }
    }

}
