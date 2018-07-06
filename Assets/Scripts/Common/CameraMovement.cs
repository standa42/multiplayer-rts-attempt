using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Common
{
    public class CameraControl : MonoBehaviour
    {
        private Vector2 CameraMovementSpeed = new Vector2(20,12);
        private float ZoomSpeed = 1f;
        private GameObject camera;

        public CameraControl(TouchInput touchInput)
        {
            camera = GameObject.Find("Main Camera");
            ResetCameraPosition();

            touchInput.SlideEvent += Slide;
        }

        // Update is called once per frame
        void Update()
        {
            HandleKeyboardInputs();
        }

        private void ResetCameraPosition()
        {
            camera.transform.position = new Vector3(8, 0, -8f);
            camera.transform.rotation = Quaternion.Euler(-25, 0, 0);
        }

        void HandleKeyboardInputs()
        {


        }

        public void Slide(Vector2 vector)
        {
            double xDiff = -(vector.x / Screen.width) * CameraMovementSpeed.x;
            double yDiff = -(vector.y / Screen.height) * CameraMovementSpeed.y;

            camera.transform.position += new Vector3((float)xDiff,(float)yDiff,0);
        }
    }

}
