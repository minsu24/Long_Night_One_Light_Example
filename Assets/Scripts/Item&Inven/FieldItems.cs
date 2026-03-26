using UnityEngine;

public class FieldItems : MonoBehaviour
{
    public Item item;
    public SpriteRenderer image;

    public void SetItem(Item _item) //주은 아이템의 데이터로 변경
    {
        item.itemName = _item.itemName;
        item.itemSprite = _item.itemSprite;
        item.itemType = _item.itemType;
        item.efts = _item.efts;
        image.sprite = item.itemSprite;
    }

    public Item GetItem()
    {
        return item;
    }
    public void DestroyItem()
    {
        Destroy(gameObject);
    }
}
