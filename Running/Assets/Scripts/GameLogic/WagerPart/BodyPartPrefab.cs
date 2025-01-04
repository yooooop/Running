using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Running.BodyPart
{
    public class BodyPartPrefab : MonoBehaviour
    {
        [SerializeField] private GameObject _organ;
        [SerializeField] private Rigidbody _body;

        public void ToggleBody()
        {
            _body.useGravity = true;
        }

    }
}