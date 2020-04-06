using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using Microsoft.SharePoint;

using System.Web.UI.WebControls.WebParts;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace FPC_WPEntregarCorrespondencia
{
    public class EntregarCorrespondencia : WebPart
    {
        #region Definicion de controles
        GridView grvCorrespondenciaFP;
        GridView grvCorrespondenciaEP;
        #endregion

        protected override void CreateChildControls()
        {
            try
            {
                #region Creacion de controles
                grvCorrespondenciaFP = new GridView();
                grvCorrespondenciaFP.ID = "grvCorrespondenciaFP";
                grvCorrespondenciaFP.GridLines = GridLines.None;
                grvCorrespondenciaFP.ForeColor = Color.FromArgb(51, 51, 51);
                grvCorrespondenciaFP.CellPadding = 4;
                grvCorrespondenciaFP.AutoGenerateColumns = false;
                //grvCorrespondenciaFP.RowStyle.BackColor = Color.FromArgb(227, 234, 235);
                grvCorrespondenciaFP.RowStyle.BackColor = Color.White;
                grvCorrespondenciaFP.HeaderStyle.Font.Bold = true;
                grvCorrespondenciaFP.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                grvCorrespondenciaFP.HeaderStyle.BackColor = Color.FromArgb(245, 245, 245);
                grvCorrespondenciaFP.AlternatingRowStyle.BackColor = Color.FromArgb(250, 250, 250);
                grvCorrespondenciaFP.Width = Unit.Percentage(100);
                grvCorrespondenciaFP.EmptyDataText = "Usted no tiene correspondencia.";

                grvCorrespondenciaEP = new GridView();
                grvCorrespondenciaEP.ID = "grvCorrespondenciaEP";
                grvCorrespondenciaEP.GridLines = GridLines.None;
                grvCorrespondenciaEP.ForeColor = Color.FromArgb(51, 51, 51);
                grvCorrespondenciaEP.CellPadding = 4;
                grvCorrespondenciaEP.AutoGenerateColumns = false;
                grvCorrespondenciaEP.RowStyle.BackColor = Color.White;
                grvCorrespondenciaEP.HeaderStyle.Font.Bold = true;
                grvCorrespondenciaEP.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
                grvCorrespondenciaEP.HeaderStyle.BackColor = Color.FromArgb(245, 245, 245);
                grvCorrespondenciaEP.AlternatingRowStyle.BackColor = Color.FromArgb(250, 250, 250);
                grvCorrespondenciaEP.Width = Unit.Percentage(100);
                grvCorrespondenciaEP.EmptyDataText = "Usted no tiene correspondencia.";
                #endregion

                #region Adicion de columnas al Grid
                BoundField bflIdFP = new BoundField();
                bflIdFP.HeaderText = "ID";
                bflIdFP.DataField = "ID";

                BoundField bflOrigenFP = new BoundField();
                bflOrigenFP.HeaderText = "Origen";
                bflOrigenFP.DataField = "Origen";

                BoundField bflReferenciaFP = new BoundField();
                bflReferenciaFP.HeaderText = "Referencia";
                bflReferenciaFP.DataField = "Referencia";

                BoundField bflDestinatarioFP = new BoundField();
                bflDestinatarioFP.HeaderText = "Destinatario";
                bflDestinatarioFP.DataField = "Destinatario";

                BoundField bflFechaRecibidaFP = new BoundField();
                bflFechaRecibidaFP.HeaderText = "Fecha recibida";
                bflFechaRecibidaFP.DataField = "FechaRecibida";

                //BoundField bflEntregadaFP = new BoundField();
                //bflEntregadaFP.HeaderText = "Entregada";
                //bflEntregadaFP.DataField = "Entregada";

                HyperLinkField hflVerFP = new HyperLinkField();
                hflVerFP.HeaderText = "";
                hflVerFP.DataTextField = "Ver";

                grvCorrespondenciaFP.Columns.Add(bflIdFP);
                grvCorrespondenciaFP.Columns.Add(bflOrigenFP);
                grvCorrespondenciaFP.Columns.Add(bflReferenciaFP);
                grvCorrespondenciaFP.Columns.Add(bflDestinatarioFP);
                grvCorrespondenciaFP.Columns.Add(bflFechaRecibidaFP);
                //grvCorrespondenciaFP.Columns.Add(bflEntregadaFP);
                grvCorrespondenciaFP.Columns.Add(hflVerFP);

                grvCorrespondenciaFP.DataSource =
                    ConectorWebPart.RecuperarCorrespondenciaFP().Tables["DataTable"];
                grvCorrespondenciaFP.DataBind();

                BoundField bflIdEP = new BoundField();
                bflIdEP.HeaderText = "ID";
                bflIdEP.DataField = "ID";

                BoundField bflOrigenEP = new BoundField();
                bflOrigenEP.HeaderText = "Origen";
                bflOrigenEP.DataField = "Origen";

                BoundField bflReferenciaEP = new BoundField();
                bflReferenciaEP.HeaderText = "Referencia";
                bflReferenciaEP.DataField = "Referencia";

                BoundField bflDestinatarioEP = new BoundField();
                bflDestinatarioEP.HeaderText = "Destinatario";
                bflDestinatarioEP.DataField = "Destinatario";

                BoundField bflFechaRecibidaEP = new BoundField();
                bflFechaRecibidaEP.HeaderText = "Fecha recibida";
                bflFechaRecibidaEP.DataField = "FechaRecibida";

                //BoundField bflEntregadaEP = new BoundField();
                //bflEntregadaEP.HeaderText = "Entregada";
                //bflEntregadaEP.DataField = "Entregada";

                HyperLinkField hflVerEP = new HyperLinkField();
                hflVerEP.HeaderText = "";
                hflVerEP.DataTextField = "Ver";

                grvCorrespondenciaEP.Columns.Add(bflIdEP);
                grvCorrespondenciaEP.Columns.Add(bflOrigenEP);
                grvCorrespondenciaEP.Columns.Add(bflReferenciaEP);
                grvCorrespondenciaEP.Columns.Add(bflDestinatarioEP);
                grvCorrespondenciaEP.Columns.Add(bflFechaRecibidaEP);
                //grvCorrespondenciaEP.Columns.Add(bflEntregadaEP);
                grvCorrespondenciaEP.Columns.Add(hflVerEP);

                grvCorrespondenciaEP.DataSource =
                    ConectorWebPart.RecuperarCorrespondenciaEP().Tables["DataTable"];
                grvCorrespondenciaEP.DataBind();
                #endregion

                #region Adiccion de controles
                this.Controls.Add(new LiteralControl("<table border='0' cellspacing='0' width='100%'>"));

                this.Controls.Add(new LiteralControl("<tr><td style='border-bottom:1px black solid'><b>Correspondencia Funda-Pro</b></td></tr>"));
                this.Controls.Add(new LiteralControl("<tr><td width='100%' valign='top' class='ms-formlabel'><H2 class='ms-standardheader'><nobr>"));
                this.Controls.Add(grvCorrespondenciaFP);
                this.Controls.Add(new LiteralControl("</nobr></H2></td></tr>"));
                this.Controls.Add(new LiteralControl("<tr><td style='border-bottom:1px black solid'><b>Correspondencia Educa-Pro</b></td></tr>"));
                this.Controls.Add(new LiteralControl("<tr><td width='100%' valign='top' class='ms-formlabel'><H2 class='ms-standardheader'><nobr>"));
                this.Controls.Add(grvCorrespondenciaEP);
                this.Controls.Add(new LiteralControl("</nobr></H2></td></tr>"));

                this.Controls.Add(new LiteralControl("</table>"));
                #endregion
            }
            catch (Exception ex)
            {
                Literal error = new Literal();
                error.Text = ex.Message;

                this.Controls.Clear();
                this.Controls.Add(error);
            }
        }
    }
}
