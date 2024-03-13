using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DunesGameplay
{
    public class BallMovement : MonoBehaviour
    {
        IA_DuneGameplay _ballInputs;
        InputAction IA_downForce;

        [SerializeField][Min(0)] float _baseForceSpeed = 0;
        [SerializeField][Min(0)] float _pressSpeed = 0;
        [SerializeField] float _gravityForce = 0;
        [SerializeField] float _perfectSlopeThreshold;
        [SerializeField] Color _finalColor;
        [SerializeField] TrailRenderer _trailRenderer;

        Rigidbody2D rb;
        SpriteRenderer rendered;

        public static Action<Transform> OnBallInitialized;

        bool _isPressingInput = false;
        public bool _canMove { get; set; } = false;

        private void Awake()
        {
            _ballInputs = new IA_DuneGameplay();
        }
        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            rendered = GetComponent<SpriteRenderer>();

            OnBallInitialized?.Invoke(transform);

            Color newColor = Color.white;
            newColor.a = 0;
            rendered.color = newColor;
            rb.gravityScale = 0;
        }
        private void OnEnable()
        {
            IA_downForce = _ballInputs.BallControls.DownSprint;
            IA_downForce.Enable();
        }
        private void OnDisable()
        {
            IA_downForce.Disable();
        }
        private void FixedUpdate()
        {
            _isPressingInput = IA_downForce.ReadValue<float>() > 0f;

            if (_canMove)
                Movement();

        }
        void Movement()
        {
            rb.gravityScale = _gravityForce;

            rb.AddForce(Vector2.right * _baseForceSpeed, ForceMode2D.Force);

            if (_isPressingInput)
                rb.AddForce(Vector2.down * _pressSpeed, ForceMode2D.Force);
        }
        [ContextMenu("INIT")]
        public void InitDebug() => Init(0);
        public void Init(float initTime)
        {
            Color newColor = Color.white;
            float alpha = 0;

            DOVirtual.Float(0, 1, initTime, (float v) =>
            {
                alpha = v;
                newColor.a = alpha;
                print(alpha);
                rendered.color = newColor;
                //print(rendered.color.a);
            }).OnComplete(() =>
            {
                _canMove = true;
            });
        }
        public void LerpColor()
        {
            Color newColor = Color.white;
            DOVirtual.Color(Color.white, _finalColor, 3, c =>
            {
                rendered.color = c;
                _trailRenderer.startColor = c;
                _trailRenderer.endColor = new Color(1, 1, 1, 0);

            }).OnComplete(() => { TransitionManager.Instance._sceneToTransitionIndex = 3; TransitionManager.Instance.FadeOut(Color.white); });
        }
        private void OnCollisionEnter2D(Collision2D collision)
        {
            //Vector3 contactNormal = collision.contacts[0].normal;

            //Vector3 hitTangent = Vector3.ProjectOnPlane(rb.velocity, contactNormal).normalized;

            //if (Vector3.Dot(hitTangent, GetCollisionVelocity(collision).normalized) > Mathf.Cos(_perfectSlopeThreshold * Mathf.Deg2Rad))
            //{
            //    Vector3 collisionDirection = collision.contacts[0].point + GetCollisionVelocity(collision).normalized;

            //    if (collisionDirection.x > collision.contacts[0].point.x && collisionDirection.y < collision.contacts[0].point.y)
            //    {
            //        print("PURRFECT");
            //    }
            //}
        }
        private void OnCollisionExit2D(Collision2D collision)
        {

        }
        Vector2 GetCollisionImpulse2D(Collision2D collision)
        {
            //gets the impulse from a 2D collision
            Vector2 impulse = Vector2.zero;

            ContactPoint2D contactPoint = collision.GetContact(0);

            impulse += contactPoint.normal * contactPoint.normalImpulse;
            impulse.x += contactPoint.tangentImpulse * contactPoint.normal.y;
            impulse.y -= contactPoint.tangentImpulse * contactPoint.normal.x;

            return impulse;
        }
        Vector2 GetCollisionVelocity(Collision2D collision)
        {
            //gets the rb's velocity at the moment of the collision
            Vector2 impulse = GetCollisionImpulse2D(collision);

            Rigidbody2D myBody = collision.otherRigidbody;
            return myBody.velocity - impulse / myBody.mass;
        }
    }
}
