using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix
{
    public static class VP_AudioSetup
    {
        //Resources/Audio/
        public const string AUDIO_PATH = "/Audio/";
        public enum AUDIO_TYPE
        {
            BGM,
            SFX,
            VOICE
        }

        public static class UI
        {
            public const string BUTTON_HOVER = "buttonOver";
            public const string BUTTON_SELECT = "buttonSelect";
            public const string CONFIRM = "confirm";
            public const string WARNING = "warning";
            public const string TYPE = "type";
        }

        public static class General
        {
            public const string BGM_TEST = "BGM_TEST_KEY";
        }     
        
        public static class Dialog
        {
            public const string ANSWER_CLIP = "answerClip";
            public const string DIALOGUE_CLIP = "dialogueClip";
        }

    }
}
