
Partial Class Samples
    Inherits System.Web.UI.MasterPage

    Protected Sub Page_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        DownloadLink.HRef = ConfigurationManager.AppSettings("DownloadURL")
    End Sub
End Class

