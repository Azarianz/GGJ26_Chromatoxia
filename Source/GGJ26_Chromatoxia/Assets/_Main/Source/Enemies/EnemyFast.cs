using UnityEngine;

public class EnemyFast : EnemyMaster
{
    [SerializeField] GameObject hitIndicator;
   [SerializeField] PlayerDamage dm;
    public override void Awake()
    {
        base.Awake();
        dm = hitIndicator.GetComponent<PlayerDamage>();
    }
    void Update()
    { 
       dm.TryDamagePlayer(player);
       Move();
       
        
    }

  
}
