using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKController : MonoBehaviour
{
  [SerializeField]
  public Transform leftLegTarget;
  [SerializeField]
  public Transform rightLegTarget;

  private Vector3 newLeftPosition;
  private Vector3 newRightPosition;

  private IKMotionController motionController;

  public LayerMask layerMask;
  public Transform rightFootBone;
  public Transform leftFootBone;

  public float stepDistance = 0.75f; // distance to move in front of root
  public float stepRadius = 4;
  public float standHeight = 1.0f;
  public float legLength = 3f;
  public float hipWidth = 1.0f;
  public float stepHeight = 0.25f;
  public float stepPeriodMax = 0.5f;
  public bool duckEnabled = false;
  public float duckPercentage = 0.0f;
  public float groundOffset = 0.25f;
  public float groundedTolerance = 0.1f;
  [SerializeField, Range(2,2)]
  public int legNumber = 2;

  private float leftStepProgress = 0.0f;
  private float rightStepProgress = 0.0f;

  private bool leftStepping = false;
  private bool rightStepping = false;
  private float stepPeriod;

  // arrays for feeties
  private Transform[] targetArray;
  private Transform[] boneArray;
  private Vector3[] newPositionArray;
  private Vector3[] legCenterArray;
  private bool[] isSteppingArray;
  private float[] stepProgressArray;

  private int leftLegIndex = 0;
  private int rightLegIndex = 1;

  struct LegPosition
  {
    public Vector3 worldPosition;
    public bool isGrounded;
  }

  // Start is called before the first frame update
  void Start()
  {
    // :)
    leftLegTarget.position = transform.position - transform.up*standHeight - transform.right*hipWidth;
    rightLegTarget.position = transform.position - transform.up*standHeight +  transform.right*hipWidth;
    motionController = GetComponent<IKMotionController>();
    stepPeriod = stepPeriodMax;

    // array-ific-ation
    targetArray = new Transform[legNumber];
    boneArray = new Transform[legNumber];
    isSteppingArray = new bool[legNumber];
    stepProgressArray = new float[legNumber];
    newPositionArray = new Vector3[legNumber];
    legCenterArray = new Vector3[legNumber];

    // leg targets
    targetArray[leftLegIndex] = leftLegTarget;
    targetArray[rightLegIndex] = rightLegTarget;

    // foot bones
    boneArray[leftLegIndex] = leftFootBone;
    boneArray[rightLegIndex] = rightFootBone;

    // stepping booleans
    isSteppingArray[leftLegIndex] = false;
    isSteppingArray[rightLegIndex] = false;

    // step progress
    stepProgressArray[leftLegIndex] = 0.0f;
    stepProgressArray[rightLegIndex] = 0.0f;

    // new positions selected
    newPositionArray[leftLegIndex] = leftLegTarget.position;
    newPositionArray[rightLegIndex] = rightLegTarget.position;

    // leg centers
    // legCenterArray[leftLegIndex] = new Vector3(-hipWidth,0.0f,0.0f);
    // legCenterArray[rightLegIndex] = new Vector3(hipWidth,0.0f,0.0f);
    legCenterArray[leftLegIndex] = -Vector3.right*hipWidth;
    legCenterArray[rightLegIndex] = Vector3.right*hipWidth;

  }

  Vector3 AirPosition(Vector3 refPosition)
  {
    Vector3 vel = motionController.rigidbody.velocity;
    Vector3 horizontalSpeed = new Vector3(vel.x, 0.0f, vel.z);
    if (vel.y >= 0)
    {
      // going up so tilt towards velocity vec
      return transform.position + transform.TransformVector(refPosition) - transform.up*(legLength*Mathf.Clamp(vel.y/2.0f, 0.5f, 1f)) - horizontalSpeed*0.5f;
    }
    else
    {
      return transform.position + transform.TransformVector(refPosition) - transform.up*(legLength*Mathf.Clamp(vel.y/2.0f, 0.5f, 1f)) + horizontalSpeed*0.5f;
    }
  }

  LegPosition FindGround(Vector3 legCenterOffset)
  {
    RaycastHit hit = new RaycastHit();
    LegPosition outputPosition = new LegPosition();
    Vector3 localOffset = transform.TransformVector(legCenterOffset);
    if (Physics.Raycast(transform.position + localOffset, -Vector3.up, out hit, legLength, layerMask))
    {
      // found ground
      Vector3 groundPosition = transform.position + localOffset - transform.up*(hit.distance - groundOffset);
      stepPeriod = (1.2f - Mathf.Clamp(motionController.rigidbody.velocity.magnitude/motionController.playerMaxSpeed,0.2f,1f)) * stepPeriodMax;
      Debug.DrawLine(transform.position + localOffset, groundPosition, Color.red);
      outputPosition.worldPosition = groundPosition;
      outputPosition.isGrounded = true;

      Debug.DrawLine(transform.position + localOffset, groundPosition, Color.green, 0.5f);

      return outputPosition;

    }
    outputPosition.worldPosition = Vector3.zero;
    outputPosition.isGrounded = false;
    return outputPosition;
  }

  void FootUpdate(int footIndex)
  {
    if (!CheckStepping() && CheckNeedStep(footIndex))
    {

      LegPosition legPosition = FindGround(legCenterArray[footIndex]);
      if (legPosition.isGrounded)
      {
        newPositionArray[footIndex] = legPosition.worldPosition;
        isSteppingArray[footIndex] = true;
      }

    }
    else if (isSteppingArray[footIndex])
    {
      stepProgressArray[footIndex] += Time.deltaTime/stepPeriod;
      targetArray[footIndex].position = SampleParabola(targetArray[footIndex].position, newPositionArray[footIndex], stepHeight, stepProgressArray[footIndex], Vector3.up);

      AngleToes(targetArray[footIndex], boneArray[footIndex]);
      if (stepProgressArray[footIndex] >= 1.0f)
      {
        isSteppingArray[footIndex] = false;
        stepProgressArray[footIndex] = 0.0f;
      }
    }
    // check flying foot
    // else if (!FootGrounded(targetArray[footIndex].position))
    else if (!FootGrounded(boneArray[footIndex].position) && !CheckStepping())
    {
      // things go here
      // Debug.Log("air foot " + footIndex + " positioning");
      targetArray[footIndex].position = AirPosition(legCenterArray[footIndex]);

    }

  }

  // Update is called once per frame
  void Update()
  {

    FootUpdate(leftLegIndex);
    FootUpdate(rightLegIndex);

  }


  bool CheckNeedStep(int footIndex)
  {
    Transform target = targetArray[footIndex];
    Transform bone = boneArray[footIndex];
    Vector3 legCenterOffset = legCenterArray[footIndex];

    if (Vector3.ProjectOnPlane(target.position - transform.position, transform.up).magnitude >= stepRadius)
    {
      // Debug.DrawLine(target.position, target.position + Vector3.ProjectOnPlane(target.position - transform.position, transform.up));
      Debug.Log("moving foot due to distance");
      return true;
    }
    // also check vertical but differently
    // if (Vector3.Project(target.position, -transform.up).magnitude >= legLength)
    // {
    //   // Debug.DrawLine(target.position, target.position + Vector3.ProjectOnPlane(target.position - transform.position, transform.up));
    //   Debug.Log("moving foot due to height");
    //   return true;
    // }

    // if feet are on wrong side of body lol
    Vector3 localOffset = transform.TransformVector(legCenterOffset);
    if (Vector3.Dot(Vector3.Project(target.position - transform.position, localOffset), localOffset) < 0.0f)
    {
      Debug.Log("feet on wrong sides, fixing");
      return true;
    }

    return false;
  }

  bool FootGrounded(Vector3 targetPosition)
  {
    if (Physics.Raycast(targetPosition, -transform.up, groundedTolerance + groundOffset, layerMask))
    {
      Debug.DrawLine(targetPosition, targetPosition + transform.up, Color.red, 0.5f);
      return true;
    }
    return false;
  }

  void AngleToes(Transform target, Transform bone)
  {
    Vector3 footAngleVector = target.position + transform.forward;
    target.LookAt(footAngleVector);
    Debug.DrawLine(target.position, target.position + footAngleVector, Color.red, 0.1f);
  }

  bool CheckStepping()
  {
    foreach (bool step in isSteppingArray)
    {
      if (step)
      {
        return true;
      }
    }
    return false;
  }

  Vector3 SampleParabola(Vector3 start, Vector3 end, float height, float t, Vector3 outDirection)
  {
    float parabolicT = t * 2 - 1;
    //start and end are not level, gets more complicated
    Vector3 travelDirection = end - start;
    Vector3 levelDirection = end - new Vector3(start.x, end.y, start.z);
    Vector3 right = Vector3.Cross(travelDirection, levelDirection);
    Vector3 up = outDirection;
    Vector3 result = start + t * travelDirection;
    result += ((-parabolicT * parabolicT + 1) * height) * up.normalized;
    return result;
  }
}
