using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
namespace Metaseed.MetaShell.Services
{
    /// <summary>
    /// Logs errors to a window shown on screen
    /// </summary>
    public class WindowLogger : LoggerImplementation
    {
        /// <summary>
        /// Logs the specified error.
        /// </summary>
        /// <param name="error">The error to log.</param>
        public override void LogError(string error)
        {
            Window errorForm = new Window();
            if (Application.Current.Windows.Count > 0)
            {
                errorForm.Width = Application.Current.Windows[0].Width;
                errorForm.Height = Application.Current.Windows[0].Height;
                errorForm.Left = Application.Current.Windows[0].Left;
                errorForm.Top = Application.Current.Windows[0].Top;
                errorForm.WindowStartupLocation = WindowStartupLocation.Manual;
                errorForm.Topmost = true;
            }
            else
            {
                errorForm.Width = 600;
                errorForm.Height = 1000;
                errorForm.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            }

            errorForm.Title = "An error has occured:";

            RichTextBox errorBox = new RichTextBox();
            errorForm.Content=errorBox;
            errorBox.Margin = new Thickness(5);
            errorBox.AppendText( error);
            errorBox.FontFamily = new FontFamily("Courier New");
            errorBox.IsReadOnly = true;
            errorBox.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

            //Button errorOk = new Button();
            //errorForm.Controls.Add(errorOk);
            //errorOk.Top = errorForm.ClientRectangle.Height - 25;
            //errorOk.Left = errorForm.ClientRectangle.Width - 5 - errorOk.Width;
            //errorOk.Text = "&OK";
            //errorOk.DialogResult = DialogResult.Cancel;
            //errorOk.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
            //errorOk.FlatStyle = FlatStyle.System;
            //errorForm.CancelButton = errorOk;
            //errorForm.acce = errorOk;

            errorForm.ShowDialog();
        }
    }
}
