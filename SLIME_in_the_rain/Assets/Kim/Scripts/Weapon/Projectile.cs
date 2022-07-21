/**
 * @brief �߻�ü ������Ʈ
 * @author ��̼�
 * @date 22-06-27
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EProjectileFlag     // ����ü Flag
{
    arrow,
    ice,
    fire,
    iceSkill,
    fireSkill,
    dagger,
    sword,
    earthworm,
    spider
}

public class Projectile : MonoBehaviour
{
    #region ����
    [SerializeField]
    protected float speed;

    private float damageAmount;
    public float DamageAmount { set { damageAmount = value; } }

    [SerializeField]
    protected EProjectileFlag flag;

    public bool isSkill;

    public Vector3 dir;

    // ĳ��
   // WaitForSeconds waitFor1s = new WaitForSeconds(1f);
    WaitForSeconds waitFor2s = new WaitForSeconds(2f);
    #endregion

    #region ����Ƽ �Լ�
    protected virtual void OnEnable()
    {
        StartCoroutine(Remove());
    }

    private void Update()
    {
        Move();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DamagedObject"))
        {
            DoDamage(other, isSkill);
        }
    }
    #endregion

    #region �ڷ�ƾ
    // 2�� �Ŀ� ������
    IEnumerator Remove()
    {
        yield return waitFor2s;

        ObjectPoolingManager.Instance.Set(this.gameObject, flag);
    }
    #endregion

    #region �Լ�
    protected virtual void Move()
    {
        //transform.Translate(dir * Time.deltaTime * speed);
        transform.Translate(Vector3.forward * Time.deltaTime * speed);
       // Debug.Log("pro : " + transform.forward);
    }

    // �������� ����
    protected virtual void DoDamage(Collider other, bool isSkill)
    {
        HideProjectile(other);
       
        IDamage damagedObject = other.transform.GetComponent<IDamage>();
        if (damagedObject != null)
        {
           if(isSkill) damagedObject.SkillDamaged();
           else damagedObject.AutoAtkDamaged();

            // ���� ��
            if (other.gameObject.layer == 8)
            {
                RuneManager.Instance.UseAttackRune(other.gameObject);
            }
        }
    }

    // ����ü�� �����
    protected void HideProjectile(Collider other)
    {
        if (other.transform.gameObject.layer == 8)
        {
            if (!other.GetComponent<Monster>().isDie)
            {
                ObjectPoolingManager.Instance.Set(this.gameObject, flag);
            }
        }
        else ObjectPoolingManager.Instance.Set(this.gameObject, flag);
    }
    #endregion
}