Imports System.IO
Imports OSGeo.MapServer

Imports Microsoft.VisualBasic

Public Class Utility
    Public Enum RESULT_CODE As Integer
        MS_SUCCESS = 0
        MS_FAILURE = 1
    End Enum

    Public Enum QUERY_TYPE As Integer
        MS_SINGLE = 0
        MS_MULTIPLE = 1
    End Enum

    Private _outputDir As String
    Public Property OutputDir() As String
        Get
            Return _outputDir
        End Get
        Set(ByVal value As String)
            _outputDir = value
        End Set
    End Property


    Private _outputDirURL As String
    Public Property OutputDirURL() As String
        Get
            Return _outputDirURL
        End Get
        Set(ByVal value As String)
            _outputDirURL = value
        End Set
    End Property


    Private _fullExtent As rectObj
    Public Property FullExtent() As rectObj
        Get
            Return _fullExtent
        End Get
        Set(ByVal value As rectObj)
            _fullExtent = value
        End Set
    End Property


    Private _mapIsDirty As Boolean
    Public Property MapIsDirty() As Boolean
        Get
            Return _mapIsDirty
        End Get
        Set(ByVal value As Boolean)
            _mapIsDirty = value
        End Set
    End Property


    Private _Page As System.Web.UI.Page
    Public ReadOnly Property Page() As System.Web.UI.Page
        Get
            Return _Page
        End Get
    End Property

    Public Property MSMap() As mapObj
        Get
            Return Page.Session("MAP")
        End Get
        Set(ByVal value As mapObj)
            Page.Session("MAP") = value
        End Set
    End Property


    Public Sub New(ByVal webPage As System.Web.UI.Page, ByVal outDir As String, ByVal outDirURL As String)
        _Page = webPage
        OutputDir = outDir
        OutputDirURL = outDirURL
    End Sub

    ''' <summary>
    ''' Redraw MSMAP, save the resultant image and update the imgMap object with the URL.
    ''' </summary>
    ''' <remarks></remarks>
    Public Sub RefreshImage(ByVal imgMap As System.Web.UI.WebControls.ImageButton)
        Deleteimages()

        Dim imageLoc As String
        Dim imageURL As String
        Dim img As imageObj
        img = MSMap.drawQuery
        imageLoc = OutputDir & "/" & System.Math.Abs(Now.ToBinary) & "." & img.format.extension
        imageURL = imageLoc.Substring(imageLoc.IndexOf(OutputDirURL))
        img.save(imageLoc, Nothing)

        'Dim imgMap As ImageButton = Page.FindControl(ImageMapID)
        With imgMap
            .Width = img.width
            .Height = img.height
            .ImageUrl = imageURL
        End With
        MapIsDirty = False
    End Sub

    ''' <summary>
    ''' Empty outputDir.
    ''' </summary>
    ''' <remarks></remarks>
    Protected Sub Deleteimages()
        For Each f As String In Directory.GetFiles(outputDir)
            File.Delete(f)
        Next
    End Sub

    Public Sub ZoomFullExtext()
        MSMap.setExtent(FullExtent.minx, FullExtent.miny, FullExtent.maxx, FullExtent.maxy)
        MapIsDirty = True
    End Sub

    ''' <summary>
    ''' Convert a user's click (image x,y) to MSMAP world coordinates.
    ''' </summary>
    ''' <param name="pt">Image X,Y</param>
    ''' <returns>pointObj in world coordinates</returns>
    ''' <remarks></remarks>
    Public Function Pixel2Geo(ByVal pt As pointObj) As pointObj
        Dim imgXMin, imgXMax, imgYMin, imgYMax As Integer
        Dim geoXMin, geoXMax, geoYMin, geoYMax As Double

        '//init corner coords
        imgXMin = 0
        imgYMin = 0
        imgXMax = MSMap.width
        imgYMax = MSMap.height


        geoXMin = MSMap.extent.minx
        geoXMax = MSMap.extent.maxx
        geoYMin = MSMap.extent.miny
        geoYMax = MSMap.extent.maxy

        Dim imgWidth, imgHeight As Integer
        Dim geoWidth, geoHeight As Double

        '//calc the width
        imgWidth = imgXMax - imgXMin
        imgHeight = imgYMax - imgYMin

        If geoXMin < geoXMax Then
            geoWidth = geoXMax - geoXMin
        Else
            geoWidth = geoXMin - geoXMax
        End If
        If geoYMin < geoYMax Then
            geoHeight = geoYMax - geoYMin
        Else
            geoHeight = geoYMin - geoYMax
        End If

        '//calc the percent along each axis
        Dim xPercent, yPercent As Double
        xPercent = pt.x / imgWidth
        yPercent = pt.y / imgHeight


        Dim newX, newY As Double
        newX = (xPercent * geoWidth) + geoXMin
        newY = geoYMax - (yPercent * geoHeight)

        Pixel2Geo = New pointObj(newX, newY, Nothing, Nothing)



    End Function

    ''' <summary>
    ''' Generalize a shape. 
    ''' </summary>
    ''' <param name="shp">Shape to generalize</param>
    ''' <param name="maxPoints">Maximum number of resulting points in shape's line. If ommited, Generalize removes every other point.</param>
    ''' <returns>Generalized shp.</returns>
    ''' <remarks>shapeObj.buffer and layerObj.queryByShape operations are faster with generalized shapes.</remarks>
    Public Shared Function Generalize(ByVal shp As shapeObj, Optional ByVal maxPoints As Integer = -1) As shapeObj
        '//always preserve the first and last points of lines and polygons
        Dim newShp As New shapeObj(shp.type)
        Dim newLine As New lineObj()
        Select Case shp.type
            Case MS_SHAPE_TYPE.MS_SHAPE_POINT
                Return shp
            Case MS_SHAPE_TYPE.MS_SHAPE_LINE, MS_SHAPE_TYPE.MS_SHAPE_POLYGON
                newLine.add(shp.line.get(0))
                For i As Integer = 1 To shp.line.numpoints - 1 Step 2
                    newLine.add(shp.line.get(i))
                Next
                newLine.add(shp.line.get(shp.line.numpoints - 1))
                newShp.add(newLine)
                If maxPoints <> -1 AndAlso newShp.line.numpoints > maxPoints Then
                    newShp = Generalize(newShp, maxPoints)
                End If
        End Select
        Return newShp
    End Function
End Class
