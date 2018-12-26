Imports System.ComponentModel
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.IO
Imports System.Net
Imports System.Text
Imports Newtonsoft.Json
Imports Newtonsoft.Json.Linq

Public Class BotCore
    Public Token As String
    Public Req_Timeout As Integer
    Public Log As String
    Public Auto_Downlaod As Boolean = False
    Private ReadOnly Req_Sender As WebRequest
    Private Working_TH As New Threading.Thread(AddressOf Service)
    Public Event Message_Recived(ByVal Type As Message_Types, ByVal Message As Object)
    Public Event Download_Completed(ByVal Failed As Boolean)
    Dim Ran As New Random
    Shared Function Raw_Upload(ByVal File_Name As String, ByVal Token As String) As Upload_Result
        Dim clientt As WebClient = New WebClient
        Dim responseBinary() As Byte = clientt.UploadFile(New Uri("https://bot.sapp.ir/" & Token & "/uploadFile"), File_Name)
        Dim buffer As String = Encoding.UTF8.GetString(responseBinary)
        Dim Final As New Upload_Result()
        If GetElement(buffer, "resultMessage") = "OK" Then
            Final.Result = True
        Else
            Final.Result = False
        End If
        Final.Result_Code = GetElement(buffer, "resultCode")
        Final.URL = GetElement(buffer, "fileUrl")
        Return Final
    End Function
    Public Structure Upload_Result
        Dim Result As Boolean
        Dim Result_Code As Integer
        Dim URL As String
        Public Sub New(ByVal Result As Boolean, ByVal Result_Code As String, ByVal URL As String)
            Me.Result = Result
            Me.Result_Code = Result_Code
            Me.URL = URL
        End Sub
    End Structure
    Public Class Incoming_Message
        Public Structure Text_Message
            Public Raw As String
            Public user As String
            Public Text As String
            Public Time As String
        End Structure
        Public Structure Stop_Message
            Dim user As String
            Dim Raw As String
            Dim Text As String
            Dim Time As String
        End Structure
        Public Structure Start_Message
            Dim user As String
            Dim Raw As String
            Dim Text As String
            Dim Time As String
        End Structure
        Public Structure Image_Message
            Dim user As String
            Dim Raw As String
            Dim Text As String
            Dim Time As String
            Dim file_Size As Double
            Dim File_Url As String
            Dim File_ID As String
            Dim File_Name As String
            Dim ThumbnailUrl As String
            Dim ImageWidth As Double
            Dim ImageHeight As Double
            Dim Image As Image
            Dim Thumbnail_Image As Image
            Dim Ran As Random
            Public Key_board As String
            Public Function Get_image(ByVal Token As String) As String
                Ran = New Random
                Dim random As Integer = Ran.Next(1000, 9000)
                Download("https://bot.sapp.ir/" & Token & "/downloadFile/" & File_Url, Environment.CurrentDirectory & "SoroushDownload\Image\" & Path.GetFileNameWithoutExtension(File_Name) & random & "." & Path.GetExtension(File_Name))
                Image = Image.FromFile(Environment.CurrentDirectory & "\SoroushDownload\Image\" & Path.GetFileNameWithoutExtension(File_Name) & random & "." & Path.GetExtension(File_Name))
                Return Environment.CurrentDirectory & "\SoroushDownload\Image\" & Path.GetFileNameWithoutExtension(File_Name) & random & "." & Path.GetExtension(File_Name)
            End Function
            Public Function Get_Thumbnail(ByVal Token As String) As String
                Ran = New Random
                Dim random As Integer = Ran.Next(1000, 9000)
                Download("https://bot.sapp.ir/" & Token & "/downloadFile/" & File_Url, Environment.CurrentDirectory & "SoroushDownload\Image\" & Path.GetFileNameWithoutExtension(ThumbnailUrl) & random & "." & Path.GetExtension(ThumbnailUrl))
                Thumbnail_Image = Image.FromFile(Environment.CurrentDirectory & "\SoroushDownload\Image\" & Path.GetFileNameWithoutExtension(ThumbnailUrl) & random & "." & Path.GetExtension(ThumbnailUrl))
                Return Environment.CurrentDirectory & "\SoroushDownload\Image\" & Path.GetFileNameWithoutExtension(File_Name) & random & "." & Path.GetExtension(File_Name)
            End Function
            Private Sub Download(ByVal Url As String, ByVal FileName As String)
                Dim client As New WebClient()
                client.DownloadFile(New Uri(Url), FileName)
            End Sub
        End Structure


        Public Structure Video_Message
            Dim user As String
            Dim Raw As String
            Dim Text As String
            Dim Time As String
            Dim file_Size As Double
            Dim File_Url As String
            Dim File_ID As String
            Dim File_Name As String
            Dim FileDuration As Double
            Dim ThumbnailUrl As String
            Dim ThumbnailWidth As Double
            Dim ThumbnailHeight As Double
        End Structure
        Public Structure Push_To_Talk_Message
            Dim user As String
            Dim Raw As String
            Dim Text As String
            Dim Time As String
            Dim file_Size As Double
            Dim File_Url As String
            Dim File_ID As String
            Dim File_Name As String
            Dim FileDuration As Double
        End Structure
        Public Structure Location_Message
            Dim user As String
            Dim Raw As String
            Dim time As String
            Dim latitude As Double
            Dim longitude As Double
        End Structure
    End Class
    Public Class Outgoing_Message
        Public Structure Push_to_Talk
            Dim File_Name As String
            Dim Text As String
            Dim Path_to_Pushtotalk As String
            Dim Path_is_Local As Boolean
            Public Sub New(Path_to_Pushtotalk As String, ByVal Path_is_Local As Boolean, Optional Text As String = "", Optional ByVal File_Name As String = "")
                Me.File_Name = File_Name
                Me.Text = Text
                Me.Path_to_Pushtotalk = Path_to_Pushtotalk
                Me.Path_is_Local = Path_is_Local
            End Sub
        End Structure
        Public Structure Image_Message
            Dim Path_to_Image As String
            Dim Image_Name As String
            Dim Image_thumbnail_URL As String
            Dim Text As String
            Dim Keyboard As String
            Private Image_Object As Image
            Private Ran As Random
            Dim Path_is_Local As Boolean
            Public Sub New(ByVal Path_to_Image As String, Path_is_Local As Boolean, Optional ByVal Text As String = "", Optional ByVal Image_thumbnail_URL As String = "", Optional ByVal Image_Name As String = "", Optional ByVal Keyboard As String = "")
                Me.Keyboard = Keyboard
                Me.Path_to_Image = Path_to_Image
                Me.Image_thumbnail_URL = Image_thumbnail_URL
                Me.Image_Name = Image_Name
                Me.Text = Text
                Me.Path_is_Local = Path_is_Local
            End Sub
            Public Sub New(ByVal Image As Image, Optional ByVal Text As String = "", Optional ByVal Image_thumbnail_URL As String = "", Optional ByVal Image_Name As String = "", Optional ByVal Keyboard As String = "")
                Ran = New Random
                Image_Object = Image
                Dim Random_Path As String = Environment.CurrentDirectory & "/temp/" & Ran.Next(1000, 9000) & ".jpg"
                Image_Object.Save(Random_Path)
                Me.Keyboard = Keyboard
                Me.Path_to_Image = Random_Path
                Me.Image_thumbnail_URL = Image_thumbnail_URL
                Me.Image_Name = Image_Name
                Me.Text = Text
                Path_is_Local = True
            End Sub
        End Structure
        Public Structure Text_Message
            Dim Text As String
            Dim Keyboard As String
            Public Sub New(ByVal Text As String, Optional ByVal Keyboard As String = "")
                Me.Keyboard = Keyboard
                Me.Text = Text
            End Sub
        End Structure
        Public Structure Video_Message
            Dim Path_to_Video As String
            Dim Video_Name As String
            Dim Keyboard As String
            Dim text As String
            Dim Path_is_Local As Boolean
            Dim Thumbnail_Url As String
            Dim Thumbnail_Height As Integer
            Dim Thumbnail_Width As Integer
            Dim File_Name As String
            Public Sub New(ByVal Path_to_Video As String, ByVal Path_is_Local As Boolean, Optional ByVal Text As String = "", Optional ByVal Keyboard As String = "", Optional ByVal Thumbnail_Url As String = "", Optional ByVal Thumbnail_Height As Integer = 0, Optional ByVal Thumbnail_Width As Integer = 0, Optional ByVal Video_Name As String = "")
                Me.Keyboard = Keyboard
                Me.text = Text
                Me.Path_is_Local = Path_is_Local
                Me.Path_to_Video = Path_to_Video
                Me.Video_Name = File_Name
                Me.Thumbnail_Url = Thumbnail_Url
                Me.Thumbnail_Height = Thumbnail_Height
                Me.Thumbnail_Width = Thumbnail_Width
            End Sub
        End Structure
        Public Structure Location_Message
            Dim Latitude As Double
            Dim longitude As Double
            Public Sub New(ByVal Latitude As Double, ByVal longitude As Double)
                Me.Latitude = Latitude
                Me.longitude = longitude
            End Sub
        End Structure
    End Class


    Public Sub Start()
        Working_TH.Priority = Threading.ThreadPriority.Highest
        Working_TH.Start()
    End Sub
    Public Sub [Stop]()
        Working_TH.Abort()
    End Sub

    Public Sub New(ByVal Token As String, Optional HighPerformance As Boolean = True, Optional Auto_Downlaod As Boolean = True, Optional Req_Timeout As Integer = 600000, Optional InternalDB As Boolean = False)
        Me.Token = Token

        Me.Req_Timeout = Req_Timeout
    End Sub

    Shared Function GetElement(ByVal Raw As String, ByVal Element As String) As String
        Dim json As String = Raw
        Dim ser As JObject = JObject.Parse(json)
        Dim data As List(Of JToken) = ser.Children().ToList
        GetElement = String.Empty

        Parallel.ForEach(data, Sub(ByVal item As JProperty)
                                   item.CreateReader()

                                   If item.Name = Element Then
                                       GetElement = item.Value
                                   End If
                               End Sub)
        Return GetElement
    End Function
    Private Function Send_Json(ByVal Json As String) As String
        Dim url As String = "https://bot.sapp.ir/" & Token & "/sendMessage"
        Dim request = DirectCast(WebRequest.Create(url), HttpWebRequest)
        Dim data = Encoding.UTF8.GetBytes(Json)
        request.Method = "POST"
        request.ContentType = "Application/json "
        request.ContentLength = data.Length
        Using stream = request.GetRequestStream()
            stream.Write(data, 0, data.Length)
        End Using
        Dim response = DirectCast(request.GetResponse(), HttpWebResponse)
        Dim responseString = New StreamReader(response.GetResponseStream()).ReadToEnd()
        Return responseString
    End Function

    Public Sub Send_Message(ByVal Message_Type As Message_Types, ByVal Message As Object, ByVal User As String)
        Select Case Message_Type
            Case Message_Types.Text
                Dim Buffer_Message As Outgoing_Message.Text_Message = Message
                Dim P_Message As New S_TEXT With {.body = Buffer_Message.Text, .[to] = User, .type = "TEXT"}
                Dim a_json As String = JsonConvert.SerializeObject(P_Message)
                Send_Json(a_json)
            Case Message_Types.FileWithImage
                Dim Buffer_Message As Outgoing_Message.Image_Message = Message
                Dim Size As Double = FileLen(Buffer_Message.Path_to_Image)
                Dim Upload_URL As String = ""
                If (Buffer_Message.Path_is_Local) Then
                    Upload_URL = GetElement(Upload(Buffer_Message.Path_to_Image), "fileUrl")
                Else
                    Upload_URL = Buffer_Message.Path_to_Image
                End If
                Dim bmp As New Bitmap(Buffer_Message.Path_to_Image)
                Dim P_Message As New S_IMAGE With {.fileName = Buffer_Message.Image_Name, .fileSize = Size, .fileType = "IMAGE", .fileUrl = Upload_URL, .to = User, .imageHeight = bmp.Height, .imageWidth = bmp.Width, .type = "FILE", .body = Buffer_Message.Text, .thumbnailUrl = Buffer_Message.Image_thumbnail_URL}
                bmp.Dispose()
                Dim a_json As String = JsonConvert.SerializeObject(P_Message)
                Send_Json(a_json)
            Case Message_Types.FileWithVideo
                Dim Buffer_Message As Outgoing_Message.Video_Message = Message
                Dim Size As Double = FileLen(Buffer_Message.Path_to_Video)
                Dim Upload_URL As String = ""
                If (Buffer_Message.Path_is_Local) Then
                    Upload_URL = GetElement(Upload(Buffer_Message.Path_to_Video), "fileUrl")
                Else
                    Upload_URL = Buffer_Message.Path_to_Video
                End If
                If Buffer_Message.File_Name = "" Then
                    Buffer_Message.File_Name = Path.GetFileName(Buffer_Message.Path_to_Video)
                End If
                Dim ffProbe As NReco.VideoInfo.FFProbe = New NReco.VideoInfo.FFProbe
                Dim videoInfo = ffProbe.GetMediaInfo(Buffer_Message.Path_to_Video)
                Dim P_Message As New S_VIDEO With {.body = Buffer_Message.text, .fileDuration = videoInfo.Duration.Seconds, .fileName = Buffer_Message.File_Name, .fileSize = Size, .fileType = "VIDEO", .fileUrl = Upload_URL, .thumbnailHeight = Buffer_Message.Thumbnail_Height, .thumbnailUrl = Buffer_Message.Thumbnail_Url, .thumbnailWidth = Buffer_Message.Thumbnail_Width, .to = User, .type = "FILE"}
                Dim a_json As String = JsonConvert.SerializeObject(P_Message)
                Send_Json(a_json)
            Case Message_Types.FileWithPushToTalk
                Dim Buffer_Message As Outgoing_Message.Push_to_Talk = Message
                Dim Size As Double = FileLen(Buffer_Message.Path_to_Pushtotalk)
                Dim Upload_URL As String = ""
                If (Buffer_Message.Path_is_Local) Then
                    Upload_URL = GetElement(Upload(Buffer_Message.Path_to_Pushtotalk), "fileUrl")
                Else
                    Upload_URL = Buffer_Message.Path_to_Pushtotalk
                End If
                If Buffer_Message.File_Name = "" Then
                    Buffer_Message.File_Name = Path.GetFileName(Buffer_Message.Path_to_Pushtotalk)
                End If
                Dim ffProbe As NReco.VideoInfo.FFProbe = New NReco.VideoInfo.FFProbe
                Dim videoInfo = ffProbe.GetMediaInfo(Buffer_Message.Path_to_Pushtotalk)
                Dim P_Message As New S_PUSH_TO_TALK With {.body = Buffer_Message.Text, .fileDuration = videoInfo.Duration.Seconds, .fileName = Buffer_Message.File_Name, .fileSize = Size, .fileType = "PUSH_TO_TALK", .fileUrl = Upload_URL, .to = User, .type = "FILE"}
                Dim a_json As String = JsonConvert.SerializeObject(P_Message)
                Send_Json(a_json)
            Case Message_Types.Keyboard_Change



            Case Message_Types.Location
                Dim Buffer_Message As Outgoing_Message.Location_Message = Message
                Dim P_Message As New S_LOCATION With {.to = User, .type = "LOCATION", .latitude = Buffer_Message.Latitude, .longitude = Buffer_Message.longitude}
                Dim a_json As String = JsonConvert.SerializeObject(P_Message)
                Send_Json(a_json)
        End Select

    End Sub
    Private Sub Check_Push(ByVal Json As String)
        Try
            Select Case GetElement((Json), "type").ToUpper
                Case "START"
                    Dim Message As New Incoming_Message.Start_Message With {.Raw = Json, .Text = String.Empty, .Time = Date_Convert(GetElement(Json, "time")), .user = GetElement(Json, "from")}
                    RaiseEvent Message_Recived(Message_Types.Start, Message)
                Case "STOP"
                    Dim Message As New Incoming_Message.Stop_Message With {.Raw = Json, .Text = String.Empty, .Time = Date_Convert(GetElement(Json, "time")), .user = GetElement(Json, "from")}
                    RaiseEvent Message_Recived(Message_Types.Stop, Message)
                Case "TEXT"
                    Dim Message As New Incoming_Message.Text_Message With {.Raw = Json, .Text = GetElement(Json, "body"), .Time = Date_Convert(GetElement(Json, "time")), .user = GetElement(Json, "from")}
                    RaiseEvent Message_Recived(Message_Types.Text, Message)
                Case "FILE"
                    Select Case GetElement(Json, "fileType")
                        Case "IMAGE"
                            Dim Message As New Incoming_Message.Image_Message With {.File_Name = GetElement(Json, "fileName"), .file_Size = GetElement(Json, "fileSize"), .File_Url = GetElement(Json, "fileUrl"), .ImageHeight = GetElement(Json, "imageHeight"), .ImageWidth = GetElement(Json, "imageWidth"), .Raw = Json, .ThumbnailUrl = GetElement(Json, "thumbnailUrl"), .Time = Date_Convert(GetElement(Json, "time")), .user = GetElement(Json, "from"), .File_ID = GetElement(Json, "fileid"), .Text = GetElement(Json, "body")}
                            If Auto_Downlaod Then
                                Download("https://bot.sapp.ir/" & Token & "/downloadFile/" & Message.File_Url, Environment.CurrentDirectory & "SoroushDownload\Image\" & Message.File_Name)
                            End If
                            RaiseEvent Message_Recived(Message_Types.FileWithImage, Message)
                        Case "VIDEO"
                            Dim Message As New Incoming_Message.Video_Message With {.Raw = Json, .FileDuration = GetElement(Json, "fileDuration"), .File_ID = GetElement(Json, "fileid"), .File_Name = GetElement(Json, "fileName"), .file_Size = GetElement(Json, "fileSize"), .File_Url = GetElement(Json, "fileUrl"), .Text = GetElement(Json, "body"), .ThumbnailHeight = GetElement(Json, "thumbnailHeight"), .ThumbnailUrl = GetElement(Json, "thumbnailUrl"), .ThumbnailWidth = GetElement(Json, "thumbnailWidth"), .Time = GetElement(Json, "time"), .user = GetElement(Json, "from")}
                            If Auto_Downlaod Then
                                Download("https://bot.sapp.ir/" & Token & "/downloadFile/" & Message.File_Url, Environment.CurrentDirectory & "SoroushDownload\Image\" & Message.File_Name)
                            End If
                            RaiseEvent Message_Recived(Message_Types.FileWithVideo, Message)
                        Case "PUSH_TO_TALK"
                            Dim Message As New Incoming_Message.Push_To_Talk_Message With {.FileDuration = GetElement(Json, "fileDuration"), .File_ID = GetElement(Json, "fileid"), .File_Name = GetElement(Json, "fileName"), .file_Size = GetElement(Json, "fileSize"), .File_Url = GetElement(Json, "fileUrl"), .Raw = Json, .Text = GetElement(Json, "body"), .Time = GetElement(Json, "time"), .user = GetElement(Json, "from")}
                            If Auto_Downlaod Then
                                Download("https://bot.sapp.ir/" & Token & "/downloadFile/" & Message.File_Url, Environment.CurrentDirectory & "SoroushDownload\Image\" & Message.File_Name)
                            End If
                            RaiseEvent Message_Recived(Message_Types.FileWithPushToTalk, Message)
                    End Select
                Case "LOCATION"
                    Dim Message As New Incoming_Message.Location_Message With {.latitude = GetElement(Json, "latitude"), .longitude = GetElement(Json, "longitude"), .Raw = Json, .user = GetElement(Json, "from")}
                    RaiseEvent Message_Recived(Message_Types.Location, Message)
            End Select

        Catch ex As Exception
            MsgBox(ex.Message, 16)
        End Try
    End Sub
    Private Sub Download(ByVal Url As String, ByVal FileName_to_Save As String)
        Dim client As New WebClient()
        AddHandler client.DownloadFileCompleted, AddressOf Download_Complete_Private
        client.DownloadFileAsync(New Uri(Url), FileName_to_Save)
    End Sub
    Private Sub Download_Complete_Private(ByVal sender As Object, ByVal e As AsyncCompletedEventArgs)
        RaiseEvent Download_Completed(e.Cancelled)
    End Sub
    Private Function Upload(ByVal File_Name As String) As String
        Dim clientt As WebClient = New WebClient
        Dim responseBinary() As Byte = clientt.UploadFile(New Uri("https://bot.sapp.ir/" & Token & "/uploadFile"), File_Name)
        Return Encoding.UTF8.GetString(responseBinary)
    End Function

    Private Sub Service()
        Dim url As String = "https://bot.sapp.ir/" & Token & "/getMessage"
        Dim req As WebRequest = WebRequest.Create(url)
        req.Method = "GET"
        Dim json As String
        req.ContentType = "application/stream+json"
        req.Timeout = Req_Timeout
        Dim resp = req.GetResponse
        While True
loopy:
            Try
                Dim stream = resp.GetResponseStream
                Dim re = New StreamReader(stream)
                json = re.ReadLine
                json = json.Replace("data:", "")
                Threading.ThreadPool.QueueUserWorkItem(Sub() Check_Push(json))
            Catch ex As Exception
                GoTo loopy
            End Try
        End While
    End Sub

    Public Enum Message_Types
        Keyboard_Change
        Text
        FileWithImage
        FileWithVideo
        FileWithPushToTalk
        Location
        Start
        [Stop]
    End Enum
    Private Function Date_Convert(ByVal raw As String) As Date
        Return DateTimeOffset.FromUnixTimeMilliseconds(raw).LocalDateTime
    End Function

    Private Structure S_TEXT
        Dim [to] As String
        Dim body As String
        Dim type As String
    End Structure
    Private Structure S_IMAGE
        Dim [to] As String
        Dim body As String
        Dim type As String
        Dim fileType As String
        Dim fileSize As Double
        Dim fileName As String
        Dim imageWidth As Double
        Dim imageHeight As Double
        Dim fileUrl As String
        Dim thumbnailUrl As String
    End Structure
    Private Structure S_VIDEO
        Dim [to] As String
        Dim body As String
        Dim type As String
        Dim fileType As String
        Dim fileDuration As String
        Dim fileSize As Double
        Dim fileName As String
        Dim thumbnailWidth As Double
        Dim thumbnailHeight As Double
        Dim fileUrl As String
        Dim thumbnailUrl As String
    End Structure
    Private Structure S_PUSH_TO_TALK
        Dim [to] As String
        Dim body As String
        Dim type As String
        Dim fileSize As Double
        Dim fileName As String
        Dim fileType As String
        Dim fileUrl As String
        Dim fileDuration As Double
    End Structure
    Private Structure S_LOCATION
        Dim [to] As String
        Dim type As String
        Dim latitude As String
        Dim longitude As String
    End Structure

    Private Structure S_CHANGE_KEYBOARD
        Dim [to] As String
        Dim type As String
        Dim keyboard As String
    End Structure
End Class