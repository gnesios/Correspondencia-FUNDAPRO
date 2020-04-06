<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<%@ Page Language="C#" %>

<script language="javascript" type="text/javascript">
    function printpage() {
        window.print()
    }
</script>

<html dir="ltr" xmlns="http://www.w3.org/1999/xhtml">

<head id="Head1" runat="server">
<meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
<title>Hoja de Ruta</title>
</head>
<body onload="printpage()">
<form id="form1" runat="server">
	<table id="HojaDeRuta" style="width: 100%; border: 2px black solid; font-size:small" cellspacing="5" cellpadding="3">
	<tr>
		<td rowspan="2" style="text-align:center"><img src="/Imgenes%20de%20Sitio/EPThumb.jpg"></td>
		<td rowspan="2" style="text-align:center; font-size:medium"><strong>HOJA DE RUTA</strong></td>
		<td style="border-bottom: 1px black solid">N&ordm; DE INGRESO: <strong><%= Request.QueryString["id"] %></strong></td>
	</tr>
	<tr>
		<td style="border-bottom: 1px black solid">FECHA: <strong><%= Request.QueryString["fr"] %></strong></td>
	</tr>
	<tr>
		<td style="border: 1px black solid">FECHA Y CITE: <strong><%= Request.QueryString["fcc"] %></strong>
		</td>
		<td style="border: 1px black solid">DESTINATARIO: <strong><%= Request.QueryString["dt"] %></strong>
		</td>
		<td rowspan="2" style="border: 1px black solid" valign="top">REFERENCIA: <strong><%= Request.QueryString["rf"] %></strong>
		</td>
	</tr>
	<tr>
		<td colspan="2" style="border: 1px black solid">ORIGEN: <strong><%= Request.QueryString["or"] %></strong>
		</td>
	</tr>
	<tr>
		<td colspan="3" style="border:1px black solid">DESTINATARIO N&ordm; 1:<br />
		<br />
		<br />
		<br />
		<br />
		</td>
	</tr>
	<tr>
		<td colspan="3" style="border:1px black solid">DESTINATARIO N&ordm; 2:<br />
		<br />
		<br />
		<br />
		<br />
		</td>
	</tr>
	<tr>
		<td colspan="3" style="border:1px black solid">DESTINATARIO N&ordm; 3:<br />
		<br />
		<br />
		<br />
		<br />
		</td>
	</tr>
</table>

<p style="text-align: right"><a href="/Lists/Correspondencia%20de%20Entrada%20EducaPro%20CB/">Continuar</a></p>

</form>
</body>
</html>
