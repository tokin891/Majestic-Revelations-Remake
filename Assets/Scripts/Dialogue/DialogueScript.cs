using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class DialogueScript : ScriptableObject
{
    public int Id;
    public TextDialogue[] texts;
    public OptionsDialogue[] allOption;

    [System.Serializable]
    public class TextDialogue
    {
        public string textPlayer;
        public string textNPC;
    }
    [System.Serializable]
    public class OptionsDialogue
    {
        public string textOption;
        public int idOption;
    }

    public bool SearchID(int id)
    {
        for (int i = 0; i < allOption.Length; i++)
        {
            if (allOption[i].idOption == id)
                return true;
        }
        return false;
    }
}