Imports OSGeo.MapServer
Partial Class BUGS
    Inherits System.Web.UI.Page


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        '//buffer a shape that is a point
        TestBufferPoint(1, 1)
        TestBufferPoint(-1, -1)
        ' x: -103.442375
        'y: 41.483628

        '//project a shapeObj and getCentroid returns coordinates in previous projection
        'TestProjectNOClone()
        'TestProjectClone()
    End Sub

    Private Sub TestBufferPoint(ByVal x As Double, ByVal y As Double)
        Dim pt As New pointObj(x, y, Nothing, Nothing)
        Dim line As New lineObj()
        line.add(pt)
        Dim shp As New shapeObj(MS_SHAPE_TYPE.MS_SHAPE_POINT)
        shp.add(line)
        Dim bShp As shapeObj = shp.buffer(1)


    End Sub

    Private Sub TestProjectNOClone()
        '//Incorrect Behaviour
        '//pCent will equal oCent

        Dim albProj As String = "+proj=aea +lat_1=29.500000000 +lat_2=45.500000000 +lat_0=23.000000000 " _
            & "+lon_0=-96.000000000 +x_0=0.000 +y_0=0.000 +datum=NAD83 +ellps=GRS80 +no_defs"
        Dim llProj As String = "+proj=longlat +ellps=GRS80 + no_defs"

        Dim shp As New shapeObj(MS_SHAPE_TYPE.MS_SHAPE_POLYGON)
        Dim shp2 As New shapeObj(MS_SHAPE_TYPE.MS_SHAPE_POLYGON)
        Dim line As New lineObj()
        Dim pt As New pointObj(-100, 35, Nothing, Nothing)
        line.add(pt)
        pt = New pointObj(-100, 36, Nothing, Nothing)
        line.add(pt)
        pt = New pointObj(-101, 36, Nothing, Nothing)
        line.add(pt)
        pt = New pointObj(-101, 35, Nothing, Nothing)
        line.add(pt)
        pt = New pointObj(-100, 35, Nothing, Nothing)
        line.add(pt)
        shp.add(line)

        Dim oCent As pointObj = shp.getCentroid()
        Dim result As Integer
        result = shp.project(New projectionObj(llProj), New projectionObj(albProj))
        Dim pCent As pointObj = shp.getCentroid()

    End Sub

    Private Sub TestProjectClone()
        '//Correct Behaviour
        '//pCent will NOT equal oCent

        Dim albProj As String = "+proj=aea +lat_1=29.500000000 +lat_2=45.500000000 +lat_0=23.000000000 " _
            & "+lon_0=-96.000000000 +x_0=0.000 +y_0=0.000 +datum=NAD83 +ellps=GRS80 +no_defs"
        Dim llProj As String = "+proj=longlat +ellps=GRS80 + no_defs"

        Dim shp As New shapeObj(MS_SHAPE_TYPE.MS_SHAPE_POLYGON)
        Dim shp2 As New shapeObj(MS_SHAPE_TYPE.MS_SHAPE_POLYGON)
        Dim line As New lineObj()
        Dim pt As New pointObj(-100, 35, Nothing, Nothing)
        line.add(pt)
        pt = New pointObj(-100, 36, Nothing, Nothing)
        line.add(pt)
        pt = New pointObj(-101, 36, Nothing, Nothing)
        line.add(pt)
        pt = New pointObj(-101, 35, Nothing, Nothing)
        line.add(pt)
        pt = New pointObj(-100, 35, Nothing, Nothing)
        line.add(pt)
        shp.add(line)

        Dim oCent As pointObj = shp.getCentroid()
        Dim pShp As shapeObj
        pShp = shp.clone()
        Dim result As Integer
        result = pShp.project(New projectionObj(llProj), New projectionObj(albProj))
        Dim pCent As pointObj = pShp.getCentroid()

    End Sub
End Class
