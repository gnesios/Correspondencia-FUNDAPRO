using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using System.Web.UI;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace FPC_WPNuevaCorrespondencia
{
    public class FormNuevaCorrespondenciaEP_CB : WebPart
    {
        //const string LISTA_OBJETIVO = "Correspondencia de Entrada Educa-Pro (CB)";
        //const string LISTA_REDIRIGIDA = "/Lists/Correspondencia%20de%20Entrada%20EducaPro%20CB/";
        const string PAGINA_IMPRESION = "/_layouts/PaginasFPC/printEP_CB.aspx?";

        #region Definicion de controles
        Panel pnlFormulario;
        Panel pnlAdjuntos;

        Label lblTipoCarta;
        Label lblDescTipoCarta;
        RadioButtonList rblTipoCarta;
        RequiredFieldValidator rfvTipoCarta;

        Label lblOrigenCarta;
        Label lblDescOrigenCarta;
        RadioButtonList rblOrigenCarta;
        DropDownList ddlOrigenCarta;
        TextBox txbOrigenCarta;
        CustomValidator ctvOrigenCarta;
        //CustomValidator ctvDdlOrigencarta;
        //CustomValidator ctvTxbOrigencarta;
        //RequiredFieldValidator rfvDdlOrigenCarta;
        //RequiredFieldValidator rfvTxbOrigenCarta;

        Label lblReferencia;
        Label lblDescReferencia;
        InputFormTextBox txbReferencia;
        RequiredFieldValidator rfvReferencia;

        Label lblFechaCarta;
        Label lblDescFechaCarta;
        DateTimeControl dtcFechaCarta;

        Label lblFechaRecibida;
        Label lblDescFechaRecibida;
        DateTimeControl dtcFechaRecibida;

        Label lblDestinatario;
        Label lblDescDestinatario;
        TextBox txbDestinatario;
        RequiredFieldValidator rfvDestinatario;

        Label lblDirigidaA;
        Label lblDescDirigidaA;
        PeopleEditor pedDirigidaA;
        RequiredFieldValidator rfvDirigidaA;

        Label lblNumCarta;
        Label lblDescNumCarta;
        TextBox txbNumCarta;

        Label lblAdjunto;
        Label lblDescAdjunto;
        TextBox txbAdjunto;
        RequiredFieldValidator rfvAdjunto;

        Label lblClase;
        Label lblDescClase;
        TextBox txbClase;
        RequiredFieldValidator rfvClase;

        Label lblPrioridad;
        Label lblDescPrioridad;
        DropDownList ddlPrioridad;
        
        //Label lblProveidoCarta;
        //Label lblDescProveidoCarta;
        //InputFormTextBox txbProveidoCarta;

        Label lblPrivada;
        Label lblDescPrivada;
        CheckBox chkPrivada;

        Label lblHojaDeRuta;
        Label lblDescHojaDeRuta;
        CheckBox chkHojaDeRuta;

        //Label lblRecibida;
        //Label lblDescRecibida;
        //CheckBox chkRecibida;

        Label lblArchivo;
        Label lblDescArchivo;
        InputFormTextBox txbArchivo;

        Label lblAdjuntos;
        Label lblDescAdjuntos;
        GridView grvAdjuntos;

        LinkButton btnAdjuntarArchivos;
        Button btnFinalizarRegistro;
        //Button btnIrAtras;
        GoBackButton btnCancelar;
        #endregion

        protected override void CreateChildControls()
        {
            try
            {
                #region Creacion de controles
                pnlFormulario = new Panel();
                pnlFormulario.ID = "pnlFormulario";
                pnlAdjuntos = new Panel();
                pnlAdjuntos.ID = "pnlAdjuntos";
                pnlAdjuntos.Visible = false;

                lblTipoCarta = new Label();
                lblTipoCarta.Text = "Tipo <span class='ms-formvalidation'>*</span>";
                lblDescTipoCarta = new Label();
                lblDescTipoCarta.Text = "Tipo de correspondencia recibida";
                rblTipoCarta = new RadioButtonList();
                rblTipoCarta.ID = "rblTipoCarta";
                rblTipoCarta.Items.Add("INTERNA");
                rblTipoCarta.Items.Add("EXTERNA");
                rfvTipoCarta = new RequiredFieldValidator();
                rfvTipoCarta.ID = "rfvTipoCarta";
                rfvTipoCarta.Text = "<br/>Tiene que especificar un valor para este campo requerido.";
                rfvTipoCarta.Display = ValidatorDisplay.Dynamic;
                rfvTipoCarta.ControlToValidate = "rblTipoCarta";
                rfvTipoCarta.SetFocusOnError = true;

                lblOrigenCarta = new Label();
                lblOrigenCarta.Text = "Origen <span class='ms-formvalidation'>*</span>";
                lblDescOrigenCarta = new Label();
                lblDescOrigenCarta.Text = "Origen de la correspondencia";
                rblOrigenCarta = new RadioButtonList();
                rblOrigenCarta.ID = "rblOrigenCarta";
                rblOrigenCarta.Items.Add(new ListItem("", "COMBO"));
                rblOrigenCarta.Items.Add(new ListItem("", "TEXTO"));
                rblOrigenCarta.SelectedIndex = 0;
                ddlOrigenCarta = new DropDownList();
                ddlOrigenCarta.ID = "ddlOrigenCarta";
                ddlOrigenCarta.Attributes.Add("style", "width:385px;");
                ddlOrigenCarta.DataSource = ConectorWebPart.RecuperarOrigenesCorrespondencia();
                ddlOrigenCarta.DataTextField = "text";
                ddlOrigenCarta.DataValueField = "value";
                ddlOrigenCarta.DataBind();
                ddlOrigenCarta.Items.Insert(0, new ListItem("", string.Empty));
                ddlOrigenCarta.SelectedIndex = 0;
                txbOrigenCarta = new TextBox();
                txbOrigenCarta.ID = "txbOrigenCarta";
                txbOrigenCarta.Attributes.Add("style", "width:385px;");
                ctvOrigenCarta = new CustomValidator();
                ctvOrigenCarta.ID = "ctvOrigenCarta";
                ctvOrigenCarta.Text = "<br/>Tiene que especificar un valor para este campo requerido.";
                ctvOrigenCarta.Display = ValidatorDisplay.Dynamic;
                ctvOrigenCarta.ControlToValidate = "rblOrigenCarta";
                ctvOrigenCarta.ServerValidate += new ServerValidateEventHandler(ctvOrigenCarta_ServerValidate);
                //rfvDdlOrigenCarta = new RequiredFieldValidator();
                //rfvDdlOrigenCarta.ID = "rfvDdlOrigenCarta";
                //rfvDdlOrigenCarta.Text = "<br/>Tiene que especificar un valor para este campo requerido.";
                //rfvDdlOrigenCarta.InitialValue = string.Empty;
                //rfvDdlOrigenCarta.Display = ValidatorDisplay.Dynamic;
                //rfvDdlOrigenCarta.ControlToValidate = "ddlOrigenCarta";
                //rfvDdlOrigenCarta.Enabled = false;
                //rfvTxbOrigenCarta = new RequiredFieldValidator();
                //rfvTxbOrigenCarta.ID = "rfvTxbOrigenCarta";
                //rfvTxbOrigenCarta.Text = "<br/>Tiene que especificar un valor para este campo requerido.";
                //rfvTxbOrigenCarta.Display = ValidatorDisplay.Dynamic;
                //rfvTxbOrigenCarta.ControlToValidate = "txbOrigenCarta";
                //rfvTxbOrigenCarta.Enabled = false;

                lblReferencia = new Label();
                lblReferencia.Text = "Referencia <span class='ms-formvalidation'>*</span>";
                lblDescReferencia = new Label();
                lblDescReferencia.Text = "<br/>Referencia de la carta";
                txbReferencia = new InputFormTextBox();
                txbReferencia.ID = "txbReferencia";
                txbReferencia.Attributes.Add("style", "width:385px;");
                txbReferencia.RichText = false;
                txbReferencia.RichTextMode = SPRichTextMode.Compatible;
                txbReferencia.Rows = 5;
                txbReferencia.TextMode = TextBoxMode.MultiLine;
                rfvReferencia = new RequiredFieldValidator();
                rfvReferencia.ID = "rfvReferencia";
                rfvReferencia.Text = "<br/>Tiene que especificar un valor para este campo requerido.";
                rfvReferencia.Display = ValidatorDisplay.Dynamic;
                rfvReferencia.ControlToValidate = "txbReferencia";
                rfvReferencia.SetFocusOnError = true;

                lblFechaCarta = new Label();
                lblFechaCarta.Text = "Fecha origen <span class='ms-formvalidation'>*</span>";
                lblDescFechaCarta = new Label();
                lblDescFechaCarta.Text = "Fecha de la correspondencia";
                dtcFechaCarta = new DateTimeControl();
                dtcFechaCarta.ID = "dtcFechaCarta";
                dtcFechaCarta.IsRequiredField = true;
                dtcFechaCarta.DateOnly = true;

                lblFechaRecibida = new Label();
                lblFechaRecibida.Text = "Fecha recibida <span class='ms-formvalidation'>*</span>";
                lblDescFechaRecibida = new Label();
                lblDescFechaRecibida.Text = "Fecha de recepción de la carta";
                dtcFechaRecibida = new DateTimeControl();
                dtcFechaRecibida.ID = "dtcFechaRecibida";
                dtcFechaRecibida.IsRequiredField = true;
                dtcFechaRecibida.SelectedDate = DateTime.Now;

                lblDestinatario = new Label();
                lblDestinatario.Text = "Destinatario <span class='ms-formvalidation'>*</span>";
                lblDescDestinatario = new Label();
                lblDescDestinatario.Text = "<br/>Destinatario indicado en la carta";
                txbDestinatario = new TextBox();
                txbDestinatario.ID = "txbDestinatario";
                txbDestinatario.Attributes.Add("style", "width:385px;");
                rfvDestinatario = new RequiredFieldValidator();
                rfvDestinatario.ID = "rfvDestinatario";
                rfvDestinatario.Text = "<br/>Tiene que especificar un valor para este campo requerido.";
                rfvDestinatario.Display = ValidatorDisplay.Dynamic;
                rfvDestinatario.ControlToValidate = "txbDestinatario";
                rfvDestinatario.SetFocusOnError = true;

                lblDirigidaA = new Label();
                lblDirigidaA.Text = "Dirigida a <span class='ms-formvalidation'>*</span>";
                lblDescDirigidaA = new Label();
                lblDescDirigidaA.Text = "Usuario(s) al(os) cual(es) será enviada la notificación de correspondencia. El primero usuario definido en este campo será el dueño de esta correspondencia.";
                pedDirigidaA = new PeopleEditor();
                pedDirigidaA.ID = "pedDirigidaA";
                pedDirigidaA.AllowEmpty = false;
                pedDirigidaA.MultiSelect = true;
                pedDirigidaA.Rows = 1;
                pedDirigidaA.PlaceButtonsUnderEntityEditor = false;
                rfvDirigidaA = new RequiredFieldValidator();
                rfvDirigidaA.ID = "rfvDirigidaA";
                rfvDirigidaA.Text = "<br/>Tiene que especificar un valor para este campo requerido.";
                rfvDirigidaA.Display = ValidatorDisplay.Dynamic;
                rfvDirigidaA.ControlToValidate = "pedDirigidaA";
                rfvDirigidaA.SetFocusOnError = true;

                lblNumCarta = new Label();
                lblNumCarta.Text = "Num. ó Cite";
                lblDescNumCarta = new Label();
                lblDescNumCarta.Text = "<br/>Código de indentificación de la carta";
                txbNumCarta = new TextBox();
                txbNumCarta.ID = "txbNumCarta";
                txbNumCarta.Attributes.Add("style", "width:385px;");

                lblAdjunto = new Label();
                lblAdjunto.Text = "Adjunto <span class='ms-formvalidation'>*</span>";
                lblDescAdjunto = new Label();
                lblDescAdjunto.Text = "<br/>Indica si la correspondencia trae documentos adjuntos o no";
                txbAdjunto = new TextBox();
                txbAdjunto.ID = "txbAdjunto";
                txbAdjunto.Attributes.Add("style", "width:385px;");
                rfvAdjunto = new RequiredFieldValidator();
                rfvAdjunto.ID = "rfvAdjunto";
                rfvAdjunto.Text = "<br/>Tiene que especificar un valor para este campo requerido.";
                rfvAdjunto.Display = ValidatorDisplay.Dynamic;
                rfvAdjunto.ControlToValidate = "txbAdjunto";
                rfvAdjunto.SetFocusOnError = true;

                lblClase = new Label();
                lblClase.Text = "Clase de documento <span class='ms-formvalidation'>*</span>";
                lblDescClase = new Label();
                lblDescClase.Text = "";
                txbClase = new TextBox();
                txbClase.ID = "txbClase";
                txbClase.Attributes.Add("style", "width:385px;");
                rfvClase = new RequiredFieldValidator();
                rfvClase.ID = "rfvClase";
                rfvClase.Text = "<br/>Tiene que especificar un valor para este campo requerido.";
                rfvClase.Display = ValidatorDisplay.Dynamic;
                rfvClase.ControlToValidate = "txbClase";
                rfvClase.SetFocusOnError = true;

                lblPrioridad = new Label();
                lblPrioridad.Text = "Prioridad <span class='ms-formvalidation'>*</span>";
                lblDescPrioridad = new Label();
                lblDescPrioridad.Text = "<br/>Prioridad de la correspondencia";
                ddlPrioridad = new DropDownList();
                ddlPrioridad.ID = "ddlPrioridad";
                ddlPrioridad.Attributes.Add("style", "width:150px;");
                ddlPrioridad.Items.Add("NORMAL");
                ddlPrioridad.Items.Add("URGENTE");
                ddlPrioridad.SelectedIndex = 0;

                lblPrivada = new Label();
                lblPrivada.Text = "Privada";
                lblDescPrivada = new Label();
                lblDescPrivada.Text = "<br/>Si se marca, esta carta será leida solo por el(los) usuario(s) indicado(s) en el campo \"Dirigida a\"";
                chkPrivada = new CheckBox();
                chkPrivada.ID = "chkPrivada";
                chkPrivada.Checked = true;

                lblHojaDeRuta = new Label();
                lblHojaDeRuta.Text = "Hoja de ruta";
                lblDescHojaDeRuta = new Label();
                lblDescHojaDeRuta.Text = "<br/>Si se marca, imprime la hoja de ruta";
                chkHojaDeRuta = new CheckBox();
                chkHojaDeRuta.ID = "chkHojaDeRuta";
                chkHojaDeRuta.Checked = true;

                lblArchivo = new Label();
                lblArchivo.Text = "Archivo";
                lblDescArchivo = new Label();
                lblDescArchivo.Text = "<br/>Descripción de la ubicación física final de la correspondencia";
                txbArchivo = new InputFormTextBox();
                txbArchivo.ID = "txbArchivo";
                txbArchivo.Attributes.Add("style", "width:385px;");
                txbArchivo.RichText = false;
                txbArchivo.RichTextMode = SPRichTextMode.Compatible;
                txbArchivo.Rows = 5;
                txbArchivo.TextMode = TextBoxMode.MultiLine;

                lblAdjuntos = new Label();
                lblAdjuntos.Text = "Adjuntos";
                lblDescAdjuntos = new Label();
                lblDescAdjuntos.Text = "Seleccione el o los archivos que desea adjuntar a este registro de correspondencia";
                grvAdjuntos = new GridView();
                grvAdjuntos.ID = "grvAdjuntos";
                grvAdjuntos.GridLines = GridLines.None;
                grvAdjuntos.ForeColor = Color.FromArgb(51, 51, 51);
                grvAdjuntos.CellPadding = 4;
                grvAdjuntos.AutoGenerateColumns = false;
                grvAdjuntos.DataKeyNames = new string[] { "RutaArchivo" };
                grvAdjuntos.RowStyle.BackColor = Color.FromArgb(227, 234, 235);
                //grvAdjuntos.HeaderStyle.BackColor = Color.FromArgb(28, 94, 85);
                grvAdjuntos.HeaderStyle.Font.Bold = true;
                //grvAdjuntos.HeaderStyle.ForeColor = Color.FromArgb(255, 255, 255);
                grvAdjuntos.AlternatingRowStyle.BackColor = Color.White;
                grvAdjuntos.Width = Unit.Percentage(100);
                //grvAdjuntos.RowDataBound += new GridViewRowEventHandler(grvAdjuntos_RowDataBound);
                
                #region Adicion de columnas al Grid
                TemplateField chkAdjuntar = new TemplateField();
                CheckBoxTemplate chkBox = new CheckBoxTemplate();
                chkAdjuntar.ItemTemplate = chkBox;

                BoundField bflNombreArchivo = new BoundField();
                bflNombreArchivo.HeaderText = "Nombre archivo";
                bflNombreArchivo.DataField = "NombreArchivo";

                BoundField bflTipoArchivo = new BoundField();
                bflTipoArchivo.HeaderText = "Tipo";
                bflTipoArchivo.DataField = "TipoArchivo";

                ImageField imfVistaPrevia = new ImageField();
                imfVistaPrevia.HeaderText = "Vista Previa";
                imfVistaPrevia.DataImageUrlField = "VistaPrevia";

                BoundField bflRutaArchivo = new BoundField();
                bflRutaArchivo.HeaderText = "URL";
                bflRutaArchivo.DataField = "RutaArchivo";
                bflRutaArchivo.Visible = false;

                grvAdjuntos.Columns.Add(chkAdjuntar);
                grvAdjuntos.Columns.Add(bflNombreArchivo);
                grvAdjuntos.Columns.Add(bflTipoArchivo);
                grvAdjuntos.Columns.Add(imfVistaPrevia);
                grvAdjuntos.Columns.Add(bflRutaArchivo);

                grvAdjuntos.DataSource = ConectorWebPart.RecuperarDocumentosEP().Tables["DataTable"];
                grvAdjuntos.DataBind();
                #endregion

                btnAdjuntarArchivos = new LinkButton();
                btnAdjuntarArchivos.ID = "btnAdjuntarArchivos";
                btnAdjuntarArchivos.Text = "Adjuntar Archivos";
                btnAdjuntarArchivos.ToolTip = "Ver el panel de adjuntar archivos.";
                btnAdjuntarArchivos.Attributes.Add("style", "font-size:8.5pt;");
                btnAdjuntarArchivos.Click += new EventHandler(btnAdjuntarArchivos_Click);
                btnAdjuntarArchivos.CausesValidation = false; //OJO
                btnFinalizarRegistro = new Button();
                btnFinalizarRegistro.ID = "btnFinalizarRegistro";
                btnFinalizarRegistro.Text = "Finalizar";
                btnFinalizarRegistro.ToolTip = "Finalizar el registro de nueva correspondencia.";
                btnFinalizarRegistro.Visible = true;
                btnFinalizarRegistro.Attributes.Add("style", "width:140px; font-size:8.5pt;");
                btnFinalizarRegistro.Click += new EventHandler(btnFinalizarRegistro_Click);
                //btnIrAtras = new Button();
                //btnIrAtras.ID = "btnIrAtras";
                //btnIrAtras.Text = "Ir Atras";
                //btnIrAtras.ToolTip = "Volver al formulario de registro";
                //btnIrAtras.Visible = false;
                //btnIrAtras.Attributes.Add("style", "width:140px; font-size:8.5pt;");
                //btnIrAtras.Click += new EventHandler(btnIrAtras_Click);
                btnCancelar = new GoBackButton();
                btnCancelar.ID = "btnCancelar";
                btnCancelar.ControlMode = SPControlMode.New;
                #endregion

                #region Adiccion de controles
                pnlFormulario.Controls.Add(new LiteralControl("<table border='0' cellspacing='0' width='100%'>"));
                pnlFormulario.Controls.Add(new LiteralControl("<tr><td colspan='2' style='border-bottom:1px black solid'><b>Datos de la Correspondencia</b></td></tr>"));

                pnlFormulario.Controls.Add(new LiteralControl("<tr><td width='190px' valign='top' class='ms-formlabel'><H3 class='ms-standardheader'><nobr>"));
                pnlFormulario.Controls.Add(lblTipoCarta);
                pnlFormulario.Controls.Add(new LiteralControl("</nobr></H3></td><td width='500px' valign='top' class='ms-formbody'>"));
                pnlFormulario.Controls.Add(rblTipoCarta);
                pnlFormulario.Controls.Add(lblDescTipoCarta);
                pnlFormulario.Controls.Add(rfvTipoCarta);
                pnlFormulario.Controls.Add(new LiteralControl("</td></tr>"));

                pnlFormulario.Controls.Add(new LiteralControl("<tr><td width='190px' valign='top' class='ms-formlabel'><H3 class='ms-standardheader'><nobr>"));
                pnlFormulario.Controls.Add(lblOrigenCarta);
                pnlFormulario.Controls.Add(new LiteralControl("</nobr></H3></td><td width='500px' valign='top' class='ms-formbody'>"));
                pnlFormulario.Controls.Add(new LiteralControl("<table><tr><td>"));
                pnlFormulario.Controls.Add(rblOrigenCarta);
                pnlFormulario.Controls.Add(new LiteralControl("</td><td>"));
                pnlFormulario.Controls.Add(ddlOrigenCarta);
                pnlFormulario.Controls.Add(new LiteralControl("<br/>"));
                pnlFormulario.Controls.Add(txbOrigenCarta);
                pnlFormulario.Controls.Add(new LiteralControl("</td></tr></table>"));
                pnlFormulario.Controls.Add(lblDescOrigenCarta);
                pnlFormulario.Controls.Add(ctvOrigenCarta);
                pnlFormulario.Controls.Add(new LiteralControl("</td></tr>"));

                pnlFormulario.Controls.Add(new LiteralControl("<tr><td width='190px' valign='top' class='ms-formlabel'><H3 class='ms-standardheader'><nobr>"));
                pnlFormulario.Controls.Add(lblReferencia);
                pnlFormulario.Controls.Add(new LiteralControl("</nobr></H3></td><td width='500px' valign='top' class='ms-formbody'>"));
                pnlFormulario.Controls.Add(txbReferencia);
                pnlFormulario.Controls.Add(lblDescReferencia);
                pnlFormulario.Controls.Add(rfvReferencia);
                pnlFormulario.Controls.Add(new LiteralControl("</td></tr>"));

                pnlFormulario.Controls.Add(new LiteralControl("<tr><td width='190px' valign='top' class='ms-formlabel'><H3 class='ms-standardheader'><nobr>"));
                pnlFormulario.Controls.Add(lblFechaCarta);
                pnlFormulario.Controls.Add(new LiteralControl("</nobr></H3></td><td width='500px' valign='top' class='ms-formbody'>"));
                pnlFormulario.Controls.Add(dtcFechaCarta);
                pnlFormulario.Controls.Add(lblDescFechaCarta);
                pnlFormulario.Controls.Add(new LiteralControl("</td></tr>"));

                pnlFormulario.Controls.Add(new LiteralControl("<tr><td width='190px' valign='top' class='ms-formlabel'><H3 class='ms-standardheader'><nobr>"));
                pnlFormulario.Controls.Add(lblFechaRecibida);
                pnlFormulario.Controls.Add(new LiteralControl("</nobr></H3></td><td width='500px' valign='top' class='ms-formbody'>"));
                pnlFormulario.Controls.Add(dtcFechaRecibida);
                pnlFormulario.Controls.Add(lblDescFechaRecibida);
                pnlFormulario.Controls.Add(new LiteralControl("</td></tr>"));

                pnlFormulario.Controls.Add(new LiteralControl("<tr><td width='190px' valign='top' class='ms-formlabel'><H3 class='ms-standardheader'><nobr>"));
                pnlFormulario.Controls.Add(lblDestinatario);
                pnlFormulario.Controls.Add(new LiteralControl("</nobr></H3></td><td width='500px' valign='top' class='ms-formbody'>"));
                pnlFormulario.Controls.Add(txbDestinatario);
                pnlFormulario.Controls.Add(lblDescDestinatario);
                pnlFormulario.Controls.Add(rfvDestinatario);
                pnlFormulario.Controls.Add(new LiteralControl("</td></tr>"));

                pnlFormulario.Controls.Add(new LiteralControl("<tr><td width='190px' valign='top' class='ms-formlabel'><H3 class='ms-standardheader'><nobr>"));
                pnlFormulario.Controls.Add(lblDirigidaA);
                pnlFormulario.Controls.Add(new LiteralControl("</nobr></H3></td><td width='500px' valign='top' class='ms-formbody'>"));
                pnlFormulario.Controls.Add(pedDirigidaA);
                pnlFormulario.Controls.Add(lblDescDirigidaA);
                pnlFormulario.Controls.Add(rfvDirigidaA);
                pnlFormulario.Controls.Add(new LiteralControl("</td></tr>"));

                pnlFormulario.Controls.Add(new LiteralControl("<tr><td width='190px' valign='top' class='ms-formlabel'><H3 class='ms-standardheader'><nobr>"));
                pnlFormulario.Controls.Add(lblNumCarta);
                pnlFormulario.Controls.Add(new LiteralControl("</nobr></H3></td><td width='500px' valign='top' class='ms-formbody'>"));
                pnlFormulario.Controls.Add(txbNumCarta);
                pnlFormulario.Controls.Add(lblDescNumCarta);
                pnlFormulario.Controls.Add(new LiteralControl("</td></tr>"));

                pnlFormulario.Controls.Add(new LiteralControl("<tr><td width='190px' valign='top' class='ms-formlabel'><H3 class='ms-standardheader'><nobr>"));
                pnlFormulario.Controls.Add(lblAdjunto);
                pnlFormulario.Controls.Add(new LiteralControl("</nobr></H3></td><td width='500px' valign='top' class='ms-formbody'>"));
                pnlFormulario.Controls.Add(txbAdjunto);
                pnlFormulario.Controls.Add(lblDescAdjunto);
                pnlFormulario.Controls.Add(rfvAdjunto);
                pnlFormulario.Controls.Add(new LiteralControl("</td></tr>"));

                pnlFormulario.Controls.Add(new LiteralControl("<tr><td width='190px' valign='top' class='ms-formlabel'><H3 class='ms-standardheader'><nobr>"));
                pnlFormulario.Controls.Add(lblClase);
                pnlFormulario.Controls.Add(new LiteralControl("</nobr></H3></td><td width='500px' valign='top' class='ms-formbody'>"));
                pnlFormulario.Controls.Add(txbClase);
                pnlFormulario.Controls.Add(lblDescClase);
                pnlFormulario.Controls.Add(rfvClase);
                pnlFormulario.Controls.Add(new LiteralControl("</td></tr>"));

                pnlFormulario.Controls.Add(new LiteralControl("<tr><td width='190px' valign='top' class='ms-formlabel'><H3 class='ms-standardheader'><nobr>"));
                pnlFormulario.Controls.Add(lblPrioridad);
                pnlFormulario.Controls.Add(new LiteralControl("</nobr></H3></td><td width='500px' valign='top' class='ms-formbody'>"));
                pnlFormulario.Controls.Add(ddlPrioridad);
                pnlFormulario.Controls.Add(lblDescPrioridad);
                pnlFormulario.Controls.Add(new LiteralControl("</td></tr>"));

                pnlFormulario.Controls.Add(new LiteralControl("<tr><td width='190px' valign='top' class='ms-formlabel'><H3 class='ms-standardheader'><nobr>"));
                pnlFormulario.Controls.Add(lblPrivada);
                pnlFormulario.Controls.Add(new LiteralControl("</nobr></H3></td><td width='500px' valign='top' class='ms-formbody'>"));
                pnlFormulario.Controls.Add(chkPrivada);
                pnlFormulario.Controls.Add(lblDescPrivada);
                pnlFormulario.Controls.Add(new LiteralControl("</td></tr>"));

                pnlFormulario.Controls.Add(new LiteralControl("<tr><td width='190px' valign='top' class='ms-formlabel'><H3 class='ms-standardheader'><nobr>"));
                pnlFormulario.Controls.Add(lblHojaDeRuta);
                pnlFormulario.Controls.Add(new LiteralControl("</nobr></H3></td><td width='500px' valign='top' class='ms-formbody'>"));
                pnlFormulario.Controls.Add(chkHojaDeRuta);
                pnlFormulario.Controls.Add(lblDescHojaDeRuta);
                pnlFormulario.Controls.Add(new LiteralControl("</td></tr>"));

                pnlFormulario.Controls.Add(new LiteralControl("<tr><td width='190px' valign='top' class='ms-formlabel'><H3 class='ms-standardheader'><nobr>"));
                pnlFormulario.Controls.Add(lblArchivo);
                pnlFormulario.Controls.Add(new LiteralControl("</nobr></H3></td><td width='500px' valign='top' class='ms-formbody'>"));
                pnlFormulario.Controls.Add(txbArchivo);
                pnlFormulario.Controls.Add(lblDescArchivo);
                pnlFormulario.Controls.Add(new LiteralControl("</td></tr>"));
                pnlFormulario.Controls.Add(new LiteralControl("</table>"));

                pnlAdjuntos.Controls.Add(new LiteralControl("<table border='0' cellspacing='0' width='100%'>"));
                pnlAdjuntos.Controls.Add(new LiteralControl("<tr><td colspan='2' style='border-bottom:1px black solid'><b>Archivos Adjuntos</b></td></tr>"));
                pnlAdjuntos.Controls.Add(new LiteralControl("<tr><td width='190px' valign='top' class='ms-formlabel'><H3 class='ms-standardheader'><nobr>"));
                pnlAdjuntos.Controls.Add(lblAdjuntos);
                pnlAdjuntos.Controls.Add(new LiteralControl("</nobr></H3></td><td width='500px' valign='top' class='ms-formbody'>"));
                pnlAdjuntos.Controls.Add(lblDescAdjuntos);
                pnlAdjuntos.Controls.Add(grvAdjuntos);
                pnlAdjuntos.Controls.Add(new LiteralControl("</td></tr>"));
                pnlAdjuntos.Controls.Add(new LiteralControl("</table>"));

                this.Controls.Add(pnlFormulario);
                this.Controls.Add(pnlAdjuntos);
                this.Controls.Add(new LiteralControl("<table border='0' cellspacing='0' width='100%'>"));
                this.Controls.Add(new LiteralControl("<tr><td style='text-align:right' class='ms-toolbar'>"));
                this.Controls.Add(new LiteralControl("<table><tr><td width='99%' class='ms-toolbar'><IMG SRC='/_layouts/images/blank.gif' width='1' height='18'/></td>"));
                this.Controls.Add(new LiteralControl("<td nowrap='nowrap' class='ms-toolbar'>"));
                this.Controls.Add(btnAdjuntarArchivos);
                this.Controls.Add(new LiteralControl("</td><td class='ms-separator'> </td><td class='ms-toolbar' align='right'>"));
                this.Controls.Add(btnFinalizarRegistro);
                this.Controls.Add(new LiteralControl("</td><td class='ms-separator'> </td><td class='ms-toolbar' align='right'>"));
                this.Controls.Add(btnCancelar);
                //this.Controls.Add(btnIrAtras);
                this.Controls.Add(new LiteralControl("</td></tr></table>"));
                this.Controls.Add(new LiteralControl("</td></tr>"));
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

        void ctvOrigenCarta_ServerValidate(object source, ServerValidateEventArgs args)
        {
            //if (args.Value == "TEXTO")
            if (rblOrigenCarta.SelectedValue == "COMBO")
            {
                if (!string.IsNullOrEmpty(ddlOrigenCarta.SelectedValue))
                    args.IsValid = true;
                else
                    args.IsValid = false;
            }
            else
            {
                if (!string.IsNullOrEmpty(txbOrigenCarta.Text))
                    args.IsValid = true;
                else
                    args.IsValid = false;
            }
        }

        void grvAdjuntos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            //if (e.Row.RowType == DataControlRowType.DataRow)
            //{
            //    HyperLink hLink = new HyperLink();
            //    hLink.Text = e.Row.Cells[1].Text;
            //    hLink.NavigateUrl = "file://localhost/" + e.Row.Cells[4].Text;
            //    e.Row.Cells[1].Controls.Add(hLink);
            //}
        }

        //void btnIrAtras_Click(object sender, EventArgs e)
        //{
        //    pnlFormulario.Visible = true;
        //    btnAdjuntarArchivos.Visible = true;
        //    btnCancelar.Visible = true;
        //    pnlAdjuntos.Visible = false;
        //    btnFinalizarRegistro.Visible = false;
        //    btnIrAtras.Visible = false;
        //}

        void btnFinalizarRegistro_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                // "/Lists/Correspondencia de Entrada EducaPro SC/NewFormEP.aspx"
                string url = this.Page.Request.Url.LocalPath;
                string urlLista = url.Remove(url.LastIndexOf('/'));

                string tipoCarta = rblTipoCarta.SelectedValue;
                int origenCartaVal = 0;
                string origenCartaTex = "";
                string referencia = txbReferencia.Text;
                DateTime fechaCarta = dtcFechaCarta.SelectedDate;
                DateTime fechaRecibida = dtcFechaRecibida.SelectedDate;
                string destinatario = txbDestinatario.Text;
                ArrayList dirigidaA = pedDirigidaA.Accounts;
                string numCarta = txbNumCarta.Text;
                string adjunto = txbAdjunto.Text;
                string clase = txbClase.Text;
                string prioridad = ddlPrioridad.SelectedValue;
                bool privada = chkPrivada.Checked;
                bool hojaRuta = chkHojaDeRuta.Checked;
                string archivo = txbArchivo.Text;

                if (rblOrigenCarta.SelectedValue == "COMBO")
                {
                    origenCartaVal = Convert.ToInt16(ddlOrigenCarta.SelectedItem.Value);
                    origenCartaTex = ddlOrigenCarta.SelectedItem.Text;
                }
                else if (rblOrigenCarta.SelectedValue == "TEXTO")
                {
                    //origenCarta = txbOrigenCarta.Text.ToUpper();
                    origenCartaVal = ConectorWebPart.InsertarNuevoOrigen(txbOrigenCarta.Text.ToUpper());
                    origenCartaTex = txbOrigenCarta.Text.ToUpper();
                }

                int idCarta = ConectorWebPart.GuardarNuevoRegistro(tipoCarta, origenCartaVal,
                    referencia, fechaCarta, fechaRecibida, destinatario, dirigidaA, numCarta,
                    adjunto, clase, prioridad, privada, hojaRuta, archivo,
                    this.ArchivosSeleccionados(), urlLista);

                ConectorWebPart.EliminarArchivosAdjuntados(this.ArchivosSeleccionados());

                if (hojaRuta)
                {
                    referencia = referencia.Replace("\r\n", " ");
                    string parametros = string.Format("id={0}&fr={1}&fcc={2}&dt={3}&rf={4}&or={5}&url={6}",
                        idCarta.ToString(), fechaRecibida.ToShortDateString(),
                        fechaCarta.ToShortDateString() + " - " + numCarta, destinatario, referencia,
                        origenCartaTex, urlLista + "/");
                    this.Page.Response.Redirect(PAGINA_IMPRESION + parametros);
                }
                else
                {
                    this.Page.Response.Redirect(urlLista);
                }
            }
        }

        void btnAdjuntarArchivos_Click(object sender, EventArgs e)
        {
            if (rfvDirigidaA.IsValid)
            {
                //btnAdjuntarArchivos.Visible = false;
                pnlAdjuntos.Visible = true;
                btnFinalizarRegistro.Focus();
            }
        }

        /// <summary>
        /// Consolida las rutas de los archivos seleccionados del Grid
        /// </summary>
        /// <returns></returns>
        private List<string> ArchivosSeleccionados()
        {
            List<string> rutas =new List<string>();

            foreach (GridViewRow row in grvAdjuntos.Rows)
            {
                CheckBox chkBox = (CheckBox)row.FindControl("chkBox");

                if (chkBox != null && chkBox.Checked)
                    rutas.Add(grvAdjuntos.DataKeys[row.RowIndex].Value.ToString());
            }

            return rutas;
        }
    }
}
