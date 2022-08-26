/**
 * @brief ���� ������Ʈ
 * @author ��̼�
 * @date 22-07-02
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jelly : PickUp
{
    #region ����
    public JellyGrade jellyGrade;
    private MeshRenderer meshRenderer;

    // ĳ��
    private JellyManager jellyManager;
    private ObjectPoolingManager objectPoolingManager;
    #endregion

    #region ����Ƽ �Լ�
    protected override void Awake()
    {
        base.Awake();

        meshRenderer = GetComponent<MeshRenderer>();
        jellyManager = JellyManager.Instance;
        objectPoolingManager = ObjectPoolingManager.Instance;
    }

    protected override void OnEnable()
    {
        InitJelly();

        base.OnEnable();
    }

    #endregion

    #region �Լ�
    // ������ ����� ����
    void InitJelly()
    {
        jellyGrade = jellyManager.GetRandomJelly();
        meshRenderer.material = jellyGrade.mat;
    }

    // ���� ȹ��
    public override void Get()
    {
        jellyManager.GetJelly(this);

        objectPoolingManager.Set(this.gameObject, EObjectFlag.jelly);       // ������Ʈ Ǯ�� ��ȯ
    }
    #endregion
}
