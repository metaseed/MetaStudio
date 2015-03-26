using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows;
using System.Windows.Controls.Primitives;
using Catel.Services;

namespace Metaseed.MetaShell.Services
{
    using Metaseed.Windows.Controls;
    public interface IBalloon
    {
        void ShowBalloonTip(string title, string message, BalloonIcon symbol);
        void ShowBalloonTip(string title, string message, Icon customIcon);
        void ShowCustomBalloon(PopupAnimation animation, int? timeout, UIElement balloon);
        void ShowCustomBalloon(string title, string message, BalloonIcon symbol = BalloonIcon.Info, PopupAnimation animation = PopupAnimation.Fade, int? timeout = 3000);
        void HideBalloonTip();
    }
    public interface IMessager
    {
        IBalloon Balloon { get; }
        IPleaseWaitService WaitMessage { get; }
        ISplashScreenService SplashScreen { get; }
        IMessageService MessageBox { get; }
    }
}
