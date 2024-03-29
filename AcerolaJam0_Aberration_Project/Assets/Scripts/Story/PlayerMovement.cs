using DG.Tweening;
using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Story
{
    public enum ControllerMode
    {
        Free,
        SoftLocked,
        Locked,
        LookOnly
    }
    public class PlayerMovement : MonoBehaviour
    {
        IA_Player _playerInputs;
        InputAction IA_move;
        InputAction IA_look;
        InputAction IA_interact;
        InputAction[] _inputActions;

        [Header("Movement")]
        [SerializeField][Min(0)] float _movementSpeed = 1;
        [SerializeField] float _movementSmoother = 20;
        [SerializeField] float _gravityScale = 1;
        [HideInInspector] public ControllerMode _controllerMode = ControllerMode.Free;
        public bool _isWalking { get; set; } = false;
        Vector2 _interpolatedAxis = Vector2.zero;

        [Header("Interact")]
        [SerializeField] float _interactDistance = 2;
        public Transform _rightHandT;
        RaycastHit _interactableHit;
        IInteractable _currentInteractable = null;
        bool _interactableInReach = false;

        [Header("Look")]
        public Transform _cameraPosT;
        [SerializeField][Min(0)] float _lookRotMultiplier = 1;

        [HideInInspector] public float _maxUpRot = 90;
        [HideInInspector] public float _maxDownRot = -90;

        [HideInInspector] public float _maxUpSoftRot = 30;
        [HideInInspector] public float _maxDownSoftRot = -30;
        [HideInInspector] public float _maxRightSoftRot = 30;
        [HideInInspector] public float _maxLeftSoftRot = -30;
        public float yRot { get; set; } = 0;
        public float xRot { get; set; } = 0;

        Vector3 _moveDirection = Vector2.zero;
        CharacterController controller;
        public Animator anim { get; private set; }

        public static Action<Transform> OnViewMoved;

        private void Awake()
        {
            _playerInputs = new IA_Player();
            controller = GetComponent<CharacterController>();
            anim = GetComponentInChildren<Animator>();
            //Cursor.lockState = CursorLockMode.Locked;

        }
        private void Start()
        {
            OnViewMoved?.Invoke(_cameraPosT);
        }
        private void OnEnable()
        {
            GameManager.OnGamePaused += GamePaused;

            IA_move = _playerInputs.Player.Move;

            IA_look = _playerInputs.Player.Look;

            IA_interact = _playerInputs.Player.Interact;
            IA_interact.performed += Interact;

            _inputActions = new InputAction[] { IA_move, IA_look, IA_interact };
            EnableInputActionsSwitch(_inputActions, true);
        }
        private void OnDisable()
        {
            GameManager.OnGamePaused -= GamePaused;

            EnableInputActionsSwitch(_inputActions, false);
        }
        private void Update()
        {
            if (_controllerMode != ControllerMode.Locked)
                MovePlayer();

            RotateView();

            if (_controllerMode != ControllerMode.Locked)
                Gravity();

            CheckForInteractables();
        }
        void MovePlayer()
        {
            _interpolatedAxis = Vector2.Lerp(_interpolatedAxis, IA_move.ReadValue<Vector2>(), Time.deltaTime * _movementSmoother);

            _moveDirection = transform.right.normalized * _interpolatedAxis.x + transform.forward.normalized * _interpolatedAxis.y;

            _isWalking = IA_move.ReadValue<Vector2>() == Vector2.zero ? false : true;

            anim.SetFloat("walkX", _interpolatedAxis.x);
            anim.SetFloat("walkY", _interpolatedAxis.y);

            anim.SetFloat("walkSpeed", _movementSpeed);

            anim.SetBool("isWalking", _isWalking);
            controller.Move(_moveDirection * Time.deltaTime * _movementSpeed);
        }
        void RotateView()
        {
            yRot += IA_look.ReadValue<Vector2>().x / 100f * _lookRotMultiplier;
            xRot += IA_look.ReadValue<Vector2>().y / 100f * _lookRotMultiplier;

            switch (_controllerMode)
            {
                case ControllerMode.Free:
                    xRot = Mathf.Clamp(xRot, _maxDownRot, _maxUpRot);
                    transform.rotation = Quaternion.Euler(0, yRot, 0);
                    _cameraPosT.rotation = Quaternion.Euler(-xRot, transform.rotation.eulerAngles.y, 0);
                    break;
                case ControllerMode.LookOnly:
                    xRot = Mathf.Clamp(xRot, _maxDownRot, _maxUpRot);
                    transform.rotation = Quaternion.Euler(0, yRot, 0);
                    _cameraPosT.rotation = Quaternion.Euler(-xRot, transform.rotation.eulerAngles.y, 0);
                    break;
                case ControllerMode.SoftLocked:
                    xRot = Mathf.Clamp(xRot, _maxDownSoftRot, _maxUpSoftRot);
                    yRot = Mathf.Clamp(yRot, _maxLeftSoftRot, _maxRightSoftRot);
                    _cameraPosT.localRotation = Quaternion.Euler(-xRot, yRot, 0);
                    break;
            }
        }
        void Gravity()
        {
            Vector3 gravity = new Vector3(0, -9.81f, 0);

            controller.Move(gravity * Time.deltaTime * _gravityScale);
        }
        void CheckForInteractables()
        {
            if (Physics.Raycast(_cameraPosT.position, _cameraPosT.forward, out _interactableHit, _interactDistance))
            {
                if (_interactableHit.transform.TryGetComponent(out IInteractable interactable))
                {
                    _currentInteractable = interactable;
                    _currentInteractable.Interactable(true);
                    _interactableInReach = true;
                    return;
                }
                if (_currentInteractable != null)
                    _currentInteractable.Interactable(false);

                _currentInteractable = null;
                _interactableInReach = false;
                return;
            }
            if (_currentInteractable != null)
                _currentInteractable.Interactable(false);

            _currentInteractable = null;
            _interactableInReach = false;
        }
        void Interact(InputAction.CallbackContext context)
        {
            if (!_interactableInReach) return;

            if (_currentInteractable != null)
            {
                _currentInteractable.Interact();
            }
        }
        void EnableInputActionsSwitch(InputAction[] inputActions, bool enable)
        {
            for (int i = 0; i < inputActions.Length; i++)
            {
                if (enable)
                    inputActions[i].Enable();
                else
                    inputActions[i].Disable();
            }
        }
        void GamePaused(bool gameIsPaused)
        {
            if (gameIsPaused)
                EnableInputActionsSwitch(_inputActions, !gameIsPaused);
            else
                SwitchControllerMode(_controllerMode);
        }
        public void SwitchControllerMode(ControllerMode controllerMode, float xDegrees = 0)
        {
            _controllerMode = controllerMode;

            switch (_controllerMode)
            {
                case ControllerMode.Free:
                    EnableInputActionsSwitch(_inputActions, true);
                    break;
                case ControllerMode.SoftLocked:
                    EnableInputActionsSwitch(new[] { IA_look }, true);
                    EnableInputActionsSwitch(new[] { IA_move, IA_interact }, false);
                    break;
                case ControllerMode.LookOnly:
                    EnableInputActionsSwitch(new[] { IA_look }, true);
                    EnableInputActionsSwitch(new[] { IA_move, IA_interact }, false);
                    break;
                case ControllerMode.Locked:
                    SlowlyLock(xDegrees);
                    break;
                default:
                    break;
            }
        }
        void SlowlyLock(float xDegrees)
        {
            EnableInputActionsSwitch(_inputActions, false);

            _isWalking = false;
            _interpolatedAxis = Vector2.zero;
            anim.SetBool("isWalking", _isWalking);

            _cameraPosT.DOLocalRotate(new Vector3(-xDegrees, 0, 0), 1).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                xRot = -_cameraPosT.localRotation.eulerAngles.x;
                yRot = transform.eulerAngles.y;

            });
        }

        [ContextMenu("Switch Controller Mode")]
        void DEBUG_SwitchControllerMode() => SwitchControllerMode(_controllerMode);

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(_cameraPosT.position, _cameraPosT.position + _cameraPosT.forward * _interactDistance);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(PlayerMovement))]
    public class PlayerMovementEditor : Editor
    {
        PlayerMovement pm;
        private void OnEnable() => pm = target as PlayerMovement;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            pm._controllerMode = (ControllerMode)EditorGUILayout.EnumPopup("Controller Mode", pm._controllerMode);

            switch (pm._controllerMode)
            {
                case ControllerMode.Free:
                    pm._maxUpRot = EditorGUILayout.FloatField("Max Up Rot", pm._maxUpRot);
                    pm._maxDownRot = EditorGUILayout.FloatField("Max Down Rot", pm._maxDownRot);
                    break;
                case ControllerMode.SoftLocked:
                    pm._maxUpSoftRot = EditorGUILayout.FloatField("Max Up Rot", pm._maxUpSoftRot);
                    pm._maxDownSoftRot = EditorGUILayout.FloatField("Max Down Rot", pm._maxDownSoftRot);
                    pm._maxRightSoftRot = EditorGUILayout.FloatField("Max Right Rot", pm._maxRightSoftRot);
                    pm._maxLeftSoftRot = EditorGUILayout.FloatField("Max Left Rot", pm._maxLeftSoftRot);
                    break;
                case ControllerMode.Locked:
                    break;
                default:
                    break;
            }

            if (GUI.changed)
                EditorUtility.SetDirty(pm);
        }
    }
#endif
}
