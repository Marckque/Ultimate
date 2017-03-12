using UnityEngine;

public class Ball : Entity
{
    public void Throw(Vector3 from, Vector3 towards)
    {
        transform.position = from;
        GetRigidbody().velocity = towards;

        Activate();
    }

    protected void OnDrawGizmosSelected()
    {
        if (Application.isPlaying)
        {
            Ray ray = new Ray(GetPosition(), GetRigidbody().velocity);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                // Velocity
                Gizmos.color = Color.blue;
                Gizmos.DrawRay(GetPosition(), GetRigidbody().velocity);

                Gizmos.color = Color.green;
                Gizmos.DrawRay(hit.point, -GetRigidbody().velocity + new Vector3(GetRigidbody().velocity.x * 2, 0f, 0f));
            }
        }
    }
}