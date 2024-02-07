﻿using UnityEngine;
using UnityEngine.UI;

namespace OpenAI
{
    public class ReWhisper : MonoBehaviour
    {
        [SerializeField] private Image progress;
        [SerializeField] private ChatGPT chatGPT;
        
        private readonly string fileName = "output.wav";
        private readonly int duration = 20;

        private AudioClip clip;
        private bool isRecording;
        private float time;
        private OpenAIApi openai = new OpenAIApi();

        public async void StartRecording()
        {
            if (isRecording)
            {
                isRecording = false;
                Debug.Log("[whisper]: Stop recording...");

                time = 0;//aloitetaan äänityksen ajanlasku aina nollasta, tämä on lisäys Sargen koodiin
                

                Microphone.End(null);
                byte[] data = SaveWav.Save(fileName, clip);

                var req = new CreateAudioTranscriptionsRequest
                {
                    FileData = new FileData() { Data = data, Name = "audio.wav" },
                    // File = Application.persistentDataPath + "/" + fileName,
                    Model = "whisper-1",
                    Language = "fi"   // kieli: fi = suomi, en = englanti
                };
                var res = await openai.CreateAudioTranscription(req);
                progress.fillAmount = 0;
                //message.text = res.Text;   // message saa arvokseen OpenAI:lta  vastaanotetun tekstin
                chatGPT.SetAndSend(res.Text);

                //tässä kohdassa message.text arvo on whisper AI:n transkriptio
                //print(message.text);
                // tämä arvo viedään AI:lle ChatGPT scriptiin

                //whisperText = message.text;

                //tässä käynnistetään ChatGPT SendReply -funkio
                //ettei tarvitse painaa Send -nappia

                //ChatGPT.SendReply();

            }
            else
            {
                //Debug.Log("Start recording...");
                isRecording = true;

                var index = PlayerPrefs.GetInt("user-mic-device-index");
                clip = Microphone.Start(Microphone.devices[PlayerPrefs.GetInt("user-mic-device-index",2)-1], false, duration, 44100);
                //clip = Microphone.Start(dropdown.options[index].text, false, duration, 44100);
            }
        }

        // vaihtoehtoisesti audion kääntämispyyntö
        //var req = new CreateAudioTranslationRequest
        //{
        //    FileData = new FileData() { Data = data, Name = "audio.wav" },
        //    // File = Application.persistentDataPath + "/" + fileName,
        //    Model = "whisper-1",
        //};






        //private async void EndRecording()
        //{
        //    message.text = "Transcripting...";

        //    #if !UNITY_WEBGL
        //    Microphone.End(null);
        //    #endif

        //    byte[] data = SaveWav.Save(fileName, clip);

        //    var req = new CreateAudioTranscriptionsRequest
        //    {
        //        FileData = new FileData() {Data = data, Name = "audio.wav"},
        //        // File = Application.persistentDataPath + "/" + fileName,
        //        Model = "whisper-1",
        //        Language = "fi"
        //    };
        //    var res = await openai.CreateAudioTranscription(req);

        //    progress.fillAmount = 0;
        //    message.text = res.Text;
        //    recordButton.enabled = true;
        //}

        //private void Update()
        //{
        //    if (isRecording)
        //    {
        //        time += Time.deltaTime;
        //        progress.fillAmount = time / duration;

        //        if (time >= duration)
        //        {
        //            time = 0;
        //            isRecording = false;
        //            EndRecording();
        //        }
        //    }
        //}

        private void Update()
        {
            if (isRecording) { 
            time += Time.deltaTime;
            progress.fillAmount = time / duration;}

            if (time < duration) return;
            time = 0;
            progress.fillAmount = 0;
            StartRecording();
        }
    }
}