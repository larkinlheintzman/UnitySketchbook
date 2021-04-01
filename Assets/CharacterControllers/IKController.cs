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
  [SerializeField, Range(2,2)]
  public int legNumber = 2;

  private float groundedTolerance = 0.1f;
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
    // stepPeriod = 0.01f * stepPeriodMax;
    // Debug.Log("air step period set to: " + stepPeriod);
    return refPosition - transform.up*(legLength/2f);
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

      return outputPosition;

    }
    outputPosition.worldPosition = Vector3.zero;
    outputPosition.isGrounded = false;
    return outputPosition;
  }

  void FootUpdate(int footIndex)
  {
    if (!CheckStepping() && CheckNeedStep(targetArray[footIndex], boneArray[footIndex], legCenterArray[footIndex]))
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
  }

  // Update is called once per frame
  void Update()
  {

    FootUpdate(leftLegIndex);
    FootUpdate(rightLegIndex);

  }


  bool CheckNeedStep(Transform target, Transform bone, Vector3 legCenterOffset)
  {
    if (Vector3.ProjectOnPlane(target.position - transform.position, transform.up).magnitude >= stepRadius)
    {
      // Debug.DrawLine(target.position, target.position + Vector3.ProjectOnPlane(target.position - transform.position, transform.up));
      return true;
    }

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
