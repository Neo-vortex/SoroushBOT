# SoroushBOT
Create SoroushBOT via .NET platform

Version 0.0.1 pre-alpha


How to create a bot ? 

    Dim WithEvents Client As New SoroushBOT.BotCore("YOUR_TOKEN", Req_Timeout:=Threading.Timeout.Infinite)
    
 Handle incomming message ?
 
    Private Sub R_V(ByVal type As BotCore.Message_Types, ByVal message As Object) Handles Client.Message_Recived
     Select Case type
            Case BotCore.Message_Types.Text
                Dim Message As BotCore.Incoming_Message.Text_Message = message
                
             Case ....
             .
             .
             .
      End Select
      
      
   How to send message ? 
   
     Dim Message As New BotCore.Outgoing_Message.Text_Message("OK")
     Client.Send_Message(BotCore.Message_Types.Message, "CHAT_ID")
     
     
   Known issue:
   Keyboard sending is not working 
