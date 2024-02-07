using UnityEngine;

public class TestCall : MonoBehaviour
{
    string ApiKey;
    void Start()
    {/*
        using (StreamReader sr = new(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + "/.openai/auth.json"))
        {
            ApiKey = JObject.Parse(sr.ReadToEnd())["api_key"].ToString();
        }*/

        GetComponent<TTSManager>().SynthesizeAndPlay("this is a test. qwertyuiopasdfghjklzxcvnm");
    }
}
