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
        /// <summary>
        /// defines speed of camera movement in both axes
        /// </summary>
        private Vector2 CameraMovementSpeed = new Vector2(20,12);

        private GameObject camera;

        public CameraControl(TouchInput touchInput)
        {
            camera = GameObject.Find("Main Camera");
            ResetCameraPosition();

            touchInput.SlideEvent += Slide;
        }
        
        /// <summary>
        /// Reset camera to defined position over map
        /// </summary>
        private void ResetCameraPosition()
        {
            camera.transform.position = new Vector3(8, 0, -8f);
            camera.transform.rotation = Quaternion.Euler(-25, 0, 0);
        }
        
        /// <summary>
        /// Callback for slide event in TouchInput
        /// </summary>
        /// <param name="vector">touch position difference between two frames</param>
        public void Slide(Vector2 vector)
        {
            double xDiff = -(vector.x / Screen.width) * CameraMovementSpeed.x;
            double yDiff = -(vector.y / Screen.height) * CameraMovementSpeed.y;

            camera.transform.position += new Vector3((float)xDiff,(float)yDiff,0);

        }
    }

}
