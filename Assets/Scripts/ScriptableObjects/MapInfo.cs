using UnityEditor;
using UnityEngine;

namespace Hyper.ScriptableObjects
{
    [CreateAssetMenu(fileName = "MapInfo", menuName = "ScriptableObjects/MapInfo", order = 1)]
    public class MapInfo : ScriptableObject
    {
        public string mapName;
        public SceneAsset mapScene;
    }
}
