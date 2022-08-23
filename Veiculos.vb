Imports System.Net
Imports System.Data
Imports System.Net.Http
Imports System.IO
Imports RestSharp
Imports Newtonsoft.Json
Imports System.Text


Public Class Veiculos

    Private sendEmail As New sendEmail
    Private Client As RestClient

    Public Sub buscarmarcas()
        Try
            Dim NomeMontadora As String
            Dim CodMontadora As String


            Dim client = New WebClient()
            Dim url As String = "http://parallelum.com.br/fipe/api/v1/carros/marcas"
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
            Dim response = client.DownloadString(New Uri(url))
            Dim Retorno As List(Of marcas) = JsonConvert.DeserializeObject(Of List(Of marcas))(response)
            For Each teste In Retorno
                NomeMontadora = teste.nome.ToUpper
                CodMontadora = teste.codigo
                inseremarca(NomeMontadora, CodMontadora)
            Next
        Catch ex As Exception

        End Try
    End Sub

    Private Sub inseremarca(ByVal NomeMontadora As String, ByVal CodMontadora As Integer)
        Dim query As String
        Dim ds As New DataSet
        Try
            query = "SELECT * FROM TB_PRODUTOMONTADORA WHERE NOME = '" & NomeMontadora & "' AND REGISTRO_ORIGINAL_ID = " & CodMontadora & " AND STATUS <> 0 AND GRUPO_ID = 9"
            ds = ClsFuncoes.WsMotor.Consultar(query)

            If ds.Tables(0).Rows.Count = 0 Then
                query = "INSERT INTO [dbo].[TB_PRODUTOMONTADORA]
               ([GRUPO_ID]
               ,[NOME]
               ,[ARQUIVADO]
               ,[OBS]
               ,[ALTERACAO_ID]
               ,[ACAO_ID]
               ,[ULT_ALT]
               ,[ULT_USER_ID]
               ,[STATUS]
               ,[USER_ID]    
               ,[REGISTRO_ORIGINAL_ID] 
               ,[ALIAS]
               )
                VALUES
               (9
               ,'" & NomeMontadora & "'
               ,0
               ,'IMPORTADO DA TABELA FIPE PELO INTEGRADOR'
               ,1
               ,1
               ,GETDATE()
               ,1
               ,1
               ,1      
               ," & CodMontadora & "
               ,'" & NomeMontadora & "')"
                ClsFuncoes.WsMotor.ExecutarSimples(query)
            End If
        Catch ex As Exception

        End Try

    End Sub


    Public Sub buscamodelo()
        Dim query As String
        Dim ds As New DataSet

        Try

            query = "SELECT MONTADORA_ID, REGISTRO_ORIGINAL_ID  FROM TB_PRODUTOMONTADORA WHERE STATUS <> 0 AND GRUPO_ID = 9 AND  REGISTRO_ORIGINAL_ID <> ''"
            ds = ClsFuncoes.WsMotor.Consultar(query)
            If ds.Tables(0).Rows.Count <> 0 Then
                For Each linha As DataRow In ds.Tables(0).Rows
                    Dim NomeModelo As String
                    Dim CodModelo As String
                    Dim client = New WebClient()
                    Dim url As String = "http://parallelum.com.br/fipe/api/v1/carros/marcas/" & linha.Item("REGISTRO_ORIGINAL_ID") & "/modelos"
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
                    Dim response = client.DownloadString(New Uri(url))
                    'Dim Retorno As List(Of veiculo) = JsonConvert.DeserializeObject(Of List(Of veiculo))(response)
                    Dim Retorno As veiculo = JsonConvert.DeserializeObject(Of veiculo)(response)
                    Dim conta As Integer = 0
                    For Each teste In Retorno.modelos
                        NomeModelo = Retorno.modelos(conta).nome.ToUpper
                        CodModelo = Retorno.modelos(conta).codigo
                        inseremodelo(NomeModelo, CodModelo, linha.Item("MONTADORA_ID"))
                        conta = conta + 1
                    Next
                    conta = 0
                Next
            End If

        Catch ex As Exception

        End Try
    End Sub

    Private Sub inseremodelo(ByVal NomeModelo As String, ByVal CodModelo As Int64, ByVal MONTADORA_ID As Int64)
        Dim query As String
        Dim ds As New DataSet
        Try
            query = "SELECT * FROM TB_PRODUTOMONTADORAMODELO WHERE MODELO = '" & NomeModelo & "' AND MONTADORA_ID = " & MONTADORA_ID & " AND REGISTRO_ORIGINAL_ID = " & CodModelo & " AND STATUS <> 0"
            ds = ClsFuncoes.WsMotor.Consultar(query)
            If ds.Tables(0).Rows.Count = 0 Then
                query = "INSERT INTO [dbo].[TB_PRODUTOMONTADORAMODELO]
                       ([GRUPO_ID]
                       ,[MODELO]
                       ,[ARQUIVADO]
                       ,[OBS]
                       ,[MONTADORA_ID]
                       ,[ALTERACAO_ID]
                       ,[ACAO_ID]
                       ,[ULT_ALT]
                       ,[ULT_USER_ID]
                       ,[STATUS]
                       ,[USER_ID]          
                       ,[REGISTRO_ORIGINAL_ID])
                         VALUES
                       (9
                       ,'" & NomeModelo & "'
                       ,0
                       ,'IMPORTADO DA TABELA FIPE PELO INTEGRADOR'
                       ," & MONTADORA_ID & "
                       ,1
                       ,1
                       ,GETDATE()
                       ,1
                       ,1
                       ,1 
                       ," & CodModelo & ")"
                ClsFuncoes.WsMotor.ExecutarSimples(query)
                System.Threading.Thread.Sleep(1000)
            End If

        Catch ex As Exception

        End Try
    End Sub
    Public Class veiculo
        Public Property modelos As New List(Of modelos)
        Public Property anos As New List(Of anos)
    End Class

    Public Class modelos
        Public Property nome As String
        Public Property codigo As String
    End Class

    Public Class anos
        Public Property nome As String
        Public Property codigo As String
    End Class


    Public Class marcas
            Public Property nome As String
            Public Property codigo As String
        End Class

    End Class
