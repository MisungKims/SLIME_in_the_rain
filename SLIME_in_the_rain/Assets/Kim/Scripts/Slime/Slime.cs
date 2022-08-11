/**
 * @brief 슬라임 오브젝트
 * @author 김미성
 * @date 22-07-24
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour
{
    #region 변수
    #region 싱글톤
    private static Slime instance = null;
    public static Slime Instance
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

    private Rigidbody rigid;

    private Animator anim;

    [SerializeField]
    private SkinnedMeshRenderer skinnedMesh;            // 슬라임의 Material
    public SkinnedMeshRenderer SkinnedMesh { get { return skinnedMesh; } }

    private Stats stat;
    public Stats Stat { get { return stat; } }

    //////// 무기
    [Header("------------ 무기")]
    public Transform weaponPos;     // 무기 장착 시 무기의 parent

    public Weapon currentWeapon;    // 장착 중인 무기

    [SerializeField]
    private LayerMask weaponLayer;

    private float detectRadius = 1.1f;      // 무기를 감지할 범위

    Collider[] colliders;
    Outline outline;

    //////// 대시
    [Header("------------ 대시")]
    public float originDashDistance = 6.5f;
    private float dashDistance = 6.5f;
    public float DashDistance { get { return dashDistance; } set { dashDistance = value; } }
    public float dashTime = 0.6f;        // 대시 지속 시간
    public bool isDash { get; set; }                // 대시 중인지?
    bool isCanDash;     // 대시 가능한지?

    public GameObject shield;

    //////// 공격
    public Transform target;

    public bool isAttacking;   // 평타 중인지?

    public bool isStealth;      // 은신 중인지?

    //////// 데미지
    private bool isStun;


    //////// 이동
    enum AnimState { idle, move, dash, damaged, die }     // 애니메이션의 상태
    AnimState animState = AnimState.idle;

    private Vector3 direction;                  // 이동 방향

    public bool canMove = true;

    private bool isInWater = false;
    public bool IsInWater { get { return isInWater; } }

    private float decreaseHPAmount = 0.5f;  // 물 안에서 감소될 체력의 양

    //////// 캐싱
    private WaitForSeconds waitForAttack = new WaitForSeconds(0.2f);       // 공격을 기다리는
    private WaitForSeconds waitFor2s = new WaitForSeconds(2f);
    private WaitForSeconds waitForDash;

    public StatManager statManager;
    private ICanvas _Canvas;

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

        waitForDash = new WaitForSeconds(dashTime);
        rigid = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        shield.SetActive(false);

        isCanDash = true;
    }

    private void Start()
    {
        stat = statManager.myStats;
        StartCoroutine(AutoAttack());
        StartCoroutine(Skill());
        StartCoroutine(DecreaseHPInWater());
        StartCoroutine(SpaceBar());
    }

    private void Update()
    {
        DetectWeapon();
    }

    void FixedUpdate()
    {
        Move();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 4) isInWater = true;       // water 레이어일 때
        else isInWater = false;
    }
    #endregion

    #region 코루틴
    // 무기를 들고 있을 때 좌클릭하면 평타
    IEnumerator AutoAttack()
    {
        while (true)
        {
            if (canMove && !isAttacking && currentWeapon && !isStun && Input.GetMouseButtonDown(0))
            {
                isAttacking = true;

                currentWeapon.SendMessage("AutoAttack", SendMessageOptions.DontRequireReceiver);

                yield return new WaitForSeconds(stat.attackSpeed);           // 각 무기의 공속 스탯에 따라 대기

                isAttacking = false;
            }

            yield return null;
        }
    }

    // 무기를 들고 있을 때 우클릭하면 스킬
    IEnumerator Skill()
    {
        while (true)
        {
            if (IsCanSkill())
            {
                isAttacking = true;

                currentWeapon.SendMessage("Skill", SendMessageOptions.DontRequireReceiver);

                yield return waitForAttack;         // 0.2초 대기

                isAttacking = false;
            }

            yield return null;
        }
    }

    // 스페이스바 누르면 앞으로 대시
    IEnumerator SpaceBar()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Space) && !isDash && canMove)
            {
                isDash = true;

                if (currentWeapon)
                {
                    currentWeapon.SendMessage("Dash", this, SendMessageOptions.DontRequireReceiver);
                }
                else
                {
                    Dash();
                }
            }
            yield return null;
        }
        
    }

    // 대시 코루틴
    IEnumerator DoDash()
    {
        isCanDash = false;

        PlayAnim(AnimState.dash);       // 대시 애니메이션 실행

        rigid.velocity = transform.forward * dashDistance;

        yield return waitForDash;

        //rigid.velocity = transform.forward;

        PlayAnim(AnimState.idle);       // 대시 애니메이션 실행

        dashDistance = originDashDistance;

        isDash = false;
        isCanDash = true;
    }

    // 스턴 코루틴
    IEnumerator DoStun(float stunTime)
    {
        isStun = true;
        PlayAnim(AnimState.damaged);

        yield return new WaitForSeconds(stunTime);

        isStun = false;
    }

    // 물 위에 있으면 체력 감소
    private IEnumerator DecreaseHPInWater()
    {
        while (true)
        {
            if (isInWater)
            {
                stat.HP -= decreaseHPAmount;

                yield return waitFor2s;
            }

            yield return null;
        }
    }

    #endregion

    #region 함수
    // 슬라임과 오브젝트 사이의 거리를 구함
    float GetDistance(Transform target)
    {
        Vector3 offset = transform.position - target.position;

        return offset.sqrMagnitude;
    }

    // 애니메이션 플레이
    void PlayAnim(AnimState state)
    {
        animState = state;

        anim.SetInteger("animation", (int)animState);
    }

    #region 움직임
    // 슬라임의 움직임
    void Move()
    {
        if (!canMove || isAttacking || isDash || isStun) return;

        float dirX = Input.GetAxis("Horizontal");
        float dirZ = Input.GetAxis("Vertical");

        if (dirX != 0 || dirZ != 0)
        {
            animState = AnimState.move;

            direction = new Vector3(dirX, 0, dirZ);

            if (direction != Vector3.zero)
            {
                float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

                transform.rotation = Quaternion.Euler(0, angle, 0);         // 회전
            }

            transform.position += direction * stat.moveSpeed * Time.deltaTime;   // 이동
        }
        else
        {
            animState = AnimState.idle;
        }

        PlayAnim(animState);
    }


    // 대시
    public void Dash()
    {
        // 대시를 할 수 없을 때 return
        if (!isCanDash || isStun)
        {
            isDash = false;
            return;
        }

        StartCoroutine(DoDash());
    }

    
    #endregion

    #region 공격
    // 스킬을 사용할 수 있는지?
    bool IsCanSkill()
    {
        if (canMove && !isAttacking && currentWeapon && currentWeapon.isCanSkill && !isStun && Input.GetMouseButtonDown(1))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
   
    #endregion
    Collider lastCollider;
    #region 무기
    // 주변에 있는 무기 감지
    void DetectWeapon()
    {
        colliders = Physics.OverlapSphere(transform.position, detectRadius, weaponLayer);

        if (colliders.Length == 1)      // 감지한 무기가 한 개일 때
        {
            lastCollider = colliders[0];
            EquipWeapon(0);
        }
        else if (colliders.Length > 1)
        {
            // 감지한 무기들 중 제일 가까운 거리에 있는 무기를 장착

            int minIndex = 0;
            float minDis = GetDistance(colliders[0].transform);

            for (int i = 1; i < colliders.Length; i++)          // 가까운 거리에 있는 무기 찾기
            {
                float distance = GetDistance(colliders[i].transform);

                if (minDis > distance)
                {
                    minDis = distance;
                    minIndex = i;
                }
            }

            // Outline을 꺼야하는 오브젝트는 끔
            if(lastCollider && !lastCollider.Equals(colliders[minIndex]))
            {
                outline = lastCollider.GetComponent<Outline>();
                outline.enabled = false;
                lastCollider = colliders[minIndex];
            }

            EquipWeapon(minIndex);
        }
        else
        {
            if(lastCollider)            // 아무것도 감지하지 않을 때 오브젝트의 아웃라인 끄기
            {
                outline = lastCollider.GetComponent<Outline>();
                outline.enabled = false;
                lastCollider = null;
            }
        }
    }

    // 감지한 무기 G 키를 눌러 장착
    void EquipWeapon(int index)
    {
        if (lastCollider)
        {
            outline = lastCollider.GetComponent<Outline>();
            outline.enabled = true;
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            RemoveCurrentWeapon();

            outline.enabled = false;
            colliders[index].SendMessage("DoAttach", SendMessageOptions.DontRequireReceiver);
        }
    }

    // 인벤토리에서 클릭 시 무기 장착
    public void EquipWeapon(Weapon weapon)
    {
        RemoveCurrentWeapon();

        weapon.ChangeWeapon();
    }

    // 현재 무기를 없앰
    void RemoveCurrentWeapon()
    {
        if (currentWeapon)
        {
            currentWeapon.gameObject.layer = 6;
            ObjectPoolingManager.Instance.Set(currentWeapon);
            currentWeapon = null;
        }
    }

    // 무기 변경
    public void ChangeWeapon(Weapon weapon)
    {
        currentWeapon = weapon;

        // 무기의 위치 설정
        currentWeapon.transform.parent = weaponPos;
        currentWeapon.transform.localPosition = Vector3.zero;

        // 변경한 무기의 스탯으로 변경
        statManager.ChangeStats(currentWeapon);

        //변경된 스탯 적용
        if (ICanvas.Instance) ICanvas.Instance.changeWeapon();

        // 슬라임의 색 변경
        ChangeMaterial();              
    }

    // 슬라임의 색(머터리얼) 변경
    void ChangeMaterial()
    {
        if (currentWeapon)
        {
            skinnedMesh.material = currentWeapon.slimeMat;
        }
    }

    #endregion

    //// 데미지를 입음
    //public void Damaged(float amount)
    //{
    //    // 대미지 = 몬스터 공격력 * (1 - 방어율)
    //    // 방어율 = 방어력 / (1 + 방어력)

    //    float damageReduction = stat.defensePower / (1 + stat.defensePower);
    //    stat.HP -= amount * (1 - damageReduction);

    //    PlayAnim(AnimState.damaged);
    //}


    // 데미지를 입음
    public void Damaged(Stats monsterStats, int atkType)
    {
        TakeDamage();
    }

    public void Damaged(float damageAmount)
    {
        TakeDamage();
    }

    private void TakeDamage()
    {
        PlayAnim(AnimState.damaged);
    }

    // 스턴
    public void Stun(float stunTime)
    {
        StartCoroutine(DoStun(stunTime));

        Debug.Log("Stun");
    }
#endregion
}