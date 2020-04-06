using System;
using System.Collections.Generic;
using System.Text;

using System.Web.UI;
using System.Web.UI.WebControls;

namespace FPC_WPNuevaCorrespondencia
{
    public class CheckBoxTemplate : ITemplate
    {
        public void InstantiateIn(System.Web.UI.Control container)
        {
            CheckBox chkBox = new CheckBox();

            chkBox.ID = "chkBox";

            container.Controls.Add(chkBox);
        }
    }
}
