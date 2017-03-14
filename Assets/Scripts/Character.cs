using UnityEngine;
using UnityEngine.UI;
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
    public string action = "TriggersR_";
}

[System.Serializable]
public class CharacterParameters
{
    // Movement
    [Header("Movement")]
    public float runVelocity = 500f;
    public float rotationSpeed = 15f;

    // Dash
    [Header("Dash")]
    public float dashVelocity = 1000f;
    public float dashDuration = 0.1f;
    public float dashStun = 0.025f;

    // Ball
    [Header("Ball")]
    public float throwForce = 5f;
    public float throwOffset = 2f;
    [Tooltip("Percentage of velocity magnitude pre-ball catch that will be applied to the next throw"), Range(-100f, 100f)]
    public float throwPreviousVelocityPercentage;
    public float maximumBallPossession = 3f;
}

[System.Serializable]
public class CharacterUI
{
    [Header("UI")]
    public Transform UI;

    [Header("Ball")]
    public Image possessionBallImage;
}

public class Character : Entity
{
    #region Variables
    [SerializeField]
    private CharacterParameters m_Parameters = new CharacterParameters();
    [SerializeField]
    private CharacterControls m_Controls = new CharacterControls();
    [SerializeField]
    private CharacterUI m_CharacterUI = new CharacterUI();

    // Other
    private bool m_LastStickUsed;

    // Movement
    private Vector3 m_MovementInput;
    private Vector3 m_LastMovementInput;
    private float m_PreviousVelocityMagnitude;

    // Dash
    private bool m_HasReleasedActionButton = true;
    private bool m_IsDashing;

    // Stun 
    private bool m_IsStun;

    // Ball related
    private Ball m_Ball;
    private float m_CatchTime;
    private Vector3 m_AimingInput;
    private Vector3 m_LastAimingInput;
    #endregion Variables

    protected void Update()
    {
        // Move and aim input
        MovementInput();
        AimInput();

        // Rotate
        Vector3 direction = m_AimingInput != Vector3.zero ? m_AimingInput : m_MovementInput;
        if (direction.magnitude > 0.8f)
        {
            RotateCharacter(direction);
        }

        // Shoot and dash input
        if (Input.GetAxisRaw(m_Controls.action + m_Controls.characterID) > 0f)
        {
            if (m_HasReleasedActionButton)
            {
                m_HasReleasedActionButton = false;

                if (!m_IsStun)
                {
                    StartCoroutine(Dash());
                }
                else
                {
                    ThrowBall();
                }
            }            
        }
        else
        {
            if (!m_IsDashing)
            {
                m_HasReleasedActionButton = true;
            }
        }

        if (m_Ball)
        {
            direction = m_LastStickUsed == true ? m_LastAimingInput : m_LastMovementInput;
            m_Ball.transform.position = transform.position + direction * m_Parameters.throwOffset;

            if (Time.time > m_CatchTime + m_Parameters.maximumBallPossession)
            {
                ThrowBall();
            }
        }
    }

    protected void MovementInput()
    {
        m_MovementInput = new Vector3(Input.GetAxis(m_Controls.horizontalMove + m_Controls.characterID), 0f, Input.GetAxis(m_Controls.verticalMove + m_Controls.characterID)).normalized;

        if (m_MovementInput.magnitude > 0.3f && !m_IsDashing)
        {
            m_LastMovementInput = m_MovementInput;
            m_LastStickUsed = false;
        }
    }

    protected void AimInput()
    {
        m_AimingInput = new Vector3(Input.GetAxis(m_Controls.horizontalAim + m_Controls.characterID), 0f, -Input.GetAxis(m_Controls.verticalAim + m_Controls.characterID)).normalized;

        if (m_AimingInput.magnitude > 0.3f)
        {
            m_LastAimingInput = m_AimingInput;
            m_LastStickUsed = true;
        }
    }

    protected void FixedUpdate()
    {
        if (!m_IsStun)
        {
            if (m_IsDashing)
            {
                if (m_AimingInput != Vector3.zero && m_MovementInput == Vector3.zero)
                {
                    Move(m_AimingInput, m_Parameters.dashVelocity);
                }
                else
                {
                    Move(m_LastMovementInput, m_Parameters.dashVelocity);
                }
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
        if (m_AimingInput != Vector3.zero)
        {
            GetRigidbody().velocity = direction * velocity * Time.fixedDeltaTime;
        }
        else if (!m_IsStun && m_MovementInput != Vector3.zero)
        {
            GetRigidbody().velocity = transform.forward * velocity * Time.fixedDeltaTime;
        }
        else
        {
            GetRigidbody().velocity = Vector3.zero;
        }
    }

    // Rotation
    protected void RotateCharacter(Vector3 targetDirection)
    {
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion newRotation = Quaternion.Slerp(GetRotation(), targetRotation, Time.deltaTime * m_Parameters.rotationSpeed);

        GetRigidbody().MoveRotation(newRotation);
    }

    // Ball related
    private void CatchBall(Ball ball)
    {
        m_Ball = ball;

        // Trail 
        m_Ball.ClearTrailRenderer();
        m_Ball.DeactivateTrailRenderer();

        StartCoroutine(BallPossession());
        
        m_Ball.DeactivateCollider();

        m_PreviousVelocityMagnitude = GetRigidbody().velocity.magnitude;

        m_IsStun = true;
    }

    private IEnumerator BallPossession()
    {
        m_CatchTime = Time.time;
        float time = 0f;
        float fill = 0f;

        m_CharacterUI.possessionBallImage.enabled = true;
        m_CharacterUI.possessionBallImage.fillAmount = 0f;
        m_CharacterUI.UI.transform.position = transform.position + Vector3.up;

        while (time < m_Parameters.maximumBallPossession)
        {
            if (!m_Ball)
            {
                m_CharacterUI.possessionBallImage.enabled = false;
                yield break;
            }

            time += Time.deltaTime;
            fill = ExtensionMethods.Remap(time, 0f, m_Parameters.maximumBallPossession, 0f, 1f);
            m_CharacterUI.possessionBallImage.fillAmount = fill;

            yield return new WaitForEndOfFrame();
        }

        m_CharacterUI.possessionBallImage.enabled = false;
    }

    private void ThrowBall()
    {
        if (m_Ball)
        {
            m_Ball.Throw(GetPosition() + (DirectionToConsider() * m_Parameters.throwOffset), // from 
                DirectionToConsider() * (m_Parameters.throwForce + (m_PreviousVelocityMagnitude * (1 + (m_Parameters.throwPreviousVelocityPercentage * 0.01f))))); // towards

            m_Ball = null;
            m_IsStun = false;
        }
    }

    private Vector3 DirectionToConsider()
    {
        return m_AimingInput != Vector3.zero ? m_AimingInput : m_MovementInput;
    }
}