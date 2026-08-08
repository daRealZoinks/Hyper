using Hyper.ScriptableObjects;
using Hyper.UI.Menus;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MapMenuManager : MonoBehaviour
{
    public List<MapInfo> maps = new();

    public MapButton mapButtonPrefab;

    public UnityEvent onMapSelected;

    [SerializeField] private GameSettingsManager gameSettingsManager;

    private GridLayoutGroup _mapIconGridLayoutGroup;

    private void Awake()
    {
        _mapIconGridLayoutGroup = GetComponentInChildren<GridLayoutGroup>();
        gameSettingsManager = FindAnyObjectByType<GameSettingsManager>();

        foreach (var map in maps)
        {
            var button = Instantiate(mapButtonPrefab, _mapIconGridLayoutGroup.transform);
            button.Initialize(map);
            button.GetComponent<Button>().onClick.AddListener(() =>
            {
                gameSettingsManager.SetMap(map);
                onMapSelected.Invoke();
            });
        }
    }
}
