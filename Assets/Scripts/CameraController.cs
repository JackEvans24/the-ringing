using Cinemachine;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts
{
    public class CameraController : MonoBehaviour
    {
        private static CameraController instance;
        public static CameraController Instance { get { return instance; } }

        private CinemachineVirtualCamera[] cameras;
        private Transform currentCameraTranform;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                instance = this;
            }
        }

        private void Start()
        {
            this.FindCameras();
        }

        private void FindCameras()
        {
            this.cameras = FindObjectsOfType<CinemachineVirtualCamera>(true);
        }

        public void SetCurrentCamera(Transform boundary)
        {
            if (this.currentCameraTranform == boundary)
                return;

            var currentCamera = boundary.parent.GetComponentInChildren<CinemachineVirtualCamera>();
            if (currentCamera == null)
            {
                Debug.LogError($"No camera found in object children: {currentCamera.name}");
                return;
            }

            currentCamera.Priority = 1;
            if (cameras == null)
                this.FindCameras();

            foreach (var camera in cameras.Where(c => c != currentCamera))
                camera.Priority = 0;

            this.currentCameraTranform = boundary;
        }

        private void OnDestroy()
        {
            instance = null;
        }
    }
}
