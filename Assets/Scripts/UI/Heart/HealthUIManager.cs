using UnityEngine;

public class HealthUIManager : MonoBehaviour
{
    public PlayerHealth player;
    public HeartUI[] hearts;          // Inspector’da 10 adet
    const int HP_PER_HEART = 10;      // 100 HP / 10 kalp = 10
    const int HALF_STEP = 5;          // yarým kalp eþiði

    int lastHP = -1;

    void OnEnable()
    {
        if (player != null)
        {
            player.onDamaged.AddListener(Refresh); 
        }
        Refresh(); 
    }

    void OnDisable()
    {
        if (player != null)
            player.onDamaged.RemoveListener(Refresh);
    }

  
    public void Refresh()
    {
        if (!player) return;

        int hp = Mathf.Clamp(player.currentHP, 0, player.maxHP);
        if (hp == lastHP) return;
        lastHP = hp;

        
        for (int i = 0; i < hearts.Length; i++)
        {
           
            int heartStartHP = i * HP_PER_HEART; 

            int rem = hp - heartStartHP;

            int state;
            if (rem >= HP_PER_HEART) state = 2; 
            else if (rem >= HALF_STEP) state = 1; 
            else if (rem > 0) state = 1; 
            else state = 0; 

            hearts[i].SetState(state);
        }
    }

    
    void Update()
    {
        
        if (player && player.currentHP != lastHP)
            Refresh();
    }
}
