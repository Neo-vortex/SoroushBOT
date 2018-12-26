Imports System.Net
Imports System.Text
Imports SoroushBOT
Module Module1
    Dim WithEvents Client As New BotCore("YOUR_TOKEN", Req_Timeout:=Threading.Timeout.Infinite)
    Sub Main()
        Client.Start()
    End Sub
    Private Sub M_r(ByVal type As BotCore.Message_Types, ByVal message As Object) Handles Client.Message_Recived

        Select Case type
            Case BotCore.Message_Types.Text
                Console.WriteLine("txt")
                Dim n As BotCore.Incoming_Message.Text_Message = message
                Dim n2 As New BotCore.Outgoing_Message.Text_Message("OK")
                Console.WriteLine(n.Text & vbCrLf & n.user & vbCrLf & n.Time & "     Text")
                Client.Send_Message(BotCore.Message_Types.Location, n2, n.user)
            Case BotCore.Message_Types.FileWithImage
                Console.WriteLine("image")
                Dim n As BotCore.Incoming_Message.Image_Message = message
                Console.WriteLine(n.Text & vbCrLf & n.user & vbCrLf & n.Time & "     FileWithImage")
            Case BotCore.Message_Types.FileWithPushToTalk
                Console.WriteLine("FileWithPushToTalk")
                Dim n As BotCore.Incoming_Message.Push_To_Talk_Message = message
                Console.WriteLine(n.Text & vbCrLf & n.user & vbCrLf & n.Time & "     FileWithPushToTalk")
            Case BotCore.Message_Types.FileWithVideo
                Console.WriteLine("FileWithVideo")
                Dim n As BotCore.Incoming_Message.Video_Message = message
                Console.WriteLine(n.Text & vbCrLf & n.user & vbCrLf & n.Time & "     FileWithVideo")
            Case BotCore.Message_Types.Location
                Console.WriteLine("location")
                Dim n As BotCore.Incoming_Message.Location_Message = message
                Console.WriteLine("location : " & vbCrLf & n.user & vbCrLf & n.time & "     location")
        End Select


    End Sub

End Module
