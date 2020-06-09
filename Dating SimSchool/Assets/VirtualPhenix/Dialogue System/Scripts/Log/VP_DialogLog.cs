using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix.Dialog
{
    public class VP_DialogLog 
    {
        public VP_DialogCharacterData m_character;
        public string m_saidText;

        public VP_DialogLog()
        {

        }

        public VP_DialogLog(VP_DialogCharacterData _character, string _text)
        {
            m_character = _character;
            m_saidText = _text;
        }

    }

}
