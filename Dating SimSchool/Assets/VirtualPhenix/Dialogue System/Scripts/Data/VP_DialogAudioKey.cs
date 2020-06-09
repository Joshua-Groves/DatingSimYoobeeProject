using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VirtualPhenix
{
    [System.Serializable, CreateAssetMenu(menuName = "Virtual Phenix/Dialogue System/Data/audiokeyData")]
    public class VP_DialogAudioKey : ScriptableObject
    {
        [SerializeField] public string key;
        [SerializeField] public Dictionary<string, string> list;
    }
}
