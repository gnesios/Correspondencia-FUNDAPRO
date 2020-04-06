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

namespace FPC_WPNuevaCorrespondencia
{
    public static class ConectorWebPart
    {
        static string UrlFPC;
        static SPSite sitio;
        static SPWeb web;

        /// <summary>
        /// Recupera todos los documentos de la ruta indicada en el archivo config
        /// </summary>
        /// <returns></returns>
        public static DataSet RecuperarDocumentosFP()
        {
            DataSet dSet = new DataSet("DataSet");
            //DataView dView = new DataView();
            DataTable dTable = new DataTable("DataTable");
            DataRow dRow;

            try
            {
                dTable.Columns.Add(new DataColumn("NombreArchivo", typeof(string)));
                dTable.Columns.Add(new DataColumn("TipoArchivo", typeof(string)));
                dTable.Columns.Add(new DataColumn("VistaPrevia", typeof(string)));
                dTable.Columns.Add(new DataColumn("RutaArchivo", typeof(string)));

                string carpetaArchivos = ConfigurationManager.AppSettings["CarpetaArchivosFP"];
                string[] archivos = Directory.GetFiles(carpetaArchivos, "*.pdf", SearchOption.AllDirectories);

                foreach (string archivo in archivos)
                {
                    string nombreArchivo = archivo.Substring(archivo.LastIndexOf('\\') + 1);
                    dRow = dTable.NewRow();

                    dRow[0] = nombreArchivo.Remove(nombreArchivo.LastIndexOf('.'));
                    dRow[1] = nombreArchivo.Substring(nombreArchivo.LastIndexOf('.') + 1);
                    dRow[2] = RecuperarRutaThumbnail(archivo);
                    dRow[3] = archivo;

                    dTable.Rows.Add(dRow);
                }

                dSet.Tables.Add(dTable);
                return dSet;
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
        /// Recupera todos los documentos de la ruta indicada en el archivo config
        /// </summary>
        /// <returns></returns>
        public static DataSet RecuperarDocumentosEP()
        {
            DataSet dSet = new DataSet("DataSet");
            //DataView dView = new DataView();
            DataTable dTable = new DataTable("DataTable");
            DataRow dRow;

            try
            {
                dTable.Columns.Add(new DataColumn("NombreArchivo", typeof(string)));
                dTable.Columns.Add(new DataColumn("TipoArchivo", typeof(string)));
                dTable.Columns.Add(new DataColumn("VistaPrevia", typeof(string)));
                dTable.Columns.Add(new DataColumn("RutaArchivo", typeof(string)));

                string carpetaArchivos = ConfigurationManager.AppSettings["CarpetaArchivosEP"];
                string[] archivos = Directory.GetFiles(carpetaArchivos, "*.pdf", SearchOption.AllDirectories);

                foreach (string archivo in archivos)
                {
                    string nombreArchivo = archivo.Substring(archivo.LastIndexOf('\\') + 1);
                    dRow = dTable.NewRow();

                    dRow[0] = nombreArchivo.Remove(nombreArchivo.LastIndexOf('.'));
                    dRow[1] = nombreArchivo.Substring(nombreArchivo.LastIndexOf('.') + 1);
                    dRow[2] = RecuperarRutaThumbnail(archivo);
                    dRow[3] = archivo;

                    dTable.Rows.Add(dRow);
                }

                dSet.Tables.Add(dTable);
                return dSet;
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
        /// Guarda el registro de nueva correspondencia en lista FundaPro
        /// </summary>
        /// <param name="tipoCarta"></param>
        /// <param name="origenCarta"></param>
        /// <param name="referencia"></param>
        /// <param name="fechaCarta"></param>
        /// <param name="fechaRecibida"></param>
        /// <param name="destinatario"></param>
        /// <param name="dirigidaA"></param>
        /// <param name="numCarta"></param>
        /// <param name="privada"></param>
        /// <param name="hojaRuta"></param>
        /// <param name="archivo"></param>
        public static int GuardarNuevoRegistro(string tipoCarta, int origenCarta,
            string referencia, DateTime fechaCarta, DateTime fechaRecibida, string destinatario,
            ArrayList dirigidaA, string numCarta, string adjunto, string clase, string prioridad,
            bool privada, bool hojaRuta, string archivo, List<string> rutas, string urlLista)
        {
            UrlFPC = ConfigurationManager.AppSettings["UrlFPC"];

            #region Consulta lista SP
            try
            {
                sitio = new SPSite(UrlFPC);
                web = sitio.OpenWeb();

                #region Conversion de usuarios para insercion
                SPFieldUserValueCollection usuariosDirigidos = new SPFieldUserValueCollection();
                foreach (string usuario in dirigidaA)
                {
                    SPFieldUserValue spv = null;

                    do
                    {
                        try
                        { spv = new SPFieldUserValue(web, web.SiteUsers[usuario].ID, usuario); }
                        catch
                        { web.SiteUsers.Add(usuario, "", "", ""); }
                    } while (spv == null);

                    usuariosDirigidos.Add(spv);
                }
                #endregion

                //SPListItem nuevoItem = web.Lists[urlLista].Items.Add();
                SPListItem nuevoItem = web.GetList(urlLista).Items.Add();
                nuevoItem["Tipo corr."] = tipoCarta;
                nuevoItem["Origen"] = origenCarta;
                nuevoItem["Referencia"] = referencia;
                nuevoItem["Fecha origen"] = fechaCarta;
                nuevoItem["Fecha recibida"] = fechaRecibida;
                nuevoItem["Destinatario"] = destinatario;
                nuevoItem["Dirigida a"] = usuariosDirigidos;
                nuevoItem["Num. ó Cite"] = numCarta;
                nuevoItem["Adjunto"] = adjunto;
                nuevoItem["Clase de documento"] = clase;
                nuevoItem["Prioridad corr."] = prioridad;
                nuevoItem["Privada"] = privada;
                nuevoItem["Hoja de ruta"] = hojaRuta;
                nuevoItem["Archivo"] = archivo;
                if (!string.IsNullOrEmpty(archivo))
                    nuevoItem["Estado corr."] = "PASIVA";
                //if (!string.IsNullOrEmpty(archivo) &&
                //    !(archivo.Equals("<DIV></DIV>", StringComparison.CurrentCultureIgnoreCase) ||
                //    archivo.Equals("<P>&nbsp;</P>", StringComparison.CurrentCultureIgnoreCase) ||
                //    archivo.Equals("<DIV>&nbsp;</DIV>", StringComparison.CurrentCultureIgnoreCase)))
                //    nuevoItem["Estado corr."] = "PASIVA";

                AdjuntarArchivos(rutas, nuevoItem);

                nuevoItem.Update();

                return nuevoItem.ID;
            }
            finally
            {
                if (web != null) web.Dispose();
                if (sitio != null) sitio.Dispose();
            }
            #endregion
        }

        /// <summary>
        /// Elimina archivos ya adjuntados a una correspondencia
        /// </summary>
        /// <param name="rutas"></param>
        public static void EliminarArchivosAdjuntados(List<string> rutas)
        {
            foreach (string archivo in rutas)
            {
                string nombreDirectorio =
                    @"c:\Program Files\Common Files\Microsoft Shared\web server extensions\12\TEMPLATE\IMAGES\FPCImages";
                string temp = archivo.Remove(archivo.LastIndexOf('.'));
                string nombreArchivo = temp.Substring(temp.LastIndexOf('\\') + 1) + ".jpg";

                SPSecurity.RunWithElevatedPrivileges(delegate()
                {//Como usuario administrador
                    if (File.Exists(archivo))
                    {
                        try
                        {
                            File.Delete(archivo);
                        }
                        catch { }
                    }

                    if (File.Exists(nombreDirectorio + "\\" + nombreArchivo))
                    {
                        try
                        {
                            File.Delete(nombreDirectorio + "\\" + nombreArchivo);
                        }
                        catch { }
                    }
                });
            }
        }

        /// <summary>
        /// Recupera la lista de Orígenes Correspondencia
        /// </summary>
        /// <returns></returns>
        public static List<ListItem> RecuperarOrigenesCorrespondencia()
        {
            UrlFPC = ConfigurationManager.AppSettings["UrlFPC"];
            List<ListItem> origenes = new List<ListItem>();
            //Hashtable origenes = new Hashtable();

            #region Consulta lista SP
            try
            {
                SPSecurity.RunWithElevatedPrivileges(delegate()
                {//Como usuario administrador
                    sitio = new SPSite(UrlFPC);
                    web = sitio.OpenWeb();
                });
                
                SPList listaOrigenes = web.Lists["Orígenes Correspondencia"];
                
                #region Llenar y ordenar lista
                foreach (SPListItem item in listaOrigenes.Items)
                {
                    ListItem origen = new ListItem(item.Title, item.ID.ToString());

                    if (!origenes.Contains(origen))
                        origenes.Add(origen);
                }

                origenes.Sort(
                    delegate(ListItem i1, ListItem i2)
                    {
                        return Comparer<string>.Default.Compare(i1.Text, i2.Text);
                    });
                #endregion

                return origenes;
            }
            finally
            {
                if (web != null) web.Dispose();
                if (sitio != null) sitio.Dispose();
            }
            #endregion
        }

        /// <summary>
        /// Inserta un nuevo origen de correspondencia en la lista correspondiente
        /// </summary>
        /// <param name="tituloOrigen"></param>
        /// <returns></returns>
        public static int InsertarNuevoOrigen(string tituloOrigen)
        {
            UrlFPC = ConfigurationManager.AppSettings["UrlFPC"];

            #region Consulta lista SP
            try
            {
                sitio = new SPSite(UrlFPC);
                web = sitio.OpenWeb();

                SPList lista = web.Lists["Orígenes Correspondencia"];

                foreach (SPListItem item in lista.Items)
                {
                    if (item.Title.Trim() == tituloOrigen.Trim())
                        return item.ID;
                }

                SPListItem nuevoItem = lista.Items.Add();
                nuevoItem["Title"] = tituloOrigen.Trim();
                nuevoItem.Update();

                return nuevoItem.ID;
            }
            finally
            {
                if (web != null) web.Dispose();
                if (sitio != null) sitio.Dispose();
            }
            #endregion
        }

        /// <summary>
        /// Recupera la URL relativa de la vista por defecto de la lista dada
        /// </summary>
        /// <param name="nombreLista"></param>
        /// <returns></returns>
        /*public static string RecuperarURLDeLista(string nombreLista)
        {
            string url = "";

            try
            {
                sitio = new SPSite(UrlFPC);
                web = sitio.OpenWeb();

                SPList lista = web.Lists[nombreLista];
                url = lista.DefaultViewUrl;
            }
            catch
            {
                if (web != null) web.Dispose();
                if (sitio != null) sitio.Dispose();
            }

            return url;
        }*/

        /// <summary>
        /// Adjunta archivo en lista
        /// </summary>
        /// <param name="rutas"></param>
        /// <param name="nuevoItem"></param>
        private static void AdjuntarArchivos(List<string> rutas, SPListItem nuevoItem)
        {
            foreach (string ruta in rutas)
            {
                FileStream file = File.OpenRead(ruta);
                byte[] bytes = new byte[file.Length];
                file.Read(bytes, 0, (int)file.Length);
                file.Close();
                file.Dispose();

                nuevoItem.Attachments.Add(ruta.Substring(ruta.LastIndexOf('\\') + 1), bytes);
            }
        }

        private static string RecuperarRutaThumbnail(string pdfFuente)
        {
            try
            {
                string temp = pdfFuente.Remove(pdfFuente.LastIndexOf('.'));
                string nombreThumb = temp.Substring(temp.LastIndexOf('\\') + 1) + ".jpg";

                string nombreDirectorio =
                    @"c:\Program Files\Common Files\Microsoft Shared\web server extensions\12\TEMPLATE\IMAGES\FPCImages";
                string thumbDestino = nombreDirectorio + "\\" + nombreThumb;

                SPSecurity.RunWithElevatedPrivileges(delegate()
                {//Como usuario administrador
                    if (!Directory.Exists(nombreDirectorio))
                    {
                        DirectoryInfo dInfo = Directory.CreateDirectory(nombreDirectorio);
                    }

                    if (!File.Exists(thumbDestino))
                        GhostscriptWrapper.GeneratePageThumb(pdfFuente, thumbDestino, 1, 15, 15);
                });

                return "~/_layouts/images/FPCImages/" + nombreThumb;
            }
            catch
            {
                return "~/_layouts/images/lg_ICDIB.gif";
            }
        }

        private static string RecuperarIDDeItem(string titulo, SPList lista)
        {
            foreach (SPListItem item in lista.Items)
            {
                if (item.Title.Trim().ToUpper() ==
                    titulo.Trim().ToUpper())
                    return item.ID.ToString();
            }

            return "";
        }
    }
}
