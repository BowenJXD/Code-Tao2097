using Framework;
using UnityEngine.UI;

namespace QFramework
{
    public class BuffIcon : QF_GameController
    {
        private Text LevelText;
        private Text mTimerNum;
        private Image mMatteImg;

        private Buff mBuff;
        private float mMaxDuration;

        private void OnEnable()
        {
            this.RegisterEvent<UpdateTestBuffEvent>(e =>
            {
                UpdateLevelText();
            })
            .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        public void Init(Buff buff, BuffConfig config)
        {
            mMatteImg = transform.Find("Matte").GetComponent<Image>();
            mTimerNum = transform.Find("Timer").GetComponent<Text>();
            LevelText = transform.Find("Level").GetComponent<Text>();

            mBuff = buff;
            mMaxDuration = config.Duration;
            // mMatteImg.sprite = config.Sprite;
            UpdateLevelText();
        }
        private void UpdateLevelText()
        {
            LevelText.text = mBuff.Level.ToString();
        }
        private void Update()
        {
            if (mBuff.IsContinue())
            {
                mMatteImg.fillAmount = RangeTo01();
                mTimerNum.text = mBuff.duration.ToString("f2");
            }
        }
        private float RangeTo01()
        {
            // ����ӳ��ֵ�Ĳ���� ���� (��ǰֵ - ��Сֵ)
            // (1-0)/ (0 - mMaxDuration) * (mBuff.duration - mMaxDuration)
            return 1 / -mMaxDuration * (mBuff.duration - mMaxDuration); // ������Сֵ��Ϊ 0 �������
        }
    }
}