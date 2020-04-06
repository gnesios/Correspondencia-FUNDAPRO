using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.SharePoint;

namespace FPC_IniciarCorrespondenciaEnX
{
    class Program
    {
        static void Main(string[] args)
        {
            SPSite sitio = null;
            SPWeb web = null;

            string readUrlSitio;
            string readListaOrigen;
            string readListaDestino;

            try
            {
                #region Toma de datos
                Console.WriteLine("***************************************************");
                Console.WriteLine("Este programa lleva los items de una lista A " +
                    "a una lista B.");
                Console.WriteLine("***************************************************");
                Console.WriteLine();

                Console.Write("Ingrese la URL del sitio: ");
                readUrlSitio = Console.ReadLine();

                Console.Write("Ingrese el nombre de la lista ORIGEN: ");
                readListaOrigen = Console.ReadLine();

                Console.Write("Ingrese el nombre de la lista DESTINO: ");
                readListaDestino = Console.ReadLine();

                Console.WriteLine();

                #endregion

                #region Consulta SP
                sitio = new SPSite(readUrlSitio);
                web = sitio.OpenWeb();

                SPList listaOrigen = web.Lists[readListaOrigen];
                SPList listaDestino = web.Lists[readListaDestino];

                foreach (SPListItem item in listaOrigen.Items)
                {
                    SPListItem itemCopiado = CopiarItem(item, listaDestino);
                    Console.WriteLine("Copiando: <" + itemCopiado.Title + "><" + itemCopiado.ID + ">");
                }
                #endregion

                Console.WriteLine("Operación completada.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Se produjo un error vuelva a intentarlo.");
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (web != null) web.Dispose();
                if (sitio != null) sitio.Dispose();
            }
        }

        static private SPListItem CopiarItem(SPListItem itemOrigen, SPList listaDestino)
        {
            //Copy sourceItem to destinationList
            //SPList listaDestino = itemOrigen.Web.Lists[nombreListaDestino];
            SPListItem itemDestino = listaDestino.Items.Add();

            foreach (SPField field in itemOrigen.Fields)
            {
                //Copy all except attachments
                if (!field.ReadOnlyField && field.InternalName != "Attachments"
                    && itemOrigen[field.InternalName] != null)
                {
                    itemDestino[field.InternalName] = itemOrigen[field.InternalName];
                }
            }

            //Copy attachments
            foreach (string nombreItem in itemOrigen.Attachments)
            {
                SPFile archivo = itemOrigen.ParentList.ParentWeb.GetFile(
                    itemOrigen.Attachments.UrlPrefix + nombreItem);
                byte[] bytes = archivo.OpenBinary();
                itemDestino.Attachments.Add(nombreItem, bytes);
            }

            itemDestino.Update();
            return itemDestino;
        }
    }
}
