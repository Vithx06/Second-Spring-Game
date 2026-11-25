using UnityEngine;

namespace SecondSpring.UnityGame
{
    [ExecuteAlways,RequireComponent(typeof(Camera))]
    public sealed class FarmSceneCamera : MonoBehaviour
    {
        private void OnEnable() { Fit(); }
        private void LateUpdate() { Fit(); }
        private void Fit()
        {
            var camera=GetComponent<Camera>();
            float mapAspect=1672f/941f,screenAspect=Screen.width/(float)Mathf.Max(1,Screen.height);
            camera.orthographic=true;camera.orthographicSize=4.705f;
            camera.rect=screenAspect>mapAspect?new Rect((1-mapAspect/screenAspect)/2,0,mapAspect/screenAspect,1)
                :new Rect(0,(1-screenAspect/mapAspect)/2,1,screenAspect/mapAspect);
        }
    }
}
