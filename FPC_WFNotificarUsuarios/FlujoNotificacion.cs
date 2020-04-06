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
using Microsoft.SharePoint.Workflow;
using Microsoft.SharePoint;
using System.Web.UI.WebControls;
using System.Globalization;

namespace FPC_WFNotificarUsuarios
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
            lanzarCorre = true;

            InitializeComponent();
        }

        public static DependencyProperty workflowPropertiesProperty = DependencyProperty.Register("workflowProperties", typeof(Microsoft.SharePoint.Workflow.SPWorkflowActivationProperties), typeof(FPC_WFNotificarUsuarios.FlujoNotificacion));

        [DesignerSerializationVisibilityAttribute(DesignerSerializationVisibility.Visible)]
        [BrowsableAttribute(true)]
        [CategoryAttribute("Misc")]
        public Microsoft.SharePoint.Workflow.SPWorkflowActivationProperties workflowProperties
        {
            get
            {
                return ((Microsoft.SharePoint.Workflow.SPWorkflowActivationProperties)(base.GetValue(FPC_WFNotificarUsuarios.FlujoNotificacion.workflowPropertiesProperty)));
            }
            set
            {
                base.SetValue(FPC_WFNotificarUsuarios.FlujoNotificacion.workflowPropertiesProperty, value);
            }
        }

        private void RecuperarInformacion_ExecuteCode(object sender, EventArgs e)
        {
            itemObjetivo = workflowProperties.Item;

            try
            {
                #region Enviar o no enviar correo?
                if (itemObjetivo["DAC"] != null &&
                    (itemObjetivo["Dirigida a"].ToString() == itemObjetivo["DAC"].ToString()))
                {
                    lanzarCorre = false;
                }
                else
                {// por defecto es lanzarCorre = true;
                    itemObjetivo["DAC"] = itemObjetivo["Dirigida a"];

                    using (DisabledItemEventsScope scope = new DisabledItemEventsScope())
                    {
                        itemObjetivo.SystemUpdate();
                    }
                }
                #endregion

                SPFieldUserValueCollection usuarios =
                    (SPFieldUserValueCollection)itemObjetivo["Dirigida a"];

                foreach (SPFieldUserValue usuario in usuarios)
                {
                    usuariosNotificados = usuariosNotificados + usuario.User.Email + "; ";
                }

                asuntoNotificacion = "Correspondencia";
                cuerpoNotificacion = this.FormatoCuerpoCorreoNotificacion();

                mensajeHistorial = "Notificación a destinatario(s) " +
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
                "Correspondencia Recibida</span></p>" +
                "</td></tr><tr><td style='border:none;border-bottom:solid #9CA3AD 1.0pt;mso-border-top-alt:solid #E8EAEC .75pt;mso-border-top-alt:solid #E8EAEC " +
                ".75pt;mso-border-bottom-alt:solid #9CA3AD .75pt;padding:4.0pt 7.5pt 4.0pt 7.5pt'>" +
                "<p><span style='font-size:8.0pt;font-family:Tahoma,sans-serif;mso-fareast-font-family:Times New Roman'> " +
                "Usted ha recibido correspondencia, para revisarla presione el enlace siguiente: {0}<br />" +
                "El ID de la correspondendia es <b>{3}</b>.</span></p>" +
                "<p><span style='font-size:8.0pt;font-family:Tahoma,sans-serif;mso-fareast-font-family:Times New Roman'>Notificación enviada por {1} en fecha {2}</span></p>" +
                "</td></tr></table>",
                "<a href='" + url + "'>" + itemObjetivo["Origen"].ToString().Substring(itemObjetivo["Origen"].ToString().IndexOf('#') + 1) + "</a>",
                "<b>" + solicitante + "</b>",
                "<b>" + DateTime.Now.ToString("d", CultureInfo.CreateSpecificCulture("es-ES")) + "</b>",
                itemObjetivo.ID.ToString());

            return literal.Text;
        }
    }

}
