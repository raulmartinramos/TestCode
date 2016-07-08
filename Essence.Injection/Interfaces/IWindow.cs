using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Essence.Injection
{
    public interface IWindow
    {
        Essence.Enums.InterfaceEnums.EssenceWindows GetWindow();
        bool ShowWaitIndicator { get; set; }
        void ShowError(string error);
        void NotifyChange(Essence.Enums.ModelsEnum.TiposModels TipoModel, Essence.Enums.ModelsEnum.TiposMetodos TipoMetodo = Enums.ModelsEnum.TiposMetodos.Get);
        String GetDescripcion();
    }
}
