using UnityEngine;

namespace Story
{
    public class FirstPersonCameraBehaviour : MonoBehaviour
    {
        Transform _cameraHolderT;
        private void OnEnable()
        {
            PlayerMovement.OnViewMoved += SetCameraTransform;
        }
        private void OnDisable()
        {
            PlayerMovement.OnViewMoved -= SetCameraTransform;
        }
        void SetCameraTransform(Transform t) => _cameraHolderT = t;
        private void LateUpdate()
        {
            if (_cameraHolderT == null) return;

            transform.position = _cameraHolderT.position;
            transform.rotation = Quaternion.Slerp(transform.rotation, _cameraHolderT.rotation, 20 * Time.deltaTime);
        }
    }
}
