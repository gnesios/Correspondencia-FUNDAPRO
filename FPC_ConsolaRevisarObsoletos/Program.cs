using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Microsoft.SharePoint;

namespace FPC_ConsolaRevisarObsoletos
{
    public class Program
    {
        const string ESTADO = "OBSOLETA";
        const string PARAMETRO = "Tiempo obsolescencia";
        static int tiempoObsoleto;

        static void Main(string[] args)
        {
            SPSite sitio = null;
            SPWeb web = null;

            try
            {
                string UrlFPC = ConfigurationManager.AppSettings["UrlFPC"];
                int contador = 0;

                sitio = new SPSite(UrlFPC);
                web = sitio.OpenWeb();

                SPList listaFundapro = web.Lists["Correspondencia de Entrada Funda-Pro"];
                SPList listaEducapro = web.Lists["Correspondencia de Entrada Educa-Pro"];
                SPList listaEducaproCB = web.Lists["Correspondencia de Entrada Educa-Pro (CB)"];
                SPList listaEducaproSC = web.Lists["Correspondencia de Entrada Educa-Pro (SC)"];
                SPList listaParametros = web.Lists["Parámetros Globales"];

                #region Recuperar parametro global
                SPListItemCollection itemsParametros = listaParametros.Items;
                foreach (SPListItem item in itemsParametros)
                {
                    if (string.Equals(item.Title.Trim(), PARAMETRO,
                        StringComparison.CurrentCultureIgnoreCase))
                    {
                        tiempoObsoleto = Convert.ToInt32(item["Valor parámetro"].ToString());
                        break;
                    }
                }
                #endregion

                #region Consulta SP (Query)
                SPQuery consulta = new SPQuery();
                consulta.Query =
                    "<Where><Or><Eq><FieldRef Name='Estado_x0020_corr_x002e_' />" +
                    "<Value Type='Text'>ACTIVA</Value></Eq><Eq>" +
                    "<FieldRef Name='Estado_x0020_corr_x002e_' />" +
                    "<Value Type='Text'>PASIVA</Value>" +
                    "</Eq></Or></Where>";
                #endregion

                #region Fundapro
                SPListItemCollection itemsFundapro = listaFundapro.GetItems(consulta);
                foreach (SPListItem item in itemsFundapro)
                {
                    if (EsItemObsoleto(item))
                    {
                        item["Estado corr."] = ESTADO;
                        using (DisabledItemEventsScope scope = new DisabledItemEventsScope())
                        {
                            item.SystemUpdate();
                        }

                        contador++;
                    }
                }

                Console.WriteLine(contador.ToString() + " item(s) obsoleto(s) FUNDAPRO.");
                contador = 0;
                #endregion

                #region Educapro La Paz
                SPListItemCollection itemsEducapro = listaEducapro.GetItems(consulta);
                foreach (SPListItem item in itemsEducapro) //La Paz
                {
                    if (EsItemObsoleto(item))
                    {
                        item["Estado corr."] = ESTADO;
                        using (DisabledItemEventsScope scope = new DisabledItemEventsScope())
                        {
                            item.SystemUpdate();
                        }
                    }
                }

                Console.WriteLine(contador.ToString() + " item(s) obsoleto(s) EDUCAPRO LA PAZ.");
                contador = 0;
                #endregion

                #region Educapro Cochabamba
                SPListItemCollection itemsEducaproCB = listaEducaproCB.GetItems(consulta);
                foreach (SPListItem item in itemsEducaproCB) //Cochabamba
                {
                    if (EsItemObsoleto(item))
                    {
                        item["Estado corr."] = ESTADO;
                        using (DisabledItemEventsScope scope = new DisabledItemEventsScope())
                        {
                            item.SystemUpdate();
                        }
                    }
                }

                Console.WriteLine(contador.ToString() + " item(s) obsoleto(s) EDUCAPRO COCHABAMBA.");
                contador = 0;
                #endregion

                #region Educapro Santa Cruz
                SPListItemCollection itemsEducaproSC = listaEducaproSC.GetItems(consulta);
                foreach (SPListItem item in itemsEducaproSC) //Santa Cruz
                {
                    if (EsItemObsoleto(item))
                    {
                        item["Estado corr."] = ESTADO;
                        using (DisabledItemEventsScope scope = new DisabledItemEventsScope())
                        {
                            item.SystemUpdate();
                        }
                    }
                }

                Console.WriteLine(contador.ToString() + " item(s) obsoleto(s) EDUCAPRO SANTA CRUZ.");
                contador = 0;
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }
            finally
            {
                if (web != null) web.Dispose();
                if (sitio != null) sitio.Dispose();
            }
        }

        /// <summary>
        /// Verifica si el elemento proveido es obsoleto o no
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        private static bool EsItemObsoleto(SPListItem item)
        {
            DateTime fechaUltimaModificacion = Convert.ToDateTime(item["Modified"]);
            //int tiempoObsoleto = Convert.ToInt16(RecuperarValorParametroGlobal(PARAMETRO));

            if (DateTime.Today.Subtract(fechaUltimaModificacion).Days > tiempoObsoleto)
                return true;

            return false;
        }
    }
}
