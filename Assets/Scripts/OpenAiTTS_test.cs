using UnityEngine;

public class OpenAiTTS_test : MonoBehaviour
{
    [SerializeField] string Text;
    void Start()=>GetComponent<TTSManager>().SynthesizeAndPlay(Text);
}
