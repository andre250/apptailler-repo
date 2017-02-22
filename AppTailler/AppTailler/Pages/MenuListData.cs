using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace AppTailler
{
    public class MenuListData : List<MenuItem>
    {
        public MenuListData(int tela_atual)
        {
            if (tela_atual == 1)
            {
                this.Add(new MenuItem()
                {
                    Title = "UNIDADES",
                    IconSource = "cliente.png",
                    TargetType = "cliente",
                    IsEnabled = false,
                    
                });

                this.Add(new MenuItem()
                {
                    Title = "LOCAIS",
                    IconSource = "unidade.png",
                    TargetType = "local",
                    IsEnabled = false,
                    
                });
                this.Add(new MenuItem()
                {
                    Title = "AREAS",
                    IconSource = "areas.png",
                    TargetType = "auditoria",
                    IsEnabled = false,
                    
                });
                this.Add(new MenuItem()
                {
                    Title = "FINALIZAR \nAUDITORIA",
                    IconSource = "cadastro.png",
                    TargetType = "cadastroresponsavel",
                    IsEnabled = false,
                    
                });
                this.Add(new MenuItem()
                {
                    Title = "ORDEM DO SERVIÇO",
                    IconSource = "ordem.png",
                    TargetType = "relatorio",
                    IsEnabled = false,
                });
            }
            else if (tela_atual == 2)
            {
                this.Add(new MenuItem()
                {
                    Title = "UNIDADES",
                    IconSource = "cliente.png",
                    TargetType = "cliente",
                    IsEnabled = true
                });

                this.Add(new MenuItem()
                {
                    Title = "LOCAIS",
                    IconSource = "unidade.png",
                    TargetType = "local",
                    IsEnabled = false
                });
                this.Add(new MenuItem()
                {
                    Title = "AREAS",
                    IconSource = "areas.png",
                    TargetType = "auditoria",
                    IsEnabled = false
                });
                this.Add(new MenuItem()
                {
                    Title = "FINALIZAR \nAUDITORIA",
                    IconSource = "cadastro.png",
                    TargetType = "cadastroresponsavel",
                    IsEnabled = false
                });
                this.Add(new MenuItem()
                {
                    Title = "ORDEM DO SERVIÇO",
                    IconSource = "ordem.png",
                    TargetType = "relatorio",
                    IsEnabled = true,
                    
                });
            }
            else if (tela_atual == 3)
            {
                this.Add(new MenuItem()
                {
                    Title = "UNIDADES",
                    IconSource = "cliente.png",
                    TargetType = "cliente",
                    IsEnabled = true,
                    
                });

                this.Add(new MenuItem()
                {
                    Title = "LOCAIS",
                    IconSource = "unidade.png",
                    TargetType = "local",
                    IsEnabled = true,
                    
                });
                this.Add(new MenuItem()
                {
                    Title = "AREAS",
                    IconSource = "areas.png",
                    TargetType = "auditoria",
                    IsEnabled = false,
                    
                });
                this.Add(new MenuItem()
                {
                    Title = "FINALIZAR \nAUDITORIA",
                    IconSource = "cadastro.png",
                    TargetType = "cadastroresponsavel",
                    IsEnabled = true,
                    
                });
                this.Add(new MenuItem()
                {
                    Title = "ORDEM DO SERVIÇO",
                    IconSource = "ordem.png",
                    TargetType = "relatorio",
                    IsEnabled = true,
                    
                });
            }
            else if (tela_atual == 4)
            {
                this.Add(new MenuItem()
                {
                    Title = "UNIDADES",
                    IconSource = "cliente.png",
                    TargetType = "cliente",
                    IsEnabled = true,
                    
                });

                this.Add(new MenuItem()
                {
                    Title = "LOCAIS",
                    IconSource = "unidade.png",
                    TargetType = "local",
                    IsEnabled = true,
                    
                });
                this.Add(new MenuItem()
                {
                    Title = "AREAS",
                    IconSource = "areas.png",
                    TargetType = "auditoria",
                    IsEnabled = true,
                    
                });
                this.Add(new MenuItem()
                {
                    Title = "FINALIZAR \nAUDITORIA",
                    IconSource = "cadastro.png",
                    TargetType = "cadastroresponsavel",
                    IsEnabled = true,
                    
                });
                this.Add(new MenuItem()
                {
                    Title = "ORDEM DO SERVIÇO",
                    IconSource = "ordem.png",
                    TargetType = "relatorio",
                    IsEnabled = true,
                    
                });
            }
            else if (tela_atual == 5)
            {
                this.Add(new MenuItem()
                {
                    Title = "UNIDADES",
                    IconSource = "cliente.png",
                    TargetType = "cliente",
                    IsEnabled = true,
                    
                });

                this.Add(new MenuItem()
                {
                    Title = "LOCAIS",
                    IconSource = "unidade.png",
                    TargetType = "local",
                    IsEnabled = true,
                    
                });
                this.Add(new MenuItem()
                {
                    Title = "AREAS",
                    IconSource = "areas.png",
                    TargetType = "auditoria",
                    IsEnabled = true,
                    
                });
                this.Add(new MenuItem()
                {
                    Title = "FINALIZAR \nAUDITORIA",
                    IconSource = "cadastro.png",
                    TargetType = "cadastroresponsavel",
                    IsEnabled = true,
                    
                });
                this.Add(new MenuItem()
                {
                    Title = "ORDEM DO SERVIÇO",
                    IconSource = "ordem.png",
                    TargetType = "relatorio",
                    IsEnabled = true,
                    
                });
            }
        }
    }
}
