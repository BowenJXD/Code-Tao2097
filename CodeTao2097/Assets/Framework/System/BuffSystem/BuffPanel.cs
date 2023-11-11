using QFramework;
using System.Collections.Generic;
using Framework;
using UnityEngine;
using UnityEngine.UI;

public class BuffPanel : QF_GameController
{
    private Dictionary<string, GameObject> mBuffs;

    private void Start()
    {
        mBuffs = new Dictionary<string, GameObject>();

        this.RegisterEvent<AddTestBuffEvent>(e =>
        {
            var icon = ResHelper.SyncLoad<GameObject>("Prefabs/BuffIcon");
            icon.GetComponent<Image>().sprite = e.config.Sprite;
            icon.GetComponent<BuffIcon>().Init(e.buff, e.config);
            icon.transform.SetParent(transform);
            mBuffs.Add(e.buff.name, icon);
        })
        .UnRegisterWhenGameObjectDestroyed(gameObject);

        this.RegisterEvent<RemoveTestBuffEvent>(e =>
        {
            if (mBuffs.TryGetValue(e.buffName, out var buffGo))
            {
                GameObject.Destroy(buffGo);
                mBuffs.Remove(e.buffName);
            }
        })
        .UnRegisterWhenGameObjectDestroyed(gameObject);
    }
}