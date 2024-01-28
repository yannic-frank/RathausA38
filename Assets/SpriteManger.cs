
using UnityEngine;


public class SpriteManager : MonoBehaviour
{
    public Sprite front;
    public Sprite back;
    public Sprite left;
    public Sprite right;

    private SpriteRenderer childSpriteRenderer;
    private Vector2 lastPosition;

    // Start is called before the first frame update
    void Start()
    {
        // Assuming the SpriteRenderer is a child of the current GameObject
        childSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate the movement direction
        Vector2 currentPosition = transform.position;
        Vector2 direction = currentPosition - lastPosition;

        // Check if there is any movement
        if (direction.magnitude > 0.01f)  // You can adjust the threshold as needed
        {
            // Determine the dominant direction
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                // Horizontal movement
                if (direction.x > 0)
                {
                    // Moving right
                    childSpriteRenderer.sprite = right;
                }
                else
                {
                    // Moving left
                    childSpriteRenderer.sprite = left;
                }
            }
            else
            {
                // Vertical movement
                if (direction.y > 0)
                {
                    // Moving up
                    childSpriteRenderer.sprite = back;
                }
                else
                {
                    // Moving down
                    childSpriteRenderer.sprite = front;
                }
            }
        }

        // Update last position for the next frame
        lastPosition = currentPosition;
    }
}
