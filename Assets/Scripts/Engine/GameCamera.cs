using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine
{
    public class GameCamera : MonoBehaviour
    {
        public Transform target;
        public bool fixedUpdate;
        public float x;
        public float y;
        public float z;
        public float rotationSpeed;
        public float upFactor;

        void Update()
        {
            if (!fixedUpdate && target)
            {
                transform.position = target.position + target.forward * z + Vector3.up * y + target.right * x;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Math.DirectionVector(transform.position, target.position + Vector3.up * upFactor)), rotationSpeed * Time.deltaTime);
            }
        }

        private void FixedUpdate()
        {
            if (fixedUpdate && target)
            {
                transform.position = target.position + target.forward * z + Vector3.up * y + target.right * x;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Math.DirectionVector(transform.position, target.position + Vector3.up * upFactor)), rotationSpeed * Time.deltaTime);
            }
        }
    }
}
