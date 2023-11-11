using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Linq;

namespace QFramework
{
    /// <summary>
    /// 坐标模式
    /// </summary>
    public enum COOR_Mode : byte { Screen, World, Viewport, UILocal }
    public interface IMessageSystem : ISystem
    {
        void Log<T>(T msg);
        void Send<T>(T msg, Vector2 pos, COOR_Mode mode = COOR_Mode.World);
    }
    public class MessageSystem : AbstractSystem, IMessageSystem
    {
        private class BaseMsg
        {
            public enum E_State : byte
            {
                Start, Finished, Wait
            }
            public string msg;
            public float alpha;
            protected Vector2 curPos;
            protected E_State state;
            public Vector2 CurPos => curPos;
            public bool Finished => state == E_State.Finished;
            public virtual void Init(string msg, Vector2 guiPos)
            {
                this.msg = msg;
                curPos = guiPos;
                alpha = 0;
                state = E_State.Wait;
            }
            public void Start()
            {
                state = E_State.Start;
            }
            public virtual void Update()
            {
                switch (state)
                {
                    case E_State.Start:
                        if (alpha <= 0) state = E_State.Finished;
                        else alpha -= Time.deltaTime;
                        break;
                    case E_State.Wait:
                        if (alpha >= 1) alpha = 1;
                        else alpha += 2 * Time.deltaTime;
                        break;
                }
            }
        }
        private class MsgInfo : BaseMsg
        {
            private float dir;
            private float curSpeed;

            public override void Init(string msg, Vector2 guiPos)
            {
                dir = Random.Range(-1f, 1f) * 200F;
                curSpeed = Random.Range(480f, 550f);
                base.Init(msg, guiPos);
            }
            public override void Update()
            {
                base.Update();
                float delta = Time.deltaTime;
                curPos.x -= dir * delta;
                curPos.y -= curSpeed * delta;
                curSpeed -= 500 * delta;
            }
        }
        private class LogInfo : BaseMsg
        {
            public float targetPosY;
            public byte EnterNum;
            public bool CanSplit => EnterNum == 0;
            public override void Init(string msg, Vector2 guiPos)
            {
                EnterNum = 0;
                base.Init(msg, guiPos);
            }
            public override void Update()
            {
                base.Update();
                curPos.y = Mathf.Lerp(curPos.y, targetPosY, 10 * Time.deltaTime);
            }
        }
        private GUIStyle style;
        private Stack<LogInfo> mLogPool;
        private Stack<MsgInfo> mMsgPool;
        private Queue<LogInfo> mLogs;
        private Queue<MsgInfo> mMsgs;
        private StringBuilder mLogMsgBuilder;

        protected override void OnInit()
        {
            PublicMono.Instance.OnGuiCall += OnGUI;

            mLogPool = new Stack<LogInfo>();
            mMsgPool = new Stack<MsgInfo>();

            mLogs = new Queue<LogInfo>();
            mMsgs = new Queue<MsgInfo>();

            mLogMsgBuilder = new StringBuilder();
            style = new GUIStyle() { fontSize = 30 };
        }
        private void OnGUI()
        {
            // 显示所有消息
            if (mLogs.Count >= 1 && ShowMsg(mLogs))
            {
                mLogPool.Push(mLogs.Dequeue());
            }
            if (mMsgs.Count >= 1 && ShowMsg(mMsgs))
            {
                mMsgPool.Push(mMsgs.Dequeue());
            }
        }

        private bool ShowMsg<T>(Queue<T> infos) where T : BaseMsg
        {
            bool canRemove = false;
            var rect = new Rect();
            var color = Color.white;
            foreach (var log in infos)
            {
                rect.position = log.CurPos;
                color.a = log.alpha;
                log.Update();
                style.normal.textColor = color;
                if (log.Finished) canRemove = true;
                GUI.Label(rect, log.msg, style);
            }
            return canRemove;
        }

        void IMessageSystem.Log<T>(T msg)
        {
            var log = mLogPool.Count == 0 ? new LogInfo() : mLogPool.Pop();
            string str = msg.ToString();
            int clamp = 15;
            float step = 50;
            float y = Screen.height - str.Length / clamp * step;
            var start = new Vector2(step, y);
            log.Init(str, start);
            mLogs.Enqueue(log);

            this.GetSystem<IDelayTimeSystem>().AddDelayTask(1f, log.Start);

            foreach (var item in mLogs.Reverse())
            {
                if (item.msg.Length <= clamp)
                {
                    start.y -= step;
                }
                else
                {
                    if (item.CanSplit)
                    {
                        mLogMsgBuilder.Clear();
                        int index = 0;
                        // 计算回车的次数
                        while (true)
                        {
                            if ((item.msg.Length - index) >= clamp)
                            {
                                mLogMsgBuilder.Append(item.msg.Substring(index, clamp) + "\r\n");
                            }
                            else
                            {
                                mLogMsgBuilder.Append(item.msg.Substring(index, item.msg.Length - index));
                                break;
                            }
                            index += clamp;
                            item.EnterNum++;
                        }
                        item.msg = mLogMsgBuilder.ToString();
                    }
                    start.y -= step + item.EnterNum * style.fontSize;
                }
                item.targetPosY = start.y;
            }
        }
        void IMessageSystem.Send<T>(T msg, Vector2 pos, COOR_Mode mode)
        {
            switch (mode)
            {
                case COOR_Mode.World: pos = Camera.main.WorldToScreenPoint(pos); break;
                case COOR_Mode.Viewport: pos = Camera.main.ViewportToScreenPoint(pos); break;
            }
            if (mode != COOR_Mode.UILocal)
            {
                pos.y = Screen.height - pos.y;
            }
            var log = mMsgPool.Count == 0 ? new MsgInfo() : mMsgPool.Pop();
            log.Init(msg.ToString(), pos);
            mMsgs.Enqueue(log);
            this.GetSystem<IDelayTimeSystem>().AddDelayTask(0.5f, log.Start);
        }
    }
}