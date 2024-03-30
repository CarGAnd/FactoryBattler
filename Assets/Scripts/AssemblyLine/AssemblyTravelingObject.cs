using System.Collections;
using UnityEngine;

public class AssemblyTravelingObject : MonoBehaviour
{
    public void MoveToPiece(Vector2Int startPosition, Vector2Int endPosition, FactoryGrid grid)
    {
        Vector3 start = grid.GetCellCenter(startPosition);
        Vector3 end = grid.GetCellCenter(endPosition);
        StartCoroutine(MoveToPieceCoroutine(new Vector3(start.x, 0.4f, start.z), new Vector3(end.x, 0.4f, end.z)));
    }

    private IEnumerator MoveToPieceCoroutine(Vector3 startPosition, Vector3 endPosition)
    {
        float time = 0;
        float duration = 1f; // Duration of the movement, adjust as needed

        while (time < duration)
        {
            time += Time.deltaTime;
            transform.position = Vector3.Lerp(startPosition, endPosition, time / duration);
            yield return null;
        }
    }
}
