using Framework;
using QFramework;
using UnityEngine;
using UnityEngine.UI;

public class MainPanel : QF_UIController
{
    private Text hpTex;
    private void Start()
    {
        hpTex = transform.Find("HpText").GetComponent<Text>();

        this.GetModel<IPlayerModel>().HP.RegisterWithInitValue(hp =>
        {
            hpTex.text = $"HP:{hp}";
        })
        .UnRegisterWhenGameObjectDestroyed(gameObject);
    }
}