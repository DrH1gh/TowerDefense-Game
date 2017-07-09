using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TowerManager : Singleton<TowerManager> {

   
    public TowerButton towerButtonPressed { get; set; }
    private SpriteRenderer spriteRenderer;

    private List<Tower> towerList = new List<Tower>();
    private List<Collider2D> buildSideList = new List<Collider2D>();
    private Collider2D buildTile;
    // Use this for initialization
    void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        buildTile = GetComponent<Collider2D>();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))  
        {
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero); // tragem unde atingem de la pct. 0,0 (bottom left)

            if (hit.collider.tag == "BuildSide")
            {
                PlaceTower(hit);
                
                RegisterBuildSide(hit.collider);
            }

           
        }

        if (spriteRenderer.enabled)
        {
            FollowMouse();
        }
    }

    public void RegisterBuildSide (Collider2D buildTag)
    {
        buildSideList.Add(buildTag);
    }

    public void RegisterTower (Tower tower)
    {
        towerList.Add(tower);
    }

    public void RenameTagsBuildSites()
    {
        foreach(Collider2D t in buildSideList)
        {
            t.tag = "BuildSide";
        }
        buildSideList.Clear();
    }

    public void DestroyAllTowers()
    {
        foreach (Tower t in towerList)
        {
            Destroy(t.gameObject);
        }
        towerList.Clear();
    }
   
    public void PlaceTower(RaycastHit2D hit)
    {
        //Daca totul este ok aici
        bool buyT = BuyTower();
        if (!buyT) return;

        //Place tower
        if (!EventSystem.current.IsPointerOverGameObject() && towerButtonPressed != null)
        {
            Tower newTower = Instantiate(towerButtonPressed.TowerObject);
            newTower.transform.position = hit.transform.position;
            //Registe Tower
            RegisterTower(newTower);

            GameManager.Instace.AudioSource.PlayOneShot(SoundManager.Instace.TowerBuild);
            disableDragSprite();
            hit.collider.tag = "BuildSideFull"; //Cand se distruge sa-mi amintesc sa-l fac iar BuildSide..altfel nu merge sa pui pe el.
        }

        
    }

    public bool BuyTower()
    {
        if (towerButtonPressed.TowerPrice > GameManager.Instace.TotalMoney) return false;

        GameManager.Instace.SubstractMoney(towerButtonPressed.TowerPrice);
        return true;
    }

    public void SelectedTower(TowerButton towerSelected)
    {
        
        towerButtonPressed = towerSelected;
        //DragSprite
        enableDragSprite(towerButtonPressed.DragSprite);
        //Ascund meniu towers
        GameManager.Instace.ShowTowers(false);
        
    }

    public void FollowMouse()
    {
        transform.position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //Resetam layerele, sa fim sigur ca-l vedel..de multe ori e totusi sub layere....
        transform.position = new Vector2(transform.position.x, transform.position.y);
    }

    public void enableDragSprite(Sprite _sprite)
    {
        spriteRenderer.enabled = true;
        spriteRenderer.sprite = _sprite;
        
    }

    public void disableDragSprite()
    {
        spriteRenderer.enabled = false;
        //Enable menu
        GameManager.Instace.ShowTowers(true);
    }
}
