using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MFarm.Inventory
{
    public class Teleport : MonoBehaviour
    {
        public string sceneToGo;
        public Vector3 positionToGo;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                EventHeadler.CallTransitionEvent(sceneToGo, positionToGo);
            }
        }
    }
}