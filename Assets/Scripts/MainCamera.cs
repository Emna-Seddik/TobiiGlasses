using UnityEngine;

namespace TobiiGlasses
{
    public class MainCamera : MonoBehaviour
    {
        public Transform target;
        private void Update()
        {
            Vector3 Position = target.transform.position;
            //transform.LookAt(Position);
        }
    }
}
