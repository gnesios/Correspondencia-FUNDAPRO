using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections;
using System.Drawing;
using System.Reflection;
using System.Workflow.ComponentModel.Compiler;
using System.Workflow.ComponentModel.Serialization;
using System.Workflow.ComponentModel;
using System.Workflow.ComponentModel.Design;
using System.Workflow.Runtime;
using System.Workflow.Activities;
using System.Workflow.Activities.Rules;

namespace FPC_WFNotificarCambioEstado
{
    public sealed partial class FlujoNotificacion
    {
        #region Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCode]
        private void InitializeComponent()
        {
            this.CanModifyActivities = true;
            System.Workflow.ComponentModel.ActivityBind activitybind1 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.ComponentModel.ActivityBind activitybind2 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.Runtime.CorrelationToken correlationtoken1 = new System.Workflow.Runtime.CorrelationToken();
            System.Workflow.ComponentModel.ActivityBind activitybind3 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.ComponentModel.ActivityBind activitybind4 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.Activities.Rules.RuleConditionReference ruleconditionreference1 = new System.Workflow.Activities.Rules.RuleConditionReference();
            System.Workflow.ComponentModel.ActivityBind activitybind6 = new System.Workflow.ComponentModel.ActivityBind();
            System.Workflow.ComponentModel.ActivityBind activitybind5 = new System.Workflow.ComponentModel.ActivityBind();
            this.ActualizarHistorialNoLanza = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.ActualizarHistorialSiLanza = new Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity();
            this.EnviarNotificacion = new Microsoft.SharePoint.WorkflowActions.SendEmail();
            this.noLanzarCorreo = new System.Workflow.Activities.IfElseBranchActivity();
            this.siLanzarCorreo = new System.Workflow.Activities.IfElseBranchActivity();
            this.SiNoNotificar = new System.Workflow.Activities.IfElseActivity();
            this.RecuperarInformacion = new System.Workflow.Activities.CodeActivity();
            this.onWorkflowActivated1 = new Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated();
            // 
            // ActualizarHistorialNoLanza
            // 
            this.ActualizarHistorialNoLanza.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808");
            this.ActualizarHistorialNoLanza.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment;
            this.ActualizarHistorialNoLanza.HistoryDescription = "Flujo terminado sin actividad.";
            this.ActualizarHistorialNoLanza.HistoryOutcome = "Completado";
            this.ActualizarHistorialNoLanza.Name = "ActualizarHistorialNoLanza";
            this.ActualizarHistorialNoLanza.OtherData = "";
            this.ActualizarHistorialNoLanza.UserId = -1;
            // 
            // ActualizarHistorialSiLanza
            // 
            this.ActualizarHistorialSiLanza.Duration = System.TimeSpan.Parse("-10675199.02:48:05.4775808");
            this.ActualizarHistorialSiLanza.EventId = Microsoft.SharePoint.Workflow.SPWorkflowHistoryEventType.WorkflowComment;
            activitybind1.Name = "FlujoNotificacion";
            activitybind1.Path = "mensajeHistorial";
            this.ActualizarHistorialSiLanza.HistoryOutcome = "";
            this.ActualizarHistorialSiLanza.Name = "ActualizarHistorialSiLanza";
            this.ActualizarHistorialSiLanza.OtherData = "";
            this.ActualizarHistorialSiLanza.UserId = -1;
            this.ActualizarHistorialSiLanza.SetBinding(Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity.HistoryDescriptionProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind1)));
            // 
            // EnviarNotificacion
            // 
            this.EnviarNotificacion.BCC = null;
            activitybind2.Name = "FlujoNotificacion";
            activitybind2.Path = "cuerpoNotificacion";
            this.EnviarNotificacion.CC = null;
            correlationtoken1.Name = "workflowToken";
            correlationtoken1.OwnerActivityName = "FlujoNotificacion";
            this.EnviarNotificacion.CorrelationToken = correlationtoken1;
            this.EnviarNotificacion.From = null;
            this.EnviarNotificacion.Headers = null;
            this.EnviarNotificacion.IncludeStatus = false;
            this.EnviarNotificacion.Name = "EnviarNotificacion";
            activitybind3.Name = "FlujoNotificacion";
            activitybind3.Path = "asuntoNotificacion";
            activitybind4.Name = "FlujoNotificacion";
            activitybind4.Path = "usuariosNotificados";
            this.EnviarNotificacion.SetBinding(Microsoft.SharePoint.WorkflowActions.SendEmail.SubjectProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind3)));
            this.EnviarNotificacion.SetBinding(Microsoft.SharePoint.WorkflowActions.SendEmail.ToProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind4)));
            this.EnviarNotificacion.SetBinding(Microsoft.SharePoint.WorkflowActions.SendEmail.BodyProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind2)));
            // 
            // noLanzarCorreo
            // 
            this.noLanzarCorreo.Activities.Add(this.ActualizarHistorialNoLanza);
            this.noLanzarCorreo.Name = "noLanzarCorreo";
            // 
            // siLanzarCorreo
            // 
            this.siLanzarCorreo.Activities.Add(this.EnviarNotificacion);
            this.siLanzarCorreo.Activities.Add(this.ActualizarHistorialSiLanza);
            ruleconditionreference1.ConditionName = "SiCondicionEsTrue";
            this.siLanzarCorreo.Condition = ruleconditionreference1;
            this.siLanzarCorreo.Name = "siLanzarCorreo";
            // 
            // SiNoNotificar
            // 
            this.SiNoNotificar.Activities.Add(this.siLanzarCorreo);
            this.SiNoNotificar.Activities.Add(this.noLanzarCorreo);
            this.SiNoNotificar.Description = "Deside si se lanza o no el correo de notificacion de cambio de estado.";
            this.SiNoNotificar.Name = "SiNoNotificar";
            // 
            // RecuperarInformacion
            // 
            this.RecuperarInformacion.Name = "RecuperarInformacion";
            this.RecuperarInformacion.ExecuteCode += new System.EventHandler(this.RecuperarInformacion_ExecuteCode);
            activitybind6.Name = "FlujoNotificacion";
            activitybind6.Path = "workflowId";
            // 
            // onWorkflowActivated1
            // 
            this.onWorkflowActivated1.CorrelationToken = correlationtoken1;
            this.onWorkflowActivated1.EventName = "OnWorkflowActivated";
            this.onWorkflowActivated1.Name = "onWorkflowActivated1";
            activitybind5.Name = "FlujoNotificacion";
            activitybind5.Path = "workflowProperties";
            this.onWorkflowActivated1.SetBinding(Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated.WorkflowIdProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind6)));
            this.onWorkflowActivated1.SetBinding(Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated.WorkflowPropertiesProperty, ((System.Workflow.ComponentModel.ActivityBind)(activitybind5)));
            // 
            // FlujoNotificacion
            // 
            this.Activities.Add(this.onWorkflowActivated1);
            this.Activities.Add(this.RecuperarInformacion);
            this.Activities.Add(this.SiNoNotificar);
            this.Name = "FlujoNotificacion";
            this.CanModifyActivities = false;

        }

        #endregion

        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity ActualizarHistorialNoLanza;
        private Microsoft.SharePoint.WorkflowActions.LogToHistoryListActivity ActualizarHistorialSiLanza;
        private Microsoft.SharePoint.WorkflowActions.SendEmail EnviarNotificacion;
        private IfElseBranchActivity noLanzarCorreo;
        private IfElseBranchActivity siLanzarCorreo;
        private IfElseActivity SiNoNotificar;
        private CodeActivity RecuperarInformacion;
        private Microsoft.SharePoint.WorkflowActions.OnWorkflowActivated onWorkflowActivated1;



















    }
}
