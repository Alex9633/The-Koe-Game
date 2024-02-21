using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Vector3 offset = new Vector3(0, 3, -13), correction = new Vector3(0, 1, 0);
    private Vector3 velocity = Vector3.zero;
    [SerializeField] private GameObject player, player2, player3;
    private GameObject toFollow;
    private Rigidbody2D rb;

    private bool freeze = false, isMoving = false;
    private Vector2 targetPos;

    private void Start()
    {
       
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePosition2D = new Vector2(mousePosition.x, mousePosition.y);

            RaycastHit2D hit = Physics2D.Raycast(mousePosition2D, Vector2.zero);
            if (hit.collider != null && hit.transform.gameObject.name == "CameraZoomOut")
            {
                freeze = true;
                targetPos = hit.transform.position;
            }
        }

        if (!freeze)
        {
            if (player.activeSelf) toFollow = player;
            else if (player2.activeSelf) toFollow = player2;
            else toFollow = player3;
            rb = toFollow.GetComponent<Rigidbody2D>();

            Vector3 pos = toFollow.transform.position + offset;
            for (int i = 15; i <= 20; i++)
            {
                if (Mathf.Abs(rb.velocity.y) > i || Mathf.Abs(rb.velocity.x) > i) pos -= correction;
            }
            transform.position = Vector3.SmoothDamp(transform.position, pos, ref velocity, 0.25f);
        }

        else
        {
            if(!isMoving)
            {
                isMoving = true;
                StartCoroutine(MoveTopos());
            }
        }
    }

    private IEnumerator MoveTopos()
    {
        Vector2 initialPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < .5f)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / .5f);
            transform.position = Vector2.Lerp(initialPosition, targetPos, t);
            yield return null;
        }

        transform.position = targetPos;
    }
}
