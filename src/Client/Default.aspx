<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Client._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
	<div class="row">
		<div class="col-md-4">
			<br />
			<strong>Current Thread Identity: </strong><%= System.Threading.Thread.CurrentPrincipal.Identity?.Name %>
			<br />
			<strong>Current Windows Identity: </strong><%= System.Security.Principal.WindowsIdentity.GetCurrent()?.Name %>
		</div>
	</div>
	<div class="row">
		<div class="col-md-4">
			<br />
			<strong>Server Thread Identity (TCP): </strong><%= ServerThreadIdentityTCP %>
			<br />
			<strong>Server Windows Identity (TCP): </strong><%= ServerWindowsIdentityTCP %>
			<br />
		</div>
		<div class="col-md-4">
			<br />
			<strong>Server Thread Identity (IPC): </strong><%= ServerThreadIdentityIPC %>
			<br />
			<strong>Server Windows Identity (IPC): </strong><%= ServerWindowsIdentityIPC %>
			<br />
		</div>
	</div>
	<div class="row">
		<div class="col-md-4">
			<br />
			<strong>With impersonation</strong>
			<br />
			<strong>Server Thread Identity (TCP): </strong><%= ServerThreadIdentityTCP %>
			<br />
			<strong>Server Windows Identity (TCP): </strong><%= ServerWindowsIdentityTCP %>
			<br />
		</div>
		<div class="col-md-4">
			<br />
			<br />
			<strong>Server Thread Identity (IPC): </strong><%= ServerThreadIdentityIPC %>
			<br />
			<strong>Server Windows Identity (IPC): </strong><%= ServerWindowsIdentityIPC %>
			<br />
		</div>
	</div>
</asp:Content>
