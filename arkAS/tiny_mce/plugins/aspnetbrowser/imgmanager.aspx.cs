using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;

public partial class tiny_mce_plugins_aspnetbrowser_imgmanager : System.Web.UI.Page
{

    #region variables

    public string host = string.Empty;
    private const string checkedAttributeHTML = "checked=\"checked\"";
    //HACK: the javascript this.style.visibility='hidden'; is a terrible hack to prevent the client doubleclick crash.
    private const string radiobuttonHTML = "<input type=\"radio\" name=\"imagegroup\" id=\"selected{0}\" value=\"{0}\" onclick=\"{1};this.style.visibility='hidden';\" {2} />";
    public const string thispage = "imgmanager.aspx";
    public string defaultUploadPath = "/uploads/images/";
    public string aspnetimagebrowserImagePath = string.Empty;
    public string physicalPath = string.Empty;
    public string uploadPath = string.Empty;
    private const string onDeleteError = "Файл не может быть удален!";
    private const string onFileExistError = "Файл уже существует!";
    private const string onFolderExistError = "Папка уже существует!";
    private const string onNoFileSelectedError = "Не выделено ни одной картинки!";
    private const string onFileSaveSuccess = "Файл успешно загружен!";
    private string currentUrl = string.Empty;
    public string aspnetVirtualFolderPath = string.Empty;
    private HttpContext context = null;
    private FileInfo[] imagefiles = new FileInfo[] { };

    #endregion

    #region properties

    private int CurrentIndex
    {
        get { return (int?)ViewState["CurrentIndex"] ?? -1; }
        set { ViewState["CurrentIndex"] = value; }
    }

    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        context = HttpContext.Current;
        currentUrl = context.Request.Url.AbsoluteUri;
        host = context.Request.ServerVariables["HTTP_HOST"];
        if (context.Request.IsSecureConnection)
        {
            host = host.Replace("http:/", "https:/");
        }

        physicalPath = context.Server.MapPath("~");
        uploadPath = context.Server.MapPath(Path.Combine(physicalPath, defaultUploadPath));
        aspnetVirtualFolderPath = ToVirtualPath(context.Request.Path.Replace(thispage, string.Empty));

        if (!Page.IsPostBack)
        {
            SelectButton.Enabled = false;
            BindDirectoryDropDown();
        }
        BindData();
    }

    #region binding
    private void BindDirectoryDropDown()
    {
        try
        {
            PathDropDown.Items.Clear();
            PathDropDown.Items.Add(new ListItem("/images/", uploadPath));
            RecursiveSearch(uploadPath, PathDropDown);
        }
        catch (UnauthorizedAccessException ex)
        {
            ErrorLiteral.Text = "UnauthorizedAccessException\n" + ex.Message;
        }
    }

    private DirectoryInfo dirInfo;
    private string parent = string.Empty;

    private void RecursiveSearch(string path, DropDownList dropdown)
    {
        if (!Directory.Exists(path))
            return;
        dirInfo = new DirectoryInfo(path);
        foreach (FileSystemInfo fileInfo in dirInfo.GetDirectories())
        {
            if (fileInfo.Attributes == FileAttributes.Directory)
            {
                parent += "/" + Directory.GetParent(fileInfo.FullName).Name;
                ListItem li = new ListItem(parent + "/" + fileInfo.Name, fileInfo.FullName);
                dropdown.Items.Add(li);
                RecursiveSearch(fileInfo.FullName, dropdown);
            }
        }
        parent = string.Empty;
    }

    #endregion

    protected void PathDropDown_SelectedIndexChanged(object sender, EventArgs e)
    {
        SelectButton.Enabled = false;
        BindData();
    }

    #region binding

    private void BindData()
    {
        if (Directory.Exists(PathDropDown.SelectedValue))
        {
            DirectoryInfo info = new DirectoryInfo(PathDropDown.SelectedValue);
            LoadFiles(info);
        }
    }

    private void LoadFiles(DirectoryInfo info)
    {
        var files = info.GetFiles();
        if (files != null)
        {
            imagefiles = files;
            foreach (var item in files)
            {
                ImageGridView.DataSource = files;
                ImageGridView.DataBind();
            }

        }
    }

    #endregion

    #region IO utilities

    private void DeleteFile(string file)
    {
        if (!string.IsNullOrEmpty(file) && File.Exists(file))
        {
            try
            {
                File.Delete(file);
            }
            catch (Exception ex)
            {
                ErrorLiteral.Text = ex.Message;
            }
        }
    }

    private void DeleteFolder(string folder)
    {
        if (!string.IsNullOrEmpty(folder) && Directory.Exists(folder))
        {
            try
            {
                Directory.Delete(folder);
            }
            catch (Exception ex)
            {
                ErrorLiteral.Text = ex.Message;
            }
        }
    }

    private void CreateFolder(string folder)
    {
        if (!string.IsNullOrEmpty(folder) && !Directory.Exists(folder))
        {
            try
            {
                Directory.CreateDirectory(folder);
                BindDirectoryDropDown();
            }
            catch (Exception ex)
            {
                ErrorLiteral.Text = ex.Message;
            }
        }
    }

    private void CreateFile(string file)
    {
        if (!string.IsNullOrEmpty(file) && !File.Exists(file))
        {
            try
            {
                File.Delete(file);
            }
            catch (Exception ex)
            {
                ErrorLiteral.Text = ex.Message;
            }
        }
    }

    #endregion

    #region create/delete directory

    protected void CreateFolderButton_Click(object sender, EventArgs e)
    {
        string folder = FolderTextBox.Text.Trim();
        if (!string.IsNullOrEmpty(folder))
        {
            string folderPath = Path.Combine(PathDropDown.SelectedValue, folder);
            CreateFolder(folderPath);
            FolderTextBox.Text = "";
        }
    }

    protected void DeleteFolderButton_Click(object sender, EventArgs e)
    {
        string directory = PathDropDown.SelectedValue;
        if (!string.IsNullOrEmpty(directory) && Directory.Exists(directory))
        {
            try
            {
                Directory.Delete(directory);
                this.BindDirectoryDropDown();
                this.BindData();
            }
            catch (IOException ex)
            {
                ErrorLiteral.Text = ex.Message;
            }
        }
    }

    #endregion

    #region upload file

    protected void UploadButton_Click(object sender, EventArgs e)
    {
        if (ImageFileUpload.HasFile)
        {
            try
            {
                if (Directory.Exists(PathDropDown.SelectedValue))
                {
                    HttpPostedFile postedFile = ImageFileUpload.PostedFile;
                    postedFile.SaveAs(Path.Combine(PathDropDown.SelectedValue, postedFile.FileName));
                    ErrorLiteral.Text = onFileSaveSuccess;
                    context.Response.Redirect(currentUrl);
                }
            }
            catch (Exception ex)
            {
                ErrorLiteral.Text = ex.Message;
            }
        }
        else
        {
            ErrorLiteral.Text = onNoFileSelectedError;
        }
    }

    #endregion

    #region gridview methods

    protected void ImageGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
    {

    }

    protected void ImageGridView_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName.Equals("Delete"))
        {
            var file = imagefiles[Convert.ToInt32(e.CommandArgument)];
            if (file != null)
            {
                DeleteFile(file.FullName);
                BindData();
            }
        }
    }

    protected void ImageGridView_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        ImageGridView.PageIndex = e.NewPageIndex;
        BindData();
    }

    protected void imageGridView_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            GridViewRow row = (GridViewRow)e.Row;
            FileInfo fi = (FileInfo)row.DataItem;

            Image imageList = e.Row.FindControl("displayImage") as Image;
            if (imageList != null)
            {
                imageList.ImageUrl = ToVirtualPath(fi, false);
            }
            Button deleteButton = e.Row.FindControl("DeleteButton") as Button;
            if (deleteButton != null)
            {
                deleteButton.CommandArgument = Convert.ToString(row.RowIndex);
            }
        }
    }

    protected void ImageGridView_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ImageGridView.SelectedIndex > -1)
        {
            GridViewRow row = ImageGridView.SelectedRow;
            if (row != null)
            {
                var file = imagefiles[ImageGridView.SelectedIndex];
                string script = "AspNetBrowserDialog.insert('{0}','{1}');return false;";
                script = string.Format(script, ToVirtualPath(file, true), file.Name);
                SelectButton.Enabled = true;
                SelectButton.OnClientClick = script;
                CurrentIndex = row.RowIndex;
            }
        }
        else
        {
            SelectButton.Enabled = false;
        }
    }

    #endregion

    #region path utilities

    private string ToVirtualPath(FileInfo fi, bool forTinyMCE = false)
    {
        string root = context.Server.MapPath("~/");
        string path = (fi.Directory.FullName + "\\" + fi.Name);
        path = path.Replace(root, string.Empty);
        path = path.Replace("\\", "/");
        if (forTinyMCE)
            return ("/" + path);
        else
            return ("~/" + path);
    }

    private string ToVirtualPath(string filename)
    {
        string root = context.Server.MapPath("~/");
        string path = filename;
        path = path.Replace(root, string.Empty);
        path = path.Replace("\\", "/");
        return ("~/" + path);
    }

    #endregion

    #region render

    protected override void Render(HtmlTextWriter writer)
    {
        foreach (GridViewRow row in ImageGridView.Rows)
        {
            if (row.RowType == DataControlRowType.DataRow)
            {
                Literal radiobuttonMarkup = row.FindControl("radiobuttonMarkup") as Literal;
                if (radiobuttonMarkup != null)
                {
                    string script = ClientScript.GetPostBackEventReference(ImageGridView, "Select$" + row.RowIndex, true);
                    string attr = string.Empty;
                    if (CurrentIndex == row.RowIndex)
                        attr = checkedAttributeHTML;
                    radiobuttonMarkup.Text = string.Format(radiobuttonHTML, row.RowIndex, script, attr);
                }
            }
        }
        base.Render(writer);
    }

    #endregion

}