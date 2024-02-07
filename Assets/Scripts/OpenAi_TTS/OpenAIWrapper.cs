using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.IO;

public class OpenAIWrapper : MonoBehaviour
{
    private string openAIKey;
    private TTSModel model = TTSModel.TTS_1;
    private TTSVoice voice = TTSVoice.Alloy;
    private float speed = 1f;
    private readonly string outputFormat = "mp3";

    private void Awake()
    {
        /* for pc */
        /*using (StreamReader sr = new(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + "/.openai/auth.json"))
        {
            openAIKey = JObject.Parse(sr.ReadToEnd())["api_key"].ToString();
        }*/

        /* for android 
        using (StreamReader sr = new("/storage/emulated/0/.openai/auth.json"))
        {
            openAIKey = JObject.Parse(sr.ReadToEnd())["api_key"].ToString();
        }*/

        try {
            using (StreamReader sr = new("/storage/emulated/0/.openai/auth.json"))
            {
                openAIKey = JObject.Parse(sr.ReadToEnd())["api_key"].ToString();
            }
        }
        catch {
            using (StreamReader sr = new(System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile) + "/.openai/auth.json"))
            {
                openAIKey = JObject.Parse(sr.ReadToEnd())["api_key"].ToString();
            }
        }
    }

    public async Task<byte[]> RequestTextToSpeech(string text)
    {
        Debug.Log("[TTSWrapper]: Sending new request to OpenAI TTS.");
        using var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", openAIKey);

        var payload = new
        {
            model = this.model.EnumToString(),
            input = text,
            voice = this.voice.ToString().ToLower(),
            response_format = this.outputFormat,
            speed = this.speed
        };

        string jsonPayload = JsonConvert.SerializeObject(payload);

        var httpResponse = await httpClient.PostAsync(
            "https://api.openai.com/v1/audio/speech", 
            new StringContent(jsonPayload, Encoding.UTF8, "application/json"));

        byte[] response = await httpResponse.Content.ReadAsByteArrayAsync();

        if (httpResponse.IsSuccessStatusCode)
        {
            return response;
        }
        Debug.Log("Error: " + httpResponse.StatusCode.ToString());
        return null;
    }
    
    
    public async Task<byte[]> RequestTextToSpeech(string text, TTSModel model, TTSVoice voice, float speed)
    {
        this.model = model;
        this.voice = voice;
        this.speed = speed;
        return await RequestTextToSpeech(text);
    }
}