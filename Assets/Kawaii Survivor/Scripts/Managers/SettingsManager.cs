using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Tabsil.Sijil;
using UnityEngine.Networking;

public class SettingsManager : MonoBehaviour, IWantToBeSaved
{
    [Header(" Elements ")]
    [SerializeField] private Button sfxButton;
    [SerializeField] private Button privacyPolicyButton;
    [SerializeField] private Button askButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private GameObject creditsPanel;

    [Header(" Settings ")]
    [SerializeField] private Color onColor;
    [SerializeField] private Color offColor;

    [Header(" Data ")]
    private bool sfxState;

    [Header(" Actions ")]
    public static Action<bool> onSFXStateChanged;

    private void Awake()
    {
        sfxButton.onClick.RemoveAllListeners();
        sfxButton.onClick.AddListener(SFXButtonCallback);

        privacyPolicyButton.onClick.RemoveAllListeners();
        privacyPolicyButton.onClick.AddListener(PrivacyPolicyButtonCallback);

        askButton.onClick.RemoveAllListeners();
        askButton.onClick.AddListener(AskButtonCallback);

        creditsButton.onClick.RemoveAllListeners();
        creditsButton.onClick.AddListener(CreditsButtonCallback);
    }

    // Start is called before the first frame update
    void Start()
    {
        HideCreditsPanel();

        onSFXStateChanged?.Invoke(sfxState);

        Load();
    }

    private void SFXButtonCallback()
    {
        sfxState = !sfxState;

        UpdateSFXVisuals();

        Save();

        // Trigger an action with the new sfx state
        onSFXStateChanged?.Invoke(sfxState);
    }

    private void UpdateSFXVisuals()
    {
        if(sfxState)
        {
            sfxButton.image.color = onColor;
            sfxButton.GetComponentInChildren<TextMeshProUGUI>().text = "ON";
        }
        else
        {
            sfxButton.image.color = offColor;
            sfxButton.GetComponentInChildren<TextMeshProUGUI>().text = "OFF";
        }
    }

    private void PrivacyPolicyButtonCallback()
    {
        Application.OpenURL("https://www.tabsil.com/privacypolicy");
    }


    private void AskButtonCallback()
    {
        string email = "tabsilgames@gmail.com";
        string subject = MyEscapeURL("Help");
        string body = MyEscapeURL("Hey ! I need help with this....");

        Application.OpenURL("mailto:" + email + "?subject=" + subject + "&body=" + body);
    }

    private string MyEscapeURL(string s)
    {
        return UnityWebRequest.EscapeURL(s).Replace("+", "%20");
    }

    private void CreditsButtonCallback()
    {
        creditsPanel.SetActive(true);
    }

    public void HideCreditsPanel()
    {
        creditsPanel.SetActive(false);
    }

    public void Load()
    {
        sfxState = true;

        if (Sijil.TryLoad(this, "sfx", out object sfxStateObject))
        {
            sfxState = (bool)sfxStateObject;
            onSFXStateChanged?.Invoke(sfxState);
        }

        UpdateSFXVisuals();
    }

    public void Save() => Sijil.Save(this, "sfx", sfxState);
}
