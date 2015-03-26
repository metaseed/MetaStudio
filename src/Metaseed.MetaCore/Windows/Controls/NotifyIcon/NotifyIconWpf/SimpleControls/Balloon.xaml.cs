using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Metaseed.Windows.Controls;
using System.ComponentModel;
using Metaseed;

using Catel;
namespace Metaseed.Windows.Controls
{
    /// <summary>
    /// Usage:
    ///  using System.Windows.Controls.Primitives;
    ///       Balloon balloon = new Balloon(){Text="Custom Balloon",BalloonDescription="abc"};
    ///      //show balloon and close it after 4 seconds
    ///       GloableStaitcInstanse.TaskbarIcon_MainWindow.ShowCustomBalloon(balloon, PopupAnimation.Slide, 4000);
    /// </summary>
    public partial class Balloon : UserControl, INotifyPropertyChanged
    {
        private bool isClosing = false;

        BalloonIcon _Icon = BalloonIcon.None;
        public BalloonIcon Icon
        {
            get { return _Icon; }
            set
            {
                if (value != _Icon)
                {
                    _Icon = value;
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    var myLinearGradientBrush = new LinearGradientBrush();
                    myLinearGradientBrush.StartPoint = new Point(0.5, 0);
                    myLinearGradientBrush.EndPoint = new Point(0.5, 1);
                    switch (value)
                    {
                        case BalloonIcon.None:
                            bitmapImage.UriSource = null;

                            myLinearGradientBrush.GradientStops.Add(new GradientStop(Colors.Green, 0.0));
                            myLinearGradientBrush.GradientStops.Add(new GradientStop(Colors.LightGreen, 1));
                            break;
                        case BalloonIcon.Info:
                            bitmapImage.UriSource = new Uri("pack://application:,,,/Metaseed.MetaCore;component/Windows/Controls/NotifyIcon/NotifyIconWpf/Images/Info.png");
                            myLinearGradientBrush.GradientStops.Add(new GradientStop(Colors.MediumSlateBlue, 0.0));
                            myLinearGradientBrush.GradientStops.Add(new GradientStop(Colors.SkyBlue, 1));
                            break;
                        case BalloonIcon.Warning:
                            bitmapImage.UriSource = new Uri("pack://application:,,,/Metaseed.MetaCore;component/Windows/Controls/NotifyIcon/NotifyIconWpf/Images/warning.png");
                            myLinearGradientBrush.GradientStops.Add(new GradientStop(Colors.YellowGreen, 0.0));
                            myLinearGradientBrush.GradientStops.Add(new GradientStop(Colors.Gold, 1));
                            break;
                        case BalloonIcon.Error:
                          
                            bitmapImage.UriSource = new Uri("pack://application:,,,/Metaseed.MetaCore;component/Windows/Controls/NotifyIcon/NotifyIconWpf/Images/error.png");
                            myLinearGradientBrush.GradientStops.Add(new GradientStop(Colors.MediumVioletRed, 0.0));
                            myLinearGradientBrush.GradientStops.Add(new GradientStop(new Color() {R=0xF3,G=0x42,B=0x26,A=0xFF}, 1));
                            break;
                        default:
                            break;
                    }
                    bitmapImage.EndInit();
                    IconSource = bitmapImage;
                    grid.Background = myLinearGradientBrush;
                    
                }

            }
        }




        ImageSource _IconSource;
        public ImageSource IconSource
        {
            get { return _IconSource; }
            set
            {
                if (_IconSource != value)
                {
                    _IconSource = value;
                }
                PropertyChanged.SafeInvoke(this, new PropertyChangedEventArgs("IconSource"));
            }
        }
        #region BalloonText dependency property

        /// <summary>
        /// Description
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title",
                                        typeof(string),
                                        typeof(Balloon),
                                        new FrameworkPropertyMetadata(""));

        /// <summary>
        /// A property wrapper for the <see cref="BalloonTextProperty"/>
        /// dependency property:<br/>
        /// Description
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        #endregion
        #region BalloonDescription dependency property
        /// <summary>
        /// Description
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description",
                                        typeof(string),
                                        typeof(Balloon),
                                        new FrameworkPropertyMetadata(""));

        /// <summary>
        /// A property wrapper for the <see cref="DescriptionProperty"/>
        /// dependency property:<br/>
        /// Description
        /// </summary>
        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }

        #endregion
        public Balloon()
        {
            InitializeComponent();
            TaskbarIcon.AddBalloonClosingHandler(this, OnBalloonClosing);
        }


        /// <summary>
        /// By subscribing to the <see cref="TaskbarIcon.BalloonClosingEvent"/>
        /// and setting the "Handled" property to true, we suppress the popup
        /// from being closed in order to display the fade-out animation.
        /// </summary>
        private void OnBalloonClosing(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            isClosing = true;
        }


        /// <summary>
        /// Resolves the <see cref="TaskbarIcon"/> that displayed
        /// the balloon and requests a close action.
        /// </summary>
        private void imgClose_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //the tray icon assigned this attached property to simplify access
            TaskbarIcon taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            taskbarIcon.CloseBalloon();
        }

        /// <summary>
        /// If the users hovers over the balloon, we don't close it.
        /// </summary>
        private void grid_MouseEnter(object sender, MouseEventArgs e)
        {
            //if we're already running the fade-out animation, do not interrupt anymore
            //(makes things too complicated for the sample)
            if (isClosing) return;

            //the tray icon assigned this attached property to simplify access
            TaskbarIcon taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            taskbarIcon.ResetBalloonCloseTimer();
        }

        /// <summary>
        /// Closes the popup once the fade-out animation completed.
        /// The animation was triggered in XAML through the attached
        /// BalloonClosing event.
        /// </summary>
        private void OnFadeOutCompleted(object sender, EventArgs e)
        {
            Popup pp = (Popup)Parent;
            pp.IsOpen = false;
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
