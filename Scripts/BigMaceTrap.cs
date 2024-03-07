using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigMaceTrap : MonoBehaviour
{
    // 컴포넌트
    Rigidbody2D rigid;
    public CharacterMove characterMove;
    AudioSource audioSource;
    public AudioClip audioFall;
    bool isFall = false;

    void Awake() // 초기화
    {
        rigid = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }

    void OnTriggerEnter2D(Collider2D collision) // Fall
    {
        if(collision.gameObject.tag == "Character") // 캐릭터와 충돌하면
        {
            // 중력에 영향을 받는다
            rigid.isKinematic = false;

            // 한번도 떨어지지 않은 경우 사운드 함수 호출
            if(isFall == false)
            {
                FallSound();
            }

            // 떨어진 상태
            isFall = true;
        }
    }

    void FallSound() // 사운드 함수
    {
        // 음원파일 지정
        audioSource.clip = audioFall;
        
        // 1초후 재생
        audioSource.PlayDelayed(1);
    }
}
