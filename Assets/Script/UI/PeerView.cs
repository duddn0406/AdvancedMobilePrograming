using System;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace WebRTCTutorial.UI
{    

    public class PeerView : MonoBehaviour
    {
        public AudioSource audioSource;  // public으로 설정하여 Inspector에서 보이게 함

        public void SetVideoTexture(Texture texture)
        {
            _videoRender.texture = texture;

            // Adjust the texture size to match the aspect ratio of the video
            var sourceAspectRatio = texture.width * 1f / texture.height;

            var currentSize = _videoRender.rectTransform.sizeDelta;
            var adjustedSize = new Vector2(currentSize.x, currentSize.x / sourceAspectRatio);

            _videoRender.rectTransform.sizeDelta = adjustedSize;
        }

        public void SetAudioSource(AudioClip microphone)
        {
            Debug.Log("dzzzzzz");
            if (microphone != null)
            {
                Debug.LogWarning("Microphone is ouptut.");
                audioSource.clip = microphone;
                audioSource.loop = true;
                audioSource.Play();
            }
            else
            {
                Debug.LogWarning("Microphone is null.");
            }
        }
        public AudioSource MakeAudioSource(AudioClip microphone)
        {
            audioSource.clip = microphone;
            return audioSource;
        }
#if UNITY_EDITOR
        // Called by Unity https://docs.unity3d.com/ScriptReference/MonoBehaviour.OnValidate.html
        protected void OnValidate()
        {
            try
            {
                // Validate that all references are connected
                Assert.IsNotNull(_videoRender);
            }
            catch (Exception)
            {
                Debug.LogError($"Some of the references are NULL, please inspect the {nameof(PeerView)} script on this object", this);
            }
        }
#endif

        [SerializeField]
        private RawImage _videoRender;
   

    }
}