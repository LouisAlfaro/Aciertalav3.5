using System;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Printing;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json.Linq;
using ZXing;
using ZXing.Common;

namespace AciertalaV3
{
    public partial class validation : Form
    {
        // — Campos de ticket —
        string _couponCode, _operatorInfo, _userInfo;
        string _sportLine, _eventLine, _dateLine, _marketSelection;
        string _stake, _totalOdds, _minWin, _maxWin;
        string _createdTime, _printedTime;

        // — Campos de documento del usuario —
        string _docType, _docNumber;

        // — Campos de verificación de jugador —
        string _playerFirstName, _playerDocNumber;


        public validation()
        {
            InitializeComponent();
            InitializeWebView();
        }


        private async void InitializeWebView()
        {
            await webView21.EnsureCoreWebView2Async(null);
            webView21.Source = new Uri("https://shop.aciertala.pe/home");

            webView21.CoreWebView2.WebResourceResponseReceived += async (sender, args) =>
            {
                var req = args.Request;
                var uri = req.Uri;

                // 1) Capturar checkout2
                if (req.Method == "POST" && uri.Contains("/api/v1/ticket/checkout2"))
                {
                    try
                    {
                        await HandleCheckout2Response(args);
                        PrintTicket();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error checkout2:\n" + ex.Message);
                    }
                }

                // 2) Capturar player-id-verification
                else if (req.Method == "POST" && uri.Contains("/api/v4/player-id-verification"))
                {
                    try
                    {
                        Stream s = await args.Response.GetContentAsync();
                        using (s)
                        using (var r = new StreamReader(s, Encoding.UTF8))
                        {
                            string body = await r.ReadToEndAsync();
                            var json = JToken.Parse(body);
                            var data = json["data"];
                            var val = data?["validation"];
                            _playerFirstName = val?["first_name"]?.ToString() ?? "";
                            _playerDocNumber = val?["document"]?["number"]?.ToString() ?? "";
                            // Ya guardados en las variables de la clase:
                            // _playerFirstName y _playerDocNumber
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error player-verif:\n" + ex.Message);
                    }
                }
            };
        }

        private async System.Threading.Tasks.Task HandleCheckout2Response(CoreWebView2WebResourceResponseReceivedEventArgs args)
        {
            Stream s = await args.Response.GetContentAsync();
            using (s)
            using (var r = new StreamReader(s, Encoding.UTF8))
            {
                string body = await r.ReadToEndAsync();
                JToken root = JToken.Parse(body);
                JToken first = root.Type == JTokenType.Array ? root.First : root;
                JToken data = first["data"] ?? first;
                JToken ticket = data["ticket"];
                JToken op = ticket["operator"];
                JToken usr = ticket["user"];
                JToken item = ticket["items"]?[0];

                // Ticket
                _couponCode = ticket["code"]?.ToString();
                _operatorInfo = $"{op["id"]} - {op["username"]}";
                _userInfo = $"{usr["id"]} - {usr["username"]}";

                // Documento del usuario
                _docType = usr["document_type"]?.ToString() ?? "";
                _docNumber = usr["document_number"]?.ToString() ?? "";

                // Apuesta
                string sport = item["sport"]?["name"]?.ToString();
                string category = item["category"]?["name"]?.ToString();
                _sportLine = $"{sport} - {category}";
                _eventLine = item["event_name"]?.ToString() ?? "";
                _dateLine = DateTimeOffset.Parse(item["event_date"]?.ToString() ?? "")
                                   .ToLocalTime()
                                   .ToString("dd.MM.yyyy HH:mm");
                _marketSelection = $"{item["market_name"]}-{item["odds_name"]}";

                _stake = ticket["stake"]?.ToString();
                _totalOdds = ticket["total_odds"]?.ToString();
                _minWin = ticket["min_win"]?.ToString();
                _maxWin = ticket["max_win"]?.ToString();

                _createdTime = DateTimeOffset.Parse(ticket["checkout_time"]?.ToString() ?? "")
                                   .ToLocalTime()
                                   .ToString("dd.MM.yyyy HH:mm");
                _printedTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
            }
        }


        private void PrintTicket()
        {
            var pd = new PrintDocument
            {
                DefaultPageSettings = { PaperSize = new PaperSize("Ticket", 300, 1400) }
            };
            pd.PrintPage += OnPrintPage;
            pd.Print();
        }


        private void OnPrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            float mgn = 5;
            float w = e.PageBounds.Width - mgn * 2;
            float y = mgn;
            var pen = Pens.Black;
            var brush = Brushes.Black;
            var fTitle = new Font("Arial", 12, FontStyle.Bold);
            var f = new Font("Arial", 9);
            var fSmall = new Font("Arial", 7);

            // —–– HEADER
            float hH = 60;
            g.DrawRectangle(pen, mgn, y, w, hH);
            g.DrawString("Aciértala", fTitle, brush, mgn + 5, y + hH / 2,
                new StringFormat { LineAlignment = StringAlignment.Center });
            using (var qr = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions { Width = (int)(hH - 10), Height = (int)(hH - 10), Margin = 0 }
            }.Write(_couponCode))
            {
                g.DrawImage(qr, mgn + w - (hH - 10) - 5, y + 5, hH - 10, hH - 10);
            }
            y += hH + 5;

            // —–– INFO
            float iH = 45;
            g.DrawRectangle(pen, mgn, y, w, iH);
            float mid = mgn + w * 0.5f;
            g.DrawString("ID cupón:", f, brush, mgn + 5, y + 5);
            g.DrawString(_couponCode, f, brush, mid, y + 5);
            g.DrawString("Operador:", f, brush, mgn + 5, y + 20);
            g.DrawString(_operatorInfo, f, brush, mid, y + 20);
            g.DrawString("Usuario:", f, brush, mgn + 5, y + 35);
            g.DrawString(_userInfo, f, brush, mid, y + 35);
            y += iH + 5;

            // —–– APUESTAS título
            float tH = 20;
            g.DrawString("Apuestas - Sencilla", f, brush, new RectangleF(mgn, y, w, tH),
                new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
            y += tH + 2;

            // —–– DETALLE
            float dH = 60;
            g.DrawRectangle(pen, mgn, y, w, dH);
            float dy = y + 5;
            g.DrawString(_sportLine, f, brush, mgn + 5, dy); dy += 15;
            g.DrawString(_eventLine, f, brush, mgn + 5, dy); dy += 15;
            g.DrawString(_dateLine, f, brush, mgn + 5, dy); dy += 15;
            g.DrawString(_marketSelection, f, brush, mgn + 5, dy);
            y += dH + 5;

            // —–– CUOTAS
            float oH = 20;
            g.DrawRectangle(pen, mgn, y, w, oH);
            g.DrawString("Cuotas Totales:", f, brush, mgn + 5, y + 3);
            g.DrawString(_totalOdds, f, brush, new RectangleF(mgn, y, w - 5, oH),
                new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center });
            y += oH + 5;

            // —–– IMPORTE
            float sH = 20;
            g.DrawRectangle(pen, mgn, y, w, sH);
            g.DrawString("Importe:", f, brush, mgn + 5, y + 3);
            g.DrawString($"||| S/ {_stake} |||", f, brush, new RectangleF(mgn, y, w - 5, sH),
                new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center });
            y += sH + 5;

            // —–– GANANCIAS
            float gH = 20;
            g.DrawString("Ganancias", f, brush, new RectangleF(mgn, y, w, gH),
                new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near });
            y += gH;
            using (var bigF = new Font("Arial", 13, FontStyle.Bold))
            {
                g.DrawString($"||| S/ {_minWin} - {_maxWin} |||", bigF, brush,
                    new RectangleF(mgn, y, w, gH),
                    new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Near });
            }
            y += gH + 5;

            // —–– DOCUMENTO
            float docH = 40;
            g.DrawRectangle(pen, mgn, y, w, docH);
            g.DrawString("Cliente:", f, brush, mgn + 5, y + 5);
            g.DrawString(_playerFirstName +" *****", f, brush, mgn + w * 0.5f, y + 5);
            g.DrawString("Número del Documento:", f, brush, mgn + 5, y + 20);
            g.DrawString(_playerDocNumber, f, brush, mgn + w * 0.5f, y + 20);
            y += docH + 5;

            // —–– FECHAS
            float dTH = 20;
            g.DrawString($"Creada : {_createdTime}", f, brush, mgn + 5, y + 3);
            g.DrawString($"Impresa: {_printedTime}", f, brush, new RectangleF(mgn, y, w, dTH),
                new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center });
            y += dTH + 5;

            // —–– LEGAL
            float lH = 40;
            g.DrawRectangle(pen, mgn, y, w, lH);
            g.DrawString("El propietario de este recibo es la única persona con\n" +
                         "derecho a retirar las ganancias potenciales.\n" +
                         "El cálculo final podría variar.",
                         fSmall, brush, new RectangleF(mgn, y, w, lH),
                         new StringFormat { Alignment = StringAlignment.Near, LineAlignment = StringAlignment.Near });
            y += lH + 5;

            // —–– CÓDIGO DE BARRAS PIE
            float cbH = 50;
            g.DrawImage(new BarcodeWriter
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions { Width = (int)w, Height = (int)cbH, Margin = 0 }
            }.Write(_couponCode), mgn, y, w, cbH);
        }
    }
}
