using UnityEngine;

namespace DunesGameplay
{
    public class CameraBehaviour : MonoBehaviour
    {
        Transform followTransform;
        [SerializeField] Vector3 _cameraOffset = Vector3.zero;
        [SerializeField] float _followSpeed = 0;

        private void OnEnable()
        {
            BallMovement.OnBallInitialized += SetFollowTransform;
        }
        private void OnDisable()
        {
            BallMovement.OnBallInitialized -= SetFollowTransform;
        }
        void SetFollowTransform(Transform t) => followTransform = t;

        private void Start()
        {
            Vector3 finalLerpPos = new Vector3(followTransform.position.x, Mathf.Clamp(followTransform.position.y / 1.5f, 7.5f, Mathf.Infinity), followTransform.position.z) + _cameraOffset;

            transform.position = finalLerpPos;
        }
        private void LateUpdate()
        {
            Vector3 finalLerpPos = new Vector3(followTransform.position.x, Mathf.Clamp(followTransform.position.y / 1.5f, 7.5f, Mathf.Infinity), followTransform.position.z) + _cameraOffset;

            transform.position = Vector3.Lerp(transform.position, finalLerpPos, Time.deltaTime * _followSpeed);
        }
    }
}
