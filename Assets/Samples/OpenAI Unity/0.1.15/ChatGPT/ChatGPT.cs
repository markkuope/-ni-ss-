using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;
using System.IO;
using Newtonsoft.Json;

namespace OpenAI
{
    public class ChatGPT : MonoBehaviour
    {
        [SerializeField] private InputField inputField;
        [SerializeField] private Button button;
        [SerializeField] private ScrollRect scroll;
        [SerializeField] private RectTransform sent;
        [SerializeField] private RectTransform received;
        [SerializeField] private UnityEvent onReply;
        [SerializeField] private TTSManager ttsmanager;

        private string LogPath;

        private float height;
        private OpenAIApi openai = new();

        private List<ChatMessage> messages = new();
        //private readonly string prompt = "Act as a random stranger in a chat room and reply to the questions. Don't break character. Don't ever mention that you are an AI model.";

        private readonly string prompt = "Tavoite: Pelaajan on sanottava lause oikein. Joka toinen kerta ohjelma lis‰‰ lauseeseen yhden sanan.\r\n\r\nAinekset:\r\n\r\nTietokone\r\nPuhetoiminnon omaava ohjelma\r\nLauselista\r\nOhjeet:\r\n\r\nLuo lauselista, joka sis‰lt‰‰ erilaisia lauseita. Lauseet voivat olla lyhyit‰ tai pitki‰, yksinkertaisia tai monimutkaisia.\r\nAseta tietokone k‰yntiin ja avaa puhetoiminnon omaava ohjelma.\r\nValitse ensimm‰inen lause lauselistasta.\r\nLue lause pelaajalle.\r\nPyyd‰ pelaajaa sanomaan lause oikein.\r\nJos pelaaja sanoo lauseen oikein, siirry seuraavaan lauseeseen.\r\nJos pelaaja sanoo lauseen v‰‰rin, toista lause ja pyyd‰ pelaajaa sanomaan se uudelleen.\r\nJoka toinen kerta, kun pelaaja sanoo lauseen oikein, lis‰‰ lauseeseen yksi sana.\r\nPeli jatkuu, kunnes pelaaja ei pysty en‰‰ sanomaan lausetta oikein.\r\nEsimerkki:\r\n\r\nLauselista:\r\n\r\nHevonen juoksee.\r\nKoira haukkuu.\r\nAurinko paistaa.\r\nLintu laulaa.\r\nPelaaja sanoo ensimm‰isen lauseen oikein. Ohjelma lis‰‰ lauseeseen yhden sanan:\r\n\r\nLause: Hevonen juoksee mets‰ss‰.\r\n\r\nPelaaja sanoo toisen lauseen oikein. Ohjelma lis‰‰ lauseeseen yhden sanan:\r\n\r\nLause: Hevonen juoksee mets‰ss‰ nopeasti.\r\n\r\nPelaaja sanoo kolmannen lauseen oikein. Ohjelma lis‰‰ lauseeseen yhden sanan:\r\n\r\nLause: Hevonen juoksee mets‰ss‰ nopeasti ja v‰sym‰tt‰.\r\n\r\nPelaaja sanoo nelj‰nnen lauseen v‰‰rin. Ohjelma toistaa lauseen ja pyyt‰‰ pelaajaa sanomaan sen uudelleen.\r\n\r\nPelaaja sanoo nelj‰nnen lauseen oikein. Ohjelma lis‰‰ lauseeseen yhden sanan:\r\n\r\nLause: Hevonen juoksee mets‰ss‰ nopeasti ja v‰sym‰tt‰, mutta se v‰syy lopulta.\r\n\r\nPeli jatkuu, kunnes lause on 7 sanaa pitk‰, sen j‰lkeen sano [ENDCONVO]";

        private void Start() {
            button.onClick.AddListener(SendReply);
            messages.Add(new ChatMessage() { Content=prompt, Role="system"});
            
            
            //LogPath = Application.dataPath + "/ChatLog/Log_" + System.DateTime.UtcNow.ToString() + ".json";
        }


        private void AppendMessage(ChatMessage message)
        {
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0);

            var item = Instantiate(message.Role == "user" ? sent : received, scroll.content);
            item.GetChild(0).GetChild(0).GetComponent<Text>().text = message.Content;
            item.anchoredPosition = new Vector2(0, -height);
            LayoutRebuilder.ForceRebuildLayoutImmediate(item);
            height += item.sizeDelta.y;
            scroll.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
            scroll.verticalNormalizedPosition = 0;

            //make sure to create a [ChatLog] directory in the Assets path
            /*
            //serializes ChatMessage list into a json string
            string MessageLog = JsonConvert.SerializeObject(messages, Formatting.None);
            StreamWriter sw = new StreamWriter(LogPath, false);
            //writes Message Log into a file
            sw.Write(MessageLog);
            sw.Close();*/
        }
        public void SetAndSend(string sentMessage)
        {
            inputField.text = sentMessage;
            SendReply();
        }

        private async void SendReply()
        {
            var newMessage = new ChatMessage()
            {
                Role = "user",
                Content = inputField.text
            };
            //add prompt into chat list
            AppendMessage(newMessage);

            if (messages.Count == 0) newMessage.Content = prompt + "\n" + inputField.text; 
            
            messages.Add(new ChatMessage() { Content=newMessage.Content.Replace(prompt,""), Role="user" });
            
            button.enabled = false;
            inputField.text = "";
            inputField.enabled = false;
            
            // Complete the instruction
            var completionResponse = await openai.CreateChatCompletion(new CreateChatCompletionRequest()
            {
                Model = "gpt-3.5-turbo-0613",
                Messages = messages
            });

            if (completionResponse.Choices != null && completionResponse.Choices.Count > 0)
            {
                var message = completionResponse.Choices[0].Message;
                message.Content = message.Content.Trim();
                messages.Add(message);
                AppendMessage(message);
                onReply.Invoke();
                ttsmanager.SynthesizeAndPlay(message.Content);
            }
            else
            {
                var message=new ChatMessage() {Content="ERROR: oh sh*t, there's no reply, make sure you have the openAI API key.", Role="function"};
                inputField.text = "ERROR";
                messages.Add(message);
                AppendMessage(message);
                Debug.LogWarning("No text was generated from this prompt.");
                onReply.Invoke();
            }

            button.enabled = true;
            inputField.enabled = true;
        }
    }
}
