using System;
using System.Collections.Generic;
using System.Linq;
using App.Core.Interfaces;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using RestSharp;
using App.Model.Core;
using System.Net;
using App.Model.FirmaDocumentoGenerico;
using Newtonsoft.Json;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using iTextSharp.text.pdf;
using System.Drawing;
using iTextSharp.text;
using iTextSharp.text.pdf.security;
using System.Security.Cryptography.X509Certificates;

namespace App.Infrastructure.Minsegpres
{
    public class Minsegpres : IMinsegpres
    {
        public byte[] Sign(byte[] documento, string OTP, int id, string Run, string Nombre)
        {
            byte[] binario = null;

            using (MemoryStream ms = new MemoryStream())
            {
                using (var reader = new PdfReader(documento))
                {
                    using (PdfStamper stamper = new PdfStamper(reader, ms, '\0', true))
                    {
                        PdfSignatureAppearance signatureAppearance = stamper.SignatureAppearance;

                        //IExternalSignature externalSignature = new X509Certificate2Signature(null, "SHA-256");

                        ////agregar folio
                        //if (!folio.IsNullOrWhiteSpace())
                        //{
                        //    try
                        //    {
                        //        //obtener informacion de la primera pagina
                        //        var pagesize = reader.GetPageSize(1);
                        //        var pdfContentFirstPage = stamper.GetOverContent(1);

                        //        //estampa de folio
                        //        ColumnText.ShowTextAligned(pdfContentFirstPage, Element.ALIGN_LEFT, new Phrase(string.Format("Folio {0}", folio), new Font(Font.FontFamily.HELVETICA, 13, Font.BOLD, BaseColor.DARK_GRAY)), pagesize.Width - 182, pagesize.Height - 167, 0);

                        //        //estampa de fecha
                        //        ColumnText.ShowTextAligned(pdfContentFirstPage, Element.ALIGN_LEFT, new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), new Font(Font.FontFamily.HELVETICA, 13, Font.BOLD, BaseColor.DARK_GRAY)), pagesize.Width - 182, pagesize.Height - 182, 0);
                        //    }
                        //    catch (System.Exception ex)
                        //    {
                        //        throw new System.Exception("Error al insertar folio en el documento:" + ex.Message);
                        //    }
                        //}

                        //agregar tabla de verificacion
                        try
                        {
                            //var img = Image.GetInstance(QR);
                            //var fontStandard = new Font(Font.FontFamily.HELVETICA, 9, Font.NORMAL, BaseColor.DARK_GRAY);
                            var fontStandard = new iTextSharp.text.Font(/*Font.FontFamily.HELVETICA, 9, Font.BOLD, BaseColor.DARK_GRAY*/);
                            var fontBold = new iTextSharp.text.Font(/*Font.FontFamily.HELVETICA, 9, Font.BOLD, BaseColor.DARK_GRAY*/);
                            var pdfContentLastPage = stamper.GetOverContent(reader.NumberOfPages);
                            var table = new PdfPTable(3) { HorizontalAlignment = Element.ALIGN_CENTER, WidthPercentage = 100 };

                            //table.TotalWidth = 520f;
                            table.TotalWidth = 520f;
                            //table.SetWidths(new float[] { 8f, 25f, 6f });
                            table.SetWidths(new float[] { 12f, 15f, 12f });
                            //table.AddCell(new PdfPCell(new Phrase("Subsecretaría de Economía y Empresas de Menor Tamaño", fontBold)) { Colspan = 1, BorderColor = BaseColor.DARK_GRAY });
                            table.AddCell(new PdfPCell(new Phrase("Documento Firmado, de acuerdo a lo establecido en artículo 40 del Reglamento de la Ley Nº 19.799", fontBold)) { Colspan = 2, BorderColor = BaseColor.DARK_GRAY });
                            //table.AddCell(new PdfPCell() { Rowspan = 5 }).AddElement(img);
                            table.AddCell(new PdfPCell(new Phrase("Fecha de Firma", fontBold)) { });
                            //table.AddCell(new PdfPCell(new Phrase(string.Join(", ", firmantes), fontStandard)) { BorderColor = BaseColor.DARK_GRAY });
                            table.AddCell(new PdfPCell(new Phrase("Nombre Funcionario", fontBold)) { BorderColor = BaseColor.DARK_GRAY });
                            //table.AddCell(new PdfPCell(new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), fontStandard)) { BorderColor = BaseColor.DARK_GRAY });
                            //table.AddCell(new PdfPCell(new Phrase("Código de verificación", fontBold)) { BorderColor = BaseColor.DARK_GRAY });
                            //table.AddCell(new PdfPCell(new Phrase(documentoId.ToString(), fontStandard)) { BorderColor = BaseColor.DARK_GRAY });
                            table.AddCell(new PdfPCell(new Phrase(Nombre, fontBold)) { BorderColor = BaseColor.DARK_GRAY });
                            //table.AddCell(new PdfPCell(new Phrase(Run, fontStandard)) { BorderColor = BaseColor.DARK_GRAY });
                            table.AddCell(new PdfPCell(new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), fontStandard)) { BorderColor = BaseColor.DARK_GRAY });
                            table.WriteSelectedRows(0, -1, 43, 100, pdfContentLastPage);
                        }
                        catch (System.Exception ex)
                        {
                            throw new System.Exception("Error al insertar tabla de validación de firma electrónica:" + ex.Message);
                        }

                        stamper.Close();
                    }
                }
                documento = ms.ToArray();
            }

            string fileContent = Convert.ToBase64String(documento);

            DateTime issuedAt = DateTime.Now;
            DateTime expires = DateTime.Now.AddMinutes(30);

            string key = "e31f64431e424c60a26436de31aad96b";

            var securityKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials
                              (securityKey, SecurityAlgorithms.HmacSha256Signature);

            var header = new JwtHeader(credentials);

            var payload = new JwtPayload
               {
                   {"entity", "Subsecretaría de Economía y Empresas de Menor Tamaño"},
                   { "run", Run},
                   { "expiration", expires},
                   { "purpose", "Propósito General"},
               };

            var secToken = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();

            var tokenString = handler.WriteToken(secToken);

            var client = new RestClient("https://api.firma.test.digital.gob.cl/firma/v2/files/tickets");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("OTP", OTP);
            request.AddHeader("Content-Type", "application/json");

            request.AddParameter("application/json", "{\r\n\r\n\"api_token_key\": \"2c368309-d7b6-49ad-9e93-b27fdc58128e\",\r\n\"token\":\r\n\"" + tokenString + "\",\r\n \"files\": [\r\n {\r\n \"content-type\": \"application/pdf\",\r\n \"content\": \"" + fileContent + "\",\r\n \"description\": \"str\",\r\n \"checksum\": \"C4863E4F3CB93450C63F8BB24725E8AB8FC03B7B71619B756294BFB1E55D6507\"\r\n }\r\n ]\r\n}", ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Root respuesta = JsonConvert.DeserializeObject<Root>(response.Content);

                foreach (var file in respuesta.files)
                {
                    var contenido = file.content;
                    binario = Convert.FromBase64String(contenido);
                }
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                //Root respuesta = JsonConvert.DeserializeObject<Root>(response.ErrorMessage);

                //foreach (var file in respuesta.files)
                //{
                //    var contenido = file.content;
                //    binario = Convert.FromBase64String(contenido);
                //}
                // manejar error
                throw new System.Exception("El servicio externo de firma electrónica retornó falla.");

            }


            return binario;
        }
    }
}
