using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farm : Built
{
    public int making;              // 식량 생산량

    void Awake()
    {
        _name = "농장";
        _code = (int)BUILT.FARM;
        maxCreateCount = 2;
        _desc = "생성까지 " + (maxCreateCount - createCount) + "턴 남음";

        GameMng.I.AddDelegate(this.waitingCreate);
    }

    void Start()
    {
        _name = string.Format("{0} 종족 농장  (소유자 : {1})", GameMng.I.getUserTribe(_uniqueNumber), GameMng.I.getUserName(_uniqueNumber));
        _emoteSide.color = GetUserColor(_uniqueNumber);

        switch ((int)NetworkMng.getInstance.myTribe)
        {
            case 0:     // 숲 종족
                _max_hp = 7;
                _hp = _max_hp;
                making = 5;
                break;
            case 1:     // 물 종족
                _max_hp = 7;
                _hp = _max_hp;
                making = 5;
                break;
            case 2:     // 사막 종족
                _max_hp = 7;
                _hp = _max_hp;
                making = 5;
                break;
        }
    }

    void init()
    {
        _activity.Add(ACTIVITY.DESTROY_BUILT);
    }
    public void waitingCreate()
    {
        createCount++;
        _desc = "생성까지 " + (maxCreateCount - createCount) + "턴 남음";

        // 2턴 후에 생성됨
        if (createCount > maxCreateCount - 1)
        {
            _desc = "식량을 생산한다";

            _anim.SetTrigger("isSpawn");

            GameMng.I.RemoveDelegate(this.waitingCreate);

            Worker unitobj;
            unitobj = GameMng.I._hextile.GetCell(SaveX, SaveY)._unitObj.GetComponent<Worker>();
            unitobj._bActAccess = true;
            unitobj._anim.SetBool("isWorking", false);
            unitobj.buildingobj = null;

            // 내꺼라면
            if (NetworkMng.getInstance.uniqueNumber.Equals(_uniqueNumber))
            {
                init();

                GameMng.I.AddDelegate(MakingFood);
            }
        }
    }

    /**
     * @brief 식량 생산
     */
    void MakingFood()
    {
        _anim.SetTrigger("isMaking");
        GameMng.I.addFood(making);
    }

    void OnDestroy()
    {
        if (createCount > maxCreateCount - 1 && NetworkMng.getInstance.uniqueNumber.Equals(_uniqueNumber))
            GameMng.I.RemoveDelegate(MakingFood);
        else
            GameMng.I.RemoveDelegate(waitingCreate);
    }
}
