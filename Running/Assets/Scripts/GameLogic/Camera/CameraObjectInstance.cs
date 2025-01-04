using Cinemachine;
using UnityEngine;
using Zenject;

namespace Running.CameraControl
{
    public class CameraObjectInstance : MonoBehaviour
    {
        [SerializeField] private CameraType _cameraType;
        [SerializeField] private Camera _cinemachineVirtualCamera;

        public Camera VCam => _cinemachineVirtualCamera;

        [Inject]
        private void OnInjected(CameraController cameraController)
        {
            Hide();
            cameraController.RegisterCamera(_cameraType, this);
        }

        private void OnValidate()
        {
            _cinemachineVirtualCamera ??= GetComponent<Camera>();
        }

        public void Show()
        {
            _cinemachineVirtualCamera.gameObject.SetActive(true);
        }

        public void Hide()
        {
            _cinemachineVirtualCamera.gameObject.SetActive(false);
        }
    }
}
