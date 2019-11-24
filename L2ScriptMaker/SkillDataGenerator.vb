Public Class SkillDataGenerator

    Private Structure SkillBone
        Dim type As Short
        Dim name As String
        Dim desc As String  'name - name of skill from skillname-e.dat
        Dim is_magic As Integer
        Dim mp_consume1 As Integer
        Dim mp_consume2 As Integer
        Dim hp_consume As Integer
        Dim cast_range As Integer
        Dim hit_time As Double
        Dim debuff As Short
        Dim is_ench As Short
        Dim ench_skill_id As Integer
    End Structure

    Dim aSkillName(1) As String

    Dim aSkill(10000, 256) As SkillBone

    Private Sub StartButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StartButton.Click

        'Dim EnchantChance() As Integer = {0, 0, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 2, 2, 6, 10, 14, 20, 30, 40, 78, 80, 82, 88, 90, 92, 93, 95, 97}
        Dim EnchantChance() As Integer = {97, 95, 93, 92, 90, 88, 82, 80, 78, 40, 30, 20, 14, 10, 6, 2, 2, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 0, 0, 0, 0, 0, 0, 0}

        Dim sSkillGrpFile As String = "skillgrp.txt"       ' Name/path of skillgrp.txt file
        Dim sSkillnameEFile As String = "skillname-e.txt"      ' Name/path of skillname-e.txt file
        Dim sSkillDataFile As String = "skilldata.txt"      ' Name/path of skilldata.txt file
        Dim sSkillEncFile As String = "skillenchantdata.txt"      ' Name/path of skilldata.txt file
        Dim sSkillPchFile As String = "skill_pch.txt"

        'ExistPchCheckBox

        ' skillgrp_begin	skill_id=1398	skill_level=101	oper_type=0	mp_consume=103	cast_range=900	cast_style=1	hit_time=4.000000	is_magic=1	ani_char=[f]	desc=[skill.su.1069]	icon_name=[icon.skill1398]	extra_eff=0	is_ench=1	ench_skill_id=10	hp_consume=0	UNK_0=9	UNK_1=11	skillgrp_end

        Dim sTemp As String, iTemp As Integer

        If System.IO.File.Exists(sSkillGrpFile) = False Then
            OpenFileDialog.FileName = ""
            OpenFileDialog.Filter = "Lineage II client text file (skillgrp.txt)|skillgrp.txt|All files|*.*"
            If OpenFileDialog.ShowDialog = Windows.Forms.DialogResult.Cancel Then Exit Sub
            sSkillGrpFile = OpenFileDialog.FileName
        End If

        If System.IO.File.Exists(sSkillnameEFile) = False Then
            OpenFileDialog.FileName = ""
            OpenFileDialog.Filter = "Lineage II client text file (skillname-e.txt)|skillname-e.txt|All files|*.*"
            If OpenFileDialog.ShowDialog = Windows.Forms.DialogResult.Cancel Then Exit Sub
            sSkillnameEFile = OpenFileDialog.FileName
        End If

        If ExistPchCheckBox.Checked = True Then
            If System.IO.File.Exists(sSkillPchFile) = False Then
                OpenFileDialog.FileName = ""
                OpenFileDialog.Filter = "Lineage II client text file (skill_pch.txt)|skill_pch.txt|All files|*.*"
                If OpenFileDialog.ShowDialog = Windows.Forms.DialogResult.Cancel Then Exit Sub
                sSkillPchFile = OpenFileDialog.FileName
            End If
            If SkillPchLoad(sSkillPchFile) = False Then Exit Sub
        End If

        SaveFileDialog.FileName = sSkillDataFile
        SaveFileDialog.Filter = "Lineage II server enchant skill script (skilldata.txt)|skilldata*.txt|All files|*.*"
        If SaveFileDialog.ShowDialog = Windows.Forms.DialogResult.Cancel Then Exit Sub
        sSkillDataFile = SaveFileDialog.FileName

        SaveFileDialog.FileName = sSkillEncFile
        SaveFileDialog.Filter = "Lineage II server skill script (skillenchantdata.txt)|skillenchantdata*.txt|All files|*.*"
        If SaveFileDialog.ShowDialog = Windows.Forms.DialogResult.Cancel Then Exit Sub
        sSkillEncFile = SaveFileDialog.FileName

        Dim inFile As System.IO.StreamReader
        Dim iId As Integer, iLevel As Integer

        ' ---- Loading 'Skillname-e.txt' ---- 
        inFile = New System.IO.StreamReader(sSkillnameEFile, True)
        ToolStripProgressBar.Maximum = CInt(inFile.BaseStream.Length)
        ToolStripStatusLabel.Text = "Loading skilldata-e.txt..."

        While inFile.EndOfStream <> True

            sTemp = inFile.ReadLine
            If sTemp <> "" And sTemp.StartsWith("//") = False Then

                'skillname_begin	id=3	level=1	name=[Power Strike]	description=[Gathers power for a fierce strike. Used when equipped with a sword or blunt weapon. Over-hit is possible.  Power 25.]	desc_add1=[none]	desc_add2=[none]	skillname_end
                iId = CInt(Libraries.GetNeedParamFromStr(sTemp, "id"))
                iLevel = CInt(Libraries.GetNeedParamFromStr(sTemp, "level"))
                aSkill(iId, iLevel).desc = Libraries.GetNeedParamFromStr(sTemp, "name")
            End If
            ToolStripProgressBar.Value = CInt(inFile.BaseStream.Position)
            StatusStrip.Update()
        End While
        ToolStripProgressBar.Value = 0
        inFile.Close()
        Me.Refresh()

        ' ---- Loading 'Skillgrp.txt' ---- 
        inFile = New System.IO.StreamReader(sSkillGrpFile, True)
        ToolStripProgressBar.Maximum = CInt(inFile.BaseStream.Length)
        ToolStripStatusLabel.Text = "Loading Skillgrp.txt"

        While inFile.EndOfStream <> True

            sTemp = inFile.ReadLine
            If sTemp <> "" And sTemp.StartsWith("//") = False Then

                iId = CInt(Libraries.GetNeedParamFromStr(sTemp, "skill_id"))
                iLevel = CInt(Libraries.GetNeedParamFromStr(sTemp, "skill_level"))
                aSkill(iId, iLevel).type = CShort(Libraries.GetNeedParamFromStr(sTemp, "oper_type"))
                aSkill(iId, iLevel).is_magic = CInt(Libraries.GetNeedParamFromStr(sTemp, "is_magic"))
                aSkill(iId, iLevel).mp_consume2 = CInt(Libraries.GetNeedParamFromStr(sTemp, "mp_consume"))
                aSkill(iId, iLevel).hp_consume = CInt(Libraries.GetNeedParamFromStr(sTemp, "hp_consume"))
                aSkill(iId, iLevel).cast_range = CInt(Libraries.GetNeedParamFromStr(sTemp, "cast_range"))
                aSkill(iId, iLevel).hit_time = CDbl(Libraries.GetNeedParamFromStr(sTemp, "hit_time"))
                aSkill(iId, iLevel).debuff = CShort(Libraries.GetNeedParamFromStr(sTemp, "extra_eff"))
                aSkill(iId, iLevel).is_ench = CShort(Libraries.GetNeedParamFromStr(sTemp, "is_ench"))
                aSkill(iId, iLevel).ench_skill_id = CInt(Libraries.GetNeedParamFromStr(sTemp, "ench_skill_id"))
            End If
            ToolStripProgressBar.Value = CInt(inFile.BaseStream.Position)
            StatusStrip.Update()

        End While
        ToolStripProgressBar.Value = 0
        inFile.Close()
        Me.Refresh()

        Dim sTemp2 As String

        If System.IO.File.Exists(sSkillDataFile) = True Then System.IO.File.Copy(sSkillDataFile, sSkillDataFile & ".bak", True)
        Dim outFile As New System.IO.StreamWriter(sSkillDataFile, False, System.Text.Encoding.Unicode, 1)
        Dim outEncFile As New System.IO.StreamWriter(sSkillEncFile, False, System.Text.Encoding.Unicode, 1)
        'skill_begin	skill_name=[s_long_range_shot1]	/* [롱 샷] */	skill_id=113	level=1	operate_type=P	magic_level=20	effect={{p_attack_range;{bow};200;diff}}	skill_end

        'skillgrp_begin	skill_id=3	skill_level=1	oper_type=0	mp_consume=10	cast_range=40	hit_time=1.080000	is_magic=0	desc=[]	hp_consume=0	skillgrp_end
        'skillenchantdata.txt
        'enchant_skill_begin	original_skill = [s_hate192]	route_id = 1	enchant_id = 1	skill_level = 101	exp = 3060000	sp = 306000	item_needed = {{[codex_of_giant];1}}	prob_76 = 82	prob_77 = 92	prob_78 = 97	enchant_skill_end

        ToolStripProgressBar.Maximum = 10000
        ToolStripStatusLabel.Text = "Saving new skilldata.txt..."
        Dim iRouteId As Short = 2
        Dim iEnchId As Integer = 0
        Dim sEnchName As String = ""

        For iId = 0 To 10000
            For iLevel = 0 To 255

                If aSkill(iId, iLevel).desc <> Nothing Then
                    sTemp = ""

                    If Shema1RadioButton.Checked = True Then
                        'shema1
                        'NPC Spinning Slasher
                        sTemp2 = aSkill(iId, iLevel).desc.Trim.ToLower.Replace(" - ", "_").Replace("'", "").Replace(":", "").Replace(".", "").Replace(",", "")
                        sTemp2 = sTemp2.Replace("%", "").Replace("!", "").Replace("&", "").Replace("-", "_")
                        sTemp2 = sTemp2.Replace(" ", "_").Replace(" ", "_").Replace("[", "").Replace("]", "").Replace("_of_", "_").Replace("_the_", "_")
                        sTemp2 = sTemp2.Replace("(", "").Replace(")", "_").Replace("/", "_")
                        sTemp2 = "s_" & sTemp2 '& iLevel

                        'Fix for first level of skill with more them 1 level
                        If iLevel = 1 Then

                            's_npc_selfdamage_shield
                            's_npc_self_damage_shield

                            If aSkill(iId, iLevel + 1).desc = "" Or aSkill(iId, iLevel + 1).desc Is Nothing Then
                                ' this skill first and alone in this skill_id
                                If Array.IndexOf(aSkillName, sTemp2) = -1 Then
                                    sTemp2 = sTemp2
                                Else
                                    sTemp2 = sTemp2 & "_" & iId                 ' s_dragon3
                                End If
                                Array.Resize(aSkillName, aSkillName.Length + 1)
                                aSkillName(aSkillName.Length - 1) = sTemp2
                            Else

                                If Array.IndexOf(aSkillName, sTemp2 & iLevel) = -1 Then
                                    sTemp2 = sTemp2 & iLevel
                                Else
                                    sTemp2 = sTemp2 & "_" & iId & "_" & iLevel        ' s_triple_slash3433_1
                                End If
                                Array.Resize(aSkillName, aSkillName.Length + 1)
                                aSkillName(aSkillName.Length - 1) = sTemp2

                            End If

                        Else
                            ' Dublicate fix
                            If Array.IndexOf(aSkillName, sTemp2 & iLevel) = -1 Then
                                sTemp2 = sTemp2 & iLevel
                            Else
                                'sTemp2 = "s_none_" & iId & "_" & iLevel    ' s_none_3433_1
                                sTemp2 = sTemp2 & "_" & iId & "_" & iLevel        ' s_triple_slash3433_1
                            End If
                            Array.Resize(aSkillName, aSkillName.Length + 1)
                            aSkillName(aSkillName.Length - 1) = sTemp2
                        End If
                    Else
                        sTemp2 = "s_" & iId & "_" & iLevel
                    End If

                    ' --- Exist skill_pch base ---
                    If ExistPchCheckBox.Checked = True Then
                        If aSkill(iId, iLevel).name Is Nothing Then
                            aSkill(iId, iLevel).name = "[" & sTemp2 & "]"
                        Else
                            Dim ajshj As Integer = 0
                        End If
                    Else
                        aSkill(iId, iLevel).name = "[" & sTemp2 & "]"
                    End If

                    If AllPassiveCheckBox.Checked = True Then
                        ' Generate all like (P)assive skill
                        aSkill(iId, iLevel).type = 2
                    End If

                    ' --- Common Header for all types
                    sTemp = sTemp & "skill_begin" & vbTab
                    sTemp = sTemp & "skill_name=" & aSkill(iId, iLevel).name & vbTab
                    If DontDescCheckBox.Checked = False Then sTemp = sTemp & "/* " & aSkill(iId, iLevel).desc & " */" & vbTab
                    sTemp = sTemp & "skill_id=" & iId & vbTab
                    sTemp = sTemp & "level=" & iLevel & vbTab
                    Select Case aSkill(iId, iLevel).type
                        Case 0
                            sTemp = sTemp & "operate_type=A1" & vbTab
                        Case 1
                            sTemp = sTemp & "operate_type=A2" & vbTab
                        Case 2
                            sTemp = sTemp & "operate_type=P" & vbTab
                        Case 3
                            sTemp = sTemp & "operate_type=T" & vbTab
                    End Select
                    sTemp = sTemp & "magic_level=1" & vbTab
                    sTemp = sTemp & "effect={}" & vbTab
                    ' --- End of Common Header of skill


                    'sTemp = sTemp & "=" & aSkill(iId, iLevel).desc & vbTab
                    Select Case aSkill(iId, iLevel).type
                        Case 0  'A1
                            'is_magic = 1	
                            sTemp = sTemp & "is_magic=" & aSkill(iId, iLevel).is_magic & vbTab

                            If aSkill(iId, iLevel).is_magic = 0 Then
                                'hp_consume = 7	
                                'mp_consume1 = 7	
                                iTemp = 0
                            Else
                                iTemp = CInt(aSkill(iId, iLevel).mp_consume2 / 5)
                                sTemp = sTemp & "mp_consume1=" & iTemp & vbTab
                            End If
                            'mp_consume2 = 28	
                            sTemp = sTemp & "mp_consume2=" & (aSkill(iId, iLevel).mp_consume2 - iTemp) & vbTab

                            If aSkill(iId, iLevel).hp_consume <> Nothing Then
                                'hp_consume = 7	
                                sTemp = sTemp & "hp_consume=" & aSkill(iId, iLevel).hp_consume & vbTab
                            End If

                            'cast_range = -1	
                            sTemp = sTemp & "cast_range=" & aSkill(iId, iLevel).cast_range & vbTab
                            'effective_range = -1	
                            If aSkill(iId, iLevel).cast_range = -1 Then
                                sTemp = sTemp & "effective_range=-1" & vbTab
                            Else
                                sTemp = sTemp & "effective_range=" & CInt(aSkill(iId, iLevel).cast_range * 1.5) & vbTab
                            End If

                            'skill_hit_time = 4	
                            sTemp = sTemp & "skill_hit_time=" & aSkill(iId, iLevel).hit_time & vbTab
                            'skill_cool_time = 0	
                            sTemp = sTemp & "skill_cool_time=0" & vbTab
                            'skill_hit_cancel_time = 0.5	
                            sTemp = sTemp & "skill_hit_cancel_time=0" & vbTab
                            'reuse_delay = 6	
                            sTemp = sTemp & "reuse_delay=" & aSkill(iId, iLevel).hit_time & vbTab

                            'attribute = attr_none	
                            sTemp = sTemp & "attribute=attr_none" & vbTab
                            'effect_point = 379	
                            sTemp = sTemp & "effect_point=0" & vbTab

                            If aSkill(iId, iLevel).cast_range = -1 Then
                                'target_type = self
                                sTemp = sTemp & "target_type=self" & vbTab
                            Else
                                'target_type = self
                                sTemp = sTemp & "target_type=target" & vbTab
                            End If
                            'affect_scope = single	
                            sTemp = sTemp & "affect_scope=single" & vbTab

                            'affect_limit = {0;0}	
                            sTemp = sTemp & "affect_limit={0;0}" & vbTab
                            'next_action = none	
                            sTemp = sTemp & "next_action=none" & vbTab

                            'ride_state = {@ride_none;@ride_wind;@ride_star;@ride_twilight}	
                            If aSkill(iId, iLevel).is_magic = 0 Then
                                sTemp = sTemp & "ride_state={@ride_none}" & vbTab
                            Else
                                sTemp = sTemp & "ride_state={@ride_none;@ride_wind;@ride_star;@ride_twilight}" & vbTab
                            End If

                        Case 1  'A2,A3
                            'is_magic = 1	
                            sTemp = sTemp & "is_magic=" & aSkill(iId, iLevel).is_magic & vbTab
                            'mp_consume2 = 28	
                            sTemp = sTemp & "mp_consume2=" & aSkill(iId, iLevel).mp_consume2 & vbTab

                            'cast_range = -1	
                            sTemp = sTemp & "cast_range=" & aSkill(iId, iLevel).cast_range & vbTab
                            'effective_range = -1	
                            If aSkill(iId, iLevel).cast_range = -1 Then
                                sTemp = sTemp & "effective_range=-1" & vbTab
                            Else
                                sTemp = sTemp & "effective_range=" & CInt(aSkill(iId, iLevel).cast_range * 1.5) & vbTab
                            End If

                            'skill_hit_time = 4	
                            sTemp = sTemp & "skill_hit_time=" & aSkill(iId, iLevel).hit_time & vbTab
                            'skill_cool_time = 0	
                            sTemp = sTemp & "skill_cool_time=0" & vbTab
                            'skill_hit_cancel_time = 0.5	
                            sTemp = sTemp & "skill_hit_cancel_time=0" & vbTab
                            'reuse_delay = 80	
                            sTemp = sTemp & "reuse_delay=80" & vbTab

                            'activate_rate = -1
                            sTemp = sTemp & "activate_rate=80" & vbTab
                            'lv_bonus_rate = 0
                            sTemp = sTemp & "lv_bonus_rate=0" & vbTab
                            'basic_property = none
                            sTemp = sTemp & "basic_property=none" & vbTab
                            'abnormal_time = 15
                            sTemp = sTemp & "abnormal_time=15" & vbTab
                            'abnormal_lv = 1
                            sTemp = sTemp & "abnormal_lv=0" & vbTab
                            'abnormal_type = speed_up_special
                            sTemp = sTemp & "abnormal_type=none" & vbTab
                            'attribute = attr_none
                            sTemp = sTemp & "attribute=attr_none" & vbTab
                            'effect_point = 204
                            sTemp = sTemp & "effect_point=0" & vbTab

                            If aSkill(iId, iLevel).cast_range = -1 Then
                                'target_type = self
                                sTemp = sTemp & "target_type=self" & vbTab
                            Else
                                'target_type = self
                                sTemp = sTemp & "target_type=target" & vbTab
                            End If
                            'affect_scope = single
                            sTemp = sTemp & "affect_scope=single" & vbTab

                            'affect_limit = {0;0}
                            sTemp = sTemp & "affect_limit={0;0}" & vbTab
                            'next_action = none
                            sTemp = sTemp & "next_action=none" & vbTab
                            'debuff = 0
                            sTemp = sTemp & "debuff=" & aSkill(iId, iLevel).debuff & vbTab 'extra_eff=1

                            'ride_state = {@ride_none;@ride_wind;@ride_star;@ride_twilight}	
                            If aSkill(iId, iLevel).is_magic = 0 Then
                                sTemp = sTemp & "ride_state={@ride_none}" & vbTab
                            Else
                                sTemp = sTemp & "ride_state={@ride_none;@ride_wind;@ride_star;@ride_twilight}" & vbTab
                            End If

                        Case 2  'P
                            'sTemp = sTemp & "skill_begin" & vbTab
                            'sTemp = sTemp & "skill_name=[" & sTemp2 & "]" & vbTab
                            'sTemp = sTemp & "/* " & aSkill(iId, iLevel).desc & " */" & vbTab
                            'sTemp = sTemp & "skill_id=" & iId & vbTab
                            'sTemp = sTemp & "level=" & iLevel & vbTab
                            'sTemp = sTemp & "operate_type=" & "P" & vbTab   'operate_type
                            'sTemp = sTemp & "magic_level=1" & vbTab
                            'sTemp = sTemp & "effect={}" & vbTab
                            'sTemp = sTemp & "skill_end"

                        Case 3  'T
                            'mp_consume1 = 2	
                            sTemp = sTemp & "mp_consume1=" & aSkill(iId, iLevel).mp_consume2 & vbTab
                            'reuse_delay = 0
                            sTemp = sTemp & "reuse_delay=0" & vbTab
                            'target_type = none
                            sTemp = sTemp & "target_type=none" & vbTab
                            'next_action = none
                            sTemp = sTemp & "next_action=none" & vbTab
                            'ride_state = {@ride_none}
                            sTemp = sTemp & "ride_state={@ride_none}" & vbTab

                        Case Else
                            ' like (P)assive
                    End Select
                    sTemp = sTemp & "skill_end"
                    outFile.WriteLine(sTemp)
                    sTemp = ""
                    ' ------------ SkillEnchant Module --------------
                    If aSkill(iId, iLevel).is_ench = 1 Then

                        'enchant_skill_begin
                        sTemp = "enchant_skill_begin" & vbTab
                        'original_skill = [s_hate192]
                        ' FIIXXXX check

                        sTemp = sTemp & "original_skill=" & aSkill(iId, aSkill(iId, iLevel).ench_skill_id).name & vbTab

                        'route_id = 1
                        If aSkill(iId, iLevel - 1).ench_skill_id = 0 Then
                            ' this first level for enchant
                            If iRouteId = 1 Then iRouteId = 2 Else iRouteId = 1
                            iEnchId = 0
                            If sEnchName <> aSkill(iId, aSkill(iId, iLevel).ench_skill_id).name Then
                                iRouteId = 1
                                sEnchName = aSkill(iId, aSkill(iId, iLevel).ench_skill_id).name
                            End If
                        End If
                        sTemp = sTemp & "route_id=" & iRouteId & vbTab

                        'enchant_id = 1
                        iEnchId += 1
                        sTemp = sTemp & "enchant_id=" & iEnchId & vbTab

                        'skill_level = 101
                        sTemp = sTemp & "skill_level=" & iLevel & vbTab
                        'exp = 3060000
                        sTemp = sTemp & "exp=3000000" & vbTab
                        'sp = 306000
                        sTemp = sTemp & "sp=300000" & vbTab
                        'item_needed = {{[codex_of_giant];1}}
                        If aSkill(iId, iLevel - 1).ench_skill_id = 0 Then
                            ' this first level for enchant
                            sTemp = sTemp & "item_needed={{[codex_of_giant];1}}" & vbTab
                        Else
                            sTemp = sTemp & "item_needed={}" & vbTab
                        End If

                        'prob_76 = 82
                        sTemp = sTemp & "prob_76=" & EnchantChance(iEnchId + 5) & vbTab
                        'prob_77 = 92
                        sTemp = sTemp & "prob_77=" & EnchantChance(iEnchId + 2) & vbTab
                        'prob_78 = 97
                        sTemp = sTemp & "prob_78=" & EnchantChance(iEnchId - 1) & vbTab

                        'enchant_skill_end
                        sTemp = sTemp & "enchant_skill_end" & vbTab

                        ' Checking for error in skillgrp. Example, enchskill_level=9 but last skilllevel=4
                        If aSkill(iId, aSkill(iId, iLevel).ench_skill_id).name = "" Or aSkill(iId, aSkill(iId, iLevel).ench_skill_id).name Is Nothing Then
                            sTemp = "//ERR: enchant skill_id:" & iId & " level:" & iLevel & vbTab & sTemp
                        End If

                        outEncFile.WriteLine(sTemp)
                        sTemp = ""
                    End If
                    ' ------------ End of SkillEnchant Module --------------

                End If
            Next
            ToolStripProgressBar.Value = iId
            StatusStrip.Update()
            'Me.Update()

        Next
        outFile.Close()
        outEncFile.Close()
        ToolStripProgressBar.Value = 0

        ToolStripStatusLabel.Text = "Complete"
        MessageBox.Show("Complete")

    End Sub

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

        Dim iId As Integer, iLevel As Integer

        While inFile.EndOfStream <> True

            '[s_power_strike11] = 769
            Try
                sTempStr = inFile.ReadLine
                sTempStr = sTempStr.Replace(" ", "").Replace(Chr(9), "") '.Replace("[", "").Replace("]", "")
                aTemp = sTempStr.Split(CChar("="))

                iId = CInt(Fix(CInt(aTemp(1)) / 256))
                iLevel = CInt(aTemp(1)) - iId * 256
                aSkill(iId, iLevel).name = aTemp(0)

            Catch ex As Exception
                MessageBox.Show("Import data invalid in line" & vbNewLine & sTempStr, "Import data invalid", MessageBoxButtons.OK, MessageBoxIcon.Error)
                inFile.Close()
                Return False
            End Try

        End While

        inFile.Close()
        Return True

    End Function

    Private Sub QuitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles QuitButton.Click
        Me.Dispose()
    End Sub

End Class