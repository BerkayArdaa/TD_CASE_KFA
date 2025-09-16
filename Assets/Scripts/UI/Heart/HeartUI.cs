using UnityEngine;
using UnityEngine.UI;

public class HeartUI : MonoBehaviour
{
    public Sprite fullHeart;
    public Sprite halfHeart;
    public Sprite emptyHeart;

    Image img;

    void Awake() => img = GetComponent<Image>();

    // 2 = full, 1 = half, 0 = empty
    public void SetState(int state)
    {
        img.sprite = state switch
        {
            2 => fullHeart,
            1 => halfHeart,
            _ => emptyHeart
        };
    }
}
