<%@ Page Language="C#" AutoEventWireup="true" Inherits="tiny_mce_plugins_aspnetbrowser_imgmanager" Codebehind="imgmanager.aspx.cs" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
    <link href="css/aspnetbrowser.css" rel="stylesheet" type="text/css" />
    <script src="../../tiny_mce.js" type="text/javascript"></script>
    <script src="../../tiny_mce_popup.js" type="text/javascript"></script>
    <script src="js/aspnetbrowser.js" type="text/javascript"></script>
</head>
<body>
    <form id="form1" runat="server" action="#">
    <h3>Выбор изображений</h3>
    <asp:ScriptManager ID="ImageManagerScriptManager" runat="server">
    </asp:ScriptManager>
    <table style="width:100%;">
        <tr>
            <td colspan="3">
                <asp:Literal ID="ErrorLiteral" runat="server" Text="" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Literal ID="PathSelectLiteral" runat="server" Text="Выберите папку" />
            </td>
            <td>
                <asp:UpdatePanel ID="PathDropDownUpdatePanel" runat="server">
                    <ContentTemplate>
                        <asp:DropDownList ID="PathDropDown" runat="server" AppendDataBoundItems="true" AutoPostBack="true"
                            OnSelectedIndexChanged="PathDropDown_SelectedIndexChanged">
                            <asp:ListItem Text="--Root--" Value="-1" />
                        </asp:DropDownList>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <td>
                <asp:Button ID="DeleteFolderButton" runat="server" Text="Удалить" OnClick="DeleteFolderButton_Click"
                    OnClientClick="javascript:return confirm('do you wish to delete this directory?');" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Literal ID="FolderLiteral" runat="server" Text="Создать папку:" />
            </td>
            <td>
                <asp:TextBox ID="FolderTextBox" runat="server" />
            </td>
            <td>
                <asp:Button ID="CreateFolderButton" runat="server" Text="Создать" OnClick="CreateFolderButton_Click" />
            </td>
        </tr>
        <tr>
            <td>
                <asp:Literal ID="SelectImageLiteral" runat="server" Text="Выбрать изображение:" />
            </td>
            <td>
                <asp:FileUpload ID="ImageFileUpload" runat="server" />
                <asp:RegularExpressionValidator ID="ImageFileUploadRegExValidator" runat="server"
                             ControlToValidate ="ImageFileUpload" ValidationExpression=".*((\.jpg)|(\.bmp)|(\.gif))"
                              ErrorMessage="только jpg, bmp and gif можно загружать" Text="*" />
            </td>
            <td>
                <asp:Button ID="UploadButton" runat="server" Text="Загрузить" OnClick="UploadButton_Click" />
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <asp:Literal ID="AvailableImagesLiteral" runat="server" Text="Изображения в папке:" />
            </td>
        </tr>
        <tr>
            <td colspan="3">
                <asp:UpdatePanel ID="ImageGridViewUpdatePanel" runat="server">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="PathDropDown" />
                    </Triggers>
                    <ContentTemplate>
                        <asp:GridView ID="ImageGridView" runat="server" DataKeyNames="Name" AllowPaging="true"
                            AutoGenerateColumns="false" EnableViewState="false" OnRowDataBound="imageGridView_RowDataBound"
                            PageSize="5" OnSelectedIndexChanged="ImageGridView_SelectedIndexChanged" OnPageIndexChanging="ImageGridView_PageIndexChanging"
                            OnRowCommand="ImageGridView_RowCommand" OnRowDeleting="ImageGridView_RowDeleting">
                            <EmptyDataTemplate>
                                No Images Found!
                            </EmptyDataTemplate>
                            <Columns>
                                <asp:TemplateField  HeaderStyle-Width="40px" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:Literal ID="radiobuttonMarkup" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Изображение" HeaderStyle-Width="450px" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:Image ID="displayImage" runat="server"  style=' max-width: 200px; max-height: 200px;' />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderStyle-Width="40px" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <asp:LinkButton ID="DeleteButton" CommandArgument='<%# Container.DataItemIndex %>' CommandName="Delete" Text="<img src='img/x.gif' alt='Удалить' />" Width="21" Height="21"
                                            ToolTip="Удалить текущее изображение." runat="server" OnClientClick="javascript:return confirm('Вы уверены, что хотите удалить изображение?');" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <SelectedRowStyle BackColor="Gray" />
                        </asp:GridView>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
        </tr>
        <tr>
            <td>
                <asp:UpdatePanel ID="SelectButtonUpdatePanel" runat="server">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ImageGridView" />
                    </Triggers>
                    <ContentTemplate>
                        <asp:Button ID="SelectButton" runat="server" CausesValidation="false" Text="Выбрать"
                            OnClientClick="AspNetBrowserDialog.insert(,);return false;" Font-Size="Large" Font-Bold="true" ForeColor="Green" />
                    </ContentTemplate>
                </asp:UpdatePanel>
            </td>
            <td>
                <asp:Button ID="CancelButton" runat="server" CausesValidation="false" Text="Отмена"
                    OnClientClick="tinyMCEPopup.close();" />
                 </td>
            </tr>
    </table>
    <asp:ValidationSummary ID="ValidationSummary1" runat="server" />
    </form>
</body>
</html>
