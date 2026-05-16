using Hyper.UI.Selector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayMenu : MonoBehaviour
{
    public ResolutionSelector resolutionSelector;
    public RefreshRateSelector refreshRateSelector;
    public DisplayModeSelector windowModeSelector;
    public Toggle vSyncToggle;

    public GameObject refreshRateSelectorContainer;

    private void Start()
    {
        SetupResolutionSelector();
        SetupRefreshRateSelector();
        SetupWindowModeSelector();
        SetupVSyncToggle();
    }

    private void SetupResolutionSelector()
    {
        var resolutions = new List<Tuple<int, int>>();

        foreach (var resolution in Screen.resolutions)
        {
            if (!resolutions.Exists(r => r.Item1 == resolution.width && r.Item2 == resolution.height))
            {
                resolutions.Add(Tuple.Create(resolution.width, resolution.height));
            }
        }

        resolutionSelector.options.AddRange(resolutions);

        resolutionSelector.SetValue(new(Screen.currentResolution.width, Screen.currentResolution.height));

        resolutionSelector.OnSelectionChanged += resolution =>
        {
            Screen.SetResolution(resolution.Item1, resolution.Item2, Screen.fullScreenMode, Screen.currentResolution.refreshRateRatio);
        };
    }

    private void SetupRefreshRateSelector()
    {
        var refreshRates = new List<RefreshRate>();

        foreach (var resolution in Screen.resolutions)
        {
            if (resolution.width == Screen.currentResolution.width && resolution.height == Screen.currentResolution.height)
            {
                var refreshRate = resolution.refreshRateRatio;

                if (!refreshRates.Exists(r => r.Equals(refreshRate)))
                {
                    refreshRates.Add(refreshRate);
                }
            }
        }

        refreshRateSelector.options.AddRange(refreshRates);

        refreshRateSelector.SetValue(Screen.currentResolution.refreshRateRatio);

        refreshRateSelector.OnSelectionChanged += refreshRate =>
        {
            Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, Screen.fullScreenMode, refreshRate);
        };
    }

    private void SetupWindowModeSelector()
    {
        FullScreenMode[] windowModes =
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            FullScreenMode.ExclusiveFullScreen,
#endif
            FullScreenMode.FullScreenWindow,
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            //FullScreenMode.MaximizedWindow,
#endif
            FullScreenMode.Windowed,
        };

        windowModeSelector.options.AddRange(windowModes);

        windowModeSelector.SetValue(Screen.fullScreenMode);

        windowModeSelector.OnSelectionChanged += mode =>
        {
            Screen.fullScreenMode = mode;
            refreshRateSelectorContainer.SetActive(mode == FullScreenMode.ExclusiveFullScreen);
        };
    }

    private void SetupVSyncToggle()
    {
        vSyncToggle.isOn = QualitySettings.vSyncCount > 0;

        vSyncToggle.onValueChanged.AddListener(isOn =>
        {
            QualitySettings.vSyncCount = isOn ? 1 : 0;
        });
    }
}
