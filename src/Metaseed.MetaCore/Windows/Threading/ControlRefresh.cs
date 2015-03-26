using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
namespace Metaseed.Windows.Threading
{
    public static class ExtensionMethods
    {

        private static Action EmptyDelegate = delegate() { };



        public static void Refresh(this UIElement uiElement)
        {

            uiElement.Dispatcher.BeginInvoke(DispatcherPriority.Render, EmptyDelegate);

        }
        /*
         private void LoopingMethod()

{

   for (int i = 0; i < 10; i++)

   {

      label1.Content = i.ToString();

      label1.Refresh();

      Thread.Sleep(500);

   }

}
         */
    }
}
