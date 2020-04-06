using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.SharePoint;
using System.Configuration;
using System.IO;

namespace FPC_EventosCorrespondenciaSaliente
{
    public class EventosCorrespondenciaSaliente : SPItemEventReceiver
    {
        const string TC_OBJETICO = "Correspondencia Saliente";
        const string PARAMETRO = "CITE";
        const string LISTA_PARAMETROS = "Parámetros Globales";

        const string CORREO_ENTRADA_FUNDAPRO = "Correspondencia de Entrada Funda-Pro";
        const string CORREO_ENTRADA_EDUCAPRO = "Correspondencia de Entrada Educa-Pro";
        const string CORREO_ENTRADA_EDUCAPRO_CB = "Correspondencia de Entrada Educa-Pro (CB)";
        const string CORREO_ENTRADA_EDUCAPRO_SC = "Correspondencia de Entrada Educa-Pro (SC)";

        const string CORREO_SALIDA_FUNDAPRO = "Correspondencia de Salida Funda-Pro";
        const string CORREO_SALIDA_EDUCAPRO = "Correspondencia de Salida Educa-Pro";
        const string CORREO_SALIDA_EDUCAPRO_CB = "Correspondencia de Salida Educa-Pro (CB)";
        const string CORREO_SALIDA_EDUCAPRO_SC = "Correspondencia de Salida Educa-Pro (SC)";

        public override void ItemAdded(SPItemEventProperties properties)
        {
            try
            {
                #region Comprobacion de lista objetivo
                if (!this.EsTCCorrecto(properties))
                    return;
                #endregion

                #region Eventos
                this.EventoGenerarCITE(properties);
                this.EventoAsociarCorrespondenciaEntrante(properties);
                #endregion
            }
            catch (Exception ex)
            {
                properties.Status = SPEventReceiverStatus.Continue;
                properties.ErrorMessage = ex.Message;
            }
        }

        //public override void ItemUpdated(SPItemEventProperties properties)
        //{
        //    try
        //    {
        //        #region Comprobacion de lista objetivo
        //        if (!this.EsTCCorrecto(properties))
        //            return;
        //        #endregion

        //        #region Eventos
        //        this.EventoGenerarCITE(properties);
        //        this.EventoAsociarCorrespondenciaEntrante(properties);
        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        properties.Status = SPEventReceiverStatus.Continue;
        //        properties.ErrorMessage = ex.Message;
        //    }
        //}

        public override void ItemUpdating(SPItemEventProperties properties)
        {
            try
            {
                #region Comprobacion de lista objetivo
                if (!this.EsTCCorrecto(properties))
                    return;
                #endregion

                #region Eventos
                this.EventoActualizarAsociarCorrespondenciaEntrante(properties);
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
                this.EventoGenerarCITE(properties);
                #endregion
            }
            catch (Exception ex)
            {
                properties.Status = SPEventReceiverStatus.Continue;
                properties.ErrorMessage = ex.Message;
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
        /// Genera y asigna el codigo CITE correlativo correspondiente
        /// </summary>
        /// <param name="properties"></param>
        private void EventoGenerarCITE(SPItemEventProperties properties)
        {
            SPListItem itemActual = properties.ListItem;
            SPUser usuarioActual = properties.OpenWeb().SiteUsers[properties.UserLoginName];

            string tipoSalida = itemActual["Tipo salida"].ToString().Trim();

            List<string> arrayParametro =
                this.RecuperarValorParametroGlobal(PARAMETRO, tipoSalida, usuarioActual);

            string valorParametro = arrayParametro[1];

            string sinLadoIzq = valorParametro.Substring(valorParametro.IndexOf('{') + 1);
            int numeroBase = Convert.ToInt16(sinLadoIzq.Remove(sinLadoIzq.IndexOf('}')));
            //string formatoBase = valorParametro.Remove(valorParametro.LastIndexOf('-') + 1);
            int numeroSiguiente = numeroBase + 1;

            #region Actualizar la columna CITE de la lista
            try
            {
                //itemActual["CITE"] = formatoBase + numeroSiguiente.ToString();
                itemActual["CITE"] = valorParametro.Replace("{" + numeroBase + "}",
                    numeroSiguiente.ToString());
            }
            catch
            {
                itemActual["Title"] = valorParametro.Replace("{" + numeroBase + "}",
                    numeroSiguiente.ToString());
            }

            using (DisabledItemEventsScope scope = new DisabledItemEventsScope())
            {
                itemActual.SystemUpdate();
            }
            #endregion

            #region Actualizar el valor del parametro correspondiente
            this.ActualizarParametroCITE(Convert.ToInt16(arrayParametro[0]),
                valorParametro.Replace("{" + numeroBase + "}",
                "{" + numeroSiguiente.ToString() + "}"));
            #endregion
        }

        /// <summary>
        /// Crea una relacion automática hacia la lista Correspondencia de Entrada
        /// </summary>
        /// <param name="properties"></param>
        private void EventoAsociarCorrespondenciaEntrante(SPItemEventProperties properties)
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

                SPListItem itemSalida = properties.ListItem;
                SPListItem itemEntrada;
                SPFieldLookupValueCollection enlacesSalida =
                    (SPFieldLookupValueCollection)itemSalida["En respuesta a"];
                SPFieldLookupValueCollection enlacesEntrada;

                #region Definir la lista usada
                string listaCorreoUsada = "";
                if (string.Equals(webAdm.Lists[properties.ListId].Title.Trim(),
                    CORREO_SALIDA_FUNDAPRO, StringComparison.CurrentCultureIgnoreCase))
                {
                    listaCorreoUsada = CORREO_ENTRADA_FUNDAPRO;
                }
                else if (string.Equals(webAdm.Lists[properties.ListId].Title.Trim(),
                    CORREO_SALIDA_EDUCAPRO, StringComparison.CurrentCultureIgnoreCase))
                {
                    listaCorreoUsada = CORREO_ENTRADA_EDUCAPRO;
                }
                else if (string.Equals(webAdm.Lists[properties.ListId].Title.Trim(),
                    CORREO_SALIDA_EDUCAPRO_CB, StringComparison.CurrentCultureIgnoreCase))
                {
                    listaCorreoUsada = CORREO_ENTRADA_EDUCAPRO_CB;
                }
                else if (string.Equals(webAdm.Lists[properties.ListId].Title.Trim(),
                    CORREO_SALIDA_EDUCAPRO_SC, StringComparison.CurrentCultureIgnoreCase))
                {
                    listaCorreoUsada = CORREO_ENTRADA_EDUCAPRO_SC;
                }
                #endregion

                #region Crear relacion sobre este elemento
                foreach (SPFieldLookupValue enlaceSalida in enlacesSalida)
                {
                    itemEntrada = webAdm.Lists[listaCorreoUsada].Items.GetItemById(
                        enlaceSalida.LookupId);
                    enlacesEntrada = (SPFieldLookupValueCollection)itemEntrada["Respuesta"];

                    SPFieldLookupValue enlaceEntrada = new SPFieldLookupValue(itemSalida.ID,
                        itemSalida["CITE"].ToString());

                    if (!enlacesEntrada.Contains(enlaceEntrada))
                        enlacesEntrada.Add(enlaceEntrada);

                    itemEntrada["Respuesta"] = enlacesEntrada;

                    using (DisabledItemEventsScope scope = new DisabledItemEventsScope())
                    {
                        itemEntrada.SystemUpdate();
                    }
                }
                #endregion
            }
            finally
            {
                if (webAdm != null) webAdm.Dispose();
                if (sitioAdm != null) sitioAdm.Dispose();
            }
        }

        /// <summary>
        /// Actualiza la relacion automática hacia la lista Correspondencia de Entrada
        /// </summary>
        /// <param name="properties"></param>
        private void EventoActualizarAsociarCorrespondenciaEntrante(SPItemEventProperties properties)
        {
            object enRespuestaAAntes = properties.ListItem["En_x0020_respuesta_x0020_a"];
            object enRespuestaADespues = properties.AfterProperties["En_x0020_respuesta_x0020_a"];

            if (((SPFieldLookupValueCollection)enRespuestaAAntes).Count == 0 &&
                string.IsNullOrEmpty(enRespuestaADespues.ToString())) //Si el campo "En respuesta a" esta vacio
                return;

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

                SPFieldLookupValueCollection enlacesSalidaAntes =
                    (SPFieldLookupValueCollection)enRespuestaAAntes;
                SPFieldLookupValueCollection enlacesSalidaDespues =
                    new SPFieldLookupValueCollection(enRespuestaADespues.ToString());

                SPListItem itemEntrada;
                SPFieldLookupValueCollection enlacesEntrada;
                SPFieldLookupValueCollection enlacesEntrada2;

                #region Definir la lista usada
                string listaCorreoUsada = "";
                if (string.Equals(webAdm.Lists[properties.ListId].Title.Trim(),
                    CORREO_SALIDA_FUNDAPRO, StringComparison.CurrentCultureIgnoreCase))
                {
                    listaCorreoUsada = CORREO_ENTRADA_FUNDAPRO;
                }
                else if (string.Equals(webAdm.Lists[properties.ListId].Title.Trim(),
                    CORREO_SALIDA_EDUCAPRO, StringComparison.CurrentCultureIgnoreCase))
                {
                    listaCorreoUsada = CORREO_ENTRADA_EDUCAPRO;
                }
                else if (string.Equals(webAdm.Lists[properties.ListId].Title.Trim(),
                    CORREO_SALIDA_EDUCAPRO_CB, StringComparison.CurrentCultureIgnoreCase))
                {
                    listaCorreoUsada = CORREO_ENTRADA_EDUCAPRO_CB;
                }
                else if (string.Equals(webAdm.Lists[properties.ListId].Title.Trim(),
                    CORREO_SALIDA_EDUCAPRO_SC, StringComparison.CurrentCultureIgnoreCase))
                {
                    listaCorreoUsada = CORREO_ENTRADA_EDUCAPRO_SC;
                }
                #endregion

                #region Eliminar relacion existente sobre este elemento
                foreach (SPFieldLookupValue enlaceSalidaAntes in enlacesSalidaAntes)
                {
                    itemEntrada = webAdm.Lists[listaCorreoUsada].Items.GetItemById(
                        enlaceSalidaAntes.LookupId);
                    enlacesEntrada = (SPFieldLookupValueCollection)itemEntrada["Respuesta"];
                    enlacesEntrada2 = (SPFieldLookupValueCollection)itemEntrada["Respuesta"];

                    for (int i = 0; i < enlacesEntrada.Count; i++)
                    {
                        if (enlacesEntrada[i].LookupId == properties.ListItemId)
                        {
                            enlacesEntrada2.RemoveAt(i);
                            break;
                        }
                    }

                    itemEntrada["Respuesta"] = enlacesEntrada2;

                    using (DisabledItemEventsScope scope = new DisabledItemEventsScope())
                    {
                        itemEntrada.SystemUpdate();
                    }
                }
                #endregion

                #region Crear relacion sobre este elemento
                foreach (SPFieldLookupValue enlaceSalidaDespues in enlacesSalidaDespues)
                {
                    itemEntrada = webAdm.Lists[listaCorreoUsada].Items.GetItemById(
                        enlaceSalidaDespues.LookupId);
                    enlacesEntrada = (SPFieldLookupValueCollection)itemEntrada["Respuesta"];
                    SPFieldLookupValue enlaceEntrada = new SPFieldLookupValue(properties.ListItemId,
                        properties.ListItem["CITE"].ToString());

                    if (!enlacesEntrada.Contains(enlaceEntrada))
                        enlacesEntrada.Add(enlaceEntrada);

                    itemEntrada["Respuesta"] = enlacesEntrada;

                    using (DisabledItemEventsScope scope = new DisabledItemEventsScope())
                    {
                        itemEntrada.SystemUpdate();
                    }
                }
                #endregion
            }
            finally
            {
                if (webAdm != null) webAdm.Dispose();
                if (sitioAdm != null) sitioAdm.Dispose();
            }
        }

        /// <summary>
        /// Recupera el valor del parametro segun los valores provistos.
        /// El parametro es retornado como array "ID"|"Valor parametro"
        /// </summary>
        /// <param name="nombreParametro"></param>
        /// <param name="usuarioParametro"></param>
        /// <returns></returns>
        private List<string> RecuperarValorParametroGlobal(string nombreParametro,
            string tipoSalida, SPUser usuarioParametro)
        {
            SPSite sitioAdm = null;
            SPWeb webAdm = null;

            List<string> parametro = new List<string>();

            try
            {
                string UrlFPC = ConfigurationManager.AppSettings["UrlFPC"];

                SPSecurity.RunWithElevatedPrivileges(delegate()
                {//Como usuario administrador
                    sitioAdm = new SPSite(UrlFPC);
                    webAdm = sitioAdm.OpenWeb();
                });

                SPList listaParametros = webAdm.Lists[LISTA_PARAMETROS];

                foreach (SPListItem item in listaParametros.Items)
                {
                    if (item["Usuario parámetro"] != null)
                    {
                        try
                        {
                            int idUsuario = Convert.ToInt16(item["Usuario parámetro"].ToString().Remove(
                                item["Usuario parámetro"].ToString().IndexOf(';')));
                            string[] tituloParametro = item.Title.Split('-');

                            if (string.Equals(tituloParametro[0].Trim(), nombreParametro,
                                StringComparison.CurrentCultureIgnoreCase) &&
                                string.Equals(tituloParametro[1].Trim(), tipoSalida,
                                StringComparison.CurrentCultureIgnoreCase) &&
                                idUsuario == usuarioParametro.ID)
                            {
                                parametro.Add(item.ID.ToString());
                                parametro.Add(item["Valor parámetro"].ToString().Trim());

                                return parametro;
                            }
                        }
                        catch { }
                    }
                }

                return null;
            }
            finally
            {
                if (webAdm != null) webAdm.Dispose();
                if (sitioAdm != null) sitioAdm.Dispose();
            }
        }

        /// <summary>
        /// Actualiza el parametro definido para el usuario actual
        /// </summary>
        /// <param name="idParametro"></param>
        /// <param name="nuevoValorParametro"></param>
        /// <param name="properties"></param>
        private void ActualizarParametroCITE(int idParametro, string nuevoValorParametro)
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

                SPList listaParametros = webAdm.Lists[LISTA_PARAMETROS];
                SPListItem itemParametro = listaParametros.Items.GetItemById(idParametro);

                itemParametro["Valor parámetro"] = nuevoValorParametro;
                itemParametro.Update();
            }
            finally
            {
                if (webAdm != null) webAdm.Dispose();
                if (sitioAdm != null) sitioAdm.Dispose();
            }
        }
    }
}
