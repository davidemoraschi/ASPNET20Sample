Imports System.IO
Imports OSGeo.MapServer

Partial Class BasicMap
    Inherits System.Web.UI.Page

    '//keeps track of the number of times the user has clicked (for zoom box)
    '//used with Session("CLICKCOUNT")
    Public ClickCount As Integer = 0
    '//holds the initial point the user clicked for zoom box 
    '//used with Session("FIRSTPT")
    Public FirstPT As pointObj

    ''' <summary>
    ''' Stores mapObj utility functions.
    ''' </summary>
    ''' <remarks></remarks>
    Private Util As Utility

    Private Enum RESULT_CODE As Integer
        MS_SUCCESS = 0
        MS_FAILURE = 1
    End Enum

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If IsPostBack And Session("UTIL") IsNot Nothing Then
            Util = Session("UTIL")
        Else
            Util = New Utility(Me.Page, Server.MapPath("output"), "output")
            Util.MSMap = New mapObj(Server.MapPath("App_Data\data\OK_BASE.map"))
            Util.FullExtent = New rectObj(Util.MSMap.extent.minx, Util.MSMap.extent.miny, Util.MSMap.extent.maxx, Util.MSMap.extent.maxy, Nothing)
            Util.MapIsDirty = True
        End If

    End Sub

    ''' <summary>
    ''' Handle the Click event of the imgMap object performing necessary actions on MSMAP.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub imgMap_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgMap.Click
        Dim clickPt As New pointObj(e.X, e.Y, Nothing, Nothing)
        Select Case MapAction.SelectedItem.Text.ToUpper
            Case "ZOOMIN"
                Util.MSMap.zoomPoint(2, clickPt, Util.MSMap.width, Util.MSMap.height, Util.MSMap.extent, Nothing)
                Util.MapIsDirty = True
                lblTip.Text = ""

            Case "ZOOMOUT"
                lblTip.Text = ""

                Util.MSMap.zoomPoint(-2, clickPt, Util.MSMap.width, Util.MSMap.height, Util.MSMap.extent, Nothing)
                Util.MapIsDirty = True
            Case "PAN"
                lblTip.Text = ""

                Util.MSMap.zoomPoint(1, clickPt, Util.MSMap.width, Util.MSMap.height, Util.MSMap.extent, Nothing)
                Util.MapIsDirty = True
            Case "ZOOMBOX"
                If Session("ClickCount") IsNot Nothing Then
                    ClickCount = Session("ClickCount")
                End If
                If ClickCount < 1 Then
                    lblTip.Text = "Click the map again to zoom to the extent outlined by your clicks."

                    ClickCount += 1
                    FirstPT = clickPt
                    '//Store to session
                    Session("FirstPT") = FirstPT
                    Session("ClickCount") = ClickCount
                Else
                    lblTip.Text = ""

                    ClickCount = 0
                    Session("ClickCount") = ClickCount
                    FirstPT = Session("FirstPT")

                    Dim minX, minY, maxX, maxY As Integer
                    If FirstPT.x < clickPt.x Then
                        minX = FirstPT.x
                        maxX = clickPt.x
                    Else
                        minX = clickPt.x
                        maxX = FirstPT.x
                    End If

                    If FirstPT.y < clickPt.y Then
                        minY = FirstPT.y
                        maxY = clickPt.y
                    Else
                        minY = e.Y
                        maxY = FirstPT.y
                    End If

                    Dim thisRect As New rectObj(0, 0, 1, 1, 0)
                    thisRect.minx = minX
                    thisRect.miny = maxY
                    thisRect.maxx = maxX
                    thisRect.maxy = minY

                    Util.MSMap.zoomRectangle(thisRect, Util.MSMap.width, Util.MSMap.height, Util.MSMap.extent, Nothing)
                    Util.MapIsDirty = True
                End If
           
        End Select

    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If Util.MapIsDirty Then
            Util.RefreshImage(imgMap)
        End If
        Session("UTIL") = Util
    End Sub

    ''' <summary>
    ''' Handle the SelectedIndexChanged event of the MapAction object.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub MapAction_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles MapAction.SelectedIndexChanged
        '//Usually we are just setting lblToolTip.Text to prompt the user for action.
        '//In the case of ZoomFullExtent, we perform the action 
        '//and reset MapAction and lblToolTip.Text to a default.
        Select Case MapAction.SelectedItem.Text.ToUpper
            Case "ZOOMIN"
                lblToolTip.Text = "Click the map to Zoom In."
            Case "ZOOMOUT"
                lblToolTip.Text = "Click the map to Zoom Out."
            Case "PAN"
                lblToolTip.Text = "Click the map to center at the point you clicked."
            Case "ZOOMEXTENT"
                lblToolTip.Text = "Click the map twice, defining the bounds of an extent to zoom to."
            Case "ZOOMFULLEXTENT"
                Util.ZoomFullExtext()
                MapAction.SelectedValue = "ZoomIn"
                lblToolTip.Text = "Click the map to Zoom In."
        End Select
    End Sub
End Class
