using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using System.ComponentModel.Composition;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Drawing;
using Catel.IoC;
using Catel.Services;

namespace Metaseed.MetaShell.Services
{
    using Metaseed.Windows.Controls;
    //[Export(typeof(IMessager))]
    public class TaskBarBalloon : IBalloon
    {
        //[Import]
        readonly TaskbarIcon _taskbarIcon;
        public TaskBarBalloon(TaskbarIcon taskbarIcon)
        {
            _taskbarIcon = taskbarIcon;
        }
        public void ShowBalloonTip(string title, string message, BalloonIcon symbol)
        {
            _taskbarIcon.ShowBalloonTip(title, message, symbol);
        }

        public void ShowBalloonTip(string title, string message, Icon customIcon)
        {
            _taskbarIcon.ShowBalloonTip(title, message, customIcon);
        }

        public void ShowCustomBalloon(PopupAnimation animation, int? timeout, UIElement balloon)
        {
            _taskbarIcon.ShowCustomBalloon(balloon, animation, timeout);
        }
        public void ShowCustomBalloon(string title, string message, BalloonIcon symbol=BalloonIcon.Info,PopupAnimation animation=PopupAnimation.Fade, int? timeout=3000 )
        {
            var baloon = new Balloon() { Title = title, Description = message, Icon = symbol };
            _taskbarIcon.ShowCustomBalloon(baloon, animation, timeout);
        }
        public void HideBalloonTip()
        {
            _taskbarIcon.HideBalloonTip();
        }
    }
    public class Messager : IMessager
    {

        IBalloon _balloon;
        public IBalloon Balloon
        {
            get { return _balloon ?? (_balloon = ServiceLocator.Default.ResolveType<IBalloon>()); }
        }
        IPleaseWaitService _waitMessage;
        public IPleaseWaitService WaitMessage
        {
            get
            {
                if (_waitMessage == null)
                {
                    _waitMessage = ServiceLocator.Default.ResolveType<IPleaseWaitService>();
                }
                return _waitMessage;
            }
        }
        ISplashScreenService _splashScreen;
        public ISplashScreenService SplashScreen
        {
            get {
                return _splashScreen ?? (_splashScreen = ServiceLocator.Default.ResolveType<ISplashScreenService>());
            }
        }
        IMessageService _messageBox;
        public IMessageService MessageBox
        {
            get { return _messageBox ?? (_messageBox = ServiceLocator.Default.ResolveType<IMessageService>()); }
        }
    }
}
