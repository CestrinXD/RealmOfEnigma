using UnityEngine;

public class Health : MonoBehaviour
{
     [Header("Stats")]
    
    public float maxHealth = 100f;
    public float maxMana = 50;
    public float maxStamina = 75;

    public float currentHealth;
    private float currentMana;
    private float currentStamina;
   [Header("UI Bars")]
    public BarController healthBar;
    public BarController manaBar;
    // public BarController staminaBar;


    void Start()
    {
        
        currentHealth = maxHealth;
        currentMana = maxMana;
        currentStamina = maxStamina;

        UpdateAllBars();
        // healthBar.SetHealth(currentHealth, maxHealth);

    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            Heal(10);
        }

       if (Input.GetKeyDown(KeyCode.K))
        {
            currentMana = Mathf.Max(currentMana - 5, 0);
            manaBar.SetValueBar(currentMana, maxMana);
        }
    }

    public void Damage(int amount)
    {
        healthBar.SetValueBar(currentHealth, maxHealth);

        if (amount < 0)
        {
            throw new System.ArgumentOutOfRangeException("Cannot have negative damage");
        }

        currentHealth -= amount;

        // IHittable hittable = GetComponent<IHittable>();
        // if (hittable != null)
        // {
        //     hittable.OnHit(hitDirection);
        // }

        if (currentHealth <= 0)
        {
            Die();
        }
        // if (damagePopupPrefab != null)
        // {
        //     GameObject popup = Instantiate(damagePopupPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
        //     popup.GetComponentInChildren<DamagePopup>().Setup(amount);
        // }
    }

    public void TakeDamage(int damage)
    {
        healthBar.SetValueBar(currentHealth, maxHealth);

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        
        if(currentHealth <= 0)
        {
            Die();
        }
    }


    public void Heal(int amount)
    {

        if (amount < 0)
        {
            throw new System.ArgumentOutOfRangeException("Cannot have negative healing");
        }

        bool wouldBeOverMaxHealth = currentHealth + amount > maxHealth;

        if (wouldBeOverMaxHealth)
        {
            currentHealth = maxHealth;
        }
        else
        {
            currentHealth += amount;
        }
        healthBar.SetValueBar(currentHealth, maxHealth);
        
    }

    void UpdateAllBars()
    {
        healthBar.SetValueBar(currentHealth, maxHealth);
        manaBar.SetValueBar(currentMana, maxMana);
        // staminaBar.SetValueBar(currentStamina, maxStamina);
    }

    public void Die()
    {
        Debug.Log("i am dead!");
        Destroy(gameObject);
    }
}
