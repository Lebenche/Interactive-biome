using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeerScript : MonoBehaviour
{
  public Animator deerAnimator;
  bool isDeerTouched;
  public Collider deerCol;
  public RaycastHit hitInfo;

  void Start()
    {
      deerCol = GetComponent<Collider>();
      deerAnimator = gameObject.GetComponent<Animator>();
      isDeerTouched = false;
      deerAnimator.SetBool("isDeerTouched", false);

  }

  void Update()
    {
    if (Input.GetMouseButtonDown(0)) {
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      if (deerCol.Raycast(ray, out hitInfo,100f)) {
        isDeerTouched = true;

      }

    }

    else { isDeerTouched = false; }

    if (isDeerTouched == false) {
      deerAnimator.SetBool("isDeerTouched", false);

    }

    if (isDeerTouched == true) {
      deerAnimator.SetBool("isDeerTouched", true);

    }
  }
}
