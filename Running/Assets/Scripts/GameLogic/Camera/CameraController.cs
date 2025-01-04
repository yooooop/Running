using Cinemachine;
using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Running.CameraControl
{
    public class CameraController : MonoBehaviour
    {

        [SerializeField] private float transitionDuration = 1.0f;

        private readonly Dictionary<CameraType, CameraObjectInstance> _cameraObjects = new Dictionary<CameraType, CameraObjectInstance>();

        private CameraType _currentActiveCameraType = CameraType.None;

        public event EventHandler<CameraType> OnCameraSwitchFinishedEvent;

        public void RegisterCamera(CameraType cameraType, CameraObjectInstance cameraObjectInstance)
        {
            if (_cameraObjects.ContainsKey(cameraType))
            {
                Debug.LogError($"CameraType {cameraType} is already registered!");
                return;
            }

            _cameraObjects[cameraType] = cameraObjectInstance;

            // If this is the default camera, immediately switch to it
            if (cameraType == CameraType.Main)
            {
                _currentActiveCameraType = cameraType;
                cameraObjectInstance.Show();
            }
        }

        public void SwitchActiveCamera(CameraType cameraType)
        {
            if (cameraType == _currentActiveCameraType)
            {
                return;
            }

            Camera fromCamera = null;
            Camera toCamera = null;

            if (_cameraObjects.TryGetValue(_currentActiveCameraType, out CameraObjectInstance cameraObjectInstance))
            {
                cameraObjectInstance.gameObject.SetActive(false);
                if (_cameraObjects.TryGetValue(CameraType.Transition, out CameraObjectInstance transitionCamera))
                {
                    fromCamera = transitionCamera.VCam;
                    fromCamera.transform.position = cameraObjectInstance.transform.position;
                    fromCamera.transform.rotation = cameraObjectInstance.transform.rotation;
                    fromCamera.fieldOfView = cameraObjectInstance.VCam.fieldOfView;
                }
            }

            _currentActiveCameraType = cameraType;

            if (_cameraObjects.TryGetValue(_currentActiveCameraType, out cameraObjectInstance))
            {
                toCamera = cameraObjectInstance.VCam;
            }

            SmoothTransition(fromCamera, toCamera).Forget();
        }

        public Camera GetCamera(CameraType cameraType)
        {
            if (cameraType == CameraType.None || !_cameraObjects.TryGetValue(cameraType, out CameraObjectInstance cameraObjectInstance))
            {
                return null;
            }

            return cameraObjectInstance.VCam;
        }

        public async UniTask SmoothTransition(Camera fromCamera, Camera toCamera)
        {
            if (fromCamera == null || toCamera == null)
            {
                Debug.LogError("Cameras are not assigned!");
                return;
            }

            Vector3 startPosition = fromCamera.transform.position;
            Quaternion startRotation = fromCamera.transform.rotation;
            float startFOV = fromCamera.fieldOfView;

            Vector3 endPosition = toCamera.transform.position;
            Quaternion endRotation = toCamera.transform.rotation;
            float endFOV = toCamera.fieldOfView;

            float elapsedTime = 0f;

            fromCamera.gameObject.SetActive(true);
            toCamera.gameObject.SetActive(false);

            while (elapsedTime < transitionDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / transitionDuration;

                t = Mathf.SmoothStep(0f, 1f, t);

                fromCamera.transform.position = Vector3.Lerp(startPosition, endPosition, t);
                fromCamera.transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
                fromCamera.fieldOfView = Mathf.Lerp(startFOV, endFOV, t);

                await UniTask.Yield(PlayerLoopTiming.Update);
            }

            fromCamera.gameObject.SetActive(false);
            toCamera.gameObject.SetActive(true);

            OnCameraSwitchFinishedEvent?.Invoke(this, _currentActiveCameraType);
        }
    }
}
