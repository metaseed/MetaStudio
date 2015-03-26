using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Controls;
using System.Windows;


namespace Metaseed.Windows.Controls
{
    //http://social.msdn.microsoft.com/Forums/en/wpf/thread/86409299-b63a-4e65-baa6-1c52ff98f84b
    public class ProportionalPanel : Panel
    {
        protected override Size ArrangeOverride(Size finalSize)
        {
            var itemsHeight = finalSize.Height / Children.Count;
            for (var i = 0; i < Children.Count; i++)
                Children[i].Arrange(new Rect(0, i * itemsHeight, finalSize.Width, itemsHeight));
            return finalSize;
        }
    }

}
