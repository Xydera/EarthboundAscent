using UnityEngine;

[ExecuteInEditMode]
public class ParallaxCamera : MonoBehaviour
{
    public delegate void ParallaxCameraDelegate(Vector2 deltaMovement);
    public ParallaxCameraDelegate onCameraTranslate;

    private Vector3 oldPosition;

    void Start()
    {
        oldPosition = transform.position;
    }

    void Update()
    {
        Vector3 newPosition = transform.position;

        if (newPosition != oldPosition)
        {
            if (onCameraTranslate != null)
            {
                Vector2 delta = new Vector2(oldPosition.x - newPosition.x, oldPosition.y - newPosition.y);
                onCameraTranslate(delta);
            }

            oldPosition = newPosition;
        }
    }
}