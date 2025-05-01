Imports System
Imports System.Diagnostics.Eventing
Imports System.Diagnostics.Tracing
Imports System.IO
Imports System.Runtime.InteropServices
Imports System.Threading
Imports System.Diagnostics

Module Program
    Sub Main(args As String())
        Dim xwhich As WhichCommand = 0
        Dim cmd As String = args(0)
        Dim fileName As String = String.Empty
        Dim bHasCommand As Boolean = False
        Select Case args(0)
            Case "-c" ' count bytes 
                xwhich = WhichCommand.Bytes
                bHasCommand = True
            Case "-w" ' Count Words 
                xwhich = WhichCommand.Words
                bHasCommand = True
            Case "-l" ' Count Lines 
                xwhich = WhichCommand.Lines
                bHasCommand = True
            Case "-m" ' Count Characters 
                xwhich = WhichCommand.Characters
                bHasCommand = True
            Case "-h"
                Console.WriteLine("Usage: wc [-c|-w|-l|-m] filename")
                Return
            Case Else ' Do All 
                xwhich = WhichCommand.Bytes Or WhichCommand.Lines Or WhichCommand.Words

        End Select
        If Console.IsInputRedirected Then
            Dim res = CountAttributes(Console.In, xwhich)
            displayValues(res.countBytes, res.countCharacters, res.countWords, res.countLines, fileName, xwhich)
        Else

            If bHasCommand And args.Length > 1 Then
                fileName = args(1)
            Else
                fileName = args(0)
            End If ' String.IsNullOrWhiteSpace(fileName) And args.Length > 1 
            If Not String.IsNullOrWhiteSpace(fileName) Then
                ' Do I have a File to process.... 
                If DoesFileExist(fileName) Then
                    Dim reader As New StreamReader(fileName, System.Text.Encoding.Default)
                    Dim res = CountAttributes(reader, xwhich)

                    displayValues(res.countBytes, res.countCharacters, res.countWords, res.countLines, fileName, xwhich)
                Else
                    Console.WriteLine("No File Provided")
                End If

            Else
                Console.WriteLine("No input provided.")
            End If
        End If


        Debug.WriteLine("Command: " & xwhich)
        Debug.WriteLine("---------------------")
        Debug.WriteLine("IsBytes: " & CStr(xwhich And WhichCommand.Bytes))
        Debug.WriteLine("Lines: " & CStr(xwhich And WhichCommand.Lines))
        Debug.WriteLine("Words: " & CStr(xwhich And WhichCommand.Words))
        Debug.WriteLine("Characters: " & CStr(xwhich And WhichCommand.Characters))
        Debug.WriteLine("---------------------")

    End Sub

    Private Sub displayValues(cBytes As Integer, cChars As Integer, cWords As Integer, cLines As Integer, filename As String, which As WhichCommand)

        If which.HasFlag(WhichCommand.Bytes) AndAlso cBytes > -1 Then
            Console.Write(vbTab)
            Console.Write(cBytes)
        End If
        If which.HasFlag(WhichCommand.Lines) AndAlso cLines > -1 Then
            Console.Write(vbTab)
            Console.Write(cLines)
        End If
        If which.HasFlag(WhichCommand.Words) AndAlso cWords > -1 Then
            Console.Write(vbTab)
            Console.Write(cWords)
        End If

        If which.HasFlag(WhichCommand.Characters) AndAlso cChars > -1 Then
            Console.Write(vbTab)
            Console.Write(cChars)
        End If

        If Not String.IsNullOrWhiteSpace(filename) Then
            Console.Write(vbTab)
            Dim fn As String = IO.Path.GetFileName(filename)
            Console.Write(fn)
        End If
        Console.WriteLine(String.Empty)

    End Sub


    Private Function DoesFileExist(filename As String) As Boolean
        DoesFileExist = False
        If System.IO.File.Exists(filename) Then
            DoesFileExist = True
        End If
    End Function

    Private Function CountAttributes(reader As TextReader, which As WhichCommand) As (countBytes As Integer, countCharacters As Integer, countWords As Integer, countLines As Integer)
        Dim b As Integer = 0
        Dim c As Integer = 0
        Dim w As Integer = 0
        Dim l As Integer = 0

        Dim line As String = Nothing
        While (reader.Peek() <> -1)
            line = reader.ReadLine()

            c += line.Count
            b += System.Text.Encoding.Default.GetByteCount(line) + System.Text.Encoding.Default.GetByteCount(Environment.NewLine)

            w += line.Split(New Char() {" "c}).Length
            l += 1
        End While

        Return (b, c, w, l)
    End Function



    Private Enum WhichCommand
        None = 0
        Bytes = 1
        Characters = 2
        Words = 4
        Lines = 8
    End Enum



End Module
