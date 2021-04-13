using System;
using System.Linq;
using App.Core.Interfaces;
using System.Text;
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
using iTextSharp.text;
using Image = iTextSharp.text.Image;

namespace App.Infrastructure.Minsegpres
{
    public class Minsegpres : IMinsegpres
    {
        protected readonly IGestionProcesos _repository;
        protected readonly IFolio _folio;
        protected readonly IFile _file;

        public Minsegpres(IGestionProcesos repository, IFolio folio, IFile file)
        {
            _repository = repository;
            _folio = folio;
            _file = file;

        }

        public byte[] SignConOtp(byte[] documento, string OTP, int id, string Run, string Nombre, bool TipoDocumento, int DocumentoId)
        {
            //var url_tramites_en_linea = "https://tramites.economia.gob.cl/";
            //var _qrResponse = _file.CreateQr(string.Concat(url_tramites_en_linea, "/GPDocumentoVerificacion/Details/", DocumentoId));

            byte[] binario = null;

            //using (MemoryStream ms = new MemoryStream())

            //{
            //    using (var reader = new PdfReader(documento))
            //    {
            //        using (PdfStamper stamper = new PdfStamper(reader, ms, '\0', true))
            //        {
            //            var respuesta = new ResponseMessage();

            //            var model = _repository.GetById<FirmaDocumentoGenerico>(id);

            //            if (model.TipoDocumento == false)
            //            {
            //                string folio = null;

            //                //si el documento ya tiene folio, no solicitarlo nuevamente
            //                if (string.IsNullOrWhiteSpace(model.Folio))
            //                {
            //                    try
            //                    {
            //                        //var _folioResponse = _folio.GetFolio(string.Join(", ", emailsFirmantes), firmaDocumento.TipoDocumentoCodigo, persona.SubSecretaria);
            //                        var _folioResponse = _folio.GetFolio(string.Join(", ", model.Email), "MEMO", model.Subsecretaria);
            //                        if (_folioResponse == null)
            //                            respuesta.Errors.Add("Error al llamar el servicio externo de folio");

            //                        if (_folioResponse != null && _folioResponse.status == "ERROR")
            //                            respuesta.Errors.Add(_folioResponse.error);

            //                        model.Folio = _folioResponse.folio;
            //                        folio = model.Folio;

            //                        _repository.Update(model);
            //                        _repository.Save();
            //                    }
            //                    catch (Exception ex)
            //                    {
            //                        respuesta.Errors.Add(ex.Message);
            //                    }
            //                }

            //                //agregar tabla de verificacion
            //                try
            //                {
            //                    try
            //                    {
            //                        //obtener informacion de la primera pagina
            //                        var pagesize = reader.GetPageSize(1);
            //                        var pdfContentFirstPage = stamper.GetOverContent(1);

            //                        //estampa de folio
            //                        //ColumnText.ShowTextAligned(pdfContentFirstPage, Element.ALIGN_LEFT, new Phrase(string.Format("Folio {0}", folio), new Font(Font.FontFamily.HELVETICA, 13, Font.BOLD, BaseColor.DARK_GRAY)), pagesize.Width - 182, pagesize.Height - 167, 0);
            //                        ColumnText.ShowTextAligned(pdfContentFirstPage, Element.ALIGN_LEFT, new Phrase(string.Format("Folio {0}", folio), new Font(iTextSharp.text.Font.FontFamily.HELVETICA, 13, iTextSharp.text.Font.BOLD, BaseColor.DARK_GRAY)), pagesize.Width - 182, pagesize.Height - 167, 0);

            //                        //estampa de fecha
            //                        ColumnText.ShowTextAligned(pdfContentFirstPage, Element.ALIGN_LEFT, new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), new Font(iTextSharp.text.Font.FontFamily.HELVETICA, 13, iTextSharp.text.Font.BOLD, BaseColor.DARK_GRAY)), pagesize.Width - 182, pagesize.Height - 182, 0);
            //                    }
            //                    catch (Exception ex)
            //                    {
            //                        throw new Exception("Error al insertar folio en el documento:" + ex.Message);
            //                    }
            //                }
            //                catch (Exception ex)
            //                {
            //                    throw new Exception("Error al insertar tabla de validación de firma electrónica:" + ex.Message);
            //                }
            //            }

            //            //agregar tabla de verificacion
            //            try
            //            {
            //                //var img = Image.GetInstance(QR);
            //                var img = Image.GetInstance(_qrResponse);
            //                //var fontStandard = new Font(Font.FontFamily.HELVETICA, 9, Font.NORMAL, BaseColor.DARK_GRAY);
            //                var fontStandard = new Font(iTextSharp.text.Font.FontFamily.HELVETICA, 9, iTextSharp.text.Font.NORMAL, BaseColor.DARK_GRAY);
            //                //var fontBold = new iTextSharp.text.Font(Font.FontFamily.HELVETICA, 9, Font.BOLD, BaseColor.DARK_GRAY);
            //                var fontBold = new Font(iTextSharp.text.Font.FontFamily.HELVETICA, 9, iTextSharp.text.Font.BOLD, BaseColor.DARK_GRAY);
            //                var pdfContentLastPage = stamper.GetOverContent(reader.NumberOfPages);
            //                var table = new PdfPTable(3) { HorizontalAlignment = Element.ALIGN_CENTER, WidthPercentage = 100 };

            //                table.TotalWidth = 520f;
            //                table.SetWidths(new float[] { 8f, 25f, 6f });
            //                table.AddCell(new PdfPCell(new Phrase("Información de firma electrónica:", fontBold)) { Colspan = 2, BorderColor = BaseColor.DARK_GRAY });
            //                table.AddCell(new PdfPCell() { Rowspan = 5 }).AddElement(img);
            //                table.AddCell(new PdfPCell(new Phrase("Firmantes", fontBold)) { });
            //                //table.AddCell(new PdfPCell(new Phrase(string.Join(", ", firmantes), fontStandard)) { BorderColor = BaseColor.DARK_GRAY });
            //                table.AddCell(new PdfPCell(new Phrase(string.Join(", ", Nombre), fontStandard)) { BorderColor = BaseColor.DARK_GRAY });
            //                table.AddCell(new PdfPCell(new Phrase("Fecha de firma", fontBold)) { BorderColor = BaseColor.DARK_GRAY });
            //                table.AddCell(new PdfPCell(new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), fontStandard)) { BorderColor = BaseColor.DARK_GRAY });
            //                table.AddCell(new PdfPCell(new Phrase("Código de verificación", fontBold)) { BorderColor = BaseColor.DARK_GRAY });
            //                //table.AddCell(new PdfPCell(new Phrase(documentoId.ToString(), fontStandard)) { BorderColor = BaseColor.DARK_GRAY });
            //                table.AddCell(new PdfPCell(new Phrase(string.Join(", ", DocumentoId), fontStandard)) { BorderColor = BaseColor.DARK_GRAY });
            //                table.AddCell(new PdfPCell(new Phrase("URL de verificación", fontBold)) { BorderColor = BaseColor.DARK_GRAY });
            //                //table.AddCell(new PdfPCell(new Phrase(url, fontStandard)) { BorderColor = BaseColor.DARK_GRAY });
            //                table.AddCell(new PdfPCell(new Phrase(string.Join(", ", url_tramites_en_linea), fontStandard)) { BorderColor = BaseColor.DARK_GRAY });

            //                table.WriteSelectedRows(0, -1, 43, 100, pdfContentLastPage);
            //            }
            //            catch (Exception ex)
            //            {
            //                throw new Exception("Error al insertar tabla de validación de firma electrónica:" + ex.Message);
            //            }

            //            stamper.Close();
            //        }
            //    }
            //    documento = ms.ToArray();
            //}

            string fileContent = Convert.ToBase64String(documento);

            DateTime issuedAt = DateTime.Now;
            DateTime expires = DateTime.Now.AddMinutes(30);

            var headerNew = new
            {
                alg = "HS256",
                typ = "JWT"
            };

            var headerPart = Base64UrlEncoder.Encode(JsonConvert.SerializeObject(headerNew));

            var payloadNew = new
            {
                entity = "Subsecretaría de Economía y Empresas de Menor Tamaño",
                run = Run,
                expiration = expires,
                purpose = "Propósito General"
                //purpose = "Desatendido"
            };

            var payloadPart = Base64UrlEncoder.Encode(JsonConvert.SerializeObject(payloadNew));

            var secret = "8d7a6d0fea8541b99b0dce110fd0d077";
            var sha256 = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes($"{headerPart}.{payloadPart}"));
            var hash = Base64UrlEncoder.Encode(hashBytes);

            var jwt = $"{headerPart}.{payloadPart}.{hash}";


            //Testing
            var client = new RestClient("https://api.firma.cert.digital.gob.cl/firma/v2/files/tickets");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("OTP", OTP);
            request.AddHeader("Content-Type", "application/json");
            //Testing
            //request.AddParameter("application/json", "{\r\n\r\n\"api_token_key\": \"012edbcc-7eb7-4043-8c16-a7702f1ffc40\",\r\n\"token\":\r\n\"" + tokenString + "\",\r\n \"files\": [\r\n {\r\n \"content-type\": \"application/pdf\",\r\n \"content\": \"" + fileContent + "\",\r\n \"description\": \"str\",\r\n \"checksum\": \"C4863E4F3CB93450C63F8BB24725E8AB8FC03B7B71619B756294BFB1E55D6507\"\r\n }\r\n ]\r\n}", ParameterType.RequestBody);
            request.AddParameter("application/json", "{\r\n\r\n\"api_token_key\": \"012edbcc-7eb7-4043-8c16-a7702f1ffc40\",\r\n\"token\":\r\n\"" + jwt + "\",\r\n \"files\": [\r\n {\r\n \"content-type\": \"application/pdf\",\r\n \"content\": \"" + fileContent + "\",\r\n \"description\": \"str\",\r\n \"checksum\": \"C4863E4F3CB93450C63F8BB24725E8AB8FC03B7B71619B756294BFB1E55D6507\"\r\n }\r\n ]\r\n}", ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            Root obj = JsonConvert.DeserializeObject<Root>(response.Content);


            if (response.StatusCode == HttpStatusCode.OK)
                foreach (var file in obj.files)
                    binario = Convert.FromBase64String(file.content);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception(string.Join(",", obj.files.Select(q=>q.status)));

            return binario;
        }

        public byte[] SignSinOtp(byte[] documento, int id, string Run, string Nombre, bool TipoDocumento, int DocumentoId)
        {

            byte[] binario = null;

            string fileContent = Convert.ToBase64String(documento);

            DateTime issuedAt = DateTime.Now;
            DateTime expires = DateTime.Now.AddMinutes(30);

            var headerNew = new
            {
                alg = "HS256",
                typ = "JWT"
            };

            var headerPart = Base64UrlEncoder.Encode(JsonConvert.SerializeObject(headerNew));

            var payloadNew = new
            {
                entity = "Subsecretaría de Economía y Empresas de Menor Tamaño",
                run = Run,
                expiration = expires,
                purpose = "Desatendido"
            };

            var payloadPart = Base64UrlEncoder.Encode(JsonConvert.SerializeObject(payloadNew));

            var secret = "8d7a6d0fea8541b99b0dce110fd0d077";
            var sha256 = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes($"{headerPart}.{payloadPart}"));
            var hash = Base64UrlEncoder.Encode(hashBytes);

            var jwt = $"{headerPart}.{payloadPart}.{hash}";

            //Testing
            var client = new RestClient("https://api.firma.cert.digital.gob.cl/firma/v2/files/tickets");
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            //request.AddHeader("OTP", OTP);
            request.AddHeader("Content-Type", "application/json");
            //Testing
            request.AddParameter("application/json", "{\r\n\r\n\"api_token_key\": \"012edbcc-7eb7-4043-8c16-a7702f1ffc40\",\r\n\"token\":\r\n\"" + jwt + "\",\r\n \"files\": [\r\n {\r\n \"content-type\": \"application/pdf\",\r\n \"content\": \"" + fileContent + "\",\r\n \"description\": \"str\",\r\n \"checksum\": \"C4863E4F3CB93450C63F8BB24725E8AB8FC03B7B71619B756294BFB1E55D6507\"\r\n }\r\n ]\r\n}", ParameterType.RequestBody);

            IRestResponse response = client.Execute(request);

            Root obj = JsonConvert.DeserializeObject<Root>(response.Content);


            if (response.StatusCode == HttpStatusCode.OK)
                foreach (var file in obj.files)
                    binario = Convert.FromBase64String(file.content);

            if (response.StatusCode != HttpStatusCode.OK)
                throw new Exception(string.Join(",", obj.files.Select(q => q.status)));

            return binario;
        }
    }
}
