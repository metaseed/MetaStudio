using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Metaseed.MVVM.Commands
{
    public class RibbonUIData
    {
        
        private RibbonUIPosition _uiPosition;
        public RibbonUIPosition UiPosition
        {
            get
            {
                return this._uiPosition;
            }
            set
            {
                if ((this._uiPosition != null))
                {
                    if ((_uiPosition.Equals(value) != true))
                    {
                        this._uiPosition = value;
                        //this.OnPropertyChanged("UiPosition");
                    }
                }
                else
                {
                    this._uiPosition = value;
                    //this.OnPropertyChanged("UiPosition");
                }
            }
        }

        virtual public string ValidateAndSerialize()
        {
            return string.Empty;
        }
    }
}
