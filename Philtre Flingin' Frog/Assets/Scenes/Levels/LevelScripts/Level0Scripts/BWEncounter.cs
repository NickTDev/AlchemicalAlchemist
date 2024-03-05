using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BWEncounter : MonoBehaviour
{
   [SerializeField] GameObject objectiveSystem;

   private bool shouldLerp;
   [SerializeField] float lerpTime;
   [SerializeField] GameObject playerObject;
   private Vector3 startPos;
   private Vector3 endPos;
   private float elapsedTime;
   [SerializeField] Vector3 targetScale;
   private Vector3 startScale;

   private void Start()
   {
      objectiveSystem.SetActive(false);
      startPos = transform.position;
      startScale = transform.localScale;

      playerObject = GameObject.FindGameObjectWithTag("Player");
   }

   public void EnableObjectives(){
      objectiveSystem.SetActive(true);
      shouldLerp = true;
   }

   private void Update()
   {
   
      if(shouldLerp){
         elapsedTime += Time.deltaTime;
         float percentComplete = elapsedTime / lerpTime;
         endPos = playerObject.transform.position;

         transform.position = Vector3.Lerp(startPos, endPos, percentComplete);
         transform.localScale = Vector3.Lerp(startScale, targetScale, percentComplete);
         if(elapsedTime >= lerpTime){
            Destroy(gameObject);
         }
      }
   }
}
