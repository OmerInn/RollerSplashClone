using UnityEngine;
using GG.Infrastructure.Utils.Swipe;
using DG.Tweening;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Linq;
// added this line to sort tiles by distance from the ray's origin using LINQ Queries  ( Line 50 & 51 ):
//// LINQ Sorgularý kullanarak döþemeleri ýþýnýn kaynaðýndan uzaklýða göre sýralamak için bu satýrý ekledik


public class BallMovement : MonoBehaviour
{
    [SerializeField] private SwipeListener swipeListener;
    [SerializeField] private LevelManager levelManager;

    /// <summary>
    /// 1)kaydýrýcý yönünde bir raycast ekleyin
    /// 2)isabet nesnesinin dizisini al
    /// 3)son yol döþemesi konumunu al
    /// </summary>

    [SerializeField] private float stepDuration=0.1f;
    [SerializeField] private LayerMask wallsAndRoadsLayer;
    private const float MAX_RAY_DISTANCE = 10F;

    public UnityAction<List<RoadTile>, float> onMoveStart;


    private Vector3 moveDirection; //hareket yönü
    private bool canMove = true; //hareket edebilir

    // Start is called before the first frame update
    void Start()
    {
        //varsayýlan top pozisyonunu deðiþtir:
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
            //kaydýrma yönünde (toptan) raycast ekleyin
            RaycastHit[] hits = Physics.RaycastAll(transform.position, moveDirection, MAX_RAY_DISTANCE, wallsAndRoadsLayer.value)
                 .OrderBy(hit => hit.distance).ToArray();
                               
            Vector3 targetPosition = transform.position;

            int steps = 0;

            List<RoadTile> pathRoadTiles = new List<RoadTile>();
            //steps : hedefe ulaþmak için karo sayýsý
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.isTrigger) //road tile
                {
                    // boyanacak listeye yol karolarý ekleyin:
                    pathRoadTiles.Add(hits[i].transform.GetComponent<RoadTile>());
                }
                else //wall tile
                {
                    if (i==0) //duvarýn topa yakýn olduðu anlamýna gelir
                    {
                        canMove = true;
                        return;
                    }
                    //else:
                    steps = i;
                    targetPosition = hits[i - 1].transform.position; // i duvar ise ondan bir öncesi bizim road tile (yani ilerleyebildiðimiz ) yerdir.
                    break;
                }
            }
            //topu hedef Pozisyona taþý
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
