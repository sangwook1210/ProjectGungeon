using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    // 캐릭터의 타입을 나타내는 enum
    public enum CharType
    {
        Player, Normal, Boss, Wall
    }

    protected CharType charType;    // 캐릭터의 타입
    protected float maxHp;      // 캐릭터의 최대 체력   
    public float MaxHp
    {
        protected set { maxHp = value; }
        get { return maxHp; }
    }
    protected float hp;             // 캐릭터의 현재 체력
    public float Hp
    {
        protected set { hp = value; }
        get { return hp; }
    }
    protected float Speed;          // 캐릭터의 속도

    public bool IsReady = false; // 캐릭터가 준비되었음을 저장할 변수
    protected bool CanMove = true;  // 캐릭터가 움직일 수 있는지를 저장할 변수

    protected AudioSource charAudio;

    protected ItemDatabase _itemDB;

    // Character를 초기화하는 함수
    protected void CharInit(CharType type, float maxHp, float speed)
    {
        charType = type;
        MaxHp = maxHp;
        Hp = maxHp;
        Speed = speed;

        charAudio = gameObject.GetComponent<AudioSource>();
        _itemDB = GameObject.FindGameObjectWithTag("ItemDB").GetComponent<ItemDatabase>();
    }

    // Char의 Hp가 0보다 같거나 작아져 죽었을 때 실행될 함수
    protected virtual void CharDead()
    {
        Destroy(gameObject);
    }

    // Char의 Hp가 감소하는 함수
    protected virtual void CharHpDecrease(float damage)
    {
        Hp -= damage;

        if (Hp <= 0f)
        {
            CharDead();
        }
    }

    // Char가 Bullet에 맞았을 때 실행될 함수
    protected virtual void CharHit(Collider hitObject)
    {
        if (hitObject.gameObject.CompareTag("Bullet"))
        {
            float damage = hitObject.GetComponent<Bullet>().BulletHit(charType);
            CharHpDecrease(damage);

        }
    }

    // 매개변수로 입력받은 audioClip을 재생하는 함수
    protected void PlayAudioClip(AudioClip audioClip)
    {
        charAudio.PlayOneShot(audioClip, 1.0f);
    }

    protected void PlayAudioClip(List<AudioClip> audioClips)
    {
        int randomAudio = Random.Range(0, audioClips.Count);
        charAudio.PlayOneShot(audioClips[randomAudio], 1.0f);
    }

    private void OnTriggerEnter(Collider hitObject)
    {
        if (IsReady)
        {
            CharHit(hitObject);
        }
    }
}
