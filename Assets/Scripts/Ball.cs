using UnityEngine;

public class Ball : Entity
{
    [SerializeField]
    private CapsuleCollider m_CapsuleCollider;
    [SerializeField]
    private TrailRenderer m_TrailRenderer;

    public void Throw(Vector3 from, Vector3 towards)
    {
        transform.position = from;
        GetRigidbody().velocity = towards;

        ActivateCollider();
        ActivateTrailRenderer();
    }

    public void ResetBall(Vector3 spawn)
    {
        ClearTrailRenderer();
        DeactivateTrailRenderer();

        GetRigidbody().velocity = Vector3.zero;
        transform.position = spawn + Vector3.up * 2f;

        ActivateTrailRenderer();
    }

    public void ActivateCollider()
    {
        m_CapsuleCollider.enabled = true;
    }

    public void DeactivateCollider()
    {
        m_CapsuleCollider.enabled = false;
    }

    public void ClearTrailRenderer()
    {
        m_TrailRenderer.Clear();
    }
    
    public void ActivateTrailRenderer()
    {
        m_TrailRenderer.enabled = true;
    }

    public void DeactivateTrailRenderer()
    {
        m_TrailRenderer.enabled = false;
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