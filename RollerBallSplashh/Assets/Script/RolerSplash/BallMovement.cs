using UnityEngine;
using GG.Infrastructure.Utils.Swipe;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;
// added this line to sort tiles by distance from the ray's origin using LINQ Queries  ( Line 50 & 51 ):
//// LINQ Sorgular� kullanarak d��emeleri ���n�n kayna��ndan uzakl��a g�re s�ralamak i�in bu sat�r� ekledik


public class BallMovement : MonoBehaviour
{
    [SerializeField] private SwipeListener swipeListener;
    [SerializeField] private LevelManager levelManager;

    /// <summary>
    /// 1)kayd�r�c� y�n�nde bir raycast ekleyin
    /// 2)isabet nesnesinin dizisini al
    /// 3)son yol d��emesi konumunu al
    /// </summary>

    [SerializeField] private float stepDuration=0.1f;
    [SerializeField] private LayerMask wallsAndRoadsLayer;
    private const float MAX_RAY_DISTANCE = 10F;

    public UnityAction<List<RoadTile>, float> onMoveStart;


    private Vector3 moveDirection; //hareket y�n�
    private bool canMove = true; //hareket edebilir

    // Start is called before the first frame update
    void Start()
    {
        //varsay�lan top pozisyonunu de�i�tir:
        transform.position = levelManager.defaultBallRoadTile.position;
        swipeListener.OnSwipe.AddListener(swipe =>
        {
            switch (swipe)
            {
                case "Right":
                    moveDirection = Vector3.right;
                    break;
                case "Left":
                    moveDirection = Vector3.left;
                    break;
                case "Up":
                    moveDirection = Vector3.forward;
                    break;
                case "Down":
                    moveDirection = Vector3.back;
                    break;
            }
            MoveBall();
        });
    }

    private void MoveBall()
    {
        if (canMove)
        {
            canMove = false;
            //kayd�rma y�n�nde (toptan) raycast ekleyin
            RaycastHit[] hits = Physics.RaycastAll(transform.position, moveDirection, MAX_RAY_DISTANCE, wallsAndRoadsLayer.value)
                 .OrderBy(hit => hit.distance).ToArray();
                               
            Vector3 targetPosition = transform.position;

            int steps = 0;

            List<RoadTile> pathRoadTiles = new List<RoadTile>();
            //steps : hedefe ula�mak i�in karo say�s�
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.isTrigger) //road tile
                {
                    // boyanacak listeye yol karolar� ekleyin:
                    pathRoadTiles.Add(hits[i].transform.GetComponent<RoadTile>());
                }
                else //wall tile
                {
                    if (i==0) //duvar�n topa yak�n oldu�u anlam�na gelir
                    {
                        canMove = true;
                        return;
                    }
                    //else:
                    steps = i;
                    targetPosition = hits[i - 1].transform.position; // i duvar ise ondan bir �ncesi bizim road tile (yani ilerleyebildi�imiz ) yerdir.
                    break;
                }
            }
            //topu hedef Pozisyona ta��
            float moveDuration = stepDuration * steps;
            transform
               .DOMove(targetPosition, moveDuration)
               .SetEase(Ease.OutExpo)
               .OnComplete(() => canMove = true);

            if (onMoveStart != null)
                onMoveStart.Invoke(pathRoadTiles, moveDuration);
        }
    }
}
