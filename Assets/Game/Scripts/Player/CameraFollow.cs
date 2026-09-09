using UnityEngine;

namespace LightJam
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] Transform target;
        [SerializeField] Vector3 offset = new Vector3(0f, 1.1f, -10f);
        [SerializeField] float smooth = 6f;
        [SerializeField] Vector2 xBounds = new Vector2(-6.5f, 8.5f);

        public void SetTarget(Transform followTarget)
        {
            target = followTarget;
            if (target != null)
                transform.position = ClampPosition(target.position + offset);
        }

        void LateUpdate()
        {
            if (target == null)
                return;

            Vector3 desired = ClampPosition(target.position + offset);
            transform.position = Vector3.Lerp(transform.position, desired, 1f - Mathf.Exp(-smooth * Time.deltaTime));
        }

        Vector3 ClampPosition(Vector3 position)
        {
            position.x = Mathf.Clamp(position.x, xBounds.x, xBounds.y);
            return position;
        }
    }
}
