using AppTailler.Models;
using SignaturePad.Forms;
using System;
using System.IO;
using System.Linq;
using Xamarin.Forms;

namespace AppTailler.Views
{
    public partial class SignaturePadConfigView : ContentPage
    {
        private Relatorio rela;
        private usuarios us;
        private clientes cl;
        private locais lo;
        private string tipo;

        public SignaturePadConfigView(Relatorio rel, usuarios user, clientes cli, locais loc, string tip)
        {
            InitializeComponent();
            tipo = tip;
            rela = rel;
            us = user;
            cl = cli;
            lo = loc;
            NavigationPage.SetTitleIcon(this, "logo_title.png");
            BackgroundColor = new Color(0, 0, 0, 0.5);
        }
        private async void OnGetStats(object sender, EventArgs e)
        {
            String timeStamp = GetTimestamp(DateTime.Now);
            var points = padView.Points.ToArray();
            if (points.Length > 0)
            {
                var docFolder = "/storage/emulated/0/Pictures/Tailler/";
                var signatureFile = Path.Combine(docFolder, timeStamp + ".png");

                var image = await padView.GetImageStreamAsync(SignatureImageFormat.Png);
                byte[] m_Bytes = ReadToEnd(image);
                using (var dados = new DataAccess())
                {
                    if(tipo == "fechamento")
                    {
                        cl.AssinaturaArray = m_Bytes;
                        dados.AtualizarClientes(cl);
                    }
                    else if(tipo == "local")
                    {
                        lo.AssinaturaArray = m_Bytes;
                        dados.AtualizarLocais(lo);
                    }
                }
                if (rela == null)
                {
                    App.NP.Navigation.RemovePage(App.NP.Navigation.NavigationStack[App.NP.Navigation.NavigationStack.Count - 1]);
                    App.NP.Navigation.RemovePage(App.NP.Navigation.NavigationStack[App.NP.Navigation.NavigationStack.Count - 1]);
                    await App.NP.Navigation.PushAsync(new CadastroResponsavel(us, cl, lo, m_Bytes));
                    image.Dispose();
                }
                else
                {
                    App.NP.Navigation.RemovePage(App.NP.Navigation.NavigationStack[App.NP.Navigation.NavigationStack.Count - 1]);
                    App.NP.Navigation.RemovePage(App.NP.Navigation.NavigationStack[App.NP.Navigation.NavigationStack.Count - 1]);
                    await App.NP.Navigation.PushAsync(new Relatorio(us, cl, lo, m_Bytes));
                    image.Dispose();
                }
            }
        }
        public static byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }
        public static String GetTimestamp(DateTime value)
        {
            return value.ToString("yyyyMMddHHmmssffff");
        }
    }
}
