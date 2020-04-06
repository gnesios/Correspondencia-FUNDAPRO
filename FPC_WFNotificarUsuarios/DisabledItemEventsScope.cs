using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;

namespace FPC_WFNotificarUsuarios
{
    class DisabledItemEventsScope : SPItemEventReceiver, IDisposable
    {
        public DisabledItemEventsScope()
        {
            base.DisableEventFiring();
        }

        #region IDisposable Members
        public void Dispose()
        {
            base.EnableEventFiring();
        }
        #endregion
    }
}
