/**
 * @brief ���� ��ų ����ü ������Ʈ
 * @author ��̼�
 * @date 22-06-28
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceProjectile : Projectile
{
    #region �Լ�
    // �������� ����
    protected override void DoDamage(Collider other)
    {
        ObjectPoolingManager.Instance.Set(this.gameObject, EObjectFlag.arrow);

        IDamage damagedObject = other.transform.GetComponent<IDamage>();
        if (damagedObject != null)
        {
            damagedObject.Damaged();
            damagedObject.Stun();       // ����
        }
    }
    #endregion
}