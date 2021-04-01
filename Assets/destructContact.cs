using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destructContact : MonoBehaviour
{

    private bool isDamaged;
    private float damagePercentage;
    private float damageScale;
    [SerializeField]
    public float damageVariance = 1f;
    [SerializeField]
    public float damageAmplitude = 1f;
    [SerializeField, Range(0,5)]
    public float healingFactor;

    private (Vector3, GameObject) damageToRemove;
    private List<(Vector3, GameObject)> damageObjects;
    // Start is called before the first frame update
    void Start()
    {
      damageObjects = new List<(Vector3, GameObject)>();
    }

    // Update is called once per frame
    void Update()
    {


      if (isDamaged)
      {
        bool removalFlag = false;

        foreach((Vector3, GameObject) dmg in damageObjects)
        {
          // figure out which way to move object
          Vector3 updateDirection = dmg.Item2.transform.position - dmg.Item1;
          dmg.Item2.transform.position += updateDirection*Time.deltaTime*healingFactor;

          Debug.Log("moving damage outward " + updateDirection);

          // if not contacting original target, delete
          if (Vector3.Distance(dmg.Item1, dmg.Item2.transform.position) >= 5f)
          {
            damageToRemove = dmg;
            removalFlag = true;
          }
        }

        if (removalFlag)
        {
          damageObjects.Remove(damageToRemove); // delete outside foreach
          GameObject.Destroy(damageToRemove.Item2);
        }

        // check if there are any damages left
        if (damageObjects.Count == 0)
        {
          isDamaged = false;
          Debug.Log("no longer damaged");
        }
      }

    }

    void OnCollisionEnter(Collision col){

      ContactPoint contactPoint = col.GetContact(0);
      GameObject damage = new GameObject();
      Shape shape = damage.AddComponent<Shape>();
      shape.shapeType = Shape.ShapeType.Sphere; // sphere
      shape.operation = Shape.Operation.Cut; // cut
      shape.colour = Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);

      damage.transform.position = col.gameObject.transform.position;
      damage.transform.parent = gameObject.transform;

      damage.transform.localScale = Vector3.one*(Random.Range(0.0f,damageVariance) + damageAmplitude);

      GameObject.Destroy(col.gameObject); // destroy projectile

      // add impact to list of game objects to move them around
      damageObjects.Add((contactPoint.point, damage));
      isDamaged = true; // we been hit

      Debug.Log("have been hit");
    }
}
