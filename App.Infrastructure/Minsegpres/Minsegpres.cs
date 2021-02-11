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
        protected readonly IGestionProcesos _repository;
        protected readonly IFolio _folio;

        public Minsegpres(IGestionProcesos repository, IFolio folio)
        {
            _repository = repository;
            _folio = folio;
        }

        public byte[] Sign(byte[] documento, string OTP, int id, string Run, string Nombre, bool TipoDocumento)
        {
            byte[] binario = null;

            using (MemoryStream ms = new MemoryStream())
            {
                using (var reader = new PdfReader(documento))
                {
                    using (PdfStamper stamper = new PdfStamper(reader, ms, '\0', true))
                    {
                        var respuesta = new ResponseMessage();

                        var model = _repository.GetById<FirmaDocumentoGenerico>(id);

                        if (TipoDocumento == false)
                        {
                            string folio = null;

                            //si el documento ya tiene folio, no solicitarlo nuevamente
                            if (string.IsNullOrWhiteSpace(model.Folio))
                            {
                                try
                                {
                                    //var _folioResponse = _folio.GetFolio(string.Join(", ", emailsFirmantes), firmaDocumento.TipoDocumentoCodigo, persona.SubSecretaria);
                                    var _folioResponse = _folio.GetFolio(string.Join(", ", model.Email), "MEMO", model.Subsecretaria);
                                    if (_folioResponse == null)
                                        respuesta.Errors.Add("Error al llamar el servicio externo de folio");

                                    if (_folioResponse != null && _folioResponse.status == "ERROR")
                                        respuesta.Errors.Add(_folioResponse.error);

                                    model.Folio = _folioResponse.folio;
                                    folio = model.Folio;

                                    _repository.Update(model);
                                    _repository.Save();
                                }
                                catch (Exception ex)
                                {
                                    respuesta.Errors.Add(ex.Message);
                                }
                            }

                            //agregar tabla de verificacion
                            try
                            {
                                try
                                {
                                    //obtener informacion de la primera pagina
                                    var pagesize = reader.GetPageSize(1);
                                    var pdfContentFirstPage = stamper.GetOverContent(1);

                                    //estampa de folio
                                    //ColumnText.ShowTextAligned(pdfContentFirstPage, Element.ALIGN_LEFT, new Phrase(string.Format("Folio {0}", folio), new Font(Font.FontFamily.HELVETICA, 13, Font.BOLD, BaseColor.DARK_GRAY)), pagesize.Width - 182, pagesize.Height - 167, 0);
                                    ColumnText.ShowTextAligned(pdfContentFirstPage, Element.ALIGN_LEFT, new Phrase(string.Format("Folio {0}", folio), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 13, iTextSharp.text.Font.BOLD, BaseColor.DARK_GRAY)), pagesize.Width - 182, pagesize.Height - 167, 0);

                                    //estampa de fecha
                                    ColumnText.ShowTextAligned(pdfContentFirstPage, Element.ALIGN_LEFT, new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 13, iTextSharp.text.Font.BOLD, BaseColor.DARK_GRAY)), pagesize.Width - 182, pagesize.Height - 182, 0);
                                }
                                catch (System.Exception ex)
                                {
                                    throw new System.Exception("Error al insertar folio en el documento:" + ex.Message);
                                }
                            }
                            catch (System.Exception ex)
                            {
                                throw new System.Exception("Error al insertar tabla de validación de firma electrónica:" + ex.Message);
                            }
                        }

                        //agregar tabla de verificacion
                        try
                        {


                            //var img = Image.GetInstance(QR);
                            //var fontStandard = new Font(Font.FontFamily.HELVETICA, 9, Font.NORMAL, BaseColor.DARK_GRAY);
                            var fontStandard = new iTextSharp.text.Font(/*Font.FontFamily.HELVETICA, 9, Font.BOLD, BaseColor.DARK_GRAY*/);
                            var fontBold = new iTextSharp.text.Font(/*Font.FontFamily.HELVETICA, 9, Font.BOLD, BaseColor.DARK_GRAY*/);
                            var pdfContentLastPage = stamper.GetOverContent(reader.NumberOfPages);
                            var table = new PdfPTable(3) { HorizontalAlignment = Element.ALIGN_CENTER, WidthPercentage = 100 };

                            table.TotalWidth = 520f;
                            table.SetWidths(new float[] { 10f, 19f, 10f });
                            table.AddCell(new PdfPCell(new Phrase("Documento Firmado, de acuerdo a lo establecido en artículo 40 del Reglamento de la Ley Nº 19.799", fontBold)) { Colspan = 2, BorderColor = BaseColor.DARK_GRAY });
                            table.AddCell(new PdfPCell(new Phrase("Fecha de Firma", fontBold)) { });
                            table.AddCell(new PdfPCell(new Phrase("Nombre Funcionario", fontBold)) { BorderColor = BaseColor.DARK_GRAY });
                            table.AddCell(new PdfPCell(new Phrase(Nombre, fontBold)) { BorderColor = BaseColor.DARK_GRAY });
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
            // Status Code 200
            if (response.StatusCode == HttpStatusCode.OK)
            {
                Root respuesta = JsonConvert.DeserializeObject<Root>(response.Content);

                foreach (var file in respuesta.files)
                {
                    var contenido = file.content;
                    binario = Convert.FromBase64String(contenido);
                }
            }
            // Status Code 400
            else if (response.StatusCode == HttpStatusCode.BadRequest)
            {
                Root respuesta = JsonConvert.DeserializeObject<Root>(response.Content);

                var mensajeError = respuesta.error;

                var statusCode = respuesta.status;

                throw new System.Exception("Status Code : " + statusCode + " - Mensaje : " + mensajeError + ".");
            }
            // Status Code 403
            else if (response.StatusCode == HttpStatusCode.Forbidden)
            {
                Root respuesta = JsonConvert.DeserializeObject<Root>(response.Content);

                var mensajeError = respuesta.error;

                var statusCode = respuesta.status;

                throw new System.Exception("Status Code : " + statusCode + " - Mensaje : " + mensajeError + ".");
            }
            // Status Code 404
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                Root respuesta = JsonConvert.DeserializeObject<Root>(response.Content);

                var mensajeError = respuesta.error;

                var statusCode = respuesta.status;

                throw new System.Exception("Status Code : " + statusCode + " - Mensaje : " + mensajeError + ".");
            }
            // Status Code 412
            else if (response.StatusCode == HttpStatusCode.PreconditionFailed)
            {

                throw new System.Exception("ERROR: Verificación de OTP fallido.Por favor vuelve a intentar.");
            }
            return binario;
        }
    }
}
