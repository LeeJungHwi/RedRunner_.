using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMove : MonoBehaviour
{
    // 컴포넌트
    public Transform startPos; // 처음 시작지점
    public Transform endPos; // 처음 도착지점
    public Transform desPos; // 현재 도착지점
    public float speed; // 속도 조절

    void Awake() // 초기화
    {
        // 처음 시작지점
        transform.position = startPos.position;

        // 처음 도착지점
        desPos = endPos;
    }

    void FixedUpdate() // 이동
    {
        // 타일 이동
        transform.position = Vector2.MoveTowards(transform.position, desPos.position, Time.deltaTime * speed);

        if(Vector2.Distance(transform.position, desPos.position) <= 0.05f) // 타일의 위치와 도착지점의 위치사이 거리가 0.05 이하이면 현재도착지점에 도달함
        {
            if(desPos == endPos) // 현재도착지점이 처음도착지점이면
                desPos = startPos; // 도착지점을 처음시작지점으로 셋
            else // 현재도착지점이 처음시작지점이면
                desPos = endPos; // 도착지점을 처음도착지점으로 셋
        }
    }
}
