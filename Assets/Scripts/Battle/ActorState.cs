using System.Linq;
using UnityEngine;

public class ActorState
{
    public int Id { get; }
    public int CurrentMapId { get; private set; }
    public int PrevMapId { get; private set; }
    public bool IsPlayer { get; }
    public ActorThinkStatus ActorThinkStatus { get; private set; }

    public int Level { get; private set; }
    public int Exp { get; private set; }
    public bool[] Skills { get; private set; }
    public float HP { get; private set; }

    public bool IsDead => HP <= 0;

    public int AttackInterval => (150 - Skills.Take(3).Count(x => x) * 30) / 2;
    public float MoveSpeed => (0.03f + Skills.Take(3).Count(x => x) * 0.02f) * 5;
    public int HPMax => 50 + Skills.Skip(3).Take(3).Count(x => x) * 50;
    public float HPRegeneration => (0.003f + Skills.Skip(3).Take(3).Count(x => x) * 0.002f) * 5;
    public int AttackDamage => 10 + Skills.Skip(6).Take(3).Count(x => x) * 5;
    public float BulletSpeed => (0.15f + Skills.Skip(6).Take(3).Count(x => x) * 0.1f) * 5;

    public bool IsMoveAttack => Skills.Take(3).All(x => x);
    public bool IsDodge => Skills.Skip(3).Take(3).All(x => x);
    public bool IsDeviationShoot => Skills.Skip(6).Take(3).All(x => x);

    // Skills 9
    // x x x 攻撃速度と移動速度(移動攻撃)
    // y y y 最大HPとHP自動回復(dodge)
    // z z z 攻撃力と弾速(偏差射撃)

    public ActorState(int id, int mapId, bool isPlayer)
    {
        Id = id;
        CurrentMapId = mapId;
        PrevMapId = mapId;
        IsPlayer = isPlayer;

        Skills = new bool[9];
        HP = 100;

        ActorThinkStatus = ActorThinkStatus.Idle;
    }

    public void TransitionMap(int mapId)
    {
        PrevMapId = CurrentMapId;
        CurrentMapId = mapId;
    }

    public void LearnSkill(int skillIndex)
    {
        Skills[skillIndex] = true;
    }

    public void Damage(int damage)
    {
        HP -= damage;
    }

    public void Regeneration()
    {
        if (IsDead)
        {
            return;
        }

        HP = Mathf.Min(HP + HPRegeneration, HPMax);
    }

    public void OnKill(ActorState actorState)
    {
        GetExp(100);
        UnityEngine.Debug.Log($"Player:{Id} -> Player:{actorState.Id} Killed");
    }

    public void GetExp(int addExp)
    {
        if (Skills.All(x => x))
        {
            return;
        }

        Exp += addExp;

        if (Exp >= 100)
        {
            Level += Exp / 100;
            Exp %= 100;
        }
    }
}