Imports System.Collections.Specialized
Imports System.Collections
'Imports System.Web.HttpRequest
Imports System.Web
Imports System


Public Class QueryString
    Inherits NameValueCollection
    Private _document As String

    Public ReadOnly Property Document() As String
        Get
            Return _document
        End Get
    End Property

    Sub New()
    End Sub
    ''' <summary>
    ''' Recibe del request.querystring los valores
    ''' </summary>
    ''' <param name="clone">QueryString de la pagina actual</param>
    ''' <remarks></remarks>
    Sub New(ByVal clone As NameValueCollection)
        MyBase.New(clone)
    End Sub
    ''' <summary>
    ''' permite convertir un string en un NameValueCollection con el QueryString
    ''' </summary>
    ''' <param name="vConversion">String que concatena todo el queryString</param>
    ''' <remarks></remarks>
    Sub New(ByVal vConversion As String)
        MyBase.New(FromUrl(vConversion))
    End Sub
    'Public Shared Function FromCurrent() As QueryString
    '    Return FromUrl(HttpContext.Current.Request.Url.AbsoluteUri)
    'End Function

    Public Shared Function FromUrl(ByVal url As String) As QueryString
        Dim parts As String() = url.Split("?".ToCharArray)
        Dim qs As QueryString = New QueryString
        qs._document = parts(0)
        If parts.Length = 1 Then
            Return qs
        End If
        Dim keys As String() = parts(1).Split("&".ToCharArray)

        For Each key As String In keys
            Dim part As String() = key.Split("=".ToCharArray)
            If part.Length = 1 Then
                qs.Add(part(0), "")
            Else
                qs.Add(part(0), part(1))
            End If
        Next
        Return qs
    End Function

    Public Sub ClearAllExcept(ByVal except As String)
        ClearAllExcept(New String() {except})
    End Sub

    Public Sub ClearAllExcept(ByVal except As String())
        Dim toRemove As ArrayList = New ArrayList
        For Each s As String In Me.AllKeys
            For Each e As String In except
                If s.ToLower = e.ToLower Then
                    If Not toRemove.Contains(s) Then
                        toRemove.Add(s)
                    End If
                End If
            Next
        Next
        For Each s As String In toRemove
            Me.Remove(s)
        Next
    End Sub

    Public Overloads Overrides Sub Add(ByVal name As String, ByVal value As String)
        If Not (Me(name) Is Nothing) Then
            Me(name) = value
        Else
            MyBase.Add(name, value)
        End If
    End Sub

    Public Overloads Overrides Function ToString() As String
        Return ToString(False)
    End Function

    Public Overloads Function ToString(ByVal includeUrl As Boolean) As String
        Dim parts(Me.Count) As String
        Dim keys As String() = Me.AllKeys

        Dim i As Integer = 0
        While i < keys.Length
            parts(i) = keys(i) + "=" + HttpContext.Current.Server.UrlEncode(Me(keys(i)))
            System.Math.Min(System.Threading.Interlocked.Increment(i), i - 1)
        End While




        Dim url As String = String.Join("&", parts)
        If (Not (url Is Nothing) OrElse Not (url = String.Empty)) AndAlso Not url.StartsWith("?") Then
            url = "?" + url
        End If
        If includeUrl Then
            url = Me._document + url
        End If
        Return url
    End Function
End Class