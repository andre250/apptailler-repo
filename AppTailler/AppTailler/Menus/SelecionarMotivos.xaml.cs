using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AppTailler.Models;
using Xamarin.Forms;
using System.IO;
using AppTailler.Menus;
using System.Linq;

namespace AppTailler
{
    public partial class SelecionarMotivos : ContentPage
    {
        private ObservableCollection<motivos> motivosSelecionados = new ObservableCollection<motivos>();
        private List<motivos> motivosList = new List<motivos>();
        private motivos motivoSelecionado;
        private string idAudit;
        private int motivoDefault;
        private int idSub;
        private locais loc;
        private MediaFile _mediaFile;
        private byte[] targetImageByte;
        private byte[] bytes;
        public SelecionarMotivos(string idAuditoria, locais local)
        {
            InitializeComponent();
            loc = local;
            idAudit = idAuditoria;
        }

        public async void GetAsync(string idAuditoria)
        {
            using (var dados = new DataAccess())
            {
                auditoria auditoria = dados.GetAuditoria(idAuditoria);
                if (auditoria.SubPhotoUrl != null)
                {
                    var imagemFotoTemp = new Image
                    {
                        WidthRequest = 70,
                        HeightRequest = 70
                    };
                    string DB_ANAGRAFICA_FILENAME = auditoria.SubPhotoUrl + ".jpg";
                    imagemFotoTemp.Source = ImageSource.FromFile("/storage/emulated/0/Pictures/Tailler/" + DB_ANAGRAFICA_FILENAME);
                    listaFotos.Children.Clear();
                    listaFotos.Children.Add(imagemFotoTemp);
                }

                idSub = auditoria.IdSubItem;


                if (App.motivoSelected != null)
                {
                    motivos motivo = dados.GetMotivos(auditoria.SubDescId);
                    motivosSelecionados.Clear();
                    motivoSelecionado = null;
                    motivosSelecionados.Add(App.motivoSelected);
                    motivoSelecionado = App.motivoSelected;
                    App.motivoSelected = null;
                }
                else if (auditoria.SubDescId != 0)
                {
                    motivos motivo = dados.GetMotivos(auditoria.SubDescId);
                    motivosSelecionados.Clear();
                    motivosSelecionados.Add(motivo);
                }
                motivosList = new List<motivos>();
                motivosList = dados.GetAllMotivos(idSub);
                BackgroundColor = new Color(0, 0, 0, 0.5);
                if (motivoSelecionado == null && motivosSelecionados.Count == 0)
                {
                    motivos mot = new motivos()
                    {
                        IdTextoVistoria = 0,
                        idSubItem = 0,
                        txtNumTexto = 0,
                        txtTexto = auditoria.SubDescTexto
                    };
                    motivosSelecionados.Clear();
                    if (auditoria.SubDescTexto != null)
                    {
                        motivosSelecionados.Add(mot);
                    }

                }
                listMotivos.ItemsSource = motivosSelecionados;

                listMotivos.ItemTemplate = new DataTemplate(() =>
                {
                    var itemLabel = new Label
                    {
                        FontSize = 16,
                        TextColor = Color.Black,
                        VerticalOptions = LayoutOptions.Fill,
                        HorizontalOptions = LayoutOptions.FillAndExpand
                    };
                    itemLabel.SetBinding(Label.TextProperty, "txtTexto");

                    var stackLayout = new StackLayout
                    {
                        Children = { itemLabel },
                        BackgroundColor = Color.FromHex("#E8EAEB"),
                        Padding = new Thickness(5, 5, 5, 5),
                        HorizontalOptions = LayoutOptions.FillAndExpand,
                        VerticalOptions = LayoutOptions.Fill
                    };
                    return new ViewCell
                    {
                        View = new StackLayout
                        {
                            Children = { stackLayout },
                        },
                    };
                });
            }
        }

        async void closeClicked(object sender, EventArgs e)
        {
            await DisplayAlert("Alerta", "Ao sair suas alterações não serão salvas. Clique no botão salvar, para salva-las", "OK");
            App.NP.Navigation.RemovePage(App.NP.Navigation.NavigationStack[App.NP.Navigation.NavigationStack.Count - 1]);
        }

        public async void SalvarClicked(object sender, EventArgs e)
        {
            try
            {
                if (motivoSelecionado == null
                    && String.IsNullOrEmpty(escreverMotivo.Text)
                    && (motivosSelecionados.Any() || motivosSelecionados == null || motivosSelecionados.Count == 0))
                {
                    await DisplayAlert("Alerta", "Você deve inserir pelo menos um motivo.", "Sim");
                    return;
                }
                else
                {
                    using (var dados = new DataAccess())
                    {
                        if (!String.IsNullOrEmpty(escreverMotivo.Text))
                        {
                            var motivoNovo = await DisplayAlert("Alerta", "Você está inserindo um motivo novo deseja continuar?", "Sim", "Não");
                            if (!motivoNovo)
                            {
                                return;
                            }
                            else
                            {
                                if (motivoSelecionado == null)
                                {
                                    motivos mot = new motivos()
                                    {
                                        IdTextoVistoria = 0,
                                        idSubItem = 0,
                                        txtNumTexto = 0,
                                        txtTexto = escreverMotivo.Text
                                    };
                                    motivoSelecionado = mot;
                                }
                                motivoSelecionado.IdTextoVistoria = 0;
                                motivoSelecionado.txtTexto = escreverMotivo.Text;
                            }
                        }
                        auditoria auditoria = dados.GetAuditoria(idAudit);
                        auditoria.IdAuditoria = auditoria.IdAuditoria;
                        if (motivoSelecionado != null)
                        {
                            auditoria.SubDescId = motivoSelecionado.IdTextoVistoria;
                            auditoria.SubDescTexto = motivoSelecionado.txtTexto;
                        }
                        auditoria.SubPhotoUrl = auditoria.SubPhotoUrl;
                        auditoria.SubMotivoBytes = auditoria.SubMotivoBytes;
                        if (_mediaFile != null)
                        {
                            var stream = _mediaFile.GetStream();
                            targetImageByte = ReadFully(stream);
                            auditoria.SubMotivoBytes = targetImageByte;
                        }
                        dados.AtualizarAuditoria(auditoria);
                        await DisplayAlert("Sucesso", "Salvo com sucesso!", "Ok");
                    }
                }
                App.NP.Navigation.RemovePage(App.NP.Navigation.NavigationStack[App.NP.Navigation.NavigationStack.Count - 1]);
            }
             catch
            {

            }
        }

        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }

        public async void botFotoClicked(object sender, EventArgs e)
        {
            await CrossMedia.Current.Initialize();
            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("Sem câmera.", "A câmera está indisponível.", "OK");
                return;
            }
            String timeStamp = GetTimestamp(DateTime.Now);
            _mediaFile = await CrossMedia.Current.TakePhotoAsync(
                new StoreCameraMediaOptions
                {
                    SaveToAlbum = true,
                    Directory = "Tailler",
                    Name = timeStamp,
                    PhotoSize = PhotoSize.Medium,
                    CompressionQuality = 50
                });
            using (var memoryStream = new MemoryStream())
            {
                _mediaFile.GetStream().CopyTo(memoryStream);
                bytes = memoryStream.ToArray();
            }
            using (var dados = new DataAccess())
            {
                auditoria auditoria = dados.GetAuditoria(idAudit);
                auditoria.SubPhotoUrl = timeStamp;
                auditoria.SubMotivoBytes = bytes;
                dados.AtualizarAuditoria(auditoria);
            }
            if (_mediaFile == null)
                return;

            var imagemFoto = new Image
            {
                WidthRequest = 70,
                HeightRequest = 70
            };

            var btTut = new TapGestureRecognizer();
            btTut.Tapped += async delegate
            {
                var answer = await DisplayAlert("O que você deseja fazer?", "Você tem certeza que deseja deletar essa foto?", "Sim", "Não");
                if (answer)
                {
                    listaFotos.Children.Remove(imagemFoto);
                    using (var dados = new DataAccess())
                    {
                        auditoria auditoria = dados.GetAuditoria(idAudit);
                        auditoria.SubPhotoUrl = null;
                        dados.AtualizarAuditoria(auditoria);
                    }

                }
            };

            imagemFoto.GestureRecognizers.Add(btTut);

            imagemFoto.Source = ImageSource.FromStream(() =>
            {
                var stream = _mediaFile.GetStream();
                return stream;
            });
            // _mediaFile.Dispose();
            listaFotos.Children.Add(imagemFoto);
        }

        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            GetAsync(idAudit);

        }

        protected async void botOnClick(object sender, EventArgs e)
        {
            await App.NP.Navigation.PushAsync(new MotivosListView(motivosList));
        }
    }
}