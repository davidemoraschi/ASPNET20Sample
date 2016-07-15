Imports OSGeo.MapServer

Partial Class Selection
    Inherits System.Web.UI.Page

    Private Util As Utility

    '//keeps track of the number of times the user has clicked (for zoom box)
    '//used with Session("CLICKCOUNT")
    Public ClickCount As Integer = 0
    '//holds the initial point the user clicked for zoom box 
    '//used with Session("FIRSTPT")
    Public FirstPT As pointObj
    Private res As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If IsPostBack And Session("UTIL") IsNot Nothing Then
            Util = Session("UTIL")

            'remove buffer layer
            Dim l As layerObj = Util.MSMap.getLayerByName("BUFFER")
            Do While l IsNot Nothing
                Util.MSMap.removeLayer(l.index)
                l = Util.MSMap.getLayerByName("BUFFER")
            Loop
        Else
            Util = New Utility(Me.Page, Server.MapPath("output"), "output")
            Util.MSMap = New mapObj(Server.MapPath("App_Data\data\OK_BASE.map"))
            Util.FullExtent = New rectObj(Util.MSMap.extent.minx, Util.MSMap.extent.miny, Util.MSMap.extent.maxx, Util.MSMap.extent.maxy, Nothing)
            Util.MapIsDirty = True
        End If
    End Sub

    Protected Sub Page_PreRender(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.PreRender
        If Util.MapIsDirty Then
            Util.RefreshImage(imgMap)
        End If
        Session("UTIL") = Util
    End Sub

    ''' <summary>
    ''' Handle the Click event of the imgMap object performing necessary actions on MSMAP.
    ''' </summary>
    ''' <param name="sender"></param>
    ''' <param name="e"></param>
    ''' <remarks></remarks>
    Protected Sub imgMap_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles imgMap.Click
        Dim clickPt As New pointObj(e.X, e.Y, Nothing, Nothing)
        Dim mapPt As pointObj = Util.Pixel2Geo(clickPt)
        Dim rect As New rectObj(mapPt.x - 10, mapPt.y - 10, mapPt.x + 10, mapPt.y + 10, 0)

        Select Case MapAction.SelectedItem.Text.ToUpper
            Case "IDENTIFY"
                lblResult.Text = ""
                Dim l As layerObj = Util.MSMap.getLayerByName("Counties")
                res = l.queryByRect(Util.MSMap, rect)
                If res = Utility.RESULT_CODE.MS_SUCCESS Then
                    Dim results As resultCacheObj = l.getResults()
                    If results IsNot Nothing Then
                        Dim r As resultCacheMemberObj = results.getResult(0)
                        l.open()
                        lblResult.Text = "You selected " & l.getFeature(r.shapeindex, r.tileindex).getValue(1) & " county."
                        l.close()
                    End If
                End If
                Util.MapIsDirty = True
                lblTip.Text = ""
            Case "SELECTEXTENT"
                If Session("ClickCount") IsNot Nothing Then
                    ClickCount = Session("ClickCount")
                End If
                If ClickCount < 1 Then
                    lblTip.Text = "Click the map again to select the extent outlined by your clicks."

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

                    Dim pt As pointObj = Util.Pixel2Geo(New pointObj(thisRect.minx, thisRect.miny, Nothing, Nothing))
                    Dim pt2 As pointObj = Util.Pixel2Geo(New pointObj(thisRect.maxx, thisRect.maxy, Nothing, Nothing))
                    thisRect.minx = pt.x
                    thisRect.miny = pt.y
                    thisRect.maxx = pt2.x
                    thisRect.maxy = pt2.y
                    Dim l As layerObj = Util.MSMap.getLayerByName("Counties")
                    res = l.queryByRect(Util.MSMap, thisRect)
                    If res = Utility.RESULT_CODE.MS_SUCCESS Then
                        Dim results As resultCacheObj = l.getResults()
                        If results IsNot Nothing Then
                            Dim r As resultCacheMemberObj = results.getResult(0)
                            l.open()
                            lblResult.Text = "You selected " & results.numresults & " counties."
                            l.close()
                        End If
                    End If
                    Util.MapIsDirty = True
                End If
        End Select

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
            Case "IDENTIFY"
                lblToolTip.Text = "Click the map to Identify features."

            Case "SELECTEXTENT"
                lblToolTip.Text = "Click the map twice, defining the bounds of an extent to select."

            Case "QUERY COUNTY BY NAME"
                lblResult.Text = ""
                Dim l As layerObj = Util.MSMap.getLayerByName("Counties")
                res = l.queryByAttributes(Util.MSMap, "NAME", "ALFALFA", Utility.QUERY_TYPE.MS_MULTIPLE)
                If res = Utility.RESULT_CODE.MS_SUCCESS Then
                    Dim results As resultCacheObj = l.getResults()
                    If results IsNot Nothing Then
                        Dim r As resultCacheMemberObj = results.getResult(0)
                        l.open()
                        lblResult.Text = "You selected " & l.getFeature(r.shapeindex, r.tileindex).getValue(1) & " county."
                        l.close()
                    End If
                End If
                Util.MapIsDirty = True
                lblTip.Text = ""

                MapAction.SelectedValue = "Identify"
                lblToolTip.Text = "Click the map to Identify features."

            Case "QUERY COUNTY BY MULTIPLE NAME"
                lblResult.Text = ""
                Dim l As layerObj = Util.MSMap.getLayerByName("Counties")
                res = l.queryByAttributes(Util.MSMap, "NAME", "/\b(ALFALFA|ADAIR)\b/", mapscript.MS_MULTIPLE)
                If res = Utility.RESULT_CODE.MS_SUCCESS Then
                    Dim results As resultCacheObj = l.getResults()
                    If results.numresults > 0 Then
                        lblResult.Text = "You selected " & results.numresults & " counties."
                    Else
                        lblResult.Text = "Query succeeded. 0 results."
                    End If
                Else
                    lblResult.Text = "Query failed."
                End If

                Util.MapIsDirty = True
                lblTip.Text = ""

                MapAction.SelectedValue = "Identify"
                lblToolTip.Text = "Click the map to Identify features."

            Case "BUFFER"
                lblResult.Text = ""
                Dim selectedShapes As New Generic.List(Of shapeObj)
                Dim bufferedShapes As New Generic.List(Of shapeObj)

                Dim l As layerObj = Util.MSMap.getLayerByName("Counties")


                '//if we have an existing selection use it, otherwise make a selection first
                If l.getResults Is Nothing OrElse l.getResults.numresults <= 0 Then
                    res = l.queryByAttributes(Util.MSMap, "NAME", "/\b(JACKSON|ADAIR)\b/", mapscript.MS_MULTIPLE)
                    If res = Utility.RESULT_CODE.MS_SUCCESS Then
                        Dim results As resultCacheObj = l.getResults()
                        If results.numresults > 0 Then
                            l.open()
                            For i As Integer = 0 To results.numresults - 1
                                selectedShapes.Add(l.getFeature(results.getResult(i).shapeindex, results.getResult(i).tileindex))
                            Next
                            l.close()
                            lblResult.Text = "Created buffer using a new selection (there was nothing selected)."
                        Else
                            lblResult.Text = "Query succeeded. 0 results."
                        End If
                    Else
                        lblResult.Text = "Query failed."
                    End If
                Else
                    lblResult.Text = "Created buffer using exiting selection."
                    Dim results As resultCacheObj = l.getResults()
                    If results.numresults > 0 Then
                        l.open()
                        For i As Integer = 0 To results.numresults - 1
                            selectedShapes.Add(l.getFeature(results.getResult(i).shapeindex, results.getResult(i).tileindex))
                        Next
                        l.close()
                    End If
                End If

                '//buffer the selected shapes
                'Dim mapProj As String = Util.MSMap.getProjection()
                'Dim lProj As String = l.getProjection()

                Dim bShp As shapeObj
                For Each s As shapeObj In selectedShapes
                    '//buffering shapes with a large number of points takes a LONG time
                    '//subsequent use of the buffered shape for selection is also costly with a lot of points.
                    '//hence - generalize. NOTE that this reduces the accuracy of the result.
                    s = Utility.Generalize(s, 50)
                    s = s.buffer(0.001)
                    bShp = s.clone()
                    res = bShp.project(New projectionObj(l.getProjection()), New projectionObj(Util.MSMap.getProjection()))
                    bufferedShapes.Add(bShp)
                Next

                '//add layer to map with buffered shapes
                Dim bLayer As New layerObj(Util.MSMap)
                With bLayer
                    .name = "BUFFER"
                    .type = MS_LAYER_TYPE.MS_LAYER_POLYGON
                    .insertClass(Util.MSMap.getLayerByName("POLYGON SELECT").getClass(0), -1)
                    .template = Util.MSMap.getLayerByName("POLYGON SELECT").template
                    .status = 2
                    .setProjection(Util.MSMap.getProjection)
                    For Each shp As shapeObj In bufferedShapes
                        .addFeature(shp)
                    Next
                    .opacity = 10
                End With


                '//select features from Counties layer by bLayer features
                '//NOTE: you have to use queryByIndex because other 
                '//      query methods do not allow adding to current set
                Dim lIndexes As New Generic.List(Of Integer)
                For i As Integer = 0 To bufferedShapes.Count - 1 '.getNumFeatures - 1
                    res = l.queryByShape(Util.MSMap, bufferedShapes(i))
                    If res = Utility.RESULT_CODE.MS_SUCCESS Then
                        Dim results As resultCacheObj = l.getResults()
                        If results.numresults > 0 Then
                            Dim rcmo As resultCacheMemberObj
                            For nR As Integer = 0 To l.getNumResults - 1
                                rcmo = results.getResult(nR)
                                lIndexes.Add(rcmo.shapeindex)
                            Next
                        End If
                    End If
                Next
                '//clear selection (KLUDGE)
                res = l.queryByIndex(Util.MSMap, l.tileindex, -1, mapscript.MS_FALSE)
                '//select by index
                For Each i As Integer In lIndexes
                    res = l.queryByIndex(Util.MSMap, l.tileindex, i, mapscript.MS_TRUE)
                Next

                Util.MapIsDirty = True
                lblTip.Text = ""

                MapAction.SelectedValue = "Identify"
                lblToolTip.Text = "Click the map to Identify features."


        End Select
    End Sub


End Class
