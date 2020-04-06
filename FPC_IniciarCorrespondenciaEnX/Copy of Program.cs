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

            string urlSitio;
            string nombreLista;
            int idInicio;

            try
            {
                #region Toma de datos
                Console.WriteLine("Este programa permite iniciar una determinada " +
                    "lista con un valor ID definido.");
                Console.WriteLine();

                Console.Write("Ingrese la URL del sitio: ");
                urlSitio = Console.ReadLine();

                Console.Write("Ingrese el nombre de la lista: ");
                nombreLista = Console.ReadLine();

                Console.Write("Ingrese el ID de inicio: ");
                idInicio = Convert.ToInt16(Console.ReadLine());
                #endregion

                #region Consulta SP
                sitio = new SPSite(urlSitio);
                web = sitio.OpenWeb();

                SPList lista = web.Lists[nombreLista];
                SPListItemCollection itemsAntes = lista.Items;

                if (itemsAntes.Count == 0)
                {
                    #region Crear items
                    for (int i = 1; i < idInicio; i++)
                    {
                        SPListItem item = itemsAntes.Add();
                        item["Title"] = "Item " + i;
                        item.Update();
                    }
                    #endregion

                    #region Eliminar items
                    SPListItemCollection itemsDespues = lista.Items;
                    StringBuilder batchString = new StringBuilder();
                    batchString.Append("<?xml version=\"1.0\" encoding=\"UTF-8\"?><Batch>");
                    foreach (SPListItem item in itemsDespues)
                    {
                        batchString.Append("<Method>");
                        batchString.Append("<SetList Scope=\"Request\">" + Convert.ToString(item.ParentList.ID) + "</SetList>");
                        batchString.Append("<SetVar Name=\"ID\">" + Convert.ToString(item.ID) + "</SetVar>");
                        batchString.Append("<SetVar Name=\"Cmd\">Delete</SetVar>");
                        batchString.Append("</Method>");
                    }
                    batchString.Append("</Batch>");
                    web.ProcessBatchData(batchString.ToString());

                    // Eliminar papelera
                    sitio.RecycleBin.DeleteAll();

                    Console.WriteLine("Proceso completado exitosamente.");
                    #endregion
                }
                else
                {
                    Console.WriteLine("Este proceso solo puede ser aplicado " +
                        "a una lista que no contenga items.");
                }
                #endregion
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
    }
}
