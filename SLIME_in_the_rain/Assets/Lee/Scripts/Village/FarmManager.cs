using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmManager : MonoBehaviour
{
    [Header("Tower Prefabs")]
    public List<GameObject> EmptyList;
    public List<GameObject> HPList;
    public List<GameObject> CoolTimeList;
    public List<GameObject> MoveSpeedList;
    public List<GameObject> AttackSpeedList;
    public List<GameObject> AttackPowerList;
    public List<GameObject> AttackRangeList;
    public List<GameObject> DefensePowerList;
    public List<GameObject> InventorySlotList;

    string level;

    private void Start()
    {
        level = PlayerPrefs.GetString(this.name + "level");
        LevelDefault();
        TowerBuilding(int.Parse(level));
    }
    void LevelDefault()
    {
        if (this.name == "Empty")
        {
            PlayerPrefs.SetString(this.name + "level", Random.Range(5, 15).ToString());
            PlayerPrefs.GetString(this.name + "level");
        }
        else
        {
            if (level == "")
            {
                PlayerPrefs.SetString(this.name + "level", "0");
                PlayerPrefs.GetString(this.name + "level");
            }
        }
    }
    public void TowerBuilding(int makeNum)
    {

        List<GameObject> list = GetTowerList(this.name);

        //자식으로 오브젝트 생성
        for (int i = 0; i < makeNum; i++)
        {
            GameObject building;
            building = Instantiate(list[Random.Range(0, list.Count)]);
            building.transform.parent = this.transform;

            //Position
            Vector3 setPos;
            setPos.x = Random.Range(-1.9f, 1.9f);
            setPos.y = 0;
            setPos.z = -2 + Random.Range(-1.9f, 1.9f);
            building.transform.localPosition = setPos;
            building.SetActive(true);

            //Rotation
            Quaternion setRot = new Quaternion();
            setRot.y = Random.Range(0, 360);
            building.transform.rotation = setRot;

            //Scale
            int ran = Random.Range(0, 100);
            float minmax = 2.5f;
            if (ran > 99)
            {
                building.transform.localScale *= 10f;
            }
            else if (ran > 95)
            {
                building.transform.localScale *= Random.Range(minmax, 3f);
            }
            else
            {
                building.transform.localScale *= Random.Range(1f, minmax);
            }
        }
    }
    public List<GameObject> GetTowerList(string name)
    {
        switch (name)
        {
            case "MaxHP":
                return HPList;
            case "CoolTime":
                return CoolTimeList;
            case "MoveSpeed":
                return MoveSpeedList;
            case "AttackSpeed":
                return AttackSpeedList;
            case "AttackPower":
                return AttackPowerList;
            case "AttackRange":
                return AttackRangeList;
            case "DefensePower":
                return DefensePowerList;
            case "InventorySlot":
                return InventorySlotList;
            case "Empty":
                return EmptyList;
            default:
                return null;
        }
    }
}
