using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace FPC_ECBImprimirHojaDRuta
{
    public partial class ImprimirHojaDRuta : LayoutsPageBase
    {
        const string LISTA_OBJETIVO_FP = "Correspondencia de Entrada Funda-Pro";
        const string LISTA_OBJETIVO_EP = "Correspondencia de Entrada Educa-Pro";
        const string LISTA_OBJETIVO_EP_CB = "Correspondencia de Entrada Educa-Pro (CB)";
        const string LISTA_OBJETIVO_EP_SC = "Correspondencia de Entrada Educa-Pro (SC)";
        const string PAGINA_IMPRESION_FP = "/_layouts/PaginasFPC/printFP.aspx?";
        const string PAGINA_IMPRESION_EP = "/_layouts/PaginasFPC/printEP.aspx?";
        const string PAGINA_IMPRESION_EP_CB = "/_layouts/PaginasFPC/printEP_CB.aspx?";
        const string PAGINA_IMPRESION_EP_SC = "/_layouts/PaginasFPC/printEP_SC.aspx?";

        string referencia;
        int idCarta;
        DateTime fechaRecibida;
        DateTime fechaCarta;
        string numCarta;
        string destinatario;
        string origenCarta;
        string url;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                string retorno = this.RecuperarDatosCorrespondencia();

                ltlResultados.Text = "Redirigiendo...";

                referencia = referencia.Replace("\r\n", " ");
                string parametros = string.Format("id={0}&fr={1}&fcc={2}&dt={3}&rf={4}&or={5}&url={6}",
                        idCarta.ToString(), fechaRecibida.ToShortDateString(),
                        fechaCarta.ToShortDateString() + " - " + numCarta, destinatario, referencia,
                        origenCarta, url);

                if (retorno == "FP")
                    this.Page.Response.Redirect(PAGINA_IMPRESION_FP + parametros);
                else if (retorno == "EP")
                    this.Page.Response.Redirect(PAGINA_IMPRESION_EP + parametros);
                else if (retorno == "EP_CB")
                    this.Page.Response.Redirect(PAGINA_IMPRESION_EP_CB + parametros);
                else if (retorno == "EP_SC")
                    this.Page.Response.Redirect(PAGINA_IMPRESION_EP_SC + parametros);
            }
            catch (Exception ex)
            {
                ltlResultados.Text = ex.Message;
            }
        }

        private string RecuperarDatosCorrespondencia()
        {
            SPWeb web = this.Web;

            string listaId = Request.QueryString["ListId"];
            string itemId = Request.QueryString["ItemId"];

            //SPList lista = web.Lists[LISTA_OBJETIVO_FP];
            SPList lista = web.Lists[new Guid(listaId)];
            SPListItem item = lista.Items.GetItemById(Convert.ToInt32(itemId));

            referencia = item["Referencia"].ToString();
            idCarta = item.ID;
            fechaRecibida = Convert.ToDateTime(item["Fecha recibida"]);
            fechaCarta = Convert.ToDateTime(item["Fecha origen"]);
            if (item["Num. ó Cite"] != null)
                numCarta = item["Num. ó Cite"].ToString();
            else
                numCarta = "";
            destinatario = item["Destinatario"].ToString();
            origenCarta = item["Origen"].ToString().Substring(
                item["Origen"].ToString().IndexOf('#') + 1);
            url = "/" + item.Url.Remove(item.Url.LastIndexOf('/') + 1);

            if (web != null) web.Dispose();
            //if (web.Lists[new Guid(listaId)].Title == LISTA_OBJETIVO_FP)
            //    return "FP";
            //if (web.Lists[new Guid(listaId)].Title == LISTA_OBJETIVO_EP)
            //    return "EP";
            //else if (web.Lists[new Guid(listaId)].Title == LISTA_OBJETIVO_EP_CB)
            //    return "EP_CB";
            //else if (web.Lists[new Guid(listaId)].Title == LISTA_OBJETIVO_EP_SC)
            //    return "EP_SC";
            //else
            //    return "";

            if (web.Lists[new Guid(listaId)].Title.Contains(LISTA_OBJETIVO_FP))
                return "FP";
            if (web.Lists[new Guid(listaId)].Title.Contains(LISTA_OBJETIVO_EP))
                return "EP";
            else if (web.Lists[new Guid(listaId)].Title.Contains(LISTA_OBJETIVO_EP_CB))
                return "EP_CB";
            else if (web.Lists[new Guid(listaId)].Title.Contains(LISTA_OBJETIVO_EP_SC))
                return "EP_SC";
            else
                return "";
        }
    }
}
