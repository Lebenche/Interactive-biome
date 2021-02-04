using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Break : MonoBehaviour
{
  public GameObject ObjToBreak;
  public Transform transformParent;
  public float breakForce = 5;
  Collider rockCol;
  public RaycastHit hitInfo;

  void Start()
    {
      rockCol = GetComponent<Collider>();
    }

    void Update()
    {
    if (Input.GetMouseButtonDown(0)) {
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      if (rockCol.Raycast(ray, out hitInfo, 100f)) {
        BreakObject();
        }
      }  
    }

  void BreakObject()
  {
    Instantiate(ObjToBreak, transform.position, transform.rotation, transformParent);
    foreach (Rigidbody rb in ObjToBreak.GetComponentsInChildren<Rigidbody>()) {
      Vector3 force = (rb.transform.position ).normalized * breakForce;
      rb.AddForce(force);
    }
    
    Destroy(gameObject);
  }
}
