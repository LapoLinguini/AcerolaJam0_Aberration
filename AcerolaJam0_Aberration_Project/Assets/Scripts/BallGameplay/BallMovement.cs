using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace DunesGameplay
{
    public class BallMovement : MonoBehaviour
    {
        [SerializeField][Min(0)] float _baseForceSpeed = 0;
        [SerializeField][Min(0)] float _pressSpeed = 0;
        [SerializeField] float _gravityForce = 0;
        [SerializeField] float _perfectSlopeThreshold;

        Rigidbody2D rb;

        public GameObject triangle;

        [SerializeField] GameObject PS_BallTrail;
        GameObject _trailEffect;

        public static Action<Transform> OnBallInitialized;


        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            OnBallInitialized?.Invoke(transform);
        }
        private void FixedUpdate()
        {
            rb.gravityScale = _gravityForce;

            rb.AddForce(Vector2.right * _baseForceSpeed, ForceMode2D.Force);

            if (Input.GetMouseButton(0))
                rb.AddForce(Vector2.down * _pressSpeed, ForceMode2D.Force);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Vector3 contactNormal = collision.contacts[0].normal;

            Vector3 hitTangent = Vector3.ProjectOnPlane(rb.velocity, contactNormal).normalized;

            //GameObject triangle = Instantiate(this.triangle, collision.contacts[0].point, Quaternion.identity);
            //triangle.transform.up = GetsCollisionVelocity(collision);

            if (Vector3.Dot(hitTangent, GetsCollisionVelocity(collision).normalized) > Mathf.Cos(_perfectSlopeThreshold * Mathf.Deg2Rad))
            {
                print("PURRFECT");
            }
            //_trailEffect = Instantiate(PS_BallTrail, transform.position - new Vector3(), );
        }
        private void OnCollisionExit2D(Collision2D collision)
        {
            
        }
        Vector2 GetCollisionImpulse2D(Collision2D collision)
        {
            //gets impulse from a 2D collision
            Vector2 impulse = Vector2.zero;

            ContactPoint2D contactPoint = collision.GetContact(0);

            impulse += contactPoint.normal * contactPoint.normalImpulse;
            impulse.x += contactPoint.tangentImpulse * contactPoint.normal.y;
            impulse.y -= contactPoint.tangentImpulse * contactPoint.normal.x;

            return impulse;
        }
        Vector2 GetsCollisionVelocity(Collision2D collision)
        {
            //gets the rb's velocity at the moment of the collision
            Vector2 impulse = GetCollisionImpulse2D(collision);

            Rigidbody2D myBody = collision.otherRigidbody;
            return myBody.velocity - impulse / myBody.mass;
        }
    }
}
