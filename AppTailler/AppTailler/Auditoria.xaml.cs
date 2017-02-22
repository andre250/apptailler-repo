using AppTailler.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using AppTailler.Menus;
using Xamarin.Forms;
using AppTailler.Cells;
using Plugin.Connectivity;
using System.Threading.Tasks;
using AppTailler.Pages;

namespace AppTailler
{

    public partial class Auditoria : ContentPage, INotifyPropertyChanged
    {
        private ObservableCollection<ItemGroups> grouped { get; set; }
        public List<areas_sql> areasList;
        public List<itens_sql> itensList;
        public ObservableCollection<ItemGroups> ListItens = new ObservableCollection<ItemGroups>();
        public List<auditoria> auditoriaList;
        public List<auditoria> ModifiedAuditoriasList;
        private StackLayout areasCell_selected;
        private StackLayout primeiraareasCell_selected;
        private locais localglobal;
        private clientes cli;
        private usuarios user;
        private RootPage menu;
        public string AreasCellColor { get; set; }
        private bool isLoading;
        public bool IsLoading
        {
            get
            {
                return this.isLoading;
            }

            set
            {
                this.isLoading = value;
                RaisePropertyChanged("IsLoading");
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void RaisePropertyChanged(string propName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }
        }
        public Auditoria(locais loc, clientes cl, usuarios us, RootPage menu_param)
        {
            IsLoading = false;
            BindingContext = this;
            InitializeComponent();
            menu = menu_param;
            //Ação do evento de clicar no botão menu lateral superior esquerdo
            var menuTut = new TapGestureRecognizer();
            menuTut.Tapped += delegate
            {
                menu.IsPresented = true;
            };

            menuLogo.GestureRecognizers.Add(menuTut);
            //Inicia o loading da tela
            int firstTime = 0;
            //Armazena as variaveis
            localglobal = loc;
            cli = cl;
            user = us;
            if (localglobal.IdAuditoria == 0)
            {
                firstTime = 1;
            }
            //Inicia construção da tela
            GetAsync(firstTime, loc);
            NavigationPage.SetTitleIcon(this, "logotitle.png");
            //Finaliza loading

        }
        public async void GetAsync(int firstTime, locais local)
        {
            IsLoading = true;
            //Inicia loading
            var internet = CrossConnectivity.Current.IsConnected;
            if (!internet)
            {
                List<areas> listadeAreas = await GetAuditoria();
                if (listadeAreas.Count == 0)
                {
                    await DisplayAlert("Alerta", "Você precisa carregar a lista de auditoria online ou não existe auditoria cadastrada", "Ok");
                    App.NP.Navigation.RemovePage(App.NP.Navigation.NavigationStack[App.NP.Navigation.NavigationStack.Count - 1]);
                    return;
                }
                using (var dados = new DataAccess())
                {
                    localglobal.IdAuditoria = localglobal.IdLocal;
                    localglobal.StatusAuditoria = 1;
                    cli.StatusAuditoria = 1;
                    dados.AtualizarStatusAuditoriaClientes(cli);
                    dados.AtualizarStatusAuditoriaLocais(localglobal);
                }
                atualizarTela(listadeAreas, listadeAreas[0].ListaItens);
            }
            else
            {
                //Atualizar atualizar = new Atualizar();
                //auditoriaList = await atualizar.AtualizarAuditoria(localglobal, cli);
                using (var dados = new DataAccess())
                {
                    localglobal.IdAuditoria = localglobal.IdLocal;
                    localglobal.StatusAuditoria = 1;
                    cli.StatusAuditoria = 1;
                    dados.AtualizarStatusAuditoriaClientes(cli);
                    dados.AtualizarStatusAuditoriaLocais(localglobal);
                }
                List<areas> listadeAreas = await GetAuditoria();
                if (listadeAreas.Count == 0)
                {
                    await DisplayAlert("Alerta", "Não existe auditoria cadastrada.", "Ok");
                    App.NP.Navigation.RemovePage(App.NP.Navigation.NavigationStack[App.NP.Navigation.NavigationStack.Count - 1]);
                    return;
                }
                atualizarTela(listadeAreas, listadeAreas[0].ListaItens);
            }
            IsLoading = false;
            //App.GLOBAL_SETS.horaFim = GetTimestamp(DateTime.Now);
            //await DisplayAlert("alerta", "Comecou: " + App.GLOBAL_SETS.horaInicio + "\n Terminou: " + App.GLOBAL_SETS.horaFim,
            //    "Ok");

        }
        private void generateGrid(List<areas> content)
        {
            int x = 0;
            foreach (areas item in content)
            {
                var areasCell_label = new Label()
                {
                    Text = item.AreaNome,
                    TextColor = Color.White,
                    HorizontalOptions = LayoutOptions.Center,
                    FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label))
                };

                //var imageStatus = new Image
                //{
                //    VerticalOptions = LayoutOptions.Fill,
                //    Source = "concluido.png"
                //};

                var areasCell = new StackLayout()
                {
                    Spacing = 0,
                    VerticalOptions = LayoutOptions.Fill,
                    HorizontalOptions = LayoutOptions.Fill,
                    Padding = new Thickness(30, 30, 30, 30),
                    Children = { areasCell_label }
                };

                if (x == 0 && areasCell_selected == null)
                {
                    areasCell.BackgroundColor = Color.FromHex("#384D79");
                    primeiraareasCell_selected = areasCell;
                }
                else if
                (areasCell_selected == null)
                {
                    areasCell.BackgroundColor = Color.FromHex("#5b5b5e");
                }
                var btTut = new TapGestureRecognizer();
                btTut.Tapped += async (object s, EventArgs e) =>
                {
                    await App.NP.Navigation.PushAsync(new Loading("Estamos carregando sua auditoria. Aguarde..."));
                    if (areasCell_selected != null)
                    {
                        areasCell_selected.BackgroundColor = Color.FromHex("#5b5b5e");
                    }
                    if (primeiraareasCell_selected != null)
                    {
                        primeiraareasCell_selected.BackgroundColor = Color.FromHex("#5b5b5e");
                    }

                    StackLayout stack = (StackLayout)s;
                    stack.BackgroundColor = Color.FromHex("#384D79");
                    areasCell_selected = stack;
                    List<areas> listadeAreas = new List<areas>();
                    listadeAreas.Clear();
                    listadeAreas = await GetAuditoria();
                    ObservableCollection<ItemGroups> listaitens;
                    lay_gridListas.Children.Clear();
                    int index = listadeAreas.FindIndex(z => z.AreaNome == item.AreaNome);
                    listaitens = listadeAreas[index].ListaItens;
                    if (listaitens == null)
                    {
                        await DisplayAlert("Alerta", "Não existem itens para esta área.", "Ok");
                    }
                    else
                    {
                        atualizarTela(content, listaitens);
                    }
                    App.NP.Navigation.RemovePage(App.NP.Navigation.NavigationStack[App.NP.Navigation.NavigationStack.Count - 1]);
                };

                areasCell.GestureRecognizers.Add(btTut);
                gridView.Children.Add(areasCell, x, 0);
                x++;
            }
        }
        private void generateListas(ObservableCollection<ItemGroups> list)
        {
            var listView = new ListView();
            listView.VerticalOptions = LayoutOptions.FillAndExpand;
            listView.ItemsSource = list;
            listView.IsGroupingEnabled = true;
            listView.GroupDisplayBinding = new Binding("ItemDescricao");
            listView.GroupHeaderTemplate = new DataTemplate(typeof(HeaderCell));
            listView.RowHeight = 125;
            listView.ItemTemplate = new DataTemplate(() =>
                {
                    var itemLabel = new Label
                    {
                        FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                        TextColor = Color.FromHex("#5B5B5E"),
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.FillAndExpand,
                        WidthRequest = 175,
                    };
                    itemLabel.SetBinding(Label.TextProperty, "SubDescricao");

                    var imgConfirm = new Image
                    {
                        WidthRequest = 40,
                        HeightRequest = 40,
                    };
                    imgConfirm.SetBinding(Image.SourceProperty, "SubCheckConfirmSource");

                    var imgUnconfirm = new Image
                    {
                        WidthRequest = 40,
                        HeightRequest = 40,
                    };
                    imgUnconfirm.SetBinding(Image.SourceProperty, "SubCheckUnconfirmSource");

                    var imgNa = new Image
                    {
                        WidthRequest = 40,
                        HeightRequest = 40,
                    };
                    imgNa.SetBinding(Image.SourceProperty, "SubCheckNaSource");

                    var botConfirm = new Button
                    {
                        WidthRequest = 40,
                        HeightRequest = 40,
                        BackgroundColor = Color.Transparent,
                    };
                    botConfirm.SetBinding(Button.CommandParameterProperty, ".");
                    botConfirm.Command = new Command<SubItems>((SubItems itemSelecionado) =>
                    {
                        imgConfirm.Source = null;
                        imgUnconfirm.Source = null;
                        imgNa.Source = null;
                        GC.Collect();
                        imgConfirm.Source = "confirmon.png";
                        imgUnconfirm.Source = "unconfirmnull.png";
                        imgNa.Source = "nanull.png";
                        using (var dados = new DataAccess())
                        {
                            auditoria auditoria = dados.GetAuditoria(itemSelecionado.IdAuditoria);
                            auditoria.SubCheck = 1;
                            auditoria.SubCheckUnconfirmSource = "unconfirmnull.png";
                            auditoria.SubCheckConfirmSource = "confirmon.png";
                            auditoria.SubCheckNaSource = "nanull.png";
                            auditoria.SubChecked = 1;
                            dados.CheckAtualizarAuditoria(auditoria);
                        }

                    });

                    var botUnconfirm = new Button
                    {
                        WidthRequest = 40,
                        HeightRequest = 40,
                        BackgroundColor = Color.Transparent
                    };

                    botUnconfirm.SetBinding(Button.CommandParameterProperty, ".");
                    botUnconfirm.Command = new Command<SubItems>(async (SubItems itemSelecionado) =>
                    {
                        botUnconfirm.IsEnabled = false;
                        imgConfirm.Source = null;
                        imgUnconfirm.Source = null;
                        imgNa.Source = null;
                        GC.Collect();
                        imgUnconfirm.Source = "unconfirmon.png";
                        imgConfirm.Source = "confirmnull.png";
                        imgNa.Source = "nanull.png";
                        Page modal2 = new SelecionarMotivos(itemSelecionado.IdAuditoria, localglobal);
                        await App.NP.Navigation.PushAsync(modal2);
                        using (var dados = new DataAccess())
                        {
                            auditoria auditoria = dados.GetAuditoria(itemSelecionado.IdAuditoria);
                            auditoria.SubCheck = 2;
                            auditoria.SubCheckUnconfirmSource = "unconfirmon.png";
                            auditoria.SubCheckConfirmSource = "confirmnull.png";
                            auditoria.SubCheckNaSource = "nanull.png";
                            auditoria.SubChecked = 1;
                            dados.CheckAtualizarAuditoria(auditoria);
                        }
                        botUnconfirm.IsEnabled = true;
                    });

                    var botNa = new Button
                    {
                        WidthRequest = 40,
                        HeightRequest = 40,
                        BackgroundColor = Color.Transparent
                    };
                    botNa.SetBinding(Button.CommandParameterProperty, ".");
                    botNa.Command = new Command<SubItems>((SubItems itemSelecionado) =>
                    {
                        imgConfirm.Source = null;
                        imgUnconfirm.Source = null;
                        imgNa.Source = null;
                        GC.Collect();
                        imgNa.Source = "naon.png";
                        imgConfirm.Source = "confirmnull.png";
                        imgUnconfirm.Source = "unconfirmnull.png";
                        using (var dados = new DataAccess())
                        {
                            auditoria auditoria = dados.GetAuditoria(itemSelecionado.IdAuditoria);
                            auditoria.SubCheck = 3;
                            auditoria.SubCheckUnconfirmSource = "unconfirmnull.png";
                            auditoria.SubCheckConfirmSource = "confirmnull.png";
                            auditoria.SubCheckNaSource = "naon.png";
                            auditoria.SubChecked = 1;
                            dados.CheckAtualizarAuditoria(auditoria);
                        }
                    });

                    AbsoluteLayout.SetLayoutFlags(itemLabel,
                                    AbsoluteLayoutFlags.PositionProportional);

                    AbsoluteLayout.SetLayoutFlags(botConfirm,
                        AbsoluteLayoutFlags.PositionProportional);

                    AbsoluteLayout.SetLayoutFlags(imgConfirm,
                        AbsoluteLayoutFlags.PositionProportional);

                    AbsoluteLayout.SetLayoutFlags(botUnconfirm,
                        AbsoluteLayoutFlags.PositionProportional);

                    AbsoluteLayout.SetLayoutFlags(imgUnconfirm,
                        AbsoluteLayoutFlags.PositionProportional);

                    AbsoluteLayout.SetLayoutFlags(botNa,
                        AbsoluteLayoutFlags.PositionProportional);

                    AbsoluteLayout.SetLayoutFlags(imgNa,
                        AbsoluteLayoutFlags.PositionProportional);

                    AbsoluteLayout.SetLayoutBounds(itemLabel,
                        new Rectangle(0.2,
                            0.3, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

                    AbsoluteLayout.SetLayoutBounds(botConfirm,
                        new Rectangle(0.7,
                            0.2, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

                    AbsoluteLayout.SetLayoutBounds(imgConfirm,
                        new Rectangle(0.7,
                            0.2, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

                    AbsoluteLayout.SetLayoutBounds(botUnconfirm,
                        new Rectangle(0.85,
                            0.2, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

                    AbsoluteLayout.SetLayoutBounds(imgUnconfirm,
                        new Rectangle(0.85,
                            0.2, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

                    AbsoluteLayout.SetLayoutBounds(botNa,
                        new Rectangle(1,
                            0.2, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

                    AbsoluteLayout.SetLayoutBounds(imgNa,
                        new Rectangle(1,
                            0.2, AbsoluteLayout.AutoSize, AbsoluteLayout.AutoSize));

                    var absoluteLayout = new AbsoluteLayout
                    {
                        Padding = new Thickness(0, 20, 20, 0),
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.CenterAndExpand,
                        Children = { itemLabel, botConfirm, imgConfirm, botUnconfirm, imgUnconfirm, botNa, imgNa },
                    };

                    return new ViewCell
                    {
                        View = new StackLayout
                        {
                            Children = { absoluteLayout },
                            Orientation = StackOrientation.Vertical,
                            VerticalOptions = LayoutOptions.FillAndExpand,
                            BackgroundColor = Color.White
                        },

                    };
                });
            lay_gridListas.Children.Add(listView);
        }
        private void atualizarTela(List<areas> content, ObservableCollection<ItemGroups> list)
        {
            IsLoading = true;

            generateGrid(content);

            generateListas(list);

            var nome_resp = "";
            if (localglobal.RespNome != null)
            {
                nome_resp = localglobal.RespNome;
            }
            else if (cli.Administrador == null)
            {
                nome_resp = cli.Administrador;
            }
            else if (cli.Gestor == null)
            {
                nome_resp = cli.Gestor;
            }
            footer_up_label.Text = "Responsável: " + nome_resp + "\nÚltima auditoria: " + localglobal.DtUltimaAuditoria + "\nÚltima nota: " + localglobal.ultimaNota;
            footer_down_label.Text = cli.Unidade + " > " + localglobal.Nome + " > " + localglobal.Referencia;
            IsLoading = false;
        }
        private async Task<List<areas>> GetAuditoria()
        {
            return await Task.Run(() => CarregarAuditoria());
        }
        private List<areas> CarregarAuditoria()
        {
            using (var dados = new DataAccess())
            {
                auditoriaList = dados.GetAllAuditorias(localglobal.IdLocal);
                var listareas = auditoriaList
                    .GroupBy(o => o.IdArea)
                    .Select(o => o.FirstOrDefault());
                List<areas> ListAreas = new List<areas>();
                foreach (auditoria a in listareas)
                {
                    var listitens = from d in auditoriaList
                                    where d.IdArea == a.IdArea
                                    group d by d.IdItem;
                    var grouped = new ObservableCollection<ItemGroups>();
                    foreach (var aud in listitens)
                    {
                        auditoria item = aud.FirstOrDefault();

                        var Itens = new ItemGroups() { ItemCodigo = item.IdItem, ItemDescricao = item.DescItem };
                        foreach (var audi in aud)
                        {
                            auditoria subitem = audi;
                            SubItems subit = new SubItems()
                            {
                                ItemCodigo = item.IdItem,
                                IdAuditoria = subitem.IdAuditoria,
                                SubCheckConfirmSource = subitem.SubCheckConfirmSource,
                                SubPeso = subitem.SubPeso,
                                SubCodigoTexto = subitem.SubDescId,
                                SubPhotoUrl = subitem.SubPhotoUrl,
                                SubCheckNaSource = subitem.SubCheckNaSource,
                                SubCodigo = subitem.IdSubItem,
                                SubCheckUnconfirmSource = subitem.SubCheckUnconfirmSource,
                                SubDescricao = subitem.DescSubItem,
                                SubChecked = subitem.SubChecked,
                                SubCheck = subitem.SubCheck,
                                SubMotivoBytes = subitem.SubMotivoBytes
                            };
                            Itens.Add(subit);
                        }
                        grouped.Add(Itens);
                    }
                    areas ar = new areas { AreaCodigo = a.IdArea, AreaNome = a.DescArea, AreaStatus = a.StatusArea, ListaItens = grouped };
                    ListAreas.Add(ar);
                }
                dados.Dispose();
                return ListAreas;
            }
        }
        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("HH:mm:ss:ffff");
        }
        protected override void OnAppearing()
        { 
            base.OnAppearing();
        }
    }

}
