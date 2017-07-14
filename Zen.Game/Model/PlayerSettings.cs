using Zen.Game.Msg.Impl;

namespace Zen.Game.Model
{
    public class PlayerSettings
    {
        public bool AutoRetaliating { get; private set; } = true;
        public bool Running { get; private set; }
        public bool TwoButtonMouse { get; private set; } = true;
        public bool ChatFancy { get; private set; } = true;
        public bool SplitPrivateChat { get; private set; }
        public bool AcceptingAid { get; private set; }
        public int AttackStyle { get; private set; }

        private readonly Player _player;

        public PlayerSettings(Player player)
        {
            _player = player;
        }

        public void RefreshAll()
        {
            RefreshAttackStyle();
            RefreshAutoRetaliating();
            RefreshRunning();
            RefreshTwoButtonMouse();
            RefreshChatFancy();
            RefreshSplitPrivateChat();
            RefreshAcceptingAid();
        }

        public void SetAttackStyle(int style)
        {
            AttackStyle = style;
            RefreshAttackStyle();
        }

        public void ToggleAutoRetaliating()
        {
            AutoRetaliating = !AutoRetaliating;
            RefreshAutoRetaliating();
        }

        public void ToggleRunning()
        {
            Running = !Running;
            RefreshRunning();
        }

        public void ToggleTwoButtonMouse()
        {
            TwoButtonMouse = !TwoButtonMouse;
            RefreshTwoButtonMouse();
        }

        public void ToggleChatFancy()
        {
            ChatFancy = !ChatFancy;
            RefreshChatFancy();
        }

        public void ToggleSplitPrivateChat()
        {
            SplitPrivateChat = !SplitPrivateChat;
            RefreshSplitPrivateChat();
        }

        public void ToggleAcceptingAid()
        {
            AcceptingAid = !AcceptingAid;
            RefreshAcceptingAid();
        }

        private void RefreshAttackStyle() => _player.Send(new ConfigMessage(43, AttackStyle));
        private void RefreshAutoRetaliating() => _player.Send(new ConfigMessage(172, AutoRetaliating ? 0 : 1));
        private void RefreshRunning() => _player.Send(new ConfigMessage(173, Running ? 1 : 0));
        private void RefreshTwoButtonMouse() => _player.Send(new ConfigMessage(170, TwoButtonMouse ? 0 : 1));
        private void RefreshChatFancy() => _player.Send(new ConfigMessage(171, ChatFancy ? 0 : 1));
        private void RefreshSplitPrivateChat() => _player.Send(new ConfigMessage(287, SplitPrivateChat ? 1 : 0));
        private void RefreshAcceptingAid() => _player.Send(new ConfigMessage(472, AcceptingAid ? 1 : 0));
    }
}