using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    public bool paused;
    public bool updatingRotationManually = false;
    //  public Dropdown dropdown;
    // public PauseMenu pauseMenu;
    //  [HideInInspector]
    //public bool focusPause = false;
    // [SerializeField] private GameObject groundCheck;

    public float maxWalkSpeed = 4.5f;
    [SerializeField]
    public float m_WalkSpeed; 
    [SerializeField]
    public UnityStandardAssets.Characters.FirstPerson.MouseLook m_MouseLook;
    [SerializeField]
    public bool m_UseHeadBob;
    [SerializeField]
    private CurveControlledBob m_HeadBob = new CurveControlledBob();
    [SerializeField]
    private float m_StepInterval;

    public Camera m_Camera;
    private float m_YRotation;
    private Vector2 m_Input;
    private Vector3 m_MoveDir = Vector3.zero;
    private CharacterController m_CharacterController;
    private CollisionFlags m_CollisionFlags;
    private Vector3 m_OriginalCameraPosition;
    private float m_StepCycle;
    private float m_NextStep;
    private float horizontal = 0;
    private float vertical = 0;

    private RaycastHit rayHit;

    //private FMODUnity.StudioEventEmitter m_PlayerFootsteps;
    //private SurfaceMaterial currentSurface;

    // Use this for initialization
    private void Start()
    {
        m_CharacterController = GetComponent<CharacterController>();
        m_Camera = Camera.main;
        m_OriginalCameraPosition = m_Camera.transform.localPosition;
        m_HeadBob.Setup(m_Camera, m_StepInterval);
        m_StepCycle = 0f;
        m_NextStep = m_StepCycle / 2f;
        m_MouseLook.Init(transform, m_Camera.transform);
        //	m_PlayerFootsteps = GetComponent<FMODUnity.StudioEventEmitter> ();
    }


    // Update is called once per frame
    private void Update()
    {
        if (!paused && !updatingRotationManually)
            RotateView();
        CharacterUpdate();
    }

    private void CharacterUpdate()
    {
        if (!paused)
        {
            float speed;
            GetInput(out speed);
            Debug.Log(speed);
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                               m_CharacterController.height / 2f);

            if (hitInfo.collider && !hitInfo.collider.isTrigger)
            {
                // desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;
                desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal);
                desiredMove = Vector3.ClampMagnitude(desiredMove, 1);
            }

            m_MoveDir.x = desiredMove.x * speed;
            m_MoveDir.z = desiredMove.z * speed;

            m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.deltaTime);

            ProgressStepCycle(speed);
            UpdateCameraPosition(speed);
        }
    }
    public void AdjustSensitivity(float sens)
    {
        // if (pauseMenu.MouseControl)
        //  {
        m_MouseLook.XSensitivity = sens;
        m_MouseLook.YSensitivity = sens;
        //  }  
        //   else
        //   {
        //       m_MouseLook.XSensitivity = sens * 2;
        //        m_MouseLook.YSensitivity = sens * 2;
        //    }   
    }

    private void ProgressStepCycle(float speed)
    {
        if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
        {
            m_StepCycle += (m_CharacterController.velocity.magnitude + speed) * Time.deltaTime;
        }

        if (!(m_StepCycle > m_NextStep))
        {
            return;
        }

        m_NextStep = m_StepCycle + m_StepInterval;

        //	PlayFootStepAudio ();
    }

    //private void PlayFootStepAudio()
    //{
    //	Physics.Raycast(groundCheck.transform.position, groundCheck.transform.TransformDirection(Vector3.down), out rayHit, 10);

    //	if (rayHit.transform) //if not null
    //	{
    //		if (rayHit.transform.GetComponent<FloorTag>() && rayHit.transform.GetComponent<FloorTag>().floorTag != SurfaceMaterial.None) //if floor
    //		{
    //                  SurfaceMaterial floorTag = rayHit.transform.GetComponent<FloorTag>().floorTag;
    //                  if (currentSurface != floorTag)
    //                  {
    //                      //Debug.Log(rayHit.transform.GetComponent<FloorTag>().floorTag);
    //                      if (rayHit.transform.GetComponent<FloorTag>().floorTag == SurfaceMaterial.Wood)
    //                      {
    //                          m_PlayerFootsteps.SetParameter("SurfaceMaterial", (float)SurfaceMaterial.Wood);
    //                          currentSurface = SurfaceMaterial.Wood;
    //                      }
    //                      else if (rayHit.transform.GetComponent<FloorTag>().floorTag == SurfaceMaterial.Tile)
    //                      {
    //                          m_PlayerFootsteps.SetParameter("SurfaceMaterial", (float)SurfaceMaterial.Tile);
    //                          currentSurface = SurfaceMaterial.Tile;
    //                      }
    //                      else if (rayHit.transform.GetComponent<FloorTag>().floorTag == SurfaceMaterial.Carpet)
    //                      {
    //                          m_PlayerFootsteps.SetParameter("SurfaceMaterial", (float)SurfaceMaterial.Carpet);
    //                          currentSurface = SurfaceMaterial.Carpet;
    //                      }

    //                  }
    //                  m_PlayerFootsteps.Play();
    //		}
    //	}
    //}

    private void UpdateCameraPosition(float speed)
    {

        Vector3 newCameraPosition;
        if (!m_UseHeadBob)
        {
            return;
        }
        if (m_CharacterController.velocity.magnitude > 0)
        {
            m_Camera.transform.localPosition =
                m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude + speed);
            newCameraPosition = m_Camera.transform.localPosition;
            newCameraPosition.y = m_Camera.transform.localPosition.y;
        }
        else
        {
            newCameraPosition = m_Camera.transform.localPosition;
            newCameraPosition.y = m_OriginalCameraPosition.y;
        }
        m_Camera.transform.localPosition = newCameraPosition;
    }


    private void GetInput(out float speed)
    {
        horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
        vertical = CrossPlatformInputManager.GetAxis("Vertical");

        speed = m_WalkSpeed;
        m_Input = new Vector2(horizontal, vertical);

        // normalize input if it exceeds 1 in combined length:
        if (m_Input.sqrMagnitude > 1)
        {
            m_Input.Normalize();
        }
    }


    private void RotateView()
    {
        m_MouseLook.LookRotation(transform, m_Camera.transform);

    }


    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        //Rigidbody body = hit.collider.attachedRigidbody;
        ////dont move the rigidbody if the character is on top of it
        //if (m_CollisionFlags == CollisionFlags.Below)
        //{
        //    return;
        //}

        //if (body == null || body.isKinematic)
        //{
        //    return;
        //}
        //body.AddForceAtPosition(m_CharacterController.velocity*0.1f, hit.point, ForceMode.Impulse);
    }
}