using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemName; // 아이템의 이름
    public int id;  // 아이템의 고유 번호
    public ItemType itemType;   // 아이템의 타입
    public Sprite ItemSprite;   // 아이템의 스프라이트

    protected ItemData itemData;
    protected ItemDatabase _itemDB;

    protected AudioSource ItemAudio;
    public AudioClip SpawnClip;
    public AudioClip PickUpClip;

    protected void SetItemDatabase()
    {
        _itemDB = GameObject.FindGameObjectWithTag("ItemDB").GetComponent<ItemDatabase>();
    }

    protected virtual void SetItemData(int id)
    {
        SetItemDatabase();

        if (_itemDB != null)
        {
            itemData = _itemDB.FindItemDataWithId(id);
            ItemInit(itemData.name, itemData.id, itemData.itemtype, itemData.itemSprite);
        }

    }

    protected void ItemInit(string name, int id, ItemType itemType, Sprite itemSprite)
    {
        this.itemName = name;
        this.id = id;
        this.itemType = itemType;
        this.ItemSprite = itemSprite;

        ItemAudio = gameObject.GetComponent<AudioSource>();
    }

    // 아이템이 스폰되었을 때 실행될 함수
    protected void ItemSpawn()
    {
        PlayAudioClip(SpawnClip);
    }

    // 매개변수로 입력받은 audioClip을 재생하는 함수
    protected void PlayAudioClip(AudioClip audioClip)
    {
        ItemAudio.PlayOneShot(audioClip, 1.0f);
    }

    protected void PlayAudioClip(List<AudioClip> audioClips)
    {
        int randomAudio = Random.Range(0, audioClips.Count);
        ItemAudio.PlayOneShot(audioClips[randomAudio], 1.0f);
    }
}
