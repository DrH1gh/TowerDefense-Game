using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum gameStatus
{
    next, play, gameover, win
}

public class GameManager : Singleton<GameManager> {

    [SerializeField]
    private int totalWaves = 10;

    [SerializeField]
    private Text totalMoneyLbl;

    [SerializeField]
    private Text curentWaveLbl;

    [SerializeField]
    private Text playButtonLbl;

    [SerializeField]
    private Text totalEscapeLbl;

    [SerializeField]
    private Button playButton;

    [SerializeField] //[SerializeField] - To make them public for the inspector, to set them. Not public for the game classes.
    private GameObject SpawnPoint;
    [SerializeField]
    private Enemy[] Enemies; //drag and drop la Enemy Prefab -> Element 0 - x

    [SerializeField]
    private int TotalEnemies = 3;
    [SerializeField]
    private int EnemyPerSpawn;


    private int waveNumber = 0;
    private int totalMoney = 10;
    private int totalEscaped = 0;
    private int roundEscaped = 0;
    private int totalKills = 0;
    private int whickEnemiesToSpawn = 0;

    private GameObject uiTowers;

    private gameStatus currentState = gameStatus.play;
    private AudioSource audioSource;
    private bool isPause = true;

    const float spawnDelay = 0.7f;

    public List<Enemy> EnemyList = new List<Enemy>();

    public int TotalEscaped
    {
        get
        {
            return totalEscaped;
        }
        set
        {
            totalEscaped = value;
        }
    }

    public int RoundEscaped
    {
        get
        {
            return roundEscaped;
        }
        set
        {
            roundEscaped = value;
        }
    }

    public int TotalKills
    {
        get
        {
            return totalKills;
        }
        set
        {
            totalKills = value;
        }
    }

    public int TotalMoney
    {
        get
        {
            return totalMoney;
        }set
        {
            totalMoney = value;
            totalMoneyLbl.text = totalMoney.ToString();
        }
    }

    public AudioSource AudioSource
    {
        get { return audioSource; }
    }


    // Use this for initialization
    void Start () {
        //StartCoroutine(SpawnEnemyInterval());
        playButton.gameObject.SetActive(false);
        uiTowers = GameObject.Find("TowerPanel");
        audioSource = GetComponent<AudioSource>();
        ShowMenu();
        
        ShowTowers(false);
    }

    void Update()
    {
        HandleEscape();
    }
   
    IEnumerator SpawnEnemyInterval()
    {
        if (EnemyPerSpawn > 0 && EnemyList.Count < TotalEnemies)
        {
          
            for (int i = 0; i < EnemyPerSpawn; i += 1)
            {
                if (EnemyList.Count < TotalEnemies)
                {
                   
                    //Fix

                    //Random generator
                    int x = UnityEngine.Random.Range(0, 3);
                    int y = UnityEngine.Random.Range(0, 2);
                    Enemy enemyType = Enemies[0];
                    if (waveNumber > 0 && waveNumber < 2) //waveNumer incepe de la 0
                        enemyType = Enemies[y];
                    else if (waveNumber > 2) //waveNumer incepe de la 0
                        enemyType = Enemies[x];
                    


                    Enemy newEnemy = Instantiate(enemyType);

                    newEnemy.transform.position = SpawnPoint.transform.position;
                    //parent spawn
                    newEnemy.transform.parent = GameObject.FindGameObjectWithTag("EnemiesGroup").transform;
                    
                }
            }

            yield return new WaitForSeconds(spawnDelay);
            StartCoroutine(SpawnEnemyInterval());
        }
    } 

    public void RegisterEnemy(Enemy enemy)
    {
        EnemyList.Add(enemy);
    }

    public void UnregisterEnemy(Enemy enemy)
    {
        EnemyList.Remove(enemy);
        Destroy(enemy.gameObject);
        
    }

    public void DistroyAllEnemies()
    {
        foreach(Enemy e in EnemyList)
        {
            Destroy(e.gameObject);
        }
        EnemyList.Clear(); //make it emty list again;
    }

    public void AddMoney(int amount)
    {
        TotalMoney += amount;
    }

    public void SubstractMoney(int amount)
    {
        TotalMoney -= amount;
    }

    public void IsWaveOver()
    {
        totalEscapeLbl.text = "Escaped " + TotalEscaped + "/10";
        if((RoundEscaped + TotalKills) == TotalEnemies)
        {
            SetCurentGameState();
            ShowMenu();
        }
    }

    public void SetCurentGameState()
    {
        if(TotalEscaped >= 10)
        {
            currentState = gameStatus.gameover;
        }else if( (waveNumber == 0) && (TotalKills + RoundEscaped ) == 0)
        {
            currentState = gameStatus.play;
        }else if(waveNumber >= totalWaves)
        {
            currentState = gameStatus.win;
        }else
        {
            currentState = gameStatus.next;
        }
    }

    public void ShowMenu()
    {
        switch (currentState)
        {
            case gameStatus.gameover:
                playButtonLbl.text = "Play Again!";
                audioSource.PlayOneShot(SoundManager.Instace.GameOver);
                break;

            case gameStatus.next:
                playButtonLbl.text = "Next Waive";
                break;

            case gameStatus.play:
                playButtonLbl.text = "Play Game";
                break;

            case gameStatus.win:
                playButtonLbl.text = "Play Game";
                
                break;
        }
        //Mereu arata butonu in showmenu
        playButton.gameObject.SetActive(true);
    }

    public void ShowTowers(bool show)
    {
        
        uiTowers.SetActive(show);
    }

    public void PlayButton_Pressed()
    {
        switch (currentState)
        {
            case gameStatus.next:
                waveNumber += 1;
                TotalEnemies += waveNumber + 1;
                break;
            default:
                TotalEnemies = 3;
                TotalEscaped = 0;
                TotalMoney = 10;
                TowerManager.Instace.DestroyAllTowers();
                TowerManager.Instace.RenameTagsBuildSites();
                totalMoneyLbl.text = TotalMoney.ToString();
                totalEscapeLbl.text = "Escaped " + TotalEscaped + "/10";
                audioSource.PlayOneShot(SoundManager.Instace.NewGame);
                break;

        }
        DistroyAllEnemies();
        TotalKills = 0;
        RoundEscaped = 0;
        curentWaveLbl.text = "Wave " + (waveNumber + 1);
        //Mic delay de 2 secunde la inceput :)
        StartCoroutine(WaitForSeconds(2));
        
        playButton.gameObject.SetActive(false);
        ShowTowers(true);

        //SartSpawning
        StartCoroutine(SpawnEnemyInterval());

    }

    private void HandleEscape()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TowerManager.Instace.disableDragSprite();
            TowerManager.Instace.towerButtonPressed = null;
        }
    }

    IEnumerator WaitForSeconds(int seconds)
    {
        Debug.Log("wait time " + seconds + "s");
        Time.timeScale = 0f;
        float pauseEndTime = Time.realtimeSinceStartup + seconds;
        while (Time.realtimeSinceStartup < pauseEndTime)
        {
            yield return 0;
        }
        Debug.Log("done");
        Time.timeScale = 1f;
    }

}
