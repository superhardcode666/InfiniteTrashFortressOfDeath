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
    public class CameraFade : MonoBehaviour
    {
        // Rather than Lerp or Slerp, we allow adaptability with a configurable curve
        public AnimationCurve Curve = new(new Keyframe(0, 1),
            new Keyframe(0.5f, 0.5f, -1.5f, -1.5f), new Keyframe(1, 0));

        public bool enableFade;
        public Color fadeColor = Color.black;

        public float speedScale = 0.5f;
        public bool startFadedOut;
        private float alpha;

        private int direction;
        private Texture2D texture;
        private float time;

        public void OnGUI()
        {
            if (alpha > 0f) GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), texture);
            if (direction != 0)
            {
                time += direction * Time.deltaTime * speedScale;
                alpha = Curve.Evaluate(time);
                texture.SetPixel(0, 0, new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha));
                texture.Apply();
                if (alpha <= 0f || alpha >= 1f) direction = 0;
            }
        }

        public void OnCreated()
        {
            if (startFadedOut) alpha = -1f;
            else alpha = 0f;
            texture = new Texture2D(1, 1);
            texture.SetPixel(0, 0, new Color(fadeColor.r, fadeColor.g, fadeColor.b, alpha));
            texture.Apply();
        }

        public void OnUpdate()
        {
            if (direction == 0 && !enableFade)
            {
                if (alpha >= 1f) // Fully faded out
                {
                    alpha = 1f;
                    time = 0f;
                    direction = 1;
                }
                else // Fully faded in
                {
                    alpha = 0f;
                    time = 1f;
                    direction = -1;
                }

                enableFade = false;
            }
        }
    }
}