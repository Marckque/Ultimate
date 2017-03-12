using UnityEngine;

public class Entity : MonoBehaviour
{
    [SerializeField]
    protected float m_MaximumVelocityMagnitude = 10f; // !!! NOT IN USE FOR NOW !!!
    protected Rigidbody m_Rigidbody;

    protected void Awake()
    {
        SetRigidbody();
    }

    private void SetRigidbody()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb)
        {
            m_Rigidbody = rb;
        }
        else
        {
            Debug.LogError("No rigidbody for: " + name);
        }
    }

    public Transform GetTransform()
    {
        return transform;
    }

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    public Rigidbody GetRigidbody()
    {
        return m_Rigidbody;
    }

    public void ClampMaxVelocity()
    {
        GetRigidbody().velocity = Vector3.ClampMagnitude(GetRigidbody().velocity, m_MaximumVelocityMagnitude);
    }

    public void Activate()
    {
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}