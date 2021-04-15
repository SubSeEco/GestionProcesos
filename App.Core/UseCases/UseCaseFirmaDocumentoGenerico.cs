using System;
using App.Model.Core;
using App.Model.FirmaDocumentoGenerico;
using App.Core.Interfaces;
using App.Model.Sigper;
using System.Linq;
using App.Model.DTO;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
//using iTextSharp.text.pdf;
//using iTextSharp.text;
//using Image = iTextSharp.text.Image;

namespace App.Core.UseCases
{
    public class UseCaseFirmaDocumentoGenerico
    {
        protected readonly IGestionProcesos _repository;
        //protected readonly IHSM _hsm;
        protected readonly ISigper _sigper;
        protected readonly IEmail _email;
        protected readonly IFolio _folio;
        protected readonly IFile _file;
        protected readonly IMinsegpres _minsegpres;
        public UseCaseFirmaDocumentoGenerico(IGestionProcesos repository)
        {
            _repository = repository;
        }

        public UseCaseFirmaDocumentoGenerico(IGestionProcesos repository, ISigper sigper, IFile file, IFolio folio, /* IHSM hsm, */IEmail email, IMinsegpres minsegpres)
        {
            _repository = repository;
            _sigper = sigper;
            _file = file;
            _folio = folio;
            //_hsm = hsm;
            _email = email;
            _minsegpres = minsegpres;
        }
        public ResponseMessage Insert(FirmaDocumentoGenerico obj)
        {
            var response = new ResponseMessage();

            try
            {
                if (response.IsValid)
                {
                    _repository.Create(obj);
                    _repository.Save();
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }
        public ResponseMessage Update(FirmaDocumentoGenerico obj)
        {
            var response = new ResponseMessage();

            try
            {
                if (response.IsValid)
                {
                    _repository.Update(obj);
                    _repository.Save();
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }
        public ResponseMessage Delete(int id)
        {
            var response = new ResponseMessage();

            try
            {
                var obj = _repository.GetById<FirmaDocumentoGenerico>(id);
                if (obj == null)
                    response.Errors.Add("Dato no encontrado");

                if (response.IsValid)
                {
                    _repository.Delete(obj);
                    _repository.Save();
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public ResponseMessage Firma(byte[][] documentos, string OTP, string tokenJWT, int id, string Rut, string Nombre, bool TipoDocumento, int DocumentoId)
        {
            var response = new ResponseMessage();
            var model = _repository.GetById<FirmaDocumentoGenerico>(id);

            var url_tramites_en_linea = "https://tramites.economia.gob.cl/";
            var _qrResponse = _file.CreateQr(string.Concat(url_tramites_en_linea, "/GPDocumentoVerificacion/Details/", DocumentoId));

            foreach (var documento in documentos)
            {
                using (MemoryStream ms = new MemoryStream())

                {
                    using (var reader = new PdfReader(documento))
                    {
                        using (PdfStamper stamper = new PdfStamper(reader, ms, '\0', true))
                        {
                            var respuesta = new ResponseMessage();

                            //var model = _repository.GetById<FirmaDocumentoGenerico>(id);

                            if (model.TipoDocumento == false)
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
                                        ColumnText.ShowTextAligned(pdfContentFirstPage, Element.ALIGN_LEFT, new Phrase(string.Format("Folio {0}", folio), new Font(iTextSharp.text.Font.FontFamily.HELVETICA, 13, iTextSharp.text.Font.BOLD, BaseColor.DARK_GRAY)), pagesize.Width - 182, pagesize.Height - 167, 0);

                                        //estampa de fecha
                                        ColumnText.ShowTextAligned(pdfContentFirstPage, Element.ALIGN_LEFT, new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), new Font(iTextSharp.text.Font.FontFamily.HELVETICA, 13, iTextSharp.text.Font.BOLD, BaseColor.DARK_GRAY)), pagesize.Width - 182, pagesize.Height - 182, 0);
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new Exception("Error al insertar folio en el documento:" + ex.Message);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    throw new Exception("Error al insertar tabla de validación de firma electrónica:" + ex.Message);
                                }
                            }

                            //agregar tabla de verificacion
                            try
                            {
                                //var img = Image.GetInstance(QR);
                                var img = Image.GetInstance(_qrResponse);
                                //var fontStandard = new Font(Font.FontFamily.HELVETICA, 9, Font.NORMAL, BaseColor.DARK_GRAY);
                                var fontStandard = new Font(iTextSharp.text.Font.FontFamily.HELVETICA, 9, iTextSharp.text.Font.NORMAL, BaseColor.DARK_GRAY);
                                //var fontBold = new iTextSharp.text.Font(Font.FontFamily.HELVETICA, 9, Font.BOLD, BaseColor.DARK_GRAY);
                                var fontBold = new Font(iTextSharp.text.Font.FontFamily.HELVETICA, 9, iTextSharp.text.Font.BOLD, BaseColor.DARK_GRAY);
                                var pdfContentLastPage = stamper.GetOverContent(reader.NumberOfPages);
                                var table = new PdfPTable(3) { HorizontalAlignment = Element.ALIGN_CENTER, WidthPercentage = 100 };

                                table.TotalWidth = 520f;
                                table.SetWidths(new float[] { 8f, 25f, 6f });
                                table.AddCell(new PdfPCell(new Phrase("Información de firma electrónica:", fontBold)) { Colspan = 2, BorderColor = BaseColor.DARK_GRAY });
                                table.AddCell(new PdfPCell() { Rowspan = 5 }).AddElement(img);
                                table.AddCell(new PdfPCell(new Phrase("Firmantes", fontBold)) { });
                                //table.AddCell(new PdfPCell(new Phrase(string.Join(", ", firmantes), fontStandard)) { BorderColor = BaseColor.DARK_GRAY });
                                table.AddCell(new PdfPCell(new Phrase(string.Join(", ", Nombre), fontStandard)) { BorderColor = BaseColor.DARK_GRAY });
                                table.AddCell(new PdfPCell(new Phrase("Fecha de firma", fontBold)) { BorderColor = BaseColor.DARK_GRAY });
                                table.AddCell(new PdfPCell(new Phrase(DateTime.Now.ToString("dd/MM/yyyy"), fontStandard)) { BorderColor = BaseColor.DARK_GRAY });
                                table.AddCell(new PdfPCell(new Phrase("Código de verificación", fontBold)) { BorderColor = BaseColor.DARK_GRAY });
                                //table.AddCell(new PdfPCell(new Phrase(documentoId.ToString(), fontStandard)) { BorderColor = BaseColor.DARK_GRAY });
                                table.AddCell(new PdfPCell(new Phrase(string.Join(", ", DocumentoId), fontStandard)) { BorderColor = BaseColor.DARK_GRAY });
                                table.AddCell(new PdfPCell(new Phrase("URL de verificación", fontBold)) { BorderColor = BaseColor.DARK_GRAY });
                                //table.AddCell(new PdfPCell(new Phrase(url, fontStandard)) { BorderColor = BaseColor.DARK_GRAY });
                                table.AddCell(new PdfPCell(new Phrase(string.Join(", ", url_tramites_en_linea), fontStandard)) { BorderColor = BaseColor.DARK_GRAY });

                                table.WriteSelectedRows(0, -1, 43, 100, pdfContentLastPage);
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("Error al insertar tabla de validación de firma electrónica:" + ex.Message);
                            }

                            stamper.Close();
                        }
                    }
                    //documento = ms.ToArray();
                }

                var binario = this._minsegpres.SignConOtp(documento, OTP, id, Rut, Nombre, TipoDocumento, DocumentoId);

                var persona = new Sigper();


                ////si el documento ya tiene folio, no solicitarlo nuevamente
                //if (string.IsNullOrWhiteSpace(model.Folio))
                //{
                //    try
                //    {
                //        //var _folioResponse = _folio.GetFolio(string.Join(", ", emailsFirmantes), firmaDocumento.TipoDocumentoCodigo, persona.SubSecretaria);
                //        var _folioResponse = _folio.GetFolio(string.Join(", ", "ereyes@economia.cl"), "MEMO", "ECONOMIA");
                //        if (_folioResponse == null)
                //            response.Errors.Add("Error al llamar el servicio externo de folio");

                //        if (_folioResponse != null && _folioResponse.status == "ERROR")
                //            response.Errors.Add(_folioResponse.error);

                //        model.Folio = _folioResponse.folio;

                //        _repository.Update(model);
                //        _repository.Save();
                //    }
                //    catch (Exception ex)
                //    {
                //        response.Errors.Add(ex.Message);
                //    }
                //}

                if (!response.IsValid)
                    return response;

                if (binario != null)
                {
                    model.ArchivoFirmado = binario;

                    DTOFileMetadata data = new DTOFileMetadata();
                    //int tipoDoc = 0;
                    //int IdDocto = 0;
                    string Name = string.Empty;

                    //tipoDoc = 15;
                    Name = "Documento Genérico nro" + " " + model.FirmaDocumentoGenericoId.ToString() + ".pdf";

                    ////var email = username;
                    //var email = "";
                    ////var email = UserExtended.Email(User);
                    //var doc = new Documento();
                    //doc.Fecha = DateTime.Now;
                    //doc.Email = email;
                    //doc.FileName = Name;
                    //doc.File = binario;
                    //doc.ProcesoId = model.ProcesoId.Value;
                    //doc.WorkflowId = model.WorkflowId.Value;
                    //doc.Signed = false;
                    //doc.Texto = data.Text;
                    //doc.Metadata = data.Metadata;
                    //doc.Type = data.Type;
                    //doc.TipoPrivacidadId = 1;
                    //doc.TipoDocumentoId = tipoDoc;

                    //doc.Folio = model.Folio;
                    //doc.File = model.ArchivoFirmado;

                    //_repository.Delete(doc);
                    //_repository.Create(doc);
                    ////_repository.Update(doc);
                    //_repository.Save();
                    int idDoctoViatico = 0;

                    /*si se crea una resolucion se debe validar que ya no exista otra, sino se actualiza la que existe*/
                    var firmagenerica = _repository.Get<Documento>(d => d.ProcesoId == model.ProcesoId);
                    if (firmagenerica != null)
                    {
                        foreach (var firm in firmagenerica)
                        {
                            if (firm.TipoDocumentoId == 15)
                                idDoctoViatico = firm.DocumentoId;
                        }
                    }

                    //var docOld = _repository.GetById<Documento>(DocumentoId);
                    var docOld = _repository.GetById<Documento>(idDoctoViatico);

                    docOld.Fecha = DateTime.Now;
                    docOld.File = binario;
                    docOld.Signed = false;
                    docOld.Texto = data.Text;
                    docOld.Metadata = data.Metadata;
                    docOld.Type = data.Type;
                    _repository.Update(docOld);
                    _repository.Save();
                }
            }
            return response;
        }

        //public ResponseMessage Firma2(byte[] documento, string OTP, string tokenJWT, int id, string Rut, string Nombre, string TipoDocumento, int DocumentoId)
        //{
        //    var response = new ResponseMessage();
        //    var model = _repository.GetById<FirmaDocumentoGenerico>(id);
        //    var binario = this._minsegpres.SignSinOtp(documento, OTP, id, Rut, Nombre, TipoDocumento, DocumentoId);

        //    var persona = new SIGPER();


        //    ////si el documento ya tiene folio, no solicitarlo nuevamente
        //    //if (string.IsNullOrWhiteSpace(model.Folio))
        //    //{
        //    //    try
        //    //    {
        //    //        //var _folioResponse = _folio.GetFolio(string.Join(", ", emailsFirmantes), firmaDocumento.TipoDocumentoCodigo, persona.SubSecretaria);
        //    //        var _folioResponse = _folio.GetFolio(string.Join(", ", "ereyes@economia.cl"), "MEMO", "ECONOMIA");
        //    //        if (_folioResponse == null)
        //    //            response.Errors.Add("Error al llamar el servicio externo de folio");

        //    //        if (_folioResponse != null && _folioResponse.status == "ERROR")
        //    //            response.Errors.Add(_folioResponse.error);

        //    //        model.Folio = _folioResponse.folio;

        //    //        _repository.Update(model);
        //    //        _repository.Save();
        //    //    }
        //    //    catch (Exception ex)
        //    //    {
        //    //        response.Errors.Add(ex.Message);
        //    //    }
        //    //}

        //    if (!response.IsValid)
        //        return response;

        //    if (binario != null)
        //    {
        //        model.ArchivoFirmado2 = binario;

        //        DTOFileMetadata data = new DTOFileMetadata();
        //        //int tipoDoc = 0;
        //        //int IdDocto = 0;
        //        string Name = string.Empty;

        //        //tipoDoc = 15;
        //        Name = "Documento Genérico nro" + " " + model.FirmaDocumentoGenericoId.ToString() + ".pdf";

        //        ////var email = username;
        //        //var email = "";
        //        ////var email = UserExtended.Email(User);
        //        //var doc = new Documento();
        //        //doc.Fecha = DateTime.Now;
        //        //doc.Email = email;
        //        //doc.FileName = Name;
        //        //doc.File = binario;
        //        //doc.ProcesoId = model.ProcesoId.Value;
        //        //doc.WorkflowId = model.WorkflowId.Value;
        //        //doc.Signed = false;
        //        //doc.Texto = data.Text;
        //        //doc.Metadata = data.Metadata;
        //        //doc.Type = data.Type;
        //        //doc.TipoPrivacidadId = 1;
        //        //doc.TipoDocumentoId = tipoDoc;

        //        //doc.Folio = model.Folio;
        //        //doc.File = model.ArchivoFirmado;

        //        //_repository.Delete(doc);
        //        //_repository.Create(doc);
        //        ////_repository.Update(doc);
        //        //_repository.Save();

        //        var docOld = _repository.GetById<Documento>(DocumentoId);
        //        docOld.Fecha = DateTime.Now;
        //        docOld.File = binario;
        //        docOld.Signed = false;
        //        docOld.Texto = data.Text;
        //        docOld.Metadata = data.Metadata;
        //        docOld.Type = data.Type;
        //        _repository.Update(docOld);
        //        _repository.Save();
        //    }

        //    return response;
        //}

        public ResponseMessage FirmaMasiva(byte[][] documentos,  int id, string Rut, string Nombre, bool TipoDocumento, int DocumentoId)
        {
            var response = new ResponseMessage();
            var model = _repository.GetById<FirmaDocumentoGenerico>(id);

            foreach (var documento in documentos)
            {
                var binario = this._minsegpres.SignSinOtp(documento, id, Rut, Nombre, TipoDocumento, DocumentoId);

                var persona = new Sigper();


                ////si el documento ya tiene folio, no solicitarlo nuevamente
                //if (string.IsNullOrWhiteSpace(model.Folio))
                //{
                //    try
                //    {
                //        //var _folioResponse = _folio.GetFolio(string.Join(", ", emailsFirmantes), firmaDocumento.TipoDocumentoCodigo, persona.SubSecretaria);
                //        var _folioResponse = _folio.GetFolio(string.Join(", ", "ereyes@economia.cl"), "MEMO", "ECONOMIA");
                //        if (_folioResponse == null)
                //            response.Errors.Add("Error al llamar el servicio externo de folio");

                //        if (_folioResponse != null && _folioResponse.status == "ERROR")
                //            response.Errors.Add(_folioResponse.error);

                //        model.Folio = _folioResponse.folio;

                //        _repository.Update(model);
                //        _repository.Save();
                //    }
                //    catch (Exception ex)
                //    {
                //        response.Errors.Add(ex.Message);
                //    }
                //}

                if (!response.IsValid)
                    return response;

                //if (binario != null)
                //{
                //    model.ArchivoFirmado = binario;

                //    DTOFileMetadata data = new DTOFileMetadata();
                //    int tipoDoc = 0;
                //    int IdDocto = 0;
                //    string Name = string.Empty;

                //    tipoDoc = 15;
                //    Name = "Documento Genérico nro" + " " + model.FirmaDocumentoGenericoId.ToString() + ".pdf";

                //    ////var email = username;
                //    //var email = "";
                //    ////var email = UserExtended.Email(User);
                //    //var doc = new Documento();
                //    //doc.Fecha = DateTime.Now;
                //    //doc.Email = email;
                //    //doc.FileName = Name;
                //    //doc.File = binario;
                //    //doc.ProcesoId = model.ProcesoId.Value;
                //    //doc.WorkflowId = model.WorkflowId.Value;
                //    //doc.Signed = false;
                //    //doc.Texto = data.Text;
                //    //doc.Metadata = data.Metadata;
                //    //doc.Type = data.Type;
                //    //doc.TipoPrivacidadId = 1;
                //    //doc.TipoDocumentoId = tipoDoc;

                //    //doc.Folio = model.Folio;
                //    //doc.File = model.ArchivoFirmado;

                //    //_repository.Delete(doc);
                //    //_repository.Create(doc);
                //    ////_repository.Update(doc);
                //    //_repository.Save();

                //    var docOld = _repository.GetById<Documento>(DocumentoId);
                //    docOld.Fecha = DateTime.Now;
                //    docOld.File = binario;
                //    docOld.Signed = false;
                //    docOld.Texto = data.Text;
                //    docOld.Metadata = data.Metadata;
                //    docOld.Type = data.Type;
                //    _repository.Update(docOld);
                //    _repository.Save();
                //}

                //model.ArchivoFirmado = binario;

                DTOFileMetadata data = new DTOFileMetadata();
                //int tipoDoc = 0;
                //int IdDocto = 0;
                string Name = string.Empty;

                //tipoDoc = 15;
                Name = "Documento Genérico nro" + " " + model.FirmaDocumentoGenericoId.ToString() + ".pdf";

                ////var email = username;
                //var email = "";
                ////var email = UserExtended.Email(User);
                //var doc = new Documento();
                //doc.Fecha = DateTime.Now;
                //doc.Email = email;
                //doc.FileName = Name;
                //doc.File = binario;
                //doc.ProcesoId = model.ProcesoId.Value;
                //doc.WorkflowId = model.WorkflowId.Value;
                //doc.Signed = false;
                //doc.Texto = data.Text;
                //doc.Metadata = data.Metadata;
                //doc.Type = data.Type;
                //doc.TipoPrivacidadId = 1;
                //doc.TipoDocumentoId = tipoDoc;

                //doc.Folio = model.Folio;
                //doc.File = model.ArchivoFirmado;

                //_repository.Delete(doc);
                //_repository.Create(doc);
                ////_repository.Update(doc);
                //_repository.Save();

                //var docOld = _repository.GetById<Documento>(DocumentoId);
                var docOld = _repository.GetById<Documento>(id);

                docOld.Fecha = DateTime.Now;
                docOld.File = binario;
                docOld.Signed = false;
                docOld.Texto = data.Text;
                docOld.Metadata = data.Metadata;
                docOld.Type = data.Type;
                _repository.Update(docOld);
                _repository.Save();
            }

            return response;
        }


        public ResponseMessage WorkflowUpdate(Workflow obj)
        {
            var response = new ResponseMessage();
            var persona = new Sigper();

            try
            {
                if (obj == null)
                    throw new Exception("Debe especificar un workflow.");
                var workflowActual = _repository.GetFirst<Workflow>(q => q.WorkflowId == obj.WorkflowId) ?? null;
                if (workflowActual == null)
                    throw new Exception("No se encontró el workflow.");
                var definicionworkflowlist = _repository.Get<DefinicionWorkflow>(q => q.Habilitado && q.DefinicionProcesoId == workflowActual.Proceso.DefinicionProcesoId).OrderBy(q => q.Secuencia).ThenBy(q => q.DefinicionWorkflowId) ?? null;
                if (!definicionworkflowlist.Any())
                    throw new Exception("No se encontró la definición de tarea del proceso asociado al workflow.");
                if (string.IsNullOrWhiteSpace(obj.Email))
                    throw new Exception("No se encontró el usuario que ejecutó el workflow.");
                if (workflowActual.DefinicionWorkflow != null && workflowActual.DefinicionWorkflow.RequiereAprobacionAlEnviar && (obj.TipoAprobacionId == null || obj.TipoAprobacionId == 0 || obj.TipoAprobacionId == 1))
                    throw new Exception("Es necesario aceptar o rechazar la tarea.");

                //generar tags de proceso
                workflowActual.Proceso.Tags += workflowActual.Proceso.GetTags();

                //generar tags de negocio
                var fdg = _repository.GetFirst<FirmaDocumentoGenerico>(q => q.ProcesoId == workflowActual.ProcesoId);
                if (fdg != null)
                    workflowActual.Proceso.Tags += fdg.GetTags();

                //si ingreso esta ok y firmador aprueba sin firma...
                if (workflowActual.DefinicionWorkflow.DefinicionProceso.Entidad.Codigo == App.Util.Enum.Entidad.FirmaDocumento.ToString() && workflowActual.DefinicionWorkflow.Secuencia == 2)
                {
                    var firma = _repository.GetFirst<FirmaDocumentoGenerico>(q => q.WorkflowId == obj.WorkflowId);
                    if (firma == null)
                        throw new Exception("No se encontró el ingreso de documento.");
                }

                //terminar workflow actual
                workflowActual.FechaTermino = DateTime.Now;
                workflowActual.Observacion = obj.Observacion;
                workflowActual.Terminada = true;
                workflowActual.Pl_UndCod = obj.Pl_UndCod;
                workflowActual.GrupoId = obj.GrupoId;
                workflowActual.Email = obj.Email;
                workflowActual.To = obj.To;

                //actualiazar tags
                workflowActual.Proceso.Tags = string.Concat(workflowActual.Proceso.GetTags(), " ", fdg.GetTags());

                if (workflowActual.DefinicionWorkflow.RequiereAprobacionAlEnviar)
                    workflowActual.TipoAprobacionId = obj.TipoAprobacionId;

                if (!workflowActual.DefinicionWorkflow.RequiereAprobacionAlEnviar)
                    workflowActual.TipoAprobacionId = (int)App.Util.Enum.TipoAprobacion.Aprobada;

                //determinar siguiente tarea en base a estado y definicion de proceso
                DefinicionWorkflow definicionWorkflow = null;

                //si permite multiple evaluacion generar la misma tarea
                if (workflowActual.DefinicionWorkflow.PermitirMultipleEvaluacion)
                    definicionWorkflow = _repository.GetById<DefinicionWorkflow>(workflowActual.DefinicionWorkflowId);

                if (workflowActual.TipoAprobacionId == (int)App.Util.Enum.TipoAprobacion.Aprobada)
                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.Secuencia > workflowActual.DefinicionWorkflow.Secuencia);
                else
                    definicionWorkflow = definicionworkflowlist.FirstOrDefault(q => q.DefinicionWorkflowId == workflowActual.DefinicionWorkflow.DefinicionWorkflowRechazoId);

                //en el caso de no existir mas tareas, cerrar proceso
                if (definicionWorkflow == null)
                {
                    workflowActual.Proceso.EstadoProcesoId = (int)App.Util.Enum.EstadoProceso.Terminado;
                    //workflowActual.Proceso.Terminada = true;
                    workflowActual.Proceso.FechaTermino = DateTime.Now;
                    _repository.Save();

                    //notificar al dueño del proceso
                    _email.NotificarFinProceso(workflowActual.Proceso,
                    _repository.GetFirst<Configuracion>(q => q.Nombre == nameof(App.Util.Enum.Configuracion.plantilla_fin_proceso)),
                    _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.AsuntoCorreoNotificacion));
                }

                //en el caso de existir mas tareas, crearla
                if (definicionWorkflow != null)
                {
                    var workflow = new Workflow();
                    workflow.FechaCreacion = DateTime.Now;
                    workflow.TipoAprobacionId = (int)App.Util.Enum.TipoAprobacion.SinAprobacion;
                    workflow.Terminada = false;
                    workflow.DefinicionWorkflow = definicionWorkflow;
                    workflow.ProcesoId = workflowActual.ProcesoId;
                    workflow.Mensaje = obj.Observacion;
                    //workflow.TareaPersonal = false;

                    if (definicionWorkflow.TipoEjecucionId == (int)App.Util.Enum.TipoEjecucion.CualquierPersonaGrupo)
                    {
                        if (obj.Pl_UndCod.HasValue)
                        {
                            var unidad = _sigper.GetUnidad(obj.Pl_UndCod.Value);
                            if (unidad == null)
                                throw new Exception("No se encontró la unidad en SIGPER.");

                            workflow.Pl_UndCod = unidad.Pl_UndCod;
                            workflow.Pl_UndDes = unidad.Pl_UndDes;
                            workflow.TareaPersonal = false;

                            var emails = _sigper.GetUserByUnidad(workflow.Pl_UndCod.Value).Select(q => q.Rh_Mail.Trim());
                            if (emails.Any())
                                workflow.Email = string.Join(";", emails);

                        }

                        if (!string.IsNullOrEmpty(obj.To))
                        {
                            persona = _sigper.GetUserByEmail(obj.To);
                            if (persona == null)
                                throw new Exception("No se encontró el usuario en SIGPER.");

                            workflow.Email = persona.Funcionario.Rh_Mail.Trim();
                            workflow.NombreFuncionario = persona.Funcionario.PeDatPerChq.Trim();
                            workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                            workflow.Pl_UndDes = persona.Unidad.Pl_UndDes;
                            workflow.TareaPersonal = true;
                        }
                    }

                    if (definicionWorkflow.TipoEjecucionId == (int)App.Util.Enum.TipoEjecucion.EjecutaDestinoInicial)
                    {
                        var workflowInicial = _repository.Get<Workflow>(q => q.ProcesoId == workflowActual.ProcesoId && (q.To != null || q.Pl_UndCod != null) && q.WorkflowId != workflowActual.WorkflowId).OrderByDescending(q => q.WorkflowId).FirstOrDefault();
                        if (workflowInicial == null)
                            throw new Exception("No se encontró el workflow inicial.");

                        if (workflowInicial.Pl_UndCod.HasValue)
                        {
                            var unidad = _sigper.GetUnidad(workflowInicial.Pl_UndCod.Value);
                            if (unidad == null)
                                throw new Exception("No se encontró la unidad en SIGPER.");

                            workflow.Pl_UndCod = unidad.Pl_UndCod;
                            workflow.Pl_UndDes = unidad.Pl_UndDes;
                            workflow.TareaPersonal = false;

                            var emails = _sigper.GetUserByUnidad(workflow.Pl_UndCod.Value).Select(q => q.Rh_Mail.Trim());
                            if (emails.Any())
                                workflow.Email = string.Join(";", emails);
                        }

                        if (!string.IsNullOrEmpty(workflowInicial.To))
                        {
                            persona = _sigper.GetUserByEmail(workflowInicial.To);
                            if (persona == null)
                                throw new Exception("No se encontró el usuario en SIGPER.");

                            workflow.Email = persona.Funcionario.Rh_Mail.Trim();
                            workflow.NombreFuncionario = persona.Funcionario.PeDatPerChq.Trim();
                            workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                            workflow.Pl_UndDes = persona.Unidad.Pl_UndDes;
                            workflow.TareaPersonal = true;
                        }
                    }

                    if (definicionWorkflow.TipoEjecucionId == (int)App.Util.Enum.TipoEjecucion.EjecutaQuienIniciaElProceso)
                    {
                        persona = _sigper.GetUserByEmail(workflowActual.Proceso.Email);
                        if (persona == null)
                            throw new Exception("No se encontró el usuario en SIGPER.");
                        workflow.Email = persona.Funcionario.Rh_Mail.Trim();
                        workflow.NombreFuncionario = persona.Funcionario.PeDatPerChq.Trim();
                        workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                        workflow.Pl_UndDes = persona.Unidad.Pl_UndDes;
                        workflow.TareaPersonal = true;
                    }

                    if (definicionWorkflow.TipoEjecucionId == (int)App.Util.Enum.TipoEjecucion.EjecutaPorJefaturaDeQuienIniciaProceso)
                    {
                        persona = _sigper.GetUserByEmail(workflowActual.Proceso.Email);
                        if (persona == null)
                            throw new Exception("No se encontró el usuario en SIGPER.");

                        var jefatura = _sigper.GetUserByEmail(persona.Jefatura.Rh_Mail.Trim());
                        if (jefatura == null)
                            throw new Exception("No se encontró la jefatura en SIGPER.");
                        workflow.Email = jefatura.Funcionario.Rh_Mail.Trim();
                        workflow.NombreFuncionario = jefatura.Funcionario.PeDatPerChq.Trim();
                        workflow.Pl_UndCod = jefatura.Unidad.Pl_UndCod;
                        workflow.Pl_UndDes = jefatura.Unidad.Pl_UndDes;

                        workflow.TareaPersonal = true;
                    }

                    if (definicionWorkflow.TipoEjecucionId == (int)App.Util.Enum.TipoEjecucion.EjecutaPorJefaturaDeQuienEjecutoTareaAnterior)
                    {
                        persona = _sigper.GetUserByEmail(obj.Email);
                        if (persona == null)
                            throw new Exception("No se encontró el usuario en SIGPER.");
                        workflow.Email = persona.Jefatura.Rh_Mail.Trim();
                        workflow.NombreFuncionario = persona.Funcionario.PeDatPerChq.Trim();
                        workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                        workflow.Pl_UndDes = persona.Unidad.Pl_UndDes;
                        workflow.TareaPersonal = true;
                    }

                    if (definicionWorkflow.TipoEjecucionId == (int)App.Util.Enum.TipoEjecucion.EjecutaGrupoEspecifico)
                    {
                        if (!definicionWorkflow.Pl_UndCod.HasValue && !definicionWorkflow.GrupoId.HasValue)
                            throw new Exception("No se especificó la unidad o grupo de destino.");

                        if (definicionWorkflow.Pl_UndCod.HasValue)
                        {
                            var unidad = _sigper.GetUnidad(definicionWorkflow.Pl_UndCod.Value);
                            if (unidad == null)
                                throw new Exception("No se encontró la unidad destino en SIGPER.");
                            workflow.Pl_UndCod = definicionWorkflow.Pl_UndCod;
                            workflow.Pl_UndDes = definicionWorkflow.Pl_UndDes;
                            workflow.TareaPersonal = false;
                            var emails = _sigper.GetUserByUnidad(workflow.Pl_UndCod.Value).Select(q => q.Rh_Mail.Trim());
                            if (emails.Any())
                                workflow.Email = string.Join(";", emails);
                        }
                        if (definicionWorkflow.GrupoId.HasValue)
                        {
                            var grupo = _repository.GetById<Grupo>(definicionWorkflow.GrupoId.Value);
                            if (grupo == null)
                                throw new Exception("No se encontró el grupo de destino.");
                            workflow.GrupoId = definicionWorkflow.GrupoId;
                            workflow.Pl_UndCod = null;
                            workflow.Pl_UndDes = null;
                            workflow.TareaPersonal = false;
                            var emails = grupo.Usuarios.Where(q => q.Habilitado).Select(q => q.Email);
                            if (emails.Any())
                                workflow.Email = string.Join(";", emails);
                        }

                    }

                    if (definicionWorkflow.TipoEjecucionId == (int)App.Util.Enum.TipoEjecucion.EjecutaUsuarioEspecifico)
                    {
                        persona = _sigper.GetUserByEmail(definicionWorkflow.Email);
                        if (persona == null)
                            throw new Exception("No se encontró el usuario en SIGPER.");

                        workflow.Pl_UndCod = persona.Unidad.Pl_UndCod;
                        workflow.Pl_UndDes = persona.Unidad.Pl_UndDes.Trim();
                        workflow.Email = persona.Funcionario.Rh_Mail.Trim();
                        workflow.NombreFuncionario = persona.Funcionario.PeDatPerChq.Trim();
                        workflow.TareaPersonal = true;
                    }

                    //guardar información
                    _repository.Create(workflow);
                    _repository.Save();

                    //notificar por email al ejecutor de proxima tarea
                    //se adjunta copia al autor
                    if (workflow.DefinicionWorkflow.NotificarAsignacion)
                        _email.NotificarNuevoWorkflow(workflow,
                        _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.PlantillaNuevaTarea),
                        _repository.GetById<Configuracion>((int)App.Util.Enum.Configuracion.AsuntoCorreoNotificacion));
                }

            }
            catch (Exception ex)
            {
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        public ResponseMessage FixFileMetadata()
        {
            var response = new ResponseMessage();

            var ids = _repository.Get<Documento>(q => string.IsNullOrEmpty(q.Metadata)).Select(q => q.DocumentoId);
            foreach (var id in ids)
            {
                var doc = _repository.GetById<Documento>(id);

                //obtener metadata del documento
                var metadata = _file.BynaryToText(doc.File);
                if (metadata != null)
                {
                    doc.Texto = metadata.Text;
                    doc.Metadata = metadata.Metadata;
                    doc.Type = metadata.Type;
                }

                //actualizar datos
                _repository.Update(doc);
                _repository.Save();
            }

            return response;
        }
    }
}
