using System.Collections.Generic;
using HexKit3D;
using UnityEngine;

public class TreeNode : FixedMapElement
{
    [Header("Tree Node")]
    [SerializeField] LootTable loot;
    [SerializeField] Animator anim;
    [SerializeField] float maxHp = 100.0f;
    [SerializeField] float respawnTime = 20.0f;
    [field: SerializeField] public float requiredTier { get; private set; } = 0;
    public Lumberjack queuedLumberjack;

    [SerializeField] ParticleSystem chopParticle;
    public bool available { get; private set; } = true;
    float hp, counter = 0.0f;
    private void Awake()
    {
        hp = maxHp;
    }
    readonly int fallID = Animator.StringToHash("Fall");
    readonly int regenID = Animator.StringToHash("Regen");
    private void Update()
    {
        if (!available)
        {
            counter = Mathf.Min(counter + Time.deltaTime, respawnTime);
            anim.SetFloat(regenID, counter / respawnTime);
            if(counter >= respawnTime)
            {
                hp = maxHp;
                available = true;
            }
        }
    }
    public void GetDamage(float damage)
    {
        hp -= damage;
        if(hp <= 0.0f)
        {
            hp = 0.0f;
            counter = 0.0f;
            available = false;
            anim.SetTrigger(fallID);
            queuedLumberjack = null;
            foreach (var i in loot.GetLoot()) EmpireManager.Instance.AddItem(i.item, i.count);
        }
        else
        {
            chopParticle.transform.rotation = Quaternion.Euler(0, queuedLumberjack.transform.eulerAngles.y + 90.0f, 0);
            chopParticle.Play();
        }
    }
}