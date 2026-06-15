using UnityEngine;

namespace Hyper.ScriptableObjects
{
    [CreateAssetMenu(fileName = "HyperCharacterInfo", menuName = "ScriptableObjects/HyperCharacterInfo", order = 1)]
    public class HyperCharacterInfo : ScriptableObject
    {
        public string characterName;
        public Sprite characterProfilePicture;
    }
}