using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Linq;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Workflow;
using Microsoft.SharePoint.WorkflowActions;
using System.Web.UI.WebControls;
using System.Globalization;

namespace FPC_WFNotificarCambioEstado
{
    public sealed partial class FlujoNotificacion : SequentialWorkflowActivity
    {
        public string usuariosNotificados;
        public string asuntoNotificacion;
        public string cuerpoNotificacion;
        public string mensajeHistorial;
        public SPListItem itemObjetivo;
        public bool lanzarCorre;

        public FlujoNotificacion()
        {
            usuariosNotificados = "";
            asuntoNotificacion = "";
            cuerpoNotificacion = "";
            mensajeHistorial = "";
            itemObjetivo = null;
            lanzarCorre = false;

            InitializeComponent();
        }

        public Guid workflowId = default(System.Guid);
        public SPWorkflowActivationProperties workflowProperties = new SPWorkflowActivationProperties();

        private void RecuperarInformacion_ExecuteCode(object sender, EventArgs e)
        {
            itemObjetivo = workflowProperties.Item;

            try
            {
                #region Enviar o no enviar correo?
                if (itemObjetivo["Archivo"] != null &&
                    !string.IsNullOrEmpty(itemObjetivo["Archivo"].ToString().Trim()))
                {
                    lanzarCorre = true;
                }
                #endregion

                usuariosNotificados = workflowProperties.OriginatorEmail;
                asuntoNotificacion = "Correspondencia, cambio de estado";
                cuerpoNotificacion = this.FormatoCuerpoCorreoNotificacion();
                mensajeHistorial = "Notificación de cambio de estado a " +
                    usuariosNotificados + " realizada exitosamente.";
                ActualizarHistorialSiLanza.EventId = SPWorkflowHistoryEventType.WorkflowCompleted;
                ActualizarHistorialSiLanza.HistoryOutcome = "Completado";
            }
            catch (Exception ex)
            {
                ActualizarHistorialSiLanza.EventId = SPWorkflowHistoryEventType.WorkflowError;
                ActualizarHistorialSiLanza.HistoryOutcome = "Error";

                mensajeHistorial = ex.Message;
            }
        }

        /// <summary>
        /// Formatea el cuerpo del correo de notificacion a enviar
        /// </summary>
        /// <returns></returns>
        private string FormatoCuerpoCorreoNotificacion()
        {
            Literal literal = new Literal();

            //string url = itemObjetivo.Web.Url + "/" + itemObjetivo.Url.Remove(itemObjetivo.Url.LastIndexOf("/")) +
            //    "/DispForm.aspx?ID=" + itemObjetivo.ID.ToString();
            string url = itemObjetivo.Web.Url;
            string solicitante = itemObjetivo["Modificado por"].ToString().Substring(
                itemObjetivo["Modificado por"].ToString().IndexOf("#") + 1);

            literal.Text = string.Format(
                "<table border='0' cellspacing='0' cellpadding='0' width='100%' style='width:100%;border-collapse:collapse;mso-yfti-tbllook:1184'>" +
                "<tr><td style='border:solid #E8EAEC 1.0pt;mso-border-alt:solid #E8EAEC .75pt; background:#F8F8F9;padding:12.0pt 7.5pt 15.0pt 7.5pt'>" +
                "<p><span style='font-size:16.0pt;font-family:Verdana,sans-serif; mso-fareast-font-family:Times New Roman;mso-bidi-font-family:Tahoma'>" +
                "Cambio de Estado de Correspondencia</span></p>" +
                "</td></tr><tr><td style='border:none;border-bottom:solid #9CA3AD 1.0pt;mso-border-top-alt:solid #E8EAEC .75pt;mso-border-top-alt:solid #E8EAEC " +
                ".75pt;mso-border-bottom-alt:solid #9CA3AD .75pt;padding:4.0pt 7.5pt 4.0pt 7.5pt'>" +
                "<p><span style='font-size:8.0pt;font-family:Tahoma,sans-serif;mso-fareast-font-family:Times New Roman'> " +
                "La correspondendia con ID <b>{2}</b> ha cambiado de estado a <b>PASIVA</b>.</span></p>" +
                "<p><span style='font-size:8.0pt;font-family:Tahoma,sans-serif;mso-fareast-font-family:Times New Roman'>Cambio realizado por {0} en fecha {1}</span></p>" +
                "</td></tr></table>",
                "<b>" + solicitante + "</b>",
                "<b>" + DateTime.Now.ToString("d", CultureInfo.CreateSpecificCulture("es-ES")) + "</b>",
                itemObjetivo.ID.ToString());

            return literal.Text;
        }
    }
}
