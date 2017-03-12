using UnityEngine;
using System.Collections;

[System.Serializable]
public class CharacterControls
{
    [Header("Character ID")]
    public string characterID = "1";

    [Header("Controls")]
    public string horizontalMove = "L_XAxis_";
    public string verticalMove = "L_YAxis_";
    public string horizontalAim = "R_XAxis_";
    public string verticalAim = "R_YAxis_";
    public string dash = "RT_";
    public string throwBall = "LT_";
}

[System.Serializable]
public class CharacterParameters
{
    // Movement
    [Header("Movement")]
    public float runVelocity;

    // Dash
    [Header("Dash")]
    public float dashVelocity;
    public float dashDuration;
    public float dashStun;

    // Throw
    [Header("Throw")]
    public float throwForce;
    public float throwOffset;
}

public class Character : Entity
{
    #region Variables
    [SerializeField]
    private CharacterParameters m_Parameters = new CharacterParameters();
    [SerializeField]
    private CharacterControls m_Controls = new CharacterControls();

    // Ball related
    private Ball m_Ball;

    // Movement
    private Vector3 m_MovementInput;

    // Dash
    private bool m_IsDashing;
    private Vector3 m_LastMovementInput;

    // Stun 
    private bool m_IsStun;

    // Throw
    private Vector3 m_AimingInput;
    #endregion Variables

    protected void Update()
    {
        m_MovementInput = new Vector3(Input.GetAxis(m_Controls.horizontalMove + m_Controls.characterID), 0f, Input.GetAxis(m_Controls.verticalMove + m_Controls.characterID)).normalized;
        m_AimingInput = new Vector3(Input.GetAxis(m_Controls.horizontalAim + m_Controls.characterID), 0f, -Input.GetAxis(m_Controls.verticalAim + m_Controls.characterID)).normalized;

        Debug.DrawRay(GetPosition(), m_MovementInput, Color.green, 0.05f);
        Debug.DrawRay(GetPosition(), m_AimingInput, Color.cyan, 0.05f);

        if (m_MovementInput.magnitude > 0.3f && !m_IsDashing)
        {
            m_LastMovementInput = m_MovementInput;
        }

        if (Input.GetButtonDown(m_Controls.throwBall + m_Controls.characterID))
        {
            ThrowBall();
        } 

        if (Input.GetButtonDown(m_Controls.dash + m_Controls.characterID))
        {
            if (!m_IsDashing)
            {
                StartCoroutine(Dash());
            }
        }
	}

    protected void FixedUpdate()
    {
        if (!m_IsStun)
        {
            if (m_IsDashing)
            {
                Move(m_LastMovementInput, m_Parameters.dashVelocity);
            }
            else
            {
                Move(m_MovementInput, m_Parameters.runVelocity);
            }
        }
        else
        {
            GetRigidbody().velocity = Vector3.zero;
        }
    }

    protected void OnTriggerEnter(Collider other)
    {
        Ball ball = other.GetComponent<Ball>();

        if (ball)
        {
            CatchBall(ball);
        }
    }

    // States
    private IEnumerator Dash()
    {
        m_IsDashing = true;
        yield return new WaitForSeconds(m_Parameters.dashDuration);

        if (!m_IsStun)
        {
            m_IsStun = true;
            StartCoroutine(Stun(m_Parameters.dashStun));
        }

        m_IsDashing = false;
    }

    private IEnumerator Stun(float duration)
    {
        yield return new WaitForSeconds(duration);
        m_IsStun = false;
    }

    // Move
    protected void Move(Vector3 direction, float velocity)
    {
        GetRigidbody().velocity = direction * velocity * Time.fixedDeltaTime;
    }

    // Ball related
    private void CatchBall(Ball ball)
    {
        m_Ball = ball;
        m_Ball.Deactivate();
    }

    private void ThrowBall()
    {
        Vector3 direction = m_AimingInput != Vector3.zero ? m_AimingInput : m_MovementInput;

        if (m_Ball)
        {
            m_Ball.Throw(GetPosition() + (direction * m_Parameters.throwOffset), direction * m_Parameters.throwForce);
            m_Ball = null;
        }
    }
}