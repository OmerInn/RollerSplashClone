using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class BallRoadColor : MonoBehaviour
{ 
 [SerializeField] private LevelManager levelManager;
[SerializeField] private BallMovement ballMovement;
[SerializeField] private MeshRenderer ballMeshRenderer;

public int paintedRoadTiles = 0;

private void Start()
{
    //paint ball:
    ballMeshRenderer.material.color = levelManager.paintColor;

    //paint default ball tile:
    Paint(levelManager.defaultBallRoadTile, .5f, 0f);

    //paint ball road :
    ballMovement.onMoveStart += OnBallMoveStartHandler;
}

    private void OnBallMoveStartHandler(List<RoadTile> roadTiles, float totalDuration)
    {
        float stepDuration = totalDuration / roadTiles.Count;
        for (int i = 0; i < roadTiles.Count; i++)
        {
            RoadTile roadTile = roadTiles[i];
            if (!roadTile.isPainted)
            {
                float duration = totalDuration / 2f;
                float delay = i * (stepDuration / 2f);
                Paint(roadTile, duration, delay);

                //Check if Level Completed:
                if (paintedRoadTiles == levelManager.roadTileList.Count)
                {
                    levelManager.Invoke("Levelcomplete",1.5f);
                }
            }
        }
    }

    private void Paint(RoadTile roadTile, float duration, float delay)
{
    roadTile.meshRenderer.material
       .DOColor(levelManager.paintColor, duration)
       .SetDelay(delay);

    roadTile.isPainted = true;
    paintedRoadTiles++;
}
}
