using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneDesign : MonoBehaviour
{
    #region 변수
    #region 싱글톤
    private static SceneDesign instance = null;
    public static SceneDesign Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }
    #endregion
    //public
    //씬 순서 입력 받음
    public int randomNomal;
    public int randomGimmik;
    [Header(" ")]
    public int s_result;
    public int s_boss;
    public int s_nomal;
    public int s_gimmick;
    public int s_bonus;
    //씬 관리용 변수
    public int next;
    public bool mapClear;       //맵 클리어시 관리용
    public bool goBoss;         //보스로 가야할때
    //ResultCanvas에 보낼 변수
    public bool finalClear;     //게임 클리어시 관리용
    public int mapCounting;
    public float Timer = 0f;
    public int jellyInit;
    public int bossLevel;

    //private
    int bossCount;


    //singleton
    Slime slime;


    #endregion

    #region 유니티 함수
    void Awake()
    {
        if (null == instance)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

    }
    IEnumerator StraightClear()
    {
        while(!finalClear)
        {
            yield return null;
        }
        SceneManager.LoadScene(s_result);
    }
    private void Start()
    {
        slime = Slime.Instance;
        StartCoroutine(StraightClear());
    }
    private void Update()
    {
        if(!finalClear)
        {
            Timer += Time.deltaTime;
        }
        
    }
    #endregion

    #region 함수

    public void MapCount()
    {
        if(!goBoss)
        {
            bossCount++;
            mapCounting++;
            if (bossCount == 5)  //맵 5개 클리어하면 다음은 보스방 열림
            {
                goBoss = true;
                bossCount = 0;
                bossLevel++;
            }
        }
        else
        {
            mapCounting++;
            bossCount = 0;
            goBoss = false;
            if(mapCounting == 18)
            {
                finalClear = true;
                SceneManager.LoadScene(s_result);

            }
        }
    }


    public int NextScene(int now)
    {
        next = -1;
        do
        {
            if (goBoss)     //[던전 -> 보스] 로 가야하면 보스맵 얼림
            {
                next = bossLevel + s_boss - 1;       // 3,4,5
            }
            else if (now >= s_nomal)
            {
                int ran = Random.Range(0, 100);
                if (ran < randomNomal)      //70%확률로 일반맵
                {
                    next = Random.Range(s_nomal, s_gimmick);
                }
                else if (ran < randomNomal + randomGimmik)
                {
                    next = Random.Range(s_gimmick, s_bonus);
                }
                else
                {
                    next = Random.Range(s_bonus, SceneManager.sceneCountInBuildSettings);
                }
            }
            else if (now >= s_boss)   //[보스맵 2, 3 -> 던전]  무조건 일반 던전
            {
                next = Random.Range(s_nomal, s_gimmick);
            }
            else if (now == 1)  //[마을 -> 던전] 무조건 일반 던전
            {
                next = 11;
                //next = Random.Range(s_nomal, s_gimmick);

            }
        } while (next == now);
        return next;
    }

    public void SceneInit()     //VillageManager 끝나면 실행함
    {
        next = -1;
        mapClear = false;  
        goBoss = false; 
        finalClear = false;
        
        bossCount = 0;

        bossLevel = 0;
        Timer = 0f;
        mapCounting = 0;
    }
    #endregion
}
