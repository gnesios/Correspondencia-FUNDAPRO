using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;
using System.Globalization;
using System.Configuration;

using System.Drawing.Printing;
using System.Drawing;

using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace FPC_EventosCorrespondenciaEntrante
{
    public class EventosCorrespondenciaEntrante : SPItemEventReceiver
    {
        const string CORREO_FUNDAPRO = "Correspondencia de Entrada Funda-Pro";
        const string CORREO_EDUCAPRO = "Correspondencia de Entrada Educa-Pro";
        const string CORREO_EDUCAPRO_CB = "Correspondencia de Entrada Educa-Pro (CB)";
        const string CORREO_EDUCAPRO_SC = "Correspondencia de Entrada Educa-Pro (SC)";

        const string GRUPO_COLABORADORES = "Colaboradores Gestión de Correspondencia";
        const string GRUPO_COLABORADORES_CB = "Colaboradores Gestión de Correspondencia (CB)";
        const string GRUPO_COLABORADORES_SC = "Colaboradores Gestión de Correspondencia (SC)";
        //const string GRUPO_COLABORADORES_EP = "Visitantes Correspondencia Educa-Pro";

        const string GRUPO_INTEGRANTES_FP = "Integrantes Correspondencia Funda-Pro";
        const string GRUPO_INTEGRANTES_EP = "Integrantes Correspondencia Educa-Pro";
        const string GRUPO_INTEGRANTES_EP_CB = "Integrantes Correspondencia Educa-Pro (CB)";
        const string GRUPO_INTEGRANTES_EP_SC = "Integrantes Correspondencia Educa-Pro (SC)";

        const string TC_OBJETICO = "Correspondencia Entrante";
        //const string ESTADO = "PASIVA";

        public override void ItemAdded(SPItemEventProperties properties)
        {
            try
            {
                #region Comprobacion de lista objetivo
                if (!this.EsTCCorrecto(properties))
                    return;
                #endregion

                #region Eventos
                this.EventoLlenarColumnasOcultasAdded(properties);
                this.EventoHacerCorrespondenciaPrivada(properties);
                //this.EventoImprimirHojaDeRuta(properties);
                #endregion
            }
            catch (Exception ex)
            {
                properties.Status = SPEventReceiverStatus.CancelWithError;
                properties.ErrorMessage = ex.Message;
                properties.Cancel = true;
            }
        }

        public override void ItemUpdating(SPItemEventProperties properties)
        {
            try
            {
                #region Comprobacion de lista objetivo
                if (!this.EsTCCorrecto(properties))
                    return;
                #endregion

                #region Eventos
                this.EventoRestringirEdicionDeCampos(properties);
                #endregion
            }
            catch (Exception ex)
            {
                properties.Status = SPEventReceiverStatus.CancelWithError;
                properties.ErrorMessage = ex.Message;
                properties.Cancel = true;
            }
        }

        public override void ItemUpdated(SPItemEventProperties properties)
        {
            try
            {
                #region Comprobacion de lista objetivo
                if (!this.EsTCCorrecto(properties))
                    return;
                #endregion

                #region Eventos
                this.EventoLlenarColumnasOcultasUpdated(properties);
                this.EventoHacerCorrespondenciaPrivada(properties);
                this.EventoCambiarEstadoCorrespondencia(properties);
                this.EventoEntregarCorrespondencia(properties);
                #endregion
            }
            catch (Exception ex)
            {
                properties.Status = SPEventReceiverStatus.CancelWithError;
                properties.ErrorMessage = ex.Message;
                properties.Cancel = true;
            }
        }

        /// <summary>
        /// Valida si el tipo de contenido es el correcto
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        private bool EsTCCorrecto(SPItemEventProperties properties)
        {
            bool band = false;
            SPContentTypeCollection contentTypes =
                properties.OpenWeb().Lists[properties.ListId].ContentTypes;

            foreach (SPContentType contentType in contentTypes)
            {
                if (string.Equals(contentType.Name, TC_OBJETICO,
                    StringComparison.CurrentCultureIgnoreCase))
                {
                    band = true;
                    break;
                }
            }

            return band;
        }

        /// <summary>
        /// Llena las columnas "Título" y "Id_NumCarta"
        /// </summary>
        /// <param name="properties"></param>
        private void EventoLlenarColumnasOcultasAdded(SPItemEventProperties properties)
        {
            SPListItem listItem = properties.ListItem;
            SPFieldUserValueCollection usuarios = 
                (SPFieldUserValueCollection)listItem["Dirigida a"];

            string referenciaFormateada = listItem["Referencia"].ToString().Trim();
            if (referenciaFormateada.Length > 20)
                referenciaFormateada = referenciaFormateada.Substring(0, 19);

            try
            {
                DisableEventFiring();
                listItem["Title"] = string.Format("{0} - {1}",
                    listItem["Origen"].ToString().Substring(listItem["Origen"].ToString().IndexOf('#') + 1),
                    referenciaFormateada);
                listItem["Id_NumCarta"] = string.Format("{0} ({1})",
                    listItem.ID, listItem["Num. ó Cite"]);
                listItem["Recibida."] = "<strong style=\"color:#FF0000\">NO</strong>";

                string entregadas = "";
                foreach (SPFieldUserValue usuario in usuarios)
                {
                    entregadas = entregadas + string.Format("<strong style=\"color:#FF0000\">No entregada</strong> " +
                        "a {0}; ", usuario.User.Name);
                }
                listItem["Entregada"] = entregadas;

                //Copiar el valor del campo Dirigida a
                //listItem["DAC"] = listItem["Dirigida a"];

                listItem.SystemUpdate();
            }
            finally
            {
                EnableEventFiring();
            }
        }

        /// <summary>
        /// Llena las columnas "Título" y "Id_NumCarta"
        /// </summary>
        /// <param name="properties"></param>
        private void EventoLlenarColumnasOcultasUpdated(SPItemEventProperties properties)
        {
            SPListItem listItem = properties.ListItem;
            SPFieldUserValueCollection usuarios =
                (SPFieldUserValueCollection)listItem["Dirigida a"];
            string creadoPor = properties.ListItem["Creado por"].ToString().Remove(
                properties.ListItem["Creado por"].ToString().IndexOf(';'));

            try
            {
                DisableEventFiring();
                if (properties.CurrentUserId == Convert.ToInt32(creadoPor))
                {
                    string referenciaFormateada = listItem["Referencia"].ToString().Trim();
                    if (referenciaFormateada.Length > 20)
                        referenciaFormateada = referenciaFormateada.Substring(0, 19);

                    listItem["Title"] = string.Format("{0} - {1}",
                        listItem["Origen"].ToString().Substring(listItem["Origen"].ToString().IndexOf('#') + 1),
                        referenciaFormateada);
                    listItem["Id_NumCarta"] = string.Format("{0} ({1})",
                        listItem.ID, listItem["Num. ó Cite"]);
                }

                #region Campo Recibida
                if (listItem["Recibida"] != null)
                {
                    string textoRecibida =
                        listItem["Recibida"].ToString().Replace("<div>", "").Replace("</div>", "");

                    if (!string.IsNullOrEmpty(textoRecibida))
                        listItem["Recibida."] = "<strong style=\"color:#008000\">SI</strong>";
                }
                #endregion

                #region Campo Entregada
                string entregadas = listItem["Entregada"].ToString();
                foreach (SPFieldUserValue usuario in usuarios)
                {
                    if (!entregadas.Contains(usuario.User.Name))
                    {
                        entregadas = entregadas + string.Format(
                            "<strong style=\"color:#FF0000\">No entregada</strong> " +
                            "a {0}; ", usuario.User.Name);

                        listItem["Entregada"] = entregadas;
                    }
                }
                #endregion

                listItem.SystemUpdate();
            }
            finally
            {
                EnableEventFiring();
            }
        }

        /// <summary>
        /// Ejecuta acciones en caso de marcar la correspondencia como "Privada"
        /// </summary>
        /// <param name="properties"></param>
        private void EventoHacerCorrespondenciaPrivada(SPItemEventProperties properties)
        {
            SPSite sitioAdm = null;
            SPWeb webAdm = null;

            try
            {
                string UrlFPC = ConfigurationManager.AppSettings["UrlFPC"];

                SPSecurity.RunWithElevatedPrivileges(delegate()
                {//Como usuario administrador
                    sitioAdm = new SPSite(UrlFPC);
                    webAdm = sitioAdm.OpenWeb();
                });

                bool esPrivada = (bool)properties.ListItem["Privada"];
                SPList lista = webAdm.Lists[properties.ListId];
                SPListItem listItem = lista.GetItemById(properties.ListItemId);

                if (!listItem.HasUniqueRoleAssignments)
                {
                    listItem.BreakRoleInheritance(true);

                    #region Eliminar grupos
                    if (esPrivada)
                    {
                        try
                        {
                            if (webAdm.Lists[properties.ListId].Title.Contains(CORREO_EDUCAPRO_SC))
                            {
                                listItem.RoleAssignments.RemoveById(webAdm.Groups[GRUPO_COLABORADORES_SC].ID);
                                listItem.RoleAssignments.RemoveById(webAdm.Groups[GRUPO_INTEGRANTES_EP_SC].ID);
                            }
                            else if (webAdm.Lists[properties.ListId].Title.Contains(CORREO_EDUCAPRO_CB))
                            {
                                listItem.RoleAssignments.RemoveById(webAdm.Groups[GRUPO_COLABORADORES_CB].ID);
                                listItem.RoleAssignments.RemoveById(webAdm.Groups[GRUPO_INTEGRANTES_EP_CB].ID);
                            }
                            else if (webAdm.Lists[properties.ListId].Title.Contains(CORREO_EDUCAPRO))
                            {
                                listItem.RoleAssignments.RemoveById(webAdm.Groups[GRUPO_COLABORADORES].ID);
                                listItem.RoleAssignments.RemoveById(webAdm.Groups[GRUPO_INTEGRANTES_EP].ID);
                            }
                            else if (webAdm.Lists[properties.ListId].Title.Contains(CORREO_FUNDAPRO))
                            {
                                listItem.RoleAssignments.RemoveById(webAdm.Groups[GRUPO_COLABORADORES].ID);
                                listItem.RoleAssignments.RemoveById(webAdm.Groups[GRUPO_INTEGRANTES_FP].ID);
                            }

                            //if (string.Equals(webAdm.Lists[properties.ListId].Title.Trim(),
                            //    CORREO_FUNDAPRO, StringComparison.CurrentCultureIgnoreCase))
                            //{
                            //    listItem.RoleAssignments.RemoveById(webAdm.Groups[GRUPO_COLABORADORES].ID);
                            //    listItem.RoleAssignments.RemoveById(webAdm.Groups[GRUPO_INTEGRANTES_FP].ID);
                            //}
                            //else if (string.Equals(webAdm.Lists[properties.ListId].Title.Trim(),
                            //    CORREO_EDUCAPRO, StringComparison.CurrentCultureIgnoreCase))
                            //{
                            //    listItem.RoleAssignments.RemoveById(webAdm.Groups[GRUPO_COLABORADORES].ID);
                            //    listItem.RoleAssignments.RemoveById(webAdm.Groups[GRUPO_INTEGRANTES_EP].ID);
                            //}
                            //else if (string.Equals(webAdm.Lists[properties.ListId].Title.Trim(),
                            //    CORREO_EDUCAPRO_CB, StringComparison.CurrentCultureIgnoreCase))
                            //{
                            //    listItem.RoleAssignments.RemoveById(webAdm.Groups[GRUPO_COLABORADORES_CB].ID);
                            //    listItem.RoleAssignments.RemoveById(webAdm.Groups[GRUPO_INTEGRANTES_EP_CB].ID);
                            //}
                            //else if (string.Equals(webAdm.Lists[properties.ListId].Title.Trim(),
                            //    CORREO_EDUCAPRO_SC, StringComparison.CurrentCultureIgnoreCase))
                            //{
                            //    listItem.RoleAssignments.RemoveById(webAdm.Groups[GRUPO_COLABORADORES_SC].ID);
                            //    listItem.RoleAssignments.RemoveById(webAdm.Groups[GRUPO_INTEGRANTES_EP_SC].ID);
                            //}
                        }
                        catch { }
                    }
                    #endregion
                }

                string idEditor = listItem["Editor"].ToString().Remove(
                    listItem["Editor"].ToString().IndexOf(';'));
                SPFieldUserValueCollection usuarios =
                    (SPFieldUserValueCollection)properties.ListItem["Dirigida a"];

                if (Convert.ToInt32(idEditor) != usuarios[0].User.ID)
                {
                    #region Agregar usuario emisor
                    listItem.RoleAssignments.RemoveById(Convert.ToInt32(idEditor));

                    SPRoleDefinitionCollection roleDefinitionsColab = webAdm.RoleDefinitions;
                    SPRoleAssignmentCollection roleAssignmentsColab = webAdm.RoleAssignments;
                    SPRoleAssignment roleAssignmentColab = new SPRoleAssignment(
                        webAdm.SiteUsers.GetByID(Convert.ToInt32(idEditor)).LoginName, "", "", "");

                    SPRoleDefinitionBindingCollection roleDefinitionBindingsColab =
                        roleAssignmentColab.RoleDefinitionBindings;
                    roleDefinitionBindingsColab.Add(roleDefinitionsColab["Leer"]);
                    roleAssignmentsColab.Add(roleAssignmentColab);

                    listItem.RoleAssignments.Add(roleAssignmentColab);
                    #endregion

                    #region Agregar usuarios receptores
                    for (int i = 0; i < usuarios.Count; i++)
                    {
                        SPFieldUserValue usuario = usuarios[i];

                        SPRoleDefinitionCollection roleDefinitionsLect = webAdm.RoleDefinitions;
                        SPRoleAssignmentCollection roleAssignmentsLect = webAdm.RoleAssignments;
                        SPRoleAssignment roleAssignmentLect =
                            new SPRoleAssignment(usuario.User.LoginName, "", "", "");

                        SPRoleDefinitionBindingCollection roleDefinitionBindingsLect =
                            roleAssignmentLect.RoleDefinitionBindings;
                        if (i == 0)
                            roleDefinitionBindingsLect.Add(roleDefinitionsLect["Colaborar Res."]);
                        else
                            roleDefinitionBindingsLect.Add(roleDefinitionsLect["Leer"]);
                        roleAssignmentsLect.Add(roleAssignmentLect);

                        listItem.RoleAssignments.Add(roleAssignmentLect);
                    }
                    #endregion
                }
            }
            finally
            {
                if (webAdm != null) webAdm.Dispose();
                if (sitioAdm != null) sitioAdm.Dispose();
            }
        }

        /// <summary>
        /// Restringe la edicion de campos particulares de un nuevo registro de correspondencia
        /// </summary>
        /// <param name="properties"></param>
        private void EventoRestringirEdicionDeCampos(SPItemEventProperties properties)
        {
            string creadoPor = properties.ListItem["Creado por"].ToString().Remove(
                properties.ListItem["Creado por"].ToString().IndexOf(';'));

            if (properties.UserLoginName != "SHAREPOINT\\system")
            {
                if (properties.CurrentUserId != Convert.ToInt32(creadoPor))
                {
                    if (properties.ListItem["Estado corr."].ToString() != "PASIVA")
                    {
                        string tipoCartaAntes = properties.ListItem["Tipo_x0020_carta"].ToString();
                        string origenCartaAntes = properties.ListItem["Origen"].ToString().Remove(
                            properties.ListItem["Origen"].ToString().IndexOf(';'));
                        string referenciaAntes = properties.ListItem["Referencia"].ToString();
                        DateTime fechaCartaAntes = Convert.ToDateTime(properties.ListItem["Fecha_x0020_carta"]);
                        DateTime fechaRecibidaAntes = Convert.ToDateTime(properties.ListItem["Fecha_x0020_recibida"]);
                        string destinatarioAntes = properties.ListItem["Destinatario"].ToString();
                        string numCartaAntes;
                        if (properties.ListItem["Num_x002e__x0020_carta"] != null)
                            numCartaAntes = properties.ListItem["Num_x002e__x0020_carta"].ToString();
                        else
                            numCartaAntes = "";
                        string adjuntoAntes = properties.ListItem["Adjunto"].ToString();
                        string claseAntes = properties.ListItem["Clase_x0020_de_x0020_documento"].ToString();
                        string prioridadAntes = properties.ListItem["Prioridad_x0020_corr_x002e_"].ToString();
                        string privadaAntes = properties.ListItem["Privada"].ToString();
                        string hojaRutaAntes = properties.ListItem["Hoja_x0020_de_x0020_ruta"].ToString();
                        //string archivoAntes;
                        //if (properties.ListItem["Archivo"] != null)
                        //{
                        //    if (string.IsNullOrEmpty(properties.ListItem["Archivo"].ToString()) ||
                        //        properties.ListItem["Archivo"].ToString().Equals("<P>&nbsp;</P>", StringComparison.CurrentCultureIgnoreCase) ||
                        //        properties.ListItem["Archivo"].ToString().Equals("<DIV>&nbsp;</DIV>", StringComparison.CurrentCultureIgnoreCase))
                        //        archivoAntes = "<DIV></DIV>";
                        //    else
                        //        archivoAntes = properties.ListItem["Archivo"].ToString();
                        //}
                        //else
                        //    archivoAntes = "<DIV></DIV>";

                        string tipoCartaDespues = properties.AfterProperties["Tipo_x0020_carta"].ToString();
                        string origenCartaDespues = properties.AfterProperties["Origen"].ToString();
                        string referenciaDespues = properties.AfterProperties["Referencia"].ToString();
                        DateTime fechaCartaDespues = Convert.ToDateTime(properties.AfterProperties["Fecha_x0020_carta"]).ToUniversalTime();
                        DateTime fechaRecibidaDespues = Convert.ToDateTime(properties.AfterProperties["Fecha_x0020_recibida"]).ToUniversalTime();
                        string destinatarioDespues = properties.AfterProperties["Destinatario"].ToString();
                        string numCartaDespues = properties.AfterProperties["Num_x002e__x0020_carta"].ToString();
                        string adjuntoDespues = properties.AfterProperties["Adjunto"].ToString();
                        string claseDespues = properties.AfterProperties["Clase_x0020_de_x0020_documento"].ToString();
                        string prioridadDespues = properties.AfterProperties["Prioridad_x0020_corr_x002e_"].ToString();
                        string privadaDespues = properties.AfterProperties["Privada"].ToString();
                        string hojaRutaDespues = properties.AfterProperties["Hoja_x0020_de_x0020_ruta"].ToString();
                        //string archivoDespues = properties.AfterProperties["Archivo"].ToString();
                        //if (archivoDespues.Equals("<P>&nbsp;</P>", StringComparison.CurrentCultureIgnoreCase) ||
                        //    archivoDespues.Equals("<DIV>&nbsp;</DIV>", StringComparison.CurrentCultureIgnoreCase))
                        //    archivoDespues = "<DIV></DIV>";

                        if (tipoCartaAntes != tipoCartaDespues)
                        {
                            properties.ErrorMessage = "Usted no tiene permitido modificar el campo " +
                                "<b>Tipo corr.</b>, el cual tiene el valor de \"" + tipoCartaAntes + "\" " +
                                "que intenta ser remplazado por el valor \"" + tipoCartaDespues + "\". " +
                                "Por favor presione el boton 'Atras' de su explorador y vuelva a intentarlo.";
                            properties.Cancel = true;
                        }
                        else if (origenCartaAntes != origenCartaDespues)
                        {
                            properties.ErrorMessage = "Usted no tiene permitido modificar el campo " +
                                "<b>Origen</b>, el cual tiene el valor de \"" + origenCartaAntes + "\" " +
                                "que intenta ser remplazado por el valor \"" + origenCartaDespues + "\". " +
                                "Por favor presione el boton 'Atras' de su explorador y vuelva a intentarlo.";
                            properties.Cancel = true;
                        }
                        else if (referenciaAntes != referenciaDespues)
                        {
                            properties.ErrorMessage = "Usted no tiene permitido modificar el campo " +
                                "<b>Referencia</b>, el cual tiene el valor de \"" + referenciaAntes + "\" " +
                                "que intenta ser remplazado por el valor \"" + referenciaDespues + "\". " +
                                "Por favor presione el boton 'Atras' de su explorador y vuelva a intentarlo.";
                            properties.Cancel = true;
                        }
                        else if (fechaCartaAntes != fechaCartaDespues)
                        {
                            properties.ErrorMessage = "Usted no tiene permitido modificar el campo " +
                                "<b>Fecha origen</b>, el cual tiene el valor de \"" + fechaCartaAntes + "\" " +
                                "que intenta ser remplazado por el valor \"" + fechaCartaDespues + "\". " +
                                "Por favor presione el boton 'Atras' de su explorador y vuelva a intentarlo.";
                            properties.Cancel = true;
                        }
                        else if (fechaRecibidaAntes != fechaRecibidaDespues)
                        {
                            properties.ErrorMessage = "Usted no tiene permitido modificar el campo " +
                                "<b>Fecha recibida</b>, el cual tiene el valor de \"" + fechaRecibidaAntes + "\" " +
                                "que intenta ser remplazado por el valor \"" + fechaRecibidaDespues + "\". " +
                                "Por favor presione el boton 'Atras' de su explorador y vuelva a intentarlo.";
                            properties.Cancel = true;
                        }
                        else if (destinatarioAntes != destinatarioDespues)
                        {
                            properties.ErrorMessage = "Usted no tiene permitido modificar el campo " +
                                "<b>Destinatario</b>, el cual tiene el valor de \"" + destinatarioAntes + "\" " +
                                "que intenta ser remplazado por el valor \"" + destinatarioDespues + "\". " +
                                "Por favor presione el boton 'Atras' de su explorador y vuelva a intentarlo.";
                            properties.Cancel = true;
                        }
                        else if (numCartaAntes != numCartaDespues)
                        {
                            properties.ErrorMessage = "Usted no tiene permitido modificar el campo " +
                                "<b>Num. ó Cite</b>, el cual tiene el valor de \"" + numCartaAntes + "\" " +
                                "que intenta ser remplazado por el valor \"" + numCartaDespues + "\". " +
                                "Por favor presione el boton 'Atras' de su explorador y vuelva a intentarlo.";
                            properties.Cancel = true;
                        }
                        else if (adjuntoAntes != adjuntoDespues)
                        {
                            properties.ErrorMessage = "Usted no tiene permitido modificar el campo " +
                                "<b>Adjunto</b>, el cual tiene el valor de \"" + adjuntoAntes + "\" " +
                                "que intenta ser remplazado por el valor \"" + adjuntoDespues + "\". " +
                                "Por favor presione el boton 'Atras' de su explorador y vuelva a intentarlo.";
                            properties.Cancel = true;
                        }
                        else if (claseAntes != claseDespues)
                        {
                            properties.ErrorMessage = "Usted no tiene permitido modificar el campo " +
                                "<b>Clase de documento</b>, el cual tiene el valor de \"" + claseAntes + "\" " +
                                "que intenta ser remplazado por el valor \"" + claseDespues + "\". " +
                                "Por favor presione el boton 'Atras' de su explorador y vuelva a intentarlo.";
                            properties.Cancel = true;
                        }
                        else if (prioridadAntes != prioridadDespues)
                        {
                            properties.ErrorMessage = "Usted no tiene permitido modificar el campo " +
                                "<b>Prioridad corr.</b>, el cual tiene el valor de \"" + prioridadAntes + "\" " +
                                "que intenta ser remplazado por el valor \"" + prioridadDespues + "\". " +
                                "Por favor presione el boton 'Atras' de su explorador y vuelva a intentarlo.";
                            properties.Cancel = true;
                        }
                        else if (privadaAntes != privadaDespues)
                        {
                            properties.ErrorMessage = "Usted no tiene permitido modificar el campo " +
                                "<b>Privada</b>, el cual tiene el valor de \"" + privadaAntes + "\" " +
                                "que intenta ser remplazado por el valor \"" + privadaDespues + "\". " +
                                "Por favor presione el boton 'Atras' de su explorador y vuelva a intentarlo.";
                            properties.Cancel = true;
                        }
                        else if (hojaRutaAntes != hojaRutaDespues)
                        {
                            properties.ErrorMessage = "Usted no tiene permitido modificar el campo " +
                                "<b>Hoja de ruta</b>, el cual tiene el valor de \"" + hojaRutaAntes + "\" " +
                                "que intenta ser remplazado por el valor \"" + hojaRutaDespues + "\". " +
                                "Por favor presione el boton 'Atras' de su explorador y vuelva a intentarlo.";
                            properties.Cancel = true;
                        }
                        //else if (!archivoAntes.Equals(archivoDespues, StringComparison.CurrentCultureIgnoreCase))
                        //{
                        //    properties.ErrorMessage = "Usted no tiene permitido modificar el campo " +
                        //        "<b>Archivo</b>, el cual tiene el valor de \"" + archivoAntes + "\" " +
                        //        "que intenta ser remplazado por el valor \"" + archivoDespues + "\". " +
                        //        "Por favor presione el boton 'Atras' de su explorador y vuelva a intentarlo.";
                        //    properties.Cancel = true;
                        //}
                    }
                    else
                    {
                        properties.Status = SPEventReceiverStatus.CancelWithError;
                        properties.ErrorMessage = "Usted no tiene permitido modificar los campos de un " +
                            "registro de correspondencia que se encuentra en estado <b>PASIVA</b>.";
                        properties.Cancel = true;
                    }
                }
            }
        }

        /// <summary>
        /// Cambia el estado de la correspondencia de ACTIVA a PASIVA
        /// </summary>
        /// <param name="properties"></param>
        private void EventoCambiarEstadoCorrespondencia(SPItemEventProperties properties)
        {
            SPListItem listItem = properties.ListItem;
            string textoArchivo = null;
            if (listItem["Archivo"] != null)
                textoArchivo = listItem["Archivo"].ToString();

            try
            {
                DisableEventFiring();

                //if (!string.IsNullOrEmpty(textoArchivo) &&
                //    !(textoArchivo.Equals("<DIV></DIV>", StringComparison.CurrentCultureIgnoreCase) ||
                //    textoArchivo.Equals("<P>&nbsp;</P>", StringComparison.CurrentCultureIgnoreCase) ||
                //    textoArchivo.Equals("<DIV>&nbsp;</DIV>", StringComparison.CurrentCultureIgnoreCase)))
                if (!string.IsNullOrEmpty(textoArchivo))
                    listItem["Estado corr."] = "PASIVA";
                else
                    listItem["Estado corr."] = "ACTIVA";

                listItem.SystemUpdate();
            }
            finally
            {
                EnableEventFiring();
            }
        }

        /// <summary>
        /// Cambia el valor de la columna "Entregada"
        /// </summary>
        /// <param name="properties"></param>
        private void EventoEntregarCorrespondencia(SPItemEventProperties properties)
        {
            SPListItem listItem = properties.ListItem;

            string entregadas = "";
            if (listItem["Entregada"] != null)
                entregadas = listItem["Entregada"].ToString();

            try
            {
                DisableEventFiring();

                if (entregadas.Contains(string.Format(
                    "<strong style=\"color:#FF0000\">No entregada</strong> " +
                    "a {0}; ", properties.UserDisplayName)))
                {
                    entregadas =
                        entregadas.Replace(string.Format("<strong style=\"color:#FF0000\">No entregada</strong> " +
                        "a {0}; ", properties.UserDisplayName), string.Format("<strong style=\"color:#008000\">Entregada</strong> a " +
                        "{0} el {1}; ", properties.UserDisplayName, string.Format("{0:d/MM/yyyy HH:mm}", DateTime.Now)));

                    listItem["Entregada"] = entregadas;
                    listItem.SystemUpdate();
                }
            }
            finally
            {
                EnableEventFiring();
            }
        }
    }
}
