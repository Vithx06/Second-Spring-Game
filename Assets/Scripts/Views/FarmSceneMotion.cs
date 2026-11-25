using UnityEngine;

namespace SecondSpring.UnityGame
{
    public sealed class FarmSceneMotion : MonoBehaviour
    {
        public SpriteRenderer target;
        public Sprite[] frames;
        public float framesPerSecond = 6;
        public float rotationSpeed;
        public float driftSpeed;
        public float phase;
        private void Update()
        {
            if (rotationSpeed != 0) transform.Rotate(0,0,rotationSpeed * Time.deltaTime);
            if (driftSpeed != 0)
            {
                transform.position += Vector3.right * (driftSpeed * Time.deltaTime);
                if (transform.position.x > 10) transform.position = new Vector3(-10,transform.position.y,transform.position.z);
            }
            if (target && frames != null && frames.Length>0)
                target.sprite = frames[(int)((Time.time+phase)*framesPerSecond)%frames.Length];
        }
    }
}
