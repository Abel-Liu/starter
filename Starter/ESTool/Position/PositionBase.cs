using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace ESTool.Position
{
    public class PositionBase
    {
        public PositionBase(IntPtr _startButtonHandle, System.Windows.Window _mainWindow)
        {
            this.startButtonHandle = _startButtonHandle;
            this.mainWindow = _mainWindow;
        }

        protected readonly string dllname = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ESRemote.dll");

        protected IntPtr startButtonHandle;
        protected System.Windows.Window mainWindow;

        /// <summary>
        /// explorer process
        /// </summary>
        protected Process process;

        public virtual void SetAppPos(bool moveRebar)
        { }

        public virtual void InsertButton()
        { }

        public virtual void RemoveButton()
        { }

        public virtual void InjectDll()
        {

        }

        public virtual void UnInjectDll()
        {

        }

    }
}
