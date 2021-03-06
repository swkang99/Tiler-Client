using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turret : Built
{
    public static int attack;              // 공격력

    public Tile tilestate;          // 터렛이 올라가 있는 타일 정보

    public static int maintenanceCost = 0;   // 유지 비용

    public int _TurnCount = 0;
    void Awake()
    {
        _name = "터렛";
        _code = (int)BUILT.ATTACK_BUILDING;
        _attackdistance = 2;
        maxCreateCount = 3;
        attack = 5;
        _desc = "생성까지 " + (maxCreateCount - createCount) + "턴 남음";

        GameMng.I.AddDelegate(this.waitingCreate);
    }

    void Start()
    {
        _name = string.Format("{0} 종족 터렛  (소유자 : {1})", GameMng.I.getUserTribe(_uniqueNumber), GameMng.I.getUserName(_uniqueNumber));
        _emoteSide.color = GetUserColor(_uniqueNumber);

        switch ((int)NetworkMng.getInstance.myTribe)
        {
            case 0:     // 숲 종족
                _max_hp = 7;
                _hp = _max_hp;
                attack = 5;
                maintenanceCost = 2;
                break;
            case 1:     // 물 종족
                _max_hp = 7;
                _hp = _max_hp;
                attack = 5;
                maintenanceCost = 3;
                break;
            case 2:     // 사막 종족
                _max_hp = 7;
                _hp = _max_hp;
                attack = 5;
                maintenanceCost = 4;
                break;
        }
    }

    void init()
    {
        _activity.Add(ACTIVITY.DESTROY_BUILT);
        GameMng.I.AddDelegate(this.Attack);
    }

    public void waitingCreate()
    {
        createCount++;
        _desc = "생성까지 " + (maxCreateCount - createCount) + "턴 남음";


        // 2턴 후에 생성됨
        if (createCount > maxCreateCount - 1)
        {
            _desc = "턴이 끝날 때 사정거리 안의 적을 공격한다";

            _anim.SetTrigger("isSpawn");

            GameMng.I.RemoveDelegate(this.waitingCreate);

            Worker unitobj;
            unitobj = GameMng.I._hextile.GetCell(SaveX, SaveY)._unitObj.GetComponent<Worker>();
            unitobj._bActAccess = true;
            unitobj._anim.SetBool("isWorking", false);
            unitobj.buildingobj = null;

            if (NetworkMng.getInstance.uniqueNumber.Equals(_uniqueNumber))
            {
                init();
                _TurnCount = 0;
                GameMng.I.AddDelegate(maintenance);
            }
        }
    }

    public void maintenance()
    {
        GameMng.I.minGold(maintenanceCost);
    }


    /**
     * @brief 사거리 내에 있는 적을 공격
     */
    public void Attack()
    {
        maintenance();

        _TurnCount++;

        tilestate = gameObject.transform.parent.GetComponent<Tile>();
        GameMng.I._hextile.FindDistancesTo(tilestate);

        if ((_TurnCount % 3).Equals(0))
        {
            NetworkMng.getInstance.SendMsg(string.Format("ATTACK_TURRET:{0}:{1}:{2}:{3}:{4}",
            tilestate.PosX,
            tilestate.PosZ,
            attack,
            this._uniqueNumber,
            _TurnCount));
        }
    }

    void OnDestroy()
    {
        if (createCount < maxCreateCount - 1)
            GameMng.I.RemoveDelegate(waitingCreate);
        else
            GameMng.I.RemoveDelegate(Attack);
    }
}
