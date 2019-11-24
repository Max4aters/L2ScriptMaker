Public Class SkillAcquireEditor

    'fighter_begin
    'skill_begin	/* [럭키] */	skill_name = [s_lucky]	get_lv = 1	lv_up_sp = 0	auto_get = true	item_needed = {}	skill_end
    'skill_begin	/* [엑스퍼티즈 D] */	skill_name = [s_expertise_d]	get_lv = 20	lv_up_sp = 0	auto_get = true	item_needed = {}	skill_end
    'fighter_end
    'include_fighter	


    Dim UpdateCell As Boolean = True

    Dim PrevSelClass As Integer = -1

    Dim ItemPch(0) As String
    Dim SkillPch(0) As String

    Private Structure SkillAcquire
        Dim skill_name As String
        Dim get_lv As Integer
        Dim lv_up_sp As Integer
        Dim auto_get As Boolean
        Dim item_needed1 As String
        Dim item_needed1_value As Integer
        Dim item_needed2 As String
        Dim item_needed2_value As Integer
        Dim note As String
    End Structure

    Dim ClassAmount(100) As Integer
    Dim ClassInherits(100) As String
    Dim Skillss(100, 1000) As SkillAcquire ' (1 - number of class's, 2 - amount skills in class)

    Private Function LoadAcquire(ByVal Number As Integer) As Boolean

        Dim iTemp As Integer
        If PrevSelClass = -1 Then Exit Function

        DataGridView.Rows.Clear()

        For iTemp = 0 To ClassAmount(Number)
            DataGridView.Rows.Add()

            If Skillss(Number, iTemp).skill_name <> Nothing Then
                DataGridView.Item(0, iTemp).Value = Skillss(Number, iTemp).skill_name
            Else
                DataGridView.Item(0, iTemp).Value = skill_name.Items(0).ToString
            End If
            If Skillss(Number, iTemp).get_lv > 0 Then
                DataGridView.Item(1, iTemp).Value = Skillss(Number, iTemp).get_lv
            Else
                DataGridView.Item(1, iTemp).Value = 0
            End If
            If Skillss(Number, iTemp).lv_up_sp > 0 Then
                DataGridView.Item(2, iTemp).Value = Skillss(Number, iTemp).lv_up_sp
            Else
                DataGridView.Item(2, iTemp).Value = 0
            End If
            If Skillss(Number, iTemp).auto_get = False Or Skillss(Number, iTemp).auto_get = True Then
                DataGridView.Item(3, iTemp).Value = Skillss(Number, iTemp).auto_get.ToString
            Else
                DataGridView.Item(3, iTemp).Value = "False"
            End If
            If Skillss(Number, iTemp).item_needed1 <> Nothing Then
                DataGridView.Item(4, iTemp).Value = Skillss(Number, iTemp).item_needed1
            Else
                DataGridView.Item(4, iTemp).Value = Nothing
            End If
            If Skillss(Number, iTemp).item_needed1_value <> Nothing Then
                DataGridView.Item(5, iTemp).Value = Skillss(Number, iTemp).item_needed1_value
            Else
                DataGridView.Item(5, iTemp).Value = Nothing
            End If
            If Skillss(Number, iTemp).item_needed2 <> Nothing Then
                DataGridView.Item(6, iTemp).Value = Skillss(Number, iTemp).item_needed2
            Else
                DataGridView.Item(6, iTemp).Value = Nothing
            End If
            If Skillss(Number, iTemp).item_needed2_value <> Nothing Then
                DataGridView.Item(7, iTemp).Value = Skillss(Number, iTemp).item_needed2_value
            Else
                DataGridView.Item(7, iTemp).Value = Nothing
            End If

            If Skillss(Number, iTemp).note <> "" Then
                DataGridView.Item(8, iTemp).Value = Skillss(Number, iTemp).note
            Else
                DataGridView.Item(8, iTemp).Value = Nothing
            End If
        Next

        If AutoSortCheckBox.Checked = True Then
            DataGridView.Sort(DataGridView.Columns(DataGridView.Columns(AutoSortComboBox.Text).Name), System.ComponentModel.ListSortDirection.Ascending)
        End If

        'DataGridView.Rows.Add()

    End Function

    Private Function SaveAcquire(ByVal Number As Integer) As Boolean

        Dim iTemp As Integer

        If PrevSelClass = -1 Then Exit Function


        ClassAmount(Number) = DataGridView.Rows.Count - 2
        For iTemp = 0 To DataGridView.Rows.Count - 2
            Try
                Skillss(Number, iTemp).skill_name = DataGridView.Item(0, iTemp).Value.ToString
            Catch ex As Exception
                Skillss(Number, iTemp).skill_name = skill_name.Items(0).ToString
            End Try
            Try
                Skillss(Number, iTemp).get_lv = CInt(DataGridView.Item(1, iTemp).Value)
            Catch ex As Exception
                Skillss(Number, iTemp).get_lv = 0
            End Try
            Try
                Skillss(Number, iTemp).lv_up_sp = CInt(DataGridView.Item(2, iTemp).Value)
            Catch ex As Exception
                Skillss(Number, iTemp).lv_up_sp = 0
            End Try
            Try
                Skillss(Number, iTemp).auto_get = CBool(DataGridView.Item(3, iTemp).Value)
            Catch ex As Exception
                Skillss(Number, iTemp).auto_get = False
            End Try
            Try
                Skillss(Number, iTemp).item_needed1 = DataGridView.Item(4, iTemp).Value.ToString
            Catch ex As Exception
                Skillss(Number, iTemp).item_needed1 = Nothing
            End Try
            Try
                Skillss(Number, iTemp).item_needed1_value = CInt(DataGridView.Item(5, iTemp).Value)
            Catch ex As Exception
                Skillss(Number, iTemp).item_needed1_value = Nothing
            End Try

            Try
                Skillss(Number, iTemp).item_needed2 = DataGridView.Item(6, iTemp).Value.ToString
            Catch ex As Exception
                Skillss(Number, iTemp).item_needed2 = Nothing
            End Try
            Try
                Skillss(Number, iTemp).item_needed2_value = CInt(DataGridView.Item(7, iTemp).Value)
            Catch ex As Exception
                Skillss(Number, iTemp).item_needed2_value = Nothing
            End Try

            Try
                Skillss(Number, iTemp).note = DataGridView.Item(8, iTemp).Value.ToString
            Catch ex As Exception
                Skillss(Number, iTemp).note = ""
            End Try
        Next


    End Function

    Private Sub ClassListBox_SelectedValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles ClassListBox.SelectedValueChanged

        If PrevSelClass <> -1 And (PrevSelClass <> ClassListBox.SelectedIndex) Then
            SaveAcquire(PrevSelClass)
            PrevSelClass = ClassListBox.SelectedIndex
            LoadAcquire(PrevSelClass)
            InheritsComboBox.Text = ClassInherits(PrevSelClass)
        End If

    End Sub

    Private Sub SkillAquireEditor_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        PrevSelClass = -1

        Me.Show()

        Dim sTemp As String

        sTemp = "skill_pch.txt"
        If System.IO.File.Exists("skill_pch.txt") = False Then
            OpenFileDialog.Title = "Select skill_pch file..."
            OpenFileDialog.Filter = "Lineage II Server skill definition config|skill_pch.txt|All files|*.*"
            If OpenFileDialog.ShowDialog = Windows.Forms.DialogResult.Cancel Then Me.Dispose() : Exit Sub
            sTemp = OpenFileDialog.FileName
        End If
        SkillPchLoad(sTemp)

        sTemp = "item_pch.txt"
        If System.IO.File.Exists(sTemp) = False Then
            OpenFileDialog.Title = "Select Item_pch file..."
            OpenFileDialog.Filter = "Lineage II Server item definition config|item_pch.txt|All files|*.*"
            If OpenFileDialog.ShowDialog = Windows.Forms.DialogResult.Cancel Then Me.Dispose() : Exit Sub
            sTemp = OpenFileDialog.FileName
        End If
        ItemPchLoad(sTemp)

        ClassListBox.SelectedIndex = 0
        PrevSelClass = 0

        DataGridView.Rows.Clear()
        DataGridView.Rows.Add()
        ClassAmount(0) = 0
        UpdateCell = False

    End Sub

    Private Sub LoadSkillAcquireButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LoadSkillAcquireButton.Click

        'Dim ClassAmount(100) As Integer
        'Dim ClassInherits(100) As String
        'Dim Skillss(100, 100) As SkillAcquire ' (1 - number of class's, 2 - amount skills in class)

        Dim iMarkerInClass As Integer = -1
        Dim sTemp As String, sTemp2 As String
        Dim iTemp As Integer

        Array.Clear(ClassAmount, 0, ClassAmount.Length)
        Array.Clear(ClassInherits, 0, ClassInherits.Length)
        Array.Clear(Skillss, 0, Skillss.Length)

        OpenFileDialog.Filter = "Lineage II server skills learn script (skillacquire.txt)|skillacquire.txt|All files|*.*"
        OpenFileDialog.FileName = "skillacquire.txt"
        If OpenFileDialog.ShowDialog = Windows.Forms.DialogResult.Cancel Then Exit Sub
        InheritsComboBox.Text = ""
        ClassListBox.Items.Clear()
        InheritsComboBox.Items.Clear()

        Dim inFile As New System.IO.StreamReader(OpenFileDialog.FileName, System.Text.Encoding.Default, True, 1)

        'fighter_begin
        'skill_begin	/* [럭키] */	skill_name = [s_lucky]	get_lv = 1	lv_up_sp = 0	auto_get = true	item_needed = {}	skill_end
        'skill_begin	/* [엑스퍼티즈 D] */	skill_name = [s_expertise_d]	get_lv = 20	lv_up_sp = 0	auto_get = true	item_needed = {}	skill_end
        'fighter_end
        'include_fighter	

        Dim aTemp() As String


        While inFile.EndOfStream <> True

            sTemp = inFile.ReadLine
            If sTemp.Trim <> "" Or sTemp.StartsWith("//") = False Then

                If sTemp.Trim.EndsWith("_begin") = True Then
                    iMarkerInClass += 1
                    ClassAmount(iMarkerInClass) = -1
                    ClassListBox.Items.Add(sTemp.Trim.Replace("_begin", ""))
                    InheritsComboBox.Items.Add(sTemp.Trim.Replace("_begin", ""))
                End If
                If sTemp.Trim.StartsWith("include_") = True Then
                    ClassInherits(iMarkerInClass) = sTemp.Trim.Replace("include_", "")
                End If
                If sTemp.Trim.EndsWith("_end") = True And InStr(sTemp, " ") = 0 Then
                    'End of class definition. Nothing happening
                End If
                If sTemp.Trim.StartsWith("skill_begin") = True Then
                    'Loading skills

                    ClassAmount(iMarkerInClass) += 1
                    iTemp = ClassAmount(iMarkerInClass)
                    Skillss(iMarkerInClass, iTemp).skill_name = Libraries.GetNeedParamFromStr(sTemp, "skill_name")
                    Skillss(iMarkerInClass, iTemp).get_lv = CInt(Libraries.GetNeedParamFromStr(sTemp, "get_lv"))
                    Skillss(iMarkerInClass, iTemp).lv_up_sp = CInt(Libraries.GetNeedParamFromStr(sTemp, "lv_up_sp"))
                    Skillss(iMarkerInClass, iTemp).auto_get = CBool(Libraries.GetNeedParamFromStr(sTemp, "auto_get"))

                    sTemp2 = Libraries.GetNeedParamFromStr(sTemp, "item_needed")
                    If sTemp2 = "{}" Then
                        Skillss(iMarkerInClass, iTemp).item_needed1 = Nothing
                        Skillss(iMarkerInClass, iTemp).item_needed2 = Nothing
                    Else
                        'item_needed = {{[sb_drain_energy1];1}}
                        'item_needed = {{[sb_drain_energy1];1};{[adena];2}}
                        sTemp2 = sTemp2.Replace("};{", "|").Replace("{", "").Replace("}", "")
                        aTemp = sTemp2.Split(CChar("|"))

                        '[sb_drain_energy1];1
                        Skillss(iMarkerInClass, iTemp).item_needed1 = aTemp(0).Substring(0, InStr(aTemp(0), "]"))
                        Skillss(iMarkerInClass, iTemp).item_needed1_value = CInt(aTemp(0).Replace(Skillss(iMarkerInClass, iTemp).item_needed1 & ";", ""))


                        'Skillss(iMarkerInClass, iTemp).item_needed1 = Mid(aTemp(0), InStr(aTemp(0), "["), InStr(aTemp(0), "]") - InStr(aTemp(0), "[") + 1)
                        'Skillss(iMarkerInClass, iTemp).item_needed1_value = CInt(Mid(aTemp(0), InStr(aTemp(0), ";") + 1, InStr(aTemp(0), "}}") - InStr(aTemp(0), ";") - 1))

                        If aTemp.Length > 1 Then
                            'Skillss(iMarkerInClass, iTemp).item_needed2 = Mid(aTemp(1), InStr(sTemp2, "["), InStr(aTemp(1), "]") - InStr(aTemp(1), "[") + 1)
                            'Skillss(iMarkerInClass, iTemp).item_needed2_value = CInt(Mid(aTemp(1), InStr(aTemp(1), ";") + 1, InStr(aTemp(1), "}}") - InStr(aTemp(1), ";") - 1))
                            Skillss(iMarkerInClass, iTemp).item_needed2 = aTemp(1).Substring(0, InStr(aTemp(1), "]"))
                            Skillss(iMarkerInClass, iTemp).item_needed2_value = CInt(aTemp(1).Replace(Skillss(iMarkerInClass, iTemp).item_needed2 & ";", ""))
                        End If
                    End If
                    Skillss(iMarkerInClass, iTemp).note = Mid(sTemp, InStr(sTemp, "/*") + 2, InStr(sTemp, "*/") - InStr(sTemp, "/*") - 2).Replace("[", "").Replace("]", "").Trim
                End If

            End If
        End While
        inFile.Close()

        PrevSelClass = 0
        LoadAcquire(0)
        ClassListBox.SelectedIndex = 0


    End Sub

    Private Sub SaveSkillAquireButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SaveSkillAcquireButton.Click


        'Dim sTemp As String
        Dim iTemp As Integer, iTemp2 As Integer

        SaveAcquire(PrevSelClass)

        SaveFileDialog.FileName = "skillacquire.txt"
        OpenFileDialog.Filter = "Lineage II Server skills learn script (skillacquire.txt)|*.txt|All files|*.*"
        SaveFileDialog.OverwritePrompt = True
        If SaveFileDialog.ShowDialog = Windows.Forms.DialogResult.Cancel Then Exit Sub
        If System.IO.File.Exists(SaveFileDialog.FileName) = True Then System.IO.File.Copy(SaveFileDialog.FileName, SaveFileDialog.FileName & ".bak", True)
        Dim outFile As New System.IO.StreamWriter(SaveFileDialog.FileName, False, System.Text.Encoding.Unicode, 1)

        outFile.WriteLine("//SkillAquire generated by L2ScriptMaker at " & Now)

        For iTemp = 0 To ClassListBox.Items.Count - 1

            outFile.WriteLine(ClassListBox.Items.Item(iTemp).ToString & "_begin")

            If ClassInherits(iTemp) <> Nothing Then
                outFile.WriteLine("include_" & ClassInherits(iTemp).ToString)
            End If

            For iTemp2 = 0 To ClassAmount(iTemp)

                'skill_begin	/* [모탈 블로우] */	skill_name = [s_mortal_blow33]	get_lv = 15	lv_up_sp = 1300	auto_get = false	item_needed = {}	skill_end

                outFile.Write("skill_begin" & vbTab)
                outFile.Write("/* [" & Skillss(iTemp, iTemp2).note & "] */" & vbTab)
                outFile.Write("skill_name=" & Skillss(iTemp, iTemp2).skill_name & vbTab)
                outFile.Write("get_lv=" & Skillss(iTemp, iTemp2).get_lv & vbTab)
                outFile.Write("lv_up_sp=" & Skillss(iTemp, iTemp2).lv_up_sp & vbTab)
                outFile.Write("auto_get=" & Skillss(iTemp, iTemp2).auto_get.ToString.ToLower & vbTab)

                If Skillss(iTemp, iTemp2).item_needed1 = Nothing Then
                    outFile.Write("item_needed={}" & vbTab)
                Else
                    'item_needed = {{[sb_drain_energy1];1}}
                    outFile.Write("item_needed={")
                    outFile.Write("{" & Skillss(iTemp, iTemp2).item_needed1 & ";" & Skillss(iTemp, iTemp2).item_needed1_value & "}")
                    If Skillss(iTemp, iTemp2).item_needed2 IsNot Nothing Or Skillss(iTemp, iTemp2).item_needed2 <> "" Then
                        outFile.Write(";{" & Skillss(iTemp, iTemp2).item_needed2 & ";" & Skillss(iTemp, iTemp2).item_needed2_value & "}")
                    End If
                    outFile.Write("}" & vbTab)

                End If
                outFile.WriteLine("skill_end")
            Next

            outFile.WriteLine(ClassListBox.Items.Item(iTemp).ToString & "_end")

        Next

        outFile.Close()

        MessageBox.Show("New " & SaveFileDialog.FileName & " saved success.", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information)


    End Sub

    Private Sub InheritsComboBox_Validated(ByVal sender As Object, ByVal e As System.EventArgs) Handles InheritsComboBox.Validated
        ClassInherits(PrevSelClass) = InheritsComboBox.Text
    End Sub

    Private Function ItemPchLoad(ByVal FileName As String) As Boolean

        Dim inFile As System.IO.StreamReader
        Try
            inFile = New System.IO.StreamReader(FileName, System.Text.Encoding.Default, True, 1)
        Catch ex As Exception
            Return False
        End Try
        Dim sTempStr As String = ""
        Dim aTemp(1) As String
        Dim iTemp As Integer = -1

        ProgressBar.Maximum = CInt(inFile.BaseStream.Length)
        ProgressBar.Value = 0
        While inFile.EndOfStream <> True

            '[small_sword]	=	1
            Try
                sTempStr = inFile.ReadLine
                sTempStr = sTempStr.Replace(" ", "").Replace(Chr(9), "") '.Replace("[", "").Replace("]", "")
                aTemp = sTempStr.Split(CChar("="))

                'ItemPch(CInt(aTemp(1))) = aTemp(0)
                iTemp += 1
                Array.Resize(ItemPch, iTemp + 1)
                ItemPch(iTemp) = aTemp(0)
                'item_needed.Items.Add(aTemp(0))
            Catch ex As Exception
                MessageBox.Show("Import data invalid in line" & vbNewLine & sTempStr, "Import data invalid", MessageBoxButtons.OK, MessageBoxIcon.Error)
                inFile.Close()
                Return False
            End Try

            ProgressBar.Value = CInt(inFile.BaseStream.Position) : ProgressBar.Update()
            'Me.Update()

        End While

        'item_needed.Items.Contains(ItemPch)
        item_needed1.Items.AddRange(ItemPch)
        item_needed2.Items.AddRange(ItemPch)

        ProgressBar.Value = 0
        inFile.Close()
        Return True

    End Function

    Private Function SkillPchLoad(ByVal FileName As String) As Boolean

        Dim inFile As System.IO.StreamReader
        Try
            inFile = New System.IO.StreamReader(FileName, System.Text.Encoding.Default, True, 1)
        Catch ex As Exception
            Return False
        End Try
        Dim sTempStr As String = ""
        Dim aTemp(1) As String

        Dim iTemp As Integer = -1

        ProgressBar.Maximum = CInt(inFile.BaseStream.Length)
        ProgressBar.Value = 0

        While inFile.EndOfStream <> True

            '[s_power_strike11] = 769
            Try
                sTempStr = inFile.ReadLine
                sTempStr = sTempStr.Replace(" ", "").Replace(Chr(9), "") '.Replace("[", "").Replace("]", "")
                aTemp = sTempStr.Split(CChar("="))

                'SkillPch(CInt(aTemp(1))) = aTemp(0)
                iTemp += 1
                Array.Resize(SkillPch, iTemp + 1)
                SkillPch(iTemp) = aTemp(0)
                'skill_name.Items.Add(aTemp(0))
            Catch ex As Exception
                MessageBox.Show("Import data invalid in line" & vbNewLine & sTempStr, "Import data invalid", MessageBoxButtons.OK, MessageBoxIcon.Error)
                inFile.Close()
                Return False
            End Try

            ProgressBar.Value = CInt(inFile.BaseStream.Position) : ProgressBar.Update()
            'Me.Update()

        End While

        'skill_name.Items.Contains(SkillPch)
        skill_name.Items.AddRange(SkillPch)

        ProgressBar.Value = 0
        inFile.Close()
        Return True

    End Function

    Private Sub QuitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles QuitButton.Click
        Me.Dispose()
    End Sub

    Private Sub CopyPasteSkillButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CopyPasteSkillButton.Click

        ' Check before pasting
        Select Case CopyPasteComboBox.Text
            Case "skill_name"
                If Array.IndexOf(SkillPch, CopyPasteTextBox.Text) = -1 Then
                    MessageBox.Show("Skill not found. Correct name and try again.", "Skill not found", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If
            Case "item_needed1"
                If Array.IndexOf(ItemPch, CopyPasteTextBox.Text) = -1 And CopyPasteTextBox.Text <> "" Then
                    MessageBox.Show("Item not found. Correct name and try again.", "Item not found", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If
                If DataGridView.Item(5, DataGridView.CurrentRow.Index).ToString = "0" Then 'DataGridView.Columns("item_needed_value").Name =5
                    DataGridView.Item(5, DataGridView.CurrentRow.Index).Value = "1"
                End If
            Case "item_needed2"
                If Array.IndexOf(ItemPch, CopyPasteTextBox.Text) = -1 And CopyPasteTextBox.Text <> "" Then
                    MessageBox.Show("Item not found. Correct name and try again.", "Item not found", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Exit Sub
                End If
                If DataGridView.Item(7, DataGridView.CurrentRow.Index).ToString = "0" Then 'DataGridView.Columns("item_needed_value").Name =5
                    DataGridView.Item(7, DataGridView.CurrentRow.Index).Value = "1"
                End If
            Case "note"
            Case Else
                CopyPasteComboBox.Text = "skill_name"
                Exit Sub
        End Select
        ' CopyPaste skill to cell
        DataGridView.Item(DataGridView.Columns(CopyPasteComboBox.Text).Name, DataGridView.CurrentRow.Index).Value = CopyPasteTextBox.Text

    End Sub

    Private Sub MoveUpButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MoveUpButton.Click

        ' UP Line

        Dim aTemp(7) As Object
        Dim iTemp As Integer

        'Load upper line to temp array
        If DataGridView.CurrentRow.Index = 0 Then Exit Sub

        UpdateCell = True
        For iTemp = 0 To 7
            aTemp(iTemp) = DataGridView.Item(iTemp, DataGridView.CurrentRow.Index - 1).Value
            DataGridView.Item(iTemp, DataGridView.CurrentRow.Index - 1).Value = DataGridView.Item(iTemp, DataGridView.CurrentRow.Index).Value
            DataGridView.Item(iTemp, DataGridView.CurrentRow.Index).Value = aTemp(iTemp)
        Next
        UpdateCell = False

        'DataGridView.Rows(DataGridView.CurrentRow.Index).Selected = False
        'DataGridView.Rows(DataGridView.CurrentRow.Index - 1).Selected = True


    End Sub

    Private Sub MoveDownButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MoveDownButton.Click
        ' Down Line

        Dim aTemp(7) As Object
        Dim iTemp As Integer

        'Load upper line to temp array
        If DataGridView.CurrentRow.Index >= DataGridView.RowCount - 2 Then Exit Sub

        UpdateCell = True
        For iTemp = 0 To 7
            aTemp(iTemp) = DataGridView.Item(iTemp, DataGridView.CurrentRow.Index + 1).Value
            DataGridView.Item(iTemp, DataGridView.CurrentRow.Index + 1).Value = DataGridView.Item(iTemp, DataGridView.CurrentRow.Index).Value
            DataGridView.Item(iTemp, DataGridView.CurrentRow.Index).Value = aTemp(iTemp)
        Next
        UpdateCell = False

        'DataGridView.Rows(DataGridView.CurrentRow.Index).Selected = False
        'DataGridView.Rows(DataGridView.CurrentRow.Index + 1).Selected = True

    End Sub

    Private Sub CopyUpButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CopyUpButton.Click

        ' Copy UP Line

        Dim iTemp As Integer

        'Load upper line to temp array
        If DataGridView.CurrentRow.Index = 0 Then Exit Sub

        UpdateCell = True
        For iTemp = 0 To 7
            DataGridView.Item(iTemp, DataGridView.CurrentRow.Index - 1).Value = DataGridView.Item(iTemp, DataGridView.CurrentRow.Index).Value
        Next
        UpdateCell = False

    End Sub

    Private Sub CopyDownButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CopyDownButton.Click
        ' Copy Down Line

        Dim iTemp As Integer

        'Load upper line to temp array
        If DataGridView.CurrentRow.Index < DataGridView.RowCount - 2 Then
            'nothing
        ElseIf DataGridView.CurrentRow.Index = DataGridView.RowCount - 2 Then
            DataGridView.Rows.Add()
        Else
            Exit Sub
        End If

        UpdateCell = True
        For iTemp = 0 To 7
            DataGridView.Item(iTemp, DataGridView.CurrentRow.Index + 1).Value = DataGridView.Item(iTemp, DataGridView.CurrentRow.Index).Value
        Next
        UpdateCell = False

    End Sub

    Private Sub CloneUpButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CloneUpButton.Click

        ' Copy UP Line
        'Load upper line to temp array
        UpdateCell = True
        DataGridView.Rows.Insert(DataGridView.CurrentRow.Index)
        UpdateCell = True
        Dim iTemp As Integer
        For iTemp = 0 To 7
            DataGridView.Item(iTemp, DataGridView.CurrentRow.Index - 1).Value = DataGridView.Item(iTemp, DataGridView.CurrentRow.Index).Value
        Next
        UpdateCell = False

    End Sub

    Private Sub CloneDownButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CloneDownButton.Click
        If DataGridView.CurrentRow.Index > DataGridView.RowCount - 2 Then Exit Sub
        UpdateCell = True
        DataGridView.Rows.Insert(DataGridView.CurrentRow.Index + 1)
        Dim iTemp As Integer
        For iTemp = 0 To 7
            DataGridView.Item(iTemp, DataGridView.CurrentRow.Index + 1).Value = DataGridView.Item(iTemp, DataGridView.CurrentRow.Index).Value
        Next
        UpdateCell = False
    End Sub

    Private Sub AddRowBeforeButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddRowBeforeButton.Click
        DataGridView.Rows.Insert(DataGridView.CurrentRow.Index)
        'DataGridView.Item(0, DataGridView.CurrentRow.Index - 1).Value = skill_name.Items(0).ToString
    End Sub

    Private Sub AddDownButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AddDownButton.Click
        If DataGridView.CurrentRow.Index >= DataGridView.RowCount - 2 Then Exit Sub
        DataGridView.Rows.Insert(DataGridView.CurrentRow.Index + 1)
    End Sub

    Private Sub DataGridView_CellValueChanged(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles DataGridView.CellValueChanged
        'If Me.Visible = False Then Exit Sub
        If UpdateCell = True Then Exit Sub
        If e.ColumnIndex = 4 Then
            If DataGridView.Item(e.ColumnIndex, e.RowIndex).Value IsNot Nothing And DataGridView.Item(e.ColumnIndex + 1, e.RowIndex).Value Is Nothing Then
                DataGridView.Item(e.ColumnIndex + 1, e.RowIndex).Value = "1"
            End If
        End If
        If e.ColumnIndex = 6 Then
            If DataGridView.Item(e.ColumnIndex, e.RowIndex).Value IsNot Nothing And DataGridView.Item(e.ColumnIndex + 1, e.RowIndex).Value Is Nothing Then
                DataGridView.Item(e.ColumnIndex + 1, e.RowIndex).Value = "1"
            End If
        End If
    End Sub

    Private Sub AutoSortCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles AutoSortCheckBox.CheckedChanged
        If AutoSortCheckBox.Checked = True Then
            AutoSortComboBox.Enabled = True
        Else
            AutoSortComboBox.Enabled = False
        End If
    End Sub

End Class