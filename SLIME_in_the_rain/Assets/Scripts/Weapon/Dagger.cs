/**
 * @brief 단검 스크립트
 * @author 김미성
 * @date 22-06-26
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dagger : Weapon
{
    #region 변수

    #endregion

    #region 유니티 함수
    private void Awake()
    {
        weaponType = EWeaponType.dagger;
    }
    #endregion

    #region 함수

    /// <summary>
    /// 평타
    /// </summary>
    public override void AutoAttack()
    {
        Debug.Log("AutoAttack");
    }

    /// <summary>
    /// 스킬
    /// </summary>
    public override void Skill()
    {
        Debug.Log("Skill");
    }

    /// <summary>
    /// 대시
    /// </summary>
    public override void Dash()
    {
        Debug.Log("Dash");
    }
    #endregion
}
