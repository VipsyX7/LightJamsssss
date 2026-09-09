using UnityEngine;

namespace LightJam
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] float moveSpeed = 5.2f;

        Rigidbody2D body;
        SpriteRenderer spriteRenderer;

        void Awake()
        {
            body = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            body.gravityScale = 3.2f;
            body.constraints = RigidbodyConstraints2D.FreezeRotation;
            body.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            body.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        void FixedUpdate()
        {
            float input = GameState.CanMove ? GameInput.Horizontal : 0f;
            Vector2 velocity = body.linearVelocity;
            velocity.x = input * moveSpeed;
            body.linearVelocity = velocity;

            if (Mathf.Abs(input) > 0.01f && spriteRenderer != null)
                spriteRenderer.flipX = input < 0f;
        }
    }
}
