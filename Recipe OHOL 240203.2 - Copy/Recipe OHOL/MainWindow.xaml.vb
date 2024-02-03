Imports System.IO

Class MainWindow
	Public Class Trans
		Private _InOneID As Integer
		Public Property InOneID As Integer
			Get
				Return _InOneID
			End Get
			Set(ByVal value As Integer)
				_InOneID = value
			End Set
		End Property

		Private _InOneName As String
		Public Property InOneName As String
			Get
				Return _InOneName
			End Get
			Set(ByVal value As String)
				_InOneName = value
			End Set
		End Property

		Private _InTwoID As Integer
		Public Property InTwoID As Integer
			Get
				Return _InTwoID
			End Get
			Set(ByVal value As Integer)
				_InTwoID = value
			End Set
		End Property

		Private _InTwoName As String
		Public Property InTwoName As String
			Get
				Return _InTwoName
			End Get
			Set(ByVal value As String)
				_InTwoName = value
			End Set
		End Property

		Private _InTime As String
		Public Property InTime As String
			Get
				Return _InTime
			End Get
			Set(ByVal value As String)
				_InTime = value
			End Set
		End Property


		Private _OutOneID As Integer
		Public Property OutOneID As Integer
			Get
				Return _OutOneID
			End Get
			Set(ByVal value As Integer)
				_OutOneID = value
			End Set
		End Property

		Private _OutOneName As String
		Public Property OutOneName As String
			Get
				Return _OutOneName
			End Get
			Set(ByVal value As String)
				_OutOneName = value
			End Set
		End Property

		Private _OutTwoID As Integer
		Public Property OutTwoID As Integer
			Get
				Return _OutTwoID
			End Get
			Set(ByVal value As Integer)
				_OutTwoID = value
			End Set
		End Property

		Private _OutTwoName As String
		Public Property OutTwoName As String
			Get
				Return _OutTwoName
			End Get
			Set(ByVal value As String)
				_OutTwoName = value
			End Set
		End Property

		Private _OutTime As String
		Public Property OutTime As String
			Get
				Return _OutTime
			End Get
			Set(ByVal value As String)
				_OutTime = value
			End Set
		End Property
	End Class

	Public Class Obj
		Private _ID As Integer
		Public Property ID As Integer
			Get
				Return _ID
			End Get
			Set(ByVal value As Integer)
				_ID = value
			End Set
		End Property

		Private _Name As String
		Public Property Name As String
			Get
				Return _Name
			End Get
			Set(ByVal value As String)
				_Name = value
			End Set
		End Property

		Public Recipes(500) As Trans
		Public Uses(500) As Trans
		Public RecipeCount As Integer
		Public UseCount As Integer

	End Class

	Public Shared objects(10000) As Obj
	Public Shared transitions(10000) As Trans
	Public Shared ObjectCount As Integer = 0
	Public Shared TransitionCount As Integer = 0

	Public Shared SelectedType As String = "None"
	Public Shared SelectedObject As Obj
	Public Shared SelectedTrans As Trans

	Public Shared Queue(10) As Controls.Button

	Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs)
		Dim objectsDir = Directory.GetFiles("objects\")
		gridObjects.IsReadOnly = True

		For Each o In objectsDir
			Dim F As StreamReader = New StreamReader(o)
			Dim line As String = F.ReadLine()
			If line.Contains("id=") Then
				line = line.Remove(0, 3)
				Dim id = CType(line, Integer)
				Dim name As String = F.ReadLine()
				objects(id) = New Obj With {
					.ID = id,
					.Name = name
				}
				gridObjects.Items.Add(objects(id))
			End If
			F.Close()
			F.DiscardBufferedData()
			F.Dispose()

		Next

		objects(0) = New Obj With {
					.ID = 0,
					.Name = "HAND"
				}

		Dim transDir = Directory.GetFiles("transitions\")
		For Each o In transDir

			If o.Contains(".txt") = False Then
				Continue For
			End If

			Dim fileName As String = o.Substring(o.LastIndexOf("\") + 1).Replace(".txt", "")
			Dim InOne As String = fileName.Substring(0, fileName.IndexOf("_"))
			Dim InTwo As String = fileName.Substring(fileName.IndexOf("_") + 1)
			If InTwo.Contains("_") Then
				InTwo = InTwo.Substring(0, InTwo.IndexOf("_"))
			End If

			Dim rline As String = ""
			Dim OutOne As String = ""
			Dim OutTwo As String = ""
			Dim Time As String = ""
			Dim F As StreamReader = New StreamReader(o)


			rline = F.ReadLine()
			'MsgBox("Line:" + rline, MsgBoxStyle.Information)

			Dim index = 0
			For Each c As String In rline
				'MsgBox("Index:" + index.ToString + "Out:" + OutOne + " - " + OutTwo + " - " + Time, MsgBoxStyle.Information)
				If c <> " " Then
					Select Case index
						Case 0
							OutOne = String.Concat(OutOne, c)
						Case 1
							OutTwo = String.Concat(OutTwo, c)
						Case 2
							Time = String.Concat(Time, c)
					End Select
				Else
					index += 1
				End If
			Next

			'MsgBox("Out:" + OutOne + " - " + OutTwo + " - " + Time, MsgBoxStyle.Information)

			F.Close()
			F.DiscardBufferedData()
			F.Dispose()

			Dim t As New Trans

			If Time.Contains("h") Then
				t.InTime = Time
				t.OutTime = Time
			Else
				If CType(Time, Integer) >= 0 Then
					t.InTime = Time
					t.OutTime = Time
				Else
					t.InTime = Time
					t.OutTime = Time
				End If
			End If

			Dim i1 As Integer = CType(InOne, Integer)
			Dim i2 As Integer = CType(InTwo, Integer)
			Dim o1 As Integer = CType(OutOne, Integer)
			Dim o2 As Integer = CType(OutTwo, Integer)

			t.InOneID = i1
			If i1 < 0 Then
				t.InOneName = "TIME" + Math.Abs(i1).ToString + ": " + t.InTime
			Else
				t.InOneName = objects(i1).Name
			End If

			t.InTwoID = i2
			If i2 < 0 Then
				t.InTwoName = "TIME" + Math.Abs(i2).ToString + ": " + t.InTime
			Else
				t.InTwoName = objects(i2).Name
			End If

			If o1 < 0 Or o2 < 0 Then
				MsgBox("O1 or O2 <0, NO IDEA....", MsgBoxStyle.Information)
			Else
				t.OutOneID = o1
				t.OutTwoID = o2
				t.OutOneName = objects(o1).Name
				t.OutTwoName = objects(o2).Name
			End If

			transitions(TransitionCount) = t
			TransitionCount += 1
		Next

		'MsgBox("Loaded obj and trans, binding objects to transitions now!", MsgBoxStyle.Information)


		For Each ob As Obj In objects
			If IsNothing(ob) Then Continue For
			If ob.ID = 0 Then Continue For
			For Each tr As Trans In transitions
				If IsNothing(tr) Then Continue For
				If ob.ID = tr.InOneID Or ob.ID = tr.InTwoID Then
					Try
						ob.Uses(ob.UseCount) = tr
						ob.UseCount += 1
					Catch ex As Exception
						MsgBox("ERROR Adding uses, obj:" + ob.ID.ToString + " UseCount:" + ob.UseCount.ToString, MsgBoxStyle.Information)
					End Try

				End If

				If ob.ID = tr.OutOneID Or ob.ID = tr.OutTwoID Then
					Try
						ob.Recipes(ob.RecipeCount) = tr
						ob.RecipeCount += 1
					Catch ex As Exception
						MsgBox("ERROR Adding recipes, obj:" + ob.ID.ToString + " RecipeCount:" + ob.RecipeCount.ToString, MsgBoxStyle.Information)
					End Try

				End If

			Next
		Next

		'MsgBox("Binded all successfully", MsgBoxStyle.Information)
		Queue(0) = BtnQueue1
		Queue(1) = BtnQueue2
		Queue(2) = BtnQueue3
		Queue(3) = BtnQueue4
		Queue(4) = BtnQueue5
		Queue(5) = BtnQueue6
		Queue(6) = BtnQueue7
		Queue(7) = BtnQueue8
		Queue(8) = BtnQueue9
		Queue(9) = BtnQueue10
		Queue(10) = BtnQueue11

		For i As Integer = 0 To 10 Step 1
			Queue(i).Content = "NONE"
			Queue(i).Tag = Nothing
		Next

	End Sub

	Private Function FilterDataGrid(ByVal o As Object) As Boolean
		Dim s As Obj = New Obj
		s = TryCast(o, Obj)
		If (s.ID.ToString.Equals(txtFilterID.Text) Or txtFilterID.Text = "") And
			s.Name.ToLower.Contains(txtFilterName.Text.ToLower) Then
			Return True
		End If
	End Function


	Private Sub TextBox_TextChanged(sender As Object, e As TextChangedEventArgs)
		gridObjects.Items.Filter = New Predicate(Of Object)(AddressOf Me.FilterDataGrid)
	End Sub

	Private Sub DataGrid_SelectionChanged(sender As Object, e As SelectionChangedEventArgs)

		If e.AddedItems.Count > 0 Then
			If e.AddedItems.Item(0).GetType = objects(0).GetType Then
				Dim o As Obj = e.AddedItems.Item(0)
				'MsgBox(o.ID, MsgBoxStyle.Information)
				SelectedType = "Object"
				SelectedObject = o

				lblSelected.Content = SelectedType + ": " +
					SelectedObject.ID.ToString + ": " +
					SelectedObject.Name

				gridObjectsRecipes.Items.Clear()
				For Each r As Trans In o.Recipes
					If IsNothing(r) Then Continue For
					gridObjectsRecipes.Items.Add(r)
				Next

				gridObjectsUses.Items.Clear()
				For Each u As Trans In o.Uses
					If IsNothing(u) Then Continue For
					gridObjectsUses.Items.Add(u)
				Next
			ElseIf e.AddedItems.Item(0).GetType = transitions(0).GetType Then
				Dim t As Trans = e.AddedItems.Item(0)
				SelectedType = "Transition"
				SelectedTrans = t

				lblSelected.Content = SelectedType + ": " +
					SelectedTrans.InOneName + " + " +
					SelectedTrans.InTwoName + " = " +
					SelectedTrans.OutOneName + " + " +
					SelectedTrans.OutTwoName
			End If
		Else
			'lblSelected.Content = "Nothing"
		End If
	End Sub

	Private Sub GridObjects_BeginningEdit(sender As Object, e As DataGridBeginningEditEventArgs) Handles gridObjects.BeginningEdit
		Try

		Catch ex As Exception

		End Try
	End Sub

	Private Sub BtnAddQueue_Click(sender As Object, e As RoutedEventArgs) Handles btnAddQueue.Click
		If IsNothing(SelectedObject) Then
			MsgBox("Please select object first!", MsgBoxStyle.Critical)
		Else
			For i As Integer = 10 To 1 Step -1
				Queue(i).Content = Queue(i - 1).Content
				Queue(i).Tag = Queue(i - 1).Tag
			Next
			Queue(0).Content = SelectedObject.ID.ToString + ":" + SelectedObject.Name
			Queue(0).Tag = SelectedObject
		End If
	End Sub

	Private Sub BtnClearQueue_Click(sender As Object, e As RoutedEventArgs) Handles btnClearQueue.Click
		For i As Integer = 0 To 10 Step 1
			Queue(i).Content = "NONE"
			Queue(i).Tag = Nothing
		Next
	End Sub

	Private Sub BtnQueue_Click(sender As Object, e As RoutedEventArgs)
		If IsNothing(sender.tag) Then
			MsgBox("Nothing stored here!", MsgBoxStyle.Critical)
		Else
			SelectedType = "Object"
			SelectedObject = sender.tag

			Dim items(1) As Obj
			Dim remo(0) As Obj
			items(0) = SelectedObject
			Dim re As RoutedEvent = e.RoutedEvent

			Dim ee As New SelectionChangedEventArgs(re, remo, items) With {
				.Source = sender
			}

			DataGrid_SelectionChanged(sender, ee)
		End If
	End Sub
End Class
