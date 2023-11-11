using Framework;
using QFramework;
using UnityEngine;

public struct AddTestBuffEvent
{
    public Buff buff;
    public BuffConfig config;
}
public struct RemoveTestBuffEvent
{
    public string buffName;
}
public struct UpdateTestBuffEvent
{
    
}
public class BuffID
{
    public const string Poison = "Poison";
    public const string Healing = "Healing";
    public const string CritRateBonus = "±©»÷%";
    public const string Weaken = "Ï÷Èõ%";
}
public class BuffTest : QF_GameController, IBuffOwner
{
    [SerializeField] private float HP = 200;
    private float Attack = 10;
    [SerializeField] private float mCurAttack = 0;

    private void Start()
    {
        mCurAttack = Attack;
    }
    void IBuffOwner.SendRemoveBuffEvent(string buffName)
    {
        this.SendEvent(new RemoveTestBuffEvent() { buffName = buffName });
    }
    float IBuffOwner.GetSourceValue(string buffName)
    {
        switch (buffName)
        {
            case BuffID.Poison: return HP;
            case BuffID.Healing: return HP;
            case BuffID.CritRateBonus: return Attack;
            case BuffID.Weaken: return Attack;
        }
        return 0f;
    }
    void IBuffOwner.SetValue(string buffName, float result)
    {
        switch (buffName)
        {
            case BuffID.Poison: HP = result; break;
            case BuffID.Healing: HP = result; break;
            case BuffID.CritRateBonus: mCurAttack = result; break;
            case BuffID.Weaken: mCurAttack = result; break;
        }
    }
    void IBuffOwner.SendUpdateBuffEvent()
    {
        this.SendEvent(new UpdateTestBuffEvent());
    }
    void IBuffOwner.SendAddBuffEvent(Buff buff, BuffConfig config)
    {
        this.SendEvent(new AddTestBuffEvent() { buff = buff, config = config });
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            this.GetSystem<IBuffSystem>().AddBuff(BuffID.Poison, this);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            this.GetSystem<IBuffSystem>().AddBuff(BuffID.Healing, this);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            this.GetSystem<IBuffSystem>().AddBuff(BuffID.CritRateBonus, this);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            this.GetSystem<IBuffSystem>().AddBuff(BuffID.Weaken, this);
        }
    }
    // Õâ¸öÊý¾Ý´¦ÀíÓÐÊ²Ã´ÓÃÄØ£¿£¿ ¼ÙÉè ÎÒÃÇÔÚbuffµÄ»ù´¡ÉÏ ÐèÒªÍ¨¹ýÒ»Ð©ÌØÊâµÄ¹«Ê½È¥±ä¸übuffµÄÊµ¼ÊÄÜÁ¦
    // ÀýÈçËµ ÎÒ×°±¸ÁËÒ»¼þ¿ÉÒÔÔö¼ÓÁ½±¶±¬»÷¼Ó³ÉµÄ×°±¸ ÎÒÃÇ¿ÉÒÔÔÚ±©»÷µÄ»ù´¡Êý¾ÝÉÏ ÈÃ¼ÓÖµ x 2
    public float DataProcess(string buffName, float source)
    {
        return buffName switch
        {
            BuffID.Poison => source,
            BuffID.Healing => source,
            BuffID.CritRateBonus => source,
            BuffID.Weaken => source,
            _ => source
        };
    }
}