using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.POCO;
using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DawnQuant.App.ViewModels
{
    [POCOViewModel]
    public class MainWindowModel
    {
        protected ICurrentWindowService CurrentWindowService { get { return this.GetService<ICurrentWindowService>(); } }
        protected INotifyIconService NotifyIconService { get { return this.GetService<INotifyIconService>(); } }
        protected IMessageBoxService MessageBoxService { get { return this.GetService<IMessageBoxService>(); } }


        public event Action ShowMainWindow;

        protected void OnShowMainWindow()
        {
            ShowMainWindow?.Invoke();
        }

        public void ActivateMainWindow()
        {
            OnShowMainWindow();
        }

    }
}
