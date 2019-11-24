Public Class SkillPch2Maker

    'Dim TabSymbol As String = Chr(9)
    Dim TabSymbol As String = " "

    Private Function LoadingManual(ByVal FileName As String) As Boolean

        Dim sTemp As String
        Dim inAbFile As New System.IO.StreamReader(FileName, System.Text.Encoding.Default, True, 1)

        AbnormalListTextBox.Text = ""

        '[none] = -1
        While inAbFile.EndOfStream <> True

            sTemp = inAbFile.ReadLine.Trim.ToLower
            If sTemp.StartsWith("[ab_") = True Then
                sTemp = sTemp.Replace(Chr(32), "").Replace(Chr(9), "")  ' .Replace("[ab_", "[")
                AbnormalListTextBox.AppendText(sTemp & vbNewLine)
            End If

            If sTemp.StartsWith("[attr_") = True Or sTemp.StartsWith("[trait_") = True Then
                sTemp = sTemp.Replace(Chr(32), "").Replace(Chr(9), "")  ' .Replace("[ab_", "[")
                AbnormalListTextBox.AppendText(sTemp & vbNewLine)
            End If

        End While

        inAbFile.Close()
        Return True

    End Function

    Private Sub LoadAbListButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LoadAbListButton.Click

        Dim sAbFile As String
        OpenFileDialog.InitialDirectory = System.Environment.CurrentDirectory
        OpenFileDialog.Filter = "Lineage II definition config (manual_pch.txt)|manual_pch.txt|All files (*.*)|*.*"
        If OpenFileDialog.ShowDialog() = Windows.Forms.DialogResult.Cancel Then Exit Sub
        sAbFile = OpenFileDialog.FileName

        LoadingManual(sAbFile)

        MessageBox.Show("Abnormal list loaded success. Made required correction in listist and use Pch/Pch2 generator", "Load success", MessageBoxButtons.OK, MessageBoxIcon.Information)

    End Sub


    Private Sub StartButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles StartButton.Click

        Dim TempStr As String
        Dim sAttr As String, sAttr2 As String 'sAttr1 As String, 
        Dim SkillDataFile, SkillDataDir As String

        'Dim aAbnList(0) As String

        Dim aAbList(0) As String
        Dim aAttrList(0) As String

        If UseCustomAbnormalsCheckBox.Checked = True Then
            If AbnormalListTextBox.Lines.Length = 0 Then
                MessageBox.Show("No abnormals and attributes in text window. Load from manual_pch file or enter manualy with format [abnormal_name] = abnormal_id", "No data", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Exit Sub
            End If

            Dim iTemp As Integer
            Dim aTemp() As String
            For iTemp = 0 To AbnormalListTextBox.Lines.Length - 1

                aTemp = AbnormalListTextBox.Lines(iTemp).ToLower.Split(CChar("="))


                If aTemp(0).StartsWith("[ab_") = True Then
                    ' Preparing text to parsing.
                    If CheckBoxAsIs.Checked = False Then
                        aTemp(0) = aTemp(0).Replace("[ab_", "[")
                    End If
                    aTemp(0) = aTemp(0).Replace("[", "").Replace("]", "").Replace(" ", "").Replace(Chr(9), "")
                    If CInt(aTemp(1)) >= aAbList.Length Then
                        Array.Resize(aAbList, CInt(aTemp(1)) + 1)
                    End If
                    If CInt(aTemp(1)) > -1 Then aAbList(CInt(aTemp(1))) = aTemp(0)
                End If

                If aTemp(0).StartsWith("[attr_") = True Or aTemp(0).StartsWith("[trait_") = True Then

                    ' Preparing text to parsing.
                    If CheckBoxAsIs.Checked = False Then
                        aTemp(0) = aTemp(0).Replace("[ab_", "[")
                    End If
                    aTemp(0) = aTemp(0).Replace("[", "").Replace("]", "").Replace(" ", "").Replace(Chr(9), "")

                    If CInt(aTemp(1)) >= aAttrList.Length Then
                        Array.Resize(aAttrList, CInt(aTemp(1)) + 1)
                    End If
                    If CInt(aTemp(1)) > -1 Then aAttrList(CInt(aTemp(1))) = aTemp(0)

                End If


            Next

        End If

        OpenFileDialog.InitialDirectory = System.Environment.CurrentDirectory
        OpenFileDialog.Filter = "Lineage II skill config|skilldata*.txt|All files (*.*)|*.*"
        If OpenFileDialog.ShowDialog() = Windows.Forms.DialogResult.Cancel Then
            Exit Sub
        End If
        SkillDataFile = OpenFileDialog.FileName
        SkillDataDir = System.IO.Path.GetDirectoryName(SkillDataFile)

        Dim inScillData As New System.IO.StreamReader(SkillDataFile, System.Text.Encoding.Default, True, 1)
        Dim oPchFile As System.IO.Stream = New System.IO.FileStream(SkillDataDir + "\skill_pch.txt", _
                IO.FileMode.Create, IO.FileAccess.Write)
        Dim outScillPchData As System.IO.StreamWriter = New System.IO.StreamWriter(oPchFile, _
                System.Text.Encoding.Unicode)

        Dim oPch2File As System.IO.Stream = New System.IO.FileStream(SkillDataDir + "\skill_pch2.txt", _
                IO.FileMode.Create, IO.FileAccess.Write)
        Dim outScillPch2Data As System.IO.StreamWriter = New System.IO.StreamWriter(oPch2File, _
                System.Text.Encoding.Unicode)

        ProgressBar.Maximum = CInt(inScillData.BaseStream.Length)
        ProgressBar.Value = 0

        Dim iT1id As Integer = 0
        If CheckBoxKamaelIDStandart.Checked = False Then
            iT1id = 256
        Else
            iT1id = 65536
        End If

        While inScillData.BaseStream.Position <> inScillData.BaseStream.Length

            TempStr = Replace(inScillData.ReadLine, Chr(9), TabSymbol)
            TempStr = Replace(TempStr, " = ", "=")

            ' null string fix
            If TempStr <> "" Then
                If Mid(TempStr, 1, 2) <> "//" Then

                    ' check: skill_begin or not?
                    If Mid(TempStr, 1, 11) <> "skill_begin" And Mid(TempStr, 1, 2) <> "//" Then
                        MessageBox.Show("File is not Skilldata or bad data." & Chr(13) & Chr(10) & TempStr, "Bad data", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Exit While
                    End If

                    outScillPchData.WriteLine(Libraries.GetNeedParamFromStr(TempStr, "skill_name") & TabSymbol & "=" & TabSymbol & (Val(Libraries.GetNeedParamFromStr(TempStr, "skill_id")) * iT1id + Val(Libraries.GetNeedParamFromStr(TempStr, "level"))).ToString)

                    ' Stage 1 (Ready from Pch file) ! Need make ID generator
                    '769=ID from skill_pch ----ok
                    outScillPch2Data.Write((Val(Libraries.GetNeedParamFromStr(TempStr, "skill_id")) * iT1id + Val(Libraries.GetNeedParamFromStr(TempStr, "level"))).ToString & TabSymbol)

                    ' Stage 2 (Ready)
                    '50=cast_range ------ok  cast_range = 400 
                    'outScillPch2Data.Write(Libraries.GetNeedParamFromStr(TempStr, "cast_range") & TabSymbol)
                    Select Case Libraries.GetNeedParamFromStr(TempStr, "cast_range")
                        Case Is <> ""
                            outScillPch2Data.Write(Libraries.GetNeedParamFromStr(TempStr, "cast_range") & TabSymbol)
                        Case Else
                            outScillPch2Data.Write("0" & TabSymbol)
                    End Select

                    ' Stage 3 (Ready)
                    '0=hp_consume ------ok
                    'outScillPch2Data.Write(Libraries.GetNeedParamFromStr(TempStr, "hp_consume") & TabSymbol)
                    Select Case Libraries.GetNeedParamFromStr(TempStr, "hp_consume")
                        Case Is <> ""
                            outScillPch2Data.Write(Int(Val(Libraries.GetNeedParamFromStr(TempStr, "hp_consume"))).ToString & TabSymbol)
                        Case Else
                            outScillPch2Data.Write("0" & TabSymbol)
                    End Select

                    ' Stage 4 (Ready)
                    '10=mp_consume2 ------problem here,if there's CONSUME1 it should be(mp_consume 1+2)
                    Dim Mp_consume1, Mp_consume2 As Double
                    Mp_consume1 = Val(Libraries.GetNeedParamFromStr(TempStr, "mp_consume1"))
                    Mp_consume2 = Val(Libraries.GetNeedParamFromStr(TempStr, "mp_consume2"))
                    outScillPch2Data.Write(Int(Mp_consume1 + Mp_consume2).ToString & TabSymbol)

                    ' Stage 5 (Ready)
                    '3=target_type ------ok

                    '[STGT_SELF]			=	0
                    '[STGT_TARGET]			=	1
                    '[STGT_OTHERS]			=	2
                    '[STGT_ENEMY]			=	3
                    '[STGT_ENEMY_ONLY]		=	4
                    '[STGT_REAL_ENEMY_ONLY]		=	4
                    '[STGT_ITEM]			=	5
                    '[STGT_SUMMON]			=	6
                    '[STGT_HOLYTHING]		=	7
                    '[STGT_MY_PLEDGE]		=	8
                    '[STGT_DOOR_TREASURE]		=	9
                    '[STGT_PC_BODY]			=	10
                    '[STGT_NPC_BODY]			=	11
                    '[STGT_WYVERN_TARGET]		=	12
                    '[STGT_GROUND]			=	13
                    '[STGT_NONE] = 14
                    ' default metod from old configs and etc
                    Select Case Libraries.GetNeedParamFromStr(TempStr, "target_type")
                        Case "self"
                            outScillPch2Data.Write("0" & TabSymbol)
                        Case "target"
                            outScillPch2Data.Write("1" & TabSymbol)
                        Case "others"
                            outScillPch2Data.Write("2" & TabSymbol)
                        Case "enemy"
                            outScillPch2Data.Write("3" & TabSymbol)
                        Case "enemy_only"
                            outScillPch2Data.Write("4" & TabSymbol)
                        Case "real_enemy_only"
                            outScillPch2Data.Write("4" & TabSymbol)
                        Case "item"
                            outScillPch2Data.Write("5" & TabSymbol)
                        Case "summon"
                            outScillPch2Data.Write("6" & TabSymbol)
                        Case "holything"
                            'NPC_ENEMY_ONLY_BUT_SIEGE
                            outScillPch2Data.Write("7" & TabSymbol)
                        Case "my_pledge"
                            outScillPch2Data.Write("8" & TabSymbol)
                        Case "door_treasure"
                            outScillPch2Data.Write("9" & TabSymbol)
                        Case "pc_body"
                            outScillPch2Data.Write("10" & TabSymbol)
                        Case "npc_body"
                            outScillPch2Data.Write("11" & TabSymbol)
                        Case "wyvern_target"
                            outScillPch2Data.Write("12" & TabSymbol)
                        Case "ground"
                            outScillPch2Data.Write("13" & TabSymbol)
                        Case "pvp_target"
                            outScillPch2Data.Write("14" & TabSymbol)
                        Case "none"
                            outScillPch2Data.Write("15" & TabSymbol)
                        Case ""
                            outScillPch2Data.Write("15" & TabSymbol)
                        Case Else
                            'outScillPch2Data.Write("0" & TabSymbol)
                            MessageBox.Show("Unknown target_type: " + Libraries.GetNeedParamFromStr(TempStr, "target_type"), "Unknown target_type", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            Exit Sub
                    End Select

                    ' Stage 6 (Ready)
                    '-52=effect_point -----ok
                    'outScillPch2Data.Write(Libraries.GetNeedParamFromStr(TempStr, "effect_point") & TabSymbol)
                    Select Case Libraries.GetNeedParamFromStr(TempStr, "effect_point")
                        Case Is <> ""
                            outScillPch2Data.Write(Libraries.GetNeedParamFromStr(TempStr, "effect_point") & TabSymbol)
                        Case Else
                            outScillPch2Data.Write("0" & TabSymbol)
                    End Select

                    ' Stage 7 (Ready)
                    '29=attribute -----ok
                    ' default metod from old configs and etc
                    If La2GuardAttrCheckBox.Checked = False Then

                        If Array.IndexOf(aAttrList, Libraries.GetNeedParamFromStr(TempStr, "attribute")) = -1 And Libraries.GetNeedParamFromStr(TempStr, "attribute") <> "" Then
                            MessageBox.Show("Unknown attribute: " & Libraries.GetNeedParamFromStr(TempStr, "attribute") & vbNewLine & _
                                            "into skill_name:" & Libraries.GetNeedParamFromStr(TempStr, "skill_name") & "skill_id:" & Libraries.GetNeedParamFromStr(TempStr, "skill_id") & " level" & Libraries.GetNeedParamFromStr(TempStr, "level"), _
                                            "Unknown attribute", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            ProgressBar.Value = 0
                            inScillData.Close()
                            outScillPchData.Close()
                            outScillPch2Data.Close()
                            Exit Sub
                        End If
                        outScillPch2Data.Write(Array.IndexOf(aAttrList, Libraries.GetNeedParamFromStr(TempStr, "attribute")) & TabSymbol)

                    Else

                        sAttr = "attr_none"

                        'If Libraries.GetNeedParamFromStr(TempStr, "skill_id") = "113" Then
                        '    Dim asd As Boolean = True
                        'End If

                        'sAttr1 = Libraries.GetNeedParamFromStr(TempStr, "attribute")
                        'If sAttr1.Length > 1 Then sAttr1 = sAttr1.Substring(1, sAttr1.IndexOf(";") - 1)
                        'If Array.IndexOf(aAttrList, sAttr1) = -1 And sAttr1 <> "" Then
                        '    MessageBox.Show("Unknown attribute: " & sAttr1 & vbNewLine & _
                        '                    "into skill_name:" & Libraries.GetNeedParamFromStr(TempStr, "skill_name") & "skill_id:" & Libraries.GetNeedParamFromStr(TempStr, "skill_id") & " level" & Libraries.GetNeedParamFromStr(TempStr, "level"), _
                        '                    "Unknown attribute", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        '    ProgressBar.Value = 0
                        '    inScillData.Close()
                        '    outScillPchData.Close()
                        '    outScillPch2Data.Close()
                        '    Exit Sub
                        'Else
                        '    ' Attribute now is 'attribute' param
                        '    If sAttr1 <> "" Then sAttr = sAttr1
                        'End If

                        sAttr2 = Libraries.GetNeedParamFromStr(TempStr, "trait")
                        If sAttr2.Length > 1 Then sAttr2 = sAttr2.Substring(1, sAttr2.IndexOf("}") - 1)
                        If Array.IndexOf(aAttrList, sAttr2) = -1 And sAttr2 <> "" Then
                            MessageBox.Show("Unknown trait: " & sAttr2 & vbNewLine & _
                                            "into skill_name:" & Libraries.GetNeedParamFromStr(TempStr, "skill_name") & "skill_id:" & Libraries.GetNeedParamFromStr(TempStr, "skill_id") & " level" & Libraries.GetNeedParamFromStr(TempStr, "level"), _
                                            "Unknown trait", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            ProgressBar.Value = 0
                            inScillData.Close()
                            outScillPchData.Close()
                            outScillPch2Data.Close()
                            Exit Sub
                        Else
                            If sAttr2 <> "trait_none" Then
                                Dim asd As Boolean = True
                                If sAttr2 <> "" Then sAttr = sAttr2
                            End If
                            ' Attribute now is 'attribute' param
                        End If
                        outScillPch2Data.Write(Array.IndexOf(aAttrList, sAttr) & TabSymbol)

                    End If
                    
                    ' Stage 8
                    '-1=abnormal_type -----ok
                    ' default metod from old configs and etc
                    ' metod from manual_pch

                    If UseCustomAbnormalsCheckBox.Checked = True Then
                        Select Case Libraries.GetNeedParamFromStr(TempStr, "abnormal_type")
                            Case "none"
                                outScillPch2Data.Write("-1" & TabSymbol)
                            Case ""
                                outScillPch2Data.Write("-1" & TabSymbol)
                            Case Else
                                If Array.IndexOf(aAbList, Libraries.GetNeedParamFromStr(TempStr, "abnormal_type")) = -1 Then
                                    If IgnoreCustomAbnormalsCheckBox.Checked = False Then
                                        MessageBox.Show("Unknown abnormal_type: " + Libraries.GetNeedParamFromStr(TempStr, "abnormal_type") & vbNewLine & _
                                                        "into skill_name:" & Libraries.GetNeedParamFromStr(TempStr, "skill_name") & "skill_id:" & Libraries.GetNeedParamFromStr(TempStr, "skill_id") & " level" & Libraries.GetNeedParamFromStr(TempStr, "level"), _
                                                        "Unknown abnormal_type", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                        ProgressBar.Value = 0
                                        inScillData.Close()
                                        outScillPchData.Close()
                                        outScillPch2Data.Close()
                                        Exit Sub
                                    End If
                                End If
                                outScillPch2Data.Write(Array.IndexOf(aAbList, Libraries.GetNeedParamFromStr(TempStr, "abnormal_type")) & TabSymbol)
                        End Select
                    Else
                        Select Case Libraries.GetNeedParamFromStr(TempStr, "abnormal_type")
                            Case "none"
                                outScillPch2Data.Write("-1" & TabSymbol)
                            Case "pa_up"
                                outScillPch2Data.Write("0" & TabSymbol)
                            Case "pa_up_special"
                                outScillPch2Data.Write("1" & TabSymbol)
                            Case "pa_down"
                                outScillPch2Data.Write("2" & TabSymbol)
                            Case "pd_up"
                                outScillPch2Data.Write("3" & TabSymbol)
                            Case "pd_up_special"
                                outScillPch2Data.Write("4" & TabSymbol)
                            Case "ma_up"
                                outScillPch2Data.Write("5" & TabSymbol)
                            Case "md_up"
                                outScillPch2Data.Write("6" & TabSymbol)
                            Case "md_up_attr"
                                outScillPch2Data.Write("7" & TabSymbol)
                            Case "avoid_up"
                                outScillPch2Data.Write("8" & TabSymbol)
                            Case "avoid_up_special"
                                outScillPch2Data.Write("9" & TabSymbol)
                            Case "hit_up"
                                outScillPch2Data.Write("10" & TabSymbol)
                            Case "hit_down"
                                outScillPch2Data.Write("11" & TabSymbol)
                            Case "fatal_poison"
                                outScillPch2Data.Write("12" & TabSymbol)
                            Case "fly_away"
                                outScillPch2Data.Write("13" & TabSymbol)
                            Case "turn_stone"
                                outScillPch2Data.Write("14" & TabSymbol)
                            Case "casting_time_down"
                                outScillPch2Data.Write("15" & TabSymbol)
                            Case "attack_time_down"
                                outScillPch2Data.Write("16" & TabSymbol)
                            Case "speed_up"
                                outScillPch2Data.Write("17" & TabSymbol)
                            Case "possession"
                                outScillPch2Data.Write("18" & TabSymbol)
                            Case "attack_time_up"
                                outScillPch2Data.Write("19" & TabSymbol)
                            Case "speed_down"
                                outScillPch2Data.Write("20" & TabSymbol)
                            Case "hp_regen_up"
                                outScillPch2Data.Write("21" & TabSymbol)
                            Case "max_mp_up"
                                outScillPch2Data.Write("22" & TabSymbol)
                            Case "antaras_debuff"
                                outScillPch2Data.Write("23" & TabSymbol)
                            Case "critical_prob_up"
                                outScillPch2Data.Write("24" & TabSymbol)
                            Case "cancel_prob_down"
                                outScillPch2Data.Write("25" & TabSymbol)
                            Case "bow_range_up"
                                outScillPch2Data.Write("26" & TabSymbol)
                            Case "max_breath_up"
                                outScillPch2Data.Write("27" & TabSymbol)
                            Case "decrease_weight_penalty"
                                outScillPch2Data.Write("28" & TabSymbol)
                            Case "poison"
                                outScillPch2Data.Write("29" & TabSymbol)
                            Case "bleeding"
                                outScillPch2Data.Write("30" & TabSymbol)
                            Case "dot_attr"
                                outScillPch2Data.Write("31" & TabSymbol)
                            Case "dot_mp"
                                outScillPch2Data.Write("32" & TabSymbol)
                            Case "dmg_shield"
                                outScillPch2Data.Write("33" & TabSymbol)
                            Case "hawk_eye"
                                outScillPch2Data.Write("34" & TabSymbol)
                            Case "resist_shock"
                                outScillPch2Data.Write("35" & TabSymbol)
                            Case "paralyze"
                                outScillPch2Data.Write("36" & TabSymbol)
                            Case "public_slot"
                                outScillPch2Data.Write("37" & TabSymbol)
                            Case "silence"
                                outScillPch2Data.Write("38" & TabSymbol)
                            Case "derangement"
                                outScillPch2Data.Write("39" & TabSymbol)
                            Case "stun"
                                outScillPch2Data.Write("40" & TabSymbol)
                            Case "resist_poison"
                                outScillPch2Data.Write("41" & TabSymbol)
                            Case "resist_derangement"
                                outScillPch2Data.Write("42" & TabSymbol)
                            Case "resist_spiritless"
                                outScillPch2Data.Write("43" & TabSymbol)
                            Case "mp_regen_up"
                                outScillPch2Data.Write("44" & TabSymbol)
                            Case "md_down"
                                outScillPch2Data.Write("45" & TabSymbol)
                            Case "heal_effect_down"
                                outScillPch2Data.Write("46" & TabSymbol)
                            Case "turn_passive"
                                outScillPch2Data.Write("47" & TabSymbol)
                            Case "turn_flee"
                                outScillPch2Data.Write("48" & TabSymbol)
                            Case "vampiric_attack"
                                outScillPch2Data.Write("49" & TabSymbol)
                            Case "duelist_spirit"
                                outScillPch2Data.Write("50" & TabSymbol)
                            Case "hp_recover"
                                outScillPch2Data.Write("51" & TabSymbol)
                            Case "mp_recover"
                                outScillPch2Data.Write("52" & TabSymbol)
                            Case "root"
                                outScillPch2Data.Write("53" & TabSymbol)
                            Case "speed_up_special"
                                outScillPch2Data.Write("54" & TabSymbol)
                            Case "majesty"
                                outScillPch2Data.Write("55" & TabSymbol)
                            Case "pd_up_bow"
                                outScillPch2Data.Write("56" & TabSymbol)
                            Case "attack_speed_up_bow"
                                outScillPch2Data.Write("57" & TabSymbol)
                            Case "max_hp_up"
                                outScillPch2Data.Write("58" & TabSymbol)
                            Case "holy_attack"
                                outScillPch2Data.Write("59" & TabSymbol)
                            Case "sleep"
                                outScillPch2Data.Write("60" & TabSymbol)
                            Case "berserker"
                                outScillPch2Data.Write("61" & TabSymbol)
                            Case "pinch"
                                outScillPch2Data.Write("62" & TabSymbol)
                            Case "life_force"
                                outScillPch2Data.Write("63" & TabSymbol)
                            Case "song_of_earth"
                                outScillPch2Data.Write("64" & TabSymbol)
                            Case "song_of_life"
                                outScillPch2Data.Write("65" & TabSymbol)
                            Case "song_of_water"
                                outScillPch2Data.Write("66" & TabSymbol)
                            Case "song_of_warding"
                                outScillPch2Data.Write("67" & TabSymbol)
                            Case "song_of_wind"
                                outScillPch2Data.Write("68" & TabSymbol)
                            Case "song_of_hunter"
                                outScillPch2Data.Write("69" & TabSymbol)
                            Case "song_of_invocation"
                                outScillPch2Data.Write("70" & TabSymbol)
                            Case "song_of_vitality"
                                outScillPch2Data.Write("71" & TabSymbol)
                            Case "song_of_flame_guard"
                                outScillPch2Data.Write("72" & TabSymbol)
                            Case "song_of_storm_guard"
                                outScillPch2Data.Write("73" & TabSymbol)
                            Case "song_of_vengeance"                          ' No in Manual_pch
                                outScillPch2Data.Write("74" & TabSymbol)
                            Case "dance_of_warrior"                         ' 75 in Manual_pch
                                outScillPch2Data.Write("75" & TabSymbol)
                            Case "dance_of_inspiration"                     ' 76 in manual_pch
                                outScillPch2Data.Write("76" & TabSymbol)
                            Case "dance_of_mystic"                          ' 77 in manual
                                outScillPch2Data.Write("77" & TabSymbol)
                            Case "dance_of_fire"                            ' 78 in Manual_pch
                                outScillPch2Data.Write("78" & TabSymbol)
                            Case "dance_of_fury"                            ' 79 in Manual_pch
                                outScillPch2Data.Write("79" & TabSymbol)
                            Case "dance_of_concentration"                   ' 80 in manual_pch
                                outScillPch2Data.Write("80" & TabSymbol)
                            Case "dance_of_light"                           ' 81 in manual_pch
                                outScillPch2Data.Write("81" & TabSymbol)
                            Case "dance_of_vampire"                      ' 83 in manual_pch
                                outScillPch2Data.Write("82" & TabSymbol)
                            Case "dance_of_aqua_guard"                      ' 83 in manual_pch
                                outScillPch2Data.Write("83" & TabSymbol)
                            Case "dance_of_earth_guard"                     ' 84 in Manual_pch
                                outScillPch2Data.Write("84" & TabSymbol)
                            Case "dance_of_protection"                      ' 85 in Manual_pch
                                outScillPch2Data.Write("85" & TabSymbol)
                            Case "detect_weakness"                          ' 86 in Manual_pch
                                outScillPch2Data.Write("86" & TabSymbol)
                            Case "thrill_fight"                             ' 87 in Manual_pch
                                outScillPch2Data.Write("87" & TabSymbol)
                            Case "resist_bleeding"                          ' 88 in Manual_pch
                                outScillPch2Data.Write("88" & TabSymbol)
                            Case "critical_dmg_up"                          ' 89 in Manual_pch
                                outScillPch2Data.Write("89" & TabSymbol)
                            Case "shield_prob_up"                           ' 90 in Manual_pch
                                outScillPch2Data.Write("90" & TabSymbol)
                            Case "hp_regen_down"                            ' 91 in Manual_pch
                                outScillPch2Data.Write("91" & TabSymbol)
                            Case "reuse_delay_up"                           ' 92 in Manual_pch
                                outScillPch2Data.Write("92" & TabSymbol)
                            Case "pd_down"                                  ' 93 in Manual_pch
                                outScillPch2Data.Write("93" & TabSymbol)
                            Case "big_head"                                 ' 94 in Manual_pch
                                outScillPch2Data.Write("94" & TabSymbol)
                            Case "snipe"                                    ' No in Manual_pch
                                outScillPch2Data.Write("95" & TabSymbol)
                            Case "cheap_magic"                              ' No in Manual_pch
                                outScillPch2Data.Write("96" & TabSymbol)
                            Case "magic_critical_up"                        ' No in Manual_pch
                                outScillPch2Data.Write("97" & TabSymbol)
                            Case "shield_defence_up"                        ' No in Manual_pch
                                outScillPch2Data.Write("98" & TabSymbol)
                            Case "rage_might"                               ' No in Manual_pch
                                outScillPch2Data.Write("99" & TabSymbol)
                            Case "ultimate_buff"                            ' No in Manual_pch
                                outScillPch2Data.Write("100" & TabSymbol)
                            Case "reduce_drop_penalty"                      ' No in Manual_pch
                                outScillPch2Data.Write("101" & TabSymbol)
                            Case "heal_effect_up"                           ' No in Manual_pch
                                outScillPch2Data.Write("102" & TabSymbol)
                            Case "ssq_town_curse"                           ' No in Manual_pch
                                outScillPch2Data.Write("103" & TabSymbol)
                            Case "ssq_town_blessing"                        ' No in Manual_pch
                                outScillPch2Data.Write("104" & TabSymbol)
                            Case "big_body"                                 ' No in Manual_pch
                                outScillPch2Data.Write("105" & TabSymbol)
                            Case "preserve_abnormal"                        ' No in Manual_pch
                                outScillPch2Data.Write("106" & TabSymbol)
                            Case "spa_disease_a"                            ' No in Manual_pch
                                outScillPch2Data.Write("107" & TabSymbol)
                            Case "spa_disease_b"                            ' No in Manual_pch
                                outScillPch2Data.Write("108" & TabSymbol)
                            Case "spa_disease_c"                            ' No in Manual_pch
                                outScillPch2Data.Write("109" & TabSymbol)
                            Case "spa_disease_d"                            ' No in Manual_pch
                                outScillPch2Data.Write("110" & TabSymbol)
                            Case "avoid_down"                               ' No in Manual_pch
                                outScillPch2Data.Write("111" & TabSymbol)
                            Case "multi_buff"                               ' No in Manual_pch
                                outScillPch2Data.Write("112" & TabSymbol)
                            Case "dragon_breath"                            ' No in Manual_pch
                                outScillPch2Data.Write("113" & TabSymbol)
                            Case "ultimate_debuff"                          ' No in Manual_pch
                                outScillPch2Data.Write("114" & TabSymbol)
                            Case "buff_queen_of_cat"                        ' No in Manual_pch
                                outScillPch2Data.Write("115" & TabSymbol)
                            Case "buff_unicorn_seraphim"                    ' No in Manual_pch
                                outScillPch2Data.Write("116" & TabSymbol)
                            Case "debuff_nightshade"                        ' No in Manual_pch
                                outScillPch2Data.Write("117" & TabSymbol)
                            Case "watcher_gaze"                             ' No in Manual_pch
                                outScillPch2Data.Write("118" & TabSymbol)
                            Case "song_of_renewal"                             ' No in Manual_pch
                                outScillPch2Data.Write("119" & TabSymbol)
                            Case "song_of_champion"                             ' No in Manual_pch
                                outScillPch2Data.Write("120" & TabSymbol)
                            Case "song_of_meditation"                             ' No in Manual_pch
                                outScillPch2Data.Write("121" & TabSymbol)
                            Case "dance_of_siren"                             ' No in Manual_pch
                                outScillPch2Data.Write("122" & TabSymbol)
                            Case "dance_of_shadow"                             ' No in Manual_pch
                                outScillPch2Data.Write("123" & TabSymbol)
                            Case "multi_debuff"                             ' No in Manual_pch
                                outScillPch2Data.Write("124" & TabSymbol)
                            Case "reflect_abnormal"                         ' No in Manual_pch
                                outScillPch2Data.Write("125" & TabSymbol)
                            Case "focus_dagger"                             ' No in Manual_pch
                                outScillPch2Data.Write("126" & TabSymbol)
                            Case "max_hp_down"                              ' No in Manual_pch
                                outScillPch2Data.Write("127" & TabSymbol)
                            Case "resist_holy_unholy"                       ' No in Manual_pch
                                outScillPch2Data.Write("128" & TabSymbol)
                            Case "resist_debuff_dispel"                     ' No in Manual_pch
                                outScillPch2Data.Write("129" & TabSymbol)
                            Case "touch_of_life"                            ' No in Manual_pch
                                outScillPch2Data.Write("130" & TabSymbol)
                            Case "touch_of_death"                           ' No in Manual_pch
                                outScillPch2Data.Write("131" & TabSymbol)
                            Case "silence_physical"                         ' No in Manual_pch
                                outScillPch2Data.Write("132" & TabSymbol)
                            Case "silence_all"                              ' No in Manual_pch
                                outScillPch2Data.Write("133" & TabSymbol)
                            Case "valakas_item"                             ' No in Manual_pch
                                outScillPch2Data.Write("134" & TabSymbol)
                            Case "gara"                                     ' No in Manual_pch
                                outScillPch2Data.Write("135" & TabSymbol)
                            Case ""
                                outScillPch2Data.Write("-1" & TabSymbol)
                            Case Else
                                MessageBox.Show("Unknown abnormal_type: " + Libraries.GetNeedParamFromStr(TempStr, "abnormal_type"), "Unknown abnormal_type", MessageBoxButtons.OK, MessageBoxIcon.Error)
                                Exit Sub
                        End Select

                    End If


                    ' Stage 9 (Ready)
                    '0=abnormal_lv -----ok
                    '----- what's below are just guess/estimate, it's skills for groups for 4-5 people
                    Select Case Libraries.GetNeedParamFromStr(TempStr, "abnormal_lv")
                        Case Is <> ""
                            outScillPch2Data.Write(Libraries.GetNeedParamFromStr(TempStr, "abnormal_lv") & TabSymbol)
                        Case Else
                            outScillPch2Data.Write("0" & TabSymbol)
                    End Select

                    ' Stage 10 
                    '2= -----this is skill_hit_time, note, it's meant for a group for 4-5 people 1.5=2
                    Dim skill_hit_time, skill_hit_cancel_time, reuse_delay, skill_cool_time As Double
                    Dim dTemp As Double

                    reuse_delay = Val(Libraries.GetNeedParamFromStr(TempStr, "reuse_delay"))
                    skill_hit_time = Val(Libraries.GetNeedParamFromStr(TempStr, "skill_hit_time"))
                    skill_hit_cancel_time = Val(Libraries.GetNeedParamFromStr(TempStr, "skill_hit_cancel_time"))
                    skill_cool_time = Val(Libraries.GetNeedParamFromStr(TempStr, "skill_cool_time"))

                    Select Case Libraries.GetNeedParamFromStr(TempStr, "skill_hit_time")
                        Case Is <> ""
                            dTemp = Math.Round((skill_hit_time + skill_cool_time), MidpointRounding.AwayFromZero)
                            outScillPch2Data.Write(CInt(dTemp).ToString & TabSymbol)
                        Case Else
                            outScillPch2Data.Write("0" & TabSymbol)
                    End Select

                    ' Stage 11 (Ready)
                    '11= -----[reuse_delay - (skill_hit_time - skill_hit_cancel_time)]\
                    'skill_hit_time = 1.08 skill_hit_cancel_time = 0.5 reuse_delay = 13 (skill_cool_time = 0.72 )

                    'dTemp = reuse_delay - (skill_hit_time - skill_hit_cancel_time) ' original
                    'aas = Int(reuse_delay - (skill_hit_time - skill_hit_cancel_time) - skill_cool_time) ' ideal work
                    'dTemp = reuse_delay - (skill_hit_time - skill_hit_cancel_time) - skill_cool_time
                    'dTemp = reuse_delay - (skill_hit_time - skill_hit_cancel_time + skill_cool_time)
                    'dTemp = reuse_delay - (skill_hit_time + skill_cool_time)

                    dTemp = reuse_delay - skill_hit_time - skill_cool_time
                    dTemp = Math.Round(dTemp, MidpointRounding.AwayFromZero)
                    If dTemp < 0 Then
                        dTemp += 1
                    End If
                    ' --- old code ----
                    ' --- old code ----

                    'dTemp = Fix(Int(dTemp))
                    dTemp = CInt(dTemp)
                    'aas = Val(Libraries.GetNeedParamFromStr(TempStr, "reuse_delay")) - Val(Libraries.GetNeedParamFromStr(TempStr, "skill_hit_time")) - Val(Libraries.GetNeedParamFromStr(TempStr, "skill_hit_cancel_time"))
                    outScillPch2Data.Write(dTemp.ToString & TabSymbol)

                    ' Stage 12 (Ready)
                    '0= -----if IS_magic 1 else 0
                    'outScillPch2Data.Write(Libraries.GetNeedParamFromStr(TempStr, "is_magic") & TabSymbol)
                    Select Case Libraries.GetNeedParamFromStr(TempStr, "is_magic")
                        Case Is <> ""
                            outScillPch2Data.Write(Libraries.GetNeedParamFromStr(TempStr, "is_magic") & TabSymbol)
                        Case Else
                            outScillPch2Data.Write("0" & TabSymbol)
                    End Select

                    ' Stage 13 (Ready)
                    outScillPch2Data.Write("-12345" & Chr(13) & Chr(10))

                    ProgressBar.Value = CInt(inScillData.BaseStream.Position)
                Else

                    ' null string fix
                    'outScillPchData.WriteLine()
                    'outScillPch2Data.WriteLine()

                End If
            End If

        End While

        inScillData.Close()
        outScillPchData.Close()
        outScillPch2Data.Close()

        System.IO.File.Create(SkillDataDir + "\skill_pch3.txt")

        MessageBox.Show("Skill Pch/Pch2/Pch3 generation complete", "Complete", MessageBoxButtons.OK)

        ProgressBar.Value = 0
    End Sub

    Private Sub SkillCacheScript_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SkillCacheScript.Click

        OpenFileDialog.FileName = ""
        OpenFileDialog.Filter = "Lineage II config (skilldata.txt)|skilldata.txt|All files (*.*)|*.*"
        If OpenFileDialog.ShowDialog() = Windows.Forms.DialogResult.Cancel Then
            Exit Sub
        End If

        Dim inEFile As System.IO.StreamReader
        Try
            inEFile = New System.IO.StreamReader(System.IO.Path.GetDirectoryName(OpenFileDialog.FileName) + "\skillname-e.txt", System.Text.Encoding.Default, True, 1)
        Catch ex As Exception
            MessageBox.Show("You must have skillname-e in work folder for generation itemcache file. Put and try again.")
            Exit Sub
        End Try

        ' Initialization
        ProgressBar.Value = 0
        Dim inFile As New System.IO.StreamReader(OpenFileDialog.FileName, System.Text.Encoding.Default, True, 1)
        Dim oAiFile As System.IO.Stream = New System.IO.FileStream(System.IO.Path.GetDirectoryName(OpenFileDialog.FileName) + "\gmskilldata.txt", _
            IO.FileMode.Create, IO.FileAccess.Write)
        Dim outAiData As System.IO.StreamWriter = New System.IO.StreamWriter(oAiFile, _
                System.Text.Encoding.Unicode, 1)

        Dim ReadStr As String
        Dim ReadSplitStr() As String

        ProgressBar.Value = 0
        ProgressBar.Maximum = CInt(inEFile.BaseStream.Length)
        ' SkillId, SkillLevel
        ' 0 - name, 1 - desc
        Dim SkillDB(55000, 700, 1) As String
        Dim SkillDBMarker1 As Integer = 0, SkillDBMarker2 As Integer = 0


        While inEFile.EndOfStream <> True

            ReadStr = inEFile.ReadLine.Replace(" = ", "=")

            If ReadStr <> Nothing Then
                If Mid(ReadStr, 1, 2) <> "//" Then
                    ReadSplitStr = ReadStr.Split(Chr(9))
                    SkillDBMarker1 = CInt(ReadSplitStr(1).Replace("id=", ""))
                    SkillDBMarker2 = CInt(ReadSplitStr(2).Replace("level=", ""))
                    If SkillDBMarker1 > 55000 Then
                        MessageBox.Show("Skill ID must be less then 55000. Custom skills not supported. Last skill_id:" + SkillDBMarker1.ToString + " skill_level: " + SkillDBMarker2.ToString, "SkillID above them 30000", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        ProgressBar.Value = 0
                        inFile.Close()
                        outAiData.Close()
                        Exit Sub
                    End If

                    '4	1	[Dash]	[Temporary burst of speed. Effect 1.]	0	A2
                    SkillDB(SkillDBMarker1, SkillDBMarker2, 0) = ReadSplitStr(3).Replace("name=", "")
                    SkillDB(SkillDBMarker1, SkillDBMarker2, 1) = ReadSplitStr(4).Replace("description=", "")
                End If
            End If
            ProgressBar.Value = CInt(inFile.BaseStream.Position * 100 / inFile.BaseStream.Length)
        End While

        Dim SkillId As Integer
        Dim SkillLevel As Integer
        Dim SkillName As String
        Dim SkillDesc As String
        Dim SkillMagic As Short
        Dim SkillOp As String

        ProgressBar.Maximum = CInt(inFile.BaseStream.Length)
        ProgressBar.Value = 0
        Do While inFile.BaseStream.Position <> inFile.BaseStream.Length

            ReadStr = Replace(inFile.ReadLine, Chr(9), Chr(32))
            ' tabs and spaces correction
            While InStr(ReadStr, "  ") <> 0
                ReadStr = Replace(ReadStr, "  ", Chr(32))
            End While
            'ReadStr = Replace(inFile.ReadLine, Chr(32), Chr(9))
            '21      3       [s_poison_recovery3]    []      1       A1

            If ReadStr <> "" Then

                If ReadStr.StartsWith("//") = False Then


                    If InStr(ReadStr, "/*") <> 0 Then
                        ' Commentarie exist
                        'Dim commentarie As String = Mid(ReadStr, InStr(ReadStr, "/*"), InStr(ReadStr, "*/") - InStr(ReadStr, "/*") + 2)
                        ReadStr = ReadStr.Replace(Mid(ReadStr, InStr(ReadStr, "/*"), InStr(ReadStr, "*/") - InStr(ReadStr, "/*") + 2), "")
                    End If
                    ReadStr = Replace(ReadStr, " = ", "=")

                    ReadSplitStr = ReadStr.Split(Chr(32))

                    SkillId = CInt(Libraries.GetNeedParamFromStr(ReadStr, "skill_id"))
                    SkillLevel = CInt(Libraries.GetNeedParamFromStr(ReadStr, "level"))
                    SkillName = Libraries.GetNeedParamFromStr(ReadStr, "skill_name")
                    SkillDesc = "[no description]"
                    If Libraries.GetNeedParamFromStr(ReadStr, "is_magic") <> "" Then
                        SkillMagic = CShort(Libraries.GetNeedParamFromStr(ReadStr, "is_magic"))
                    Else
                        SkillMagic = 0
                    End If
                    SkillOp = Libraries.GetNeedParamFromStr(ReadStr, "operate_type")

                    If SkillId <= 55000 Then
                        If SkillDB(SkillId, SkillLevel, 0) <> "" Then
                            SkillName = SkillDB(SkillId, SkillLevel, 0)
                        End If
                        If SkillDB(SkillId, SkillLevel, 1) <> "" Then
                            SkillDesc = SkillDB(SkillId, SkillLevel, 1)
                        End If
                    Else
                        If IgnoreCustomSkillBox.Checked = True Then
                            SkillName = "[L2ScriptMaker: Customs skills not supported]"
                            SkillDesc = "[L2ScriptMaker: Customs skills not supported]"
                        Else
                            MessageBox.Show("Skill ID must be less then 55000. Custom skills not supported. Last skill_id:" + SkillId.ToString + " skill_level: " + SkillLevel.ToString, "SkillID above them 30000", MessageBoxButtons.OK, MessageBoxIcon.Error)
                            ProgressBar.Value = 0
                            inFile.Close()
                            outAiData.Close()
                            Exit Sub
                        End If
                    End If

                    '4	1	[Dash]	[Temporary burst of speed. Effect 1.]	0	A2
                    ' 500 symbols fix
                    If SkillDesc.Length > 255 Then SkillDesc = SkillDesc.Substring(0, 255) & "]"

                    outAiData.WriteLine(SkillId.ToString + Chr(9) + SkillLevel.ToString + Chr(9) + SkillName + Chr(9) + SkillDesc + Chr(9) + SkillMagic.ToString + Chr(9) + SkillOp)
                End If

            End If

            ProgressBar.Value = CInt(inFile.BaseStream.Position)
        Loop

        MessageBox.Show("gmskilldata.txt Complete")
        ProgressBar.Value = 0
        inFile.Close()
        outAiData.Close()
    End Sub

    Private Sub QuitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles QuitButton.Click
        Me.Dispose()
    End Sub

    Private Sub SkillPch2Maker_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If System.IO.File.Exists("manual_pch.txt") = True Then LoadingManual("manual_pch.txt")

    End Sub

End Class
