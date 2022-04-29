using Cinemachine;
using UnityEngine;

/*      __  _____    ____  ____ 
       / / / /   |  / __ \/ __ \
      / /_/ / /| | / /_/ / / / /
     / __  / ___ |/ _, _/ /_/ / 
    /_/_/_/_/__|_/_/_|_/_____/_ 
      / ____/ __ \/ __ \/ ____/ 
     / /   / / / / / / / __/    
    / /___/ /_/ / /_/ / /___    
    \____/\____/_____/_____/ 
        アンフェタミンを燃料
*/
namespace Hardcode.ITFOD.Camera
{
    public class CameraController : MonoBehaviour
    {
        #region Field Declarations

        [SerializeField] private CinemachineVirtualCamera playerMainCam, playerZoomCam;
        [SerializeField] private float cameraSpeed = 5f;
        private CinemachineVirtualCamera currentCam;

        #endregion

        #region Start Up

        public void OnCreated()
        {
            currentCam = playerMainCam;
        }

        public void ZoomIn()
        {
            currentCam = playerZoomCam;

            playerZoomCam.Priority = 10;
            playerMainCam.Priority = 0;
        }

        public void ZoomOut()
        {
            currentCam = playerMainCam;

            playerZoomCam.Priority = 0;
            playerMainCam.Priority = 10;
        }

        public void MoveToTarget(Vector3 target)
        {
            var step = cameraSpeed * Time.deltaTime;

            // move sprite towards the target location
            transform.position = Vector3.MoveTowards(transform.position, target, step);
        }

        #endregion
    }
}