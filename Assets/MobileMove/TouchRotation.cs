using UnityEngine;

public class TouchRotation : MonoBehaviour
{
    public float sensitivity = 1;
    public float smoothing = 1.5f;

    public Transform character;
    Vector2 velocity;
    Vector2 frameVelocity;

    public FixedTouchField touch;

    private void Update()
    {
        Vector2 mouseDelta = new Vector2(touch.TouchDist.x, touch.TouchDist.y);
        Vector2 rawFrameVelocity = Vector2.Scale(mouseDelta, Vector2.one * sensitivity);
        frameVelocity = Vector2.Lerp(frameVelocity, rawFrameVelocity, 1 / smoothing);
        velocity += frameVelocity;
        velocity.y = Mathf.Clamp(velocity.y, -90, 90);

        transform.localRotation = Quaternion.AngleAxis(-velocity.y, Vector3.right);
        character.localRotation = Quaternion.AngleAxis(velocity.x, Vector3.up);
    }
}
