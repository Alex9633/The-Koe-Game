using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleSolver : MonoBehaviour
{
    private Vector3 mouseStartPosition;
    public static bool PieceMoving = false;

    private void OnMouseDown()
    {
        mouseStartPosition = Input.mousePosition;
    }

    private void OnMouseUp()
    {
        Vector3 swipeDirection = Input.mousePosition - mouseStartPosition;
        float swipeDistance = swipeDirection.magnitude;
        swipeDirection.Normalize();

        float minSwipeDistance = 40f;

        if (swipeDistance > minSwipeDistance && !PieceMoving)
        {
            Vector3Int moveDirection = Vector3Int.zero;

            if (swipeDirection.x > 0.5f) moveDirection.x = 1;
            else if (swipeDirection.x < -0.5f) moveDirection.x = -1;
            else if (swipeDirection.y > 0.5f) moveDirection.y = 1;
            else if (swipeDirection.y < -0.5f) moveDirection.y = -1;

            if (IsValidMove(moveDirection)) MovePiece(moveDirection);
        }
    }

    private bool IsValidMove(Vector3Int moveDirection)
    {
        Vector2 dir = new Vector2(moveDirection.x, moveDirection.y);
        if (Physics2D.Raycast(new Vector2(transform.position.x + (.05f + GetComponent<BoxCollider2D>().size.x / 2) * moveDirection.x, transform.position.y + (.05f + GetComponent<BoxCollider2D>().size.x / 2) * moveDirection.y), dir, .1f, 1 << LayerMask.NameToLayer("Puzzle1")))
        {
            return false;
        }
        return true;
    }

    private void MovePiece(Vector3Int moveDirection)
    {
        if (moveDirection.x != 0) StartCoroutine(MoveToNextTile(new Vector2(transform.position.x + moveDirection.x * 5, transform.position.y)));
        else StartCoroutine(MoveToNextTile(new Vector2(transform.position.x, transform.position.y + moveDirection.y * 5)));
    }

    private IEnumerator MoveToNextTile(Vector2 targetPos)
    {
        PieceMoving = true;
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
        PieceMoving = false;
    }
}
