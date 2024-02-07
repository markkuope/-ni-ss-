using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Mic_select : MonoBehaviour
{
    private void Start()
    {
        Dropdown dropdown = GetComponent<Dropdown>();
#if UNITY_WEBGL && !UNITY_EDITOR
            dropdown.options.Add(new Dropdown.OptionData("Microphone not supported on WebGL"));
#else
        foreach (var device in Microphone.devices)
        {
            dropdown.options.Add(new Dropdown.OptionData(device));
        }
        dropdown.onValueChanged.AddListener(ChangeMicrophone);

        var index = PlayerPrefs.GetInt("user-mic-device-index", 0);
        dropdown.SetValueWithoutNotify(index);
#endif
    }

    private void ChangeMicrophone(int index)
    {
        PlayerPrefs.SetInt("user-mic-device-index", index);
        print(index);
    }

    public void MainScene() => SceneManager.LoadScene(1);
}
