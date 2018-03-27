using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : Pawn
{
    public GameObject hpDropPrefab;
    public Animator animator;
    protected Player player;
    public GameObject DmgInfoPrefab;
    public float range;
    public float wakeUpDistance;
    protected float distance;
    protected float heightDifference;
    private Color basicColor;

    [HideInInspector]
    public int maxHpValue;

    public ResourceBar HPBar;

    // Use this for initialization
    protected void Start()
    {
        maxHpValue = this.hp;
        HPBar.maxValue = maxHpValue;
        player = GameObject.FindWithTag("Player").GetComponent<Player>();
        basicColor = GetComponentInChildren<SkinnedMeshRenderer>().materials[0].color;
    }

    // Update is called once per frame
    protected void Update()
    {
        HPBar.value = hp;
        CheckIfDead();
        if (!player.timeStop)
        {
            EnemyBehaviour();
        }
    }


    protected virtual void EnemyBehaviour()
    {
        Vector3 playerPosition = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
        transform.LookAt(playerPosition);
        distance = Vector3.Distance(player.transform.position, transform.position);
        heightDifference = System.Math.Abs(player.transform.position.y - transform.position.y);
    }

    protected void CheckIfDead()
    {
        if (this.hp == 0)
        {
            RollForHpPickUp();
            Destroy(gameObject);
        }
    }

    protected void RollForHpPickUp()
    {
        var a = Random.Range(0, 6);
        if (a == 0 || a == 5)
        {
            var hpPickUp = (GameObject)Instantiate(
                hpDropPrefab,
                new Vector3(this.transform.position.x, this.transform.position.y + 0.25f, this.transform.position.z),
                this.transform.rotation);
        }
    }

    protected virtual void Movement()
    {
        // to override
    }

    protected virtual void Attack()
    {
        // to override
    }

    protected void SpawnDMGinfo()
    {
        GameObject info = Instantiate(DmgInfoPrefab) as GameObject;
        RectTransform infoRect = info.GetComponent<RectTransform>();
        info.transform.SetParent(transform.Find("Canvas"));
        infoRect.transform.localPosition = DmgInfoPrefab.transform.localPosition;
        infoRect.transform.localScale = DmgInfoPrefab.transform.localScale;
        Destroy(info, 2);
        StopCoroutine(FlashOnHit());
        GetComponentInChildren<SkinnedMeshRenderer>().materials[0].color = Color.red;
        StartCoroutine(FlashOnHit());
    }

    private IEnumerator FlashOnHit()
    {
        yield return new WaitForSeconds(0.25f);
        GetComponentInChildren<SkinnedMeshRenderer>().materials[0].color = basicColor;
    }

    public override void Damage()
    {
        base.Damage();
        SpawnDMGinfo();
    }

}
