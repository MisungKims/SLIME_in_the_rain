using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceStaff : Staff
{
    #region 유니티 함수
    private void Awake()
    {
        weaponType = EWeaponType.iceStaff;
        dashCoolTime = 5f;
    }
    #endregion
}
