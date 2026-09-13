using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace NutriAR.Services
{
    public sealed class CameraFeedController : MonoBehaviour
    {
        private RawImage preview;
        private Text status;
        private WebCamTexture cameraTexture;

        public void Initialize(RawImage previewImage, Text statusText)
        {
            preview = previewImage;
            status = statusText;
            StartCoroutine(StartCamera());
        }

        private IEnumerator StartCamera()
        {
            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);
            }

            if (!Application.HasUserAuthorization(UserAuthorization.WebCam))
            {
                SetStatus("Câmera não autorizada • use os exemplos abaixo");
                yield break;
            }

            if (WebCamTexture.devices.Length == 0)
            {
                SetStatus("Nenhuma câmera encontrada • modo demonstração");
                yield break;
            }

            var selected = WebCamTexture.devices[0];
            for (var i = 0; i < WebCamTexture.devices.Length; i++)
            {
                if (!WebCamTexture.devices[i].isFrontFacing)
                {
                    selected = WebCamTexture.devices[i];
                    break;
                }
            }

            cameraTexture = new WebCamTexture(selected.name, 1280, 720, 30);
            preview.texture = cameraTexture;
            cameraTexture.Play();
            SetStatus("Câmera ativa • centralize o alimento ou rótulo");
        }

        private void Update()
        {
            if (cameraTexture == null || !cameraTexture.isPlaying || preview == null)
            {
                return;
            }

            preview.rectTransform.localEulerAngles = new Vector3(0f, 0f, -cameraTexture.videoRotationAngle);
            var scaleY = cameraTexture.videoVerticallyMirrored ? -1f : 1f;
            preview.rectTransform.localScale = new Vector3(1f, scaleY, 1f);
        }

        private void OnDestroy()
        {
            if (cameraTexture != null && cameraTexture.isPlaying)
            {
                cameraTexture.Stop();
            }
        }

        private void SetStatus(string message)
        {
            if (status != null)
            {
                status.text = message;
            }
        }
    }
}

