using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using System.IO;
using System.Configuration;
using Microsoft.SharePoint;

using System.Drawing.Printing;
using System.Drawing;

using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web;

namespace FPC_WPEntregarCorrespondencia
{
    public static class ConectorWebPart
    {
        const string LISTA_FUNDAPRO = "Correspondencia de Entrada Funda-Pro";
        const string LISTA_EDUCAPRO = "Correspondencia de Entrada Educa-Pro";
        const string LISTA_EDUCAPRO_CB = "Correspondencia de Entrada Educa-Pro (CB)";
        const string LISTA_EDUCAPRO_SC = "Correspondencia de Entrada Educa-Pro (SC)";

        //const string LISTA_REDIRIGIDA_FP = "/Lists/Correspondencia%20de%20Entrada%20FundaPro/DispForm.aspx?ID=";
        //const string LISTA_REDIRIGIDA_EP = "/Lists/Correspondencia%20de%20Entrada%20EducaPro/DispForm.aspx?ID=";
        //const string LISTA_REDIRIGIDA_EP_CB = "/Lists/Correspondencia%20de%20Entrada%20EducaPro%20CB/DispForm.aspx?ID=";
        //const string LISTA_REDIRIGIDA_EP_SC = "/Lists/Correspondencia%20de%20Entrada%20EducaPro%20SC/DispForm.aspx?ID=";

        const string ESTADO_CORRESPONDENCIA = "OBSOLETA";

        static string UrlFPC;

        static SPSite sitioAdm;
        static SPWeb webAdm;

        /// <summary>
        /// Recupera toda la correspondencia perteneciente al usuario actual
        /// </summary>
        /// <returns></returns>
        public static DataSet RecuperarCorrespondenciaFP()
        {
            DataSet dSet = new DataSet("DataSet");
            DataTable dTable = new DataTable("DataTable");
            DataRow dRow;

            try
            {
                UrlFPC = ConfigurationManager.AppSettings["UrlFPC"];

                dTable.Columns.Add(new DataColumn("ID", typeof(Int32)));
                dTable.Columns.Add(new DataColumn("Origen", typeof(string)));
                dTable.Columns.Add(new DataColumn("Referencia", typeof(string)));
                dTable.Columns.Add(new DataColumn("Destinatario", typeof(string)));
                dTable.Columns.Add(new DataColumn("FechaRecibida", typeof(string)));
                //dTable.Columns.Add(new DataColumn("Entregada", typeof(string)));
                dTable.Columns.Add(new DataColumn("Ver", typeof(string)));

                #region Consulta lista SP
                SPSite sitio = null;
                SPWeb web = null;
                try
                {
                    sitio = new SPSite(UrlFPC);
                    web = sitio.OpenWeb();

                    SPSecurity.RunWithElevatedPrivileges(delegate()
                    {//Como usuario administrador
                        sitioAdm = new SPSite(UrlFPC);
                        webAdm = sitioAdm.OpenWeb();
                    });

                    SPList listaCorreo = webAdm.Lists[LISTA_FUNDAPRO];

                    foreach (SPListItem item in listaCorreo.Items)
                    {
                        string entregadas = "";
                        if (item["Entregada"] != null)
                            entregadas = item["Entregada"].ToString();
                        
                        SPFieldUserValueCollection usuarios =
                            (SPFieldUserValueCollection)item["Dirigida a"];
                        
                        try
                        {
                            if (entregadas.Contains(string.Format("<strong style=\"color:#FF0000\">No entregada</strong> " +
                                "a {0}; ", web.CurrentUser.Name)) &&
                                item["Estado corr."].ToString() != ESTADO_CORRESPONDENCIA)
                            {
                                entregadas = entregadas.Replace(string.Format("<strong style=\"color:#FF0000\">No entregada</strong> " +
                                    "a {0}; ", web.CurrentUser.Name), string.Format("<strong style=\"color:#008000\">Entregada</strong> a " +
                                    "{0} el {1}; ", web.CurrentUser.Name, string.Format("{0:d/MM/yyyy HH:mm}", DateTime.Now)));
                                webAdm.AllowUnsafeUpdates = true;
                                item["Entregada"] = entregadas;
                                using (DisabledItemEventsScope scope = new DisabledItemEventsScope())
                                {
                                    item.SystemUpdate();
                                }

                                dRow = dTable.NewRow();

                                dRow[0] = item.ID;
                                dRow[1] = item["Origen"].ToString().Substring(
                                    item["Origen"].ToString().IndexOf('#') + 1);
                                dRow[2] = item["Referencia"].ToString();
                                dRow[3] = item["Destinatario"].ToString();
                                dRow[4] = item["Fecha recibida"].ToString();
                                //dRow[5] = "SI";//"<strong style=\"color:#008000\">SI</strong>";
                                dRow[5] = string.Format("<a href='{0}'>Ver</a>",
                                    listaCorreo.DefaultViewUrl.Remove(listaCorreo.DefaultViewUrl.LastIndexOf('/') + 1).Replace(" ", "%20") +
                                    "DispForm.aspx?ID=" + item.ID);
                                //dRow[5] = string.Format("<a href='{0}'>Ver</a>",
                                //    LISTA_REDIRIGIDA_FP + item.ID);

                                dTable.Rows.Add(dRow);
                            }
                        }
                        catch { }
                    }

                    dSet.Tables.Add(dTable);
                    return dSet;
                }
                finally
                {
                    webAdm.AllowUnsafeUpdates = false;

                    if (web != null) web.Dispose();
                    if (sitio != null) sitio.Dispose();

                    if (webAdm != null) webAdm.Dispose();
                    if (sitioAdm != null) sitioAdm.Dispose();
                }
                #endregion
            }
            catch (Exception ex)
            {
                dTable.Columns.Add("ERROR!", typeof(string));
                dRow = dTable.NewRow();
                dRow[0] = ex.Message;
                dTable.Rows.Add(dRow);
                dSet.Tables.Add(dTable);

                return dSet;
            }
        }

        /// <summary>
        /// Recupera toda la correspondencia perteneciente al usuario actual
        /// </summary>
        /// <returns></returns>
        public static DataSet RecuperarCorrespondenciaEP()
        {
            DataSet dSet = new DataSet("DataSet");
            DataTable dTable = new DataTable("DataTable");
            DataRow dRow;

            try
            {
                UrlFPC = ConfigurationManager.AppSettings["UrlFPC"];

                dTable.Columns.Add(new DataColumn("ID", typeof(Int32)));
                dTable.Columns.Add(new DataColumn("Origen", typeof(string)));
                dTable.Columns.Add(new DataColumn("Referencia", typeof(string)));
                dTable.Columns.Add(new DataColumn("Destinatario", typeof(string)));
                dTable.Columns.Add(new DataColumn("FechaRecibida", typeof(string)));
                dTable.Columns.Add(new DataColumn("Ver", typeof(string)));

                #region Consulta lista SP
                SPSite sitio = null;
                SPWeb web = null;
                try
                {
                    sitio = new SPSite(UrlFPC);
                    web = sitio.OpenWeb();

                    SPSecurity.RunWithElevatedPrivileges(delegate()
                    {//Como usuario administrador
                        sitioAdm = new SPSite(UrlFPC);
                        webAdm = sitioAdm.OpenWeb();
                    });

                    SPList listaCorreo = webAdm.Lists[LISTA_EDUCAPRO];
                    SPList listaCorreoCB = webAdm.Lists[LISTA_EDUCAPRO_CB];
                    SPList listaCorreoSC = webAdm.Lists[LISTA_EDUCAPRO_SC];

                    #region Educapro La Paz
                    foreach (SPListItem item in listaCorreo.Items)
                    {
                        string entregadas;
                        if (item["Entregada"] != null)
                            entregadas = item["Entregada"].ToString();
                        else
                            entregadas = "";
                        SPFieldUserValueCollection usuarios =
                            (SPFieldUserValueCollection)item["Dirigida a"];

                        try
                        {
                            if (entregadas.Contains(string.Format("<strong style=\"color:#FF0000\">No entregada</strong> " +
                                "a {0}; ", web.CurrentUser.Name)) &&
                                item["Estado corr."].ToString() != ESTADO_CORRESPONDENCIA)
                            {
                                entregadas = entregadas.Replace(string.Format("<strong style=\"color:#FF0000\">No entregada</strong> " +
                                    "a {0}; ", web.CurrentUser.Name), string.Format("<strong style=\"color:#008000\">Entregada</strong> a " +
                                    "{0} el {1}; ", web.CurrentUser.Name, string.Format("{0:d/MM/yyyy HH:mm}", DateTime.Now)));
                                webAdm.AllowUnsafeUpdates = true;
                                item["Entregada"] = entregadas;
                                using (DisabledItemEventsScope scope = new DisabledItemEventsScope())
                                {
                                    item.SystemUpdate();
                                }

                                dRow = dTable.NewRow();

                                dRow[0] = item.ID;
                                dRow[1] = item["Origen"].ToString().Substring(
                                    item["Origen"].ToString().IndexOf('#') + 1);
                                dRow[2] = item["Referencia"].ToString();
                                dRow[3] = item["Destinatario"].ToString();
                                dRow[4] = item["Fecha recibida"].ToString();
                                //dRow[5] = string.Format("<a href='{0}'>Ver</a>",
                                //    redireccion + item.ID);
                                dRow[5] = string.Format("<a href='{0}'>Ver</a>",
                                    listaCorreo.DefaultViewUrl.Remove(listaCorreo.DefaultViewUrl.LastIndexOf('/') + 1).Replace(" ", "%20") +
                                    "DispForm.aspx?ID=" + item.ID);
                                //dRow[5] = string.Format("<a href='{0}'>Ver</a>",
                                //    LISTA_REDIRIGIDA_EP + item.ID);

                                dTable.Rows.Add(dRow);

                                break;
                            }
                        }
                        catch { }
                    }
                    #endregion

                    #region Educapro Cochabamba
                    foreach (SPListItem item in listaCorreoCB.Items)
                    {
                        string entregadas;
                        if (item["Entregada"] != null)
                            entregadas = item["Entregada"].ToString();
                        else
                            entregadas = "";
                        SPFieldUserValueCollection usuarios =
                            (SPFieldUserValueCollection)item["Dirigida a"];

                        try
                        {
                            if (entregadas.Contains(string.Format("<strong style=\"color:#FF0000\">No entregada</strong> " +
                                "a {0}; ", web.CurrentUser.Name)) &&
                                item["Estado corr."].ToString() != ESTADO_CORRESPONDENCIA)
                            {
                                entregadas = entregadas.Replace(string.Format("<strong style=\"color:#FF0000\">No entregada</strong> " +
                                    "a {0}; ", web.CurrentUser.Name), string.Format("<strong style=\"color:#008000\">Entregada</strong> a " +
                                    "{0} el {1}; ", web.CurrentUser.Name, string.Format("{0:d/MM/yyyy HH:mm}", DateTime.Now)));
                                webAdm.AllowUnsafeUpdates = true;
                                item["Entregada"] = entregadas;
                                using (DisabledItemEventsScope scope = new DisabledItemEventsScope())
                                {
                                    item.SystemUpdate();
                                }

                                dRow = dTable.NewRow();

                                dRow[0] = item.ID;
                                dRow[1] = item["Origen"].ToString().Substring(
                                    item["Origen"].ToString().IndexOf('#') + 1);
                                dRow[2] = item["Referencia"].ToString();
                                dRow[3] = item["Destinatario"].ToString();
                                dRow[4] = item["Fecha recibida"].ToString();
                                dRow[5] = string.Format("<a href='{0}'>Ver</a>",
                                    listaCorreoCB.DefaultViewUrl.Remove(listaCorreoCB.DefaultViewUrl.LastIndexOf('/') + 1).Replace(" ", "%20") +
                                    "DispForm.aspx?ID=" + item.ID);
                                //dRow[5] = string.Format("<a href='{0}'>Ver</a>",
                                //    LISTA_REDIRIGIDA_EP_CB + item.ID);

                                dTable.Rows.Add(dRow);

                                break;
                            }
                        }
                        catch { }
                    }
                    #endregion

                    #region Educapro Santa Cruz
                    foreach (SPListItem item in listaCorreoSC.Items)
                    {
                        string entregadas;
                        if (item["Entregada"] != null)
                            entregadas = item["Entregada"].ToString();
                        else
                            entregadas = "";
                        SPFieldUserValueCollection usuarios =
                            (SPFieldUserValueCollection)item["Dirigida a"];

                        try
                        {
                            if (entregadas.Contains(string.Format("<strong style=\"color:#FF0000\">No entregada</strong> " +
                                "a {0}; ", web.CurrentUser.Name)) &&
                                item["Estado corr."].ToString() != ESTADO_CORRESPONDENCIA)
                            {
                                entregadas = entregadas.Replace(string.Format("<strong style=\"color:#FF0000\">No entregada</strong> " +
                                    "a {0}; ", web.CurrentUser.Name), string.Format("<strong style=\"color:#008000\">Entregada</strong> a " +
                                    "{0} el {1}; ", web.CurrentUser.Name, string.Format("{0:d/MM/yyyy HH:mm}", DateTime.Now)));
                                webAdm.AllowUnsafeUpdates = true;
                                item["Entregada"] = entregadas;
                                using (DisabledItemEventsScope scope = new DisabledItemEventsScope())
                                {
                                    item.SystemUpdate();
                                }

                                dRow = dTable.NewRow();

                                dRow[0] = item.ID;
                                dRow[1] = item["Origen"].ToString().Substring(
                                    item["Origen"].ToString().IndexOf('#') + 1);
                                dRow[2] = item["Referencia"].ToString();
                                dRow[3] = item["Destinatario"].ToString();
                                dRow[4] = item["Fecha recibida"].ToString();
                                dRow[5] = string.Format("<a href='{0}'>Ver</a>",
                                    listaCorreoSC.DefaultViewUrl.Remove(listaCorreoSC.DefaultViewUrl.LastIndexOf('/') + 1).Replace(" ", "%20") +
                                    "DispForm.aspx?ID=" + item.ID);
                                //dRow[5] = string.Format("<a href='{0}'>Ver</a>",
                                //    LISTA_REDIRIGIDA_EP_SC + item.ID);

                                dTable.Rows.Add(dRow);

                                break;
                            }
                        }
                        catch { }
                    }
                    #endregion

                    dSet.Tables.Add(dTable);
                    return dSet;
                }
                finally
                {
                    webAdm.AllowUnsafeUpdates = false;

                    if (web != null) web.Dispose();
                    if (sitio != null) sitio.Dispose();

                    if (webAdm != null) webAdm.Dispose();
                    if (sitioAdm != null) sitioAdm.Dispose();
                }
                #endregion
            }
            catch (Exception ex)
            {
                dTable.Columns.Add("ERROR!", typeof(string));
                dRow = dTable.NewRow();
                dRow[0] = ex.Message;
                dTable.Rows.Add(dRow);
                dSet.Tables.Add(dTable);

                return dSet;
            }
        }

        //private static string RecuperarIDDeItem(string titulo, SPList lista)
        //{
        //    foreach (SPListItem item in lista.Items)
        //    {
        //        if (item.Title.Trim().ToUpper() ==
        //            titulo.Trim().ToUpper())
        //            return item.ID.ToString();
        //    }

        //    return "";
        //}
    }
}
