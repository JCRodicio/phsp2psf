Public Class Form1

    Dim fichero As String
    Dim npart2bloq As Integer


    Private Function LeerBloque(ByVal fichero As String, ByVal nbloque As Integer) As String
        'aqui "fichero" es el pathfile del archivo que queremos leer. ORIGEN
        'la función LeerDatos devuelve una String que escribiremos en el fichero de salida. DESTINO

        'los bloques leidos son almacenados en un StringBuilder
        Dim res As New System.Text.StringBuilder
        'abrimos el fichero para leer de él
        Dim rw As New System.IO.FileStream(fichero, IO.FileMode.Open, IO.FileAccess.Read)
        Dim j As Integer
        
        'para cada particula tendremos
        Dim latch_b(3) As Byte 'tiene parametros como la carga, nº de veces que ha interaccionado, la historia de la particula
        Dim energy_b(3) As Byte 'la energía en MeV
        Dim xx_b(3) As Byte 'coordenada x en cm
        Dim yy_b(3) As Byte 'coordenada y en cm
        Dim uu_b(3) As Byte 'coseno director en x
        Dim vv_b(3) As Byte 'coseno director en y
        Dim wt_b(3) As Byte 'el peso de la particula. El signo es el del coseno director de z
        'los datos convertidos al tipo correspondiente
        Dim latch(3) As Byte
        Dim energy As Single
        Dim xx As Single
        Dim yy As Single
        Dim zz As Single = 0.00000001
        Dim uu As Single
        Dim vv As Single
        Dim ww As Single
        Dim wt As Single
        Dim KPAR As Integer
        Dim random As New Random
        Dim DeltaN As Integer

        Dim energyStr As String
        Dim xxStr As String
        Dim yyStr As String
        Dim zzStr As String
        Dim uuStr As String
        Dim vvStr As String
        Dim wwStr As String
        Dim wtStr As String
        Dim DeltaNStr As String

        Dim bit(7) As Byte
        Dim valor As Long
        Dim valor_aux As Long
        Dim i As Integer

        'me situo en la posición del fichero correspondiente al bloque
        rw.Seek(28 + 28 * Me.npart2bloq * (nbloque - 1), IO.SeekOrigin.Begin)
        'escribo 100 partículas (cada particula son 28 bytes)
        For j = 0 To Me.npart2bloq - 1
            rw.Read(latch_b, 0, 4)
            rw.Read(energy_b, 0, 4)
            rw.Read(xx_b, 0, 4)
            rw.Read(yy_b, 0, 4)
            rw.Read(uu_b, 0, 4)
            rw.Read(vv_b, 0, 4)
            rw.Read(wt_b, 0, 4)

            valor = CLng(latch_b(3))
            valor_aux = valor
            For i = 7 To 0 Step -1
                If (2 ^ i) <= valor_aux Then
                    valor_aux = valor_aux - (2 ^ i)
                    bit(i) = 1
                Else
                    bit(i) = 0
                End If
            Next
            'If valor <> 0 Then
            ' MsgBox("valor = " & valor & " bit29 = " & bit(5) & " bit30 = " & bit(6))
            'End If
            If valor = 0 Then 'el bit29 es cero y el bit 30 => tengo un fotón KPAR = 2
                KPAR = 2
            ElseIf valor = 64 Then 'el bit30 es uno => tengo un electron KPAR = 1
                KPAR = 1
                'MsgBox("valor = " & valor)
            ElseIf valor = 32 Then 'el bit29 es uno => tengo un positron KPAR = 3
                KPAR = 3
                'MsgBox("valor = " & valor)
            End If

            'miro los bits 29 (bit(5)) y 30 (bit(6))
            'If bit(5) = 0 & bit(6) = 0 Then 'el bit29 es cero y el bit 30 => tengo un fotón KPAR = 2
            'KPAR = 2
            'ElseIf bit(6) = 1 & bit(5) = 0 Then 'el bit30 es uno => tengo un electron KPAR = 1
            'KPAR = 1
            ''MsgBox("valor = " & valor)
            'ElseIf bit(5) = 1 & bit(6) = 0 Then 'el bit29 es uno => tengo un positron KPAR = 3
            'KPAR = 3
            ''MsgBox("valor = " & valor)
            'End If

            energy = Math.Abs(BitConverter.ToSingle(energy_b, 0)) * 1000000
            xx = BitConverter.ToSingle(xx_b, 0)
            yy = BitConverter.ToSingle(yy_b, 0)
            uu = BitConverter.ToSingle(uu_b, 0)
            vv = BitConverter.ToSingle(vv_b, 0)
            ww = Math.Sqrt(1 - uu * uu - vv * vv)
            'wt = BitConverter.ToSingle(wt_b, 0)
            'wt = 0.02
            wt = 1.0
            DeltaN = Math.Round(random.Next(0, 10))
            'escribo los parámetros según el formato psf
            energyStr = String.Format("{0:E5}", energy)
            xxStr = String.Format("{0:E5}", xx)
            yyStr = String.Format("{0:E5}", yy)
            zzStr = String.Format("{0:E5}", zz)
            uuStr = String.Format("{0:E5}", uu)
            vvStr = String.Format("{0:E5}", vv)
            wwStr = String.Format("{0:E5}", ww)
            wtStr = String.Format("{0:E5}", wt)

            res.Append(KPAR & "  ")
            res.Append(energyStr & " ")
            If xx > 0 Then
                res.Append(" " & xxStr & " ")
            ElseIf xx < 0 Then
                res.Append(xxStr & " ")
            End If
            If yy > 0 Then
                res.Append(" " & yyStr & "  ")
            ElseIf yy < 0 Then
                res.Append(yyStr & "  ")
            End If
            res.Append(zzStr & " ")
            If uu > 0 Then
                res.Append(" " & uuStr & " ")
            ElseIf uu < 0 Then
                res.Append(uuStr & " ")
            End If
            If vv > 0 Then
                res.Append(" " & vvStr & " ")
            ElseIf vv < 0 Then
                res.Append(vvStr & " ")
            End If
            'segun sea el signo de WEIGHT así será la dirección del coseno director ww:
            If wt > 0 Then
                res.Append("-" & wwStr & "  ")
            ElseIf wt < 0 Then
                res.Append(wwStr & "  ")
            End If
            res.Append(wtStr & " ")
            res.Append(DeltaN)
            res.Append(" 2 1 4 0 0" & vbCrLf)
        Next

        'cerramos el buffer
        rw.Close()

        'devolvemos todo lo leido
        Return res.ToString

    End Function
    Private Function LeerCabecera(ByVal fichero As String) As String
        'aqui "fichero" es el pathfile del archivo que queremos leer. ORIGEN
        'la función LeerDatos devuelve una String que escribiremos en el fichero de salida. DESTINO

        'los bloques leidos son almacenados en un StringBuilder
        Dim res As New System.Text.StringBuilder
        'abrimos el fichero para leer de él
        Dim rw As New System.IO.FileStream(fichero, IO.FileMode.Open, IO.FileAccess.Read)
        'Dim i As Integer

        res.Append("#[Transformando el fichero binario " & fichero & "]" & vbCrLf)
        res.Append("#Head MODE_rw : npphsp : nphotphsp : ekmaxphsp : ekminphspe : nincphsp " & vbCrLf)

        'los datos los leeré en bloques de 4bytes
        Dim modeRW_b(5) As Byte
        Dim npphsp_b(4) As Byte
        Dim nphotphsp_b(4) As Byte
        Dim ekmaxphsp_b(4) As Byte
        Dim ekminphspe_b(4) As Byte
        Dim nincphsp_b(4) As Byte
        Dim cc_b(3) As Byte

        'los datos convertidos al tipo correspondiente
        Dim modeRW As String
        Dim npphsp As Integer
        Dim nphotphsp As Integer
        Dim ekmaxphsp As Single
        Dim ekminphspe As Single
        Dim nincphsp As Integer
        'Dim cc As Byte
        Dim enc As New System.Text.UTF8Encoding()

        'leo la cabecera de datos binarios y los añado al FileStream rw
        rw.Read(modeRW_b, 0, 5)
        rw.Read(npphsp_b, 0, 4)
        rw.Read(nphotphsp_b, 0, 4)
        rw.Read(ekmaxphsp_b, 0, 4)
        rw.Read(ekminphspe_b, 0, 4)
        rw.Read(nincphsp_b, 0, 4)
        rw.Read(cc_b, 0, 3)

        'transformo los datos binarios en el tipo correspondiente
        modeRW = BitConverter.ToChar(modeRW_b, 0)
        npphsp = BitConverter.ToInt32(npphsp_b, 0)
        nphotphsp = BitConverter.ToInt32(nphotphsp_b, 0)
        ekmaxphsp = BitConverter.ToSingle(ekmaxphsp_b, 0)
        ekminphspe = BitConverter.ToSingle(ekminphspe_b, 0)
        nincphsp = BitConverter.ToSingle(nincphsp_b, 0)

        'añado los datos convertidos en el StringBuilder
        res.Append(enc.GetString(modeRW_b) & vbTab)
        res.Append(npphsp & vbTab)
        res.Append(nphotphsp & vbTab)
        res.Append(ekmaxphsp & vbTab)
        res.Append(ekminphspe & vbTab)
        res.Append(nincphsp & vbCrLf)
        res.Append(vbCrLf)

        'cerramos el buffer
        rw.Close()

        'devolvemos todo lo leido
        Return res.ToString
    End Function

    Private Function LeerDatos(ByVal fichero As String) As String
        'aqui "fichero" es el pathfile del archivo que queremos leer. ORIGEN
        'la función LeerDatos devuelve una String que escribiremos en el fichero de salida. DESTINO

        'los bloques leidos son almacenados en un StringBuilder
        Dim res As New System.Text.StringBuilder
        'abrimos el fichero para leer de él
        Dim rw As New System.IO.FileStream(fichero, IO.FileMode.Open, IO.FileAccess.Read)
        Dim i As Integer

        res.Append("#Transformando el fichero binario " & fichero & vbCrLf)
        res.Append("#Head MODE_rw : npphsp : nphotphsp : ekmaxphsp : ekminphspe : nincphsp " & vbCrLf)

        'los datos los leeré en bloques de 4bytes
        Dim modeRW_b(5) As Byte
        Dim npphsp_b(4) As Byte
        Dim nphotphsp_b(4) As Byte
        Dim ekmaxphsp_b(4) As Byte
        Dim ekminphspe_b(4) As Byte
        Dim nincphsp_b(4) As Byte
        Dim cc_b(3) As Byte

        'para cada particula tendremos
        Dim latch_b(4) As Byte 'tiene parametros como la carga, nº de veces que ha interaccionado, la historia de la particula
        Dim energy_b(4) As Byte 'la energía en MeV
        Dim xx_b(4) As Byte 'coordenada x en cm
        Dim yy_b(4) As Byte 'coordenada y en cm
        Dim uu_b(4) As Byte 'coseno director en x
        Dim vv_b(4) As Byte 'coseno director en y
        Dim wt_b(4) As Byte 'el peso de la particula. El signo es el del coseno director de z
        'Dim zlast_b(4) As Byte


        'los datos convertidos al tipo correspondiente
        Dim modeRW As String
        Dim npphsp As Integer
        Dim nphotphsp As Integer
        Dim ekmaxphsp As Single
        Dim ekminphspe As Single
        Dim nincphsp As Integer
        'Dim cc As Byte
        Dim enc As New System.Text.UTF8Encoding()
        Dim latch(4) As Byte
        Dim energy As Single
        Dim xx As Single
        Dim yy As Single
        Dim uu As Single
        Dim vv As Single
        Dim wt As Integer

        'leo la cabecera de datos binarios y los añado al FileStream rw
        rw.Read(modeRW_b, 0, 5)
        rw.Read(npphsp_b, 0, 4)
        rw.Read(nphotphsp_b, 0, 4)
        rw.Read(ekmaxphsp_b, 0, 4)
        rw.Read(ekminphspe_b, 0, 4)
        rw.Read(nincphsp_b, 0, 4)
        rw.Read(cc_b, 0, 3)

        'transformo los datos binarios en el tipo correspondiente
        modeRW = BitConverter.ToChar(modeRW_b, 0)
        npphsp = BitConverter.ToInt32(npphsp_b, 0)
        nphotphsp = BitConverter.ToInt32(nphotphsp_b, 0)
        ekmaxphsp = BitConverter.ToSingle(ekmaxphsp_b, 0)
        ekminphspe = BitConverter.ToSingle(ekminphspe_b, 0)
        nincphsp = BitConverter.ToSingle(nincphsp_b, 0)
        'MsgBox(modeRW)
        'MsgBox(npphsp)
        'MsgBox(nphotphsp)
        'MsgBox(ekmaxphsp)
        'MsgBox(ekminphspe)
        'MsgBox(nincphsp)

        'añado los datos convertidos en el StringBuilder
        res.Append(enc.GetString(modeRW_b) & vbTab)
        res.Append(npphsp & vbTab)
        res.Append(nphotphsp & vbTab)
        res.Append(ekmaxphsp & vbTab)
        res.Append(ekminphspe & vbTab)
        res.Append(nincphsp & vbCrLf)
        res.Append(vbCrLf)

        'escribo cada partícula
        res.Append("#resultados de cada partícula" & vbCrLf)
        For i = 0 To 100
            rw.Read(latch_b, 0, 4)
            rw.Read(energy_b, 0, 4)
            rw.Read(xx_b, 0, 4)
            rw.Read(yy_b, 0, 4)
            rw.Read(uu_b, 0, 4)
            rw.Read(vv_b, 0, 4)
            rw.Read(wt_b, 0, 4)

            energy = BitConverter.ToSingle(energy_b, 0)
            xx = BitConverter.ToSingle(xx_b, 0)
            yy = BitConverter.ToSingle(yy_b, 0)
            uu = BitConverter.ToSingle(uu_b, 0)
            vv = BitConverter.ToSingle(vv_b, 0)
            wt = BitConverter.ToSingle(wt_b, 0)

            res.Append(energy & vbTab)
            res.Append(xx & vbTab)
            res.Append(yy & vbTab)
            res.Append(uu & vbTab)
            res.Append(vv & vbTab)
            res.Append(wt & vbCrLf)
        Next

        'cerramos el buffer
        rw.Close()

        'devolvemos todo lo leido
        Return res.ToString

    End Function

    Private Sub GuardarDatos(ByVal fichero As String, ByVal cadena As String)
        'abrimos o creamos el fichero para escribir en él
        Dim sw As New System.IO.FileStream("C:\MCcourse\work1\temp.txt", IO.FileMode.OpenOrCreate)
        'la variable datos será un array de bytes
        Dim datos() As Byte
        Dim enc As New System.Text.UTF8Encoding

        'convertimos el texto en un array de bytes
        datos = enc.GetBytes(cadena)
        'y los escribimos en el stream
        sw.Write(datos, 0, datos.Length)
        'nos aseguramos de que se escriben todos los datos
        sw.Flush()
        'y cerramos el stream
        sw.Close()

    End Sub
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim s As String

        s = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyDecimalSeparator
        MsgBox("El separador decimal es: '" & s & "'")
        'Label1.Text = "El separador decimal es: '" & s & "'"

        s = System.Globalization.CultureInfo.CurrentCulture.NumberFormat.CurrencyGroupSeparator
        MsgBox("El separador de miles es: '" & s & "'")
        'Label2.Text = "El separador de miles es: '" & s & "'"


    End Sub

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Dim i As Integer
        Dim pathfile As String
        Dim destino_path As String
        Dim nbloq As Integer
        pathfile = TextBox1.Text
        destino_path = TextBox2.Text
        nbloq = TextBox3.Text

        ProgressBar1.Visible = True
        ProgressBar1.Minimum = 0
        ProgressBar1.Maximum = nbloq


        'defino el numero de particulas por bloque que vamos a escribir
        npart2bloq = 10000
     
        Dim sw As New System.IO.StreamWriter(destino_path, True)

        'sw.WriteLine(LeerCabecera(pathfile))

        'sw.WriteLine("leyendo particulas..." & vbCrLf)
        'la función WriteLine ya incluye el salto de linea al final
        sw.WriteLine("# [PHASE SPACE FILE FORMAT penEasy v.2008-05-15]")
        sw.WriteLine("# KPAR : E : X : Y : Z : U : V : W : WGHT : DeltaN : ILB(1..5)")

        For i = 1 To nbloq 'llegar hasta 340 para tener 3.400.000 particulas
            sw.Write(LeerBloque(pathfile, i))
            ProgressBar1.Value = i
        Next
        ProgressBar1.Visible = False
        sw.Close()

    End Sub
End Class
