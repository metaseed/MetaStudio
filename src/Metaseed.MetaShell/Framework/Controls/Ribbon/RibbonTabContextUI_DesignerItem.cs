namespace Metaseed.MetaShell.Controls
{
    public class DesignerItemRibbonTabContextUI : RibbonTabContextUI
    {
        public override void Initialize()
        {
            base.Initialize();
        }
        //public override void SetDataContext(object c)
        //{
        //    DesignerItem di = (DesignerItem)c;
        //    if (di.LogicObj != null)
        //    {
        //        this.DataContext = di.LogicObj;
        //    }
        //}
        public override void Show(object c)
        {
           // DesignerItem di = (DesignerItem)c;
            base.Show(c);
        }
        public override void Hide(object c)
        {
            base.Hide(c);
        }

    }
}
